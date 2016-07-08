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
''' Layer providing access to Ecospace upwelling data.
''' </summary>
Public Class cEcospaceLayerUpwelling
    Inherits cEcospaceLayerSingle

#Region " Private vars "

    ''' <summary>Month [1, 12] to operate on.</summary>
    Private m_iMonth As Integer = 1

#End Region ' Private vars


    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerUpwelling, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerUpwelling
    End Sub


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
            Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
            Return data(iMonth)(iRow, iCol)
        End Get
        Set(ByVal value As Object)
            Dim d As Single()(,) = DirectCast(Me.Data, Single()(,))
            Dim s As Single = Convert.ToSingle(value)
            d(iMonth)(iRow, iCol) = s
            Me.Invalidate()
        End Set
    End Property


    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_UPWELLING
    End Function

#End Region


End Class
