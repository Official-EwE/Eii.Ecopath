' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterface.Ecospace.Basemap.Layers
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace.Basemap

    Public Class gridImportLayerMappings
        Inherits cEwEGrid

#Region " Private vars "

        ''' <summary>The layers available to the grid.</summary>
        Private m_aLayers As cEcospaceLayer()
        ''' <summary>The field names to map upon.</summary>
        Private m_astrFields As String() = {}
        ''' <summary>Mappings. MAPPINGS!</summary>
        Private m_dtLayerMapping As New Dictionary(Of cEcospaceLayer, String)

        Private Enum eColumnTypes As Integer
            ColumnParent = 0
            ColumnLayer
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
            Set(value As cEcospaceLayer())
                Me.m_aLayers = value
            End Set
        End Property

        Public Property Fields() As String()
            Get
                Return Me.m_astrFields
            End Get
            Set(value As String())
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

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.ColumnParent) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.ColumnLayer) = New cEwEColumnHeaderCell(SharedResources.HEADER_LAYER)
            Me(0, eColumnTypes.ColumnField) = New cEwEColumnHeaderCell(SharedResources.HEADER_FIELD)

            Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.ColumnField).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return
            If (Not Me.HasData()) Then Return

            Me.RowsCount = 1

            Dim layer As cEcospaceLayer = Nothing
            Dim cell As cEwECell = Nothing
            Dim cmb As Cells.Real.ComboBox = Nothing
            Dim lfbase As New cLayerFactoryInternal()
            Dim dtParents As New Dictionary(Of String, cEwEHierarchyGridCell)
            Dim strGroup As String = ""
            Dim iRow As Integer = 0
            Dim vizChild As New cVisualizerEwEChildRowHeader()
            Dim cellParent As cEwEHierarchyGridCell = Nothing
            Dim checkParent As cEwECheckboxCell = Nothing

            For iLayer As Integer = 0 To Me.m_aLayers.Length - 1

                layer = Me.m_aLayers(iLayer)

                strGroup = lfbase.GetLayerGroup(layer.VarName)
                If Not dtParents.ContainsKey(strGroup) Then
                    iRow = Me.AddRow()
                    For j As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, j) = New cEwERowHeaderCell() : Next

                    ' Hierarchy cell
                    cellParent = New cEwEHierarchyGridCell()
                    dtParents(strGroup) = cellParent
                    Me(iRow, eColumnTypes.ColumnParent) = cellParent

                    ' Layer group name
                    Me(iRow, eColumnTypes.ColumnLayer) = New cEwERowHeaderCell(strGroup)
                    Me(iRow, eColumnTypes.ColumnField) = New cEwERowHeaderCell("")

                    ' New row for data
                    iRow = Me.AddRow()

                Else
                    cellParent = dtParents(strGroup)
                    iRow = Me.AddRow(cellParent.Row + cellParent.NumChildRows + 1)
                End If

                If (layer.Index <= 0) Then
                    cell = New cEwERowHeaderCell("")
                Else
                    cell = New cEwERowHeaderCell(CStr(layer.Index))
                End If
                Me(iRow, eColumnTypes.ColumnParent) = cell

                cell = New cEwERowHeaderCell(layer.Name)
                cell.VisualModel = New cVisualizerEwEChildRowHeader
                Me(iRow, eColumnTypes.ColumnLayer) = cell

                cmb = New Cells.Real.ComboBox(SharedResources.GENERIC_VALUE_NONE, GetType(String), Me.m_astrFields, True)
                cmb.EditableMode = EditableMode.SingleClick
                Me(iRow, eColumnTypes.ColumnField) = cmb
                Me(iRow, eColumnTypes.ColumnField).Behaviors.Add(Me.EwEEditHandler)

                Me.Rows(iRow).Tag = layer

            Next iLayer

            Me.UpdateMappingsColumn()

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Columns(eColumnTypes.ColumnParent).Width = 20
            Me.Columns(eColumnTypes.ColumnParent).AutoSizeMode = SourceGrid2.AutoSizeMode.None
            Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.ColumnField).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
        End Sub

        Protected Overrides Function OnCellEdited(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

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

                If (layer IsNot Nothing) Then
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
                End If

            Next iRow

            Try
                RaiseEvent MappingChanged()
            Catch ex As Exception

            End Try
        End Sub

        Private Function LayerAtRow(iRow As Integer) As cEcospaceLayer
            If iRow > 0 And iRow < Me.RowsCount Then
                Return DirectCast(Me.Rows(iRow).Tag, cEcospaceLayer)
            End If
            Return Nothing
        End Function

        Private Function FieldAtRow(iRow As Integer) As String
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
            If (Me.m_aLayers Is Nothing) Then Return False
            Return (Me.m_aLayers.Count > 0)
        End Function

#End Region ' Overrides

    End Class

End Namespace
