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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace advection data.
''' </summary>
Public Class cEcospaceLayerAdvection
    Inherits cEcospaceLayerVector

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the advection layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)

        MyBase.New(theCore, cCore.NULL_VALUE, manager, _
                   My.Resources.CoreDefaults.CORE_DEFAULT_ADVECTION, _
                   eVarNameFlags.LayerAdvection, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerAdvection

    End Sub

#End Region ' Construction

#Region " Private bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get X velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Return DirectCast(Me.Data, Single()(,))(0)(iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,))(0)(iRow, iCol) = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get Y velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Return DirectCast(Me.Data, Single()(,))(1)(iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,))(1)(iRow, iCol) = value
            End If
        End Set

    End Property

#End Region ' Private bits

End Class
