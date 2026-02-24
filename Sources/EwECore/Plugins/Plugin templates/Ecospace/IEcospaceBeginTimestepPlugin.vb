' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace begin time step logic. Plug-ins of this
    ''' type are invoked as soon as the EwE Core is about to begin its calculatios
    ''' of an Ecospace time step.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceBeginTimestepPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Begin of an Ecospace time step.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">Ecospace data structures.</param>
        ''' <param name="iTime">Cumulative time step.</param>
        Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer)

    End Interface

End Namespace