' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Core

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
    Public Sub New(core As cCore,
                   manager As IEcospaceLayerManager,
                   strName As String,
                   varName As eVarNameFlags,
                   Optional iIndex As Integer = cCore.NULL_VALUE)

        MyBase.New(core, core.m_EcospaceData.GetLayerID(varName, iIndex), manager, strName, varName, iIndex, GetType(Boolean))

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
    Public Sub New(core As cCore,
                   data As Boolean(,),
                   strName As String,
                   Optional meta As cVariableMetaData = Nothing,
                   Optional vn As eVarNameFlags = eVarNameFlags.NotSet)

        MyBase.New(core, CObj(data), strName, GetType(Boolean), meta, vn)

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    ''' <inheritdocs cref="cEcospaceLayer.Cell"/>
    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Return DirectCast(Me.Data, Boolean(,))(iRow, iCol)
        End Get
        Set(value As Object)
            Dim d As Boolean(,) = DirectCast(Me.Data, Boolean(,))
            If Me.ValidateCellValue(value) Then
                Dim i As Boolean = CBool(value)
                If Me.ValidateCellPosition(iRow, iCol) Then
                    d(iRow, iCol) = i
                End If
            End If
        End Set
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.MaxValue"/>
    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1
        End Get
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.MinValue"/>
    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0
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

    Protected Overrides Function ValidateCellValue(value As Object) As Boolean

        If Convert.IsDBNull(value) Then Return False
        Dim sValue As Single = Convert.ToSingle(value)
        Return (cCore.NULL_VALUE <> sValue)

    End Function

End Class