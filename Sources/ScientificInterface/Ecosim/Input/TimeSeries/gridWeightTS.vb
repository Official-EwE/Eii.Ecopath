'==============================================================================
'
' $Log: gridWeightTS.vb,v $
' Revision 1.1  2008/09/26 07:31:45  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/23 16:07:54  jeroens
' Renamed
'
' Revision 1.6  2008/07/29 13:06:46  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.5  2008/07/01 19:13:10  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.4  2008/07/01 14:16:12  jeroens
' IsStatic works properly
'
' Revision 1.3  2008/06/02 00:01:43  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.2  2008/05/13 02:06:11  jeroens
' Woops! Fixed TS apply crash
'
' Revision 1.1  2008/05/11 04:05:48  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore

#End Region ' Imports directive

<CLSCompliant(False)> _
Public Class gridWeightTS
    Inherits EwEGrid

    Private Enum eColumnTypes As Integer
        Name = 0
        Enabled
        Weight
    End Enum

    Private m_core As cCore = Nothing

    Public Sub New()
        MyBase.New()
        Me.m_core = cCore.GetInstance()
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
        Me.m_core.UpdateTimeSeries()
    End Function

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Dock = DockStyle.Fill
        Me.FixedColumns = 1

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
        Me(0, eColumnTypes.Enabled) = New EwEColumnHeaderCell(My.Resources.HEADER_ENABLE)
        Me(0, eColumnTypes.Weight) = New EwEColumnHeaderCell(My.Resources.HEADER_WEIGHT)

    End Sub

    Protected Overrides Sub FillData()
        Dim ds As cTimeSeriesDataset = Nothing
        Dim ts As cTimeSeries = Nothing
        'Dim tsItem As TimeSeriesItem = Nothing
        Dim strTarget As String = ""
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        appl.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

        For iDS As Integer = 1 To Me.m_core.nTimeSeriesDatasets

            ' Get dataset
            ds = Me.m_core.TimeSeriesDataset(iDS)
            ' Is this dataset loaded?
            If ds.IsLoaded() Then

                ' #Yes: For all timeseries in the dataset
                For iTS As Integer = 0 To ds.Count - 1

                    ts = ds.Item(iTS)

                    ' Determine if this TS is ready to be applied
                    If ts.CanEnable() Then

                        ' #Yes: incorporate it in the dialog so it can be applied
                        If TypeOf ts Is cGroupTimeSeries Then
                            strTarget = Me.m_core.EcoPathGroupInputs(DirectCast(ts, cGroupTimeSeries).GroupIndex).Name
                        End If

                        If TypeOf ts Is cFleetTimeSeries Then
                            strTarget = Me.m_core.FleetInputs(DirectCast(ts, cFleetTimeSeries).FleetIndex).Name
                        End If

                        ' #Yes: create new ts item
                        Me.AddTimeSeriesRow(ts)

                    End If

                Next iTS

            End If
        Next

        appl.SetStatusText("", TriState.False)

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
