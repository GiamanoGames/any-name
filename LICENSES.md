# Asset License Ledger

Every non-original asset gets a row here **the day it enters the project** —
not "later." This is standard practice at real studios (usually a spreadsheet
tied to legal/publishing sign-off); for solo scale, this file is that
spreadsheet. If Steam or a distributor ever asks "can you prove you have the
rights to this," this file is the answer.

| Asset ID | Name | Source / Creator | License Type | Commercial Use? | Redistribution? | Modification? | Steam Release OK? | Attribution Required? | Notes |
|---|---|---|---|---|---|---|---|---|---|
| SFX_0001 | UI click (example row) | freesound.org / username | CC0 | Yes | Yes | Yes | Yes | No | Delete this row once real assets are added |
| MUS_0001 | Placeholder demo track (example row) | [library name] | Standard stock license | Check: video-only vs. interactive/game tier | Usually no | Depends | **Confirm before shipping** | Usually yes | Placeholder only — flag for replacement before Phase 4 |

## Fields, explained
- **License Type**: the actual license name (CC0, CC-BY, standard stock license,
  extended/game license, custom EULA, all-rights-reserved original work, etc.)
- **Commercial Use? / Redistribution? / Modification?**: read the actual license
  text, don't infer from price. Paid ≠ commercial-safe. Free ≠ unusable commercially.
- **Steam Release OK?**: the field that actually matters. Most stock-music
  licenses cover "background music in a video" — not "interactive content a
  player triggers repeatedly inside a shipped game." That's frequently a
  separate, more expensive tier. If you're not sure, the answer here is "NO —
  confirm" until you've actually checked.
- **Notes**: flag anything that's a placeholder and must be swapped before
  a shipping build.
