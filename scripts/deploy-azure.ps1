param(
    [Parameter(Mandatory = $true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory = $true)]
    [string]$Location,

    [Parameter(Mandatory = $true)]
    [string]$AppServicePlan,

    [Parameter(Mandatory = $true)]
    [string]$WebAppName,

    [Parameter(Mandatory = $true)]
    [string]$SqlServerName,

    [Parameter(Mandatory = $true)]
    [string]$SqlDatabaseName,

    [Parameter(Mandatory = $true)]
    [string]$SqlAdminUser,

    [Parameter(Mandatory = $true)]
    [securestring]$SqlAdminPassword,

    [Parameter(Mandatory = $true)]
    [securestring]$OpenAIApiKey,

    [string]$OpenAIModel = "gpt-4o-mini",
    [string]$GoogleClientId = "",
    [string]$GoogleClientSecret = "",
    [string]$Sku = "B1",
    [switch]$AllowCurrentIp,
    [switch]$RunMigrations,
    [switch]$SkipAppDeploy
)

$ErrorActionPreference = "Stop"

$defaultAzCmdPath = "C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin\az.cmd"
$azCommand = Get-Command az -ErrorAction SilentlyContinue
if ($azCommand) {
    $script:AzExe = $azCommand.Source
}
elseif (Test-Path $defaultAzCmdPath) {
    $script:AzExe = $defaultAzCmdPath
}
else {
    throw "Azure CLI was not found in PATH and not found at '$defaultAzCmdPath'. Install it first: https://learn.microsoft.com/cli/azure/install-azure-cli"
}

function Invoke-Az {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Args)
    $output = & $script:AzExe @Args 2>&1
    if ($LASTEXITCODE -ne 0) {
        $details = ($output | Out-String).Trim()
        if ([string]::IsNullOrWhiteSpace($details)) {
            $details = "(no stderr/stdout captured)"
        }

        throw "Azure CLI command failed (exit code $LASTEXITCODE): az $($Args -join ' ')`n$details"
    }

    return $output
}

function Ensure-AzProviderRegistered {
    param([Parameter(Mandatory = $true)][string]$Namespace)

    $state = (Invoke-Az provider show --namespace $Namespace --query registrationState --output tsv | Out-String).Trim()
    if ($state -eq "Registered") {
        return
    }

    Write-Host "Registering Azure resource provider '$Namespace' (current state: $state)..."
    Invoke-Az provider register --namespace $Namespace | Out-Null

    $newState = (Invoke-Az provider show --namespace $Namespace --query registrationState --output tsv | Out-String).Trim()
    if ($newState -ne "Registered") {
        throw "Azure resource provider '$Namespace' is currently '$newState'. Wait a minute, then rerun the deploy script."
    }
}


function ConvertTo-PlainText {
    param([Parameter(Mandatory = $true)][securestring]$Value)
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try {
        return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

function Test-SqlAdminPasswordPolicy {
    param(
        [Parameter(Mandatory = $true)][string]$Password,
        [Parameter(Mandatory = $true)][string]$UserName
    )

    if ($Password.Length -lt 8) {
        throw "SQL admin password must be at least 8 characters."
    }

    if ($Password.Length -gt 128) {
        throw "SQL admin password must be 128 characters or fewer."
    }

    if ($Password -cmatch [Regex]::Escape($UserName)) {
        throw "SQL admin password must not contain the SQL admin username."
    }

    $hasUpper = $Password -cmatch "[A-Z]"
    $hasLower = $Password -cmatch "[a-z]"
    $hasDigit = $Password -match "[0-9]"
    $hasSpecial = $Password -match "[^a-zA-Z0-9]"

    if (-not ($hasUpper -and $hasLower -and $hasDigit -and $hasSpecial)) {
        throw "SQL admin password must include uppercase, lowercase, number, and special character."
    }
}

$sqlAdminPasswordPlain = ConvertTo-PlainText -Value $SqlAdminPassword
$openAiApiKeyPlain = ConvertTo-PlainText -Value $OpenAIApiKey
Test-SqlAdminPasswordPolicy -Password $sqlAdminPasswordPlain -UserName $SqlAdminUser

Write-Host "Ensuring Azure login and subscription context..."
Invoke-Az account show | Out-Null

Ensure-AzProviderRegistered -Namespace "Microsoft.Sql"
Ensure-AzProviderRegistered -Namespace "Microsoft.Web"

$resourceGroupExists = (& $script:AzExe group exists --name $ResourceGroup).Trim()
if ($LASTEXITCODE -ne 0) {
    throw "Azure CLI command failed (exit code $LASTEXITCODE): az group exists --name $ResourceGroup"
}

if ($resourceGroupExists -eq "true") {
    $existingLocation = (& $script:AzExe group show --name $ResourceGroup --query location -o tsv).Trim()
    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI command failed (exit code $LASTEXITCODE): az group show --name $ResourceGroup --query location -o tsv"
    }

    Write-Host "Resource group '$ResourceGroup' already exists in '$existingLocation'. Reusing existing resource group."
}
else {
    Write-Host "Creating resource group..."
    Invoke-Az group create --name $ResourceGroup --location $Location | Out-Null
}

Write-Host "Creating SQL server and database..."
Invoke-Az sql server create `
    --name $SqlServerName `
    --resource-group $ResourceGroup `
    --location $Location `
    --admin-user $SqlAdminUser `
    --admin-password $sqlAdminPasswordPlain | Out-Null

Invoke-Az sql db create `
    --resource-group $ResourceGroup `
    --server $SqlServerName `
    --name $SqlDatabaseName `
    --service-objective S0 | Out-Null

Write-Host "Allowing Azure services to access SQL server..."
Invoke-Az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServerName `
    --name AllowAzureServices `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0 | Out-Null

if ($AllowCurrentIp) {
    $myIp = (Invoke-RestMethod -Uri "https://api.ipify.org").ToString()
    Write-Host "Allowing current public IP on SQL firewall: $myIp"
    Invoke-Az sql server firewall-rule create `
        --resource-group $ResourceGroup `
        --server $SqlServerName `
        --name AllowCurrentIp `
        --start-ip-address $myIp `
        --end-ip-address $myIp | Out-Null
}

Write-Host "Creating App Service plan and Web App..."
Invoke-Az appservice plan create `
    --name $AppServicePlan `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku $Sku `
    --is-linux | Out-Null

Invoke-Az webapp create `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --name $WebAppName `
    --runtime "DOTNETCORE:8.0" | Out-Null

$connectionString = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=$SqlDatabaseName;Persist Security Info=False;User ID=$SqlAdminUser;Password=$sqlAdminPasswordPlain;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

Write-Host "Applying app settings and connection string..."
Invoke-Az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $WebAppName `
    --settings `
        ASPNETCORE_ENVIRONMENT=Production `
        ConnectionStrings__DefaultConnection="$connectionString" `
        OpenAI__ApiKey="$openAiApiKeyPlain" `
        OpenAI__Model="$OpenAIModel" `
        Authentication__Google__ClientId="$GoogleClientId" `
        Authentication__Google__ClientSecret="$GoogleClientSecret" | Out-Null

if ($RunMigrations) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "dotnet SDK is not installed. Install .NET 8 SDK to run migrations."
    }

    Write-Host "Running EF Core migrations against Azure SQL..."
    dotnet tool install --global dotnet-ef --version 8.* | Out-Null
    $env:Path = "$env:USERPROFILE\.dotnet\tools;$env:Path"

    $repoRoot = Split-Path -Parent $PSScriptRoot
    Push-Location $repoRoot
    try {
        $env:ConnectionStrings__DefaultConnection = $connectionString
        dotnet ef database update `
            --project VetAmb/VetAmb.csproj `
            --startup-project VetAmb/VetAmb.csproj `
            --context VetAmbDbContext
    }
    finally {
        Pop-Location
    }
}

if (-not $SkipAppDeploy) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "dotnet SDK is not installed. Install .NET 8 SDK to publish the app."
    }

    Write-Host "Publishing and deploying app package..."
    $repoRoot = Split-Path -Parent $PSScriptRoot
    $publishDir = Join-Path $repoRoot "artifacts\vetamb-publish"
    $zipPath = Join-Path $repoRoot "artifacts\vetamb-publish.zip"

    if (Test-Path $publishDir) {
        Remove-Item -Recurse -Force $publishDir
    }

    if (Test-Path $zipPath) {
        Remove-Item -Force $zipPath
    }

    Push-Location $repoRoot
    try {
        dotnet publish "VetAmb/VetAmb.csproj" -c Release -o $publishDir | Out-Null
        Compress-Archive -Path "$publishDir\*" -DestinationPath $zipPath -Force
        Invoke-Az webapp deploy `
            --resource-group $ResourceGroup `
            --name $WebAppName `
            --src-path $zipPath `
            --type zip `
            --clean true `
            --track-status false | Out-Null
    }
    finally {
        Pop-Location
    }
}

Write-Host "Done."
Write-Host "Web App URL: https://$WebAppName.azurewebsites.net"
Write-Host "Next: configure GitHub OIDC secrets and push to main to enable CI/CD deployment."
