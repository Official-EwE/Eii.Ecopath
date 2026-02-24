' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that are invoked when Ecosim finishes
    ''' a run, and all <see cref="IEcosimRunCompletedPlugin"/> points have been called.
    ''' <seealso cref="IEcosimRunCompletedPlugin"/>
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimRunCompletedPostPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecosim has finished running, and all
        ''' <see cref="IEcosimRunCompletedPlugin"/> points have been called.
        ''' <seealso cref="IEcosimRunCompletedPlugin"/>
        ''' </summary>
        ''' <param name="EcosimDatastructures">Ecosim data structires.</param>
        ''' -----------------------------------------------------------------------
        Sub EcosimRunCompletedPost(EcosimDatastructures As Object)

    End Interface

End Namespace