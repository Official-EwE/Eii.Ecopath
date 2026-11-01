powershell -Command "Unblock-File -Path '%~dp0local-build.ps1'"
powershell -ExecutionPolicy RemoteSigned -File "%~dp0local-build.ps1"