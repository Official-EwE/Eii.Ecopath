# Eii.Ecopath.EwECore

The Core functionality for Ecopath with Ecosim (EwE)

https://ecopath.org/


# Debug Symbols & Step-Through Debugging

This document covers how `EwECore` (and the other `Eii.Ecopath.*` packages)
handle debug symbols, why we're not using Source Link yet, and what to do
if/when we switch.

## Current setup

The repo (`Official-EwE/Eii.Ecopath`) is **private**, and we intend to keep
it that way. That constraint drives everything below.

| Config  | `DebugType` | `EmbedAllSources` | Source in package? |
|---------|-------------|--------------------|---------------------|
| Release | `embedded`  | not set (false)    | No                  |
| Debug   | `embedded`  | `true`             | Yes                 |

- **Release** packages ship an embedded portable PDB, so consumers get
  working stack traces and line numbers, but no actual source text is
  baked in. This is what gets published normally.
- **Debug** packages additionally embed full source, enabling real
  step-through debugging. These are published
  as a separate, clearly-versioned package (see the release workflow,
  `-debug.<run>` suffix) alongside the normal Release package — not a
  replacement for it.
- Both work fully offline. No git/GitHub auth needed for either, since
  everything the debugger needs is inside the DLL itself.

## Why not Source Link (yet)

Source Link is the "modern" alternative: instead of baking source into
the PDB, it publishes just a commit SHA + repo URL, and the debugger
fetches the actual `.cs` files from GitHub on demand when you step into
them.

**Trade-offs vs. our current embedded-source approach:**

| | Embedded source (current) | Source Link |
|---|---|---|
| Package size | Larger (source baked in) | Smaller |
| Source freshness | Whatever was packed at build time | Always exact commit SHA |
| Works offline | Yes | No — needs network + auth each fetch |
| Auth required | None, ever | GitHub credentials on every consumer machine |
| Symbol server | Not needed | nuget.org's is public-only; private repos have nowhere to publish `.snupkg` unless we self-host |

Because the repo stays private permanently, Source Link's main win
(smaller package) comes at a real ongoing cost: every developer and every
CI/build agent that wants to step into `EwECore` source needs GitHub auth
configured, and if that's ever missing or expired, debugging just silently
fails to load source — no error, it just doesn't work. That's more
onboarding friction than the package-size savings are worth right now.

**Decision: stick with embedded source for both Debug and Release configs.
Revisit Source Link only if package size becomes an actual problem.**

## If we do switch later

Keep this section for reference — not currently implemented.

### MSBuild changes

```xml
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
</ItemGroup>
```

Remove `EmbedAllSources` (no longer needed — source comes from GitHub
instead of being baked in).

### Symbols

Since a private repo has no symbol server to publish `.snupkg` to
(GitHub Packages doesn't run one, and nuget.org won't index a private
repo's symbols), skip `SymbolPackageFormat=snupkg` / `IncludeSymbols`
entirely. Ship a portable PDB inside the main package instead:

```xml
<PropertyGroup>
  <DebugType>portable</DebugType>
</PropertyGroup>
```

### Recommended Debug/Release split under Source Link

Local Debug builds (dirty tree, unpushed commits) won't resolve cleanly
via Source Link — it only really works for builds made from a pushed
commit SHA. So:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DebugType>portable</DebugType>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DebugType>portable</DebugType>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
</PropertyGroup>
```

### Consumer-side setup (private repo + Source Link)

Because the repo is private, `raw.githubusercontent.com` returns
401/404 to unauthenticated requests. Every machine that wants to step
into this source would need GitHub credentials wired up:

1. **Install/rely on Git Credential Manager (GCM)** — bundled with
   recent Git for Windows and Visual Studio installs. Confirm with:

   ```
   git config --global credential.helper
   ```

   Should print `manager` or `manager-core`.

2. **One-time per machine:** trigger a GitHub auth prompt so GCM caches
   a token. Easiest way is a throwaway fetch:

   ```
   git ls-remote https://github.com/Official-EwE/Eii.Ecopath.git
   ```

   This pops the GCM browser/device-code login. Needs a GitHub account
   with at least read access to the repo. GCM stores the resulting
   token in Windows Credential Manager (or the platform equivalent on
   macOS/Linux) — no PAT to manage manually in the common case.

3. **Alternative (locked-down machine / CI agent without GCM):** use a
   fine-grained PAT with `Contents: read` on this repo:

   ```
   git config --global credential.https://github.com.username <gh-user>
   ```

   Let the credential store prompt once and paste the PAT as the
   password. For non-interactive agents, point `GIT_ASKPASS` at a small
   script that echoes the PAT — useful for build agents that need to
   symbolicate crash dumps later.

4. **In Visual Studio** (Tools > Options > Debugging > General):
   - Uncheck **Enable Just My Code**
   - Check **Enable Source Link support**

   VS uses the same credential store as Git, so if step 2/3 worked, no
   extra VS-side auth is needed.

5. **Sanity check:** set a breakpoint in consuming code, step into an
   `Eii.Ecopath` call. First hit shows a brief "Source Link" fetch in
   the status bar. If it instead prompts a login dialog or fails
   silently with "no source available," credentials aren't wired up —
   revisit steps 1–3 before assuming Source Link itself is broken.

**Known friction:** this setup must be repeated (or scripted/documented
for onboarding) on every new dev machine and any CI agent that needs to
symbolicate crashes against private source. Budget for that if this is
ever revisited.