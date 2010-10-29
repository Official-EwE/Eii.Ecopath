#Region " Imports "

Option Strict On
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

<CLSCompliant(False)> _
Public Class gridWeightTS
    Inherits EwEGrid

    Private Enum eColumnTypes As Integer
        Name = 0
        Enabled
        Weight
    End Enum

    Public Sub New()
        MyBase.New()
        Me.FixedColumnWidths = False
    End Sub

    Public Sub ResetData()

        Dim cnt As Integer = Me.RowsCount
        If cnt > 1 Then
            Me.Rows.RemoveRange(1, cnt - 1)
        End If
        Me.FillData()

    End Sub

    Public Sub CheckAll(ByVal bCheck As Boolean)
        Dim cbc As SourceGrid2.Cells.Real.CheckBox = Nothing
        For iRow As Integer = 1 To Me.RowsCount - 1
            cbc = DirectCast(Me(iRow, CInt(eColumnTypes.Enabled)), SourceGrid2.Cells.Real.CheckBox)
            cbc.Checked = bCheck
        Next
    End Sub

    Public Function Apply(Optional ByVal bEnableAll As Boolean = False) As Boolean
        Dim cbc As SourceGrid2.Cells.Real.CheckBox = Nothing
        Dim ts As cTimeSeries = Nothing
        For iRow As Integer = 1 To Me.RowsCount - 1
            cbc = DirectCast(Me(iRow, CInt(eColumnTypes.Enabled)), SourceGrid2.Cells.Real.CheckBox)
            ts = DirectCast(cbc.Tag, cTimeSeries)
            ' Enabled flag
            ts.Enabled = bEnableAll Or cbc.Checked
            ' Weight
            ts.WtType = CSng(Me(iRow, CInt(eColumnTypes.Weight)).Value)
        Next
        Me.UIContext.Core.UpdateTimeSeries(True)
    End Function

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.FixedColumns = 1

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
        Me(0, eColumnTypes.Enabled) = New EwEColumnHeaderCell(SharedResources.HEADER_ENABLE)
        Me(0, eColumnTypes.Weight) = New EwEColumnHeaderCell(SharedResources.HEADER_WEIGHT)

    End Sub

    Protected Overrides Sub FillData()
        Dim ds As cTimeSeriesDataset = Nothing
        Dim ts As cTimeSeries = Nothing
        Dim strTarget As String = ""

        If (Me.UIContext Is Nothing) Then Return

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

        For iDS As Integer = 1 To Me.UIContext.Core.nTimeSeriesDatasets

            ' Get dataset
            ds = Me.UIContext.Core.TimeSeriesDataset(iDS)
            ' Is this dataset loaded?
            If ds.IsLoaded() Then

                ' #Yes: For all timeseries in the dataset
                For iTS As Integer = 0 To ds.Count - 1

                    ts = ds.Item(iTS)

                    ' Determine if this TS is ready to be applied
                    If ts.CanEnable() Then

                        ' #Yes: incorporate it in the dialog so it can be applied
                        If TypeOf ts Is cGroupTimeSeries Then
                            strTarget = Me.UIContext.Core.EcoPathGroupInputs(DirectCast(ts, cGroupTimeSeries).GroupIndex).Name
                        End If

                        If TypeOf ts Is cFleetTimeSeries Then
                            strTarget = Me.UIContext.Core.FleetInputs(DirectCast(ts, cFleetTimeSeries).FleetIndex).Name
                        End If

                        ' #Yes: create new ts item
                        Me.AddTimeSeriesRow(ts)

                    End If

                Next iTS

            End If
        Next

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

    End Sub

    Public Sub AddTimeSeriesRow(ByVal ts As cTimeSeries)

        Dim iRow As Integer = Me.AddRow()
        Dim cell As SourceGrid2.Cells.ICell = Nothing

        cell = New SourceGrid2.Cells.Real.CheckBox(ts.Enabled)
        cell.Tag = ts
        Me(iRow, CInt(eColumnTypes.Enabled)) = cell

        cell = New EwERowHeaderCell(ts.Name)
        Me(iRow, CInt(eColumnTypes.Name)) = cell

        cell = New EwECell(ts.WtType, GetType(Single))
        Me(iRow, CInt(eColumnTypes.Weight)) = cell

    End Sub

End Class
