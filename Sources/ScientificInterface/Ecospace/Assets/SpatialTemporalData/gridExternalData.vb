#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.SpatialData
Imports SourceGrid2
Imports EwEPlugin
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridExternalData
        Inherits EwEGrid

        Private Class cSpatialAdapterSorter
            Implements IComparer(Of ISpatialDataAdapter)

            Private m_fmt As New cVarnameTypeFormatter()

            Public Sub New()
            End Sub

            Public Function Compare(ByVal x As EwEUtils.SpatialData.ISpatialDataAdapter, _
                                    ByVal y As EwEUtils.SpatialData.ISpatialDataAdapter) As Integer _
                                Implements System.Collections.Generic.IComparer(Of EwEUtils.SpatialData.ISpatialDataAdapter).Compare
                If (x Is Nothing) Then Return 1
                If (y Is Nothing) Then Return -1
                Return String.Compare(Me.m_fmt.GetDescriptor(x.VarName), Me.m_fmt.GetDescriptor(y.VarName))
            End Function

        End Class

        Private Enum eColumnTypes As Integer
            DataAdapter = 0
            DataSet
            Converter
            RelTime
        End Enum

        Private m_bAdvancedMode As Boolean = False
        Private m_bRelativeTime As Boolean = False

        Public Sub New()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.DataAdapter) = New EwEColumnHeaderCell("Ecospace variable")
            Me(0, eColumnTypes.DataSet) = New EwEColumnHeaderCell("Data set")
            Me(0, eColumnTypes.Converter) = New EwEColumnHeaderCell("Converter")
            Me(0, eColumnTypes.RelTime) = New EwEColumnHeaderCell("Relative time")

            Me.FixedColumnWidths = False
            Me.FixedColumns = 1

        End Sub

        Protected Overrides Sub FillData()

            Dim man As SpatialData.cSpatialDataConnectionManager = Nothing
            Dim adapters As ISpatialDataAdapter() = Nothing
            Dim lDatasets As New List(Of ISpatialDataSet)
            Dim lDataConverters As New List(Of ISpatialDataConverter)

            Dim fmtVar As New cVarnameTypeFormatter()
            Dim fmtSet As New cSpatialDatasetFormatter()
            Dim fmtCon As New cSpatialConverterFormatter()
            Dim iRow As Integer = 0

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            man = Me.Core.SpatialDataConnectionManager
            If (man Is Nothing) Then Return

            adapters = man.Adapters
            Array.Sort(adapters, New cSpatialAdapterSorter)

            ' Create list of datasets
            lDatasets.Add(Nothing)
            ' Add dataset templates
            For Each ip As IPlugin In Me.UIContext.Core.PluginManager.GetPlugins(GetType(ISpatialDataSet))
                lDatasets.Add(DirectCast(ip, ISpatialDataSet))
            Next
            lDatasets.AddRange(man.Datasets)
            Dim editorDataset As New EwEComboBoxCellEditor(fmtSet, lDatasets)

            lDataConverters.Add(Nothing)
            lDataConverters.AddRange(man.ConverterTemplates())
            Dim editorConverter As New EwEComboBoxCellEditor(fmtCon, lDataConverters)

            For Each adt As ISpatialDataAdapter In adapters

                iRow = Me.AddRow()

                Me(iRow, eColumnTypes.DataAdapter) = New EwERowHeaderCell(fmtVar.GetDescriptor(adt.VarName))
                Me(iRow, eColumnTypes.DataAdapter).Behaviors.Add(Me.EwEEditHandler)

                Me(iRow, eColumnTypes.DataSet) = New SourceGrid2.Cells.Real.Cell(adt.Dataset, editorDataset)
                Me(iRow, eColumnTypes.DataSet).Behaviors.Add(Me.EwEEditHandler)

                Me(iRow, eColumnTypes.Converter) = New SourceGrid2.Cells.Real.Cell(adt.Converter, editorConverter)
                Me(iRow, eColumnTypes.Converter).Behaviors.Add(Me.EwEEditHandler)

                Me(iRow, eColumnTypes.RelTime) = New EwECheckboxCell(adt.IsRelativeTime)

                Me.RowAdapter(iRow) = adt

                Me.UpdateDataRow(iRow)

            Next

        End Sub

        Private Sub UpdateDataRow(ByVal iRow As Integer)

            Dim adt As ISpatialDataAdapter = Me.RowAdapter(iRow)
            ' NOP

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.StretchColumnsToFitWidth()
        End Sub

        Protected Overrides Sub OnCellClicked(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual)
            MyBase.OnCellClicked(p, cell)
            ' ToDo: route this through command
            ''Dim cmd As cCommand = Me.CommandHandler.GetCommand("~configlayerdata")
            'Dim dlg As New dlgConfigLayerData(Me.UIContext, DirectCast(Me.Rows(p.Row).Tag, ISpatialDataAdapter))
            'dlg.ShowDialog()
        End Sub

        Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

            ' ToDo: determine how to deal with indexed layers

            Dim bSucces As Boolean = MyBase.OnCellValueChanged(p, cell)
            Dim adt As ISpatialDataAdapter = Me.RowAdapter(p.Row)
            Dim layer As cEcospaceLayer = Me.Core.EcospaceBasemap.Layer(adt.VarName)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.DataSet
                    adt.Dataset = DirectCast(cell.GetValue(p), ISpatialDataSet)
                    Me.UpdateDataRow(p.Row)

                    Me.Core.onChanged(layer)

                Case eColumnTypes.Converter
                    adt.Converter = DirectCast(cell.GetValue(p), ISpatialDataConverter)
                    Me.UpdateDataRow(p.Row)

                    Me.Core.onChanged(layer)

            End Select
            Return bSucces

        End Function

        Public Property RowAdapter(ByVal iRow As Integer) As ISpatialDataAdapter
            Get
                Return DirectCast(Me.Rows(iRow).Tag, ISpatialDataAdapter)
            End Get
            Set(ByVal adt As ISpatialDataAdapter)
                Me.Rows(iRow).Tag = adt
            End Set
        End Property

        Public Property UseRelativeTime As Boolean
            Get
                Return Me.m_bRelativeTime
            End Get
            Set(ByVal value As Boolean)
                Me.m_bRelativeTime = value
            End Set
        End Property

        Public Sub Apply()

            Dim adt As ISpatialDataAdapter = Nothing
            Dim dst As ISpatialDataSet = Nothing

            For iRow As Integer = 1 To Me.RowsCount - 1

                adt = Me.RowAdapter(iRow)
                dst = adt.Dataset

                If (dst IsNot Nothing) Then
                    adt.IsRelativeTime = Me.m_bRelativeTime
                End If
            Next iRow

        End Sub

    End Class

End Namespace
