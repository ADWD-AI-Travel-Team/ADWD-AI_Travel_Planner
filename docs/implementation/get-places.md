# GET /places implementation and learning guide

The approved behavioral source is `docs/contract/get-places.md`. This guide
explains its implementation without rewriting the approved contract. Work is
local on `feature/places-contract-implementation`, based on remote main
47f9e2e. No commit, push or repository-protection change is part of this task.

## 1. What was reused and what was repaired

The solution, four quality-check scripts, four CI jobs, pinned SDK and existing
architecture tests were reused. The new HTTP test project is registered in the
same solution, so check-tests.sh and the existing CI test job discover it; the
workflow and scripts do not need another test command.

Main initially ran four architecture tests with three passes and one failure:
Infrastructure's direct Domain reference had been removed while its test still
required that reference. With the user's approval the reference was restored,
and all four old tests passed before endpoint work. The test was not changed.

The user approved Microsoft.AspNetCore.Mvc.Testing 10.0.15. It targets the
project's .NET 10 framework and matches the existing ASP.NET Core package
version. Restore, build and execution of TestServer and WebApplicationFactory
tests on installed runtime 10.0.12 verified local compatibility. Existing xUnit
2.9.3, runner 3.1.4 and Microsoft.NET.Test.Sdk 17.14.1 versions were reused.

## 2. File responsibilities

All source paths below are relative to the repository root.

| File | Responsibility |
| --- | --- |
| `src/Domain/Places/Place.cs` | Internal catalog model, complete eligibility predicate and public-data invariant checks. Knows no HTTP status, route or JSON response shape. |
| `src/Application/Places/IPlaceCatalog.cs` | Read-only abstraction that supplies one catalog snapshot; it has no write method. |
| `src/Application/Places/PlacesPage.cs` | Validated use-case query and public DTOs. Only id/name/category/district leave the internal model. |
| `src/Application/Places/ListPlaces.cs` | Reads once, excludes ineligible records, validates candidates/unique IDs, applies AND filters, counts, sorts numerically and pages. |
| `src/Infrastructure/Places/EmptyPlaceCatalog.cs` | Safe empty catalog until a real data adapter exists. Used by default, including Production. |
| `src/Infrastructure/Places/Pa2FixturePlaceCatalog.cs` | Explicitly synthetic development adapter with the eleven reference fixture records in shuffled order. Not verified real-world venue data. |
| `src/WebApi/Places/PlacesQueryParser.cs` | Explicit query-key, multiplicity, empty-value, grammar, bounds and enum validation over framework-parsed query data. Aggregates errors before catalog access. |
| `src/WebApi/Places/PlacesEndpoints.cs` | Anonymous GET route, no-store, HTTP success and safe Problem Details mapping. Exceptions are logged internally and never returned as exception messages. |
| `src/WebApi/Program.cs` | Registers the use case and catalog adapter, and maps the route. Keeps the existing weather endpoint. |
| `tests/AITravelPlanner.ContractTests/AITravelPlanner.ContractTests.csproj` | Test-host/xUnit dependencies and reference to WebApi. |
| `tests/AITravelPlanner.ContractTests/PlacesTestHost.cs` | Real application factory and independently declared fixture/fake catalog. Replaces the dependency, not HTTP responses. |
| `tests/AITravelPlanner.ContractTests/PlacesHttpContractTests.cs` | HTTP requests, expected IDs/totals, schema/header/error assertions, eligibility, retries, safe failures and mutation-sensitive numeric-sort test. |
| `tests/AITravelPlanner.ContractTests/CatalogConfigurationHttpTests.cs` | Verifies real DI registrations: fixture requires explicit Development opt-in and never activates in Production. |
| `tests/AITravelPlanner.ContractTests/QueryParserCharacterizationTests.cs` | Isolated test-only host to record framework query behavior. |
| `AITravelPlanner.sln` | Includes the new project in existing solution checks. |

`docs/contract/get-places-implementation-plan.md` contains the preimplementation
mapping and baseline. `get-places-parser-observations.md` beside this guide
contains observed key/encoding behavior, including its limits.

## 3. Follow one request through the layers

For `/places?category=cafe&district=q1&page=2&pageSize=1`:

1. ASP.NET Core parses the query. WebApi enumerates the parsed keys and values,
   rejects malformed application parameters, and creates a PlacesQuery.
2. A validation error returns application/problem+json immediately without a
   catalog read. A valid query proceeds to ListPlaces.
3. The catalog dependency returns one snapshot. Domain eligibility excludes
   unverified/outside-city/private/draft/unpublished records, a false eligibility
   flag, and missing or empty hours/menu data. Flags cannot bypass required data.
4. Included records must have valid public fields and unique IDs. Broken
   invariants throw and become a generic application 500 at the HTTP boundary.
5. The use case filters with AND. On the reference fixture the candidates are
   IDs 1 and 6. totalItems is 2, sorting gives [1,6], and page 2 of size 1 gives [6].
6. The response is application/json with page 2, pageSize 1, totalItems 2 and
   totalPages 2. No internal verification/hours/menu metadata is exposed.

Numeric conversion accumulates ASCII digits while checking the configured
bound before multiplication can overflow. Many leading zeros still mean the
same number. Neither invalid values nor whitespace are repaired into defaults.

## 4. Fixture activation and manual requests

Tests inject a deterministic catalog independent of production registrations.
Running the API normally returns an empty collection until real data is supplied.
For synthetic development data only, use PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Catalog__UsePa2Fixture = "true"
dotnet run --project src/WebApi --launch-profile https
```

In another terminal, use the HTTPS address printed by the application to call
`/places` or the filtered request above. The repository's existing HTTPS launch
profile uses port 7204; trust a local development certificate if your client
requires it. Do not bypass TLS checks as a production configuration.

Use `Remove-Item Env:Catalog__UsePa2Fixture` afterwards if that shell should no
longer opt in. No secret, database or model is required. The internal hours and
menu strings are synthetic presence data, not real schedules or prices. This
endpoint does not implement the itinerary validator or real dataset ingestion.

## 5. How the tests exercise the contract

WebApplicationFactory<Program> starts the real application in Testing with a
controlled catalog. HttpClient requests travel through actual routing, framework
query parsing, the WebApi boundary, Application use case and Domain checks.
Only catalog access is replaced. The client uses https://localhost so the
existing HTTPS-redirection middleware is not mistaken for endpoint behavior.

| Contract groups | Test coverage |
| --- | --- |
| GP01–GP09, GP19 | Parameterized success cases: anonymous defaults, filters, bounds, sorted pages, beyond-end and no matches. |
| GP10 | Empty catalog and zero totals. |
| GP11, GP31, GP32 | Reference fixture exclusions plus independent per-condition cases; missing/empty hours/menu stay excluded even with eligible true. |
| GP12–GP18, GP20, GP22, GP23, GP28 | Parameterized invalid queries, aggregated error dictionaries, no catalog access, strict enums/case/syntax and bounded conversion. |
| GP21 | Thirteen framework characterization cases and the observation report; no invented malformed-input problem type. |
| GP24, GP25 | Read-only retries, no replay header, irrelevant bearer/cookie/Accept/body/idempotency headers and unchanged fake data. |
| GP26, GP27 | Throwing catalog, duplicate eligible IDs and invalid public fields return safe 500. |
| GP29, GP30 | Numeric sort of [10,2,1] and coherent metadata when a controlled catalog changes between requests. |
| Additional safety | Production ignores synthetic-fixture opt-in; Development requires explicit opt-in. Field boundaries include 200 Unicode scalar values and maximum int ID. |

Helpers verify response status, parsed media type, no-store, required JSON types,
exact IDs, DTO values and metadata. Error helpers verify type/title/body-status,
instance and safety. Invalid-query assertions compare exact invalid key groups
and nonempty messages without matching 400 wording. Internal errors must use
the fixed 500 detail and omit errors. Fake read counts prove validation runs
before catalog access. Tests do not call a real model or external API.

## 6. The local deliberate violation experiment

The approved local experiment changed only Application's `OrderBy` to
`OrderByDescending`, leaving contract and tests unchanged. The targeted HTTP
test `Numeric_sort_and_changed_catalog_have_coherent_per_response_totals`
failed with expected [1,2,10] and actual [10,2,1], exit code 1. A finally block
restored the original implementation bytes immediately. No broken commit was
created. Subsequent quality checks rebuild the restored implementation.

This proves local detection of that specific regression. It is not yet evidence
of red CI or a blocked merge; those require a real remote PR and required checks.

## 7. Run the same quality gate locally

Final local result on 2026-11-25: lint, format, typecheck and test all passed.
The build reported zero warnings/errors. The test script ran **137 tests**:
4 unchanged architecture cases and 133 in the new project (116 real-app HTTP
contract cases, 4 catalog-configuration HTTP cases and 13 framework parser
characterization cases). Nothing was skipped. SDK was 10.0.401, runtime
10.0.12, and Node was 24.22.3 using existing temporary local tooling.

During development the gate caught an encoding mistake introduced while
editing test data on Windows: the intended supplementary Unicode character
was corrupted, making the name exceed 200 scalars. The fixture was repaired
using explicit C# Unicode escapes; the 200-scalar rule and success assertion
were preserved. Analyzer diagnostics were addressed with typed theory rows,
serializable scenario names and reusable arrays, without disabling any rules.

No remote CI run has been performed for these uncommitted changes. The
framework parser and catalog configuration tests provide local evidence only.

Install SDK 10.0.401, Node 24 with npm and Bash as described in README. Install
existing frontend dependencies with npm ci in src/WebFrontend if necessary.
From the repository root:

```bash
bash scripts/check-lint.sh
bash scripts/check-format.sh
bash scripts/check-types.sh
bash scripts/check-tests.sh
```

The test script runs both the existing architecture project and new HTTP test
project. No workflow alteration is necessary. Local execution, a successful
build, and remote GitHub Actions are distinct evidence; do not mark remote CI
as verified before publishing and observing an actual run.

## 8. Human steps for a real PR red → blocked → fixed → green

These are instructions only; the assistant did not execute the Git mutations.
Review the working tree and create focused commits yourself. Preserve the
approved contract and architecture test. Keep this branch's implementation,
tests, solution change and documentation in the review.

1. Commit the correct local implementation/test suite after reviewing its diff,
   push `feature/places-contract-implementation`, and open a PR into main.
2. Confirm the test job runs the new project. Confirm repository rules require
   the relevant checks. A rule requiring PRs alone does not prove red checks
   block merges. Ask the repository owner if required status checks are absent.
3. With explicit team approval, make a separate implementation-only commit
   changing numeric sort to descending. Do not edit assertions or the contract.
   A suitable English message is `Demonstrate contract gate with incorrect place ordering`.
4. Push that commit to the same PR. Wait for the test job to fail. Record the
   real commit hash, failed run link and blocked-merge UI. Do not merge it.
5. Change the implementation back to ascending in a new normal fixing commit,
   such as `Restore place ordering required by the contract`. Rerun local checks,
   push, and retain the successful CI link for the fixed head.
6. Append the real evidence to AI-LOG. Keep both commits in history. Do not amend,
   rebase, force-push or invent a blocked-merge event.

The final implementation currently uses the correct ascending sort. Production
dataset integration, endpoint writes and deployed infrastructure are outside
this task. Kestrel/proxy malformed-target behavior was not verified by in-process
TestServer observations and is not claimed here.
