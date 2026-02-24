' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for a plug-in that is invoked when the Ecosim model is about to
    ''' start computing a time step, after all instances of <see cref="IEcosimBeginTimestepPlugin"/>
    ''' points have been called.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimBeginTimestepPostPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Ecosim is about to compute a time step, and all instances of <see cref="IEcosimBeginTimestepPlugin"/>
        ''' have had the opportunity to run.
        ''' </summary>
        ''' <param name="BiomassAtTimestep">The biomasses at the beginning at the time step.</param>
        ''' <param name="EcosimDatastructures">The Ecosim data structures that you can poke around in.</param>
        ''' <param name="iTime">The time step that will be executed.</param>
        Sub EcosimBeginTimeStepPost(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer)

    End Interface

End Namespace