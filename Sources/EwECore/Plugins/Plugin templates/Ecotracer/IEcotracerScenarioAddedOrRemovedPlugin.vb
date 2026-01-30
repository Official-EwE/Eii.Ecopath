' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.ContaminantTracing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for responding to adding or removing an Ecotracer scenario.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcotracerScenarioAddedOrRemovedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecotracer scenario has been added.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source to which
        ''' the scenario was added.</param>
        ''' <param name="scenarioID">The database ID of the newly created Ecotracer scenario.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub EcotracerScenarioAdded(dataSource As Object, scenarioID As Integer)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecotracer scenario has been removed.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source from which
        ''' the scenario was removed.</param>
        ''' <param name="scenarioID">The database ID of the newly created Ecotracer scenario.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub EcotracerScenarioRemoved(dataSource As Object, scenarioID As Integer)

    End Interface

End Namespace