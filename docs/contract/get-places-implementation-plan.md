# GET /places implementation and HTTP-test plan

Follow-up: this initial plan was written before dependency approval. The user
subsequently approved Mvc.Testing 10.0.15 and reference repair. Local
implementation and verification are now complete; see
`docs/implementation/get-places.md` for results. Historical baseline and
pending-language below describe the planning stage, not current progress.

## Starting point and baseline

Work branch: `feature/places-contract-implementation`, created from remote main
`47f9e2e`. No commit or push is authorized for this task.

The approved behavioral source is `docs/contract/get-places.md`; it is not edited
by this plan. The user approved its content in the implementation request even
though its historical document status still says proposed for review.

Already available: four check scripts, four CI jobs, a .NET 10 SDK pin, the
layered projects, xUnit infrastructure and four architecture test cases.
Missing: catalog domain data, catalog abstraction/implementation, list use case,
HTTP query validation and route, HTTP test host and contract tests.

Baseline command: `bash scripts/check-tests.sh`. Observed before new code:
four tests ran, three passed, one failed. The Infrastructure test expects direct
references to Application and Domain, while remote main removed Domain in
commit `1de27ce`. This is a pre-existing code/test architecture disagreement,
not a new endpoint failure or an environment failure. Do not weaken its test.
Proposed repair: restore the direct Domain reference for an Infrastructure
catalog adapter that uses the Domain model. This preserves the existing
executable architecture expectation rather than redefining it.

## Decisions and ambiguities to keep visible

- Do not interpret historical draft/status wording as permission to change the
  user's approved behavior. Keep new implementation evidence outside the contract.
- Parsed query key spelling/count behavior and encoding boundaries still need
  real observation under .NET 10. Do not promise a custom decoding policy or
  add a production debug route to observe it.
- Eligibility is a conjunction: public/published, not private/draft, HCMC,
  verified, hours and menu/ticket data present, and no false eligibility flag.
  Fixture metadata simulates these facts; it is not real-world verification.
- Normal production startup must not silently publish synthetic venues as
  verified data. Use an explicitly named PA2 fixture adapter only in an
  explicitly selected development configuration; otherwise an empty catalog
  is safer until a real dataset adapter is supplied. HTTP tests inject their
  own catalog and do not depend on environment-specific fixture activation.
- A catalog read must be coherent within a response. No snapshot across
  requests is promised. Fake catalog mutation tests exercise that boundary.
- Header/body handling follows the existing contract, including anonymous
  access, no-store, fixed representation and ignored request body.

## Requirement to implementation to test mapping

Paths below are proposed files, not claims that they already exist.

| Contract requirements | Implementation responsibility / proposed files | HTTP tests |
| --- | --- | --- |
| Eligible public HCMC records; missing hours/menu excluded | Domain/Places/Place.cs: internal catalog data and eligibility predicate | GP11, GP31, GP32, private/draft variants; assert items and totals |
| Public fields, valid IDs/names/enums; invalid eligible records fail safely | Domain/Places/Place.cs: public-record invariant checks | GP27 parameterized invalid fields, nulls, duplicates; assert safe 500 |
| Read-only catalog dependency and coherent read | Application/Places/IPlaceCatalog.cs; Infrastructure/Places adapters | Controlled fake read count, mutation/retry checks; no write API |
| DTOs and list use case | Application/Places/PlaceDto.cs, PlacesPage.cs, ListPlaces.cs | GP01–GP10: complete JSON schema, exact IDs and page metadata |
| AND filters, numeric order, total before paging | Application/Places/ListPlaces.cs | GP03–GP09, GP29, GP30; shuffled fixture and numeric IDs 1/2/10 |
| Exact query keys, duplicates, empty/syntax/range/enum checks | WebApi/Places/PlacesQueryParser.cs using Request.Query | GP12–GP20, GP22, GP23; mixed-case duplicates in both orders |
| Validation precedes catalog; aggregated errors | WebApi parser and endpoint | GP22, GP28; assert complete error key set and zero fake reads |
| Anonymous route, ignored credentials/Accept/body; no ownership | WebApi/Places/PlacesEndpoints.cs, Program.cs registration | GP01, GP25: anonymous HTTP success; verify no challenge/redirect |
| RFC 9457 application errors and no-store | WebApi/Places/PlacesEndpoints.cs: status/content/error mapping | Every HTTP case checks media type, headers and required properties; GP26/GP27 cover 500 |
| Safe exception mapping | WebApi endpoint boundary with controlled catalog exceptions | GP26 uses throwing fake and a secret sentinel; assert fixed 500 detail and no exception leakage |
| GET repeatability, no idempotency machinery | Read-only use case and route | GP24: equal values on stable fake, no replay header or mutation |
| Framework parser characterization | Test host plus separate observation report | GP21: actual key spellings/counts and encoding observations, no invented malformed-input guarantee |
| Existing checks discover new tests | New ContractTests project registered in AITravelPlanner.sln | Existing check-tests.sh and CI solution test command; report real counts |

## Proposed test-host dependency requiring approval

Add `Microsoft.AspNetCore.Mvc.Testing` version `10.0.15` to a new
`tests/AITravelPlanner.ContractTests` project. This matches the existing ASP.NET
Core package version and supplies WebApplicationFactory/TestServer, so HTTP
requests execute real routing, query parsing, application code and error mapping
inside a test host. The fake replaces the catalog dependency, not endpoint
responses. Existing xUnit and test SDK versions can be reused in that project.

`AI-PERMISSIONS.md`, section 2, requires explicit approval before adding,
removing or upgrading dependencies. No new package has been added yet.

## Implementation sequence after approval

1. Implement Domain data/eligibility and Application abstraction, DTOs and list
   use case. Keep HTTP-specific rules in WebApi.
2. Add Infrastructure catalog adapter and explicit fixture activation; repair
   its direct reference consistently with the existing architecture test.
3. Add WebApi query validation, route, DI and safe Problem Details mapping.
4. Add the HTTP test project, independent deterministic fake fixture, throwing
   fake, common schema/header assertions and contract case coverage.
5. Characterize framework query behavior and record actual observations in a
   separate report without changing the approved contract.
6. Run lint, format, typecheck and tests. Report counts and distinguish setup
   failures from assertion failures. Do not claim remote CI without a real run.
7. Temporarily break implementation behavior, such as numeric sort direction,
   while preserving tests and contract. Run the relevant HTTP test, record the
   failure, restore implementation and verify green. Do not commit the mutation.
8. Append real evidence to AI_LOG_PA2.md and write a learner walkthrough with
   file responsibilities, commands and remaining limits.

## Later human PR demonstration

Local success is not CI verification. The assistant will not commit, push,
merge or change protection for this task. After local completion, the human
can review and commit the correct implementation/test suite on this branch,
push it and open a PR. A separate deliberately broken implementation commit
can then demonstrate red CI and the required-check merge block. Keep test and
contract unchanged; capture the real failing run and blocked PR state; follow
with a normal fixing commit and retain both commits and run links. Do not
rewrite history. Confirm that all relevant jobs are required by repository
rules before claiming a failed check blocked merging.
