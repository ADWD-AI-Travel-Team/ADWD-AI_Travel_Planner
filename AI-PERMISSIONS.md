# AI Assistant Permissions

The AI coding assistant must follow these permission levels when working on this project.

## 1. Allowed — Run Freely

The agent may perform these actions without asking for approval:

- Read project files, search code with grep, and inspect the repository.
- Run `pnpm test`, `pnpm lint`, and `pnpm typecheck`.
- Run `git diff` and `git status`.
- Start and stop local development servers.
- Edit application code and tests when implementing legitimate changes.
- Perform local changes that can be reverted with one command.

## 2. Ask First — Require Human Approval

The agent must ask for explicit approval before:

- Running `git push` or opening a pull request.
- Adding, removing, or upgrading dependencies.
- Creating or modifying database migration files.
- Modifying CI/CD configuration or GitHub workflows.
- Changing shared project state or repository settings.
- Performing changes that affect other team members but can still be undone.

## 3. Never — Human Only

The agent must never perform these actions:

- Run database migrations against production.
- Deploy or roll back production environments.
- Delete production data or rotate production credentials.
- Expose, copy, or commit production secrets.
- Put AI model API keys in frontend code or browser bundles.

These actions must be performed manually by an authorized human.

## General Rule

When an action does not clearly belong to one of these permission levels, the agent must stop and ask for human approval. Security, production data, and real-user operations always take priority over convenience.