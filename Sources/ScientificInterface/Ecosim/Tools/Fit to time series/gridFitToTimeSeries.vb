'==============================================================================
'
' $Log: gridFitToTimeSeries.vb,v $
' Revision 1.2  2008/12/15 15:55:35  jeroens
' no message
'
' Revision 1.1  2008/11/19 14:40:55  jeroens
' Moved and renamed
'
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/09/23 16:15:52  jeroens
' Adding Villy's mortality penalty
'
' Revision 1.11  2008/08/11 16:13:59  jeroens
' Generalized EndEditHandler
'
' Revision 1.10  2008/06/02 00:01:39  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.9  2007/11/13 23:46:22  jeroens
' * Fixed apply structure
'
' Revision 1.8  2007/10/29 15:30:23  jeroens
' * Apply weights calls core OnChanged
'
' Revision 1.7  2007/10/27 03:14:47  jeroens
' + Added Apply
'
' Revision 1.6  2007/10/25 01:58:43  joeb
' Only load use time series data
'
' Revision 1.5  2007/10/23 03:13:18  jeroens
' * Fixed "Editor not attached to the grid" crash
'
'==============================================================================

#Region " Imports "
Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2
#End Region

<CLSCompliant(False)> _
Public Class gridFitToTimeSeries
    : Inherits EwEGrid

    Private Enum eColumnTypes
        TimeSeriesName = 0
        TimeSeriesWeight = 1
    End Enum

    Private m_core As cCore = Nothing
    Private m_bm As New EndEditHandler(Me)

    Public Sub New()
        m_core = cCore.GetInstance
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 2)

        ' ToDo_JS: globalize this
        Me(0, eColumnTypes.TimeSeriesName) = New EwEColumnHeaderCell("Time series")
        Me(0, eColumnTypes.TimeSeriesWeight) = New EwEColumnHeaderCell("Weight")

        Me.AutoStretchColumnsToFitWidth = True
        ' Fixed "Editor not attached to the grid" crash: cannot edit a fixed column, duh!
        Me.FixedColumns = 1
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

        For i As Integer = 1 To m_core.nTimeSeries
            ts = m_core.EcosimTimeSeries(i)

            'only load time series types that are used for the fitting
            'EwE5 logic comes from frmSearch.SetupSpread or cEcosim.AccumulateDataInfo
            If ts.Enabled And ts.TimeSeriesType <> eTimeSeriesType.TimeForcing And _
                        ts.TimeSeriesType <> eTimeSeriesType.FishingEffort And ts.TimeSeriesType <> eTimeSeriesType.FishingMortality Then
                Me.AddRow()

                ' Connect to TS
                ewec = New EwERowHeaderCell(ts.Name)
                ewec.Tag = ts
                Me(iRow, eColumnTypes.TimeSeriesName) = ewec

                ewec = New EwECell(0, GetType(Single))
                ewec.Value = ts.WtType
                ewec.Behaviors.Add(Me.m_bm)
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
            Me.m_core.onChanged(ts)
        End If

        Return True

    End Function

#Region " Events "

#End Region ' Events

End Class
