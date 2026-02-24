' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the logic of loading and saving Ecospace data.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospacePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecospace has loaded a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source from which
        ''' data is being loaded.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub LoadEcospaceScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecospace has saved a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source to which
        ''' data is being saved.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub SaveEcospaceScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecospace scenario has been closed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub CloseEcospaceScenario()

    End Interface

End Namespace