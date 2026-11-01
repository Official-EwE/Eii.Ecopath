# Local Ecopath build script with dev code signing
$ErrorActionPreference = "Stop"

# Settings
$certName = "EcopathDevCodeSign"
$certFolder = ".devcerts"
$pfxPath = "$certFolder\ecopath-dev-signing.pfx"
$cerPath = "$certFolder\ecopath-dev-signing.cer"

# Prompt for cert password securely (input is masked)
$securePwd = Read-Host -Prompt "Enter dev cert password" -AsSecureString

# Create the folder if it doesn't exist
if (-not (Test-Path $certFolder -PathType Container)) {
    New-Item -ItemType Directory -Path $certFolder | Out-Null
}

# Step 1: Ensure dev signing certificate exists (in cert store and as a file)
$existingCert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object { $_.Subject -eq "CN=$certName" }

if (-Not (Test-Path $pfxPath -PathType Leaf) -or -Not $existingCert) {
    Write-Host "Creating new dev code signing certificate..."

    # Remove any pre-existing cert for idempotency
    if ($existingCert) {
        Remove-Item -Path "Cert:\CurrentUser\My\$($existingCert.Thumbprint)" -Force
    }

    # Create self-signed cert
    $cert = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject "CN=$certName" `
        -CertStoreLocation "Cert:\CurrentUser\My" `
        -KeyExportPolicy Exportable `
        -KeySpec Signature `
        -NotAfter (Get-Date).AddYears(2)

    # Export as PFX (with private key for signing)
    Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePwd

    # Export as CER (public part, not always needed)
    Export-Certificate -Cert $cert -FilePath $cerPath

    Write-Host "Certificate created and exported to $pfxPath and $cerPath"
} else {
    Write-Host "Using existing dev certificate at $pfxPath"
}

# Step 2: Verify NuGet credentials and update if needed
$localNugetConfig = Join-Path $env:APPDATA "NuGet\NuGet.Config"
$repoNugetConfig  = Join-Path $PSScriptRoot "..\..\NuGet.config"

if (-not (Test-Path $localNugetConfig)) {
    Write-Host "No machine-local NuGet.Config found, copying from repository..."
    if (-not (Test-Path $repoNugetConfig)) {
        Write-Error "No NuGet.config found at $repoNugetConfig. Cannot set up credentials."
    }
    Copy-Item $repoNugetConfig $localNugetConfig
    Write-Host "Copied repo NuGet.config to $localNugetConfig"
}

function Update-NuGetCredentials {
    param($configPath)
    $nugetUsername = Read-Host -Prompt "GitHub username"
    $nugetPat      = Read-Host -Prompt "GitHub PAT (read:packages)" -AsSecureString
    $nugetPatPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($nugetPat)
    )

    [xml]$localConfig = Get-Content $configPath
    $sources = $localConfig.SelectNodes("/configuration/packageSources/add") | Where-Object { $_.value -like "*nuget.pkg.github.com*" }
    foreach ($source in $sources) {
        Write-Host "  Updating credentials for: $($source.key)"
        dotnet nuget update source $source.key `
            --username $nugetUsername `
            --password $nugetPatPlain `
            --configfile $configPath
    }

    $nugetPatPlain = $null
}

function Test-NuGetCredentials {
    param($configPath)
    Write-Host "Testing NuGet credentials..."

    [xml]$localConfig = Get-Content $configPath
    $sources = $localConfig.SelectNodes("/configuration/packageSources/add") | Where-Object { $_.value -like "*nuget.pkg.github.com*" }

    foreach ($source in $sources) {
        $credsNode = $localConfig.SelectSingleNode("/configuration/packageSourceCredentials/$($source.key)")
        if (-not $credsNode) { return $false }

        $username = ($credsNode.SelectSingleNode("add[@key='Username']")).value
        $password = ($credsNode.SelectSingleNode("add[@key='Password']")).value
        if (-not $username -or -not $password) { return $false }

        try {
            $pair = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("${username}:${password}"))
            $response = Invoke-WebRequest -Uri $source.value -Headers @{ Authorization = "Basic $pair" } -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -ne 200) { return $false }
        } catch {
            return $false
        }
    }

    return $true
}

# Test credentials and prompt to update if they fail
if (-not (Test-NuGetCredentials $localNugetConfig)) {
    Write-Host "NuGet credentials are missing or invalid."
    $maxAttempts = 3
    $attempt = 0
    $credentialsValid = $false

    while (-not $credentialsValid -and $attempt -lt $maxAttempts) {
        $attempt++
        Write-Host "Attempt $attempt of $maxAttempts - please enter your GitHub credentials:"
        Update-NuGetCredentials $localNugetConfig
        if (Test-NuGetCredentials $localNugetConfig) {
            $credentialsValid = $true
            Write-Host "Credentials verified successfully."
        } else {
            Write-Host "Credentials still invalid."
        }
    }

    if (-not $credentialsValid) {
        Write-Error "Failed to authenticate with GitHub NuGet after $maxAttempts attempts. Please check your PAT has read:packages scope."
    }
} else {
    Write-Host "NuGet credentials verified."
}

# Step 3: Find the solution or project to build
$solutions = Get-ChildItem *.sln
if ($solutions.Count -gt 1) {
    Write-Error "Multiple .sln files found. Please specify which solution to build by editing this script: $($solutions.Name -join ', ')"
}
$solution = $solutions | Select-Object -First 1

if (-not $solution) {
    $projects = Get-ChildItem -Path *.csproj, *.vbproj
    if ($projects.Count -gt 1) {
        Write-Error "Multiple project files found. Please specify which project to build by editing this script: $($projects.Name -join ', ')"
    }
    $project = $projects | Select-Object -First 1
}

if (-not $solution -and -not $project) {
    Write-Error "No .sln, .csproj, or .vbproj file found in this directory."
}

# Step 4: Run the build with signing
Write-Host "Building with code signing enabled ..."
$buildTarget = if ($solution) { $solution.Name } else { $project.Name }

# Get thumbprint for our dev certificate (most recent matching CN)
$existingCert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object { $_.Subject -eq "CN=$certName" } | Sort-Object NotAfter -Descending | Select-Object -First 1

if (-not $existingCert) {
    Write-Error "Could not find signing certificate in Cert:\CurrentUser\My"
}

$thumbprint = $existingCert.Thumbprint

$arguments = @(
    $buildTarget
    '"/t:Restore;Rebuild"'
    "/p:Configuration=Release"
    "/p:SignManifests=true"
    "/p:ManifestCertificateThumbprint=$thumbprint"
)

# Prefer dotnet msbuild (ships with the .NET SDK) over a standalone msbuild on PATH
$msbuildCmd = if (Get-Command dotnet -ErrorAction SilentlyContinue) { "dotnet msbuild" } else { "msbuild" }
Write-Host "Using build tool: $msbuildCmd"

Invoke-Expression "$msbuildCmd $($arguments -join ' ')"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build succeeded."

    $outputPath = Join-Path (Split-Path (Resolve-Path $buildTarget) -Parent) "bin\Release"

    # Sign all output executables and dlls with the dev certificate
    $filesToSign = Get-ChildItem $outputPath -Include *.exe, *.dll -Recurse
    if ($filesToSign) {
        Write-Host "Signing output binaries in $outputPath ..."
        foreach ($file in $filesToSign) {
            $sig = Get-AuthenticodeSignature $file.FullName
            if ($sig.Status -eq "NotSigned") {
                Set-AuthenticodeSignature -FilePath $file.FullName -Certificate $existingCert | Out-Null
                Write-Host "  Signed: $($file.Name)"
            } else {
                Write-Host "  Already signed, skipping: $($file.Name)"
            }
        }
        Write-Host "Signing complete."
    } else {
        Write-Host "No .exe or .dll files found to sign in $outputPath"
    }

    # Step 5: Publish self-contained
    Write-Host "Publishing self-contained ..."
    $publishPath = Join-Path (Split-Path (Resolve-Path $buildTarget) -Parent) "publish"
    dotnet publish $buildTarget `
        --configuration Release `
        --self-contained true `
        --runtime win-x64 `
        --output $publishPath

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed."
    }

    # Sign all published executables and dlls
    $filesToSign = Get-ChildItem $publishPath -Include *.exe, *.dll -Recurse
    if ($filesToSign) {
        Write-Host "Signing published binaries in $publishPath ..."
        foreach ($file in $filesToSign) {
            $sig = Get-AuthenticodeSignature $file.FullName
            if ($sig.Status -eq "NotSigned") {
                Set-AuthenticodeSignature -FilePath $file.FullName -Certificate $existingCert | Out-Null
                Write-Host "  Signed: $($file.Name)"
            } else {
                Write-Host "  Already signed, skipping: $($file.Name)"
            }
        }
        Write-Host "Signing complete."
    }

    Write-Host "Build output: $outputPath"
    Write-Host "Publish output (use this for distribution): $publishPath"
} else {
    Write-Error "Build failed."
}