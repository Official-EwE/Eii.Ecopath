' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Network Analysis data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface INetworkAnalysisData

        ReadOnly Property Ascendancy() As Single(,)

        ''' <summary>L-index per group</summary>
        ReadOnly Property LIndex As Single()

    End Interface

End Namespace
