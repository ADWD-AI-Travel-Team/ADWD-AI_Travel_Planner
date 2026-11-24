# AI-LOG.md

## 2026-11-24 — AI assistant permission policy (PA#2)

**Tool:** ChatGPT (GPT).

**Asked for:** Draft and improve `AI_PERMISSION.md` by organizing the AI coding assistant's permissions into three levels: Allowed, Ask First, and Never.

**Kept:** The three-tier permission structure and rules covering repository access, running tests, Git operations, dependency changes, database migrations, CI configuration, production deployments, and production secrets.

**Changed:** Reviewed the generated Never list and removed three rules: sending emails to real users or charging customers, modifying tests/evaluation data/validator rules just to pass checks, and making real model API calls in unit tests. These were removed from this permission document based on my team's requested scope. I still need to ensure that other project rules and the course requirements cover relevant safety and testing practices.

**Rejected:** Did not keep the generated permission policy exactly as written. I reviewed and adjusted the restrictions to match the team's intended permission boundaries rather than accepting the AI output without review.


