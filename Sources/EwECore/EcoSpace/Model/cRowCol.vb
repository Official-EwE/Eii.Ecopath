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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On

Public Class cRowCol

    Public Sub New(ByVal theRow As Integer, ByVal theCol As Integer)
        Me.Row = theRow
        Me.Col = theCol
    End Sub

    Public ReadOnly Property Row As Integer
    Public ReadOnly Property Col As Integer

    Public Overrides Function ToString() As String
        Return "Row: " & Me.Row & ", col: " & Me.Col
    End Function

End Class