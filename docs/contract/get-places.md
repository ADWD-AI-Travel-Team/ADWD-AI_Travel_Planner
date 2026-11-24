# Places catalog API — contract draft, version 1

Status: proposed for team review; not a claim of implemented behavior.

### Sources and design provenance

- **IA#3 structure:** `docs/CONTRACT_IA#3.md` was read in full. This document
  retains its five numbered headings, an error catalog before resources, and
  endpoint-specific request, processing, response and idempotency rules. The
  additional sections provide review decisions and a future test matrix.
- **Assignment requirements:** the user's supplied PA#2 context and endpoint
  requirements establish public access, eligible Ho Chi Minh City catalog
  data, the four query parameters and their limits, strict query validation,
  offset pagination, RFC 9457 errors, and a read-only operation.
- **Team proposals:** parsed-query validation, item ID representation, fixture,
  error messages, response headers, and compatibility
  policy below are proposed design decisions, not attributed to the lecturer.
- The full Vietnamese brief, rubric, and the two Vietnamese learning guides
  were not available in this checkout. This draft uses the supplied prompt;
  it does not claim those missing documents were read.
- **This review:** reread this draft, `docs/CONTRACT_IA#3.md`, `AGENTS.md`, and
  `docs/AI_LOG/AI_LOG_PA2.md`, plus the user's review instructions. The user
  supplied the brief requirement that candidates need opening hours and
  menu/ticket data; that requirement is now explicit below. No additional brief
  or rubric file was available or read in this review. Earlier drafting history
  remains as recorded above. Framework parsing has not been experimentally
  verified in this documentation task; the verification boundary is stated in
  section 4, rather than claimed as an observed implementation result.

Once approved, this document is the specification. If implementation disagrees,
fix implementation. If the contract must change, review and record the contract
change first, then update tests and code in separately identifiable changes.
No API implementation or executable tests are added by this draft.

## 1. Conventions

### Purpose, scope and access

`GET /places` lists a public catalog of eligible places in Ho Chi Minh City.
It supplies known catalog candidates; it does not invent places, call a model,
or grant a model direct database access.

The caller may be anonymous. No role, login, bearer token, API key or group
membership is required. Catalog entries are shared public resources, so there
is no per-user ownership check. Absence of credentials must not cause a 401 or
403. An optional Authorization header does not select a different catalog or
grant access to private records; this route must remain anonymous even if
authentication is configured elsewhere.

The representation contains no personal information, visit history, private
plans, drafts, or internal verification evidence. IDs identify places, not
users. Full-text search, geospatial search, writes, bookings, payments and model
calls are outside this contract.

### Wire conventions

- The exact route is **`GET /places`**, without the IA#3 template's `/v1`
  prefix. The unversioned path is the initial contract surface, not a legacy
  endpoint. Other routes and methods are outside this document.
- JSON is UTF-8. Property names and enum values are case-sensitive.
- Integers are JSON numbers without a fractional component; never strings.
  Place IDs are positive signed 32-bit integers, unique within the catalog.
- All listed response properties are required and non-null. Clients ignore
  unknown response properties to permit additive evolution. Unlike the menu
  template, unknown **query** keys are rejected, not ignored.
- Application responses use `Cache-Control: no-store`. No ETag, conditional
  request or 304 behavior is promised. These choices keep this PA#2 interface
  simple; they do not establish a multi-request snapshot.
- A successful response has media type `application/json`; an application
  error has `application/problem+json`. Either may include `charset=utf-8`.
  Media-type assertions compare the parsed media type, not header formatting.

## 2. Errors

### Application error catalog

| `type` | HTTP status / body `status` | Stable `title` | Condition |
| --- | --- | --- | --- |
| `urn:travel-planner:problem:invalid-query` | 400 | `Invalid query` | Parsed query key, occurrence, syntax, range or supported-value rules fail. |
| `urn:travel-planner:problem:internal-error` | 500 | `Internal error` | An unexpected application/catalog failure prevents a valid response, including invalid catalog invariants. |

No other application problem type is defined in this draft. An empty collection
or a page beyond the end returns 200. Authentication and ownership errors are
not invented for this public route. There is no JSON request-body validation
error because this operation does not consume a request body.

The contract covers requests that reach the application route. TLS failures,
connection loss, proxy rate limits, server-level request-size limits, malformed
HTTP rejected before routing, and unrelated route/method errors are outside
this application error catalog. A client must still handle transport failures
and unexpected infrastructure responses defensively.

### Problem Details schema

RFC 9457 permits standard members to be omitted in some circumstances. This
**project convention requires** every application error here to contain all
five members below; that requirement is not attributed to the RFC itself.

| Member | JSON type | Project rule |
| --- | --- | --- |
| `type` | string | Exactly one absolute URN from the catalog above; identifies the error category. |
| `title` | string | Exactly the stable title paired with that type. |
| `status` | integer | Exactly the HTTP response status, 400 or 500. |
| `detail` | string | For 400: a nonempty, safe general explanation; wording is not guaranteed. For 500: exactly `An unexpected error occurred. Please try again later.` |
| `instance` | string | `/places`, a URI reference identifying the request path. Do not append the query or claim this is a unique correlation ID. |
| `errors` | object | Required only for 400; absent for 500. Maps keys exposed by the framework query collection to nonempty arrays of nonempty human-readable strings. |

The 400 `errors` object is nonempty. It is a dictionary, not the IA#3 template's
array of pointers. Keys use the spelling and case exposed by the framework
query collection. If the framework merges case variants, report the key exposed
for that group; clients must not expect an entry for each original spelling.
An empty parsed key is represented by the empty string key `""`. There is no
reserved whole-query error key or application decoding-error category.

Clients branch on `type` and `status`, and associate validation feedback using
the keys of `errors`. Titles are stable display defaults. Validation messages
and the 400 `detail` may change without changing semantics; clients and tests
must not compare them verbatim. The 500 `detail` is fixed and may be asserted
exactly. Dictionary order is not significant.

No response may expose raw rejected values, exceptions, stack traces, SQL,
connection strings, local file paths or secrets. A 500 always uses the fixed
generic detail above, regardless of the underlying failure. No `errors`
extension is returned for 500.

### Aggregation and precedence

Validation occurs before catalog access. Inspect **all parsed key groups**
exposed by ASP.NET Core and report all invalid groups in
one 400. For each key report exactly one message selected in this precedence:

1. Unknown key (including an empty key).
2. Repeated known key.
3. Empty value.
4. Invalid numeric syntax for a numeric parameter.
5. Numeric value outside its range, or unsupported enum value.

A duplicate key's individual values are not validated further. Unknown keys
are reported as unknown even if repeated. Valid parameters do not appear in
`errors`. Illustrative messages are `Unknown query parameter.`,
`Query parameter must occur once.`, `Value must not be empty.`,
`Value must contain ASCII digits only.`, `Value must be between 1 and 50.`, and
`Value must be one of: cafe, food, attraction.`

## 3. Resources

### Place

| Property | JSON type | Nullability and meaning |
| --- | --- | --- |
| `id` | integer | Required, non-null, 1–2147483647. Unique stable identity; not an array position. |
| `name` | string | Required, non-null, 1–200 Unicode scalar values; no leading/trailing whitespace or control characters. Public display name, not unique. |
| `category` | string | Required, non-null, exactly `cafe`, `food` or `attraction` in this draft. |
| `district` | string | Required, non-null, exactly `q1` or `q3` for the PA#2 fixture. A catalog code, not a translated district label. |

IDs must not be reassigned to different places during this contract's lifetime.
Only these four item properties are exposed by this draft. Internal eligibility
flags do not become public fields. Invalid eligible record data or duplicate
eligible IDs is a catalog invariant failure, producing the safe 500 rather
than an invalid 200 response. Ineligible records are excluded before public
record validation.

### PlacesPage

| Property | JSON type | Rule |
| --- | --- | --- |
| `items` | array of Place | Required, non-null; may be empty; at most `pageSize` elements, strictly increasing numeric IDs, no duplicates. |
| `page` | integer | Required, non-null; requested or default page, 1–100000, including when empty. |
| `pageSize` | integer | Required, non-null; requested or default size, 1–50. |
| `totalItems` | integer | Required, non-null; nonnegative signed 32-bit count after eligibility and filtering, before pagination. |
| `totalPages` | integer | Required, non-null; 0 when `totalItems` is 0, otherwise ceiling(`totalItems / pageSize`). |

Money, timestamps, order lines and historical prices from the IA#3 Resources
section are **Not applicable**: this read-only catalog representation contains
none of those fields.

### Reference fixture for all examples and future tests

The following is proposed deterministic test data, not an official dataset.
Names are intentionally synthetic. Flags simulate verification and eligibility;
they do not assert that any real venue was checked. Each included record also
meets the public field rules above.

| ID | Name | Category | District | Internal city | Verified | Eligible | Opening hours present | Menu/ticket data present |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | Fixture Cafe One | cafe | q1 | HCMC | true | true | true | true |
| 2 | Fixture Food Two | food | q1 | HCMC | true | true | true | true |
| 3 | Fixture Attraction Three | attraction | q1 | HCMC | true | true | true | true |
| 4 | Fixture Cafe Four | cafe | q3 | HCMC | true | true | true | true |
| 5 | Fixture Food Five | food | q3 | HCMC | true | true | true | true |
| 6 | Fixture Cafe Six | cafe | q1 | HCMC | true | true | true | true |
| 7 | Fixture Unverified Seven | cafe | q1 | HCMC | false | true | true | true |
| 8 | Fixture Ineligible Eight | food | q3 | HCMC | true | false | true | true |
| 9 | Fixture Outside City Nine | cafe | q1 | OTHER | true | true | true | true |
| 10 | Fixture Missing Hours Ten | cafe | q1 | HCMC | true | true | false | true |
| 11 | Fixture Missing Menu Eleven | food | q3 | HCMC | true | true | true | false |

The inclusion predicate requires **all** of: city `HCMC`, verified `true`,
published public catalog record, neither private nor draft, opening-hours data
present, menu/ticket data present as required by the brief, and internal
`eligible` not false. Missing opening hours or menu/ticket data always excludes
a record, even if `eligible` is true. An eligibility flag may represent or
further restrict these conditions; it cannot replace or override them.

All table rows are published, non-private, non-draft fixture records. Separate
private/draft variants are excluded regardless of their other fields. The
internal presence columns simulate the required catalog data; the fixture does
not claim real-world verification. Eligibility, publication, hours and menu
metadata stay internal and add no response properties. No freshness threshold
or verification workflow is specified. Clients never determine eligibility
from the public representation.

There are six included records. Tests should deliberately feed them to the
catalog in unsorted order. Official shared data is expected later; this draft
does not claim possession or verification of the official 150-place dataset.
Supporting additional district or category codes requires the compatibility
review in section 5; fixture assumptions must not silently become claims about
the official catalog.

## 4. Endpoints

### 4.1 `GET /places` — list public eligible places

#### Request headers and body

- No application-specific request header is required.
- `Accept: application/json` is recommended. Omission is supported. For this
  endpoint, representation is fixed by outcome regardless of Accept: success
  is JSON and errors are Problem Details; no application 406 is introduced.
- Request `Content-Type` is unnecessary. No body is defined or consumed; any
  received body is ignored by this operation, subject to server HTTP limits.
  Clients should send no body.
- Authorization, cookies, `Idempotency-Key`, and other headers do not alter this
  operation's data or query behavior. No replay header is emitted.

#### Query schema

All parameters are optional; defaults apply **only when the key is absent**.

| Key | Logical type / parsed lexical form | Default | Constraints |
| --- | --- | --- | --- |
| `page` | integer represented by one or more ASCII digits `[0-9]+` | 1 | Inclusive 1–100000. Leading zeroes are accepted; response uses the numeric value. |
| `pageSize` | integer represented by `[0-9]+` | 10 | Inclusive 1–50. Leading zeroes accepted. No clamping. |
| `category` | case-sensitive string | No category filter | Exactly `cafe`, `food`, `attraction`. |
| `district` | case-sensitive string | No district filter | Exactly `q1`, `q3` in this contract draft. |

#### Framework parsing and explicit validation requirements

Use ASP.NET Core's parsed request query collection for requests accepted by
the HTTP stack. Do not introduce a raw-query parser or strict UTF-8 decoder.
Do not decode the framework's keys or values a second time. Malformed-encoding
handling is delegated to the framework/server; this contract does not promise
a specific application status or error location for such raw input.

1. Enumerate the parsed keys and their full value collections before binding
   scalar endpoint arguments. Match each exposed key spelling using ordinal,
   case-sensitive equality to `page`, `pageSize`, `category`, `district`.
   `Page` alone is unknown; default model binding is not the validation policy.
2. Reject any known key group with more than one value, even if the values are
   identical. Do not accept only the first/last value or join values into a
   scalar. Some query collections compare keys without regard to case. If
   `Page=1&page=2` is merged, it must still be rejected: an exposed `Page` is
   unknown, while an exposed `page` with two values is a duplicate. Apply the
   same rule to `page=1&Page=2`, independent of which spelling is retained.
3. Reject unknown keys, empty values (including a present key with no values)
   and invalid parsed values. Do not trim,
   case-fold, normalize Unicode, ignore invalid filters or replace invalid
   values with defaults. Defaults apply only to an absent key.
4. Apply the error precedence in section 2. Numeric bounds are checked without
   overflowing: digit sequences representing values above the bound, however
   long within accepted HTTP limits, are range errors, not 500 errors. Leading
   zeros do not change value, even when there are many leading zeros.

At the parsed-value boundary, ` 1`, `+1`, `1 `, `-1`, `1.0`, `1e2` and non-ASCII
digits are invalid numeric syntax. `0` is out of range; `01` means numeric 1.
`Cafe`, `cafe ` and a literal `%63afe` are unsupported category values. Do not
perform application decoding to turn the last value into `cafe`.

**Verification still needed before implementation is accepted:** confirm the
.NET 10 query collection's exposed spelling and preservation of value counts
for same-case and mixed-case repetitions, in both orders. If a middleware
discards occurrences, it must not be used as this validation boundary. Also
characterize ordinary percent-encoded names/values, raw plus versus encoded
plus, bare keys, and empty separators using the chosen server/test host. This
review did not run those checks, so raw encoding/separator edge cases below
are characterization tasks, not invented acceptance guarantees. No custom
parser is required to preserve the previous draft's edge cases. HTTP/server
rejection before routing does not require application Problem Details.

#### Processing, sorting and pagination

For a syntactically valid query, use one coherent catalog read per response:

1. Select only records satisfying every inclusion condition in section 3,
   including opening hours and menu/ticket data; flags cannot bypass missing
   data. Excluded records affect neither returned items nor totals.
2. Validate included public records and unique IDs; a broken invariant is 500.
3. Apply category and district filters using exact equality; combine with AND.
4. Compute `totalItems` from that filtered set.
5. Sort by **numeric ID ascending**, not string comparison, input order, name
   or current culture. Unique IDs make this a total, stable order.
6. Let `offset = (page - 1) * pageSize`; take at most `pageSize` records after
   skipping `offset`. Return metadata using the same filtered set.

A valid page beyond the end returns 200 and `items: []`, preserving requested
`page` and `pageSize`; totals still describe all matching records. An empty
filtered set has both totals zero, regardless of the requested page.

Each response must have mutually consistent items and totals. There is no
snapshot token or cursor. Catalog changes between requests can cause records
to shift, disappear, repeat across pages, or change totals. For the same
unchanged snapshot and normalized query, repeated calls return equal JSON
values and two consecutive pages do not overlap. JSON whitespace and object
property ordering are not promised byte-for-byte identical.

#### Success and error examples

Every 200 example below has `Content-Type: application/json` and
`Cache-Control: no-store`. Error examples have `Cache-Control: no-store` too.
All examples use the reference fixture without concurrent modification.

**A. Defaults — `GET /places`**, HTTP 200:

```json
{
  "items": [
    { "id": 1, "name": "Fixture Cafe One", "category": "cafe", "district": "q1" },
    { "id": 2, "name": "Fixture Food Two", "category": "food", "district": "q1" },
    { "id": 3, "name": "Fixture Attraction Three", "category": "attraction", "district": "q1" },
    { "id": 4, "name": "Fixture Cafe Four", "category": "cafe", "district": "q3" },
    { "id": 5, "name": "Fixture Food Five", "category": "food", "district": "q3" },
    { "id": 6, "name": "Fixture Cafe Six", "category": "cafe", "district": "q1" }
  ],
  "page": 1, "pageSize": 10, "totalItems": 6, "totalPages": 1
}
```

**B. Filters and pagination —
`GET /places?category=cafe&district=q1&page=2&pageSize=1`**, HTTP 200:

```json
{
  "items": [
    { "id": 6, "name": "Fixture Cafe Six", "category": "cafe", "district": "q1" }
  ],
  "page": 2, "pageSize": 1, "totalItems": 2, "totalPages": 2
}
```

**C. Beyond the last page — `GET /places?page=4&pageSize=2`**, HTTP 200:

```json
{ "items": [], "page": 4, "pageSize": 2, "totalItems": 6, "totalPages": 3 }
```

**D. No matches — `GET /places?category=attraction&district=q3`**, HTTP 200:

```json
{ "items": [], "page": 1, "pageSize": 10, "totalItems": 0, "totalPages": 0 }
```

**E. Several invalid keys — `GET /places?page=0&pageSize=&category=Cafe&sort=id`:**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
Cache-Control: no-store

{
  "type": "urn:travel-planner:problem:invalid-query",
  "title": "Invalid query",
  "status": 400,
  "detail": "One or more query parameters are invalid.",
  "instance": "/places",
  "errors": {
    "page": ["Value must be between 1 and 100000."],
    "pageSize": ["Value must not be empty."],
    "category": ["Value must be one of: cafe, food, attraction."],
    "sort": ["Unknown query parameter."]
  }
}
```

**F. Catalog exception during a valid `GET /places`:**

```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/problem+json
Cache-Control: no-store

{
  "type": "urn:travel-planner:problem:internal-error",
  "title": "Internal error",
  "status": 500,
  "detail": "An unexpected error occurred. Please try again later.",
  "instance": "/places"
}
```

#### Retry and idempotency

GET is read-only: it does not create, modify, publish, reserve, charge, or start
an agent run. Ordinary operational logging is not a catalog mutation. There is
no requirement for `Idempotency-Key`, key storage, request fingerprints,
transactions for replay, replay status, replay header, or retention period.
Those mechanisms from the IA#3 order endpoint are **Not applicable** because
this operation has no creation or publication effect to deduplicate.

Clients may retry transport failures and 500 responses using a bounded retry
policy with backoff. Do not blindly retry an unchanged invalid query after 400;
correct the query. Retry does not pin a snapshot: an unchanged catalog gives
equal successful values, while a changed catalog may give a different result.

Other menu/order endpoints in the IA#3 Endpoints section are **Not applicable**;
this draft defines only this one collection operation.

## 5. Versions and the legacy API

This heading is inherited from IA#3. A pre-existing legacy Places API is
**Not applicable**: none is defined by the supplied task. No fictional migration,
historical behavior, deprecation date or Sunset deadline is asserted.

Proposed compatibility policy for approval:

- Compatible changes include new response properties clients can ignore,
  editorial corrections that do not change semantics, and new optional query
  keys whose absence preserves existing behavior. Existing unknown keys remain
  invalid until a reviewed contract revision explicitly supports them.
- Breaking changes include removing/renaming required properties; changing
  types, nullability, path, defaults, bounds, parsing, sort, eligibility semantics,
  error type/status or error structure; and turning the public route into an
  authenticated route. Adding an enum value to responses is treated as breaking
  because clients may exhaustively interpret these documented enums.
- A new error type for an existing request path requires compatibility review
  and new matrix cases; it is not automatically additive merely because the
  IA#3 menu sample classified new problem types that way.
- Normal catalog additions, removals and name corrections within the existing
  schema and eligibility policy are data changes, not API-version changes.
- A breaking revision is published under a new explicit route such as
  `/v2/places`, with a separately reviewed contract. Keep `/places` behavior
  unchanged during migration. Announce a real deprecation date, migration guide
  and removal date before retirement, with at least 90 days' notice. At that
  time send `Deprecation`, `Sunset`, and a successor-version `Link` header on
  old-route responses; dates and URLs must match the approved announcement.
- This draft does not currently emit deprecation headers. Changes to contract,
  tests and implementation must remain traceable; do not rewrite history to
  manufacture evidence of a failing check.

## 6. Assumptions, limits and decisions for team approval

The baseline page limits, enum spellings and strict unknown/duplicate/empty
rules from the prompt are retained. No baseline value was silently changed.
Review the following proposals before freezing this draft:

1. Integer IDs and synthetic fixture. Opening hours and menu/ticket presence
   are brief requirements supplied during review, not optional team proposals.
2. Framework query parsing with explicit spelling/value-count validation,
   accepted leading zeros, and one error per invalid parsed key group with the
   documented precedence. Complete the parser characterization listed above.
3. Fixed representation despite Accept, ignored request body/irrelevant headers,
   and `Cache-Control: no-store` to avoid adding unrelated problem types.
4. Safe nonfixed 400 detail, fixed generic 500 detail, and one coherent read per
   response without a cross-request snapshot guarantee.
5. Versioning and the proposed 90-day deprecation notice. No deployed legacy API
   or actual deprecation schedule is assumed.

The PA#2 fixture is intentionally smaller than the official dataset and uses
only q1/q3. Before production data is introduced, reconcile supported districts,
categories and eligibility with the official product rules through a reviewed
contract change. This document does not define how storage, caching, real data
verification, or authentication elsewhere must be implemented.

## 7. Acceptance / future contract-test matrix

These are planned cases, not tests written or executed in this task. Unless
stated otherwise, use the reference fixture in shuffled input order, no login,
and a fresh controlled catalog. Each semicolon-separated request variant is a
separate test input. `IQ` means `urn:travel-planner:problem:invalid-query`; `IE`
means `urn:travel-planner:problem:internal-error`.

**Common assertions for every application response:** media type and no-store;
valid JSON; required properties and types; no secret/exception leakage.
For every 200: full Place field rules, requested/default page metadata, exact
expected IDs and totals, no nulls or duplicate IDs. For every error: full
Problem Details schema, exact type/title/status, HTTP/body status agreement and
`instance=/places`. For every 400: exact invalid-key set, one nonempty message
per exposed invalid key group, no valid-key errors; safe nonempty 400 detail
without matching it verbatim. For every 500: exactly the generic detail in
section 2, no `errors`. Do not match human validation sentences or JSON property
order verbatim. Encoding characterization rows do not promise application
status for malformed input rejected or transformed by the framework/server.

| ID | Request / controlled condition | Status | Problem type | Additional assertions |
| --- | --- | --- | --- | --- |
| GP01 | `/places`, no Authorization/cookie | 200 | — | IDs 1–6, page 1, size 10, totals 6/1; no challenge or redirect. |
| GP02 | `/places?page=1&pageSize=10` | 200 | — | Equal JSON values to GP01 on same fixture. |
| GP03 | `/places?category=cafe` | 200 | — | IDs [1,4,6], totals 3/1. |
| GP04 | `/places?district=q3` | 200 | — | IDs [4,5], totals 2/1. |
| GP05 | `/places?category=cafe&district=q1&page=2&pageSize=1` | 200 | — | ID [6], totals 2/2; filters use AND. |
| GP06 | `/places?page=1&pageSize=2`, then page 2 | 200 | — | [1,2] then [3,4]; no overlap; totals 6/3 in both. |
| GP07 | `/places?page=1&pageSize=1`; `/places?page=100000&pageSize=50` | 200 | — | First: [1], totals 6/6. Second: [], page 100000, size 50, totals 6/1. |
| GP08 | `/places?page=4&pageSize=2` | 200 | — | Empty page, requested page 4, totals 6/3. |
| GP09 | `/places?category=attraction&district=q3`; same with `page=5` | 200 | — | [], totals 0/0; page respectively 1/5. |
| GP10 | Empty catalog, `/places` | 200 | — | [], totals 0/0, defaults preserved. |
| GP11 | Fixture plus private/draft variants of otherwise valid rows | 200 | — | IDs 7–11 and private/draft variants absent; total remains 6. No internal or personal fields. |
| GP12 | `/places?sort=id`; `/places?Page=1`; empty key exposed by parsed query collection | 400 | IQ | Error key respectively sort, Page, empty string. |
| GP13 | `/places?page=1&page=1`; `/places?category=cafe&category=food`; `Page=1&page=2`; `page=1&Page=2` | 400 | IQ | Repetition/mixed spelling rejected in both orders, never silently accepted after case-insensitive grouping. Use exposed group key in errors. Parameterize all known keys. |
| GP14 | `/places?page=`; `/places?pageSize=`; `/places?category=`; `/places?district=`; parsed present key with no values | 400 | IQ | Present-empty never becomes a default. Bare-key transport behavior is characterized in GP21. |
| GP15 | Parsed `page` values `abc`, `1.0`, `1e2`, `-1`, non-ASCII digits; same for `pageSize` | 400 | IQ | Correct numeric parameter key; ASCII digit grammar enforced. Encode request values using verified framework behavior. |
| GP16 | `page` values `0`, `100001`, a 100-digit integer; `pageSize` values `0`, `51`, a 100-digit integer | 400 | IQ | Range failure; no clamp, overflow or 500. |
| GP17 | `category` values `Cafe`, `museum`; `district` values `Q1`, `q2` | 400 | IQ | Exact enum validation; no silently ignored filters. |
| GP18 | Parsed page values ` 1`, `+1`, `1 `; parsed category value `cafe ` | 400 | IQ | No trimming or permissive conversion; reject actual parsed values. |
| GP19 | `/places?page=01&pageSize=01` | 200 | — | ID [1], page/size 1/1, totals 6/6; no special parser needed. |
| GP20 | Parsed category value literal `%63afe` or `cafe;district=q1` | 400 | IQ | Unsupported category; application does not decode again or parse a filter inside a value. |
| GP21 | Characterize chosen framework/host: encoded key/value, raw plus, encoded plus, malformed encoding, bare key, empty separators | Characterization only | Not predefined | Record actual parsed keys/counts/values and HTTP boundary. Do not assert a new decoding problem type or build a custom parser. For accepted parsed input, apply normal validation rules. |
| GP22 | Example E; `/places?page=&page=x&district=Q1&other=1` | 400 | IQ | All invalid keys returned. Second has page, district, other; one page duplicate message, not empty/syntax messages. |
| GP23 | Repeated unknown key; parsed key ` page` | 400 | IQ | Unknown-key precedence, exposed key retained; no raw values echoed. |
| GP24 | Same valid request twice on unchanged fixture, with/without `Idempotency-Key` | 200 | — | Equal JSON values, no catalog mutation or replay header. |
| GP25 | `/places` with irrelevant Authorization header; missing Accept; `Accept: text/plain`; ignored body | 200 | — | Same public JSON result, no auth error, 406 or body validation error. Test separately. |
| GP26 | Inject catalog fake that throws for valid `/places` | 500 | IE | Safe fixed error; injected exception text and secret sentinel absent. No real service call. |
| GP27 | Duplicate eligible ID; invalid eligible id/name/category/district; null required field | 500 | IE | Catalog invariant error, never malformed 200. Parameterize ID bounds, name length/whitespace/control characters, enums and nullability. |
| GP28 | Invalid query with a catalog fake that would throw | 400 | IQ | Fake never invoked; validation takes precedence over catalog access. |
| GP29 | Numeric ordering fixture with eligible IDs [10,2,1] | 200 | — | IDs [1,2,10], not lexicographic [1,10,2]. |
| GP30 | Controlled catalog mutation between page requests | 200 | — | Each response internally coherent; do not assert cross-request no-repeat/snapshot guarantees. |
| GP31 | `/places` and `?category=cafe&district=q1` with row 10 present, eligible true but opening hours missing | 200 | — | ID 10 excluded from items and totalItems: unfiltered total 6, filtered IDs [1,6], total 2; pages computed from those totals. |
| GP32 | `/places` and `?category=food&district=q3` with row 11 present, eligible true but menu/ticket data missing | 200 | — | ID 11 excluded from items and totalItems: unfiltered total 6, filtered ID [5], total 1; pages computed from those totals. |

For GP26 inject a throwing fake through the test host's catalog dependency.
Do not add a production debug endpoint, real database outage, external API call
or model call to generate the error. Later fault-demonstration commits must be
explicitly approved and must change implementation rather than weakening the
contract/test expectations; none are created by this documentation task.
