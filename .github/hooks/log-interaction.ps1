# Log interaction hook for Copilot agent auditing and debugging
# Reads JSON from stdin and appends formatted log entry to agent_log.txt

$ErrorActionPreference = "Continue"

# Define log file path dynamically based on script location
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$logFile = Join-Path $scriptDir "agent_log.txt"
$debugFile = Join-Path $scriptDir "hook_debug.txt"

# Create timestamp
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"

# Ensure log file exists
if (-not (Test-Path $logFile)) {
    New-Item -Path $logFile -ItemType File -Force | Out-Null
}

try {
    # Read input from stdin
    $inputText = $input | Out-String
    
    # Try to parse as JSON
    if ($inputText.Trim()) {
        $jsonInput = $inputText.Trim() | ConvertFrom-Json -ErrorAction SilentlyContinue
        $eventType = if ($jsonInput -and $jsonInput.hookEventName) { $jsonInput.hookEventName } else { "Unknown" }
    } else {
        $eventType = "NoInput"
        $jsonInput = $null
    }
    
    # Debug: log raw incoming JSON at each invocation
    $debugLog = "[$timestamp] [DEBUG] event=$eventType, raw=$inputText"
    Add-Content -Path $debugFile -Value $debugLog -ErrorAction SilentlyContinue

    # Format log entry based on event type
    $logEntry = ""
    
    switch ($eventType) {
        "UserPromptSubmit" {
            $userPrompt = if ($jsonInput.userMessage) { $jsonInput.userMessage } elseif ($jsonInput.prompt) { $jsonInput.prompt } else { "N/A" }
            $logEntry = "[$timestamp] [USER_PROMPT] $userPrompt"
        }
        "AgentResponse" {
            $agentResponse = if ($jsonInput.agentMessage) { $jsonInput.agentMessage } elseif ($jsonInput.response) { $jsonInput.response } else { "N/A" }
            $logEntry = "[$timestamp] [AGENT_RESPONSE] $agentResponse"
        }
        "PreToolUse" {
            $toolName = if ($jsonInput.toolName) { $jsonInput.toolName } else { "Unknown" }
            if ($jsonInput.toolInput) {
                $toolInput = $jsonInput.toolInput | ConvertTo-Json -Compress
            } else {
                $toolInput = "N/A"
            }
            $logEntry = "[$timestamp] [PRE_TOOL_USE] Tool: $toolName | Input: $toolInput"
        }
        "PostToolUse" {
            $toolName = if ($jsonInput.toolName) { $jsonInput.toolName } else { "Unknown" }
            $success = if ($jsonInput.success) { $jsonInput.success } else { "unknown" }
            $logEntry = "[$timestamp] [POST_TOOL_USE] Tool: $toolName | Success: $success"
        }
        "SessionStart" {
            $logEntry = "[$timestamp] [SESSION_START] New agent session started"
        }
        "Stop" {
            $logEntry = "[$timestamp] [SESSION_END] Agent session ended"
        }
        "NoInput" {
            $logEntry = "[$timestamp] [HOOK_TRIGGERED] Hook executed (no input data)"
        }
        default {
            if ($jsonInput) {
                $jsonStr = $jsonInput | ConvertTo-Json -Compress
            } else {
                $jsonStr = $inputText
            }
            $logEntry = "[$timestamp] [$eventType] $jsonStr"
        }
    }
    
    # Append log entry to file
    Add-Content -Path $logFile -Value $logEntry -ErrorAction Stop
    
    # Return success response
    @{
        "continue" = $true
        "systemMessage" = "Logged: $eventType"
    } | ConvertTo-Json -Compress
    
    exit 0
}
catch {
    $errorMsg = $_.Exception.Message
    
    # Try to log the error
    try {
        Add-Content -Path $debugFile -Value "[$timestamp] ERROR: $errorMsg | Input: $inputText"
    } catch {
        # Silently fail if debug logging fails
    }
    
    # Return error response but don't block
    @{
        "continue" = $true
        "systemMessage" = "Hook warning: $_"
    } | ConvertTo-Json -Compress
    
    exit 1
}
