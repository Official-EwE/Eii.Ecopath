' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for adding items that can summarize their configuration into a 
    ''' string for computing model validation checksums.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface ISummarizable

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Summarizes the unique content of an object that allows computation of a 
        ''' checksum over this object.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Summary As String

    End Interface

End Namespace
