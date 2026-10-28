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

## Privacy and installation ID

The installer generates a random GUID named `InstallationId`, including during silent installation. It is stored in
`%APPDATA%\Ecopath with Ecosim\ScientificInterface.settings` and retained across upgrades and reinstalls while that settings file exists.
It is shared by installed EwE versions using that user profile. Only the installer creates an ID; the application never generates one.
IDE, debug, and installed runs all read the ID from the same user settings file. If no ID is stored, the value is null.
View or copy the ID in **Options > Privacy**; it cannot be edited or reset there. The field is blank when no ID is available.
When anonymous diagnostics are enabled and an ID is available, it is included as the `InstallationId` Grafana Loki label.
The label is omitted when no ID is available. Disabling diagnostics prevents sending it.

### Grafana credentials

Create an **Actions organization secret** named `EWE_GRAFANA_LOKI_TOKEN` in GitHub organization settings and grant `Official-EwE/Eii.Ecopath` access.
Use a dedicated token restricted to log ingestion (write-only), with no read or administrative permissions.
The manual release workflow requires this secret and bundles it into both installers as `grafana-loki.token` beside the executable.
It passes a temporary filename to Inno Setup, not the token on the command line, and removes the temporary file afterward.
The token is not committed to source or included in NuGet packages. **Anyone with the installer or installed files can extract it**;
GitHub secret masking does not protect distributed installers. Restrict its scope, monitor abuse, and rotate it regularly.

At runtime, `EWE_GRAFANA_LOKI_TOKEN` in the process environment overrides the installed token file.
IDE runs inherit Visual Studio's environment; restart Visual Studio after changing a user environment variable.
A user environment variable is not an encrypted secret store. If neither source provides a token, remote diagnostics are disabled with a local warning.
Diagnostic consent is still required when a token is configured. The installer does not set a global environment variable.
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

### Compile the installer
To compile the installer, you can use the `iscc` command from Inno Setup.
First, decide on the version number you want to use for the installer, e.g. `1.0.9`.

In the Visual Studio open the `Developer PowerShell` window and navigate to the `Sources` folder of the repository.

For the 32-bit installer, type:
- Type `iscc /DCompile64Bit=0 /DSemVersion="1.0.9" /O"ScientificInterface\bin\x86\publish" "..\Deployment\EwE6_relpath.iss"`
- A `ScientificInterface\bin\x86\publish\ewe_1.0.9_32-bit_setup.exe` will be created.


For the 64-bit installer, type:
- Type `iscc /DCompile64Bit=1 /DSemVersion="1.0.9" /O"ScientificInterface\bin\x64\publish" "..\Deployment\EwE6_relpath.iss"`
- A `ScientificInterface\bin\x64\publish\ewe_1.0.9_64-bit_setup.exe` will be created.

## Zenodo integration
This GitHub repository is linked with Zenodo. Every release will receive a Zenodo DOI, orchestrated via the .zenodo.json file in the root of this repo.
The following command chain explains how this works:

```
GitHub development → source always available → internal decision that a release is worth packaging → installer build → GitHub Release → Zenodo archive/DOI → optional EwE community inclusion
```

Note that every release will become a Zenodo record, but will not automatically appear in the (Zenodo EwE community)[https://zenodo.org/communities/ewe/]. This is a manual action performed by community members.
