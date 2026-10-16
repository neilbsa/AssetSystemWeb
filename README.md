# Asset System

Tracks company equipment (computers, monitors, licences …) from purchase to the person who holds it.

* **Assets** – a purchase is registered as an *asset* made of *items*; every item gets a printable code
  (`CVCDVOM-001`), serial number, status (Available / In use / Defective) and attached documents.
* **Consignments** – equipment is issued to an employee; the signed receiving copy is attached; you can always
  see who holds what and the full history of an item.
* Multi-company, role based (Administrator / Common), mobile-first UI.

This is a ground-up rebuild of the original ASP.NET MVC 5 / SQL Server application on **.NET 10**, **PostgreSQL**,
**WolverineFx**, **React** and **Tailwind CSS**, organised as a **modular monolith** following **DDD** and
**clean architecture** with **vertical slices**. See [docs/architecture.md](docs/architecture.md) for the design.

```
src/
  AssetSystem.Host/                  composition root: wires modules, Wolverine, auth, OpenAPI
  BuildingBlocks/                    SharedKernel (DDD primitives, Result), Infrastructure (EF/outbox helpers), Web
  Modules/
    Identity/       sign-in, JWT, roles, which companies a user may work in
    Organization/   companies, branches, departments, employees, suppliers, item types
    Assets/         assets, items, numbering, documents, depreciation, dashboard
    Consignments/   issuing equipment to employees, receiving copies, item history
      └─ <Module>.Domain · .Application · .Infrastructure · .Api · .Contracts
tests/              unit, architecture (layer/module rules), integration (real PostgreSQL)
web/                React + TypeScript + Tailwind CSS single-page app
docker/             Dockerfiles and nginx config
```

## Run it

### With Docker (everything)

```bash
cp .env.example .env        # set POSTGRES_PASSWORD, JWT_SIGNING_KEY (32+ chars), ADMIN_PASSWORD
docker compose up -d --build
```

Open <http://localhost:8080> and sign in as `administrator` with `ADMIN_PASSWORD`. A fresh install has no company yet:
the first sign-in asks you to create one. Set `SEED_SAMPLE_DATA=true` to start with a demo company, branches and departments.

> On the very first start the log shows a few `Failed executing DbCommand … __ef_migrations` lines. That is EF Core
> checking for its history table before creating it; it is expected and harmless.

### For development

```bash
docker compose -f docker-compose.dev.yml up -d      # PostgreSQL on :5432
dotnet run --project src/AssetSystem.Host           # API on http://localhost:5080  (docs at /docs)
cd web && npm install && npm run dev                # app on http://localhost:5173 (proxies /api to :5080)
```

Development settings create the administrator `administrator` / `Admin#12345` and a demo company
(see `src/AssetSystem.Host/appsettings.Development.json`).

## Test it

```bash
dotnet test --solution AssetSystem.slnx     # unit + architecture + integration
cd web && npm run lint && npm run typecheck && npm test && npm run build
```

Integration tests start the real application against a throw-away PostgreSQL database. Point `TEST_POSTGRES` at a
server whose user can create databases (default: `Host=localhost;Username=assetsystem;Password=assetsystem;Database=postgres`);
they skip themselves when no server is reachable. CI runs everything (`.github/workflows/ci.yml`).

## Configuration

Set through environment variables (`Section__Key`) or `appsettings*.json`.

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__Postgres` | PostgreSQL connection string (one database, one schema per module) |
| `Jwt__SigningKey` | HMAC key for access tokens, **32+ characters**, keep secret |
| `Jwt__LifetimeMinutes` | Token lifetime (default 480) |
| `Seeding__AdminPassword` | Creates the first administrator on first start (when no users exist) |
| `Seeding__SampleData` | `true` creates a demo company, branches and departments |
| `FileStore__RootPath` | Where uploaded documents are stored (a volume in Docker) |
| `Api__ExposeDocs` | `true` serves OpenAPI/Scalar docs (`/docs`) outside Development |

## What changed from the legacy app

* Roles, company selection and company-scoped data are kept; item/asset numbering is kept (`2CS0001`, `2SM0001`,
  `CVCDVOM-001`, `SMM-0001`) but now comes from atomic database counters instead of read-then-write.
* Item status "OnUsed" is now "In use". Items only become *in use* through a consignment, and serial numbers / keys are
  unique per company (enforced by the domain and by the database).
* PDF reports (wkhtmltopdf/RDLC) are replaced by print-optimised pages: use **Print** and "Save as PDF".
* The unfinished "Transfer asset" screen was not carried over.
* Existing SQL Server data is **not** migrated automatically; the schema is new (GUID ids, one schema per module).
