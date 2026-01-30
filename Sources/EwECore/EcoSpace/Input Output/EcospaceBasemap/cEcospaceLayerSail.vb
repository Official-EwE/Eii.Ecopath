' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace sailing cost data.
''' </summary>
Public Class cEcospaceLayerSail
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerSail, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerSail
    End Sub

#Region " Cell interaction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of a sailing cost cell.
    ''' </summary>
    ''' <param name="iRow">Row index of the cell to access.</param>
    ''' <param name="iCol">Column index of the cell to access.</param>
    ''' <remarks>
    ''' Note that cells will be accessed for the currently selected fleet index.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
                Return data(Me.Index)(iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
            If Me.ValidateCellPosition(iRow, iCol) Then
                data(Me.Index)(iRow, iCol) = CSng(value)
            End If
        End Set
    End Property

#End Region ' Cell interaction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        If (Me.Index = 0) Then Return My.Resources.CoreDefaults.CORE_ALL_FLEETS
        Return Me.m_core.EcopathFleetInputs(Me.Index).Name
    End Function

#End Region ' Overrides

End Class
