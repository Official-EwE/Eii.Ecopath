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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports DefaultRes = EwECore.My.Resources.CoreDefaults

Imports EwEUtils.SystemUtilities.cSystemUtils


#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace habitat capacity data.
''' </summary>
Public Class cEcospaceLayerHabitatCapacity
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal dt As eDataTypes, ByVal vn As eVarNameFlags, iIndex As Integer)
        MyBase.New(theCore, manager, _
                   String.Format(CStr(IIf(vn = eVarNameFlags.LayerHabitatCapacity, DefaultRes.CORE_DEFAULT_HABCAP, DefaultRes.CORE_DEFAULT_HABCAP_INPUT)), iIndex), _
                   vn, iIndex)
        Me.m_dataType = dt
    End Sub

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return data(iRow, iCol, Me.Index)
            Return 0
        End Get
        Set(ByVal value As Object)
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then data(iRow, iCol, Me.Index) = CSng(value)
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

#End Region ' Cell interaction

End Class
