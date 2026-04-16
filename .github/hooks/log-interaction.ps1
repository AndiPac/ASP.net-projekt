param($payload)

$logPath = "C:\git\.github\hooks\agent_log.txt"

if ($payload) {
    # Only process if it looks like JSON
    if ($payload -like '*{*') {
        $payload | ConvertFrom-Json | ConvertTo-Json -Depth 10 | Add-Content -Path $logPath
        Add-Content -Path $logPath -Value "`n"
    }
}
elseif ($input) {
    $rawInput = $input | Out-String
    if ($rawInput -like '*{*') {
        $rawInput | ConvertFrom-Json | ConvertTo-Json -Depth 10 | Add-Content -Path $logPath
        Add-Content -Path $logPath -Value "`n"
    }
}