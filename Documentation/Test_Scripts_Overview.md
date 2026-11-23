# Test scripts overview

Tests per script, run-time estimates, command-line arguments and examples.

## 1. Tests by script

`Run-AllTests.ps1` runs all of these in order (conversion handling first), once per platform. Estimated times are from the last full x64 log and include app start-up and shutdown. x86 times were not in that log; `n/a` marks tests that were not in the log (x86-only wizard tests, or tests added since).

| Test | What it checks | Script | Est. time (x64) |
|---|---|---|---|
| 1.1 - A staged working copy is created on open | Opening a model for editing creates a Temp... working copy plus marker in the staging folder. | `Run-StagingTests.ps1` | ~8s |
| 7.1 - Opening a file needing legacy updates never touches its own timestamp | The legacy update chain runs on the staged copy; the real file's timestamp stays unchanged. | `Run-StagingTests.ps1` | ~7s |
| 1.1 (rename-while-open) - Renaming the original while open has zero effect until Save | Renaming the original file while the model is open changes nothing until Save. | `Run-StagingTests.ps1` | ~12s |
| 2.x - Edit and Save completes, Save becomes disabled afterward | Edits a value, saves, and checks the save completes and the Save button is disabled again. | `Run-StagingTests.ps1` | ~33s |
| 2.3 - Lock-contention dialog (long hold) | Manual-test marker: a human holds the file locked and checks the OK/Cancel dialog. Reported as a pass-through. | `Run-StagingTests.ps1` | manual |
| 2.3-short - A short lock hold resolves silently via internal retry | A brief lock on the real file is absorbed by the internal retry with no dialog. | `Run-StagingTests.ps1` | ~15s |
| 8.1 - A cancelled lock-contention save correctly reports failure | Cancelling the lock dialog must report a failed save and keep Save enabled (no silent data loss). | `Run-StagingTests.ps1` | ~17s |
| 6.1 - Save As creates a new file at the target path | Save As writes a new file at the chosen path and re-stages it. | `Run-StagingTests.ps1` | ~26s |
| 2.1 - Kill mid-save (after save, before publish-back) | Kills the process between saving and publishing; the original file must be untouched. | `Run-CrashRecoveryTests.ps1` | ~36s |
| O.3 - Paths with spaces and non-ASCII characters work correctly | Staging, saving and cleanup work for model paths containing spaces and non-ASCII characters. | `Run-CrashRecoveryTests.ps1` | ~20s |
| 4.1 - Orphaned working copy is swept on the next write-intent open | A copy left by a dead session is deleted the next time a model is opened for editing. | `Run-CrashRecoveryTests.ps1` | ~35s |
| 4.3 - Several orphaned copies are all cleared in one sweep pass | Several orphaned copies are all removed in a single sweep. | `Run-CrashRecoveryTests.ps1` | ~62s |
| 5.1/5.2 - Several sessions in one app run, all cleaned up on final graceful close | Opens several models in one app run and checks all staged copies disappear on a graceful close. | `Run-CrashRecoveryTests.ps1` | ~48s |
| 5.4 - A hard-killed process's own copy is NOT cleaned up by exit-time cleanup | After a hard kill, the exit-time cleanup must not delete that session's copy; only the next sweep may. | `Run-CrashRecoveryTests.ps1` | ~17s |
| 1.2 - Two instances can open the same file simultaneously, no degrade | Two app instances open the same file without errors, each with its own staged copy. | `Run-MultiInstanceTests.ps1` | ~43s |
| 6.2 - Save As onto a path another instance already has open | Save As onto a file another instance has open, including the Windows overwrite prompt and the lock-contention dialog. | `Run-MultiInstanceTests.ps1` | ~45s |
| 1.3 - A caller that never sets EnableLocalStaging creates no working copy | Headless EwECore caller with staging off must not create any staged copy. | `Run-NoStagingCallerTests.ps1` | ~5s |
| Conversion: Gironde estuary.MDB imports successfully (Import-x64Helper) | Jet3 file converted through the x86 helper's console menu (x64 app). | `Run-ConversionHandlingTests.ps1` | ~34s |
| Conversion: gulfmexico.MDB imports successfully (Import-x64Helper) | Same route; currently fails on a known import bug (SalinityForcingShapeID). | `Run-ConversionHandlingTests.ps1` | ~28s |
| Conversion: Gironde estuary.MDB imports successfully (Import-Wizard32bitJet3) | Jet3 file imported through the in-process import wizard (x86 app only). | `Run-ConversionHandlingTests.ps1` | n/a |
| Conversion: gulfmexico.MDB imports successfully (Import-Wizard32bitJet3) | Wizard route, x86 only. | `Run-ConversionHandlingTests.ps1` | n/a |
| Conversion: Kenya, Lake Nakuru - 1972.mdb refers to EwE5 (TooOld-Jet3x64-confirmed) | Too-old Jet3 file shows the referral to EwE5 dialog on 64-bit. | `Run-ConversionHandlingTests.ps1` | ~11s |
| Conversion: EcopathMSY.MDB refers to EwE5 (TooOld-DirectOpen) | Too-old file opened directly shows the referral dialog. | `Run-ConversionHandlingTests.ps1` | ~11s |
| Conversion: EcopathBass - Ecopath.MDB imports the first model via the wizard (Import-WizardNonJet3) | First model of a multi-model file through the wizard; currently fails on a known bug (null Target). | `Run-ConversionHandlingTests.ps1` | ~23s |
| Conversion: EcopathBeam2000 - Ecopath.MDB imports the second model via the wizard (Import-WizardNonJet3) | Second model of the same file through the wizard. | `Run-ConversionHandlingTests.ps1` | ~18s |
| Conversion: GBR_Linked_ecosystem.MDB - first submodel imports via the helper (Import-x64Helper) | Linked-ecosystem file via the helper; currently fails on a known bug (EatEffBad). | `Run-ConversionHandlingTests.ps1` | ~33s |
| Conversion: GBR_Linked_ecosystem.MDB - first submodel imports via the wizard (Import-Wizard32bitJet3) | Wizard route, x86 only. | `Run-ConversionHandlingTests.ps1` | n/a |

**Sum of the timed tests: about 9 minutes 47 seconds on x64** (excluding the untimed x86-only tests; the 6.2 figure comes from a run that waited for a timeout, so a passing run is likely a bit faster).

### Run-ConversionTests.ps1 (generic sweep)

Not a fixed list: it classifies every legacy `.mdb` in a folder (Jet3 or not, Ecopath version, number of models) and runs one conversion test per model. By default it is headless and converts through the helper's import mode; with `-Ui` it drives the real app. Run time depends on the folder and was not in the log.

### SQLite branch only (cEwEEFDatabase): Run-SqliteTests.ps1

Headless: no app and no UI automation. It builds ScientificInterface once per PowerShell session, then loads a private copy of the build output. Times from the last run (x64).

| Test | What it checks | Script | Est. time |
|---|---|---|---|
| 1.18a - Conversion of 'Anchovy Bay Spatial.ewemdb' produces a .ewesqlite | Converts a copy of the sample model; output must use the .ewesqlite extension. | `Run-SqliteTests.ps1` | ~7s |
| 1.18 - the converted .ewesqlite is detected as Sqlite and opens | The converted file maps to the Sqlite type and loads. | `Run-SqliteTests.ps1` | ~4s |
| 4.1/4.2 - every Access table exists in the .ewesqlite with the same row count | Compares every table's row count, Access source against SQLite result. | `Run-SqliteTests.ps1` | ~4s |
| 2.1 - Save round-trip of a .ewesqlite keeps every table's row count | Load, save, reload; row counts unchanged. | `Run-SqliteTests.ps1` | ~9s |
| 2.2 - Save As to an Access extension is refused | Save As to .ewemdb must be refused and write no file. | `Run-SqliteTests.ps1` | ~5s |
| 9.3 - CanCompact is False for SQLite | SQLite data sources report that compacting is not possible. | `Run-SqliteTests.ps1` | ~4s |
| 4.4 - GetSupportedType per extension | Extension to data-source type mapping, including a sibling .ewesqlite. | `Run-SqliteTests.ps1` | <1s |
| 1.19 - Garbage .ewesqlite next to an Access file | Characterisation: a 0-byte .ewesqlite is treated as converted and fails to open. | `Run-SqliteTests.ps1` | ~2s |
| 1.20 - Conversion with the mdb2sqlite folder missing gives a clear error | Hides the converter tool and expects a clear error. | `Run-SqliteTests.ps1` | <1s |
| 3.4 - Legacy updates under nl-NL give the same data as under en-US | Compares UpdateLog, EcopathModel and EcopathGroup between cultures (new, not yet run). | `Run-SqliteTests.ps1` | n/a |
| 6.6 / 6.8 / 6.7 - Tool folders, installer association, localization TODO report | Static checks on build output and installer; 6.7 only reports a count. | `Run-SqliteTests.ps1` | <1s |

## 2. Command-line arguments

Not every script accepts every argument; the table covers the scripts as a group. Run the scripts from the repository root, for example `.\Scripts\Run-AllTests.ps1`.

| Argument | Values | Meaning |
|---|---|---|
| `-Platform` | x64 \| x86 (default x64) | Which build the test launches. Individual runners take one value. |
| `-Platforms` | x64, x86 (default both) | Run-AllTests only: which platforms to run. Pass one value to skip the other. |
| `-Only "text"` | string | Run only tests whose name contains the text (case-insensitive). Not supported by Run-AllTests. |
| `-From "text"` | string | Skip every test until the first whose name contains the text, then run it and everything after. Names are unique, so "6.2" works. |
| `-PauseOnFailure` | switch | After a failing test, wait for Enter before continuing, so you can inspect the screen. |
| `-MainWindowTimeoutSeconds` | number (default 90) | How long each app instance waits for its main window before giving up. |
| `-ModelsFolder` | path | Run-ConversionTests: folder of legacy .mdb files (asked for, and remembered, if omitted). Run-SqliteTests: run every Access model in the folder. |
| `-Recurse` | switch | Run-ConversionTests: also pick up .mdb files in subfolders. |
| `-PlanOnly` | switch | Run-ConversionTests: classify the files and print the plan without running any test. |
| `-Ui` | switch | Run-ConversionTests: drive the real app instead of the default headless conversion (slow, one app instance per model). |
| `-SourceModelPath` | path | Sample model for the headless runners. Default: Database\Anchovy Bay Spatial.ewemdb. |
| `-Configuration` | Debug \| Release (default Debug) | Build configuration for the headless runners. |
| `-RepoRoot` | path | Repository root; defaults to the folder above Scripts. |
| `-ForceBuild` | switch | Run-SqliteTests: rebuild even if this PowerShell session already built the app. |

**Also:** the scripts relaunch themselves elevated (a UAC prompt appears) because UI automation cannot drive an elevated app from a normal window. Set `$env:EWE_TEST_NO_ELEVATE=1` to skip this. `Run-StagingTests` also takes internal arguments (`-Proc`, `-ModelPath`, `-InitialModelTimestamp`, `-StagedCopyPath`) that `Run-AllTests` passes on; you normally never set them. The app is built once per PowerShell session; use a new window after changing source code.

## 3. Examples

### Run-AllTests.ps1

Everything, both platforms (about as long as the sum above, twice, plus the x86 wizard tests):

```powershell
.\Scripts\Run-AllTests.ps1
```

x64 only:

```powershell
.\Scripts\Run-AllTests.ps1 -Platforms x64
```

x86 only:

```powershell
.\Scripts\Run-AllTests.ps1 -Platforms x86
```

Resume from test 6.2 on x64, skipping everything before it:

```powershell
.\Scripts\Run-AllTests.ps1 -Platforms x64 -From "6.2"
```

Stop and wait for Enter after each failure:

```powershell
.\Scripts\Run-AllTests.ps1 -Platforms x64 -PauseOnFailure
```

\

```powershell
\
```

### Run-ConversionTests.ps1 (separate from Run-AllTests)

Show the plan for a folder of legacy models without running anything:

```powershell
.\Scripts\Run-ConversionTests.ps1 -ModelsFolder D:\Downloads\EwE5Databases -PlanOnly
```

Headless conversion of every model in the folder, including subfolders:

```powershell
.\Scripts\Run-ConversionTests.ps1 -ModelsFolder D:\Downloads\EwE5Databases -Recurse
```

Drive the real x86 app for one model only:

```powershell
.\Scripts\Run-ConversionTests.ps1 -ModelsFolder D:\Downloads\EwE5Databases -Ui -Platform x86 -Only "Gironde"
```

### A single group

Only the crash and cleanup tests whose name contains 4.3:

```powershell
.\Scripts\Run-CrashRecoveryTests.ps1 -Platform x64 -Only "4.3"
```
