# AI-LOG.md

## 2026-11-24 — AI assistant permission policy (PA#2)

**Tool:** ChatGPT (GPT).

**Asked for:** Draft and improve `AI_PERMISSION.md` by organizing the AI coding assistant's permissions into three levels: Allowed, Ask First, and Never.

**Kept:** The three-tier permission structure and rules covering repository access, running tests, Git operations, dependency changes, database migrations, CI configuration, production deployments, and production secrets.

**Changed:** Reviewed the generated Never list and removed three rules: sending emails to real users or charging customers, modifying tests/evaluation data/validator rules just to pass checks, and making real model API calls in unit tests. These were removed from this permission document based on my team's requested scope. I still need to ensure that other project rules and the course requirements cover relevant safety and testing practices.

**Rejected:** Did not keep the generated permission policy exactly as written. I reviewed and adjusted the restrictions to match the team's intended permission boundaries rather than accepting the AI output without review.

## 2026-11-24 — GitHub Actions CI base

**Tool:** AI assistant.

**Asked for:** Add a baseline GitHub Actions CI workflow for the project.

**Kept:** `.github/workflows/ci.yml` with separate .NET 10 and frontend jobs. The workflow runs on pushes, pull requests, and manual dispatch; it restores, builds, and tests the .NET solution, then installs frontend dependencies and runs lint and production build.

**Validation:** The .NET Release build completed with no warnings or errors, and the test command succeeded. Frontend lint and production build also succeeded. There are currently no test projects in the solution.

**Accepted:** The user explicitly accepted the CI workflow response and asked to record it in this log.

## 2026-11-24 — GET /places contract draft

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** API contract documentation.
- **Task given:** Read the supplied IA#3 contract and draft a reviewable English GET /places contract with strict query validation, public access, deterministic fixture examples, pagination, RFC 9457 errors, compatibility rules and a future contract-test matrix. Do not implement API code or tests.
- **What the assistant produced:** `docs/contract/get-places.md`, preserving all five IA#3 numbered headings and marking unrelated menu/order mechanisms as not applicable. The draft separates template structure, supplied assignment requirements and proposed team decisions; includes six consistent HTTP examples and thirty matrix case groups. No claim is made that the unavailable full Vietnamese brief/rubric or guides were read.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Documentation review checked parsing precedence, framework validation requirements, eligibility, numeric sorting, totals and page examples against the declared fixture, and coverage of both application error types. No executable tests were written or run for this documentation task.
- **Evidence:** Local draft `docs/contract/get-places.md` and source template `docs/CONTRACT_IA#3.md`; no commit or PR has been created for this task yet.
- **Led to rules-file line:** none.

## 2026-11-24 — GET /places contract review revisions

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** API contract review and documentation.
- **Task given:** Make opening-hours/menu eligibility explicit, distinguish nonfixed 400 detail from fixed 500 detail, replace custom decoding requirements with framework query parsing, and review consistency without implementing API code or tests.
- **What the assistant produced:** Revised `docs/contract/get-places.md`, retaining proposed-for-review status and the five IA#3 headings. Added missing-hours/menu fixture records and planned cases GP31/GP32 while preserving the six included records and existing response examples. Updated validation, mixed-case duplicate handling, error assertions and source provenance. GP21 now records pending framework characterization rather than a decoding error contract.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** The user's review identified incomplete eligibility conditions, contradictory detail stability rules and unnecessary parser complexity. Document checks compare the fixture, examples, metadata, inherited headings and planned error cases; framework parser behavior and executable contract tests have not been verified in this documentation task.
- **Evidence:** Local revised `docs/contract/get-places.md`; source template remains `docs/CONTRACT_IA#3.md`. No commit or PR has been created for this revision.
- **Led to rules-file line:** none.

## 2026-11-25 — Complete quality-check scripts and type checking

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Check scripts and documentation.
- **Task given:** Fill the four empty files under scripts to complete type checking and record the work in this log.
- **What the assistant produced:** Four Bash scripts that resolve the repository location and propagate command failures. Type checking builds the C# solution in Release with warnings as errors, then runs the existing local TypeScript compiler against both referenced frontend configurations without Vite. Lint runs C# analyzers and frontend oxlint; format checks C# whitespace; tests run the existing .NET suite with empty-test-run failure requested. Added README commands, scope and setup instructions, plus LF checkout rules for shell scripts. No dependencies or CI workflow were changed.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Installed existing locked frontend dependencies with npm ci. Executed all four scripts using Git Bash: types passed with zero C# warnings/errors, lint passed, and all four existing architecture test cases passed. Format correctly failed with WHITESPACE at src/WebApi/Program.cs:24 because of an existing extra space; no application source or test was modified to conceal that failure. Git Bash required execution outside the assistant sandbox because CreateFileMapping was denied inside it. TypeScript checking exited successfully. These results do not claim CI/local parity; wiring the scripts into CI remains separate.
- **Evidence:** Local scripts/check-types.sh, scripts/check-lint.sh, scripts/check-format.sh, scripts/check-tests.sh, README.md and the actual command results from this session. No commit or PR has been created for this task yet.
- **Led to rules-file line:** none.

## 2026-11-25 — Align local checks and CI; verify a fresh clone

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Quality checks, CI, setup documentation and verification.
- **Task given:** Assess whether one documented command per check runs identically locally and in CI, complete the missing work, explain the flow and record real evidence.
- **What the assistant produced:** Assessment found that CI bypassed the scripts, formatting failed on existing whitespace, and fresh-clone instructions were incomplete. The implementation flow was: connect four independent CI jobs to the existing four scripts; fix the source whitespace; align README and agent commands; pin the SDK; commit on a feature branch; clone into a new directory; run documented setup/checks; push the branch and inspect actual CI. README now documents prerequisites, branch selection, dependency installation, all four commands and their exact coverage. Added global.json selecting SDK 10.0.401. Updated the clone URL after GitHub reported the repository's move to ADWD-AI-Travel-Team. No test, validator or contract rule was weakened. Branch protection remains a separate owner action.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Compared README, AGENTS.md, scripts and workflow before editing. A fresh local clone of commit c27a64c, using Node 24.22.3 and SDK 10.0.401, installed locked frontend dependencies and passed all four scripts: lint, format, type checking (zero C# warnings/errors and successful TypeScript check), and four architecture test cases. The clone reused machine NuGet caches; it was a new checkout without copied project build output. Temporary Node tooling was installed outside the source repository for verification. GitHub Actions independently passed all four jobs on Ubuntu. No full frontend formatter or frontend test runner is claimed; format is C# whitespace and tests are the existing .NET tests.
- **Evidence:** Implementation commit `c27a64c`; successful CI run https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45547596703 on branch feature/pa2-check-parity. Four documented commands: `bash scripts/check-lint.sh`, `bash scripts/check-format.sh`, `bash scripts/check-types.sh`, `bash scripts/check-tests.sh`.
- **Led to rules-file line:** `Repository type check (C# and TypeScript): bash scripts/check-types.sh` and matching repository check commands in AGENTS.md replace unavailable frontend-only script names; this corrects documentation drift observed during inspection.

## 2026-11-25 — Publish documentation through a protected-branch PR

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Git workflow and documentation publishing.
- **Task given:** Explain the GH013 rejection of a direct main push and publish the existing changes correctly.
- **What the assistant produced:** Inspected the user's commit 70579bf, which adds the IA#3 contract reference and the local knowledge ignore rule. Fetched current remote state, created feature/add-ia3-contract-reference at that commit, updated origin to the repository's new organization URL, and pushed the feature branch successfully. The GitHub rejection explicitly requires a pull request; that rule is separate from CI configuration. The identity of the person who enabled the rule was not established. No protection setting, test or contract rule was changed, and no history was rewritten.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** The user's git push output reported GH013 with 'Changes must be made through a pull request.' Git inspection confirmed that local main was one documentation commit ahead of origin/main and the working tree was clean before this task. Pushing the new branch succeeded.
- **Evidence:** Existing documentation commit `70579bf`; remote branch `feature/add-ia3-contract-reference` at https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/tree/feature/add-ia3-contract-reference. PR creation is handled after this log entry; no PR number is invented here.
- **Led to rules-file line:** none.

## 2026-11-25 — Prepare GET /places implementation and HTTP contract tests

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Implementation planning and baseline verification.
- **Task given:** Implement the approved Places contract with layered code and real HTTP contract tests; first inspect main, create a new branch, map requirements and obtain permission for new dependencies. Do not commit or push.
- **What the assistant produced:** Created feature/places-contract-implementation from origin/main at 47f9e2e after fetching and comparison. Added docs/contract/get-places-implementation-plan.md with existing/missing components, file responsibilities, GP coverage mapping, unresolved framework observations and a proposed test-host dependency. Implementation and HTTP tests are pending dependency approval; no API code, test assertions or approved contract was changed at this stage.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Executed the existing check-tests.sh against the new main baseline: four architecture cases ran, three passed and Infrastructure failed because commit 1de27ce removed a direct Domain reference still required by the test. This is an existing architecture disagreement, not an endpoint regression or sandbox failure. The plan proposes restoring the implementation reference rather than weakening the architecture test.
- **Evidence:** Remote main 47f9e2e; pre-existing reference change 1de27ce; local plan docs/contract/get-places-implementation-plan.md; actual baseline test result from this session. No commit, push or CI verification was performed for this task.
- **Led to rules-file line:** none.

## 2026-11-25 — Implement GET /places and verify HTTP contract tests

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Layered endpoint implementation, HTTP tests and verification.
- **Task given:** After explicit approval for Microsoft.AspNetCore.Mvc.Testing 10.0.15 and restoration of Infrastructure's Domain reference, repair baseline, add the test project, observe framework parsing, implement the approved contract, run quality checks and a temporary implementation-only violation. Do not commit, push or change branch protection.
- **What the assistant produced:** Domain Place eligibility/public-record invariants; Application catalog abstraction, query/response DTOs and list use case; Infrastructure empty/default and explicitly opted-in synthetic development adapters; anonymous WebApi route with explicit parsed-query validation, no-store and safe 400/500 Problem Details. Added the HTTP test project to the solution, retaining existing CI/scripts. Added implementation/file-responsibility guide, GP coverage mapping, parser observations and instructions for a later human PR red/blocked/fixed/green demonstration. The approved contract and existing architecture tests are unchanged.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Restoring the approved direct reference made all four old tests pass. Package 10.0.15 restored, built and actually hosted requests on SDK 10.0.401/runtime 10.0.12. Thirteen parser cases observed merged mixed-case keys retaining last spelling and all values; two initial first-spelling guesses in characterization data were corrected to the observed behavior, not to weaken endpoint requirements. The gate caught analyzer issues and Windows encoding corruption in Unicode fixture literals; typed/serializable theory data, reusable arrays and explicit Unicode escapes resolved them without suppressions or weakening assertions. A temporary ascending-to-descending sort mutation made the real-app HTTP numeric-sort case fail (expected [1,2,10], actual [10,2,1], test process exit 1); finally restored original source bytes, leaving contract/tests unchanged during that experiment. Final lint, format, typecheck and test scripts all passed with Node 24.22.3; build had zero warnings/errors. Final tests: 4 architecture + 133 new-project cases = 137 passed, zero failed/skipped. The new project contains 116 HTTP contract cases, 4 fixture-configuration HTTP cases and 13 parser characterization cases. No real model, production service or secret was used.
- **Evidence:** Local src/Domain/Places, src/Application/Places, src/Infrastructure/Places, src/WebApi/Places, tests/AITravelPlanner.ContractTests and solution changes; actual command output from this session; docs/implementation/get-places.md and docs/implementation/get-places-parser-observations.md. No commit/PR/CI link exists for this task because no commit or push was performed. Local mutation evidence does not claim a remote blocked merge.
- **Led to rules-file line:** none.

