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
Imports EwECore.Core
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base layer providing access to Ecospace data as cells of Boolean values.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerBoolean
    Inherits cEcospaceLayer

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer of Boolean values that derives its data and 
    ''' identity from a manager.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore, _
                   ByVal manager As IEcospaceLayerManager, _
                   ByVal strName As String, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE)

        MyBase.New(core, core.m_EcoSpaceData.getLayerID(varName, iIndex), manager, strName, varName, iIndex, GetType(Boolean))

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer that is hard-linked to an array of data.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="strName">Display name for the layer.</param>
    ''' <param name="data">Data to attach to the layer, if any.</param>
    ''' <param name="meta">Optional metadata for contraining data interactions.</param>
    ''' <param name="vn">Optional varname for the layer, if <paramref name="data"/>
    ''' was left empty.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore, _
                   ByVal data As Boolean(,), _
                   ByVal strName As String, _
                   Optional ByVal meta As cVariableMetaData = Nothing, _
                   Optional ByVal vn As eVarNameFlags = eVarNameFlags.NotSet)

        MyBase.New(core, CObj(data), strName, GetType(Boolean), meta, vn)

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    ''' <inheritdocs cref="cEcospaceLayer.Cell"/>
    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Return DirectCast(Me.Data, Boolean(,))(iRow, iCol)
        End Get
        Set(ByVal value As Object)
            Dim d As Boolean(,) = DirectCast(Me.Data, Boolean(,))
            Dim i As Boolean = CBool(value)
            If Me.ValidateCellValue(i) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol) = i
                End If
            End If
        End Set
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.MaxValue"/>
    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return CSng(Math.Max(CSng(True), CSng(False)))
        End Get
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.MinValue"/>
    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return CSng(Math.Min(CSng(True), CSng(False)))
        End Get
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.NumValueCells"/>
    Public Overrides ReadOnly Property NumValueCells As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides Sub Invalidate()
        ' NOP
    End Sub

#End Region ' Cell interaction

End Class