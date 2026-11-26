# PA#2 submission finalization

## Verified foundation

- The GET /places implementation and HTTP contract tests were merged in
  [PR #9](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/9).
- The deliberate implementation-only violation, failed HTTP assertions,
  failed required check, blocked-merge screenshot, normal fixing commit and
  successful CI are linked in [the evidence index](pa2-contract-gate.md).
- Evidence images were published in
  [PR #13](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/13),
  and the submission index was filled in
  [PR #14](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/14).
- The exact `pa2` tag trigger was merged through
  [PR #15](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/pull/15).
  It runs the same four quality checks. Trigger configuration is not evidence
  that the tag run has already succeeded.

The approved contract, existing architecture tests, HTTP assertions, claimed
self-assessment scores and earlier AI-log entries are not changed by this
documentation finalization. This guide does not independently award marks.

## Human review before choosing the final commit

The assistant can correct stale descriptions and connect real evidence. Only
the team can attest to its own review decisions, provider configuration and
claimed marks. Complete these items before finalizing the submission:

| Item | What is still needed |
| --- | --- |
| AI-log human feedback | The 2026-11-26 review addendum now records contract corrections and check-parity feedback already supplied by the user. Other manual changes/rejections have not been reported; they are not invented. Approval and running Git commands are not automatically manual code edits. |
| Existing contract review | The user did reject deferred eligibility, fixed 400 detail and unnecessary custom decoding in the earlier draft. Those requests led to actual revisions. They can be described as documented user review, not human-authored source edits. |
| Rules-file traceability | Identify two actual incidents and the exact relevant product-specific rules. Do not claim the rubric's highest band solely because four headings exist. No new incidents should be invented to fill this gap. |
| Model budget | appsettings defaults are proposed configuration, not evidence of an account or spending cap. Supply real provider/cap/alert evidence if configured, or state not configured/verified. The current endpoint and tests make no model calls. |
| Secret history | Do not equate ignoring .env or scanning common patterns with a complete Git-history audit. Verify separately before asserting an absolute secret-free history. Never publish secret values as evidence. |
| Scores | Review all seven claimed marks and limitations. The current 94/100 is retained as the team's existing claim, not a fresh assistant assessment. |

Old log entries describe what was known at their date. Preserve them; append
new confirmations or corrections instead of rewriting their history. In
particular, no automatic blanket replacement of `TODO(human)` is appropriate.

## When PA2.md section 5 becomes complete

Section 5's run evidence can be verified only after the final tag is pushed and
the resulting CI run completes. Before that, an honest document must distinguish
the configured trigger from a successful tag run.

There is an unavoidable ordering detail: pushing a tag creates a new run ID.
The commit referenced by that tag was created earlier and cannot contain that
future ID. Do not move the submitted tag to a later documentation commit merely
to insert its own run URL.

The submission index therefore contains a stable lookup URL in advance:

[CI workflow runs filtered to pa2](https://github.com/ADWD-AI-Travel-Team/ADWD-AI_Travel_Planner/actions/workflows/ci.yml?query=branch%3Apa2).

This link is a navigation aid, not proof of success before a run exists. If the
filter does not show a result in the interface, open the repository's Actions
tab, select CI, and inspect runs for the tag/ref `pa2` and its exact commit SHA.

After the tag run succeeds, save the resolved `/actions/runs/<id>` URL and
screenshot in the hand-in notes. An English GitHub Release body attached to the
same immutable tag can also contain the actual run URL and tagged SHA, without
changing the commit. This is a later human publication action, not something
the assistant has performed. If a post-tag docs PR records the final URL on
main, explicitly distinguish it from the tagged snapshot.

Do not change PA2.md to claim tag success before observing it. If the course
requires the literal future run ID inside the immutable tagged PA2.md rather
than the stable lookup plus accompanying evidence, clarify that evidence format
with the lecturer; do not fake the chronology or rewrite the tag to hide it.

## Human-controlled final sequence

1. Review the documentation and its retrospective human-feedback addendum.
   Supply additional feedback only if it actually occurred; do not manufacture
   a change or rejection for every assistant task.
2. Commit/push the docs branch yourself, open a PR and merge after required
   checks pass. The assistant must not perform these GitHub mutations here.
3. Select the exact final main revision with the team. Verify its checks and
   ensure it contains all files needed for submission. Check local and remote
   tag `pa2` do not already exist. If they exist, stop rather than overwrite.
4. Obtain the user's explicit confirmation before creating/pushing the tag.
   Do not push arbitrary tags or force-push any published tag.
5. After push, verify the actual CI run is for `pa2`, its SHA matches the
   selected revision, and lint/format/typecheck/test all succeed. A push event
   alone does not distinguish branch from tag; inspect ref and SHA as well.
6. Save the real URL, screenshot, final SHA and tag name with the submission.
   Record the completed verification through an append-only evidence update
   or Release/hand-in note, clearly dated after tagging.
7. Prepare the course-required repository archive including .git and the
   self-assessment. Use the actual team IDs and confirmed total in its filename.
   Do not substitute an ordinary source ZIP that lacks .git history.

The assistant has not created/pushed `pa2`, published a Release or generated a
submission archive during this documentation review. These remain explicit
next actions, not completed events.
