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
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerDepth, cCore.NULL_VALUE)
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
