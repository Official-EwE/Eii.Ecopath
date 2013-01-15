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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace depth data.
''' </summary>
Public Class cEcospaceLayerDepth
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal meta As cVariableMetaData)
        MyBase.New(theCore, manager, _
                   My.Resources.CoreDefaults.CORE_DEFAULT_DEPTH, _
                   EwEUtils.Core.eVarNameFlags.LayerDepth, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerDepth
     End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States if a given cell is a water cell.
    ''' </summary>
    ''' <param name="iRow">The row of the cell to check.</param>
    ''' <param name="iCol">The column of the cell to check.</param>
    ''' <returns>True if the given cell is a water cell.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsWaterCell(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        If Not Me.ValidateCellPosition(iRow, iCol) Then Return False
        Return CInt(Me.Cell(iRow, iCol)) > 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States if a given cell is a land cell.
    ''' </summary>
    ''' <param name="iRow">The row of the cell to check.</param>
    ''' <param name="iCol">The column of the cell to check.</param>
    ''' <returns>True if the given cell is a land cell.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsLandCell(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        If Not Me.ValidateCellPosition(iRow, iCol) Then Return False
        Return CInt(Me.Cell(iRow, iCol)) <= 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States if a given cell is a land cell that lies beside water.
    ''' </summary>
    ''' <param name="iRow">The row of the cell to check.</param>
    ''' <param name="iCol">The column of the cell to check.</param>
    ''' <returns>True if the given cell is a coastal cell.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsCoastalCell(ByVal iRow As Integer, ByVal iCol As Integer) As Boolean
        If Not Me.IsLandCell(iRow, iCol) Then Return False
        For i As Integer = iRow - 1 To iRow + 1
            For j As Integer = iCol - 1 To iCol + 1
                If IsWaterCell(i, j) Then Return True
            Next
        Next
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of water cells in the map.
    ''' </summary>
    ''' <returns>The number of water cells in the map.</returns>
    ''' -----------------------------------------------------------------------
    Public Function NumWaterCells() As Integer
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim iNumCells As Integer = 0
        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                If Me.IsWaterCell(iRow, iCol) Then
                    iNumCells += 1
                End If
            Next
        Next
        Return iNumCells
    End Function

End Class
