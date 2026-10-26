# Eii.Ecopath.EwEUtils

The Utilities for Ecopath with Ecosim (EwE)

https://ecopath.org/

## Cross-platform application settings

`cJsonAppSettingsStore` persists per-user string settings in `settings.json`:

- Linux: `$XDG_CONFIG_HOME/Ecopath with Ecosim/settings.json`, or `~/.config/Ecopath with Ecosim/settings.json`
- Windows: `%APPDATA%\Ecopath with Ecosim\settings.json`
- macOS: `~/Library/Application Support/Ecopath with Ecosim/settings.json`

The versioned JSON document uses a `schemaVersion` field and a `values` object. For example:

```json
{
  "schemaVersion": 1,
  "values": {
    "r.executable": "/usr/bin/R"
  }
}
```

R discovery checks `r.executable`, `R_BIN`, `R_HOME`, and `PATH` in that order. Windows registry discovery remains as a compatibility fallback on Windows only.

