# Copilot instructions for Eii.Ecopath

## Build and test commands

- Main solution: `dotnet build .\Sources\EwE6.sln -c Release /p:Platform=x64 /p:UseProjectReferences=true`
- CI also builds x86: `dotnet build .\Sources\EwE6.sln -c Release /p:Platform=x86 /p:UseProjectReferences=true`
- Reusable package builds use package dependencies instead of sibling projects and stamp the GitVersion release number (example `6.7.110`):
  - `dotnet pack .\Sources\EwEUtils\EwEUtils.vbproj --configuration Release --output .\nuget /p:Version=<semver> /p:FileVersion=<semver> /p:PackageVersion=<semver> /p:UseProjectReferences=false`
  - `dotnet pack .\Sources\EwECore\EwECore.vbproj --configuration Release --output .\nuget /p:Version=<semver> /p:FileVersion=<semver> /p:PackageVersion=<semver> /p:UseProjectReferences=false`
  - `dotnet pack .\Sources\ScientificInterfaceShared\ScientificInterfaceShared.vbproj --configuration Release --output .\nuget /p:Version=<semver> /p:FileVersion=<semver> /p:PackageVersion=<semver> /p:UseProjectReferences=false`
  - Pack order matters: EwEUtils, then EwECore, then ScientificInterfaceShared, all from the same commit.
- There is no separate repo-wide lint command; the PR gate in `.github\workflows\build-check.yml` is a Windows build check of `.\Sources\EwE6.sln`.
- The only in-tree automated test-like entry point is the MSP link harness in `Sources\EwECustomPlugins\EwEMSPChallengeIntegration\EwEMSPLinkTests`:
  - Run from that directory so its relative `Input` and `output` folders resolve correctly.
  - Full harness: `dotnet run --project .\EwEMSPLinkTests.csproj --configuration Debug`
  - Single scenario: `dotnet run --project .\EwEMSPLinkTests.csproj --configuration Debug -- North_Sea_basic_1.json`
  - The harness discovers scenario files from `Input\**\*.json` by filename only.

## Repository prerequisites

- `NuGet.config` uses package source mapping so all `Eii.*` packages must come from the `github-Official-EwE` feed, not `nuget.org`.
- Follow the root `README.md` guidance to add GitHub Packages credentials in your user-level NuGet configuration before restore/build/pack commands that need `Eii.*` packages.

## High-level architecture

- `Sources\EwE6.sln` is the main solution. The big-picture split is:
  - `Sources\EwEUtils`: shared low-level utilities, logging, networking, database, drawing, and general helpers.
  - `Sources\EwECore`: the domain/model engine for Ecopath, Ecosim, Ecospace, MSE, search, spatial logic, and related calculations. This project multi-targets `net48` and `net10.0`.
  - `Sources\ScientificInterfaceShared`: reusable WinForms/UI controls and shared UI infrastructure built on top of `EwECore` and `EwEUtils`.
  - `Sources\ScientificInterface`: the `net48` WinForms desktop application that assembles the UI and references many plugin projects directly.
- Plugin support is a first-class architectural boundary:
  - The plugin contract lives in `Sources\EwECore\Plugins\Plugin templates\IPlugin.vb`.
  - Runtime loading is handled by `Sources\EwECore\Plugins\Handlers\cPluginManager.vb`.
  - The main UI shell loads plugins in `Sources\ScientificInterface\frmEwE6.vb` from both the application root and `.\plugins`, and it respects `My.Settings.DisabledPlugins`.
- Messaging inside the core uses the publisher/subscriber infrastructure under `Sources\EwECore\Publisher-Subscriber`. `cMessagePublisher` routes `cMessage` instances to registered handlers and allows plugin preprocessing before selected messages are logged.
- Not every extension is built from source in the main solution. `ScientificInterface.vbproj` also consumes packaged plugins such as `Eii.Ecopath.EcospaceSpinup-net48-Plugin` and `Eii.Ecopath.SpatialTemporalFramework-net48-Plugin`.
- Localisation is managed outside the main source tree as well: `Localization\source\<assembly>\AssemblySettings.json` contains assembly-level localisation metadata, while individual projects still carry their own `.resx` resources.

## Unit test conventions

- Every test method body must use the **Arrange-Act-Assert (AAA)** pattern with explicit inline comments:
  ```csharp
  // Arrange
  ...
  // Act
  ...
  // Assert
  ...
  ```
- When a test has no meaningful arrange step, keep the `// Arrange` comment and leave that section empty (or write a brief note such as `// no setup required`).
- When act and assert cannot be cleanly separated (e.g. a single `Should()` call on the result of a method), use `// Act & Assert` as a combined comment.
- `[Theory]` tests that receive all inputs via `[InlineData]` may use `// Arrange (from InlineData)` at the top instead of a separate setup block.
- Only use the Moq package version 4.18.0
- Only use the FluentAssertions package version 6.9.0

## Versioning and assembly metadata

- `AssemblyVersion` is pinned to `6.7.0.0` for all projects in `Sources\Directory.Build.props`. It is the strong-name binding identity that external plugin packages (e.g. `Eii.Ecopath.EcospaceSpinup-net48-Plugin`) compile against. Only bump it on a deliberate breaking change, and rebuild/republish the external plugin packages when you do.
- Release numbers never go into `AssemblyVersion`. GitVersion computes the release (e.g. `6.7.110`) in `.github\workflows\Create-nuget-packages.yml` and `Create-nuget-packages-and-release.yml`, and CI stamps it via `/p:Version=...`, `/p:FileVersion=...` and `/p:PackageVersion=...`. Never hardcode versions in project files or source.
- `Sources\Version.props` no longer exists; do not reintroduce it or an `$(EweVersion)` property.
- `Sources\EwECore\Directory.Build.props` and `Sources\EwEUtils\Directory.Build.props` must keep importing the shared `Sources\Directory.Build.props`; a nested `Directory.Build.props` otherwise shadows the repo-wide settings.
- Assembly metadata is generated by the SDK (`GenerateAssemblyInfo` is on). `EwECore` and `EwEUtils` have no `My Project\AssemblyInfo.vb`; descriptive metadata lives as MSBuild properties (`Title`, `Description`, `Product`, `Company`, `Copyright`) and remaining attributes (`CLSCompliant`, `ComVisible`, `Guid`) as `<AssemblyAttribute>` items in the `.vbproj`. Do not add `AssemblyInfo.vb` files back.
- The version shown in the UI (`EwE6ApplicationFramework.Version`) is read from `AssemblyInformationalVersion`, not `AssemblyVersion`, so it matches the release number and the exe `FileVersion`. Local builds without `/p:Version` display the SDK default `1.0.0`.
- Floating package references such as `Version="6.7.*"` in plugin repos only affect compile-time resolution; the exact `AssemblyVersion` is baked into the compiled manifest, which is why the stable `6.7.0.0` identity is required.

## Key conventions

- `UseProjectReferences=true` is the default repo behavior for local development. Packaging switches this to `false` so pack operations consume published `Eii.*` packages instead of sibling projects. Respect that toggle instead of rewriting references manually.
- Prefer changing the correct layer instead of pushing logic upward into `ScientificInterface`:
  - engine/model logic belongs in `EwECore`
  - reusable UI infrastructure belongs in `ScientificInterfaceShared`
  - app-shell composition and startup belong in `ScientificInterface`
  - optional features generally live as plugins under `Sources\EwECustomPlugins`
- Plugin discovery is interface-based, not registration-based. If you add or modify a plugin, preserve the `IPlugin` metadata contract: `Name` must stay unique and is also used for plugin UI ordering.
- Be careful with working-directory-sensitive code and assets. Plugin loading uses relative folders such as `.\` and `.\plugins`, and the MSP harness uses relative `Input` and `output` directories.
- This is a Windows-first repository: the desktop app targets `net48`, the solution is built in x86 and x64 configurations, and release automation also builds Inno Setup installers from `Deployment\EwE6_relpath.iss`.

## Documentation Comments

- Documentation comments must be a maximum of 165 characters long.