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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Cell summaryt for a single transect.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransectSummary

    Private m_values As Single()

    Public Sub New(t As cTransect, cells As ICollection(Of Point), l As cEcospaceLayer, iIndex As Integer)

        Me.Transect = t
        Me.Name = l.Name
        ReDim m_values(cells.Count - 1)
        For iCell As Integer = 0 To cells.Count - 1
            Dim pt As Point = cells(iCell)
            Me.m_values(iCell) = CSng(l.Cell(pt.Y, pt.X, iIndex))
        Next

    End Sub

    Public Sub New(t As cTransect, cells As ICollection(Of Point), strName As String, ecospaceoutput As Single(,,), iIndex As Integer)

        Me.Transect = t
        Me.Name = strName
        ReDim m_values(cells.Count - 1)
        For iCell As Integer = 0 To cells.Count - 1
            Dim pt As Point = cells(iCell)
            Me.m_values(iCell) = ecospaceoutput(pt.Y, pt.X, iIndex)
        Next

    End Sub

    Public ReadOnly Property Transect As cTransect
    Public ReadOnly Property Name As String

End Class
