
## Environment Configuration

1. Clone the repository.
2. Copy `.env.example` to `.env`.
3. Fill in the required environment variables.
4. Never commit `.env` or share its contents.

The model API key must only be configured on the backend.
It must never be exposed to the frontend or browser bundle.

## LLM API Usage and Spending Limits

AI Travel Planner uses an external LLM provider for
AI-assisted travel planning.

To prevent unexpected API expenses:

- The API key is stored exclusively in the backend environment.
- A monthly spending cap is configured in the provider dashboard.
- Usage alerts are enabled.
- The team monitors token consumption and API costs.
- Unit tests use mocked model responses instead of real API calls.

### Configured Spending Limit

- Provider: [Actual provider]
- Monthly budget/cap: [Actual configured amount]
- Alert threshold: [Actual configured threshold]
- Budget owner: [Responsible team member]

The team must review API usage regularly and must not
increase the spending limit without approval.


