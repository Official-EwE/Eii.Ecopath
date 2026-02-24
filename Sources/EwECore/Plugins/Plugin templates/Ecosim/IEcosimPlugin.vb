' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that are invoked from the EwE
    ''' Ecosim model.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecosim has loaded a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source from which
        ''' data is being loaded.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub LoadEcosimScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecosim has saved a scenario, exposing
        ''' the datasource that the scenario was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source to which
        ''' data is being saved.</param>
        ''' <remarks>This plugin point is non-exclusive; each implementation 
        ''' of this plugin point will be called.</remarks>
        ''' -----------------------------------------------------------------------
        Sub SaveEcosimScenario(dataSource As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecosim scenario has been closed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub CloseEcosimScenario()

    End Interface

End Namespace