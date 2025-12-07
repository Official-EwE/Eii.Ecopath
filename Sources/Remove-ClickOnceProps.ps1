
<#
.SYNOPSIS
Removes ClickOnce/legacy properties and selected BootstrapperPackage items from an SDK-style .vbproj/.csproj file,
cleans up empty lines, saves as UTF-8 (with BOM by default), and strips the XML declaration so the file starts with <Project ...>.

.PARAMETER ProjectPath
Path to the project file (.vbproj or .csproj).

.PARAMETER WhatIf
Shows what would be changed without saving the file.

.PARAMETER Utf8NoBom
Save as UTF-8 without BOM. If omitted, the script saves as UTF-8 WITH BOM (recommended)
to avoid mis-decoding symbols like © as Â©.

.PARAMETER PreserveBlankLines
If present, the script will NOT remove whitespace-only lines in the final file.

.NOTES
- Compatible with Windows PowerShell 5.1 and PowerShell 7+.
- This version does NOT create a .bak backup; use version control for safety.
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({
        if (-not (Test-Path $_)) { throw "File not found: $_" }
        $ext = [IO.Path]::GetExtension($_).ToLowerInvariant()
        if ($ext -ne '.vbproj' -and $ext -ne '.csproj') {
            throw "ProjectPath must be a .vbproj or .csproj file."
        }
        $true
    })]
    [string]$ProjectPath,

    [switch]$WhatIf,
    [switch]$Utf8NoBom,
    [switch]$PreserveBlankLines
)

# ClickOnce / OneClick and related legacy properties to remove from PropertyGroup
$clickOnceProps = @(
    'PublishUrl',
    'Install',
    'InstallFrom',
    'UpdateEnabled',
    'UpdateMode',
    'UpdateInterval',
    'UpdateIntervalUnits',
    'UpdatePeriodically',
    'UpdateRequired',
    'MapFileExtensions',
    'ApplicationRevision',
    'ApplicationVersion',
    'IsWebBootstrapper',
    'UseApplicationTrust',
    'BootstrapperEnabled',
    'TargetZone',
    'SccProjectName',
    'SccLocalPath',
    'SccAuxPath',
    'SccProvider',
    'RunFxCop',
    'FxCopInputAssembly',
    'ImportWindowsDesktopTargets'
)

# BootstrapperPackage entries to remove from ItemGroup by Include attribute
$bootstrapperIncludesToRemove = @(
    'Microsoft.Net.Client.3.5',
    'Microsoft.Net.Framework.2.0',
    'Microsoft.Net.Framework.3.5.SP1',
    'Microsoft.Windows.Installer.3.1'
)

function Save-Xml {
    param(
        [Parameter(Mandatory=$true)][System.Xml.XmlDocument]$XmlDoc,
        [Parameter(Mandatory=$true)][string]$Path,
        [switch]$WithBom
    )
    # Use a StreamWriter with explicit UTF-8 and BOM choice
    $utf8 = New-Object System.Text.UTF8Encoding($WithBom.IsPresent)
    $fs = [System.IO.File]::Open($Path, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)
    try {
        $sw = New-Object System.IO.StreamWriter($fs, $utf8)
        try { $XmlDoc.Save($sw) } finally { $sw.Dispose() }
    } finally { $fs.Dispose() }
}

Write-Host "Loading project: $ProjectPath" -ForegroundColor Cyan

# Load XML with preserved whitespace
$xmlContent = Get-Content -LiteralPath $ProjectPath -Raw
$xml = New-Object System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.LoadXml($xmlContent)

# Collect planned removals for preview (PropertyGroup nodes)
$plannedRemovals = @()
foreach ($propName in $clickOnceProps) {
    $nodes = $xml.SelectNodes("//PropertyGroup/$propName")
    if ($nodes -and $nodes.Count -gt 0) {
        foreach ($node in $nodes) {
            # PowerShell 5.1-safe parent Condition access
            $parentCondition = $null
            if ($node.ParentNode -and $node.ParentNode.Attributes) {
                $condAttr = $node.ParentNode.Attributes.GetNamedItem('Condition')
                if ($condAttr) { $parentCondition = $condAttr.Value }
            }
            $plannedRemovals += [PSCustomObject]@{
                Property        = $propName
                Value           = $node.InnerText
                ParentCondition = $parentCondition
            }
        }
    }
}

# Collect planned removals for preview (BootstrapperPackage nodes)
foreach ($incl in $bootstrapperIncludesToRemove) {
    $bpNodes = $xml.SelectNodes("//ItemGroup/BootstrapperPackage[@Include='$incl']")
    if ($bpNodes -and $bpNodes.Count -gt 0) {
        foreach ($bp in $bpNodes) {
            # PS 5.1-safe access to Condition on the parent ItemGroup
            $bpParentCond = $null
            if ($bp.ParentNode -and $bp.ParentNode.Attributes) {
                $condAttr2 = $bp.ParentNode.Attributes.GetNamedItem('Condition')
                if ($condAttr2) { $bpParentCond = $condAttr2.Value }
            }
            $plannedRemovals += [PSCustomObject]@{
                Property        = 'BootstrapperPackage'
                Value           = $incl
                ParentCondition = $bpParentCond
            }
        }
    }
}

if ($plannedRemovals.Count -eq 0) {
    Write-Host "No targeted properties or BootstrapperPackage items found. Nothing to remove." -ForegroundColor Yellow
    return
}

Write-Host "Found $($plannedRemovals.Count) instance(s) to remove:" -ForegroundColor Green
$plannedRemovals | Format-Table Property, Value, ParentCondition -AutoSize

if ($WhatIf) {
    Write-Host "`nWhatIf specified: no changes will be made." -ForegroundColor Yellow
    return
}

# Remove PropertyGroup nodes
foreach ($propName in $clickOnceProps) {
    $nodes = $xml.SelectNodes("//PropertyGroup/$propName")
    if ($nodes) {
        foreach ($node in @($nodes)) {
            $null = $node.ParentNode.RemoveChild($node)
        }
    }
}

# Remove BootstrapperPackage nodes by Include
foreach ($incl in $bootstrapperIncludesToRemove) {
    $bpNodes = $xml.SelectNodes("//ItemGroup/BootstrapperPackage[@Include='$incl']")
    if ($bpNodes) {
        foreach ($bp in @($bpNodes)) {
            $null = $bp.ParentNode.RemoveChild($bp)
        }
    }
}

# Remove now-empty PropertyGroup elements
$propertyGroups = $xml.SelectNodes("//PropertyGroup")
foreach ($pg in @($propertyGroups)) {
    $hasElementChildren = $false
    foreach ($child in $pg.ChildNodes) {
        if ($child.NodeType -eq [System.Xml.XmlNodeType]::Element) { $hasElementChildren = $true; break }
    }
    if (-not $hasElementChildren) { $null = $pg.ParentNode.RemoveChild($pg) }
}

# Remove now-empty ItemGroup elements
$itemGroups = $xml.SelectNodes("//ItemGroup")
foreach ($ig in @($itemGroups)) {
    $hasElementChildren = $false
    foreach ($child in $ig.ChildNodes) {
        if ($child.NodeType -eq [System.Xml.XmlNodeType]::Element) { $hasElementChildren = $true; break }
    }
    if (-not $hasElementChildren) { $null = $ig.ParentNode.RemoveChild($ig) }
}

# Save (UTF-8, BOM optional)
Save-Xml -XmlDoc $xml -Path $ProjectPath -WithBom:(!$Utf8NoBom)

# Strip XML declaration so the file begins with <Project ...>
$fileContent = Get-Content -LiteralPath $ProjectPath -Raw
if ($fileContent.StartsWith('<?xml')) {
    # Remove the XML declaration and following whitespace/newlines
    $fileContent = $fileContent -replace '^<\?xml.*?\?>\s*', ''
}

# Remove whitespace-only lines unless user wants to preserve them
if (-not $PreserveBlankLines) {
    # Delete lines that consist only of spaces/tabs
    $fileContent = [System.Text.RegularExpressions.Regex]::Replace(
        $fileContent,
        '^[\t ]*\r?\n',
        '',
        [System.Text.RegularExpressions.RegexOptions]::Multiline
    )
    # Collapse 3+ consecutive blank lines to a single blank line
    $fileContent = [System.Text.RegularExpressions.Regex]::Replace(
        $fileContent,
        '(\r?\n){3,}',
        "`r`n`r`n"
    )
}

# Write back with the chosen encoding (default: UTF-8 WITH BOM)
$bytes =
    if ($Utf8NoBom) {
        # UTF-8 without BOM
        [System.Text.Encoding]::UTF8.GetBytes($fileContent)
    } else {
        # UTF-8 with BOM
        $enc = New-Object System.Text.UTF8Encoding($true)
        $encPreamble = $enc.GetPreamble()
        $contentBytes = $enc.GetBytes($fileContent)
        $result = New-Object byte[] ($encPreamble.Length + $contentBytes.Length)
        [Array]::Copy($encPreamble, 0, $result, 0, $encPreamble.Length)
        [Array]::Copy($contentBytes, 0, $result, $encPreamble.Length, $contentBytes.Length)
        $result
    }

[System.IO.File]::WriteAllBytes($ProjectPath, $bytes)

Write-Host "Selected properties and BootstrapperPackage items removed." -ForegroundColor Green
Write-Host ("Saved as UTF-8 {0}, without XML declaration, and blank lines {1}." -f ($(if($Utf8NoBom){"(no BOM)"} else {"(with BOM)"}), $(if($PreserveBlankLines){"preserved"} else {"cleaned"}))) -ForegroundColor Green
