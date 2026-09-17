// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;

namespace EwECore.Tests.Fixtures;

/// <summary>
/// xUnit shared fixture that loads the Anchovy Bay Spatial model once per test
/// collection, runs Ecopath and Ecosim to put the core in a state ready for MSE
/// tests, then disposes the core when the collection is torn down.
/// </summary>
public sealed class ModelFixture : IAsyncLifetime
{
    private const string ModelFileName = "Anchovy Bay Spatial.ewemdb";

    public cCore Core { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        string modelPath = Path.Combine(AppContext.BaseDirectory, "TestData", ModelFileName);

        Core = new cCore() { PluginManager = new cPluginManager() };
        bool loaded = Core.LoadModel(modelPath);
        if (!loaded)
            throw new InvalidOperationException($"Failed to load model: {modelPath}");

        bool ecopathOk = Core.RunEcopath();
        if (!ecopathOk)
            throw new InvalidOperationException("Ecopath failed to run on the loaded model.");

        if (Core.nEcosimScenarios == 0)
            throw new InvalidOperationException("The model contains no Ecosim scenarios.");

        bool scenarioLoaded = Core.LoadEcosimScenario(1);
        if (!scenarioLoaded)
            throw new InvalidOperationException("Failed to load Ecosim scenario 1.");

        bool ecosimOk = Core.RunEcosim();
        if (!ecosimOk)
            throw new InvalidOperationException("Ecosim failed to run on the loaded model.");

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Core?.CloseModel();
        await Task.CompletedTask;
    }
}

/// <summary>
/// xUnit collection definition that shares a single <see cref="ModelFixture"/>
/// across all test classes decorated with <c>[Collection("Model")]</c>.
/// </summary>
[CollectionDefinition("Model")]
public sealed class ModelCollection : ICollectionFixture<ModelFixture>
{
    // This class has no code; it is a marker for the xUnit collection fixture.
}
