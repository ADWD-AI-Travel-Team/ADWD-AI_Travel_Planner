# AI Travel Planner

ASP.NET Core (.NET 10) backend with Domain, Application, Infrastructure and
WebApi projects. The separate React prototype lives in `src/WebFrontend`.

## Checks

Install Git, the **.NET SDK 10.0.100** specified in `global.json`, and **Node.js
22** (includes npm). No database, model, API key or environment file is required.

From a new directory:

```sh
git clone https://github.com/TrongNhan2401/ADWD-AI_Travel_Planner.git
cd ADWD-AI_Travel_Planner
```

For a pull request, check out its branch before running the checks. Run these
four commands from the repository root:

```sh
npm run lint
npm run format
npm run typecheck
npm run test
```

On Windows PowerShell with script execution disabled, use `npm.cmd` in place of
`npm`; this invokes the same npm scripts.

The root `package.json` is the single definition of these four commands. It has
no Node dependencies, so `npm install` is unnecessary. Each command restores
NuGet packages automatically; the first run needs access to NuGet.org.

| Command | Check |
| --- | --- |
| `npm run lint` | .NET analyzers, including informational diagnostics, without modifying files |
| `npm run format` | C# whitespace without modifying files |
| `npm run typecheck` | Release compilation with warnings treated as errors |
| `npm run test` | xUnit tests, failing if no tests are discovered |

These checks cover all backend projects and tests in `AITravelPlanner.sln`.
The frontend prototype is outside this C# gate. The smoke test runs the existing
weather endpoint through an in-process test server and makes no external calls.
It does not implement the Save endpoint contract, which is a separate task.

GitHub Actions runs exactly the same four commands in separate `lint`, `format`,
`typecheck` and `test` jobs on every pull request, pushes to `main`, and manual
dispatch. Every command must succeed. The repository owner will enable required
checks in branch protection after this workflow is merged.

## Environment

`.env.example` lists optional ASP.NET Core variable names without values.
The checks need none of them. ASP.NET Core reads environment variables directly;
this repository does not load `.env` files. Keep local credentials out of git.
No runtime model has been chosen and no model calls are needed for PA#2.
