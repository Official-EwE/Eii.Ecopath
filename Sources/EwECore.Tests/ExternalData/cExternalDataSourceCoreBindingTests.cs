// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).

using System.Reflection;
using EwECore.ExternalData;
using EwECore.Plugins;
using FluentAssertions;

namespace EwECore.Tests.ExternalData;

public sealed class cExternalDataSourceCoreBindingTests
{
    private static object? GetBoundCore(object dataSource)
    {
        var field = dataSource.GetType().GetField("m_core", BindingFlags.Instance | BindingFlags.NonPublic);
        field.Should().NotBeNull();
        return field!.GetValue(dataSource);
    }

    [Fact]
    public void Initialize_WithTwoCores_KeepsEachDataSourceBoundToItsOwnCore()
    {
        // Arrange
        using var coreA = new cCore();
        using var coreB = new cCore();
        IPlugin[] sourcesA = [new cEconomicDataSource(), new cTaxonDataSource(), new cNetworkAnalysisDataSource()];
        IPlugin[] sourcesB = [new cEconomicDataSource(), new cTaxonDataSource(), new cNetworkAnalysisDataSource()];

        // Act
        foreach (var source in sourcesA) source.Initialize(coreA);
        foreach (var source in sourcesB) source.Initialize(coreB);

        // Assert
        foreach (var source in sourcesA) GetBoundCore(source).Should().BeSameAs(coreA);
        foreach (var source in sourcesB) GetBoundCore(source).Should().BeSameAs(coreB);
    }

    [Fact]
    public void GetInstance_WithoutCoreOrLoadedPlugin_ReturnsNull()
    {
        // Arrange
        using var core = new cCore();
        core.PluginManager = new cPluginManager();

        // Act & Assert
        cEconomicDataSource.getInstance(null!).Should().BeNull();
        cEconomicDataSource.getInstance(core).Should().BeNull();
        cTaxonDataSource.GetInstance(null!).Should().BeNull();
        cTaxonDataSource.GetInstance(core).Should().BeNull();
    }

    [Fact]
    public void UnboundDataSource_ReportsNoDataAndDoesNotThrow()
    {
        // Arrange
        var economic = new cEconomicDataSource();
        var taxon = new cTaxonDataSource();

        // Act
        var economicAvailable = economic.IsDataAvailable(new cEcosimRunType());
        var taxonAvailable = taxon.IsDataAvailable(new cEcosimRunType());
        var taxonProducers = taxon.ProducerNames();

        // Assert
        economicAvailable.Should().BeFalse();
        taxonAvailable.Should().BeFalse();
        taxonProducers.Should().BeEmpty();
    }
}
