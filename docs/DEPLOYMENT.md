# Deploying the NSBM Industry Collaboration Unit platform

This guide takes the platform from the repository to a running server. Development setup is in
[ARCHITECTURE.md](ARCHITECTURE.md) §8.

## 1. What runs where

| Piece | Image / runtime | Notes |
|---|---|---|
| Database | SQL Server 2022 (container, or an existing server) | Migrations are applied by the API on start-up. |
| API | `deploy/api/Dockerfile` (.NET 8, port 8080, unprivileged user) | Stateless; can run more than one copy behind a load balancer. |
| Web | `deploy/web/Dockerfile` (static build served by nginx, port 80) | nginx also forwards `/api` to the API, so the browser sees one origin. |

Put HTTPS in front of the web container (a load balancer, a reverse proxy such as Caddy or IIS ARR, or the
university's gateway). The containers speak plain HTTP inside the private network.

## 2. Settings

Every secret comes from the environment, never from a committed file. The API refuses to start, with a list of
what to fix, if the JWT secret is short or is the development key, the connection string is missing, or no web
origin is configured.

| Setting (environment variable) | Required | Meaning |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | yes | SQL Server connection string. |
| `Jwt__Secret` | yes | At least 32 random characters. Changing it signs everyone out. |
| `AllowedOrigins__0`, `__1`… | one of these two | The web app's address(es), when it is on a different origin from the API. |
| `ServeWebOnSameOrigin=true` | | When the web app proxies `/api` (the Docker setup). |
| `ForwardedHeaders__Enabled`, `__KnownProxies__0` / `__KnownNetworks__0` | behind a proxy | Trust `X-Forwarded-For` only from your proxy, so the public-form rate limit counts real visitors. |
| `Bootstrap__AdminEmail`, `__AdminPassword`, `__AdminFirstName`, `__AdminLastName` | first start only | Creates the first Unit admin when the database has none. Remove them afterwards. |
| `Demo__Enabled` | no (default `false`) | `true` loads fictional demo people and records, demo sign-in and demo reset. Never on a real deployment. |
| `Anthropic__ApiKey` | no | Turns on the AI interviewer. Without it the feature says it is unavailable. |

The web build reads `VITE_DEMO_MODE` (default `false` for production builds) and `VITE_FACE_MODEL_URL`
(optional self-hosted camera-coaching model).

## 3. One server with Docker Compose

```powershell
Copy-Item .env.example .env      # then fill in every value
docker compose up -d --build
docker compose logs api          # look for "Created the first Unit admin"
```

Then:

1. Open `http://<server>:8080` (or your HTTPS address) and sign in as the bootstrap admin.
2. Remove the `BOOTSTRAP_ADMIN_*` values from `.env` and run `docker compose up -d` again.
3. Change the admin's password from the user menu.
4. From **Users**, create the Assistant Registrar, lecturers and partner contacts, and import each student
   cohort from CSV. The registrar adds the real lecture halls and event halls under **Venues**.

The database lives in the `db-data` volume. Back it up (section 6) before upgrades.

## 4. Without Docker (Windows Server / IIS or any host)

```powershell
dotnet publish src/NSBM.ICU.API -c Release -o publish/api
cd web; npm ci; npm run build      # static files in web/dist
```

- Host `publish/api` as an ASP.NET Core 8 app (IIS with the Hosting Bundle, or a service) and set the settings
  from section 2 as environment variables or in IIS configuration (not in `appsettings.json`).
- Serve `web/dist` from any static host. Either proxy `/api` and `/health` to the API (same origin, set
  `ServeWebOnSameOrigin=true`) or build with `VITE_API_URL=https://api.example` and list the web address in
  `AllowedOrigins`. Every unknown path must return `index.html` (single-page app).

## 5. Health and monitoring

| Endpoint | Healthy when |
|---|---|
| `/health/live` | the API process answers |
| `/health/ready` | it can also reach the database (use this for load-balancer checks) |

Logs go to standard output. The **Audit log** page records who approved, allocated, reset or changed what.

## 6. Backups and upgrades

- Back up the database daily, for example `BACKUP DATABASE NSBM_ICU TO DISK = '/var/opt/mssql/backup/icu.bak'`
  run through `docker compose exec db /opt/mssql-tools18/bin/sqlcmd ...`, and copy the file off the server.
- To upgrade: back up, `git pull`, `docker compose up -d --build`. The API applies new migrations on start-up.

## 7. Security checklist before go-live

- [ ] `Demo__Enabled` is `false` and the web image was built without `VITE_DEMO_MODE=true`.
- [ ] `Jwt__Secret` and the database password are new random values, kept only in the server's secret store or `.env`.
- [ ] The database is not reachable from the internet (the compose file publishes only the web port).
- [ ] HTTPS is in front of the web container and HTTP redirects to it.
- [ ] Bootstrap settings removed after the first admin signed in; that admin changed their password.
- [ ] Backups are scheduled and a restore has been tried once.

> **Note on history:** early commits of this repository contained a local development database password and a
> development signing key. Neither may be used anywhere else: use new values for every real environment, and
> change the local SQL Server `sa` password if it is used beyond a developer laptop.
