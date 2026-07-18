# Ceylone Nature API

ASP.NET Core 8 Web API for the Ceylon Nature Store, built with Clean Architecture (Domain / Application / Infrastructure / Api), Entity Framework Core + PostgreSQL, ASP.NET Core Identity + JWT auth, and PayPal Orders API v2 (Sandbox) payments.

## Project layout

```
CeyloneNature.sln
src/
  CeyloneNature.Domain/          Entities only, no external dependencies
  CeyloneNature.Application/     DTOs, service interfaces, business logic (IApplicationDbContext abstraction)
  CeyloneNature.Infrastructure/  EF Core AppDbContext, Identity stores, TokenService, PayPalService, DB seeding
  CeyloneNature.Api/             Controllers, Program.cs composition root, appsettings
```

## Prerequisites

- .NET 8 SDK (pinned via `global.json`)
- PostgreSQL 13+ (local install, Docker, or a hosted instance)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## Local setup

1. Create a database (e.g. `ceylon-nature`).
2. Configure secrets (never commit real secrets to `appsettings.json`):
   ```bash
   cd src/CeyloneNature.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=ceylon-nature;Username=postgres;Password=YOUR_PASSWORD"
   dotnet user-secrets set "Jwt:Key" "<a long random string, 32+ chars>"
   dotnet user-secrets set "Seed:AdminPassword" "<password for the seeded admin account>"
   # Optional, needed for real PayPal checkout:
   dotnet user-secrets set "PayPal:ClientId" "<your PayPal Sandbox Client ID>"
   dotnet user-secrets set "PayPal:ClientSecret" "<your PayPal Sandbox Client Secret>"
   ```
3. Run the API (migrations and seed data apply automatically on startup):
   ```bash
   dotnet run --project src/CeyloneNature.Api/CeyloneNature.Api.csproj
   ```
4. Swagger UI is available at `/swagger` in Development.

The seeded admin account logs in at `admin@ceylonnature.com` with the password you set in `Seed:AdminPassword`.

### Adding a new migration

```bash
dotnet ef migrations add <Name> \
  --project src/CeyloneNature.Infrastructure/CeyloneNature.Infrastructure.csproj \
  --startup-project src/CeyloneNature.Api/CeyloneNature.Api.csproj \
  --output-dir Migrations
```

## Configuration reference

All values below can be set via `appsettings.json`, user-secrets (local dev), or environment variables using `__` as the section separator (e.g. `ConnectionStrings__DefaultConnection`) — the standard ASP.NET Core convention required by Render/Railway.

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Key` | Symmetric signing key for JWTs (32+ random chars) |
| `Jwt:Issuer` / `Jwt:Audience` | JWT issuer/audience claims |
| `PayPal:Mode` | `sandbox` or `live` |
| `PayPal:ClientId` / `PayPal:ClientSecret` | PayPal REST app credentials (developer.paypal.com → Apps & Credentials) |
| `Cors:AllowedOrigins` | Comma-separated list of allowed frontend origins |
| `Seed:AdminEmail` / `Seed:AdminPassword` | Credentials for the auto-seeded admin account |

## Deploying to Render or Railway

Both `render.yaml` and `railway.json` are provided, targeting the included `Dockerfile` (a multi-stage build that restores/publishes the whole solution then runs `CeyloneNature.Api.dll`).

### Database: card-free option (recommended)

Render's own bundled free Postgres (via its Blueprint flow, or `render.yaml`'s commented-out `databases` block) currently requires a payment method on file for account verification, even though the tier itself is free. To avoid that, provision Postgres externally instead — both are genuinely free with no card required:

- **[Neon](https://neon.tech)** — recommended; free tier auto-resumes on the next query after idling, so a quiet admin dashboard doesn't need a manual restart.
- **[Supabase](https://supabase.com)** — also free, but pauses inactive free projects after ~1 week and requires a manual dashboard click to resume.

Either gives you a `postgres://user:pass@host/db` connection string — paste it directly into `ConnectionStrings__DefaultConnection`; the API normalizes that URI format to what Npgsql expects automatically (see `CeyloneNature.Infrastructure/DependencyInjection.cs`).

### Steps

1. On Render: **New +** → **Web Service** (not Blueprint, to skip the bundled-Postgres card prompt) → connect the repo → it detects the `Dockerfile` automatically. On Railway: **New Project** → **Deploy from GitHub repo**.
2. Provision Postgres externally (Neon/Supabase, see above) and copy its connection string.
3. Set the environment variables listed above in the platform's dashboard (Render: service → Environment; Railway: service → Variables), using the double-underscore form, e.g. `ConnectionStrings__DefaultConnection`.
4. Set `Cors__AllowedOrigins` to your deployed Cloudflare Pages URL (e.g. `https://ceylon-nature-store.pages.dev`).
5. Deploy — on startup the app runs `Database.MigrateAsync()` and seeds roles/admin/catalog data automatically, so no manual migration step is needed against the production database.

The API listens on port `8080` inside the container (`ASPNETCORE_URLS`), which both Render and Railway detect automatically.
