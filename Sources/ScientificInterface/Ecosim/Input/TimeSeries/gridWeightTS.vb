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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
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
        [Type]
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
        Me(0, eColumnTypes.Type) = New EwEColumnHeaderCell(SharedResources.HEADER_TYPE)
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
        Dim bCanEnable As Boolean = (ts.ValidationStatus = eStatusFlags.OK)
        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim fmt As New cTimeSeriesTypeFormatter()

        If Not bCanEnable Then style = cStyleGuide.eStyleFlags.NotEditable

        cell = New EwERowHeaderCell(ts.Name)
        Me(iRow, CInt(eColumnTypes.Name)) = cell

        cell = New EwERowHeaderCell(fmt.GetDescriptor(ts.TimeSeriesType))
        Me(iRow, CInt(eColumnTypes.Type)) = cell

        cell = New SourceGrid2.Cells.Real.CheckBox(ts.Enabled)
        cell.Tag = ts
        cell.DataModel.EnableEdit = bCanEnable
        Me(iRow, CInt(eColumnTypes.Enabled)) = cell

        ' #1079: only allow weight for reference series
        If ts.IsReference Then style = cStyleGuide.eStyleFlags.OK Else style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
        cell = New EwECell(ts.WtType, GetType(Single), style)
        Me(iRow, CInt(eColumnTypes.Weight)) = cell

    End Sub

End Class
