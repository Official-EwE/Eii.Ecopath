' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

#End Region ' Imports

Public Interface IHashSummarizer

    ''' <summary>
    ''' Return a name for the benefit of a user interface
    ''' </summary>
    Function Name() As String

    ''' <summary>
    ''' Initialize this instance for use.
    ''' </summary>
    Sub Init()

    ''' <summary>
    ''' Compute and return the hash values for a set of summarized EwE values.
    ''' </summary>
    ''' <hmm>
    ''' This should really return an array, not an actual list. A list suggests that results are open for further manipulation...
    ''' </hmm>
    Function HashValues() As List(Of cHashValues)

End Interface
