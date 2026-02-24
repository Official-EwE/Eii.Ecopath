' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' <summary>
    ''' Plugin for initialization of Ecosim Scenarios
    ''' </summary>
    ''' <remarks>Contains plugin points for initialization of Ecosim Scenarios</remarks>
    Public Interface IEcosimInitializedPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Plugin Point called when an Ecosim Scenario has loaded
        ''' </summary>
        ''' <param name="EcosimDatastructures">cEcosimDataStructures passed as an object.</param>
        ''' <remarks>Called after an Ecosim scenario has loaded but prior to initialization of data.</remarks>
        Sub EcosimInitialized(EcosimDatastructures As Object)

    End Interface

End Namespace