# Configuring GitHub as a package source

The EwE source code obtains packages from GitHub. This requires some configuration.
First, obtain a GitHub classic access token from https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens.
Then, configure NuGet. As the configuration will contain your classic access token, we recommend making the following changes in a configuration file local to your machine which will not be stored with the code.

In AppData\Roaming\NuGet\NuGet.config, add the EwE package source as follows:

    <packageSources>
      <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
      <add key="EwE github" value="https://nuget.pkg.github.com/Official-EwE/index.json" />
    </packageSources>

In AppData\Roaming\NuGet\NuGet.config, also add your GitHub access token as follows:

    <packageSourceCredentials>
	    <github>
		    <add key="Username" value="your github username" />
		    <add key="ClearTextPassword" value="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" />
	    </github>
    </packageSourceCredentials>
