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
Imports EwECore.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Core
Imports SourceGrid2
Imports ScientificInterface.Ecospace.Controls

#End Region ' Imports

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridExternalSpatialData
        Inherits EwEGrid

#Region " Private classes "

        Private Class cConnectionInfo

            Public Sub New(adt As cSpatialDataAdapter, layer As cEcospaceLayer)
                Me.Adapter = adt
                Me.Layer = layer
            End Sub

            Public Property Adapter As cSpatialDataAdapter = Nothing
            Public Property Layer As cEcospaceLayer

        End Class

#End Region ' Private classes

        Private m_man As cSpatialDataConnectionManager
        Private m_manSets As cSpatialDataSetManager
        Private m_filter As eVarNameFlags = eVarNameFlags.NotSet
        Private m_nBaseCols As Integer = 0
        Private m_bmCell As BehaviorModels.CustomEvents = Nothing

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            Enabled
        End Enum

        Public Sub New()
            Me.m_nBaseCols = [Enum].GetValues(GetType(eColumnTypes)).Length
            Me.m_bmCell = New BehaviorModels.CustomEvents()
        End Sub

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)

                If (Me.UIContext IsNot Nothing) Then
                    Me.m_man = Nothing
                    Me.m_manSets = Nothing
                    RemoveHandler m_bmCell.Click, AddressOf CellClick
                End If
                ' Peek ahead...
                If (value IsNot Nothing) Then
                    Me.m_man = value.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
                    AddHandler m_bmCell.Click, AddressOf CellClick
                End If
                MyBase.UIContext = value
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, Me.m_nBaseCols + cSpatialDataStructures.cMAX_CONN)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
            Me(0, eColumnTypes.Enabled) = New EwEColumnHeaderCell(SharedResources.HEADER_ENABLED)

            For i As Integer = 1 To cSpatialDataStructures.cMAX_CONN
                Me(0, Me.m_nBaseCols + i - 1) = New EwEColumnHeaderCell(CStr(i))
            Next

            Me.FixedColumnWidths = False
            Me.FixedColumns = Me.m_nBaseCols

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters()
            Dim fmt As New cSpatialDataAdapterFormatter()
            Dim fact As New cLayerFactoryInternal()
            Dim strAdapter As String = ""
            Dim layer As cEcospaceLayer = Nothing
            Dim layers() As cEcospaceLayer = Nothing
            Dim hgcGroup As EwEHierarchyGridCell = Nothing
            Dim iRow As Integer = 0

            Dim vizParent As New cVisualizerEwEParentRowHeader()
            Dim vizChild As New cVisualizerEwEChildRowHeader()

            Me.RowsCount = 1

            For Each adt As cSpatialDataAdapter In Me.m_man.Adapters

                If (adt.VarName = Me.m_filter) Or (Me.m_filter = eVarNameFlags.NotSet) Then

                    ' Get group name for the adapter
                    strAdapter = fmt.GetDescriptor(adt)
                    ' Get layers for the adapter
                    layers = bm.Layers(adt.VarName)

                    ' Header row
                    iRow = Me.AddRow()
                    hgcGroup = New EwEHierarchyGridCell()
                    Me(iRow, eColumnTypes.Index) = hgcGroup
                    Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(strAdapter)
                    Me(iRow, eColumnTypes.Name).VisualModel = vizParent
                    For i As Integer = 2 To Me.ColumnsCount - 1
                        Me(iRow, i) = New EwEColumnHeaderCell("")
                    Next

                    ' All layers
                    For i As Integer = 0 To layers.Count - 1
                        iRow = Me.AddRow()
                        layer = layers(i)
                        Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(layer.Index))
                        Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(layer.Name)
                        Me(iRow, eColumnTypes.Name).VisualModel = vizChild

                        Me(iRow, eColumnTypes.Enabled) = New SourceGrid2.Cells.Real.CheckBox(adt.IsEnabled(layer.Index))
                        Me(iRow, eColumnTypes.Enabled).Behaviors.Add(Me.EwEEditHandler)

                        For j As Integer = Me.m_nBaseCols To Me.ColumnsCount - 1
                            Me(iRow, j) = New Cells.Real.Cell("")
                            Me(iRow, j).Behaviors.Add(Me.m_bmCell)
                            Me(iRow, j).Tag = (j - Me.m_nBaseCols + 1)
                            Me(iRow, j).VisualModel = New VisualModels.Common()
                        Next
                        Me.ConnectionAtRow(iRow) = New cConnectionInfo(adt, layer)
                        hgcGroup.AddChildRow(iRow)

                        Me.UpdateDatasetRow(iRow)
                    Next
                End If

            Next
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.AllowBlockSelect = False
        End Sub

        Protected Sub UpdateDatasetRow(iRow As Integer)

            Dim conn As cConnectionInfo = Me.ConnectionAtRow(iRow)
            If (conn Is Nothing) Then Return

            Dim iNumDefined As Integer = 0
            Dim iNumConnected As Integer = 0

            For j As Integer = Me.m_nBaseCols To Me.ColumnsCount - 1

                Dim adt As cSpatialDataAdapter = conn.Adapter
                Dim layer As cEcospaceLayer = conn.Layer
                Dim iConn As Integer = (j - Me.m_nBaseCols + 1)
                Dim strText As String = ""
                Dim status As cStyleGuide.eApplicationColorType = cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND

                If (adt.Dataset(layer.Index, iConn) IsNot Nothing) Then
                    strText = adt.Dataset(layer.Index, iConn).DisplayName
                    If (Not conn.Adapter.IsConnected(conn.Layer.Index, iConn)) Then
                        status = cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND
                    End If
                End If

                Me(iRow, j).VisualModel.BackColor = Me.StyleGuide.ApplicationColor(status)
                Me(iRow, j).Value = strText
            Next

        End Sub

        Protected Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)
            Try
                Dim iRow As Integer = e.Position.Row
                Dim conn As cConnectionInfo = Me.ConnectionAtRow(iRow)
                If (conn Is Nothing) Then Return
                Dim dlg As New dlgApplyConnection(Me.UIContext, conn.Adapter, conn.Layer)
                If dlg.ShowDialog() = DialogResult.OK Then
                    Me.m_man.Invalidate()
                End If
                Me.UpdateDatasetRow(iRow)
            Catch ex As Exception
                ' Whoah
            End Try
        End Sub

        Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean
            If p.Column = eColumnTypes.Enabled Then
                Dim iRow As Integer = p.Row
                Dim conn As cConnectionInfo = Me.ConnectionAtRow(iRow)
                conn.Adapter.IsEnabled(conn.Layer.Index) = CBool(cell.GetValue(p))
            End If
        End Function

        Public Property Filter As eVarNameFlags
            Get
                Return Me.m_filter
            End Get
            Set(value As eVarNameFlags)
                If (value = Me.m_filter) Then Return
                Me.m_filter = value
                Me.RefreshContent()
            End Set
        End Property

        Private Property ConnectionAtRow(iRow As Integer) As cConnectionInfo
            Get
                Return DirectCast(Me(iRow, 0).Tag, cConnectionInfo)
            End Get
            Set(value As cConnectionInfo)
                Me(iRow, 0).Tag = value
            End Set
        End Property

    End Class

End Namespace
