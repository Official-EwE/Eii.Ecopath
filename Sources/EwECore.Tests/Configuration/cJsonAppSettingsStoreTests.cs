// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwEUtils.Configuration;

namespace EwECore.Tests.Configuration;

public sealed class cJsonAppSettingsStoreTests
{
    [Fact]
    public void SaveAndReload_PreservesStringSetting()
    {
        // Arrange
        string directory = Path.Combine(Path.GetTempPath(), $"EwE-settings-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string filePath = Path.Combine(directory, "settings.json");
            var store = new cJsonAppSettingsStore(filePath);
            store.SetString("r.executable", "/usr/bin/R");

            // Act
            store.Save();
            var reloaded = new cJsonAppSettingsStore(filePath);
            string value = string.Empty;
            bool found = reloaded.TryGetString("r.executable", ref value);

            // Assert
            Assert.True(found);
            Assert.Equal("/usr/bin/R", value);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void InvalidSettingsFile_IsPreservedAndStartsWithEmptyValues()
    {
        // Arrange
        string directory = Path.Combine(Path.GetTempPath(), $"EwE-settings-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string filePath = Path.Combine(directory, "settings.json");
            File.WriteAllText(filePath, "not-json");

            // Act
            var store = new cJsonAppSettingsStore(filePath);
            string value = string.Empty;
            bool found = store.TryGetString("r.executable", ref value);
            string[] backups = Directory.GetFiles(directory, "settings.json.bad-*");

            // Assert
            Assert.False(found);
            Assert.Single(backups);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
