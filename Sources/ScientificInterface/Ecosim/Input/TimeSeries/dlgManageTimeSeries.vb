'==============================================================================
'
' $Log: dlgManageTimeSeries.vb,v $
' Revision 1.8  2009/04/03 14:56:27  jeroens
' Fixed issue that prevented FF time series from showing up at end of import
'
' Revision 1.7  2009/03/23 17:30:45  jeroens
' Added sketchpad preview
'
' Revision 1.6  2009/02/26 21:41:31  jeroens
' Import TS batch lock now level TimeSeries
'
' Revision 1.5  2008/11/26 23:19:52  jeroens
' Weight! Weight, dude
'
' Revision 1.4  2008/11/10 01:56:19  jeroens
' Oops
'
' Revision 1.3  2008/11/10 01:50:53  jeroens
' Renamed resources
'
' Revision 1.2  2008/11/08 23:53:16  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:44  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/09/23 16:10:35  jeroens
' Renamed Apply to Enable
' Terminology changed to Weighting time series (applying means preciously little)
'
' Revision 1.4  2008/09/09 14:44:50  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.3  2008/08/02 03:04:14  jeroens
' Renamed resources
'
' Revision 1.2  2008/07/30 17:51:53  jeroens
' Disabled TS export
' Fixed issue 499
'
' Revision 1.1  2008/07/14 17:08:44  jeroens
' Initial version, merged load/apply + import dialogs
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Windows.Forms
Imports System.Globalization
Imports EwEUtils.Commands

#End Region ' Imports

Public Class dlgManageTimeSeries

#Region " Helper classes "

    Private Class TimeSeriesItem
        Inherits Object

        Private m_ts As cTimeSeries = Nothing

        Public Sub New(ByVal ts As cTimeSeries)
            Me.m_ts = ts
        End Sub

        Public ReadOnly Property TimeSeries() As cTimeSeries
            Get
                Return Me.m_ts
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.m_ts.Name
        End Function

    End Class

#End Region ' Helper classes

    Public Enum eModeType As Integer
        Load = 0
        Weight
        Import
        ' Export
        Delete
    End Enum

    Private m_core As cCore = cCore.GetInstance()
    Private m_mode As eModeType = eModeType.Load
    Private m_strDataset As String = ""
    Private m_bInitialized As Boolean = False

    Private m_gridEnableTS As gridWeightTS = Nothing

    Private m_tr As cTimeSeriesTextReader = New cTimeSeriesCSVReader(cCore.GetInstance())
    Private m_strImportFileName As String = String.Empty
    Private m_tsh As cTimeSeriesShapeGUIHandler = Nothing

    Public Sub New(ByVal mode As eModeType)

        Me.InitializeComponent()

        ' -- Enable --
        ' Prepare grid
        Me.m_gridEnableTS = New gridWeightTS()
        ' Show grid
        Me.m_plEnableTimeSeries.Controls.Add(Me.m_gridEnableTS)
        ' CFG
        Me.m_gridEnableTS.ResetData()

        ' -- IMPORT --
        'Me.m_strImportDecimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
        Me.m_tbImportAuthor.Text = Me.m_core.EwEModel.Author
        Me.m_tbImportContact.Text = Me.m_core.EwEModel.Contact
        Me.m_tsh = New cTimeSeriesShapeGUIHandler(Me.m_core, Nothing, Nothing, Me.m_spTimeSeriesPreview, Nothing)

        Me.FillImportDatasetCombo()
        Me.ReloadTimeSeries()

        ' Validate mode
        If mode = eModeType.Weight And Not Me.m_core.HasTimeSeries Then
            mode = eModeType.Load
        End If

        Me.m_bInitialized = True
        Me.Mode = mode

    End Sub

#Region " Events "

#Region " Generic "

    Private Sub dlgTimeSeries_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.UpdateLoadPage()
        Me.UpdateWeightsPage()
        Me.UpdateDeletePage()
        Me.UpdateControls()
        ' Switch to the appropriate page
        Me.Mode = Me.m_mode
    End Sub

    Private Sub OnTabSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tcMain.SelectedIndexChanged

        Select Case Me.m_tcMain.SelectedIndex
            Case 0
                Me.Mode = eModeType.Load
            Case 1
                Me.Mode = eModeType.Weight
            Case 2
                Me.Mode = eModeType.Import
            Case 3
                Me.Mode = eModeType.Delete
                'Case 4
                '    Me.Mode = eModeType.Export
        End Select
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOk.Click

        Select Case Me.m_mode
            Case eModeType.Load
                Me.LoadDatasets()
                If (Me.m_cbLoadEnableOnLoad.Checked = True) Then
                    Me.ApplyTimeSeries(True)
                End If

            Case eModeType.Weight
                Me.ApplyTimeSeries()

            Case eModeType.Import
                Me.ImportDataset()

                'Case eModeType.Export

            Case eModeType.Delete
                Me.DeleteDatasets()

        End Select

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

#End Region ' Generic

#Region " Load "

    Private Sub m_lvLoadDatasets_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
         Handles m_lvLoadDatasets.DoubleClick
        Me.OnOK(sender, e)
    End Sub

    Private Sub m_lvLoadDatasets_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_lvLoadDatasets.SelectedIndexChanged
        Dim ds As cTimeSeriesDataset = Me.GetLoadSelectedDataset()
        If (ds IsNot Nothing) Then
            Me.DatasetName = ds.Name
        End If
        Me.UpdateControls()
    End Sub

#End Region ' Load

#Region " Apply "

    Private Sub OnApplyCheckAll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnApplyCheckAll.Click
        Me.m_gridEnableTS.CheckAll(True)
    End Sub

    Private Sub OnApplyCheckNone(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnApplyCheckNone.Click
        Me.m_gridEnableTS.CheckAll(False)
    End Sub

#End Region ' Apply

#Region " Import "

    ' -- SOURCE --

    Private Sub OnImportBrowseSource(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnImportBrowse.Click

        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

        cmdFO.Invoke(Me.m_strImportFileName, "", My.Resources.FILEFILTER_CSV & "|" & My.Resources.FILEFILTER_TEXT)

        If (cmdFO.Result = DialogResult.OK) Then
            Me.m_strImportFileName = cmdFO.FileName
            Me.SetSource(cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.CSV)
            Me.ReloadTimeSeries()
        End If

    End Sub

    Private Sub OnImportSourceFileNameEntered(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tbImportFileName.TextChanged
        Me.m_strImportFileName = Me.m_tbImportFileName.Text
    End Sub

    Private Sub OnImportSourceFileNameFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tbImportFileName.GotFocus
        Me.SetSource(cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.CSV)
    End Sub

    Private Sub OnImportSetTextFileSource(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbImportSourceTextFile.CheckedChanged
        Me.SetSource(cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.CSV)
    End Sub

    Private Sub OnImportSetClipboardSource(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbImportSourceClipboard.CheckedChanged
        Me.SetSource(cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.Clipboard)
    End Sub

    ' -- DESTINATION --

    Private Sub m_cmbImportDataset_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbImportDataset.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub m_cmbImportDataset_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbImportDataset.SelectedIndexChanged
        Me.DatasetName = m_cmbImportDataset.Text
    End Sub

    ' -- Delimiters and separators --

    Private Sub OnImportDelimiterOrSeparatorChanged(ByVal sender As Object, ByVal arg As EventArgs) _
            Handles m_tbImportDelimiter.TextChanged, m_tbImportSeparator.TextChanged
        Me.ReloadTimeSeries()
    End Sub

    ' -- Preview --

    Private Sub OnSelectPreviewTS(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_cmbTimeSeriesPreview.SelectedIndexChanged

        Dim iShape As Integer = Me.m_cmbTimeSeriesPreview.SelectedIndex

        If (iShape = -1) Then
            Me.m_tsh.SelectedShape = Nothing
        Else
            Me.m_tsh.SelectedShape = Me.m_tr(iShape)
        End If

    End Sub

#End Region ' Import

#Region " Export "

#End Region ' Export

#Region " Delete "

    Private Sub m_lvDeleteDatasets_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
         Handles m_lvDeleteDatasets.DoubleClick
        Me.OnOK(sender, e)
    End Sub

    Private Sub m_lvDeleteDatasets_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_lvDeleteDatasets.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

#End Region ' Delete

#End Region ' Events

#Region " Internal implementation "

#Region " Generic "

    Public Property Mode() As eModeType
        Get
            Return Me.m_mode
        End Get
        Set(ByVal mode As eModeType)
            Me.m_mode = mode
            Select Case mode
                Case eModeType.Load
                    Me.m_tcMain.SelectedIndex = 0
                Case eModeType.Weight
                    Me.m_tcMain.SelectedIndex = 1
                Case eModeType.Import
                    Me.m_tcMain.SelectedIndex = 2
                Case eModeType.Delete
                    Me.m_tcMain.SelectedIndex = 3
                    'Case eModeType.Export
                    '    Me.m_tcMain.SelectedIndex = 4
            End Select

            Me.m_btnOk.Text = Me.m_tcMain.SelectedTab().Text
            Me.UpdateControls()

        End Set
    End Property

    Private Sub UpdateControls()

        Dim bCanPerformAction As Boolean = False

        ' -- APPLY --
        'Me.m_cbLoadApplyOnLoad.Enabled = (Me.m_lvLoadDatasets.SelectedItems.Count > 0)

        ' -- IMPORT --
        Dim bHasPreview As Boolean = Not (Object.ReferenceEquals(Me.m_tr.Preview(), Nothing))
        Dim bHasDataset As Boolean = Not (String.IsNullOrEmpty(Me.m_cmbImportDataset.Text))

        ' Update file name control
        Me.m_tbImportFileName.Text = Me.m_strImportFileName

        ' Update source radio buttons
        If TypeOf Me.m_tr Is cTimeSeriesCSVReader Then
            Me.m_rbImportSourceTextFile.Checked = True
        ElseIf TypeOf Me.m_tr Is cTimeSeriesClipboardReader Then
            Me.m_rbImportSourceClipboard.Checked = True
        End If

        'Me.m_tbImportDelimiter.Character = CChar(Me.m_strImportDelimiter)
        'Me.m_tbImportSeparator.Character = CChar(Me.m_strImportDecimalSeparator)

        ' Update preview controls
        Me.m_dgvImportPreview.Enabled = bHasPreview
        Me.m_lblImportPreview.Enabled = bHasPreview

        Select Case Me.Mode
            Case eModeType.Load : bCanPerformAction = (Me.m_lvLoadDatasets.SelectedItems.Count > 0)
            Case eModeType.Weight : bCanPerformAction = True
            Case eModeType.Import : bCanPerformAction = bHasPreview And bHasDataset
                'Case eModeType.Export : bCanPerformAction = False
            Case eModeType.Delete : bCanPerformAction = (Me.GetDeleteSelectedDatasets.Length > 0)
        End Select
        Me.m_btnOk.Enabled = bCanPerformAction

    End Sub

#End Region ' Generic 

#Region " Load "

    Private Function UpdateLoadPage() As Boolean

        Dim ds As cTimeSeriesDataset = Nothing
        Dim item As ListViewItem = Nothing
        Dim strLoaded As String = ""
        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim aitems(Me.m_core.nTimeSeriesDatasets - 1) As ListViewItem
        Me.m_lvLoadDatasets.Items.Clear()

        appl.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

        Me.m_lvLoadDatasets.Items.Add(My.Resources.GENERIC_VALUE_NONE)
        For iDS As Integer = 1 To Me.m_core.nTimeSeriesDatasets
            ' Get dataset
            ds = Me.m_core.TimeSeriesDataset(iDS)
            If ds.IsLoaded Then
                strLoaded = My.Resources.HEADER_YES
            Else
                strLoaded = ""
            End If
            item = New ListViewItem(New String() {ds.Name, strLoaded, ds.NumTimeSeries.ToString})
            item.Tag = ds
            item.Selected = (String.Compare(ds.Name, Me.DatasetName, False) = 0)
            aitems(iDS - 1) = item
        Next
        Me.m_lvLoadDatasets.Items.AddRange(aitems)

        ' No default item to select?
        If String.IsNullOrEmpty(Me.DatasetName) Then
            ' #Yes: Find first item to select, this being either the first dataset,
            '       or if no datasets avaiable, the (none) string
            item = Me.m_lvLoadDatasets.Items(Math.Min(Me.m_lvLoadDatasets.Items.Count - 1, 1))
            item.Selected = True
        End If

        Me.m_lvLoadDatasets.Select()

        ' Select the first one by default, change if saving previous runs
        If Me.m_lvLoadDatasets.Items.Count > 0 Then
            Me.m_lvLoadDatasets.TopItem.Focused = True
            Me.m_lvLoadDatasets.TopItem.Selected = True
        End If

        appl.SetStatusText("", TriState.False)
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load selected TS datasets into memory
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Function LoadDatasets() As Boolean

        Dim ds As cTimeSeriesDataset = Nothing
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        appl.SetStatusText(My.Resources.STATUS_LOADING, TriState.True)
        ds = Me.GetLoadSelectedDataset()
        If (ds IsNot Nothing) Then
            Me.m_core.LoadTimeSeries(ds.Index)
        Else
            Me.m_core.LoadTimeSeries(0)
        End If
        appl.SetStatusText("", TriState.False)

        Me.UpdateWeightsPage(True)

        Return True
    End Function

    Private Function GetLoadSelectedDataset() As cTimeSeriesDataset
        If (Me.m_lvLoadDatasets.SelectedItems.Count = 1) Then
            Return DirectCast(Me.m_lvLoadDatasets.SelectedItems(0).Tag, cTimeSeriesDataset)
        End If
        Return Nothing
    End Function

#End Region ' Load

#Region " Apply "



    Private Function UpdateWeightsPage(Optional ByVal bAutoApply As Boolean = False) As Boolean
        Me.m_gridEnableTS.ResetData()
        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Apply selected TS
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Function ApplyTimeSeries(Optional ByVal bApplyAll As Boolean = False) As Boolean
        Me.m_gridEnableTS.Apply(bApplyAll)
    End Function

#End Region ' Apply

#Region " Import "

    Private Sub FillImportDatasetCombo()
        Dim core As cCore = cCore.GetInstance()
        Dim ds As cTimeSeriesDataset = Nothing
        Dim iSelection As Integer = 0
        Dim strDatasetNew As String = ""

        Me.m_cmbImportDataset.Items.Clear()

        If Me.m_tr IsNot Nothing Then
            strDatasetNew = Me.m_tr.Dataset
        Else
            strDatasetNew = My.Resources.ECOSIM_DEFAULT_NEWDATASET
        End If

        For iDS As Integer = 1 To core.nTimeSeriesDatasets
            ds = core.TimeSeriesDataset(iDS)
            Me.m_cmbImportDataset.Items.Add(ds.Name)
        Next

        ' Try to select new dataset name
        iSelection = Me.m_cmbImportDataset.FindStringExact(strDatasetNew)
        If (iSelection >= 0) Then
            Me.m_cmbImportDataset.SelectedIndex = iSelection
        Else
            Me.m_cmbImportDataset.Items.Insert(0, strDatasetNew)
            Me.m_cmbImportDataset.SelectedIndex = 0
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Set the <see cref="cTimeSeriesReaderFactory.eTimeSeriesReaderTypes">source</see>
    ''' to import from.
    ''' </summary>
    ''' <param name="src">
    ''' The <see cref="cTimeSeriesReaderFactory.eTimeSeriesReaderTypes">source</see> 
    ''' to import from
    ''' </param>
    ''' -------------------------------------------------------------------
    Private Sub SetSource(ByVal src As cTimeSeriesReaderFactory.eTimeSeriesReaderTypes)
        Select Case src

            Case cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.CSV
                If TypeOf Me.m_tr Is cTimeSeriesCSVReader Then Return
                Me.m_tr = New cTimeSeriesCSVReader(cCore.GetInstance())

            Case cTimeSeriesReaderFactory.eTimeSeriesReaderTypes.Clipboard
                If TypeOf Me.m_tr Is cTimeSeriesClipboardReader Then Return
                Me.m_tr = New cTimeSeriesClipboardReader(cCore.GetInstance())

            Case Else
                Debug.Assert(False)

        End Select

        Me.ReloadTimeSeries()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load time series available for import from current <see cref="SetSource">source</see>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub ReloadTimeSeries()

        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If (Not Me.m_bInitialized) Then Return

        appl.SetStatusText(My.Resources.STATUS_PREVIEW_LOADING, TriState.True)
        Try
            If TypeOf Me.m_tr Is cTimeSeriesClipboardReader Then
                Me.m_tr.Read(CStr(Me.m_tbImportDelimiter.Character), CStr(Me.m_tbImportSeparator.Character))
            ElseIf TypeOf Me.m_tr Is cTimeSeriesCSVReader Then
                DirectCast(Me.m_tr, cTimeSeriesCSVReader).Read(Me.m_strImportFileName, CStr(Me.m_tbImportDelimiter.Character), CStr(Me.m_tbImportSeparator.Character))
            End If

            Me.UpdatePreview()
        Catch ex As Exception

        End Try
        appl.SetStatusText("", TriState.False)

    End Sub

    Private Sub UpdatePreview()

        Dim tsrPreview As cTimeSeriesTextReader.cPreview = Nothing
        Dim drow As DataGridViewRow = Nothing

        ' Try to obtain preview
        If Me.m_tr IsNot Nothing Then
            tsrPreview = Me.m_tr.Preview()
        End If

        Me.FillImportDatasetCombo()
        Me.UpdateControls()

        If Object.ReferenceEquals(tsrPreview, Nothing) Then
            Return
        End If

        ' Populate preview grid
        Me.m_dgvImportPreview.SuspendLayout()
        Me.m_dgvImportPreview.RowCount = tsrPreview.RowCount
        Me.m_dgvImportPreview.ColumnCount = tsrPreview.ColumnCount
        For iRow As Integer = 1 To tsrPreview.RowCount
            drow = Me.m_dgvImportPreview.Rows(iRow - 1)
            drow.ErrorText = tsrPreview.RowError(iRow).ToString()
            For iCol As Integer = 1 To tsrPreview.ColumnCount
                drow.Cells(iCol - 1).Value = tsrPreview.Value(iCol, iRow)
            Next
        Next
        Me.m_dgvImportPreview.ResumeLayout()

        ' Populate TS combo box
        Me.m_cmbTimeSeriesPreview.Items.Clear()
        For iTS As Integer = 0 To Me.m_tr.Count - 1
            Me.m_cmbTimeSeriesPreview.Items.Add(Me.m_tr(iTS).Name)
        Next
        Me.m_cmbTimeSeriesPreview.SelectedIndex = Me.m_tr.Count - 1

    End Sub

    Private Sub ImportDataset()

        Dim core As cCore = cCore.GetInstance()
        Dim ds As cTimeSeriesDataset = Nothing
        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim clf As cCore.eBatchChangeLevelFlags = cCore.eBatchChangeLevelFlags.TimeSeries
        Dim bSucces As Boolean = True

        If Not core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return

        ' Create new dataset
        bSucces = core.AppendTimeSeriesDataset(Me.DatasetName, Me.m_tbImportDescription.Text, _
                Me.m_tbImportAuthor.Text, Me.m_tbImportContact.Text, Me.m_tr.FirstYear, Me.m_tr.NumYears)

        ' Dataset succesfully removed?
        If (bSucces = True) Then
            ' #Yes: start importing
            appl.SetStatusText(String.Format(My.Resources.STATUS_IMPORTINGDATASET, Me.DatasetName), TriState.True)
            Try
                For Each ts As cTimeSeriesImport In Me.m_tr
                    If core.ImportEcosimTimeSeries(ts, core.ActiveTimeSeriesDatasetIndex) Then
                        Select Case cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)

                            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing
                                clf = DirectCast(Math.Min(clf, cCore.eBatchChangeLevelFlags.Ecosim), cCore.eBatchChangeLevelFlags)

                            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group, _
                                 cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                                clf = DirectCast(Math.Min(clf, cCore.eBatchChangeLevelFlags.Ecosim), cCore.eBatchChangeLevelFlags)

                        End Select
                    Else
                        bSucces = False
                    End If
                Next
            Catch ex As Exception
                bSucces = False
            End Try
            appl.SetStatusText("", TriState.False)
        End If

        ' Release appropriate level
        core.ReleaseBatchLock(clf)

        ' Need to apply on load?
        If (bSucces And Me.m_cbImportEnableOnImport.Checked) Then
            ' Start apply process
            appl.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)
            Try
                ' For each dataset
                For iDS As Integer = 1 To Me.m_core.nTimeSeriesDatasets
                    ' Get dataset #
                    Dim tsDS As cTimeSeriesDataset = Me.m_core.TimeSeriesDataset(iDS)
                    ' Is this the dataset that we just imported?
                    If (String.Compare(tsDS.Name, Me.DatasetName, True) = 0) Then
                        ' #Yes: Load and apply the dataset
                        Me.m_core.LoadTimeSeries(iDS, True)
                        ' Done!
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bSucces = False
            End Try
            appl.SetStatusText("", TriState.False)
        End If

        ' Close dialog
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

#End Region ' Import

#Region " Delete "

    Private Function UpdateDeletePage() As Boolean

        Dim ds As cTimeSeriesDataset = Nothing
        Dim item As ListViewItem = Nothing
        Dim strLoaded As String = ""
        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim aitems(Me.m_core.nTimeSeriesDatasets - 1) As ListViewItem
        Me.m_lvDeleteDatasets.Items.Clear()

        appl.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

        For iDS As Integer = 1 To Me.m_core.nTimeSeriesDatasets
            ' Get dataset
            ds = Me.m_core.TimeSeriesDataset(iDS)
            If ds.IsLoaded Then
                strLoaded = My.Resources.HEADER_YES
            Else
                strLoaded = ""
            End If
            item = New ListViewItem(New String() {ds.Name, strLoaded, ds.NumTimeSeries.ToString})
            item.Tag = ds
            item.Selected = (String.Compare(ds.Name, Me.DatasetName, False) = 0)
            aitems(iDS - 1) = item
        Next
        Me.m_lvDeleteDatasets.Items.AddRange(aitems)

        appl.SetStatusText("", TriState.False)
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Delete datasets from the database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Function DeleteDatasets() As Boolean

        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim bSucces As Boolean = True

        ' Ask for confirmation
        If MsgBox(My.Resources.ECOSIM_PROMPT_DELETE_TSDATASETS, MsgBoxStyle.Question Or MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then Return False

        ' Save any changes
        If Not Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False
        appl.SetStatusText(My.Resources.STATUS_LOADING, TriState.True)

        Try
            For Each ds As cTimeSeriesDataset In Me.GetDeleteSelectedDatasets
                If ds IsNot Nothing Then
                    bSucces = bSucces And Me.m_core.RemoveTimeSeriesDataset(ds)
                End If
            Next
        Catch ex As Exception
            bSucces = False
        End Try

        Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, bSucces)
        appl.SetStatusText("", TriState.False)

        Return bSucces
    End Function

    Private Function GetDeleteSelectedDatasets() As cTimeSeriesDataset()
        Dim lDatasets As New List(Of cTimeSeriesDataset)
        For Each item As ListViewItem In Me.m_lvDeleteDatasets.SelectedItems
            lDatasets.Add(DirectCast(item.Tag, cTimeSeriesDataset))
        Next
        Return lDatasets.ToArray()
    End Function

#End Region ' Delete

#End Region ' Internal implementation

#Region " Public properties "

    Public Property DatasetName() As String
        Get
            Return Me.m_strDataset
        End Get
        Set(ByVal value As String)
            Me.m_strDataset = value
        End Set
    End Property

#End Region ' Public properties

End Class
