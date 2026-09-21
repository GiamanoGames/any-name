# Roadmap

This is a living checklist, not the vision doc. The vision doc (your original
pasted prompt) is the north star; this file is what we're actually building,
in order, and it should get edited constantly as decisions land. Check boxes
as we finish things — this doc is the source of truth for "where are we."

**Platform (locked): PC/Steam, mouse + keyboard, no mobile build target.**
Core interaction model (DAW-precision mixing, drag/drop timeline editing,
step-sequencer grid input) doesn't survive a port to touch without becoming
a different, simpler game — not building toward mobile compatibility.

**VR: not currently planned, same underlying reasoning as mobile above.** A
DAW-precision, mouse+keyboard interface doesn't translate to motion
controllers without a comparably large redesign across the entire game, not
just one subsystem. Not ruled out forever — a possible far-future spinoff
once the core game has shipped and succeeded sits in the same "decade+
stretch goal" category as Phase 14's plugin hosting — but genuinely
uncertain, not a committed target, worth revisiting only much later.

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

- [x] `SongData` ScriptableObject (stems, BPM, key, metadata) — done, plus a
      working `SongPlayer` proving sample-accurate multi-stem sync playback
      via `AudioSettings.dspTime` + `PlayScheduled`
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
- [ ] *(Design hook, not required for the playtest)* Support grouping stems
      into buses/submixes (e.g. "Strings," "Brass," "Percussion"), not just a
      flat per-track fader list. A pop song with 4-6 stems doesn't need this,
      but content like the 30-track orchestral challenge (see Cross-Cutting:
      Difficulty & Time-Scale Modes) does — real orchestral mixing consoles
      work this way too, so it's authentic, not just a technical fix.
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
- **Curated challenge scenarios (design locked):** hand-built, one-off
  mixing/mastering challenges outside the normal Career loop — the flagship
  example being a hard-difficulty orchestral challenge: the player is
  handed a real, complex, already-recorded multitrack (a 30-track live
  orchestral session) and has to mix and master it well. Uses the exact
  same deterministic scoring engine as the normal loop (see Cross-Cutting:
  Mix Quality Scoring above) — this is content, not a new system. Orchestral
  material specifically is a great test of real skill: correct mixing
  there is largely about *preserving* natural dynamic range rather than
  the loudness/punch instinct pop mixing rewards, so the target values for
  a challenge like this look meaningfully different from a normal pop song,
  which is exactly the point.

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

## Cross-Cutting: No Visible Human Character Models
- **Decision (locked):** the game never shows rendered human characters —
  no 3D character models, no photorealistic faces/portraits anywhere.
  Believable human models are expensive and easy to get wrong; a mediocre
  one looks worse than none (the uncanny valley is real). Precedent this
  works well, not just avoids a problem: Firewatch builds one of the most
  well-regarded characters in recent narrative games entirely through
  voice-only radio conversations — never shown on screen once.
- **How conversations work:** phone calls (real voice) and text/email
  messages, never a rendered face. Phone calls for higher-impact moments
  (signing an artist, a real conflict, a career milestone); text/email as
  the everyday default. This split is about production scale, not just
  style — voicing every interaction across potentially hundreds of artists
  over a full playthrough isn't sustainable even across a decade. Text
  scales; voice gets spent where it earns its cost.
- **Identification without faces:** any UI needing to show "who is this"
  (inbox, contact list, roster) uses stylized, deliberately-not-realistic
  identifiers — an icon, a silhouette, album-cover-style art. Same
  "lean into stylization" principle Papers, Please uses for its character
  windows.
- **Ripple effects on already-planned phases:** Phase 7's studio hub gets
  populated with atmosphere (gear, blinking monitors, ambient sound), not
  walking human NPCs. Phase 8's AI-run employees are a roster/data screen
  with a stylized icon, not a character model in the hub.

## Cross-Cutting: In-Game Devices — Phone, Laptop, and the In-Game Internet
- **Decision (locked):** the player has two devices — a phone and a laptop
  — as the actual interface for the texts/emails/calls system above, and as
  the diegetic container for browsing an in-game "internet."
- **The internet doubles as a UI skin for systems already planned, not new
  gameplay logic.** Fits naturally as browsable "sites" instead of menu
  screens: the beat/instrumental marketplace, the charts, the studio gear/
  plugin store, even the magazine critic reviews (a site like Falling
  Rock's, not just inbox text). Real work is the browser UI chrome itself
  (address bar, tabs, bookmarks) — most sites behind it reuse systems that
  already exist.
- **Easter-egg/joke websites (design locked):** pure content, no gameplay
  logic attached, there purely to be found and shared. Flagship example: a
  Glassdoor/Yelp-style "studio review site" where artists review what it's
  like working with the player's label — matches the exact behavior GTA V's
  in-game internet is known for (players screenshot and share the joke
  sites specifically). Made *reactive*, not static: artist relationship/
  satisfaction scores (already called for in the vision doc) drive which
  reviews generate — a demanding, low-relationship playthrough produces
  different reviews than a well-run label. Same "hidden metric drives
  flavor text" mechanism already built for magazine critics, pointed at a
  funnier target. Cheap to keep adding more of over time — good long-tail
  post-launch content that never touches core systems.

## Phase 2 — Artist / A&R loop
Demo inbox, listen-to-submission flow, sign/pass decision, first artist
attributes (kept partially hidden from the player, per the vision doc).
- **Discovery mechanic (design locked): tapes/demos, not live visual
  auditions.** Matches how A&R genuinely works — reviewing submitted
  recordings really is the real first-pass method, live auditions come
  later in the actual industry too. Also the natural fit for the no-humans
  rule above: a demo review is audio plus a text profile, nothing to
  render. The vision doc's "invite to audition" option stays audio-only —
  a rawer, less-produced live-feel recording rather than a polished demo,
  not a visual performance.

## Phase 3 — Recording & feedback loop
Player gives an artist direction, artist responds per personality, retakes.
- **Live playable instruments (design locked):** the PC keyboard doubles as
  a virtual piano/keyboard (letter-key-to-note mapping) and virtual drum
  pads, same pattern as the number-key mute controls already working in
  `SongPlayer` — nobody needs MIDI hardware just to make something happen.
  Real MIDI hardware (keyboards, pad controllers, MPC-style controllers) is
  supported on top of that, not instead of it. Real path: Minis, an
  established, actively-maintained open-source Unity package (Keijiro
  Takahashi) built on Unity's New Input System. Worth flagging honestly:
  this means standardizing keyboard/controller input on the New Input
  System when we get here, rather than the legacy `Input` class our current
  mute-key code uses — a clean swap at that point, not a rewrite.
- **Quantization (design locked):** recorded note timing snaps to the
  nearest beat subdivision against the song's actual tempo. Free
  architecturally — `SongData.bpm` already exists from Phase 1.
- **MPC-style pads:** the same interaction pattern already locked in for
  Phase 12's beat-battle step-sequencer/pad grid — not a new system, the
  same UI component reused inside Career mode's Producer desk.
- **Guitar amp/cabinet simulation:** a genuinely deep DSP specialty on its
  own — Neural DSP and Kemper exist as companies to do only this. Fits
  architecturally as a particularly demanding `IEffectPlugin` instance, not
  a new system. A solid, usable basic amp sim is achievable at normal
  effort; matching state-of-the-art captured-amp quality is its own
  multi-year specialty — same "capable now, keep deepening long-term"
  framing as the rest of the DAW ambition.

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
- **Room list, first draft:** Control Room/Studio (the DAW — Producer,
  Recording, and Mixing/Mastering roles all share this one desk), A&R
  Office/Listening Room, Artist Lounge, Your Office (Label Exec — marketing,
  distribution, finances). Four real spaces, not a literal room per role.
- **Realism ambition (design locked, matches the Phase 9/14 "staged, not
  capped" logic):** the long-term target is genuinely hyper-real studio
  environments — real gear-accurate assets, not placeholder-quality forever.
  Production method for getting there without wasting work: grey-box the
  room layout first and playtest *that* — distances, flow, door placement —
  before a single high-fidelity asset goes in. Layout problems are cheap to
  fix in grey-box and expensive to fix after a full art pass.
- **Studio progression (design locked):** the studio starts rough — bare
  walls, entry-level gear — and is upgraded with earned cash: better
  computers, bigger mixing consoles, soundproofing/wall treatment, eventually
  unlocking new rooms/addons. Equipment tier is not purely cosmetic — it
  raises the quality ceiling and available toolset in the Phase 1 mixing
  chain (the `IAudioProcessor`/`StudioEquipmentData` hook planted back in
  Phase 1 pays off here). This progression is also the natural vehicle for
  staging in hyper-real assets over time — a rough starting studio can look
  and sound cheap on purpose; the hyper-real gear becomes what success
  looks like, rather than needing every asset finished on day one.
- New data object: `StudioEquipmentData` — tier, cost, audio-quality effect,
  unlocked effect/plugin slots, visual prefab reference for the hub.
- **DAW-to-environment connection (design locked):** the DAW stays a flat
  2D screen-space UI, not a fully-3D-diegetic interface with literal
  world-space knobs — precision work (waveforms, meters, exact numbers) is
  far more usable flat, and it's honestly more authentic anyway, since real
  studio editing happens on a screen either way. Purchased/hyper-real 3D
  assets are environment art plus the interactable trigger object at each
  desk: approach or click the console prop, the view transitions into that
  desk's 2D UI (DAW at the Control Room, inbox at A&R, etc.) — same
  mechanism, every room. This is exactly what `StudioEquipmentData.
  visualPrefab` above was built for: each purchased asset becomes a prefab
  assigned to a tier, swapped in as the player upgrades. The camera
  transition itself should feel like the physical motion of sitting down —
  a smooth move into a close, seated framing, not an instant cut. Concrete
  tool for this: Cinemachine, Unity's own official camera package, built
  specifically for state-based blends like this. Optional later polish: a
  Render Texture showing a live DAW preview on the in-world monitor's
  screen mesh, so it looks functional before the transition — nice touch,
  not required.

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
- **Scope note, aligned with the Phase 14 plugin-hosting ambition:**
  short-term (what actually ships at launch) — a genuinely capable creative
  tool: real multi-track import/export, real mixing through the in-house
  plugin chain, good enough that a musician could actually finish a real
  personal track in it, not a toy. Long-term (post-launch, same decade+,
  no-fixed-deadline horizon as Phase 14) — the honest target is a real,
  professional-grade DAW, not a permanently-simplified one; deepening
  feature completeness alongside real plugin hosting is the actual aim once
  there's a shipped, successful game to build it on top of. The staged
  sequencing exists to give the decade+ vision a foundation to stand on —
  it's not a ceiling on the ambition itself.
## Phase 10 — Multiplayer rhythm mode

## Phase 11 — Multiplayer live karaoke
- **Decision (locked): karaoke uses only in-game/player-created songs —
  never real-world commercial tracks.** Real karaoke businesses need actual
  music-publisher licensing deals to legally let people sing along to real
  hits; that's a business-development undertaking we're not taking on. The
  clean, IP-safe, and honestly more thematic version: players sing karaoke
  to songs their own (or another player's) in-game label actually released,
  or to original demo tracks built for the game. Same "no real branding/IP"
  rule as everywhere else in this project, applied to songs instead of
  plugin names or magazine titles.
- **New technical domain, worth knowing going in:** this needs microphone
  input capture (Unity's built-in `Microphone` class handles the capture
  side) plus real-time pitch detection to score how well the player's sung
  pitch tracks the reference melody. That's a genuinely different DSP
  discipline from the mixing/mastering chain — not harder in an absolute
  sense, just a separate skill arc, similar in spirit to the native plugin
  hosting work in Phase 14.
- Multiplayer shape: players take turns or sing together live, everyone
  listens, similar spirit to the beat-battle mode below but built around
  vocal performance instead of composition.

## Phase 12 — Multiplayer beat-battle mode
- **Input tool (design locked):** this mode gets its own lightweight
  step-sequencer + piano-roll UI (FL Studio-style grid input) rather than
  reusing Career mode's full DAW. It's the right tool for fast, casual
  composition under a hard time limit, and it's lighter to keep in sync over
  the network — pattern/step data is tiny compared to raw audio, matching the
  "sync metadata, render locally" networking approach already planned for
  this mode. The underlying audio engine and asset data stay shared with
  Career mode; only the input surface is different.
- Matches the already-locked design exactly: everyone gets the same
  assigned sample set (drum kit, bass, synth, FX), a shared time limit,
  everyone submits, everyone listens together, everyone votes.

## Phase 13 — Steam integration, achievements, cloud saves, polish

## Phase 14 — Real plugin hosting (CLAP primary, VST3 secondary) — long-term, no fixed timeline
- **Decision (locked): the ambition is real, not a simplified-forever
  simulation.** Genuine third-party plugin hosting is the long-term target.
  This phase starts only once the core game has shipped and proven itself —
  it is explicitly not a prerequisite for launch, and nothing about it
  changes any current Phase 1-12 work.
- **CLAP over VST3 as the primary target.** CLAP (cleveraudio.org) is a
  genuinely open plugin standard — no restrictive proprietary licensing
  agreement required to host it. Real adoption already (Reaper and other
  DAWs host it, 100+ plugins exist in the format), and notably, Unreal
  Engine has announced plans to support it for in-game audio — real
  precedent that hosting plugins inside a game engine is a legitimate,
  active industry direction.
- **VST3 as a secondary target, eyes open.** Steinberg offers a real,
  signable license path for hosting VST3 (the "Proprietary Steinberg VST3"
  license) — achievable, not fantasy. Honest limitation: Steinberg stopped
  issuing new VST2 host licenses years ago, so a large amount of the
  industry's legacy VST2-only plugin library would remain permanently
  unhostable by us, no matter how much time is invested. Known going in.
- **What this actually requires:** a native (C/C++) plugin host bridged
  into Unity via its native plugin interface — real third-party binaries
  can't be hosted from C# alone. Means learning native interop,
  cross-language marshaling, and crash-safe plugin sandboxing — a
  genuinely different engineering discipline from the rest of this
  project, worth its own dedicated arc when the time comes.
- **Nothing built earlier is wasted.** The `IEffectPlugin`/`IAudioProcessor`
  architecture from Phase 1 is the right foundation regardless — a real
  plugin host becomes another implementation behind that same interface,
  not a replacement for it.

---
**Rule of thumb**: nothing from Phase *n+1* gets started until Phase *n*'s
manual playtest checkpoint has actually happened and felt good. This is the
part solo devs skip and regret.
