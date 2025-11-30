param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectPath,

    [Parameter(Mandatory = $true)]
    [string]$ApiKey
)

# Validate file exists
if (-not (Test-Path $ProjectPath)) {
    Write-Error "The specified file does not exist: $ProjectPath"
    exit 1
}

# Load XML while preserving formatting
$xml = New-Object System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.Load($ProjectPath)

# Find the <Version> element regardless of namespaces
$versionNode = $xml.SelectSingleNode("/*/*[local-name()='PropertyGroup']/*[local-name()='Version']")

if (-not $versionNode) {
    throw "No <Version> element found in $ProjectPath"
}

$original = $versionNode.InnerText.Trim()

# Parse SemVer-like version and increment the patch
$semverPattern = '^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?<suffix>.*)$'
if ($original -notmatch $semverPattern) {
    throw "Version value '$original' is not in the expected 'x.y.z' format."
}

$major  = [int]$Matches['major']
$minor  = [int]$Matches['minor']
$patch  = [int]$Matches['patch'] + 1
$suffix = $Matches['suffix']

$newVersion = "$major.$minor.$patch$suffix"

# Write the new version (no backup)
$versionNode.InnerText = $newVersion
$xml.Save($ProjectPath)


Write-Host "✅ Version updated: $original → $newVersion"

# Restore
Write-Host "🔄 Running dotnet restore..."
dotnet restore $ProjectPath

# Pack
Write-Host "📦 Creating NuGet package...  Build with UseLocalProject=false !!!"
dotnet pack $ProjectPath -c Debug -p:UseLocalProject=false

# Find the generated .nupkg file
$projectDir = Split-Path -Parent $ProjectPath
$packageDir = Join-Path $projectDir "bin\Debug"
$packageFile = Get-ChildItem $packageDir -Filter "*.nupkg" | Where-Object { $_.Name -like "*$newVersion*" } | Select-Object -First 1
if (-not $packageFile) { throw "NuGet package not found for version $newVersion" }


Write-Host "✅ Package created: $($packageFile.FullName)"

# Push
$NuGetSource = "https://nuget.pkg.github.com/Official-EwE/index.json"
Write-Host "🚀 Pushing package to $NuGetSource..."
dotnet nuget push $packageFile.FullName --source $NuGetSource --api-key $ApiKey

Write-Host "✅ NuGet package pushed successfully!"
