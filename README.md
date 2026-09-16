# Bison

A command-line observation log. Users record wildlife/nature observations with a location, and comment on each other's observations. Data is stored in flat CSV files.

BDSA 2026, Group 18.

## Layout

| Path | Purpose |
| --- | --- |
| `src/Bison.CLI` | CLI entry point (`Program.cs`) and argument parsing (`CLIHandler.cs`) |
| `src/SimpleDB` | `CSVDatabase<T>` — a singleton CSV-backed repository implementing `IDatabaseRepository<T>`, plus the CSV data files |
| `tests/SimpleDB.Tests` | Unit tests for read/write against a fixture CSV |
| `tests/Bison.CLI.Tests` | End-to-end tests that invoke `Program.Main` and capture stdout |

Two record types are persisted: `Observation` (Id, Author, Description, Timestamp, Location) and `Comment` (ParentId, Author, Description, Timestamp). Author is taken from the OS username; timestamps are Unix seconds.

## Requirements

.NET 8 SDK.

## Build and run

```bash
./Build.sh          # builds Debug and Release
./Build.sh -r       # Release only
./Build.sh -d       # Debug only
./Build.sh -f FLAG_TEST   # build with a preprocessor feature flag

./Run.sh --read
```

`Run.sh` executes the Release build. The CSV paths are resolved relative to the working directory (`src/SimpleDB/`), so run from the repository root.

## Commands

| Command | Alias | Arguments | Description |
| --- | --- | --- | --- |
| `--read` | `-r` | — | Print all observations |
| `--observe` | `-o` | `<description> <location>` | Record a new observation |
| `--comment` | `-c` | `<comment> <id>` | Comment on an observation by id |
| `--discussion` | `-d` | `<observationId>` | Print all comments on an observation |
| `--location` | `-l` | `<location>` | Print observations at a location |

```bash
./Run.sh --observe "Bison by the lake" Amager
./Run.sh --location Amager
./Run.sh --comment "Nice one" 3664203166191316372
./Run.sh --discussion 3664203166191316372
```

## Tests

```bash
dotnet test tests/SimpleDB.Tests/
dotnet test tests/Bison.CLI.Tests/
```

## Formatting

Formatting is enforced in CI with [CSharpier](https://csharpier.com/) (tabs, 100-column width; see `.csharpierrc.json`).

```bash
dotnet tool restore
dotnet csharpier format .
```

## Releases

`ArtifactBuilder.sh` publishes self-contained binaries for Windows, Linux and MacOS on x86 and Arm64. CI runs build, tests and this script on pushes and PRs against `main`, uploading each target as a workflow artifact.
