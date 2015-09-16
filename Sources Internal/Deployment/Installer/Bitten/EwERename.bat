echo Updating build version numbers
set buildNumber=6.1.0.%date:~4,2%%date:~7,2%
cd "C:\Ecopath\Releases\Ecopath6_Internal\AWinstall_Source\"
EwERenameAssemblies.exe "C:\Ecopath\Releases\Ecopath6\Sources", %buildNumber%
EwERenameAssemblies.exe "C:\Ecopath\Releases\Ecopath6_Internal\Sources\EwECustomPlugins", %buildNumber%