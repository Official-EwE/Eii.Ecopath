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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace vector data.
''' </summary>
Public Class cEcospaceLayerWind
    Inherits cEcospaceLayerSingle

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
    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerWind, iIndex)
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

#Region " Overrides "

    Public Overrides Property Cell(iRow As Integer, iCol As Integer) As Object
        Get
            Return Me.Cell(iRow, iCol, Me.m_iMonth)
        End Get
        Set(ByVal value As Object)
            Me.Cell(iRow, iCol, Me.m_iMonth) = value
        End Set
    End Property

    Public Overloads Property Cell(iRow As Integer, iCol As Integer, iMonth As Integer) As Object
        Get
            Return DirectCast(Me.Data, Single(,,))(iRow, iCol, iMonth)
        End Get
        Set(ByVal value As Object)
            Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
            Dim s As Single = Convert.ToSingle(value)
            d(iRow, iCol, iMonth) = s
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_WIND,
                                     cSystemUtils.IIF(Me.Index = 1, My.Resources.CoreDefaults.CORE_DEFAULT_X_VELOCITY, My.Resources.CoreDefaults.CORE_DEFAULT_Y_VELOCITY))
    End Function

#End Region ' Overrides
End Class
