param(
    [Parameter(Mandatory=$true)]
    [string]$Message
)

$payload = @{ hookEventName = 'AgentResponse'; agentMessage = $Message } | ConvertTo-Json

# Send to existing hook handler
$payload | .\log-interaction.ps1

Write-Output "Sent AgentResponse: $Message"
