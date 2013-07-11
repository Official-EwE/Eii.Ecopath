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

Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports EwECore

#End Region ' Imports

Namespace Ecospace.Basemap

    <CLSCompliant(False)> _
    Public Class gridImportLayerMappings
        Inherits EwEGrid

#Region " Private vars "

        ''' <summary>The layers available to the grid.</summary>
        Private m_aLayers As cEcospaceLayer()
        ''' <summary>The field names to map upon.</summary>
        Private m_astrFields As String() = {}
        ''' <summary>Mappings. MAPPINGS!</summary>
        Private m_dtLayerMapping As New Dictionary(Of cEcospaceLayer, String)

        Private Enum eColumnTypes As Integer
            ColumnLayer = 0
            ColumnField
        End Enum

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event MappingChanged()

        Public Property Layers() As cEcospaceLayer()
            Get
                Return Nothing
            End Get
            Set(ByVal value As cEcospaceLayer())
                Me.m_aLayers = value
            End Set
        End Property

        Public Property Fields() As String()
            Get
                Return Me.m_astrFields
            End Get
            Set(ByVal value As String())
                Dim lstr As New List(Of String)
                If (value IsNot Nothing) Then lstr.AddRange(value)
                If lstr.IndexOf(SharedResources.GENERIC_VALUE_NONE) = -1 Then lstr.Insert(0, SharedResources.GENERIC_VALUE_NONE)
                Me.m_astrFields = lstr.ToArray()
                Me.RefreshContent()
            End Set
        End Property

        Public Function Mappings() As Dictionary(Of cEcospaceLayer, String)
            Return Me.m_dtLayerMapping
        End Function

        Public Function HasMappings() As Boolean
            For Each l As cEcospaceLayer In Me.m_dtLayerMapping.Keys
                If Not String.IsNullOrWhiteSpace(Me.m_dtLayerMapping(l)) Then
                    Return True
                End If
            Next
            Return False
        End Function

#End Region ' Public interfaces

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If Not Me.HasData() Then Return

            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.ColumnLayer) = New EwEColumnHeaderCell(SharedResources.HEADER_LAYER)
            Me(0, eColumnTypes.ColumnField) = New EwEColumnHeaderCell(SharedResources.HEADER_FIELD)

            Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.ColumnField).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            If Not Me.HasData Then Return

            Me.RowsCount = 1

            Dim layer As cEcospaceLayer = Nothing
            Dim ewec As EwECell = Nothing
            Dim cmb As Cells.Real.ComboBox = Nothing

            For iLayer As Integer = 0 To Me.m_aLayers.Length - 1

                Me.AddRow()
                layer = Me.m_aLayers(iLayer)

                ewec = New EwECell(layer.Name, GetType(String))
                ewec.Style = (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable)
                Me(iLayer + 1, eColumnTypes.ColumnLayer) = ewec

                cmb = New Cells.Real.ComboBox(SharedResources.GENERIC_VALUE_NONE, GetType(String), Me.m_astrFields, True)
                cmb.EditableMode = EditableMode.SingleClick
                Me(iLayer + 1, eColumnTypes.ColumnField) = cmb
                Me(iLayer + 1, eColumnTypes.ColumnField).Behaviors.Add(Me.EwEEditHandler)

                Me.Rows(iLayer + 1).Tag = layer

            Next iLayer

            Me.UpdateMappingsColumn()

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.StretchColumnsToFitWidth()
        End Sub

        Protected Overrides Function OnCellEdited(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            Dim strField As String = Me.FieldAtRow(p.Row)
            Dim layer As cEcospaceLayer = Me.LayerAtRow(p.Row)

            Try
                Me.m_dtLayerMapping(layer) = strField
                Me.UpdateMappingsColumn()
            Catch ex As Exception
            End Try

            Return True

        End Function

        Private Sub UpdateMappingsColumn()

            Dim layer As cEcospaceLayer = Nothing
            Dim strField As String = ""
            Dim cmb As Cells.Real.ComboBox = Nothing
            Dim dm As DataModels.EditorComboBox = Nothing
            Dim strValue As String = ""

            For iRow As Integer = 1 To Me.RowsCount - 1

                layer = Me.LayerAtRow(iRow)

                cmb = DirectCast(Me(iRow, eColumnTypes.ColumnField), Cells.Real.ComboBox)
                dm = DirectCast(cmb.DataModel, DataModels.EditorComboBox)
                dm.DefaultValue = SharedResources.GENERIC_VALUE_NONE

                If Me.m_dtLayerMapping.ContainsKey(layer) Then
                    strValue = Me.m_dtLayerMapping(layer)
                Else
                    strValue = SharedResources.GENERIC_VALUE_NONE
                End If

                Try
                    cmb.Value = strValue
                Catch ex As Exception
                End Try

            Next iRow

            Try
                RaiseEvent MappingChanged()
            Catch ex As Exception

            End Try
        End Sub

        Private Function LayerAtRow(ByVal iRow As Integer) As cEcospaceLayer
            If iRow > 0 And iRow < Me.RowsCount Then
                Return DirectCast(Me.Rows(iRow).Tag, cEcospaceLayer)
            End If
            Return Nothing
        End Function

        Private Function FieldAtRow(ByVal iRow As Integer) As String
            Dim strField As String = ""
            If iRow > 0 And iRow < Me.RowsCount Then
                strField = CStr(Me(iRow, eColumnTypes.ColumnField).Value)
                If (strField = SharedResources.GENERIC_VALUE_NONE) Then
                    strField = ""
                End If
            End If
            Return strField
        End Function

        Private Function HasData() As Boolean
            Return (Me.m_aLayers IsNot Nothing)
        End Function

#End Region ' Overrides

    End Class

End Namespace
