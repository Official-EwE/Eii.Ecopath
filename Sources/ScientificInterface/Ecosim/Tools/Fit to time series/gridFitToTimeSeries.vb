#Region " Imports "
Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region

<CLSCompliant(False)> _
Public Class gridFitToTimeSeries
    : Inherits EwEGrid

    Private Enum eColumnTypes
        TimeSeriesName = 0
        TimeSeriesWeight = 1
    End Enum

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 2)

        Me(0, eColumnTypes.TimeSeriesName) = New EwEColumnHeaderCell(SharedResources.HEADER_TIMESERIES)
        Me(0, eColumnTypes.TimeSeriesWeight) = New EwEColumnHeaderCell(SharedResources.HEADER_WEIGHT)

        ' Fixed "Editor not attached to the grid" crash: cannot edit a fixed column, duh!
        Me.FixedColumns = 1
        Me.FixedColumnWidths = False
    End Sub

    Protected Overrides Sub FillData()
        Me.RefreshGrid()
    End Sub

    Public Sub RefreshGrid()

        Dim ts As cTimeSeries = Nothing
        Dim iRow As Integer = 0
        Dim ewec As EwECellBase = Nothing

        ' Remove existing rows
        Me.RowsCount = 1

        ' Populate rows for applied TS
        iRow = Me.RowsCount

        For i As Integer = 1 To Me.Core.nTimeSeries
            ts = Me.Core.EcosimTimeSeries(i)

            'only load time series types that are used for the fitting
            'EwE5 logic comes from frmSearch.SetupSpread or cEcosim.AccumulateDataInfo
            If (ts.Enabled = True) And _
               (ts.TimeSeriesType <> eTimeSeriesType.TimeForcing) And _
               (ts.TimeSeriesType <> eTimeSeriesType.FishingEffort) And _
               (ts.TimeSeriesType <> eTimeSeriesType.FishingMortality) Then
                Me.AddRow()

                ' Connect to TS
                ewec = New EwERowHeaderCell(ts.Name)
                ewec.Tag = ts
                Me(iRow, eColumnTypes.TimeSeriesName) = ewec

                ewec = New EwECell(0, GetType(Single))
                ewec.Value = ts.WtType
                ewec.Behaviors.Add(Me.EwEEditHandler)
                Me(iRow, eColumnTypes.TimeSeriesWeight) = ewec

                iRow += 1
            End If
        Next
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Updates the weights in the TS grid and applies the TS.
    ''' </summary>
    ''' <returns>True if TS weights applied succesfully.</returns>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellEdited(ByVal pos As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Dim ts As cTimeSeries = Nothing
        Dim objTest As Object = Nothing

        If (pos.Column <> eColumnTypes.TimeSeriesWeight) Then Return False

        objTest = Me(pos.Row, eColumnTypes.TimeSeriesName).Tag
        If (TypeOf objTest Is cTimeSeries) Then
            ts = DirectCast(objTest, cTimeSeries)
            ts.WtType = CSng(cell.GetValue(pos))
            Me.Core.onChanged(ts)
        End If

        Return True

    End Function

#Region " Events "

#End Region ' Events

End Class
