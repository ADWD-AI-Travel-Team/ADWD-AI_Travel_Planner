
## Checks

Prerequisites: Git, .NET SDK **10.0.401** (pinned in `global.json`), Node.js 24
with npm, and Bash (Git Bash on Windows). From a new directory:

```sh
git clone https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner.git
cd ADWD-AI_Travel_Planner
# If reviewing an unmerged PR, check out its published branch here.
cd src/WebFrontend
npm ci
cd ../..
```

An unmerged branch must be published before others can clone its changes.
Current Places implementation work is local on
`feature/places-contract-implementation` until the author commits and pushes it.
After merge, use main. On Windows PowerShell, use `npm.cmd ci`
if script execution policy blocks `npm.ps1`. Run checks in Git Bash.
The .NET commands restore NuGet packages automatically and
require access to NuGet on the first run. No database or model key is needed.

Run from the repository root:

```sh
bash scripts/check-lint.sh
bash scripts/check-format.sh
bash scripts/check-types.sh
bash scripts/check-tests.sh
```

| Command | Scope |
| --- | --- |
| `bash scripts/check-lint.sh` | C# analyzers and the existing frontend oxlint script. |
| `bash scripts/check-format.sh` | C# whitespace verification only; no frontend formatter is installed. |
| `bash scripts/check-types.sh` | Release build of the C# solution with warnings as errors, followed by TypeScript project checking using the installed local compiler and existing noEmit settings. Does not run Vite. |
| `bash scripts/check-tests.sh` | Existing .NET solution tests; requests failure if no tests are discovered. No frontend test runner is configured. |

Each script locates the repository relative to itself, stops on command failure
and returns a nonzero exit code. They do not need another check to run first.
TypeScript can update incremental build metadata under ignored `node_modules`.
Formatting checks report differences without rewriting source files.

GitHub Actions invokes exactly these four check commands in separate `lint`,
`format`, `typecheck` and `test` jobs on pull requests targeting any branch,
pushes to main and feature/**, and manual dispatch. Jobs install only the tools
and dependencies needed for their check; `npm ci` is setup, not another check.
The scripts are the single definition of the check operations. No job hides
failures. Branch protection is configured separately by the repository owner;
the workflow alone does not make checks required.

## GET /places (PA#2)

The public endpoint follows [the approved contract](docs/contract/get-places.md).
See [the implementation guide](docs/implementation/get-places.md) for layer/file
responsibilities, manual requests, test coverage, and the human PR red-to-green
demonstration. [Parser observations](docs/implementation/get-places-parser-observations.md)
record the .NET 10 test-host behavior without changing the contract.

By default the catalog is empty; no official dataset is bundled. Synthetic
PA#2 records require both Development environment and explicit
`Catalog:UsePa2Fixture=true` configuration (`Catalog__UsePa2Fixture` as an
environment variable). They are never enabled in Production. Contract tests
inject their own deterministic fake catalog and exercise the real HTTP app.

The existing `bash scripts/check-tests.sh` includes the new contract test
project through the solution; no separate CI invocation is needed.

## Environment Configuration

The checks above need no environment file. Optional frontend configuration is
documented by `src/WebFrontend/.env.example`. Do not put real credentials in
tracked configuration or commit a local `.env` file.

The model API key must only be configured on the backend.
It must never be exposed to the frontend or browser bundle.

## LLM API Usage and Spending Limits

AI Travel Planner uses an external LLM provider for
AI-assisted travel planning.

To prevent unexpected API expenses:

- The API key is stored exclusively in the backend environment.
- A monthly spending cap is configured in the provider dashboard.
- Usage alerts are enabled.
- The team monitors token consumption and API costs.
- Unit tests use mocked model responses instead of real API calls.

### Configured Spending Limit

- Provider: [Actual provider]
- Monthly budget/cap: [Actual configured amount]
- Alert threshold: [Actual configured threshold]
- Budget owner: [Responsible team member]

The team must review API usage regularly and must not
increase the spending limit without approval.


