# AI activity log

## PA#2 review summary

- **Tool / model:** Codex / GPT-6; the earlier teammate entries identify their
  own tools separately in the detailed log.
- **Parts assisted:** quality-check scripts and CI, GET /places contract,
  layered implementation, real HTTP contract tests, and submission evidence.
- **Changed through human review:** the user requested explicit hours/menu
  eligibility, safe nonfixed 400 detail versus fixed 500 detail, and framework
  query parsing. The assistant implemented those corrections. The user also
  required local/CI command parity after the initial script work was incomplete.
- **Rejected through human review:** deferring the required eligibility data,
  unnecessary custom raw-query decoding, and contradictory detail-stability
  rules in the initial contract draft. These are actual review decisions, not
  invented manual code changes.
- **Other manual changes/rejections:** no additional ones have been reported
  for the remaining assistant tasks. This is not a claim that the human edited
  source or that every historical task has a confirmed None response.
- **Evidence:** [detailed contract review and human addendum](docs/AI_LOG/AI_LOG_PA2.md),
  [check-parity PR #4](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/4),
  and [HTTP contract gate evidence](docs/evidence/pa2-contract-gate.md).

The team's append-only PA#2 activity log is maintained in
[docs/AI_LOG/AI_LOG_PA2.md](docs/AI_LOG/AI_LOG_PA2.md).
This root entry point makes the existing log discoverable without duplicating
or rewriting its historical entries.

The log distinguishes assistant output, observed validation and human feedback.
`TODO(human)` means human Changed/Rejected feedback has not been supplied for
that entry; it is not evidence of an action that occurred. Later human review
must be appended with references to the relevant entries rather than invented
or used to hide earlier mistakes. See
[the finalization guide](docs/evidence/pa2-submission-finalization.md) for the
remaining review questions and submission-tag evidence procedure.

The entry **2026-11-26 — Human review addendum for earlier PA2 tasks** records
the user's actual contract corrections and check-parity feedback. It separates
human-directed revisions from assistant-authored edits. For those covered
topics, consult the later addendum alongside the historical TODO fields.
For other tasks, no additional manual changes or rejections have been reported;
this is not a blanket claim that every task was manually reviewed or unchanged.
