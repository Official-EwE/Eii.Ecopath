# Ecopath with Ecosim (EwE) - Source Code
This repository contains the **source code** for Ecopath with Ecosim (EwE), an ecosystem modeling software suite developed by the Ecopath International Initiative (EII).

The master branch contains the latest stable release of EwE. 
- **You can not commit directly to the master branch!**
- **A Build Check will be performed before you can merge a PR into master**

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

The way to do this is described in the [EwE Wiki, package source secrets](https://github.com/Official-EwE/Ecopath-project/wiki/NuGet-packages#package-source-secrets).

This describes how you can run a CLI command to encrypt your access token and store it in your local NuGet.config file.

In AppData\Roaming\NuGet\NuGet.config, the EwE package source is shown as follows:

    <packageSources>
      <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
      <add key="EwE github" value="https://nuget.pkg.github.com/Official-EwE/index.json" />
    </packageSources>

In AppData\Roaming\NuGet\NuGet.config, your GitHub access token is shown as an encrypted secret as follows:

    <packageSourceCredentials>
	    <github>
		    <add key="Username" value="your github username" />
		    <add key="ClearTextPassword" value="your github access key" />
	    </github>
    </packageSourceCredentials>


## Creating and pushing NuGet packages to GitHub

This solution contains 4 NuGet packages:
- EwECore
- EwEUtils
- EwEPlugin
- ScientificInterfaceShared

These packages can be created and pushed to GitHub using the `CreateAndPushNugetPackage.ps1` Powershell file located in the `Sources` directory of the repository.
To push those packages you will need to have a Personal Access Token (PAT) with `write:packages` scope. (Also known as an ApiKey)

For example, to create and push the ScientificInterfaceShared package, follow these steps:
- In Visual Studio, open the `Developer PowerShell` window
- Navigate to the `Sources` directory of the repository
- Type `.\CreateAndPushNugetPackage.ps1 -ProjectPath ScientificInterfaceShared\ScientificInterfaceShared.vbproj -ApiKey <ApiKey>`
- The version patch number of the vbproj file will be incremented and the packages will be build and pushed to `https://nuget.pkg.github.com/Official-EwE/index.json`