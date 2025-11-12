# Ecopath with Ecosim (EwE) - Source Code
This repository contains the **source code** for Ecopath with Ecosim (EwE), an ecosystem modeling software suite developed by the Ecopath International Initiative (EII).

The master branch contains the latest stable release of EwE. **You can not commit directly to the master branch!**

To add code to the master branch, please fork the repository, create a new branch, and submit a pull request.

For now, we follow a **'trunk based development'** strategy, meaning that all new features and bug fixes are directly merged into the master branch after code review and testing.

Long lived feature branches are discouraged. If you need to work on a large feature, please create a feature branch from master, and regularly merge master into your feature branch to keep it up to date.

## Ecopath with Ecosim Project
Other items related to the Ecopath with Ecosim project can be found in the [Project repo](https://github.com/Official-EwE/Ecopath-project)

The project repo contais:
- The [Scrum board](https://github.com/orgs/Official-EwE/projects/8) with all issues
- The [EwE Wiki](https://github.com/Official-EwE/Ecopath-project/wiki) with developer documentation
- Definately NO CODE!


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
