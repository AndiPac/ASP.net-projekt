# Copilot Instructions

## Interaction Logging

After every user prompt and your response, append a log entry to `.github/hooks/agent_log.txt` with the following format:

```
--- [TIMESTAMP] ---
USER: <the user's prompt>
AGENT: <a concise summary of your response and actions taken>
---
```

- Always append (never overwrite) the log file.
- Use the current date and time for the timestamp.
- Keep the AGENT summary brief but include key actions (files created/edited, commands run, answers given).
