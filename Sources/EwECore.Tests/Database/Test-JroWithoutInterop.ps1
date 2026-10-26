# SPDX-License-Identifier: EUPL-1.2
# This file is part of Ecopath with Ecosim (EwE).
# Copyright (c) Ecopath International Initiative (EII)

param(
    [Parameter(Mandatory = $true)]
    [string] $CoreAssemblyPath,
    [switch] $Probe
)

$ErrorActionPreference = 'Stop'

if ($Probe) {
    # Arrange
    if ($PSVersionTable.PSEdition -ne 'Desktop') {
        throw 'The JRO regression probe requires Windows PowerShell (.NET Framework).'
    }
    if (Test-Path (Join-Path (Split-Path $CoreAssemblyPath) 'Interop.JRO.dll')) {
        throw 'The isolated probe must not contain Interop.JRO.dll.'
    }
    [void][Reflection.Assembly]::LoadFrom($CoreAssemblyPath)
    $compactor = New-Object EwECore.Database.cCompactJRO

    # Act
    $available = $compactor.CanCompact()
    $availableAgain = $compactor.CanCompact()
    $result = $compactor.Compact('', '', '', '')

    # Assert
    if ($available -or $availableAgain) {
        throw 'JRO must be unavailable when its interop assembly is missing.'
    }
    if ($result.ToString() -ne 'Failed_OSUnsupported') {
        throw "Expected Failed_OSUnsupported, received $result."
    }
    if ([AppDomain]::CurrentDomain.GetAssemblies().GetName().Name -contains 'Interop.JRO') {
        throw 'The probe unexpectedly resolved Interop.JRO from another location.'
    }
    Write-Output 'PASS: missing Interop.JRO returns unsupported without crashing, including repeated capability checks.'
    exit 0
}

$assembly = Get-Item -LiteralPath $CoreAssemblyPath
if ($assembly.Name -ne 'EwECore.dll' -or $assembly.Directory.Name -ne 'net48') {
    throw 'Supply the path to a built net48 EwECore.dll.'
}
$testDirectory = Join-Path ([IO.Path]::GetTempPath()) ('EwE-JroRegression-' + [Guid]::NewGuid().ToString('N'))
[void](New-Item -ItemType Directory -Path $testDirectory)
try {
    Get-ChildItem -LiteralPath $assembly.DirectoryName -Filter '*.dll' |
        Where-Object { $_.Name -ne 'Interop.JRO.dll' } |
        Copy-Item -Destination $testDirectory
    $isolatedAssembly = Join-Path $testDirectory 'EwECore.dll'
    $windowsPowerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'
    & $windowsPowerShell -NoProfile -NonInteractive -File $PSCommandPath -CoreAssemblyPath $isolatedAssembly -Probe
    if ($LASTEXITCODE -ne 0) {
        throw "The isolated JRO regression probe failed with exit code $LASTEXITCODE."
    }
}
finally {
    Remove-Item -LiteralPath $testDirectory -Recurse -Force
}
