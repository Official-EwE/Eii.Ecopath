' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin points for the initialization of Ecosim data cEcosimDataStructures 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimModifyTimeseriesPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Ecosim is about to initialize for a run. This point allows plug-ins to 
        ''' adjust loaded reference data prior to a run.
        ''' </summary>
        ''' <param name="TimeSeriesDataStructures">cTimeSeriesDataStructures instance.</param>
        ''' <remarks>Call prior to initialization of run data.</remarks>
        Sub EcosimModifyTimeseries(TimeSeriesDataStructures As Object)

    End Interface

End Namespace