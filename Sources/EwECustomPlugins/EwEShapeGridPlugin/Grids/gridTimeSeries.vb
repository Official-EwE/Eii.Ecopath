' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid displaying <see cref="cTimeSeries"/>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridTimeSeries
    Inherits gridShapeBase

    ''' <summary>Rows in the grid</summary>
    Private Enum eRowType As Integer
        Header = 0
        Thumbnail
        Name
        Type
        Usage
        Scaling
        Weight
        PoolPrimary
        PoolSecundary
        Interval
        FirstTime
    End Enum

    ''' <summary>Time series UI display handler thingy</summary>
    Private m_handler As cTimeSeriesShapeGUIHandler = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Grid overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub FillData()

        If Me.UIContext Is Nothing Then Return

        cApplicationStatusNotifier.StartProgress(Me.UIContext.Core, SharedResources.STATUS_UPDATING)

        Dim nPoints As Integer = Me.Core.nTimeSeriesYears
        Dim ats As cShapeData() = Me.Shapes
        Dim nTS As Integer = ats.Length
        Dim ts As cTimeSeries = Nothing
        Dim cell As SourceGrid2.Cells.ICell = Nothing

        Dim selDatTypePrim As cCoreInputOutputBase = Nothing
        Dim selDatTypeSec As cCoreInputOutputBase = Nothing
        Dim fmtInterval As New cTimeSeriesDatasetIntervalTypeFormatter()
        Dim fmtCore As New cCoreInterfaceFormatter()
        Dim fmtTSType As New cTimeSeriesTypeFormatter()

        Dim styleEditable As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim styleReadOnly As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.NotEditable

        Array.Sort(Shapes, New cShapeDataComparer())

        Me.Redim(nPoints + [Enum].GetValues(GetType(eRowType)).Length, nTS + 1)

        ' Create row headers
        Me(eRowType.Header, 0) = New cEwEColumnHeaderCell(SharedResources.HEADER_INDEX)
        Me(eRowType.Thumbnail, 0) = New cEwERowHeaderCell(SharedResources.HEADER_IMAGE)
        Me(eRowType.Name, 0) = New cEwERowHeaderCell(SharedResources.HEADER_NAME)
        Me(eRowType.PoolPrimary, 0) = New cEwERowHeaderCell(SharedResources.HEADER_TARGET)
        Me(eRowType.PoolSecundary, 0) = New cEwERowHeaderCell(SharedResources.HEADER_TARGET_SECOND)
        Me(eRowType.Type, 0) = New cEwERowHeaderCell(SharedResources.HEADER_TYPE)
        Me(eRowType.Scaling, 0) = New cEwERowHeaderCell(SharedResources.HEADER_SCALING)
        Me(eRowType.Usage, 0) = New cEwERowHeaderCell(SharedResources.HEADER_USAGE)
        Me(eRowType.Weight, 0) = New cEwERowHeaderCell(SharedResources.HEADER_WEIGHT)
        Me(eRowType.Interval, 0) = New cEwERowHeaderCell(My.Resources.HEADER_INTERVAL)
        Me(eRowType.Type, 0) = New cEwERowHeaderCell(SharedResources.HEADER_TYPE)

        For i As Integer = 0 To nPoints - 1
            Me(eRowType.FirstTime + i, 0) = New cEwERowHeaderCell(Me.Label(i))
        Next

        For i As Integer = 0 To nTS - 1
            ts = DirectCast(ats(i), cTimeSeries)

            Me.Shape(i + 1) = ts

            Me(eRowType.Header, i + 1) = New cEwEColumnHeaderCell(CStr(ts.Index))

            cell = New cEwECell(ts, cStyleGuide.eStyleFlags.NotEditable)
            cell.VisualModel = New cVisualModelThumbnail(Me.Handler)
            Me(eRowType.Thumbnail, i + 1) = cell

            cell = New cEwECell(ts.Name, GetType(String))
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(eRowType.Name, i + 1) = cell

            selDatTypePrim = Nothing
            If (TypeOf ts Is cGroupTimeSeries) Then
                Dim gts As cGroupTimeSeries = DirectCast(ts, cGroupTimeSeries)
                If (gts.GroupIndex >= 1) Then
                    selDatTypePrim = Me.Core.EcopathGroupInputs(gts.GroupIndex)
                End If
            Else
                Dim fts As cFleetTimeSeries = DirectCast(ts, cFleetTimeSeries)
                If (fts.FleetIndex >= 1) Then
                    selDatTypePrim = Me.Core.EcopathFleetInputs(fts.FleetIndex)
                End If
                If (cTimeSeries.Category(ts.TimeSeriesType) = eTimeSeriesCategoryType.FleetGroup) Then
                    If (fts.GroupIndex >= 1) Then
                        selDatTypeSec = Me.Core.EcopathGroupInputs(fts.GroupIndex)
                    End If
                End If
            End If

            If (selDatTypePrim IsNot Nothing) Then
                Me(eRowType.PoolPrimary, i + 1) = New cEwECell(fmtCore.ToString(selDatTypePrim), cStyleGuide.eStyleFlags.NotEditable)
            Else
                Me(eRowType.PoolPrimary, i + 1) = New cEwECell("", cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable)
            End If

            If (selDatTypeSec IsNot Nothing) Then
                Me(eRowType.PoolSecundary, i + 1) = New cEwECell(fmtCore.ToString(selDatTypeSec), cStyleGuide.eStyleFlags.NotEditable)
            Else
                Me(eRowType.PoolSecundary, i + 1) = New cEwECell("", cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable)
            End If

            Me(eRowType.Type, i + 1) = New cEwECell(fmtTSType.ToString(ts.TimeSeriesType), cStyleGuide.eStyleFlags.NotEditable)

            Me(eRowType.Scaling, i + 1) = New cEwECell(If(ts.IsRelative, SharedResources.VALUE_GENERIC_RELATIVE, SharedResources.VALUE_GENERIC_ABSOLUTE), cStyleGuide.eStyleFlags.NotEditable)
            Me(eRowType.Usage, i + 1) = New cEwECell(If(ts.IsReference, SharedResources.VALUE_GENERIC_REFERENCE, SharedResources.VALUE_GENERIC_FORCING), cStyleGuide.eStyleFlags.NotEditable)

            cell = New cEwECell(fmtInterval.ToString(ts.Interval), GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(eRowType.Interval, i + 1) = cell

            cell = New cEwECell(ts.WtType, GetType(Single))
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(eRowType.Weight, i + 1) = cell

            For j As Integer = 0 To nPoints - 1
                cell = New cEwECell(ts.ShapeData(j + 1), GetType(Single), cStyleGuide.eStyleFlags.NotEditable)
                DirectCast(cell, cEwECell).SuppressZero = Not ts.SupportsNull()
                Me(eRowType.FirstTime + j, i + 1) = cell
            Next
        Next

        cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Rows(eRowType.Thumbnail).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
        Me.Rows(eRowType.Thumbnail).Height = 48
        For i As Integer = 1 To Me.ColumnsCount - 1
            Me.Columns(i).Width = Math.Max(Me.Columns(i).Width, 48)
        Next
        ' Fix all descriptive rows
        Me.FixedRows = 2
        ' Fix header column
        Me.FixedColumns = 1
    End Sub

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            If (Me.m_handler Is Nothing) Then
                Me.m_handler = New cTimeSeriesShapeGUIHandler(Me.UIContext)
            End If
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As IEnumerable
        Get
            If Me.Core.ActiveTimeSeriesDatasetIndex <= 0 Then Return Nothing
            Dim lts As New List(Of cTimeSeries)
            lts.AddRange(Me.Core.EcosimGroupTimeseries)
            lts.AddRange(Me.Core.EcosimFleetTimeseries)
            Return lts.ToArray
        End Get
    End Property

#End Region ' Grid overrides

#Region " Edits "

    Dim m_bInEdit As Boolean = False

    Protected Overrides Function OnCellEdited(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        Dim ts As cTimeSeries = DirectCast(Me.Shape(p.Column), cTimeSeries)
        Select Case DirectCast(p.Row, eRowType)
            Case eRowType.Name
                ts.Name = CStr(cell.GetValue(p))
            Case eRowType.PoolPrimary, eRowType.PoolSecundary
                Debug.Assert(False)
                'ts.DatPool = DirectCast(cell.GetValue(p), cCoreInputOutputBase).Index
            Case eRowType.Type
                Debug.Assert(False)
                'ts.TimeSeriesType = DirectCast(cell.GetValue(p), eTimeSeriesType)
            Case eRowType.Weight
                ts.WtType = CSng(cell.GetValue(p))
            Case Else
                ' Don't allow
                'Dim iTime As Integer = p.Row - eRowType.FirstTime
                'ts.ShapeData(iTime + 1) = CSng(cell.GetValue(p))
        End Select

        ' Do not invalidate individual shapes on a batch cell edit
        If Me.IsInBatchEdit Then
            Me.InvalidateShape(p.Column)
        Else
            ' Stop local cell edits from refreshing the content of the entire grid (see OnRefreshed)
            Me.m_bInEdit = True
            ts.Update()
            Me.m_bInEdit = False
        End If

        Return MyBase.OnCellEdited(p, cell)
    End Function

    Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean
        Me.OnCellEdited(p, cell)
        Return MyBase.OnCellValueChanged(p, cell)
    End Function

#End Region ' Edits

#Region " Updates "

    Protected Overrides Sub OnRefreshed(sender As ScientificInterfaceShared.Controls.cShapeGUIHandler)
        ' Unpleasant: a refresh can be triggered from an external edit or by 
        ' this very interface in response to a cell edit. If a cell edit is in
        ' progress the grid content cannot be refreshed.

        ' In local cell edit?
        If Me.m_bInEdit Then
            ' #Yes: just invalidate the thumbnail
            Me.InvalidateRange(New SourceGrid2.Range(eRowType.Thumbnail, 0, eRowType.Thumbnail, Me.ColumnsCount - 1))
        Else
            ' #No: refresh the whole lot
            Me.RefreshContent()
        End If
    End Sub

#End Region ' Updates

#Region " Helper methods "

    Protected Overrides Function Label(iPoint As Integer) As String
        Dim ds As cTimeSeriesDataset = Nothing
        If (Me.Core.ActiveTimeSeriesDatasetIndex = -1) Then Return "?"
        ds = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)
        Select Case ds.TimeSeriesInterval
            Case eTSDataSetInterval.Annual
                Return CStr(iPoint + Me.Core.EcosimFirstYear)
            Case eTSDataSetInterval.TimeStep
                Dim iMonth As Integer = (iPoint Mod 12) + 1
                If Not Me.IsSeasonal And (iMonth = 1) Then
                    Return CStr(Math.Floor(iPoint / cCore.N_MONTHS) + Me.Core.EcosimFirstYear)
                End If
                Return cDateUtils.GetMonthName(iMonth)
            Case Else
                Debug.Assert(False)
        End Select
        Return "?"
    End Function

#End Region ' Helper methods

End Class
