' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports EwECore
Imports EwECore.Common

''' ---------------------------------------------------------------------------
''' <summary>
''' Cell summaryt for a single transect.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransectSummary

    Private m_values As Single()

    Public Sub New(t As cTransect, bm As cEcospaceBasemap, l As cEcospaceLayer, iIndex As Integer)

        Me.Transect = t
        Me.Name = l.Name

        Dim cells As Point() = t.Cells(bm)
        ReDim Me.m_values(cells.Count - 1)
        For iCell As Integer = 0 To cells.Count - 1
            Dim pt As Point = cells(iCell)
            Dim sValue As Single = cCore.NULL_VALUE
            If bm.IsModelledCell(pt.Y, pt.X) Or l.VarName = eVarNameFlags.LayerDepth Then
                sValue = CSng(l.Cell(pt.Y, pt.X, iIndex))
            End If
            Me.m_values(iCell) = sValue
        Next

    End Sub

    Public Sub New(t As cTransect, bm As cEcospaceBasemap, strName As String, ecospaceoutput As Single(,,), iIndex As Integer)

        Me.Transect = t
        Me.Name = strName

        Dim cells As Point() = t.Cells(bm)
        ReDim Me.m_values(cells.Count - 1)
        For iCell As Integer = 0 To cells.Count - 1
            Dim pt As Point = cells(iCell)
            Dim sValue As Single = cCore.NULL_VALUE
            If bm.IsModelledCell(pt.Y, pt.X) Then
                sValue = ecospaceoutput(pt.Y, pt.X, iIndex)
            End If
            Me.m_values(iCell) = sValue
        Next

    End Sub

    Public ReadOnly Property Transect As cTransect
    Public ReadOnly Property Name As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="i">Zero-based index</param>
    ''' <returns></returns>
    Public ReadOnly Property Value(i As Integer) As Single
        Get
            Return Me.m_values(i)
        End Get
    End Property

End Class