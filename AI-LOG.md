# AI-LOG.md

## 1. Tools used

| Tool | Model / version | Used for (which part) | Used by |
| --- | --- | --- | --- |
| Codex | GPT-6 | Backend checks, CI, README and smoke test | Team, through this assistant session |

## 2. Entries

### #1 — 2026-11-24 — Four reproducible backend checks

- **Tool / model:** Codex / GPT-6.
- **Part of the work:** Checks, CI, documentation and sample test.
- **Task given:** Define one command per lint, format, typecheck and test check; document fresh-clone setup; run the same commands in four pull-request CI jobs; add a tiny test and environment-file hygiene; open a separate-branch PR.
- **What the assistant produced:** Root package.json with exactly four scripts, a pinned .NET SDK, README Checks instructions, four GitHub Actions jobs, an in-process xUnit weather endpoint smoke test, C# formatting configuration, and environment-file ignores with a names-only .env.example. All four commands passed locally and in a separate fresh temporary clone, with one test passing and zero build warnings or errors. The gate covers the C# solution; the existing frontend prototype is outside this task.
- **Changed:** TODO(human)
- **Rejected:** TODO(human)
- **How we found the problem:** Running the proposed commands exposed an unsupported VSTest command-line switch and analyzer ASP0027 for an unnecessary Program declaration. The assistant corrected the runner configuration and removed its own unnecessary declaration without changing the smoke test. The formatter also corrected pre-existing double whitespace in Program.cs. A source-tree scan found no common private-key or token patterns; git check-ignore confirmed that .env and .env.local are ignored while .env.example remains trackable.
- **Evidence:** Implementation commit `d34e5e78acbfa79799548d1897f3f6c12b5ebd82` (short form: `d34e5e7`).
- **Led to rules-file line:** none; no rules-file changes were requested in this task.

## 3. Rules-file traceability

No rules-file lines have been added by this task.

## 5. Model and cost (for the README as well)

- **Model used at runtime (if any):** Not chosen yet. No runtime model is called by these checks or the smoke test.
- **Spending cap set on the API key:** Not configured; no model API key is used for this task.
- **Estimated cost of one plan:** Not measured; plan generation is outside this task.
