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
''' Layer providing access to Ecospace sailing cost data.
''' </summary>
Public Class cEcospaceLayerSail
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, _
                   String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_SAILCOST, iIndex), _
                   eVarNameFlags.LayerSail, iIndex)
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
    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return data(Me.Index, iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then
                data(Me.Index, iRow, iCol) = CSng(value)
            End If
        End Set
    End Property

#End Region ' Cell interaction

End Class
