# Agent Logging Hook

This hook automatically logs Copilot agent interactions for auditing and debugging purposes.

## Files

- **`github-os.json`** - Hook configuration (event listeners)
- **`log-interaction.ps1`** - PowerShell script that handles logging
- **`agent_log.txt`** - Output log file (auto-created on first use)

## How It Works

The hook intercepts these agent lifecycle events and logs them:

| Event | When Triggered | Logged Data |
|-------|----------------|------------|
| `SessionStart` | Agent session begins | Session start timestamp |
| `UserPromptSubmit` | User submits a prompt | User's input message |
| `PreToolUse` | Before tool execution | Tool name and parameters |
| `PostToolUse` | After tool execution | Tool name and success status |
| `Stop` | Agent session ends | Session end timestamp |

## Log Format

Each log entry includes:
- Timestamp (yyyy-MM-dd HH:mm:ss.fff)
- Event type (USER_PROMPT, PRE_TOOL_USE, POST_TOOL_USE, etc.)
- Relevant data for that event

**Example log output:**
```
[2026-04-01 14:35:22.123] [SESSION_START] New agent session started
[2026-04-01 14:35:25.456] [USER_PROMPT] Create a model for a vet clinic
[2026-04-01 14:35:26.789] [PRE_TOOL_USE] Tool: create_file | Input: {"filePath": "..."}
[2026-04-01 14:35:27.012] [POST_TOOL_USE] Tool: create_file | Success: true
[2026-04-01 14:40:15.345] [SESSION_END] Agent session ended
```

## Testing

1. **Start a new Copilot chat session** - This triggers `SessionStart`
2. **Ask Copilot a question** - This triggers `UserPromptSubmit`
3. **Watch Copilot use tools** - This triggers `PreToolUse` and `PostToolUse`
4. **End the session** - This triggers `Stop`
5. **Check `agent_log.txt`** - You should see entries for all triggered events

## Troubleshooting

**Log file not being created:**
- Verify PowerShell execution policy allows scripts
- Check that the path `C:\git\ASP.net-projekt\.github\hooks\agent_log.txt` is writable
- Review VS Code output console for hook errors

**Incomplete logs:**
- Some events may require specific agent actions (e.g., `PostToolUse` only logs when a tool succeeds)
- Check timeout settings (currently 5 seconds per hook)

## Configuration

To modify logging:
1. Edit `log-interaction.ps1` to change log format or add filters
2. Edit `github-os.json` to add/remove events or adjust timeouts
3. Reload VS Code or start a new chat session for changes to take effect
