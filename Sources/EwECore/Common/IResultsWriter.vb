' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







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
