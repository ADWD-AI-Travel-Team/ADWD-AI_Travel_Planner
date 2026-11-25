# .NET 10 query parser observations

Observed locally on 2026-11-25 using SDK 10.0.401, runtime .NET/ASP.NET Core
10.0.12, and Microsoft.AspNetCore.Mvc.Testing/TestHost packages 10.0.15.
The package restored, the project compiled, and HTTP TestServer tests actually
executed. This establishes compatibility in this environment, not every future
runtime/host configuration.

`QueryParserCharacterizationTests` creates an isolated test-only WebApplication
and sends HTTP through TestServer. Its query-echo route exists only in test code;
no diagnostic endpoint was added to the product application. Endpoint contract
tests separately use WebApplicationFactory<Program> to exercise the real app.

## Observed cases

| Raw request query | Exposed key | Exposed values |
| --- | --- | --- |
| `?page=1&page=2` | `page` | `1`, `2` |
| `?Page=1&page=2` | `page` | `1`, `2` |
| `?page=1&Page=2` | `Page` | `1`, `2` |
| `?%70age=%31` | `page` | `1` |
| `?page=+1` | `page` | ` 1` (leading space) |
| `?page=%2B1` | `page` | `+1` (literal plus) |
| `?page` | `page` | One empty string |
| `?&&` | No key | Empty collection |
| `?category=%2563afe` | `category` | `%63afe` |
| `?category=%GG` | `category` | `%GG` |
| `?category=%` | `category` | `%` |
| `?=x` | Empty string | `x` |
| `?category=cafe;district=q1` | `category` | `cafe;district=q1` |

For these mixed-case examples, the framework retained the **last spelling** and
all values in a single group. The original characterization data incorrectly
guessed first spelling; two observation tests failed and were corrected to
record the actual framework behavior. No approved contract expectation was
changed: either exposed noncanonical spelling or duplicate values must still
cause a 400 in the real endpoint.

## Implementation consequences

- Enumerate Request.Query, compare exposed key spelling using ordinal exact
  matching, and check value count before converting a scalar.
- Unknown keys fail first. Known repeated keys fail next. Never silently take
  a first or last value. Both mixed-case orders above return application 400.
- Do not decode a parsed value again. A literal percent sequence in a category
  remains an unsupported category rather than being repaired by the endpoint.
- Do not trim parsed values. Space or plus in a numeric value fails the ASCII
  digit grammar. Bare keys fail the present-empty rule.
- Do not introduce a strict decoder or new whole-query application problem.
  Malformed sequences retained by this host become ordinary invalid enum
  values when applicable. Other hosts can reject malformed input earlier.

These observations do not claim that Kestrel, a reverse proxy, a browser, or a
different HttpClient will preserve every malformed request target identically.
In particular, arbitrary invalid UTF-8 was not characterized here. Infrastructure
rejections before routing remain outside the application's Problem Details
guarantee. The approved contract file is unchanged.

Run the observations again with detailed output:

```sh
dotnet test tests/AITravelPlanner.ContractTests/AITravelPlanner.ContractTests.csproj --configuration Release --filter FullyQualifiedName~QueryParserCharacterizationTests --logger "console;verbosity=detailed"
```
