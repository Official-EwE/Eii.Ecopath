<#
.SYNOPSIS
    Headless SQLite-transition tests (SQLiteTransitionTestPlan.md): no app, no UI Automation. Loads a private copy of
    the EwECore build output and drives cCore directly, like Run-NoStagingCallerTests.ps1.

.DESCRIPTION
    Plan items covered (the number is in each test name):
      1.18/1.18a  Access -> .ewesqlite conversion produces a .ewesqlite, re-open reuses it
      4.1/4.2     Data fidelity: every Access table exists in the .ewesqlite with the same row count
      1.19        Characterisation: a garbage .ewesqlite next to an Access file (reports what happens)
      1.20        Conversion with the mdb2sqlite tool folder missing -> clear error
      2.1         Save round-trip of a .ewesqlite
      2.2         Save As to an Access extension is refused
      2.3         Save As to a .ewesqlite keeps the data and re-targets the staged working copy
      4.4         cDataSourceFactory.GetSupportedType per extension
      9.3         CanCompact is False for SQLite
      3.4         Legacy update chain under nl-NL vs en-US gives identical data
      6.7         Report of 'Todo: localize the message' markers
      6.x         Static: mdb2sqlite + migrator tools in the build output, installer registers .ewesqlite
    Fixture: <repo>\Database\Anchovy Bay Spatial.ewemdb by default; -SourceModelPath picks another file and
    -ModelsFolder runs every *.ewemdb / *.eweaccdb / *.mdb / *.accdb in a folder. The fidelity and conversion tests run once per fixture. First-ever conversion on a machine needs
    internet (mdb2sqlite downloads mdbtools); a network failure is reported as a failure of that test.

    The PowerShell process bitness must match -Platform (x86 needs 32-bit PowerShell).

.PARAMETER Only
    Run only tests whose name contains this text.
.PARAMETER From
    Skip every test before the first one whose name contains this text.
.PARAMETER PauseOnFailure
    Wait for Enter after a failing test.
.EXAMPLE
    .\Run-SqliteTests.ps1 -Only "4.1"
#>
param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Configuration = "Debug",
    [string]$ModelsFolder,
    [string]$SourceModelPath,
    [string]$Only,
    [switch]$PauseOnFailure,
    [string]$From,
    # Rebuild even if this PowerShell session already built the app.
    [switch]$ForceBuild
)

if ($PauseOnFailure) { $env:EWE_TEST_PAUSE_ON_FAIL = "1" }
if ($From) { $env:EWE_TEST_FROM = $From }
. (Join-Path $PSScriptRoot "TestHelpers.ps1")
$runStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$script:TestNameFilter = $Only

# ---- fixtures -------------------------------------------------------------------------------------------------------
# Default: just the sample model every other runner uses. -ModelsFolder opts in to a whole folder.
if (-not $ModelsFolder -and -not $SourceModelPath) { $SourceModelPath = Join-Path $RepoRoot "Database\Anchovy Bay Spatial.ewemdb" }
$fixtures = @()
if ($SourceModelPath) { $fixtures = @(Get-Item $SourceModelPath) }
elseif (Test-Path $ModelsFolder) {
    $fixtures = @(Get-ChildItem $ModelsFolder -File | Where-Object { $_.Extension -in ".ewemdb", ".eweaccdb", ".mdb", ".accdb" })
}
if ($fixtures.Count -eq 0) { throw "No Access fixtures found in '$ModelsFolder' (use -ModelsFolder or -SourceModelPath)." }

# The app's output folder is the shipped layout: EwECore.dll plus the mdb2sqlite\ and migrator\ tool folders that the
# converter expects next to it. EwECore's own output has no tools, so it is only a fallback.
# Build once per PowerShell session, using the same session table as Start-TestInstance.ps1: a stale output folder
# (e.g. built on another branch) has no SQLite classes at all. Building ScientificInterface also builds EwECore.
$siProj = Join-Path $RepoRoot "Sources\ScientificInterface\ScientificInterface.vbproj"
if (-not $global:EwEBuiltProjects) { $global:EwEBuiltProjects = @{} }
$buildKey = "$siProj|$Configuration|$Platform".ToLowerInvariant()
if ($global:EwEBuiltProjects[$buildKey] -and -not $ForceBuild) {
    Write-Host "Already built in this session ($Configuration|$Platform) - skipping the build (use -ForceBuild to rebuild)."
} else {
    Write-Host "Building ScientificInterface ($Configuration|$Platform)..."
    dotnet build $siProj -c $Configuration -p:Platform=$Platform | ForEach-Object { Write-Host $_ }
    if ($LASTEXITCODE -ne 0) { throw "Build failed (dotnet build exit code $LASTEXITCODE)." }
    $global:EwEBuiltProjects[$buildKey] = $true
}
$binFolder = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48"
if (-not (Test-Path (Join-Path $binFolder "EwECore.dll"))) {
    $binFolder = Join-Path $RepoRoot "Sources\EwECore\bin\$Platform\$Configuration\net48"
}
$workRoot = Join-Path $env:TEMP "EwE-TestRun\Sqlite-$(Get-Random)"
New-Item -ItemType Directory -Path $workRoot -Force | Out-Null
$script:CoreBin = $null

# ---- helpers --------------------------------------------------------------------------------------------------------
# Loads a PRIVATE COPY of the build output: the converter locates mdb2sqlite/migrator relative to the loaded EwECore.dll,
# which a byte-array load cannot provide, and a copy avoids locking the real output folder.
function Initialize-EwECore {
    if ($script:CoreBin) { return }
    $dll = Join-Path $binFolder "EwECore.dll"
    if (-not (Test-Path $dll)) {
        Write-Step "Building EwECore ($Configuration|$Platform)" {
            dotnet build (Join-Path $RepoRoot "Sources\EwECore\EwECore.vbproj") -c $Configuration -p:Platform=$Platform | ForEach-Object { Write-Host $_ }
            if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }
        }
    }
    Assert-True (Test-Path $dll) "EwECore.dll not found at $dll"
    $script:CoreBin = Join-Path $workRoot "bin"
    Write-Step "Copying the build output to a private folder" { Copy-Item $binFolder $script:CoreBin -Recurse -Force }
    # LoadFile, not LoadFrom: LoadFrom hands back an already-loaded same-identity EwECore (e.g. from an earlier run in
    # this PowerShell session) instead of this private copy. LoadFile always loads the given file and sets Location.
    # Resolver is plain C#, not a PowerShell scriptblock: a scriptblock handler re-enters the PowerShell engine from inside
    # assembly loading, which recursed until the process died with a StackOverflowException.
    if (-not ("EwETestPrivateResolver" -as [type])) {
        Add-Type -TypeDefinition @"
using System; using System.Collections.Generic; using System.IO; using System.Reflection;
public static class EwETestPrivateResolver {
    public static string Folder;
    static readonly Dictionary<string, Assembly> Cache = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
    static bool installed;
    public static void Install(string folder) {
        Folder = folder; Cache.Clear();
        if (!installed) { AppDomain.CurrentDomain.AssemblyResolve += Resolve; installed = true; }
    }
    static Assembly Resolve(object sender, ResolveEventArgs e) {
        string name = new AssemblyName(e.Name).Name;
        if (name.EndsWith(".resources") || name.EndsWith(".XmlSerializers")) return null;
        lock (Cache) {
            Assembly a;
            if (Cache.TryGetValue(name, out a)) return a;
            Cache[name] = null;                       // stops recursion
            string path = Path.Combine(Folder, name + ".dll");
            if (!File.Exists(path)) return null;
            a = Assembly.LoadFile(path);
            Cache[name] = a;
            return a;
        }
    }
}
"@
    }
    [EwETestPrivateResolver]::Install($script:CoreBin)
    $script:CoreAsm = [Reflection.Assembly]::LoadFile((Join-Path $script:CoreBin "EwECore.dll"))
}

# Finds a type by simple name in OUR private copy of EwECore.dll only: earlier runs in the same PowerShell session
# leave older same-named EwECore assemblies loaded, and a plain type-name lookup would pick one of those.
function Get-EwEType {
    param([Parameter(Mandatory)][string]$Name)
    Initialize-EwECore
    $asm = $script:CoreAsm
    if (-not $asm) { throw "Private EwECore.dll is not loaded from $script:CoreBin" }
    $err = $null
    try { $types = $asm.GetTypes() }
    catch [System.Reflection.ReflectionTypeLoadException] {
        $types = $_.Exception.Types | Where-Object { $_ }
        $err = ($_.Exception.LoaderExceptions | Select-Object -First 3 | ForEach-Object { $_.Message }) -join " | "
    }
    $t = $types | Where-Object { $_.Name -eq $Name } | Select-Object -First 1
    if ($t) { return $t }
    $like = ($types | Where-Object { $_.Name -like "*Sqlite*" -or $_.Name -like "*Mdb2*" } | ForEach-Object { $_.FullName }) -join ", "
    Write-Host "    $($types.Count) types in $($asm.Location); loader errors: $err; Sqlite/Mdb2 types: $like" -ForegroundColor Yellow
    throw "Type '$Name' not found in $($asm.Location) - is the build output ($binFolder) up to date? Rebuild EwECore ($Configuration|$Platform)."
}

# Calls a public static method; unwraps TargetInvocationException so callers see the real error.
function Invoke-EwEStatic {
    param([string]$TypeName, [string]$Method, [object[]]$Arguments)
    $t = Get-EwEType $TypeName
    # Values coming out of cmdlets are PSObject-wrapped; reflection needs the raw objects. Plain loop on purpose.
    $argList = @($Arguments)
    $raw = New-Object 'object[]' $argList.Count
    for ($i = 0; $i -lt $argList.Count; $i++) {
        $v = $argList[$i]
        # .psobject.BaseObject works for every value; a bare .BaseObject is $null on a plain string.
        if ($null -ne $v) { $v = $v.psobject.BaseObject }
        $raw[$i] = $v
    }
    try { return $t.GetMethod($Method).Invoke($null, [object[]]$raw) }
    catch {
        # Reflection and PowerShell both wrap the real exception; surface the innermost one.
        $inner = $_.Exception
        while ($inner.InnerException) { $inner = $inner.InnerException }
        throw $inner
    }
}

function New-WorkCopy {
    param([Parameter(Mandatory)][string]$Source, [string]$Label = "m")
    $dir = Join-Path $workRoot "$Label-$(Get-Random)"
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
    $dest = Join-Path $dir (Split-Path -Leaf $Source)
    Copy-Item $Source $dest -Force
    Set-ItemProperty $dest -Name IsReadOnly -Value $false
    $dest
}

function Open-Model {
    param([Parameter(Mandatory)][string]$Path)
    Initialize-EwECore
    $core = [Activator]::CreateInstance((Get-EwEType 'cCore'))
    if (-not $core.LoadModel($Path)) { try { $core.Dispose() } catch {}; throw "LoadModel() returned False for $Path" }
    $core
}
function Close-Model { param($Core) try { $Core.CloseModel() | Out-Null; $Core.Dispose() } catch {} }

# Table name -> row count, read through the data source's own open connection.
function Get-TableCounts {
    param([Parameter(Mandatory)]$Core)
    $conn = $Core.DataSource.Connection.GetConnection()   # DataSource wraps the cEwEDatabase
    if ($conn.State -ne [System.Data.ConnectionState]::Open) { $conn.Open() }
    $names = @()
    if ($conn.GetType().Name -like "*Sqlite*" -or $Core.DataSource.Connection.GetType().Name -like "*EF*") {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'"
        $r = $cmd.ExecuteReader(); while ($r.Read()) { $names += [string]$r[0] }; $r.Close()
    } else {
        $schema = $conn.GetSchema("Tables")
        foreach ($row in $schema.Rows) {
            if ($row["TABLE_TYPE"] -eq "TABLE" -and $row["TABLE_NAME"] -notlike "MSys*" -and $row["TABLE_NAME"] -notlike "~*") { $names += [string]$row["TABLE_NAME"] }
        }
    }
    $counts = @{}
    foreach ($n in $names) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT COUNT(*) FROM [$n]"
        try { $counts[$n.ToLowerInvariant()] = [int64]$cmd.ExecuteScalar() } catch { $counts[$n.ToLowerInvariant()] = -1 }
    }
    $counts
}

# All rows of a table as invariant-culture strings, sorted: comparable across runs and cultures.
function Get-TableDump {
    param([Parameter(Mandatory)]$Core, [Parameter(Mandatory)][string]$Table)
    $conn = $Core.DataSource.Connection.GetConnection()
    if ($conn.State -ne [System.Data.ConnectionState]::Open) { $conn.Open() }
    $cmd = $conn.CreateCommand(); $cmd.CommandText = "SELECT * FROM [$Table]"
    $r = $cmd.ExecuteReader(); $rows = @()
    while ($r.Read()) {
        $vals = for ($i = 0; $i -lt $r.FieldCount; $i++) {
            $v = $r.GetValue($i)
            if ($v -is [DBNull]) { "<null>" } else { [Convert]::ToString($v, [Globalization.CultureInfo]::InvariantCulture) }
        }
        $rows += ($vals -join "|")
    }
    $r.Close()
    $rows | Sort-Object
}

# Mirrors the app (frmEwE6.LoadEcopathModelHandlingConversion): load once so the legacy update chain runs on the
# copy, then run the converter. Returns the .ewesqlite path and the Access table counts taken after the updates.
function Convert-Fixture {
    param([Parameter(Mandatory)][string]$AccessCopy)
    $core = Open-Model -Path $AccessCopy
    try { $accessCounts = Get-TableCounts -Core $core } finally { Close-Model $core }
    $out = [IO.Path]::ChangeExtension($AccessCopy, ".ewesqlite")
    Invoke-EwEStatic 'cMdb2SqliteConverter' 'ConvertMdbToSqlite' ($AccessCopy, $out)
    [pscustomobject]@{ SqlitePath = $out; AccessCounts = $accessCounts }
}

# Per-fixture conversion is expensive, so it is done once and shared by the tests below.
$script:Converted = @{}
function Get-ConvertedFixture {
    param([Parameter(Mandatory)]$Fixture)
    if (-not $script:Converted.ContainsKey($Fixture.FullName)) {
        $copy = New-WorkCopy -Source $Fixture.FullName -Label "conv"
        $script:Converted[$Fixture.FullName] = Convert-Fixture -AccessCopy $copy
    }
    $script:Converted[$Fixture.FullName]
}

# Fixtures that cannot be loaded at all (e.g. deliberately broken ones) are skipped, not failed: this suite tests conversion.
Initialize-EwECore
$loadable = @()
foreach ($fx in $fixtures) {
    $probe = New-WorkCopy -Source $fx.FullName -Label "probe"
    try { $c = Open-Model -Path $probe; Close-Model $c; $loadable += $fx }
    catch {
        Write-Host ("Skipping fixture '{0}': cannot be loaded ({1})" -f $fx.Name, $_.Exception.Message) -ForegroundColor Yellow
        if (-not $Only -and -not $env:EWE_TEST_FROM) { Add-TestResult -Name "Fixture '$($fx.Name)' (not loadable)" -Result "SKIP" }
    }
}
$fixtures = $loadable
if ($fixtures.Count -eq 0) { throw "None of the Access fixtures could be loaded." }
$firstFixture = $fixtures[0]

# ---- 1.18 / 1.18a / 4.1 / 4.2 per fixture ---------------------------------------------------------------------------
foreach ($fx in $fixtures) {
    Invoke-Test "1.18a - Conversion of '$($fx.Name)' produces a .ewesqlite" {
        $c = Write-Step "Converting a copy" { Get-ConvertedFixture -Fixture $fx }
        Assert-True (Test-Path $c.SqlitePath) "No output file at $($c.SqlitePath)"
        Assert-True ($c.SqlitePath.EndsWith(".ewesqlite")) "Output does not use the .ewesqlite extension: $($c.SqlitePath)"
        Write-Host ("    Source '{0}' ({1}) -> '{2}'" -f $fx.Name, $fx.Extension, (Split-Path -Leaf $c.SqlitePath)) -ForegroundColor Cyan
    }

    Invoke-Test "1.18 - '$($fx.Name)': the converted .ewesqlite is detected as Sqlite and opens (redirect itself is UI-only, see Run-ConversionHandlingTests)" {
        $c = Get-ConvertedFixture -Fixture $fx
        $accessCopy = [IO.Path]::ChangeExtension($c.SqlitePath, [IO.Path]::GetExtension($fx.Name))
        $type = (Invoke-EwEStatic 'cDataSourceFactory' 'GetSupportedType' @($c.SqlitePath)).ToString()
        Assert-True ($type -eq "Sqlite") "A .ewesqlite maps to '$type', expected Sqlite."
        Assert-True (Test-Path $accessCopy) "Access copy missing: $accessCopy"
        $core = Write-Step "Loading the converted file" { Open-Model -Path $c.SqlitePath }
        Close-Model $core
    }

    Invoke-Test "4.1/4.2 - '$($fx.Name)': every Access table exists in the .ewesqlite with the same row count" {
        $c = Get-ConvertedFixture -Fixture $fx
        $core = Write-Step "Loading the converted file" { Open-Model -Path $c.SqlitePath }
        try { $sqliteCounts = Get-TableCounts -Core $core } finally { Close-Model $core }
        $problems = @()
        foreach ($t in ($c.AccessCounts.Keys | Sort-Object)) {
            if (-not $sqliteCounts.ContainsKey($t)) { $problems += "missing table '$t' (Access rows: $($c.AccessCounts[$t]))"; continue }
            if ($sqliteCounts[$t] -ne $c.AccessCounts[$t]) { $problems += "'$t': Access $($c.AccessCounts[$t]) rows, SQLite $($sqliteCounts[$t])" }
        }
        Write-Host ("    {0} Access tables, {1} SQLite tables, {2} difference(s)" -f $c.AccessCounts.Count, $sqliteCounts.Count, $problems.Count) -ForegroundColor Cyan
        Assert-True ($problems.Count -eq 0) ("Differences:`n      " + ($problems -join "`n      "))
    }
}

# ---- 2.1 / 2.2 / 9.3 ------------------------------------------------------------------------------------------------
Invoke-Test "2.1 - Save round-trip of a .ewesqlite keeps every table's row count" {
    $c = Get-ConvertedFixture -Fixture $firstFixture
    $work = New-WorkCopy -Source $c.SqlitePath -Label "save"
    $core = Write-Step "Loading a copy" { Open-Model -Path $work }
    try {
        $before = Get-TableCounts -Core $core
        $saved = Write-Step "Saving" { $core.Save() }
        Assert-True $saved "Save() returned False."
    } finally { Close-Model $core }
    $core = Open-Model -Path $work
    try { $after = Get-TableCounts -Core $core } finally { Close-Model $core }
    $diff = $before.Keys | Where-Object { $after[$_] -ne $before[$_] }
    Assert-True (-not $diff) ("Row counts changed by Save+reload in: " + ($diff -join ", "))
    # NOTE: Save() without edits; editing + save through the real UI is covered by Run-StagingTests.ps1 (2.x).
}

Invoke-Test "2.2 - Save As to an Access extension is refused" {
    $c = Get-ConvertedFixture -Fixture $firstFixture
    $work = New-WorkCopy -Source $c.SqlitePath -Label "saveas"
    $target = Join-Path (Split-Path -Parent $work) "refused.ewemdb"
    $core = Open-Model -Path $work
    try {
        $result = $null; $threw = $false
        try { $result = $core.Save($target) } catch { $threw = $true }
        Write-Host ("    Save('{0}') -> threw: {1}, returned: {2}, file written: {3}" -f (Split-Path -Leaf $target), $threw, $result, (Test-Path $target)) -ForegroundColor Cyan
        Write-Host ("    EwECore.dll under test: built {0}" -f (Get-Item (Join-Path $script:CoreBin "EwECore.dll")).LastWriteTime) -ForegroundColor Cyan
        Assert-True ($threw -or $result -eq $false -or -not (Test-Path $target)) "Save As to '$target' appears to have succeeded."
        Assert-True (-not (Test-Path $target)) "An Access file was written: $target"
    } finally { Close-Model $core }
}

Invoke-Test "2.3 - Save As to a .ewesqlite keeps the data and re-targets the working copy" {
    $c = Get-ConvertedFixture -Fixture $firstFixture
    $work = New-WorkCopy -Source $c.SqlitePath -Label "saveas-ok"
    $target = Join-Path (Split-Path -Parent $work) "saved-as.ewesqlite"
    $hashBefore = (Get-FileHash $work -Algorithm SHA256).Hash
    # Staging is off by default in this headless host; the staged path (re-target, no reopen) needs it on.
    $stagingProp = (Get-EwEType 'cEwEDatabase').GetProperty('EnableLocalStaging')
    $stagingProp.SetValue($null, $true)
    try {
        $core = Write-Step "Loading a copy (staging on)" { Open-Model -Path $work }
        try {
            $before = Get-TableCounts -Core $core
            $ok = Write-Step "Save As" { $core.Save($target) }
            Assert-True $ok "Save($target) returned False."
            Assert-True (Test-Path $target) "No file at the Save As target."
            $afterSame = Get-TableCounts -Core $core
            $diff = $before.Keys | Where-Object { $afterSame[$_] -ne $before[$_] }
            Assert-True (-not $diff) ("The open session changed after Save As in: " + ($diff -join ", "))
        } finally { Close-Model $core }
    } finally { $stagingProp.SetValue($null, $false) }
    $core = Write-Step "Reopening the Save As target" { Open-Model -Path $target }
    try { $inTarget = Get-TableCounts -Core $core } finally { Close-Model $core }
    $diff = $before.Keys | Where-Object { $inTarget[$_] -ne $before[$_] }
    Assert-True (-not $diff) ("The Save As target differs from the source in: " + ($diff -join ", "))
    Assert-True ((Get-FileHash $work -Algorithm SHA256).Hash -eq $hashBefore) "The original file was modified by Save As."
}

Invoke-Test "9.3 - CanCompact is False for SQLite" {
    $c = Get-ConvertedFixture -Fixture $firstFixture
    $core = Open-Model -Path $c.SqlitePath
    try {
        $can = $core.DataSource.Connection.CanCompact($c.SqlitePath, $c.SqlitePath)
        Assert-True (-not $can) "CanCompact returned True for a SQLite database."
    } finally { Close-Model $core }
}

# ---- 3.4 ------------------------------------------------------------------------------------------------------------
Invoke-Test "3.4 - Legacy updates under nl-NL give the same data as under en-US (UpdateLog, EcopathModel, EcopathGroup)" {
    Initialize-EwECore
    $tables = "UpdateLog", "EcopathModel", "EcopathGroup"
    $dumps = @{}
    $thread = [Threading.Thread]::CurrentThread
    $oldCulture = $thread.CurrentCulture; $oldUi = $thread.CurrentUICulture
    try {
        foreach ($culture in "en-US", "nl-NL") {
            $ci = [Globalization.CultureInfo]::GetCultureInfo($culture)
            $thread.CurrentCulture = $ci; $thread.CurrentUICulture = $ci
            [Globalization.CultureInfo]::DefaultThreadCurrentCulture = $ci
            $copy = New-WorkCopy -Source $firstFixture.FullName -Label "cult-$culture"
            $core = Write-Step "Loading (legacy updates) under $culture" { Open-Model -Path $copy }
            try { $dumps[$culture] = @{}; foreach ($t in $tables) { $dumps[$culture][$t] = @(Get-TableDump -Core $core -Table $t) } }
            finally { Close-Model $core }
        }
    } finally {
        $thread.CurrentCulture = $oldCulture; $thread.CurrentUICulture = $oldUi
        [Globalization.CultureInfo]::DefaultThreadCurrentCulture = $null
    }
    $problems = @()
    foreach ($t in $tables) {
        $a = $dumps["en-US"][$t]; $b = $dumps["nl-NL"][$t]
        Write-Host ("    {0}: {1} rows (en-US) / {2} rows (nl-NL)" -f $t, $a.Count, $b.Count) -ForegroundColor Cyan
        $diff = Compare-Object $a $b
        if ($diff) { $problems += "$t differs, e.g. " + (($diff | Select-Object -First 3 | ForEach-Object { "$($_.SideIndicator) $($_.InputObject)" }) -join " ;; ") }
    }
    Assert-True ($problems.Count -eq 0) ($problems -join "`n      ")
}

# ---- 4.4 ------------------------------------------------------------------------------------------------------------
Invoke-Test "4.4 - GetSupportedType per extension" {
    Initialize-EwECore
    $dir = Join-Path $workRoot "types"; New-Item -ItemType Directory $dir -Force | Out-Null
    $gst = { param($f) (Invoke-EwEStatic 'cDataSourceFactory' 'GetSupportedType' @((Join-Path $dir $f))).ToString() }
    $expectMdb = if ($Platform -eq "x64") { "Access2007" } else { "Access2003" }
    $checks = @(
        @("a.eweaccdb", "Access2007"), @("a.accdb", "Access2007"),
        @("a.ewemdb", $expectMdb), @("a.mdb", $expectMdb),
        @("a.ewesqlite", "Sqlite"), @("a.sqlite", "Sqlite")
    )
    $bad = @()
    foreach ($ch in $checks) { $got = & $gst $ch[0]; Write-Host ("    {0,-12} -> {1}" -f $ch[0], $got) -ForegroundColor Cyan; if ($got -ne $ch[1]) { $bad += "$($ch[0]): got $got, expected $($ch[1])" } }
    # .accdb with a same-named .ewesqlite next to it => comparison mode; .mdb never is.
    foreach ($f in "b.eweaccdb", "b.accdb", "b.ewemdb") { Set-Content (Join-Path $dir $f) "x" }
    Set-Content (Join-Path $dir "b.ewesqlite") "x"
    foreach ($f in "b.eweaccdb", "b.accdb") { $got = & $gst $f; Write-Host ("    {0,-12} (+b.ewesqlite) -> {1}" -f $f, $got) -ForegroundColor Cyan; if ($got -ne "AccessVsSqlite") { $bad += "$f with sibling: got $got, expected AccessVsSqlite" } }
    $got = & $gst "b.ewemdb"; if ($got -ne $expectMdb) { $bad += "b.ewemdb with sibling: got $got, expected $expectMdb (never AccessVsSqlite)" }
    Assert-True ($bad.Count -eq 0) ($bad -join "; ")
}

# ---- 1.19 / 1.20 / 1.21 ---------------------------------------------------------------------------------------------
Invoke-Test "1.19 - Garbage .ewesqlite next to an Access file (characterisation)" {
    Initialize-EwECore
    $copy = New-WorkCopy -Source $firstFixture.FullName -Label "stale"
    $sqlite = [IO.Path]::ChangeExtension($copy, ".ewesqlite")
    [IO.File]::WriteAllBytes($sqlite, (New-Object byte[] 0))
    $type = (Invoke-EwEStatic 'cDataSourceFactory' 'GetSupportedType' @($copy)).ToString()
    Write-Host "    GetSupportedType('$([IO.Path]::GetFileName($copy))') with a 0-byte .ewesqlite beside it: $type" -ForegroundColor Cyan
    $opened = $false; $err = ""
    try { $core = Open-Model -Path $sqlite; $opened = $true; Close-Model $core } catch { $err = $_.Exception.Message }
    Write-Host "    Opening the 0-byte .ewesqlite: $(if ($opened) { 'SUCCEEDED (treated as a valid model!)' } else { "failed - $err" })" -ForegroundColor Cyan
    Assert-True (-not $opened) "A 0-byte .ewesqlite opened as a model: the factory only checks File.Exists, so a crashed conversion is treated as 'already converted' (plan 1.19)."
}

Invoke-Test "1.20 - Conversion with the mdb2sqlite folder missing gives a clear error" {
    Initialize-EwECore
    $tool = Join-Path $script:CoreBin "mdb2sqlite"
    $hidden = "$tool.hidden"
    $copy = New-WorkCopy -Source $firstFixture.FullName -Label "notool"
    Assert-True (Test-Path $tool) "mdb2sqlite folder not in the build output: $tool"
    Rename-Item $tool $hidden
    try {
        $msg = $null
        try { Invoke-EwEStatic 'cMdb2SqliteConverter' 'ConvertMdbToSqlite' ($copy, ([IO.Path]::ChangeExtension($copy, ".ewesqlite"))) } catch { $msg = $_.Exception.ToString() }
        Assert-True ($null -ne $msg) "No error although mdb2sqlite.exe is missing."
        Assert-True ($msg -match "mdb2sqlite\.exe not found") "Unexpected error (expected a clear 'mdb2sqlite.exe not found'): $msg"
    } finally { Rename-Item $hidden $tool }
}

# 1.21 (target held open during conversion) is deliberately not automated: when the target .ewesqlite already exists,
# mdb2sqlite.exe stops at an interactive "Overwrite? (y/n)" prompt, so such a test would hang. The app never gets there
# (an existing .ewesqlite is opened instead of converting). See SQLiteTransitionTestPlan.md 1.21.

# ---- 6.x static -----------------------------------------------------------------------------------------------------
Invoke-Test "6.6 - mdb2sqlite and migrator tools are in the build output" {
    foreach ($rel in "mdb2sqlite\mdb2sqlite.exe", "migrator\Eii.Ecopath.Storage.Migrator.exe") {
        Assert-True (Test-Path (Join-Path $binFolder $rel)) "Missing from the build output: $rel"
    }
}

Invoke-Test "6.8 - The installer registers the .ewesqlite extension" {
    $iss = Get-ChildItem (Join-Path $RepoRoot "Deployment") -Filter "*.iss" -Recurse -ErrorAction SilentlyContinue
    Assert-True ($iss.Count -gt 0) "No Inno Setup (.iss) script found under Deployment."
    $hit = $iss | Select-String -Pattern "ewesqlite" -SimpleMatch
    Assert-True ($null -ne $hit) "No .iss script mentions 'ewesqlite'."
}

Invoke-Test "6.7 - Count of 'Todo: localize the message' markers (report only)" {
    $hits = Get-ChildItem (Join-Path $RepoRoot "Sources") -Filter "*.vb" -Recurse -ErrorAction SilentlyContinue |
        Select-String -Pattern "Todo: localize the message" -SimpleMatch
    $files = $hits | Group-Object Path | Sort-Object Count -Descending | Select-Object -First 8
    Write-Host ("    {0} marker(s) in {1} file(s)" -f @($hits).Count, @($files).Count) -ForegroundColor Cyan
    foreach ($f in $files) { Write-Host ("      {0,3}  {1}" -f $f.Count, $f.Name.Replace($RepoRoot, "")) -ForegroundColor DarkGray }
}

Show-TestSummary
$runStopwatch.Stop()
Write-Host "`nTotal duration: $(Format-Duration $runStopwatch.Elapsed)" -ForegroundColor Cyan
Remove-Item $workRoot -Recurse -Force -ErrorAction SilentlyContinue
if ($MyInvocation.InvocationName -ne '.') {
    Remove-Item Env:\EWE_TEST_PAUSE_ON_FAIL, Env:\EWE_TEST_FROM -ErrorAction SilentlyContinue
}
Clear-TestArtifacts
