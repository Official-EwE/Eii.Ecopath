@echo off
dotnet publish Eii.Ecopath.Storage.Migrator.csproj -c Release -r win-x64 -o ..\Publish\win-x64
dotnet publish Eii.Ecopath.Storage.Migrator.csproj -c Release -r win-x86 -o ..\Publish\win-x86
pause
