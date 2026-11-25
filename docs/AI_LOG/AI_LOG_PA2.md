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

