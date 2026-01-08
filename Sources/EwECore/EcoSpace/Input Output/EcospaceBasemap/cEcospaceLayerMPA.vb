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

#Region " Imports "

Option Strict On

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace migration data.
''' </summary>
Public Class cEcospaceLayerMPA
    Inherits cEcospaceLayerInteger

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerMPA, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerMPA
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Dim d As Integer()(,) = DirectCast(Me.Data, Integer()(,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return d(Me.Index)(iRow, iCol) Else Return CInt(cCore.NULL_VALUE)
        End Get
        Set(value As Object)
            Dim d As Integer()(,) = DirectCast(Me.Data, Integer()(,))
            Dim s As Integer = Convert.ToInt16(value)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(Me.Index)(iRow, iCol) = s
                    Me.Invalidate()
                End If
            End If
        End Set
    End Property

    Protected Overrides Function DefaultName() As String
        Return Me.m_core.EcospaceMPAs(Me.Index).Name
    End Function

    Protected Overrides Sub RecalcStats()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim iRows As Integer = bm.InRow
        Dim iCols As Integer = bm.InCol

        Me.m_iMaxValue = Integer.MinValue
        Me.m_iMinValue = Integer.MaxValue
        Me.m_iNumValueCells = 0

        For iRow As Integer = 1 To iRows
            For iCol As Integer = 1 To iCols
                If (bm.IsModelledCell(iRow, iCol)) Then
                    Dim i As Integer = CInt(Me.Cell(iRow, iCol))
                    If i > 0 Then
                        Me.m_iMaxValue = Math.Max(i, Me.m_iMaxValue)
                        Me.m_iMinValue = Math.Min(i, Me.m_iMinValue)
                        Me.m_iNumValueCells += 1
                    End If
                End If
            Next iCol
        Next iRow

        If (Me.m_iMaxValue = Me.m_iMinValue) Then
            Me.m_iMinValue = 0
        End If

        Me.m_bInvalidateStats = False

    End Sub
End Class
