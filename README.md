# Bison

A web app for logging wildlife/nature observations. Users post observations tagged with a taxon (species), comment on each other's observations, and propose alternative species identifications.

BDSA 2026, Group 18.

Built with ASP.NET Core 8 Razor Pages, Entity Framework Core (SQLite) and ASP.NET Core Identity. Deployed to Azure App Service (`bdsagroup18bison`).

## Layout

| Path | Purpose |
| --- | --- |
| `src/Bison.Razor` | The web app |
| `src/Bison.Razor/Program.cs` | Startup: DI, database setup/seeding, Identity, middleware |
| `src/Bison.Razor/BisonDbContext.cs` | EF Core context (`IdentityDbContext<User>`) and table/column mappings |
| `src/Bison.Razor/DBObjects/` | Entities: `User`, `Post` (base), `Observation`, `Comment`, `Proposal`, `Taxon` |
| `src/Bison.Razor/PostRepository.cs` | Data access (`IPostRepository`) |
| `src/Bison.Razor/BisonService.cs` | `ObservationService`: maps entities to view models for the pages |
| `src/Bison.Razor/ObservationFilter.cs` | Taxon filtering, backed by the Dafny-generated code |
| `src/Bison.Razor/DbInitializer.cs` | Seed data (users, taxa, observations, comments, proposals) |
| `src/Bison.Razor/Pages/` | Razor pages and shared layout/nav (`Shared/_Layout`, `Shared/_LoginPartial`) |
| `src/Bison.Razor/Areas/Identity/Pages/Account/` | Custom `Login` and `Register` pages (other Identity pages come from the default Identity UI) |
| `src/Bison.Razor/Migrations/` | EF Core migrations |
| `dafny/` | Formally verified observation-by-taxon filter (`.dfy`) and its translated C# |
| `data/taxon.csv` | Taxon source data |
| `src/Database/` | Legacy SQL schema/dumps and `initDB.sh` |
| `tests/API.Tests` | Integration tests using `WebApplicationFactory` against `tests/data/apitestdata.db` |
| `tests/ObservationFilter.Tests` | Unit tests for the taxon filter |

### Data model

`Post` is the base type (author, text, publish date) mapped with table-per-type, so `Observation`, `Comment` and `Proposal` each get their own table:

- **Observation**: a post about a sighting of a `Taxon`
- **Comment**: a post on an `Observation`
- **Proposal**: suggests a different `Taxon` for an `Observation`
- **Taxon**: a hierarchical species tree (`ParentTaxon` / `ChildTaxons`)
- **User**: extends `IdentityUser` with a required display `Name`

## Pages

| Route | Page |
| --- | --- |
| `/obs` | Public timeline of all observations |
| `/obs/{author}` | One user's observations |
| `/ob/{id}` | A single observation with its comments and proposals |
| `/Identity/Account/Register` | Create an account (username, email, password) |
| `/Identity/Account/Login` | Log in with email and password |

## Requirements

- .NET 8 SDK
- To rebuild the Dafny filter: `dotnet tool restore` and `z3` installed

## Build and run

```bash
./Build.sh          # builds Debug and Release
./Build.sh -r       # Release only
./Build.sh -d       # Debug only
./Build.sh -f FLAG  # build with a preprocessor feature flag

./Run.sh            # runs the Release build
```

Or during development:

```bash
dotnet run --project src/Bison.Razor
```

The `http` launch profile serves on `http://localhost:5273`; the `https` profile adds `https://localhost:7102`.

### Database

- If `BISONDBPATH` is set, the database is `src/Database/<BISONDBPATH>`; otherwise it's `bison.db` in the system temp directory.
- On startup, migrations are applied and seed data is inserted.
- **In the `Development` environment the database is deleted and recreated on every run.**
- In the `Testing` environment (used by the API tests) startup migration and seeding are skipped.

## Tests

```bash
dotnet test tests/API.Tests/
dotnet test tests/ObservationFilter.Tests/
```

### Dafny filter

The filter in `dafny/ObservationTaxonFilter.dfy` is verified and translated to C#, which is compiled into the web app:

```bash
cd dafny
dotnet tool run dafny -- verify ObservationTaxonFilter.dfy --allow-warnings --solver-path /usr/bin/z3
dotnet tool run dafny -- translate cs ObservationTaxonFilter.dfy --allow-warnings --include-runtime --solver-path /usr/bin/z3
```

## Formatting

Code is formatted with CSharpier, and PRs are checked in CI:

```bash
dotnet tool restore
dotnet csharpier format .
dotnet csharpier check .
```

## CI/CD

| Workflow | Trigger | What it does |
| --- | --- | --- |
| `build_and_test.yml` | push/PR to `main` | Verifies Dafny, builds, runs tests, publishes self-contained builds for Windows/Linux/macOS (x64 + arm64) |
| `validate_formatting.yml` | PR to `main` | `csharpier check` |
| `main_bdsagroup18bison.yml` | push to `main` | Builds and deploys to Azure App Service |
