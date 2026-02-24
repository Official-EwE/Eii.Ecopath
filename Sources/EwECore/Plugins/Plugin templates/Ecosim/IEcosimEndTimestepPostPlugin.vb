' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin points for the end of an Ecosim time step, after all <see cref="IEcosimEndTimestepPlugin"/>
    ''' plug-ins have ran.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimEndTimestepPostPlugin
        Inherits IPlugin

        Sub EcosimEndTimeStepPost(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object)

    End Interface

End Namespace