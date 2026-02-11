' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace excluded cells.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerExclusion
    Inherits cEcospaceLayerBoolean

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_EXCLUSION, eVarNameFlags.LayerExclusion, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerExclusion
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States if a given cell is excluded from Ecospace dynamics.
    ''' </summary>
    ''' <param name="iRow">The row of the cell to check.</param>
    ''' <param name="iCol">The column of the cell to check.</param>
    ''' <returns>True if the given cell is an excluded cell.</returns>
    ''' <seealso cref="IsIncludedCell(Integer, Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Property IsExcludedCell(iRow As Integer, iCol As Integer) As Boolean
        Get
            Return (CBool(Me.Cell(iRow, iCol)) = True)
        End Get
        Set(value As Boolean)
            If Not Me.ValidateCellPosition(iRow, iCol) Then Return
            Me.Cell(iRow, iCol) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States if a given cell is included for Ecospace dynamics.
    ''' </summary>
    ''' <param name="iRow">The row of the cell to check.</param>
    ''' <param name="iCol">The column of the cell to check.</param>
    ''' <returns>True if the given cell is an excluded cell.</returns>
    ''' <seealso cref="IsExcludedCell(Integer, Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Property IsIncludedCell(iRow As Integer, iCol As Integer) As Boolean
        Get
            Return Not Me.IsExcludedCell(iRow, iCol)
        End Get
        Set(value As Boolean)
            Me.IsExcludedCell(iRow, iCol) = Not value
        End Set
    End Property

End Class
