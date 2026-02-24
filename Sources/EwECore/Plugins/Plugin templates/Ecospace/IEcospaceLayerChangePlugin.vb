' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace events before and after a spatial layer 
    ''' receives content through the spatial-temporal data framework.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceLayerChangePlugin
        Inherits IPlugin

        ''' <summary>
        ''' An Ecospace layer is about to receive data through the spatial-temporal
        ''' data framework. This call allows plug-ins to gather the data in the 
        ''' layer before it will be altered.
        ''' </summary>
        ''' <param name="iTime">Cumulative time step.</param>
        ''' <param name="dt">Absolute time for the time step.</param>
        ''' <param name="layer">The Ecospace basemap layer that is about to receive data.</param>
        Sub EcospaceBeginLayerChange(iTime As Integer, dt As Date, layer As Object)

        ''' <summary>
        ''' An Ecospace layer has just received data through the spatial-temporal
        ''' data framework. This call allows plug-ins to gather the data in the 
        ''' layer after it has been altered and integrated into Ecospace.
        ''' </summary>
        ''' <param name="iTime">Cumulative time step.</param>
        ''' <param name="dt">Absolute time for the time step.</param>
        ''' <param name="layer">The Ecospace basemap layer that received data.</param>
        Sub EcospaceEndLayerChange(iTime As Integer, dt As Date, layer As Object)

    End Interface

End Namespace