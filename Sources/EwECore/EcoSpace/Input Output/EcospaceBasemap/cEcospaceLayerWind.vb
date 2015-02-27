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
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace vector data.
''' </summary>
Public Class cEcospaceLayerWind
    Inherits cEcospaceLayerVector

#Region " Private vars "

    ''' <summary>Month [1, 12] to operate on.</summary>
    Private m_iMonth As Integer = 1

#End Region ' Private vars

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the wind layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, 1, manager, "", eVarNameFlags.LayerWind, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerWind
    End Sub

#End Region ' Construction

#Region " Filter "

    Public Property Month() As Integer
        Get
            Return Me.m_iMonth
        End Get
        Set(ByVal value As Integer)
            value = Math.Max(1, Math.Min(cCore.N_MONTHS, value))
            If (value <> Me.m_iMonth) Then
                Me.m_iMonth = value
                Me.Invalidate()
            End If
        End Set
    End Property

#End Region ' Filter

#Region " Private bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get X velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Return DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth)
        End Get
        Set(ByVal value As Single)
            DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get Y velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Return DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth)
        End Get
        Set(ByVal value As Single)
            DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth) = value
        End Set

    End Property

#End Region ' Private bits

    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_WIND
    End Function

End Class
