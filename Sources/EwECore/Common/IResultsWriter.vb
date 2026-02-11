' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' <summary>
    ''' Interface for writing Ecospace time step results to file
    ''' </summary>
    Public Interface IResultsWriter

        ''' <summary>
        ''' Inititialize a writer.
        ''' </summary>
        ''' <param name="theCore">The core to initialize with.</param>
        Sub Init(theCore As Object)

        ''' <summary>
        ''' Start writing.
        ''' </summary>
        Sub StartWrite()

        ''' <summary>
        ''' End writing.
        ''' </summary>
        Sub EndWrite()

        ''' <summary>
        ''' Return a human-legible name of the data that this writer produces.
        ''' </summary>
        ReadOnly Property DisplayName() As String

        ''' <summary>
        ''' Get/set whether this writer is allowed to write outputs.
        ''' </summary>
        Property Enabled As Boolean

        ReadOnly Property OutputPath As String

    End Interface

End Namespace
