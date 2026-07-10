# ASP.net-projekt

## Run with Docker

This app targets .NET 8 and uses SQL Server. The default local connection string in [appsettings.json](appsettings.json) points to LocalDB, which does not exist inside containers, so Docker runs should use the compose stack from the repository root.

The repository includes two Docker entry points:

- `docker-compose.yml`: local development and laptop transfer
- `docker-compose.prod.yml`: production-oriented override with stricter defaults

### Prerequisites

- Docker Desktop installed on the machine
- Ports 8080 and 1433 available

### Start the app on another machine

From the repository root:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

The app will be available at `http://localhost:8080`.

### Start with production-oriented settings

From the repository root:

```powershell
Copy-Item .env.example .env
docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d
```

In this mode:

- the app runs as `Production`
- the app is published on port `80`
- the SQL Server container is not exposed to the host
- both containers restart automatically unless stopped manually

### Secrets and configuration

Set these environment variables before starting the stack if you need the related features:

- `SA_PASSWORD`: SQL Server `sa` password for the containerized database
- `OPENAI_API_KEY`: enables the AI chat endpoint
- `GOOGLE_CLIENT_ID`
- `GOOGLE_CLIENT_SECRET`
- `SEED_DEFAULT_USERS`: when `true`, creates the default admin and vet accounts on startup

You can either edit `.env` after copying `.env.example`, or set them directly in PowerShell.

Example in PowerShell:

```powershell
$env:SA_PASSWORD = "Your_strong_password_123!"
$env:OPENAI_API_KEY = "replace-me"
$env:GOOGLE_CLIENT_ID = "replace-me-if-needed"
$env:GOOGLE_CLIENT_SECRET = "replace-me-if-needed"
$env:SEED_DEFAULT_USERS = "true"
docker compose up --build
```

### Notes

- SQL data is persisted in the Docker volume `vetamb-sqldata`.
- Serilog file logs are written to `artifacts/docker-logs` on the host.
- Docker startup enables `Database__AutoMigrateOnStartup=true`, so the app applies pending EF Core migrations automatically inside the container.
- The default laptop/dev Docker stack enables `SEED_DEFAULT_USERS=true`, which creates `admin@vetamb.com` / `AdminPassword123!` and `vet@vetamb.com` / `VetPassword123!` if they do not already exist.
- Outside Docker, startup migrations remain off by default.
- If you use the production override on a laptop and port `80` is unavailable, change the published port in [docker-compose.prod.yml](docker-compose.prod.yml).