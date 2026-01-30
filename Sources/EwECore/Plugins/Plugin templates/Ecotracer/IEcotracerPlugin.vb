' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.ContaminantTracing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that are invoked from the EwE
    ''' Ecosim model.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcotracerPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecotracer has loaded a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source from which
        ''' data is being loaded.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub LoadEcotracerScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecotracer has saved a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source to which
        ''' data is being saved.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub SaveEcotracerScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecotracer scenario has been closed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub CloseEcospaceScenario()

    End Interface

End Namespace