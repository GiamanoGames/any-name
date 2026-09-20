# Toolchain — Everything We're Using, and When

This is the full picture, organized by *when* each piece actually enters the
process. Nothing under "Shipping to Steam" needs any attention today — it's
here so "tell me everything" means everything, not because there's a task
attached to it yet.

## Right now — Phase 0 setup

- **Unity Hub** — not the game engine itself. It's a launcher/version
  manager: installs Editor versions, opens/creates projects, manages
  modules. You've already been living in this.
- **Unity Editor (6.3 LTS)** — the actual application where scenes,
  GameObjects, and the Inspector live. This is where you assemble and
  preview the game.
- **A C# code editor** — the Unity Editor is *not* where you write code
  day-to-day. Double-click a script in Unity and it opens a separate
  application. Two real choices:
  - **Visual Studio Community** (free) — Unity's most fully-supported
    option, best out-of-the-box debugging.
  - **VS Code** — lighter, and likely already familiar from HTML5/JS work.
    Unity has an official VS Code integration with equivalent debugging and
    IntelliSense. Either is a correct choice; pick what you're comfortable
    with, don't run both.
- **Git** — version control. Tracks every change to every file over time.
- **Git LFS** — an add-on to Git specifically for large binaries (audio,
  art) so the repo doesn't balloon in size.
- **GitHub** (or similar) — remote hosting for the repo. Local git alone
  isn't backed up; this is what protects you from a hard-drive failure.

## Building it — Phase 1 onward

- **URP (Universal Render Pipeline)** — the rendering system, already
  selected via the 3D (URP) project template. Governs how lighting and
  materials render.
- **C#** — Unity's scripting language. All game logic gets written in it.
- **ScriptableObjects** — Unity's built-in system for data-as-assets
  (`SongData`, `ArtistData`, etc. — already covered in the roadmap).
- **Unity's AudioMixer** — the built-in DSP/effects system we're using as
  the foundation of the DAW's mixing chain (also already covered).
- **itch.io** (planned, not set up yet) — once Phase 1 is playable, this is
  where we'll host test builds so you can actually run the game outside the
  Editor — the same role Firebase App Distribution played for the maze game.

## Shipping to Steam — Phase 12, informational only for now

- **A Steamworks Partner account** — Valve's developer program. One-time
  $100 fee per account, largely refundable once a game meets Valve's
  sales/review threshold. Required before anything can be published on
  Steam at all.
- **A Steamworks C# wrapper** — Valve's actual Steamworks SDK is C++;
  Unity projects almost always go through a C# wrapper instead. The two
  standard open-source options are **Steamworks.NET** and
  **Facepunch.Steamworks**. This is what lets game code call things like
  achievements, Steam Cloud saves, and friend invites.
- **SteamPipe** (`steamcmd` / the App Build tool) — Valve's upload utility
  for pushing a finished build to Steam's servers for distribution.

Nothing in that last section needs a decision today. It's a map, not a
to-do list yet.
