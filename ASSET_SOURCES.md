# Asset Sourcing Guide — 3D Props (Studio Gear, Furniture, etc.)

Quick reference for whenever you're browsing for mixing consoles, monitors,
desks, chairs, wall treatment, or any other studio prop.

## File types to look for

- **.fbx** — the gold standard for Unity. Supports meshes, materials
  references, and rigging/animation if a prop ever needs it. Default choice.
- **.obj** — simpler, mesh + basic material only, no animation support.
  Perfectly fine for static props (a desk, a chair, a console) that never move.
- **.unitypackage** — Unity's own bundle format. Anything from the Unity
  Asset Store comes as this — model, textures, materials, and often a ready
  prefab, all in one file, one-click import. No manual setup needed.
- **.blend** — native Blender files. Unity *can* import these directly if
  Blender is installed on the machine (it calls Blender in the background
  to convert), but it's a little fragile across Blender versions. Safer to
  export to FBX from Blender yourself if you're modeling anything custom.
- **Textures**: expect `.png` or `.jpg` alongside the mesh — look for **PBR**
  material sets specifically (albedo/base color, normal, metallic/roughness,
  ambient occlusion maps). URP's Lit shader is built for exactly this set;
  older non-PBR assets still work but need extra manual material setup.

## Two things worth checking before you buy anything

- **"Game-ready" / "real-time," not "render-only."** A lot of 3D marketplaces
  serve architectural-visualization and film artists too — their models can
  have millions of polygons, meant for offline rendering, and will tank
  performance in an actual real-time game. Look for listings tagged
  "game-ready," "real-time," or with a stated polygon/triangle count.
- **Real-world scale.** Unity treats 1 unit = 1 meter. A mixing console
  modeled to correct real-world dimensions will drop in at the right size;
  one that wasn't will look comically huge or tiny until you rescale it.
  Not a big deal, just don't be surprised if it happens.

## Where to actually look

- **Unity Asset Store** (assetstore.unity.com) — check here first. Assets
  are pre-packaged for Unity specifically (`.unitypackage`), often with
  prefabs already set up, least setup work of any option. Still verify each
  asset's individual license for commercial use, same as always.
- **Fab** (fab.com) — worth knowing this one directly: Epic Games merged
  the old Unreal Marketplace, the Sketchfab *store*, and Quixel Megascans
  into a single unified marketplace called Fab in late 2024. If you go
  looking for "Sketchfab" expecting a separate storefront, you'll land here
  instead — that's correct, not a dead end. Fab explicitly supports Unity,
  not just Unreal, and is a huge, high-quality library worth browsing for
  studio gear and furniture specifically.
- **CGTrader** (cgtrader.com) and **TurboSquid** (turbosquid.com) —
  long-established, independent 3D marketplaces, strong for hero/unique
  props (a specific, detailed mixing console model, for instance). Wider
  price range, sometimes pricier, often higher production value.
- **Poly Haven** (polyhaven.com) — completely free, CC0 (public-domain
  equivalent) models, textures, and HDRIs. No license to track, no
  attribution required. Smaller catalog than the paid sites, but zero
  friction — worth checking first for generic props before paying for
  anything.

## Search terms that actually work

"mixing console," "studio desk," "recording studio pack," "audio equipment
3D model," "studio monitor speaker," "acoustic panel," "soundproofing
foam" — these are established categories across all the sites above.

## Don't forget

Anything you bring in that isn't yours gets a line in `LICENSES.md` the
day it enters the project — same rule as always, doesn't change with the
site it came from.

## A note on audio specifically

Everything above is about 3D props and environment art. For actual music
and sound content — stems, demo tracks, the eventual orchestral challenge
material — original production is the primary path, not stock libraries.
That sidesteps every licensing question in `LICENSES.md` entirely (you own
it outright) and it's simply better content than anything licensed would
be. Stock/CC0 sources stay useful for quick placeholder work early on, but
they're not the long-term plan for anything that matters.
