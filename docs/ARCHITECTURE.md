# NSBM Industry Collaboration Unit — Architecture Blueprint

> Platform for NSBM Green University's Industry Collaboration Unit (Faculty of Computing).
> Connects **Students**, **Lecturers**, **Industry partners** and **Admins** around career
> pathways, industry tasks, interview practice, CVs and internships.
>
> Unit lead: **Ms. Pavithra Subhashini**, Senior Lecturer, Department of Software Engineering
> and Computer Security, Faculty of Computing.

This document is the **rulebook** for the codebase. Every change must follow it.

---

## 1. Stack

| Layer      | Technology                                                        |
|------------|-------------------------------------------------------------------|
| API        | ASP.NET Core 8, MediatR (CQRS), FluentValidation, JWT bearer auth |
| Data       | EF Core 8 + SQL Server (`.\SQLEXPRESS`, database `NSBM_ICU`)      |
| Identity   | ASP.NET Core Identity, roles: `Student`, `Lecturer`, `Industry`, `Admin` |
| Web        | React 19, Vite, TypeScript, Tailwind CSS, TanStack Query, Zustand, React Router |

| Service | URL                     |
|---------|-------------------------|
| API     | http://localhost:5300   |
| Web     | http://localhost:5301   |

---

## 2. Solution layout

```
Code/
├─ NSBM.ICU.slnx
├─ start.bat                        one-click: migrate DB, start API + web
├─ docs/                            architecture + guides
├─ src/
│  ├─ NSBM.ICU.Domain/              entities, enums, constants — no dependencies
│  ├─ NSBM.ICU.Application/         use cases (CQRS), DTOs, validation, interfaces
│  ├─ NSBM.ICU.Infrastructure/      EF Core, Identity, JWT, seeding, external services
│  └─ NSBM.ICU.API/                 HTTP surface: controllers, middleware, DI wiring
└─ web/                             React client
```

### Dependency rule (never broken)

```
API ──► Application ──► Domain
 └────► Infrastructure ──► Application
```

* **Domain** knows nothing about other layers.
* **Application** depends only on Domain and on *interfaces it owns*
  (`IApplicationDbContext`, `ICurrentUser`, `ITokenService`, `IIdentityService`, `IDateTime`).
* **Infrastructure** implements those interfaces.
* **API** is thin: it maps HTTP ⇄ MediatR requests. No business logic in controllers.

---

## 3. Backend conventions

### 3.1 Domain (`NSBM.ICU.Domain`)

```
Common/          BaseEntity, AuditableEntity
Constants/       Roles (string constants used by [Authorize] and seeding)
Enums/           one enum per file (persisted and serialized as strings)
Rules/           pure, stateless business rules (e.g. SkillSignalRules)
Entities/<Area>/ one entity per file, grouped by business area
```

* Entities expose `Id` and audit fields from the base classes.
* State changes with a rule attached go through an entity method (e.g. `Student.ChooseCareerPath`).
* Partner-authored, moderated content (`IndustryTask`, `Vacancy`) derives from `PartnerContent`, which owns
  the moderation lifecycle (Pending → Approved/Rejected, resubmit on edit, `CanBeEdited`, `CanBeReviewed`).
* Enums are persisted as strings (configured in Infrastructure).

### 3.2 Application (`NSBM.ICU.Application`)

```
Common/
  Behaviours/    ValidationBehaviour (MediatR pipeline)
  Exceptions/    NotFoundException, ForbiddenAccessException, ValidationException, BusinessRuleException
  Evidence/      the skill-evidence engine (listed vs practice signal vs missing) shared by readiness,
                 the match center and candidate review
  Extensions/    profile resolution (GetCurrentStudent/Partner/AdministratorAsync, EnsureCompanyAccessAsync)
  Moderation/    IReviewCommand, ReviewCommandValidator<T>, ApplyReview / EnsureEditableBy — shared by
                 every review and partner-edit handler
  Interfaces/    IApplicationDbContext, ICurrentUser, ITokenService, IIdentityService, IDateTime, IDemoDataService
  Models/        cross-feature models (AuthenticatedUser, AccessToken, later PagedResult<T>)
Features/<Feature>/
  Commands/<UseCase>/   <UseCase>Command.cs  (+ handler + validator in the same file)
  Queries/<UseCase>/    <UseCase>Query.cs    (+ handler in the same file)
  Dtos/                 response DTOs for the feature
  *.cs (feature root)   internal helpers shared by that feature's handlers only (builders, loaders)
DependencyInjection.cs
```

Rules:

1. **One use case per file**: request record, its validator and its handler live together in
   `Features/<Feature>/{Commands|Queries}/<UseCase>/<UseCase>Command.cs`. Nothing else goes in.
2. Handlers return DTOs, never entities.
3. Validation is declarative (FluentValidation) and runs automatically through
   `ValidationBehaviour` — handlers do not re-validate input shape.
4. Business rule violations throw `BusinessRuleException`; missing data throws
   `NotFoundException`; role/ownership violations throw `ForbiddenAccessException`.
5. Authorization by role is declared on controllers (`[Authorize(Roles = Roles.X)]`).
   Ownership checks (e.g. "this vacancy belongs to my company") live in handlers; the signed-in
   profile is resolved with `GetCurrentStudentAsync` / `GetCurrentPartnerAsync` / `GetCurrentAdministratorAsync`.
6. Application stays database-provider agnostic: only core EF Core APIs (no `AsSplitQuery`, no SQL).
   Provider behaviour (e.g. split queries by default) is configured in Infrastructure.
7. When a projection needs a domain rule (e.g. `IndustryTask.CanBeEdited`), load the entity and map in
   memory rather than re-writing the rule as a LINQ expression.

### 3.3 Infrastructure (`NSBM.ICU.Infrastructure`)

```
Persistence/
  ApplicationDbContext.cs                    conventions: all DateTimes stored/read as UTC
  Configurations/<Entity>Configuration.cs    one IEntityTypeConfiguration per entity; each declares its
                                             plural table name with ToTable("<Plural>") and every string length
  Interceptors/AuditableEntityInterceptor.cs fills CreatedAt/By, UpdatedAt/By
  Migrations/
  Seed/                                      DatabaseSeeder (migrate + seed) + one seeder per area
  Seed/Data/                                 seed content as pure data (no logic)
Identity/        ApplicationUser, TokenService, IdentityService, JwtOptions, DemoOptions, DemoAccounts
Services/        DateTimeService and other technical services
DependencyInjection.cs
```

### 3.4 API (`NSBM.ICU.API`)

```
Controllers/ApiControllerBase.cs   [ApiController], route api/v1/[controller], exposes Mediator
Controllers/<Feature>Controller.cs one controller per feature, each with [Route(ApiRoutes.V1 + "/...")]
Controllers/Requests/              request bodies shared by several controllers (e.g. ReviewRequest)
                                   student-scoped resources live under /me/... (e.g. /me/roadmap)
Middleware/ExceptionHandlingMiddleware.cs  → RFC 7807 ProblemDetails
Extensions/PresentationServiceExtensions.cs  JWT, CORS, Swagger, controllers
Services/CurrentUser.cs            ICurrentUser from HttpContext claims
Program.cs                         composition root only
```

Error contract (all errors):

```json
{ "status": 400, "title": "Validation failed", "detail": "...", "errors": { "Email": ["..."] } }
```

---

## 4. Web conventions (`web/`)

```
src/
  app/            App.tsx (providers), router.tsx, guards.tsx, layout/ (AppShell, Sidebar, Topbar, UserMenu)
                  — the only layer allowed to compose several features
  config/         brand.ts (name, colours, logo paths), navigation.ts (menu per role)
  components/
    ui/           design-system primitives: Button, Card, Badge, Input, Spinner, EmptyState …
    layout/       PageHeader and other feature-agnostic layout pieces
    brand/        BrandLogo
    evidence/     SkillEvidenceList, MatchMeter (match always shown with its responsible-use note)
    ModerationStatusBadge, ApplicationStatusBadge
    feedback/     Toaster + toast()
    ui/controlClass.ts  shared style for native inputs/selects/textareas (used with FormField)
  features/<feature>/
    api/          TanStack Query hooks + request functions for that feature only
    components/   components used only inside the feature
    pages/        route-level pages (default export)
    types.ts      feature types mirroring API DTOs
    index.ts      public surface — the only file other code may import from
  lib/            http client, cn(), formatters, download, queryKeys (shared query-key roots),
                  formErrors (maps API validation errors onto react-hook-form fields)
  types/          API enums shared by several features
  stores/         global client state only (auth, theme, ui)
  styles/         globals.css (NSBM design tokens + component classes)
```

Rules:

1. **Feature isolation** — a feature never imports another feature's internals; it may import
   another feature's `index.ts` only. Shared code is
   promoted to `components/`, `lib/` or `config/`.
2. **Server state lives in TanStack Query**; Zustand only holds client state (session, theme, UI).
   Queries about the signed-in user's own data use keys starting with `ME` (`lib/queryKeys.ts`), so
   one invalidation refreshes them all when the user's learning context changes.
3. **No hard-coded colours** in components — use the CSS variables / Tailwind tokens from
   `styles/globals.css` and `tailwind.config.ts`.
4. **No inline API URLs** in components — every call goes through `features/<x>/api`.
5. Pages are lazy-loaded from `app/router.tsx`; navigation per role comes from `config/navigation.ts`.
   A menu item is flipped to `ready: true` in the same change that adds its route.
6. Colours come from CSS variables, so Tailwind opacity modifiers (`bg-primary/20`) do not work —
   use the `*-soft` tokens instead.

---

## 5. Brand

Source: nsbm.ac.lk design tokens.

| Token           | Hex       | Use                          |
|-----------------|-----------|------------------------------|
| NSBM Blue 600   | `#0B5CAD` | primary actions, links       |
| NSBM Blue 800   | `#0B3C70` | headers, sidebar emphasis    |
| NSBM Blue 950   | `#071E3A` | dark surfaces                |
| NSBM Green 600  | `#168550` | accent, success, progress    |
| NSBM Green 500  | `#1FA463` | gradients, highlights        |
| Neutral 50      | `#F5F8FC` | page background              |
| Neutral 900     | `#172232` | headings                     |

Brand gradient: `linear-gradient(120deg, #0A4A8C 0%, #1A6FD1 50%, #1FA463 100%)`.
Logo files live in `web/public/brand/`.

---

## 6. Feature map (from the IndustryReady feature guide)

| Portal    | Features                                                                                         |
|-----------|--------------------------------------------------------------------------------------------------|
| Student   | Dashboard, Career pathways, Skills baseline, Roadmap, Readiness & gaps, Task library, Task workbench, Knowledge check, Team simulation, Mentor center, Portfolio, Interview prep (soft skills), Technical interview, Interview report, CV studio, Career center (internships), Applications, Planner |
| Lecturer  | Student directory, Student insights, Task assignment, Support signals, Feedback notes, Interview review, Research analytics |
| Industry  | Industry challenges, Task skills, Vacancies (industry hub), Applications review, Shortlist/invite, Candidate review, Mentor replies |
| Admin     | Task approval, Vacancy approval, Research overview, Settings / demo reset, Users (create/edit any role, deactivate, reset passwords, CSV student import), Reports, Audit log, Announcements to everyone or one group, Hall allocations (read-only) |
| Assistant Registrar | Venues (halls, labs, auditoriums, event halls), Hall allocations: lecture halls to cohorts (weekly repeats) and halls to partner or Unit events, with capacity, teaching-hours, hall and cohort clash checks |

### Build milestones

| # | Milestone                                   |
|---|---------------------------------------------|
| 1 | Foundation: solution, auth, roles, branded shell, login, role dashboards — **done** |
| 2 | Catalogue: career paths, skills, roadmap, baseline, student overview — **done**   |
| 3 | Industry tasks: create → approve → library → workbench → knowledge check — **done** |
| 4 | Vacancies & applications: create → approve → match → apply → track; readiness; candidate review — **done** |
| 5 | Interviews: soft skills (+ follow-ups, optional voice/camera), technical, report + practice log — **done** |
| 6 | CV studio + keyword finder, planner, mentor center, partner reviews, team simulation, portfolio — **done** |
| 7 | Lecturer directory/insights, support signals, notes, assignments, interview review, research analytics, admin settings + demo reset — **done** |
| 8 | Year calendar: admin plans events, task releases, career-plan milestones, interview drives; every role sees its own view with personal dates merged — **done** |
| 9 | Notifications centre (bell, page, written in the same save as the change) + live weekly digest on every dashboard — **done** |
| 10 | Industry events & hackathons: partner hosting with Unit approval, registration and places, teams with join codes, attendance, verifiable certificates (public /verify page), on the calendar — **done** |
| 11 | AI interviewer (Claude behind `IAiInterviewer`, follow-ups, rubric feedback via structured outputs, consent, private to the student) + on-device camera coaching (MediaPipe: framing, facing the camera, light, steadiness; pace and filler words) — no recording, no cheating or emotion scoring — **done** |
| 12 | Skill badges (requested from practice evidence, reviewed by lecturers, verification codes), weekly practice streaks, roadmap levels, opt-in public profile card (/card/:slug) — **done** |
| 13 | Public showcase homepage at / (live aggregate stats, career paths, open internships, public events, opt-in student profiles) + rate-limited partner enquiry form with honeypot and admin inbox — **done** |
| 14 | Playwright end-to-end suite (web/e2e): public pages, sign-in and role guards, every sidebar page per portal, student journey, cross-portal approvals, calendar, events, badges, AI interviewer state — **done** |
| 15 | Assistant Registrar portal: venues, hall allocation for lectures (cohort = programme + year) and events, clash and capacity rules (`VenueBookingRules`), everyone affected notified (cohort, registered students, host company, admins); students see their halls on the calendar; admin announcements to everyone or one group — **done** |
| 16 | User management: admin creates and edits accounts for every role (account + role profile saved in one transaction), deactivates/reactivates (open sessions stop at once via a token check; never yourself or the last admin), resets passwords, imports a cohort from CSV with a per-row report; everyone can change their own password; demo reset restores exactly the seeded accounts — **done** |
| 17 | Reports & exports: hall timetable (admin, registrar), event attendance and internship applications (admin), student participation as counts, never a ranking (admin, lecturers). Each report is one `ReportTable` served as JSON for preview and printing (save as PDF from the print dialog) or as a typed, filterable Excel file (`IReportFileWriter`, ClosedXML) — **done** |
| 18 | Audit log: `IAuditTrail` records approvals, account changes, password resets (never the password), hall allocations, announcements, calendar changes, partner application decisions, certificates and demo resets in the same save as the change; append-only (no edit/delete endpoint, kept through demo resets); admin page with area, search (incl. account e-mail), date filters, paging and Excel export — **done** |
| 19 | Deployment readiness: no secrets in committed settings (environment / user-secrets), the API refuses unsafe settings at start-up, real deployments seed no demo data and get their first admin from Bootstrap settings, UTC timestamps sent with `Z`, forwarded-headers support for the rate limiter, `/health/live` and `/health/ready`, security headers and HSTS, demo UI compiled out of production builds, Dockerfiles + docker-compose (SQL Server, API, nginx web) and [DEPLOYMENT.md](DEPLOYMENT.md) — **done** |
| 20 | E-mail notifications: a background `EmailDispatcher` treats the notifications table as an outbox (only committed notifications are e-mailed, one batched e-mail per person per cycle, failed sends retried), weekly digest on Monday mornings (Asia/Colombo), welcome e-mail for new accounts (never the password), security alerts always sent, calendar broadcasts kept in-app, per-person opt-out on a new My settings page, admin test e-mail. Senders: SMTP (MailKit), File (.eml inbox under `App_Data/mail` in development) or Off — **done** |

## 7. Responsible-use principles (from the guide)

* The platform never makes hiring or suitability decisions — people do.
* Match percentages count skill overlap only; they are labelled as such in the UI.
* Practice signals are not proof of skill; the UI says so where they appear.
* Camera/voice features are optional, nothing is recorded, text-only is always available.

---

## 8. Running locally

```
start.bat                      # installs web deps (first run), starts API + web, opens the browser
```

Manual:

```
dotnet run --project src/NSBM.ICU.API --launch-profile http     # API :5300, migrates + seeds
npm run dev --prefix web                                         # Web :5301
```

Settings: `appsettings.json` holds safe defaults only (no secrets, demo off). `appsettings.Development.json` holds
local development values, with a Windows-authentication connection string to `.\SQLEXPRESS`. If your SQL Server
uses a SQL login instead, keep that connection string out of the repository with user-secrets:

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>" --project src/NSBM.ICU.API
```

In development, e-mails are written as `.eml` files to `src/NSBM.ICU.API/App_Data/mail` (open them in Outlook or any mail app); nothing is sent.

Always start the API with the `http` launch profile (as above). `dotnet run --environment ...` does not set the
ASP.NET environment on the .NET 10 SDK. Deploying to a server: see [DEPLOYMENT.md](DEPLOYMENT.md).

New migration:

```
dotnet ef migrations add <Name> --project src/NSBM.ICU.Infrastructure --startup-project src/NSBM.ICU.API --output-dir Persistence/Migrations
```

Regression check (API running, demo mode on). Resets demo data, walks every portal's journeys and
role permissions, resets again, and exits non-zero on any failure:

```
python tests/api/regression_test.py
```

Unit tests (domain rules, AI interview flow with a fake AI, prompt/schema builder):

```
dotnet test tests/NSBM.ICU.UnitTests
```

Browser end-to-end tests (Playwright, runs in the installed Microsoft Edge; starts the API and web servers if
they are not running). They reset the demo data before and after, so only run them against a demo database:

```
npm run test:e2e --prefix web          # all journeys
npm run test:e2e:report --prefix web   # open the HTML report (traces and screenshots of failures)
```

AI interviewer (optional). Without a key the page says it is not set up and points to the standard practice.
The key is never stored in source control:

```
dotnet user-secrets set "Anthropic:ApiKey" "<key>" --project src/NSBM.ICU.API
```

(or set the `ANTHROPIC_API_KEY` environment variable). `Anthropic:Model` defaults to `claude-opus-5-5`.
Only the student's typed answers are sent; camera coaching runs entirely in the browser. Its MediaPipe runtime is
copied into `web/public/mediapipe/` on `npm install`; the face model is loaded from Google's published URL unless
`VITE_FACE_MODEL_URL` points to a self-hosted copy.

Demo accounts (fictional, seeded by `PeopleSeeder` from `DemoAccounts`):

| Role     | Email               | Password     |
|----------|---------------------|--------------|
| Admin    | icu.head@nsbm.demo  | Admin@123    |
| Lecturer | lecturer@nsbm.demo  | Lecturer@123 |
| Student  | student@nsbm.demo   | Student@123  |
| Industry | partner@nsbm.demo   | Industry@123 |
| Registrar | registrar@nsbm.demo (Assistant Registrar) | Registrar@123 |
| Industry | partner2@nsbm.demo (Serendib Software Labs), partner3@nsbm.demo (Ceylon QualityFirst) | Industry@123 |

Demo sign-in (`POST /api/v1/auth/demo-login`) is controlled by `Demo:Enabled` in `appsettings.json`;
the web shows demo controls when `VITE_DEMO_MODE=true` in `web/.env`.
