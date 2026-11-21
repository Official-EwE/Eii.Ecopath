# EwEImportHelper — build & publish workflow

This tool lives under `Tools\EwEImportHelper\`, the same layout as
`Tools\mdb2sqlite` and `Tools\Eii.Ecopath.Storage.Migrator`:

- `Source\` — the buildable project (`EwEImportHelper.vbproj`)
- `Publish\win-x86\` — the committed, ready-to-ship output that
  `ScientificInterface.vbproj` copies into its own output directory at
  build time (via a `None`/`Link` item, same pattern as the other two
  tools)

It is **not** part of the main solution's build graph — `ScientificInterface.vbproj`
has no `ProjectReference` to it.

## Why

`EwEImportHelper` must run as a genuine x86 process (the classic `Microsoft.Jet.OLEDB.4.0`
provider it depends on has no 64-bit build), which means it must reference an x86 build of
`EwECore.dll`. But `EwECore` is multi-targeted and the rest of the solution builds it as
x64 (the real app's platform). Across repeated attempts — `SetPlatform="Platform=x86"` on
the project reference, an explicit pre-build task forcing a separate x86 build of `EwECore`
first, a post-build copy step forcibly overwriting the wrong-platform DLL — MSBuild, in
this solution, reliably builds `EwECore` in only whichever platform the *solution's own
active configuration* is set to, never an extra x86 copy alongside it. When
`EwEImportHelper` was part of the same build graph as the rest of the solution (building
as x64), its own reference to `EwECore` always resolved to that x64 DLL, which an x86
`EwEImportHelper.exe` then fails to load at runtime (`BadImageFormatException`).

Building this project **by itself**, in isolation, with the active configuration manually
set to x86 first, does not have this problem — there is only one platform in play for that
one build, so the normal `ProjectReference` resolves correctly. So its output is built that
way, once per change, and committed under `Publish\win-x86\`, exactly like
`Tools\Eii.Ecopath.Storage.Migrator\Publish\<rid>\` is already built and committed via
`dotnet publish -r <rid>`.

## When you need to rebuild and republish

Only when `Tools\EwEImportHelper\Source\Program.vb` (or anything else in this project)
changes. If you haven't touched this project, there's nothing to do.

## Steps

1. From the repo root, run:
   ```
   dotnet build Tools\EwEImportHelper\Source\EwEImportHelper.vbproj -c Debug -p:Platform=x86
   ```
   (Pick `Release` instead if that's what EwE's other published builds use — be consistent
   with whichever configuration `Eii.Ecopath.Storage.Migrator`'s own `Publish\` folder was
   built with.)
   This one invocation only ever touches `EwEImportHelper` and x86 copies of its own
   dependencies (`EwECore`, `EwEUtils`) — it does not affect the x64 outputs the rest of
   the solution normally builds, and nothing needs to be switched back afterward.
2. Copy the entire contents of `Tools\EwEImportHelper\Source\bin\x86\Debug\net48\`
   **except** the `.pdb` file into `Tools\EwEImportHelper\Publish\win-x86\`, replacing
   what's there. This should be exactly three files: `EwEImportHelper.exe`, `EwECore.dll`,
   `EwEUtils.dll` (net48 projects copy their direct dependencies' DLLs alongside the exe by
   default, so these three end up together in that one output folder with no extra steps).
3. Commit the updated contents of `Tools\EwEImportHelper\Publish\win-x86\` alongside your
   source change.

`ScientificInterface.vbproj` copies all of this into its own output directory
(`EwEImportHelper\` subfolder) on every build automatically from that point on — no further
action needed until the next time this project's source changes.
