# Legacy Jet3 Ecopath 5 import fix - full delivery

Extract this zip directly over your master checkout root - every path below
is relative to the repo root (`Sources/...`), matching the real layout.

## Files modified (already exist in your checkout)

- `Sources/EwECore/Database/cEwEDatabase.vb`
  Adds `Public Shared CompatibilityFromVersion(sVersion, sMaxDBVersion)`,
  extracted out of `Compatibility()` (which now just calls it) so a version
  number obtained a different way - from the x86 helper below - reaches
  the exact same TooOld/Importable/EwE6/Future verdict.

- `Sources/EwECore/Database/cEwEAccessDatabase.vb`
  Adds two `Public Shared` methods:
  - `IsLegacyJetFormat(path)` - reads the single Jet-version byte at file
    offset `0x14` directly (no connection attempt at all); True only for
    byte value `0` (genuine Jet3). Mapping confirmed against mdbtools' own
    source (`mdbtools/include/mdbtools.h.in`).
  - `TryGetLegacyVersionViaX86Helper(path, ByRef sVersion)` - spawns
    `EwEImportHelper.exe version "<path>"` (hidden window, redirected
    output, 15s timeout) and parses its single-line stdout.

- `Sources/EwECore/DataSource/cDatasourceFactory.vb`
  `GetCompatibility`'s Access-case branch: when `IsLegacyJetFormat` is True
  AND the current process is 64-bit, tries the x86 helper for the real
  version number and maps it via `CompatibilityFromVersion`; falls back to
  the conservative `TooOld` only if the helper is missing or fails. Every
  other case (Jet4+, ACCDB, 32-bit, non-legacy files) is completely
  unchanged - same `db.Open()`/`db.Compatibility` path as before.

- `Sources/ScientificInterface/frmEwE6.vb`
  Two separate changes, both in `CovertToEwE6`:
  1. The access-error gate now excludes the TooOld case (so a correctly-
     classified TooOld legacy file still reaches the real, correct
     message instead of a generic access-failure one).
  2. The `Case Importable` branch: when the file is confirmed Jet3 AND the
     process is 64-bit, skips `Import.dlgImportDatabase` entirely (it
     cannot open the file either - same ACE limitation) and calls
     `Import.cLegacyImportLauncher.TryRunFullImport` instead. Every other
     case (32-bit, or a non-Jet3 Importable file) is unchanged - same
     dialog as before.

- `Sources/ScientificInterface/ScientificInterface.vbproj`
  One added `<ProjectReference>` to the new `EwEImportHelper` project, so
  MSBuild copies its built `.exe` into this project's own output folder
  automatically.

## Solution file modified

- `Sources/EwE6.sln` - adds the new `EwEImportHelper` project:
  - A new `Project(...)` entry using `{F184B08F-C81C-45F6-A57F-5ABD9991F28F}`
    (the same project-type GUID `ScientificInterface`/`ScientificInterfaceShared`
    use - both single-`TargetFramework` SDK-style VB projects, confirmed
    directly against this solution file; `EwECore`/`EwEUtils` use a
    *different* GUID, `{778DAE3C-...}`, specifically because they're
    multi-targeted (`TargetFrameworks`, plural) - `EwEImportHelper` is
    single-targeted, so it needed the other one).
  - Twelve `ProjectConfigurationPlatforms` lines mapping all six solution
    configurations (Debug/Release x Any CPU/x64/x86) to `Debug|x86` or
    `Release|x86` respectively - so this project always builds as x86,
    regardless of which platform the solution itself is building.

  **⚠️ Unlike every other file here, this replaces your entire `EwE6.sln`
  wholesale rather than patching alongside your own copy** - if your local
  solution file has since diverged (other projects added/removed, local
  changes), extracting this will silently discard that. Worth diffing
  against your current `EwE6.sln` before overwriting, rather than
  assuming this one is a safe drop-in the way the rest of this zip is.

## Files added (new)

- `Sources/EwEImportHelper/EwEImportHelper.vbproj` - new project. net48,
  x86-only (the one combination with a classic Jet OLEDB provider
  available - the reason this helper exists at all). References EwECore
  the same way ScientificInterfaceShared.vbproj already does
  (`$(UseProjectReferences)`-gated local reference vs. NuGet package).

- `Sources/EwEImportHelper/Program.vb` - the helper itself, two modes:
  - `version "<source>"` - prints the real version number to stdout, used
    by `cDatasourceFactory.GetCompatibility` above.
  - `import "<source>" "<output folder>" "<result file>"` - runs the
    FULL Ecopath-5-to-EwE6 conversion in its own visible console window
    (creates the target, opens the source, converts, prints progress via
    the core's own publisher-subscriber messages, prompts with a plain
    numbered menu if the source has more than one model). Writes
    `OK`/path or `FAIL`/reason to the result file.

- `Sources/ScientificInterface/Other/Import/cLegacyImportLauncher.vb` -
  spawns the helper in `import` mode with a visible window (no timeout -
  this mode is interactive), reads the result file back.

## Build note

After extracting, `EwEImportHelper` needs to actually build as x86/net48
and its `.exe` needs to land next to `ScientificInterface.exe` - this is
new, multi-project MSBuild wiring that hasn't been compile-tested here.
If the ProjectReference doesn't resolve cleanly, that's the first thing
to check.

## Testing

1. Rebuild `ScientificInterface` (x64, since that's the whole point).
2. Open a genuine Jet3 Ecopath 5 file (confirmed via `mdb-ver`, byte 0 at
   offset `0x14`) with exactly one model - should show the real
   TooOld/Importable message, and for Importable, launch the helper's
   console window and complete the import automatically.
3. Repeat with a multi-model source file to exercise the numbered menu.
