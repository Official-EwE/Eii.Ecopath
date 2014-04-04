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

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' <summary>
''' Layer providing access to Ecospace migration data.
''' </summary>
Public Class cEcospaceLayerMigration
    Inherits cEcospaceLayer

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, "", eVarNameFlags.LayerMigration, iIndex, Nothing)
        Me.m_dataType = eDataTypes.EcospaceLayerMigration
    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Private m_asData As Single(,)

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            If Me.m_asData Is Nothing Then
                Me.Refresh()
            End If
            Return Me.m_asData(iRow, iCol)
        End Get
        Set(ByVal value As Object)
            Dim i As Integer = CInt(Math.Max(Math.Min(cCore.N_MONTHS, CInt(value)), 1))

            Me.PrefRow(Me.Index, i) = iRow
            Me.PrefCol(Me.Index, i) = iCol
            Me.Invalidate()
        End Set
    End Property

    Public Overrides Sub Invalidate()
        Me.m_asData = Nothing
    End Sub

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return cCore.N_MONTHS
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property NumValueCells As Integer
        Get
            Return cCore.N_MONTHS
        End Get
    End Property

#End Region ' Cell interaction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_MIGRATION, Me.Index, Me.m_core.EcoPathGroupInputs(Me.Index).Name)
    End Function

#End Region ' Overrides

#Region " Private bits "

    Private Function PrefRow() As Integer(,)
        Dim d As Object = Me.Data
        Return DirectCast(d, Integer()(,))(0)
    End Function

    Private Function PrefCol() As Integer(,)
        Dim d As Object = Me.Data
        Return DirectCast(d, Integer()(,))(1)
    End Function

    Private Sub Refresh()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim aiPrefRow As Integer(,) = Me.PrefRow
        Dim aiPrefCol As Integer(,) = Me.PrefCol

        ReDim m_asData(bm.InRow, bm.InCol)

        For iRowTest As Integer = 1 To bm.InRow
            For iColTest As Integer = 1 To bm.InCol
                Me.m_asData(iRowTest, iColTest) = cCore.NULL_VALUE
            Next
        Next

        For iMonth As Integer = 1 To cCore.N_MONTHS
            Dim iRow As Integer = CInt(aiPrefRow(Me.Index, iMonth))
            Dim iCol As Integer = CInt(aiPrefCol(Me.Index, iMonth))
            If Me.ValidateCellPosition(iRow, iCol) Then
                Me.m_asData(iRow, iCol) = iMonth
            End If
        Next
    End Sub

#End Region ' Private bits

End Class
