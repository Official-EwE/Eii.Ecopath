' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that are invoked when Ecosim finishes
    ''' a run.
    ''' <seealso cref="IEcosimRunCompletedPostPlugin"/>
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimRunCompletedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecosim has finished running.
        ''' <seealso cref="IEcosimRunCompletedPostPlugin"/>
        ''' </summary>
        ''' <param name="EcosimDatastructures">Ecosim data structires.</param>
        ''' -----------------------------------------------------------------------
        Sub EcosimRunCompleted(EcosimDatastructures As Object)

    End Interface

End Namespace