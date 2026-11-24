
# AI Travel Planner — Agent Instructions

## 1. Stack

- Frontend: React + TypeScript.
- Backend: ASP.NET Core Web API + C#.
- Database: PostgreSQL.
- ORM: Entity Framework Core.
- API style: REST with JSON.
- Testing: Unit, integration, and contract tests.
- CI: GitHub Actions.

Follow the existing project structure and installed dependency versions. Do not introduce a different framework or database without approval.

## 2. Commands

Run commands from the appropriate project directory.

- Install frontend dependencies: `npm ci`
- Start frontend: `npm run dev`
- Frontend lint: `npm run lint`
- Frontend format check: `npm run format:check`
- Frontend type check: `npm run typecheck`
- Backend restore: `dotnet restore`
- Backend build: `dotnet build --no-restore`
- Backend tests: `dotnet test --no-build`

Use the repository's documented commands. Do not invent alternative quality-check commands.

## 3. Conventions
- Follow the existing ASP.NET Core architecture.
- Keep business logic out of controllers.
- Use DTOs for API requests and responses.
- Validate all external input on the backend.
- Enforce authentication, authorization, and resource ownership on the server.
- Check resource ownership before accessing private data.
- Handle errors consistently and return appropriate HTTP status codes.
- Write contract tests for API behavior.

## 4. Never
- Never modify existing tests just to make implementation pass.
- Never modify validator rules to bypass failing validation.
- Never commit secrets, passwords, tokens, or private credentials.
- Never suppress compiler warnings, lint errors, or test failures just to obtain a green CI result.
- Never automatically run migrations to db without approval.