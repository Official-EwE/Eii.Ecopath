# Local staging copy: release overview

Ecopath with Ecosim - summary for stakeholders

## What it does

- **Staged working copy.** A model opened for editing is worked on as a private copy in `%LocalAppData%\Ecopath with Ecosim\DBs`. This stops cloud-sync clients (OneDrive, Dropbox) rewriting an open database mid-save and corrupting it.
- **Safe publish on Save.** Save copies the finished copy next to the real file, then swaps it in. A crash mid-save cannot corrupt the original.
- **Locked file handling.** If the real file is locked, for example by a sync client, Save retries briefly. It then shows a dialog naming the locking application, with Retry and Cancel.
- **Orphan cleanup.** Each staged copy has a marker file. Copies left behind by a crashed session are deleted on the next start, and the app removes its own copy on exit. After a crash the user reopens the last saved version; recovery of unsaved work is not included.
- **Save As and read-only.** Save As re-stages the new file; read-only opens skip staging.
- **No multi-user protection.** The exclusive-lock mode is removed. Two sessions on one file each work on their own copy and the last save wins. Avoiding concurrent edits is the user's responsibility.
- **Old Ecopath 5 models.** Very old (Jet3) files get a clear "outdated database" message. On 64-bit they can be imported through a small x86 helper tool.

## Quality

- An automated PowerShell and UI Automation test suite covers the test plan on x64 and x86.
- The last full run passed on both platforms, apart from a known x86 timing flake in the orphan-sweep test 4.3 (not yet root-caused) and the three conversion tests that report the known import bugs below.

## Bugs found along the way

### Also on master

- **Old Ecopath 5 files were reported as "already in use". Fixed** (details in the call-out below).
- **Three import bugs** reported by the conversion tests: `SalinityForcingShapeID` in gulfmexico, a null `Target` in Bass and `EatEffBad` in GBR. These come from the import path rather than staging; not fixed yet.

### Found and fixed in the new code

- A cancelled publish dialog left the Save button disabled and the app acting as if the save had worked, although the real file was not updated.

> **Old Ecopath 5 files reported as "already in use" - fixed**
>
> - On 64-bit, every `.mdb`/`.accdb` is opened through the ACE database provider, because no 64-bit version of the classic Jet provider exists.
> - ACE cannot open a genuine Jet3 (Ecopath 5-era) file at all. Opening fails with a generic error (`E_FAIL`).
> - That generic error was mapped unconditionally to "already in use", so users saw a misleading message for a file that was not locked.
> - Fix: the Jet3 format is detected from the file header before opening, and the correct "outdated Ecopath 5 database" message is shown.

## Known limitations

- No recovery of unsaved work after a crash.
- Last save wins when two people edit the same file.
- The three import bugs remain open.

## Separate track

SQLite support and automatic Access-to-SQLite conversion live on the `cEwEEFDatabase` branch and are not part of this release.
