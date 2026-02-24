' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace end time step logic. Plug-ins of this
    ''' type are invoked as soon as the EwE Core has finished its calculatios
    ''' of an Ecospace time step, and after all IEcospaceBeginTimestepPlugin points
    ''' have been called.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceBeginTimestepPostPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Begin of an Ecospace time step, invoked after all <see cref="IEcospaceBeginTimestepPlugin"/> calls have been made.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">Ecospace data structures.</param>
        ''' <param name="iTime">Cumulative time step.</param>
        Sub EcospaceBeginTimeStepPost(EcospaceDatastructures As Object, iTime As Integer)

    End Interface

End Namespace