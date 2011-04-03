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

    Public Sub CheckAll(ByVal bCheck As Boolean)
        Dim cbc As SourceGrid2.Cells.Real.CheckBox = Nothing
        For iRow As Integer = 1 To Me.RowsCount - 1
            cbc = DirectCast(Me(iRow, CInt(eColumnTypes.Enabled)), SourceGrid2.Cells.Real.CheckBox)
            cbc.Checked = bCheck
        Next
    End Sub

    Public Function Apply(Optional ByVal bEnableAll As Boolean = False) As Boolean

        ' Make sure this method is executed only when allowed
        If (Me.Core.ActiveTimeSeriesDatasetIndex <= 0) Then Return True

        Try
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
        Catch ex As Exception
            Return False
        End Try
        Return True
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

        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_PLEASE_WAIT)

        For iDS As Integer = 1 To Me.UIContext.Core.nTimeSeriesDatasets

            ' Get dataset
            ds = Me.UIContext.Core.TimeSeriesDataset(iDS)
            ' Is this dataset loaded?
            If ds.IsLoaded() Then

                ' #Yes: For all timeseries in the dataset
                For iTS As Integer = 0 To ds.Count - 1
                    ' Get TS
                    ts = ds.Item(iTS)
                    ' #Yes: create new ts item
                    Me.AddTimeSeriesRow(ts)
                Next iTS
            End If
        Next

        cApplicationStatusNotifier.EndProgress(Me.Core)

    End Sub

    Public Sub AddTimeSeriesRow(ByVal ts As cTimeSeries)

        Dim iRow As Integer = Me.AddRow()
        Dim cell As SourceGrid2.Cells.ICell = Nothing
        Dim bCanEnable As Boolean = ts.CanEnable
        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        If Not bCanEnable Then style = cStyleGuide.eStyleFlags.NotEditable

        cell = New SourceGrid2.Cells.Real.CheckBox(ts.Enabled)
        cell.Tag = ts
        cell.DataModel.EnableEdit = bCanEnable
        Me(iRow, CInt(eColumnTypes.Enabled)) = cell

        cell = New EwERowHeaderCell(ts.Name)
        Me(iRow, CInt(eColumnTypes.Name)) = cell

        cell = New EwECell(ts.WtType, GetType(Single), style)
        Me(iRow, CInt(eColumnTypes.Weight)) = cell

    End Sub

End Class
