
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
The Places implementation is merged into main. When reviewing another unmerged
change, select its published PR branch. On Windows PowerShell, use `npm.cmd ci`
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
`format`, `typecheck` and `test` jobs on pull requests targeting main,
pushes to main and feature/**, pushes of the exact submission tag `pa2`, and
manual dispatch. The tag run checks the commit referenced by the tag using
the same four scripts. Jobs install only the tools
and dependencies needed for their check; `npm ci` is setup, not another check.
The scripts are the single definition of the check operations. No job hides
failures. Branch protection is configured separately by the repository owner;
the workflow alone does not make checks required.

Submission tagging is a human action after reviewing the final main revision.
Do not create or push `pa2` before the submission is ready. After the tag is
pushed, verify all four jobs succeed and record the actual tag-run link in
the submission evidence. Adding this trigger alone is not evidence of a
successful tag run. An existing published tag must not be moved or overwritten.

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

## PA#2 evidence and submission

- [Submission evidence index](PA2.md)
- [Contract-gate screenshots and commit/run links](docs/evidence/pa2-contract-gate.md)
- [Team self-assessment](SELF_ASSESSMENT_REPORT.md)
- [AI activity log](AI-LOG.md)
- [Finalization procedure and remaining human decisions](docs/evidence/pa2-submission-finalization.md)

## LLM API Usage and Spending Limits

PA#2's implemented catalog endpoint does not call a model. Its HTTP tests use a
controlled catalog, not a live model, production database or external API.
The committed appsettings.json contains proposed provider/model defaults
(OpenAI / gpt-4.1-mini); those strings do not prove a live integration, account,
API key or provider budget exists.

- Runtime model execution in this PA#2 implementation: none.
- Provider account, spending cap and alert configuration: not verified in this
  repository review. Do not claim they are configured without real evidence.
- Model-call cost incurred by these checks/tests: zero; no model calls are made.
- Estimated cost of generating one travel plan: not measured; plan generation
  is not implemented by this endpoint.

Before a real model integration, the team must choose its provider/model,
configure an approved spending cap and alerts, and document the actual owner
and limits. Model credentials stay on the backend and out of committed files.
Do not increase an approved spending limit without human approval.
