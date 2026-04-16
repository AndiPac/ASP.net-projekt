# Copilot Instructions

## Interaction Logging
- **Automated Hooks:** Logging is handled automatically by the system via `github-os.json` and the `.github/hooks/log-interaction.ps1` script.
- **No Manual Logging:** Do NOT manually append text, use `Add-Content`, or run terminal commands to write to `.github/hooks/agent_log.txt`. 
- **Data Capture:** The system automatically captures `UserPromptSubmit` and `PreToolUse` events. You do not need to summarize your actions or the user's prompt into the log file manually.
- **Focus:** Provide direct assistance with the codebase. Trust the background hooks to record the session data.

## Project Context
- This is an ASP.NET project located in `C:\git\`.
- Administrative scripts and hooks are located in `C:\git\.github\hooks\`.