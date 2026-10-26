# Eii.Ecopath.EwECore

The Core functionality for Ecopath with Ecosim (EwE)

https://ecopath.org/

## Optional JRO compaction support

The `net48` build copies the supplied `Includes\Interop.JRO.dll` to the build output. MDB compaction also requires the native Windows JRO COM engine. If the interop assembly or COM engine is unavailable, capability checks return false and compaction returns `Failed_OSUnsupported` instead of crashing startup or command updates.

After building EwECore, run the missing-interop regression check from the repository root on Windows:

```powershell
.\Sources\EwECore.Tests\Database\Test-JroWithoutInterop.ps1 -CoreAssemblyPath .\Sources\EwECore\bin\x64\Release\net48\EwECore.dll
```

The check runs in an isolated Windows PowerShell (.NET Framework) process without `Interop.JRO.dll`; it does not alter the application output.

dd