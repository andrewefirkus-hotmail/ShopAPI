# SimpleApi (ShopAPI)

A playground solution for trying out architecture ideas around a small "shop" domain (Users, Products, Orders).
One ASP.NET Core Web API backed by SQL Server (EF Core code-first), with different front ends consuming it.

## Projects

| Project | Type | Purpose |
|---|---|---|
| `ShopAPI` | ASP.NET Core Web API (.NET 10) | REST API with Swagger, Serilog, EF Core. Controllers → Services → DAOs. Applies migrations on startup. |
| `DataAccess` | Class library | `ShopDbContext`, entities (`User`, `Product`, `Order`), EF migrations incl. seed data. |
| `ShopShared` | Class library | DTOs shared by the Blazor front ends. |
| `ShopBlazorServer` | Blazor Server (.NET 10) | Interactive-server UI calling the API via a named `HttpClient`. |
| `ShopBlazorWasm` | Blazor WebAssembly (.NET 10) | Same UI running in the browser; served by nginx in Docker. |
| `ShopAngular` | Angular 21 (+ small .NET launcher) | Same UI in Angular. `ShopAngular.csproj` just runs `npm start` so it can be launched from Visual Studio. |

## API endpoints

Each of `users`, `products`, `orders` exposes the same CRUD shape:

```
GET    /api/{resource}
GET    /api/{resource}/{id:guid}
POST   /api/{resource}
PUT    /api/{resource}/{id:guid}
DELETE /api/{resource}/{id:guid}
```

Swagger UI: `/swagger`. Sample requests: `ShopAPI/ShopAPI.http`.

## Prerequisites

- .NET 10 SDK
- Docker Desktop (SQL Server 2022 + the full stack)
- Node.js + npm (for Angular; Angular CLI is pulled in via `npm install`)
- Optional: `dotnet-ef` tool for creating migrations (`dotnet tool install --global dotnet-ef`)

## Run everything with Docker

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| ShopAPI (Swagger) | http://localhost:8080/swagger |
| Blazor Server | http://localhost:8081 |
| Blazor WASM | http://localhost:8082 |
| Angular | http://localhost:8083 |
| SQL Server | `localhost,1433` (user `sa`) |

The API waits for SQL Server's healthcheck, then runs migrations (schema + seed data) on startup.

## Run locally (debugging)

1. Start only the database:
   ```bash
   docker compose up -d sqlserver
   ```
2. Run the API (Development settings point at `localhost,1433`):
   ```bash
   dotnet run --project ShopAPI --launch-profile https
   ```
   → https://localhost:7025/swagger (http: 5043)
3. Run a front end:
   - Blazor Server: `dotnet run --project ShopBlazorServer` (dev `ApiBaseUrl` = `http://localhost:8080`)
   - Blazor WASM: `dotnet run --project ShopBlazorWasm` (`wwwroot/appsettings.json` → `http://localhost:8080`)
   - Angular: `cd ShopAngular && npm install && npm start` → https://localhost:4200 (dev `apiUrl` = `https://localhost:7025`)

> Note: the Blazor apps expect the API on port **8080** (the Docker port), while Angular dev expects **7025** (the `dotnet run` port).
> When mixing local + Docker, either keep the API container running or adjust `ApiBaseUrl` / `environment.development.ts`.

## Database / migrations

- Migrations live in `DataAccess/Migrations` and are applied automatically when the API starts.
- Two connection strings: `DefaultConnection` (runtime) and `MigrationConnection` (DDL). Both currently use `sa`.
- Add a migration:
  ```bash
  dotnet ef migrations add <Name> --project DataAccess --startup-project ShopAPI
  ```

## Known TODOs / not production-ready

- `sa` password is hard-coded in `docker-compose.yml` and `appsettings*.json` — dev only.
- Swap to separate restricted (runtime) and privileged (migration) SQL logins.
- CORS uses `AllowAnyOrigin` — lock down to the real front-end origins.
- No authentication/authorization.
- Template leftovers (`Counter`, `Weather` pages) still in the Blazor projects.
