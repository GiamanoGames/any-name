# Roadmap

This is a living checklist, not the vision doc. The vision doc (your original
pasted prompt) is the north star; this file is what we're actually building,
in order, and it should get edited constantly as decisions land. Check boxes
as we finish things — this doc is the source of truth for "where are we."

**Platform (locked): PC/Steam, mouse + keyboard, no mobile build target.**
Core interaction model (DAW-precision mixing, drag/drop timeline editing,
step-sequencer grid input) doesn't survive a port to touch without becoming
a different, simpler game — not building toward mobile compatibility.

## Phase 0 — Foundation (no gameplay yet)
- [ ] Unity project created (URP, current LTS)
- [ ] Git repo initialized, Git LFS tracking audio/art binaries
- [ ] `_Project` folder structure in place
- [ ] `LICENSES.md` ledger started (even if empty rows for now)
- [ ] First empty scene loads and runs

## Phase 1 — DAW / Mix Prototype (de-risks the hardest, most unique part first)
Goal: one song, multiple stems, a timeline you can actually mix, and audible
proof that raw → edited → mixed → mastered sound *different and better*.
This is the whole bet the game is making — prove it before building anything
else around it.

- [ ] `SongData` ScriptableObject (stems, BPM, key, metadata)
- [ ] Timeline view: load stems as tracks, play/pause/stop, scrub
- [ ] Per-track volume, pan, mute, solo
- [ ] `IAudioProcessor` abstraction wrapping Unity's native `AudioMixer` (ParamEQ,
      Compressor, SFX Reverb exposed as runtime-adjustable parameters)
- [ ] Player can hear an EQ/compression/reverb change in real time
- [ ] Basic mastering stage (limiter/loudness pass) on the final bounce
- [ ] Manual playtest: does toggling raw vs. mixed vs. mastered actually feel
      like an upgrade? (This is the go/no-go checkpoint for the whole concept.)
- [ ] *(Design hook, not required for the playtest)* Have `IAudioProcessor` read
      its quality ceiling and available effect slots from a starter-tier
      `StudioEquipmentData` asset, even though purchasing upgrades isn't built
      yet. Costs nothing extra now and means the "rough studio" framing is
      true from day one, not bolted on later — see Phase 7.
- [ ] *(Design hook, not required for the playtest)* Write the stem loader to
      accept an arbitrary file path, not just a reference to a bundled game
      asset. This one decision is what makes "import your own audio" (Phase 9)
      nearly free later instead of a rewrite.
- [ ] *(Design hook, not required for the playtest)* Build `IAudioProcessor`
      as the base of a real plugin-chain architecture: an ordered,
      add/remove/reorder-able list of `IEffectPlugin` instances per track,
      each with its own exposed parameters (for real knob/slider UI),
      presets, and a `PluginData` asset (fictional brand name, category,
      unlock tier/cost).

**Decision (locked): in-house plugin architecture, not literal third-party
VST hosting.** Real VST hosting means Steinberg's trademark/compliance
requirements, sandboxing arbitrary third-party binary code so one bad plugin
can't crash the game, and individual licensing deals with every plugin
vendor to legally bundle their software — each of those alone is a
multi-person, multi-year undertaking at companies that do nothing else.
What actually delivers the *feeling* you're after: every "plugin" is ours,
built on `IAudioProcessor`, with a real knob-and-meter UI, real parameter
automation, save/load presets, and its own fictional brand identity — same
"inspired by, not copied from" rule the vision doc already sets for the DAW
UI generally. This isn't a compromise version of the idea; it's the version
that ships. Precedent it works: OFFBEAT (a real, shipped Steam title)
markets itself as "a fully functional audio workstation where all
instruments are simulated," with every track exportable as a real WAV —
same approach, well received, no third-party plugin hosting involved.

## Cross-Cutting: Difficulty & Time-Scale Modes
- **Decision (locked):** four presets — Easy, Normal, Hard, Simulation — scale
  both how long a release *takes* and how many steps/constraints it
  *requires*, not just economic forgiveness (the original vision doc's
  framing). Easy: sign an artist → record → buy an instrumental → edit/mix/
  master → release a single in roughly 20-60 real-world minutes, fewer
  required steps; a full album takes a bit more. Simulation: mirrors
  real-world production timelines and depth — a release can take real-world
  weeks of engagement, full constraint/step detail, closer to how an actual
  label operates.
- **Architecture implication:** don't hardcode step counts or time costs
  anywhere. Every system with a time or step cost — recording, mixing,
  release scheduling, chart movement — should read those numbers from a
  `DifficultyProfileData` asset instead of a literal value in code. Cheap to
  set up now; expensive to retrofit once Phase 2 onward has real numbers
  hardcoded everywhere.
- **Open question, not blocking yet:** does "weeks of gameplay" in Simulation
  mode mean the game keeps simulating time while you're away between
  sessions (closer to a live-service check-in pace), or many individual
  "advance the day/week" actions inside active play sessions (closer to how
  Football Manager or Stardew Valley handle time)? Worth deciding before
  Phase 6's calendar/time system locks in — flagging it now so it isn't
  decided by accident later.

## Cross-Cutting: Mix Quality Scoring & Critic Feedback
- **Decision (locked): deterministic, skill-based scoring — no RNG in the
  grade itself.** For every gradeable parameter (vocal level relative to the
  instrumental, EQ balance, dynamic range/compression, mastering loudness,
  stereo width, later distortion/separation), a song carries a target value
  and a tolerance range:
  `deviation = |player_value − target_value| / tolerance`
  `score = clamp(1 − deviation, 0, 1)`
  0% deviation = full points, deviation at the edge of tolerance = zero
  points, linear in between. Overall mix score = weighted average of every
  parameter's score, weights set per song/genre. If playtesting shows the
  linear falloff feels too harsh or too forgiving at the edges, swapping in
  a curved falloff is a tuning change, not an architecture change.
- **Tie-in to difficulty modes:** tolerance width scales with the difficulty
  profile above — Easy is forgiving, Simulation is closer to real
  mastering-engineer tightness.
- **The score itself stays hidden from the player**, same as the hidden
  mix-quality metrics already planned back in the original vision doc — no
  raw "vocal score: 73/100" HUD. The score drives what critics say instead.
- **Magazine/critic feedback (design locked):** fictional publications
  (Falling Rock, etc. — parody names only, same "inspired by, not copied
  from" rule as everywhere else) review a submitted release. Each
  per-parameter deviation and its direction (too hot / too buried /
  over-compressed) maps to a bank of hand-authored critique lines, picked to
  match. Randomly picking *among equally-valid phrasings* is fine for text
  variety — that's presentation, not a grade-affecting roll. This is how the
  player gets real insight into their shortcomings without a number ever
  appearing on screen.
- **Important distinction: this is NOT the same system as commercial
  success.** Mix quality score feeds Phase 5's chart/reception calculation
  as one input among several (marketing spend, fanbase, genre trend, timing/
  competition, and some controlled randomness) — preserving the original
  vision doc's "a technically great record can still underperform
  commercially" tension. Engineering skill is 100% deterministic and always
  yours; whether the record becomes a hit is a separate, multi-factor
  outcome layered on top.
- New data: `MixTargetData` (per-song target values + tolerances),
  `MagazineData`/`ReviewerData` (name, personality/bias, genre focus),
  a critique-line content bank keyed by parameter + deviation band.

## Phase 2 — Artist / A&R loop
Demo inbox, listen-to-submission flow, sign/pass decision, first artist
attributes (kept partially hidden from the player, per the vision doc).

## Phase 3 — Recording & feedback loop
Player gives an artist direction, artist responds per personality, retakes.

## Phase 4 — Release pipeline
Package a release, pick a distributor, schedule it, ship it.

## Phase 5 — Charts & reception simulation
Reception derived from song quality + marketing + fanbase + genre trend +
randomness — not a single hidden "hit chance" roll. Critic/magazine reviews
(see Cross-Cutting: Mix Quality Scoring & Critic Feedback above) are the
main vehicle for showing the player what their mix did well or poorly, in
character, never as a raw score.

## Phase 6 — Economy
Advances, budgets, royalties, expenses vs. revenue.
- **Decision (locked):** cash-only economy. No credit/loan/debt system —
  income comes from missions, record sales, and royalties; player spends on
  studio upgrades, plugins, and instrumentals. Cut deliberately for scope.
  Revisit only if playtesting turns up real "broke and permanently stuck"
  dead ends — the fix then would be a small guaranteed-income activity
  (session work, a cheap rush job), not a full lending system.

## Phase 7 — Role switching / studio hub
Only once Phases 1–6 are proven fun standalone: build the walkable studio hub
(grey-box first) with desk-based transitions into each role's UI.
- **Studio progression (design locked):** the studio starts rough — bare
  walls, entry-level gear — and is upgraded with earned cash: better
  computers, bigger mixing consoles, soundproofing/wall treatment, eventually
  unlocking new rooms/addons. Equipment tier is not purely cosmetic — it
  raises the quality ceiling and available toolset in the Phase 1 mixing
  chain (the `IAudioProcessor`/`StudioEquipmentData` hook planted back in
  Phase 1 pays off here).
- New data object: `StudioEquipmentData` — tier, cost, audio-quality effect,
  unlocked effect/plugin slots, visual prefab reference for the hub.

## Phase 8 — Employees / AI-run roles
## Phase 9 — Sandbox / creative mode
- **Import your own audio (design locked):** players can import their own
  external audio files as new stems/tracks and use the full mixing/mastering
  toolset on them, not just in-game-provided material. Largely free once
  Phase 1's stem loader is written generically (see the Phase 1 hook above) —
  "Import" becomes a file picker pointed at the same loader.
- **Export to disk (design locked):** the final bounce/render already built
  for Phase 1's mastering step gets exposed here as a real "Save As..." file
  export — a native save dialog, writing a standard .wav/.mp3 wherever the
  player chooses — not just saved inside game data.
- **Scope note:** the honest target is "good enough to mix a rough idea or
  finish a small personal track," not "replaces a real DAW." Sample-accurate
  timing, VST/AU plugin support, and professional metering are what entire
  companies (Ableton, Image-Line, Steinberg) do full-time for years — that's
  not this game's job. Aim for "surprisingly capable creative tool," not
  "DAW competitor" — the smaller target is still a great feature.
## Phase 10 — Multiplayer rhythm mode
## Phase 11 — Multiplayer beat-battle mode
- **Input tool (design locked):** this mode gets its own lightweight
  step-sequencer + piano-roll UI (FL Studio-style grid input) rather than
  reusing Career mode's full DAW. It's the right tool for fast, casual
  composition under a hard time limit, and it's lighter to keep in sync over
  the network — pattern/step data is tiny compared to raw audio, matching the
  "sync metadata, render locally" networking approach already planned for
  this mode. The underlying audio engine and asset data stay shared with
  Career mode; only the input surface is different.
## Phase 12 — Steam integration, achievements, cloud saves, polish

---
**Rule of thumb**: nothing from Phase *n+1* gets started until Phase *n*'s
manual playtest checkpoint has actually happened and felt good. This is the
part solo devs skip and regret.
