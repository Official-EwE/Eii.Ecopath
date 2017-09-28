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

''' -----------------------------------------------------------------------
''' <summary>
''' A default complexity rule for the user to select.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cComplexityRuleDefault
    Inherits cComplexityRule

#Region " Construction "

    Public Sub New(strName As String, a As Single, b As Single, c As Single)
        MyBase.New(strName, a, b, c)
    End Sub

#End Region ' Construction

#Region " Overrides "

    Public Overrides Function IsDefault() As Boolean
        Return True
    End Function

#End Region ' Overrides

End Class
