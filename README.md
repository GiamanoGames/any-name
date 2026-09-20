# StudioSim — Project Starter Scaffold

("StudioSim" is a placeholder codename — rename freely once you've got a real title.)

This is **not** a full Unity project. Unity itself has to generate the `Library/`,
`Packages/`, `ProjectSettings/`, and `.csproj` files via the Editor — those can't be
faked. What this scaffold gives you is everything *around* that: the folder
convention, the git setup, and the living docs a real team keeps in the repo.

## Where everything actually lives

```
C:\Dev\
└── GiamanoGames\                 ← studio-level folder — just a container, no
    │                                loose files belong directly in it
    ├── StudioSim\                 ← THIS project's root = one full git repo.
    │   │                            Unity creates this folder itself in step 1
    │   │                            below — you never make it by hand.
    │   ├── Assets\                 ← Unity auto-creates this
    │   │   ├── Scenes\              ← Unity auto-creates this too (default scene inside)
    │   │   └── _Project\            ← from this scaffold's zip — everything ours
    │   │       ├── Scripts\           lives under here. See "Folder guide" below
    │   │       ├── Data\              for what each subfolder is for. Mostly
    │   │       ├── Audio\             empty (.gitkeep placeholders) until we
    │   │       ├── Art\               start writing Phase 1 code.
    │   │       ├── Prefabs\
    │   │       ├── Scenes\
    │   │       └── UI\
    │   ├── Library\                ← Unity auto-generates. NEVER touch or commit
    │   │                             this — it's a rebuildable cache, already
    │   │                             excluded in .gitignore. Safe to delete
    │   │                             entirely if it ever gets corrupted; Unity
    │   │                             regenerates it on next open.
    │   ├── Packages\                ← Unity auto-creates, tracks installed packages
    │   ├── ProjectSettings\          ← Unity auto-creates, project-wide config
    │   ├── .git\                     ← hidden, created by `git init` in step 3
    │   ├── .gitignore                ← from this scaffold
    │   ├── .gitattributes            ← from this scaffold
    │   ├── README.md                 ← this file
    │   ├── ROADMAP.md                ← from this scaffold
    │   ├── LICENSES.md               ← from this scaffold
    │   └── TOOLCHAIN.md              ← from this scaffold
    └── (future projects, if any, would each get their own sibling folder
         here — e.g. the maze game, only if you ever choose to consolidate
         it here for consistency; no need to touch it now)
```

The six files listed under `StudioSim\` (`.gitignore` through `TOOLCHAIN.md`)
are the *only* loose files this scaffold adds — everything else in that tree
is either Unity-generated or something you'll build over time.

## Step-by-step: turning this into a real project

1. **Create the Unity project.**
   Open Unity Hub → New Project → pick the **3D (URP)** template (not bare 3D Core —
   you want the Universal Render Pipeline for the studio interior lighting).
   Store it *outside* any OneDrive/Dropbox-synced folder, same as you already do for
   the maze game — cloud sync + Unity's `Library` folder is a bad combination.

2. **Merge the folder structure.**
   Copy everything inside `UnityProject-Assets/_Project/` from this scaffold into
   your new project's `Assets/_Project/` folder. The `_Project` prefix keeps your
   own code/data visually separated from anything you import from the Asset Store
   or a package — that separation matters once the project gets big.

3. **Initialize git at the project root** (the folder that contains `Assets/`,
   `Packages/`, `ProjectSettings/` — not inside `Assets`):
   ```
   git init
   git lfs install
   ```
   Copy `.gitignore`, `.gitattributes`, `ROADMAP.md`, `LICENSES.md`, and
   `TOOLCHAIN.md` from this scaffold into that same root folder.

4. **First commit.**
   ```
   git add .
   git commit -m "Project scaffold: folder structure, LFS, roadmap, license ledger"
   ```

5. **Confirm LFS is actually tracking things** before you import any real audio —
   drop a test `.wav` into `Assets/_Project/Audio/Placeholder/`, run
   `git status`, and confirm git reports it as an LFS-tracked file (check
   `git lfs ls-files` after adding it). Catching this now saves you from a
   multi-gigabyte repo six weeks from now.

## Folder guide

| Folder | What lives here |
|---|---|
| `Scripts/Core` | Game state, time simulation, save system, event bus |
| `Scripts/Audio` | The audio abstraction layer (`IAudioProcessor` etc.), mixer control |
| `Scripts/Data` | `ScriptableObject` definitions — `ArtistData`, `SongData`, `ContractData`, `StudioEquipmentData`... |
| `Scripts/UI` | Screen/desk UI controllers |
| `Scripts/Career` | Career-mode/role logic |
| `Scripts/Editor` | Editor-only tooling (debug panels, data generators) — never ships in a build |
| `Data` | The actual `.asset` ScriptableObject instances (data, not code) |
| `Audio/Music`, `Audio/SFX`, `Audio/Placeholder` | Keep dev/placeholder audio physically separate from anything license-cleared for shipping |

## Next step
Once this is committed, we start Phase 1 from `ROADMAP.md` — the single-song DAW
prototype. Tell me when the repo's live and I'll write the first script.
