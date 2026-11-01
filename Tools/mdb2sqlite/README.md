# mdb2sqlite

Convert `.mdb` or `.eweaccdb` (MS Access) files to SQLite.

On **Windows**, use the prebuilt `mdb2sqlite.exe`. On **Linux**, run the PowerShell script via `run-ps1.sh` (installs PowerShell automatically if needed).

## Requirements

| Platform | Required files |
|----------|----------------|
| Windows  | `mdb2sqlite.exe` + `sqlite3.exe` (both in the same folder) |
| Linux    | `mdb2sqlite.ps1` + `run-ps1.sh` — dependencies (`mdbtools`, `sqlite3`, PowerShell) are installed automatically |

`sqlite3.exe` for Windows can be downloaded from https://www.sqlite.org/download.html.

> [!WARNING]
> **Known issue: "...cannot be loaded... is not digitally signed..."**
>
> Running `mdb2sqlite.ps1` directly on Windows (e.g. `.\mdb2sqlite.ps1 -generateExe`, or `.\mdb2sqlite.ps1 -inDatabase ...`) may fail with:
> ```
> File ...\mdb2sqlite.ps1 cannot be loaded. The file ...\mdb2sqlite.ps1 is not
> digitally signed. You cannot run this script on the current system.
> ```
> This is Windows PowerShell's default execution policy blocking unsigned local scripts - unrelated to this script specifically, and does not affect the prebuilt `mdb2sqlite.exe`. Fix it either:
> - **Once, for just that invocation:**
>   ```powershell
>   powershell -ExecutionPolicy Bypass -File .\mdb2sqlite.ps1 -generateExe
>   ```
> - **Permanently, for your user account** (no admin rights needed; applies to this and any other local `.ps1` script going forward):
>   ```powershell
>   Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
>   ```

## Usage

### Windows — using the `.exe`

Convert with auto-generated output name (same path, `.sqlite` extension):
```cmd
mdb2sqlite.exe -inDatabase "C:\path\to\input.eweaccdb"
```

Convert with an explicit output path:
```cmd
mdb2sqlite.exe -inDatabase "C:\path\to\input.eweaccdb" -outDatabase "C:\path\to\output.sqlite"
```

### Windows — using the `.ps1` directly

```powershell
.\mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb"
```

### Linux — using `run-ps1.sh`

> Run with `sudo` so the script can install dependencies if needed.

Convert with auto-generated output name:
```sh
sudo ./run-ps1.sh mdb2sqlite.ps1 -inDatabase "/path/to/input.eweaccdb"
```

Convert with an explicit output path:
```sh
sudo ./run-ps1.sh mdb2sqlite.ps1 -inDatabase "/path/to/input.eweaccdb" -outDatabase "/path/to/output.sqlite"
```

## Viewing the output

To inspect the resulting `.sqlite` file, use **DB Browser for SQLite**:  
https://sqlitebrowser.org/dl/

## Regenerating the `.exe` (Windows only)

The `.exe` is compiled from `mdb2sqlite.ps1` using [ps2exe](https://github.com/MScholtes/PS2EXE). To recompile:

```powershell
.\mdb2sqlite.ps1 -generateExe
```

This installs the `ps2exe` module if needed and writes `mdb2sqlite.exe` next to the script.

## How it works

1. **mdbtools** (`mdb-schema`, `mdb-tables`, `mdb-export`) reads the Access database and generates SQL.
2. The SQL is written to a temporary `.Cache/conversion.sql` file.
3. **sqlite3** imports the SQL into a new `.sqlite` database.

On Windows, mdbtools binaries are downloaded automatically from [mdbtools-win](https://github.com/lsgunth/mdbtools-win) and cached in `.Cache/mdbtools-win/`.  
On Linux, mdbtools is built from source if the system version is too old (requires v1.0.1+).

## Links

- [mdbtools](https://github.com/mdbtools/mdbtools) — source of the Access conversion tools
- [mdbtools-win](https://github.com/lsgunth/mdbtools-win) — Windows build of mdbtools
- [SQLite tools](https://www.sqlite.org/download.html) — source of `sqlite3.exe`
- [DB Browser for SQLite](https://sqlitebrowser.org/dl/) — GUI client for viewing `.sqlite` files
