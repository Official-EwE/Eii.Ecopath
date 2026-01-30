' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' <summary>
    ''' Interface for writing Monte Carlo results to file
    ''' </summary>
    Public Interface IMonteCarloResultsWriter

        Sub Init()

        Sub Save(iTrial As Integer)

        Sub Finish()

        Function DisplayName() As String

        Function DataName() As String



    End Interface

End Namespace
