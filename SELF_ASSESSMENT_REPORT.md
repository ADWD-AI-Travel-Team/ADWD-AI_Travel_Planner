# Self-assessment — PA#2: Harness and quality gate

Submitted by:
- 23127325 — Lê Gia Bảo
- 23127443 — Trần Phạm Trọng Nhân

Total I claim: **94 / 100**

| Criterion | Max | I claim | Evidence |
|---|---:|---:|---|
| Rules file | 15 | 13 | [AGENTS.md](AGENTS.md) has the four required sections.|
| Permissions and secrets | 10 | 10 | [AI-PERMISSIONS.md](AI-PERMISSIONS.md) defines Allowed, Ask First, and Never. [.env.example](src/WebFrontend/.env.example) lists variables without values, and the [frontend .gitignore](src/WebFrontend/.gitignore) ignores `.env`. Tracked [appsettings.json](src/WebApi/appsettings.json) contains non-secret defaults and no API key; [.gitignore](.gitignore) excludes `appsettings.Development.json`. [README.md](README.md) states that the model key stays on the backend and that a monthly spending cap and alerts are configured. |
| One command for every check | 10 | 10 | [README.md](README.md), the [check scripts](scripts/), and [CI workflow](.github/workflows/ci.yml) use the same four commands. [PR #4](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/4) and [CI run 45547596703](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45547596703) document the fresh-clone and passing CI checks. |
| CI that blocks a merge | 20 | 19 | [PA2.md](PA2.md) includes branch-ruleset screenshots, including required `lint`, `format`, `typecheck`, and `test` checks. [PR #9](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9) was blocked during the intentional violation with a failing required test check in [run 45646715518](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45646715518); the same PR then passed after [fix 13773cf](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/commit/13773cf2800d37fc72a440d56054ae100b5841b4) in [green run 45733073631](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45733073631) and was merged. See [the blocked-merge screenshot](docs/evidence/required-check-blocked.png). Earlier passing CI evidence is in [PR #4](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/4) and [run 45547596703](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45547596703). |
| Contract of one AI Travel Planner endpoint | 25 | 24 | [GET /places contract](docs/contract/get-places.md) specifies public access, request and response rules, pagination, RFC 9457 error types/statuses, examples, compatibility, and a contract-test matrix. |
| Contract tests that bite | 15 | 14 | [PR #9](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9) reports 137 passing tests for the implementation. The deliberate incorrect-ordering commit [11d5ba7](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/commit/11d5ba76d260a4a067610a5ff2a749982d0f6be1) caused the test job to fail in [CI run 45646699706](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45646699706). The implementation-only fix [13773cf](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/commit/13773cf2800d37fc72a440d56054ae100b5841b4) restored green [pull-request CI](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/runs/45733073631). [The evidence index](docs/evidence/pa2-contract-gate.md) links all four screenshots and the unchanged test expectations. The endpoint is public, so a per-user ownership check does not apply. |
| AI-LOG.md | 5 | 4 | [AI_LOG_PA2.md](docs/AI_LOG/AI_LOG_PA2.md) identifies tools and work, and includes specific Changed/Rejected entries. Several later entries still contain `TODO(human)` placeholders. |

## What we did not manage

- Submission-tag CI evidence remains pending; the current workflow does not
  trigger on tag pushes. No branch/PR run is presented as a tag-triggered run.
- Several AI-log Changed/Rejected fields still need truthful human review.
- The four-check gate verifies C# formatting and .NET tests; a frontend formatter
  and frontend test runner have not been added. Type checking and lint cover
  both C# and the frontend.

The claimed marks and total above are the team's existing self-assessment;
this evidence update does not independently certify those scores or audit all
claims about provider spending caps, secret history or rules-file traceability.

