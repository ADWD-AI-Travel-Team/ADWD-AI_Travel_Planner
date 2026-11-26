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


## 2026-11-25 — Diagnose required checks waiting despite successful CI

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Read-only GitHub CI/ruleset diagnosis and logging.
- **Task given:** Investigate four required checks stuck at Expected/Waiting while actual workflow jobs succeed; do not change branch protection.
- **What the assistant produced:** Compared local workflow and GitHub API check-run names with effective main rules. Ruleset 28039538 requires contexts CI / lint, CI / format, CI / typecheck and CI / test, while GitHub Actions reports lint, format, typecheck and test. This name mismatch explains the expected checks never being satisfied. Recommended an owner update only the required context selections to the existing job names while keeping all four checks required. No workflow, test, implementation, contract or repository setting was changed.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** GET-only API inspection showed PR #9 at head b8275ff3fce145fec6d437f72b4cb64bb38ecafd with all eight push/pull_request check runs successful but mergeable_state blocked. Effective rules on main listed four differently named required contexts. This is not evidence of an intentional contract-test regression blocking merge. PR #8 separately has a genuinely failing test check and is outside the current contract-test demonstration.
- **Evidence:** PR https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; successful CI run https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45637930466 ; repository ruleset ID 28039538 from the effective main rules API. No fix to settings has been performed or verified yet. No commit or push performed.
- **Led to rules-file line:** none.

## 2026-11-25 — Trace workflow naming history and verify corrected rule names

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** CI configuration diagnosis.
- **Task given:** Determine whether required-check name mismatches came from a teammate's configuration and a later workflow rename.
- **What the assistant produced:** Compared both tracked ci.yml revisions and read current ruleset 28039538. Initial workflow e3cd19b used jobs .NET and Frontend. Commit c27a64c replaced them with lint, format, typecheck and test during the assistant's earlier implementation task. Neither tracked version emitted job names CI / lint, CI / format, CI / typecheck or CI / test. The prefix mismatch therefore is not demonstrated to be a rename from those prefixed job names; confusion with UI display labels remains an inference. The API does not identify who selected the earlier required contexts.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Read git history and diffs for ci.yml, then GET ruleset metadata and current required contexts. The active Protect Main ruleset now requires typecheck, format, lint and test with GitHub Actions integration, matching the actual reported job names. Its updated_at was 2026-11-25T21:56:37.498+07:00. No settings were changed by the assistant. PR mergeability after this update was not checked in this task.
- **Evidence:** Commits e3cd19b and c27a64c; GitHub API ruleset 28039538 under ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner. No commit or push performed for this diagnosis.
- **Led to rules-file line:** none.

## 2026-11-25 — Stage intentional ordering violation for the human PR demonstration

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Contract-gate evidence, local intentional failure.
- **Task given:** After explicit approval, change ascending place ordering to descending, run the targeted HTTP test and leave the violation in place for the human to commit/push. Do not modify contract/tests or perform Git publishing operations.
- **What the assistant produced:** Changed only the sort call in src/Application/Places/ListPlaces.cs from OrderBy to OrderByDescending. Left the intentional violation present. Contract and test files are unchanged. The human previously confirmed saving the green baseline screenshot and supplied PR #9, baseline b8275ff3fce145fec6d437f72b4cb64bb38ecafd and green CI run 45637930466.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Ran dotnet test with the FullyQualifiedName filter for Numeric_sort_and_changed_catalog_have_coherent_per_response_totals. One real-app HTTP test ran and failed, zero passed/skipped, process exit code 1. Its assertion expected [1,2,10] and received [10,2,1]. This is an intentional ordering-contract violation, not a build or environment failure. Remote red CI and blocked merge have not yet been observed for this mutation.
- **Evidence:** Local implementation diff and test output from this session. Existing PR https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; green baseline run https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45637930466 . No mutation commit hash or red-run link exists yet; the assistant has not committed or pushed.
- **Led to rules-file line:** none.

## 2026-11-25 — Restore correct ordering after preserving red-check evidence

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Contract-gate demonstration, repair and local verification.
- **Task given:** Continue the staged human-controlled demonstration after the human confirmed saving the HTTP failure screenshot and the required-check blocked-merge screenshot. Restore only the intentional implementation violation and run all local checks; do not commit or push.
- **What the assistant produced:** Replaced OrderByDescending with OrderBy in src/Application/Places/ListPlaces.cs. The implementation file, approved contract and all test files match green baseline b8275ff. No assertion, architecture rule, workflow or branch protection was changed. The human reports saving contract-test-red.png and required-check-blocked.png; those files have not been inspected or added to the repository by the assistant.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** GitHub API verified job 139319509892 failed on intentional violation commit 11d5ba76d260a4a067610a5ff2a749982d0f6be1; run 45646699706 is a push run. PR #9 was open with mergeable_state blocked, and the active rule required test plus lint/format/typecheck from GitHub Actions. The human separately confirmed the blocked-merge UI screenshot. After local restoration, all four quality-check scripts passed with Node 24.22.3 and SDK 10.0.401: zero build warnings/errors, 4 architecture tests plus 133 new-project cases passed, none failed/skipped. A remote fixing commit and green fix run do not exist yet; the human will create and push the fix.
- **Evidence:** PR https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; baseline b8275ff3fce145fec6d437f72b4cb64bb38ecafd ; violation 11d5ba76d260a4a067610a5ff2a749982d0f6be1 ; red job https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45646699706/job/139319509892 ; local repair diff and successful check outputs. Screenshot filenames are human-reported local evidence, not repository links.
- **Led to rules-file line:** none.

## 2026-11-25 — Preserve final contract-gate evidence during GitHub PR update delay

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Submission evidence and read-only remote diagnosis.
- **Task given:** Explain how to integrate the implementation and finish documentation while PR #9 remains at Processing updates after teammate docs merged into main. Do not commit, push, merge or change branch rules.
- **What the assistant produced:** Added docs/evidence/pa2-contract-gate.md with the actual baseline, intentional failure, fix, CI links, human-reported screenshot names and clearly pending PR synchronization/merge/image-link evidence. Fetched main without merging; teammate PA2.md and SELF_ASSESSMENT_REPORT.md are already present there. PA2.md still has empty evidence sections. Existing teammate documents were not overwritten.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** The feature branch ref is 13773cf2800d37fc72a440d56054ae100b5841b4 and all four checks on push run 45649066961 succeeded, while the PR API still reports head 11d5ba7 and mergeable_state unknown. GitHub Status reports degraded Pull Requests, Actions and Webhooks, consistent with an update delay but not proof of the individual PR root cause. Remote main is 64942d2 after teammate documentation merges. No conflict resolution or successful PR merge is claimed.
- **Evidence:** PR https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; green fix run https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45649066961 ; https://www.githubstatus.com/ ; docs/evidence/pa2-contract-gate.md. Screenshot files still need human-provided repository paths. No commit or push performed.
- **Led to rules-file line:** none.

## 2026-11-26 — Recheck PR synchronization after GitHub service recovery

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Read-only PR/CI diagnosis and recovery guidance.
- **Task given:** Investigate why PR #9 still does not recognize the fixing commit after GitHub's incident was resolved; propose a history-preserving recovery.
- **What the assistant produced:** Verified the branch ref is 13773cf while refs/pull/9/head and the PR API still point to 11d5ba7. The fixing commit's push run 45649066961 is successful; no fixing-commit pull_request run is listed. GitHub Status currently reports all systems operational. Recommended the human close and reopen the same PR as a reversible recovery attempt, preserving commits and evidence; no guarantee of resynchronization is claimed. If that fails, contact GitHub Support with the mismatched refs and existing evidence rather than force-pushing or bypassing checks.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** GET-only API requests, git ls-remote and GitHub Status summary. The inconsistency is server-side PR metadata/ref synchronization rather than a missing push; the exact underlying cause remains unknown.
- **Evidence:** PR https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; successful fix run https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45649066961 ; https://www.githubstatus.com/api/v2/summary.json . No commit, push, PR state change or protection change performed.
- **Led to rules-file line:** none.

## 2026-11-26 — Verify synchronized fixing PR and green required checks

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Contract-gate evidence verification.
- **Task given:** Verify the green pull_request run supplied by the human before guiding the human merge.
- **What the assistant produced:** Confirmed run 45733073631 is a successful pull_request run for fixing commit 13773cf2800d37fc72a440d56054ae100b5841b4. All four jobs lint, format, typecheck and test succeeded. PR #9 now reports the same fixing head, mergeable true and mergeable_state clean. The human reported reopening the same PR and saving contract-fix-green.png after synchronization recovered. The screenshot itself has not been inspected or committed by the assistant.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** GET-only inspection of the supplied run, its jobs and PR #9. PR is still open and merged false at verification time. No successful merge is claimed yet; the human will perform it without squashing or rebasing to preserve the demonstrated commits.
- **Evidence:** https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45733073631 ; https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; fixing commit 13773cf2800d37fc72a440d56054ae100b5841b4. No commit, push, merge or protection change performed by the assistant.
- **Led to rules-file line:** none.

## 2026-11-26 — Verify human merge of the contract-test demonstration PR

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Final contract-gate evidence documentation.
- **Task given:** Continue evidence completion after the human reported merging PR #9. Do not commit, push or merge automatically.
- **What the assistant produced:** Verified PR #9 merged=true, closed, fixing head 13773cf2800d37fc72a440d56054ae100b5841b4 and merge commit 90862b1fadb979cc76da4a8a7c45da9451adaee6. Updated the local evidence index with the actual merge link and time. Screenshot integration and publication of pending evidence/log changes remain human-controlled next steps.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Read-only GitHub PR API reports merged_at 2026-11-26T01:15:55Z. Working tree still has pending AI-LOG modifications and the untracked evidence directory; no local changes were discarded. The human, not the assistant, performed the merge.
- **Evidence:** https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 ; https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/commit/90862b1fadb979cc76da4a8a7c45da9451adaee6 ; docs/evidence/pa2-contract-gate.md. No commit or push performed by the assistant.
- **Led to rules-file line:** none.

## 2026-11-26 — Inspect and link the four contract-gate screenshots

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Submission evidence documentation.
- **Task given:** Check the four copied screenshots and link them into the contract-gate evidence document without committing or pushing.
- **What the assistant produced:** Located the four images directly under docs/evidence rather than its images subfolder. Visually inspected each and added accurate relative image links/captions to docs/evidence/pa2-contract-gate.md. Consolidated historical PR synchronization notes and retained verified merge and run links. No images were moved or edited; teammate PA2.md on main was read but not overwritten.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** The red-test screenshot shows Consecutive_pages_do_not_overlap_and_retries_do_not_mutate_snapshot, expected [1,2] versus actual [6,5], with 23 failures/110 passes in the 133-case project. It does not show the separate numeric-sort case's [1,2,10] assertion, so its caption explicitly distinguishes the cases. The blocked screenshot shows commit 11d5ba7, failed Required test checks and a disabled merge button. Baseline and fix images show green checks on the same PR. Remote main was fetched to confirm merged implementation and teammate docs; no merge or working-tree overwrite was performed.
- **Evidence:** docs/evidence/baseline-green.png, docs/evidence/contract-test-red.png, docs/evidence/required-check-blocked.png, docs/evidence/contract-fix-green.png and docs/evidence/pa2-contract-gate.md. These local files are not yet claimed as published. No commit or push performed.
- **Led to rules-file line:** none.

## 2026-11-26 — Complete submission evidence links after PRs 9 and 13 merged

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** PA2 submission documentation.
- **Task given:** On the human-created feature/pa2-evidence-index branch, fill PA2.md evidence sections and update historical PR state in self-assessment without changing claimed marks or inventing tag evidence.
- **What the assistant produced:** Linked the real violation/fix commits, red/green runs, merge commit and existing screenshots from PA2.md; documented the actual pagination failure shown in the image. Updated SELF_ASSESSMENT_REPORT.md to state that PR #9 was blocked during the violation, then fixed, green and merged. Preserved every claimed criterion score and the total 94/100. Added explicit remaining limitations instead of leaving the limitations section empty. No application, contract, test, workflow or branch-rule changes.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Read the current submission docs and workflow on the branch created from origin/main after evidence PR #13 merged at da87713. PA2.md had blank blocked/green/break sections and self-assessment still described PR #9 as currently blocked. The workflow has no tag-push trigger, so submission-tag CI remains explicitly pending rather than being inferred from a branch run. Local Markdown targets are checked for existence; no quality tests are claimed rerun for this documentation-only change.
- **Evidence:** PA2.md, SELF_ASSESSMENT_REPORT.md, docs/evidence/pa2-contract-gate.md; PRs https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9 and https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/13 ; actual red/green runs linked in PA2.md. No commit or push performed by the assistant.
- **Led to rules-file line:** none.
