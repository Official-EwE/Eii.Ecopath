# Ecopath with Ecosim (EwE) - Source Code
This repository contains the **source code** for Ecopath with Ecosim (EwE), an ecosystem modeling software suite developed by the Ecopath International Initiative (EII).

## 2026 migration from SVN to GitHub
Our old and beloved EwE SVN development server is no more. We transferred to a much more mature development backend, GitHub. The EwE source code is now really open source, and you can extract the source code without any user account. However, if you want to contrite to EwE, contact us and we'll allow your GitHub account into the EwE repo.

## GitHub code layout

The master branch contains the latest stable release of EwE. 
- **You can not commit directly to the master branch!!**
- **A Build Check will be performed before you can merge a PR into master!!**

[More information about working with Git](https://github.com/Official-EwE/Ecopath-project/wiki/Git-how-to)

## Ecopath with Ecosim Project
Other items related to the Ecopath with Ecosim project can be found in the [Project repo](https://github.com/Official-EwE/Ecopath-project)

The project repo contais:
- The [Scrum board](https://github.com/orgs/Official-EwE/projects/8) with all issues
- The [EwE Wiki](https://github.com/Official-EwE/Ecopath-project/wiki) with developer documentation
- Definitely NO CODE!


### Configuring GitHub as a package source

The EwE source code obtains packages from GitHub. This requires some configuration.
First, obtain a GitHub classic access token from https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens.
Then, configure NuGet. As the configuration will contain your classic access token, we recommend making the following changes in a configuration file local to your machine which will not be stored with the code.

The way to do this is described in the [EwE Wiki, package source secrets](https://github.com/Official-EwE/Ecopath-project/wiki/NuGet-packages#adding-package-source-secrets-to-your-pc).

This describes how you can run a CLI command to encrypt your access token and store it in your local NuGet.config file.

In AppData\Roaming\NuGet\NuGet.config, the EwE package source is shown as follows:

    <packageSources>
      <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
      <add key="github-Official-EwE" value="https://nuget.pkg.github.com/Official-EwE/index.json" />
    </packageSources>

In AppData\Roaming\NuGet\NuGet.config, your GitHub access token is shown as an encrypted secret as follows:

    <packageSourceCredentials>
	    <github>
		    <add key="Username" value="your github username" />
		    <add key="ClearTextPassword" value="your github access key" />
	    </github>
    </packageSourceCredentials>

## Creating and pushing NuGet packages to GitHub

This solution contains 3 NuGet packages:
- EwECore
- EwEUtils
- ScientificInterfaceShared

### Created automatically after merging to master
The easy way is to let the CI/CD pipeline create and push those packages automatically after merging a PR into master.

### Creating and pushing manually
These packages can be created and pushed to GitHub using the `CreateAndPushNugetPackage.ps1` Powershell file located in the `Sources` directory of the repository.
To push those packages you will need to have a Personal Access Token (PAT) with `write:packages` scope. (Also known as an ApiKey)

For example, to create and push the ScientificInterfaceShared package, follow these steps:
- In Visual Studio, open the `Developer PowerShell` window
- Navigate to the `Sources` directory of the repository
- Type `.\CreateAndPushNugetPackage.ps1 -ProjectPath ScientificInterfaceShared\ScientificInterfaceShared.vbproj -ApiKey <ApiKey>`

## Checking for updates

In EwE6, select **Help > Check for Updates**, directly below the log-file entry. EwE checks the latest GitHub release and shows whether an update is available, the installed version is current, or a newer development version is installed. Release versions are displayed without the tag's `v` prefix. If the check fails, the message shows the installed version and explains that the latest version could not be checked. When an update is available, click the releases hyperlink in the message to download it from https://github.com/Official-EwE/Eii.Ecopath/releases/.

## Crash logging

After logging is initialized, unhandled managed exceptions during startup, on UI threads (including the splash screen), or on background threads are logged at **Fatal** level with exception details before termination. UI exceptions terminate the application instead of showing the WinForms continue dialog.
Logs are flushed on normal shutdown and on these crash paths. Local logs are stored in `%LOCALAPPDATA%\Ecopath with Ecosim\Logs\log-yyyyMMdd.txt`.
Errors and fatal events also use the existing Grafana Loki sink when credentials and anonymous diagnostics consent are configured; remote delivery is best-effort.
Forced process termination, `Environment.FailFast`, stack overflows, and some native/runtime failures cannot be reliably caught or logged in-process. Failures before logging initialization are not covered.

## Privacy and installation ID

The installer generates a random GUID named `InstallationId`, including during silent installation. It is stored in
`%APPDATA%\Ecopath with Ecosim\ScientificInterface.settings` and retained across upgrades and reinstalls while that settings file exists.
It is shared by installed EwE versions using that user profile. Only the installer creates an ID; the application never generates one.
IDE, debug, and installed runs all read the ID from the same user settings file. If no ID is stored, the value is null.
View or copy the ID in **Options > Privacy**; it cannot be edited or reset there. The field is blank when no ID is available.
When anonymous diagnostics are enabled and an ID is available, it is included as the `InstallationId` Grafana Loki label.
The label is omitted when no ID is available. Disabling diagnostics prevents sending it.

### Grafana credentials

In the `Official-EwE/Eii.Ecopath` repository, open **Settings > Secrets and variables > Actions > New repository secret**
and create `EWE_GRAFANA_LOKI_TOKEN`. No organization secret is required.
Use a dedicated token restricted to log ingestion (write-only), with no read or administrative permissions.
The manual release workflow reads this repository secret through `${{ secrets.EWE_GRAFANA_LOKI_TOKEN }}` and bundles it into both installers for temporary extraction.
The installer writes it as the plain-text `GrafanaLokiToken` setting in `%APPDATA%\Ecopath with Ecosim\ScientificInterface.settings`.
It updates the token on reinstall, including silent installs. Builds without bundled credentials leave any existing token unchanged.
The workflow passes a temporary filename to Inno Setup, not the token on the command line, and removes the temporary file afterward.
No separate token file is installed beside the executable. The token is not committed to source or included in NuGet packages.
**Anyone with the installer or access to the settings file can read the token**; it is not encrypted.
GitHub secret masking does not protect distributed installers. Restrict its scope, monitor abuse, and rotate it regularly.

At runtime, EwE reads credentials only through `My.Settings.GrafanaLokiToken`, including when running from the IDE.
If the setting is missing or blank, remote diagnostics are disabled with a local warning. Diagnostic consent is still required when a token is configured.
For local installer builds, supply `/DGrafanaTokenFile="C:\path\to\token-file"`; omit it to build without bundled credentials.
If your binaries are in the non-platform output folder, also pass `/DDefSrc="Sources\ScientificInterface\bin\Release\net48"`.
Never commit a real token file. Revoke and replace any token previously committed to Git, even after removing it from branch history.

## Creating an EwE6 installer

### Creating the installer in Github
The easy way is to create the installer by running the Github action. After running this action everything will be automatic. A new version number is created and a tag is set. Building a new release typically takes 10 minutes and after that its ready for download at https://github.com/Official-EwE/Eii.Ecopath/releases.

* Go to https://github.com/Official-EwE/Eii.Ecopath/actions
* Click on "Manually create all NuGetPackages and a Release"
* Click on "Run workflow"
* Click on the "Run workflow" button

<img alt="image" src="Manually Create a Release.png" alt="Create release on Github" width="600"/>

You can monitor the progress of the workflow by clicking on the workflow run you just created.

When its finished, you can download the newly created release from https://github.com/Official-EwE/Eii.Ecopath/releases



### Creating the installer on your desktop
The installer is created using Inno Setup. The script is located in the `Deployment` folder of the repository.

When publishing `ScientificInterface`, the project retains StrongNamer's signed dependency DLLs instead of their unsigned NuGet originals.
Keep this publish target enabled: the signed DLLs are required for .NET Framework assembly binding. Duplicate-output validation remains enabled.

To be able to use the Inno Setup compiler, you have to install it. It is recommended to install Inno Setup as a CLI tool.
This is the way the tool is used in the build check and CI/CD pipelines.


### Install Chocolatey and Inno Setup
And to do that, you probably have to install `chocolatey` first.
- Open a PowerShell window as Administrator
- Execute the following command to install chocolatey:
  ```powershell
  Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
  ```

  When Chocolatey is installed, you can install Inno Setup:
  ```powershell
  choco install innosetup -y
  ```

### Prepare the local Grafana token (optional)

Local Inno Setup builds do not download GitHub repository secrets. To include a token, create a file **outside the repository**, for example:

```text
C:\Users\Rik\Secrets\grafana-loki.token
```

Create the directory if needed and put only the restricted, write-only Grafana token in the file, with no quotes or setting name.
Pass its path using `/DGrafanaTokenFile`, never the token itself. Do not commit the file or print its contents in build logs.
The installer bundles the token and writes it as plain-text `GrafanaLokiToken` in the user's
`%APPDATA%\Ecopath with Ecosim\ScientificInterface.settings`; no separate token file is installed beside the executable.
Both installed and IDE runs read this setting. Diagnostic consent remains required.

To build without bundled credentials, omit `/DGrafanaTokenFile` from the commands below.
Such an installer leaves any existing token in user settings unchanged; new users will have no remote diagnostics until a token is configured.
Bundling a token does not keep it secret from installer recipients. See [Grafana credentials](#grafana-credentials) for permissions and exposure.

### Build the application binaries

Open Developer PowerShell in Visual Studio and navigate to the **repository root** (the folder containing `Sources` and `Deployment`).
The examples below use release number `6.7.100`. Configure GitHub Packages credentials as described above before building.

For 32-bit binaries:

```powershell
dotnet build .\Sources\EwE6.sln -c Release /p:Platform=x86 /p:UseProjectReferences=true /p:Version=6.7.100 /p:FileVersion=6.7.100
```

For 64-bit binaries:

```powershell
dotnet build .\Sources\EwE6.sln -c Release /p:Platform=x64 /p:UseProjectReferences=true /p:Version=6.7.100 /p:FileVersion=6.7.100
```

The installer requires the complete build output, including DLLs, configuration, tools, user guides, and the matching GDAL native libraries.
Build the architecture you intend to package; `/DCompile64Bit` selects installer behavior but does not convert the binaries.

### Compile the installer

Run these commands from the **repository root**. Replace the token-file path and version as appropriate.
If `iscc` is not on your PATH, use `& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"` instead.

For the 32-bit installer (reads `Sources\ScientificInterface\bin\x86\Release\net48`):

```powershell
iscc /DCompile64Bit=0 /DSemVersion=6.7.100 `
  /DGrafanaTokenFile="C:\Users\Rik\Secrets\grafana-loki.token" `
  ".\Deployment\EwE6_relpath.iss"
```

Output: `Deployment\Output\ewe_6.7.100_32-bit_setup.exe`.

For the 64-bit installer (reads `Sources\ScientificInterface\bin\x64\Release\net48`):

```powershell
iscc /DCompile64Bit=1 /DSemVersion=6.7.100 `
  /DGrafanaTokenFile="C:\Users\Rik\Secrets\grafana-loki.token" `
  ".\Deployment\EwE6_relpath.iss"
```

Output: `Deployment\Output\ewe_6.7.100_64-bit_setup.exe`.

#### Using binaries from the non-platform output folder

If your existing **32-bit** binaries are in `Sources\ScientificInterface\bin\Release\net48`, override the source folder:

```powershell
iscc /DCompile64Bit=0 /DSemVersion=6.7.100 `
  /DDefSrc="Sources\ScientificInterface\bin\Release\net48" `
  /DGrafanaTokenFile="C:\Users\Rik\Secrets\grafana-loki.token" `
  ".\Deployment\EwE6_relpath.iss"
```

`DefSrc` is relative to the repository root. Only use `/DCompile64Bit=1` with this override if those binaries and their native dependencies are actually 64-bit.
Use `/O"C:\path\to\output"` to choose a different installer output directory.
These local commands do not run the GitHub workflow's SignPath signing step; installers are unsigned with the script's default `CodeSigning=0`.

## Zenodo integration
This GitHub repository is linked with Zenodo. Every release will receive a Zenodo DOI, orchestrated via the .zenodo.json file in the root of this repo.
The following command chain explains how this works:

```
GitHub development → source always available → internal decision that a release is worth packaging → installer build → GitHub Release → Zenodo archive/DOI → optional EwE community inclusion
```

Note that every release will become a Zenodo record, but will not automatically appear in the (Zenodo EwE community)[https://zenodo.org/communities/ewe/]. This is a manual action performed by community members.
