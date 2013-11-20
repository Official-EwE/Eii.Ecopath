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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecospace.Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Configuration interface for setting up a <see cref="cSpatialDataAdapter"/>.
    ''' </summary>
    ''' <remarks>
    ''' This interface allows users to define new datasets, configure datasets, 
    ''' change dataset selections, define new converters and configure the
    ''' existing converter.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class ucConfigAdapter
        Implements IUIElement
        Implements IDisposable

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing

        ''' <summary>Selected data adapter</summary>
        Private m_adt As cSpatialDataAdapter = Nothing
        ''' <summary>Selected layer</summary>
        Private m_layer As cEcospaceLayer = Nothing

        ''' <summary>Flag to break looped layer change updates/notifications</summary>
        Private m_bInUpdate As Boolean = False

        ''' <summary>Flag that states whether there is any data in the cache</summary>
        Private m_bHasCachedData As Boolean = False

        Private m_bHasDatasetTemplates As Boolean = False

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Mandatory overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As cUIContext)

                If (Me.m_uic IsNot Nothing) Then
                    '' Stop indexing
                    'Me.m_manSets.IndexDataset = Nothing 

                    ' Disconnect from data objects first; we do not want disconnecting UI
                    ' elements from screwing up the last configuration
                    Me.m_adt = Nothing
                    Me.m_layer = Nothing

                    Me.m_gridDatasets.UIContext = Nothing

                    Me.m_manSets.Save()
                    Me.m_manSets = Nothing
                    Me.m_man = Nothing
                End If

                Me.m_uic = uic

                If (Me.m_uic IsNot Nothing) Then
                    ' Set new
                    Me.m_man = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
                    Me.m_gridDatasets.UIContext = Me.m_uic
                End If
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Populate all
            Me.FillTemplateDatasetBox()
            Me.m_gridDatasets.Fill(Me.m_adt, Nothing)

            ' Update cache state (will also update controls)
            Me.EvaluateCache()

            AddHandler Me.m_gridDatasets.OnSelectionChanged, AddressOf OnSelectDS
        End Sub

#End Region ' Mandatory overrides etc

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the connection (adapter + layer) to configure.
        ''' </summary>
        ''' <param name="adt"><see cref="cSpatialDataAdapter"/> to configure.</param>
        ''' <param name="layer"><see cref="cEcospaceLayer"/> to configure.</param>
        ''' -------------------------------------------------------------------
        Public Sub SetConnection(adt As cSpatialDataAdapter, layer As cEcospaceLayer)

            ' Store refs
            Me.m_adt = adt
            Me.m_layer = layer

            Me.m_bInUpdate = True

            ' Set initials
            If (adt IsNot Nothing) And (layer IsNot Nothing) Then
                Me.m_gridDatasets.Fill(Me.m_adt, adt.Dataset(layer.Index))
                Me.SelectConverter(adt.Converter(layer.Index))
            End If

            ' Done
            Me.PopulateAdapterControls()
            Me.UpdateControls()

            Me.m_bInUpdate = False

        End Sub

#End Region ' Public bits

#Region " Control events "

        Private Sub OnDatasetTemplateSelected(sender As Object, e As System.EventArgs) _
            Handles m_cmbNewDS.SelectedIndexChanged
            ' A new data set template has been selected. Make UI respond
            Me.UpdateControls()
        End Sub

        ''' <summary>
        ''' Event handler for customizing how datasets are displayed in this UI.
        ''' </summary>
        Private Sub OnFormatDS(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_cmbNewDS.Format

            Dim fmt As New cSpatialDatasetFormatter()
            If e.ListItem.Equals(String.Empty) Then
                e.Value = fmt.GetDescriptor(Nothing)
            Else
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If

        End Sub

        ''' <summary>
        ''' User wants to create a dataset of the selected type.
        ''' </summary>
        Private Sub OnCreateDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCreateDS.Click
            Me.Cursor = Cursors.WaitCursor
            Try
                Me.CreateDS()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
            Me.Cursor = Cursors.Default
        End Sub

        ''' <summary>
        ''' User has selected a dataset for the current adapter and layer.
        ''' </summary>
        Private Sub OnSelectDS(selection As SourceGrid2.CellVirtualCollection)

            Try
                Me.SelectedDataset = Me.m_gridDatasets.SelectedDataset
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' User wants to configure the currently selected dataset.
        ''' </summary>
        Private Sub OnConfigDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigDS.Click

            Me.Cursor = Cursors.WaitCursor
            Try
                Me.ConfigDS(Me.SelectedDataset)
                Me.m_gridDatasets.Fill(Me.m_adt, Me.SelectedDataset)
                Me.m_manSets.IndexDataset = Me.SelectedDataset
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucConficAdapter::OnConfigureDS")
            End Try
            Me.Cursor = Cursors.Default
            Me.LayerChanged()

        End Sub

        ''' <summary>
        ''' User wants to delete the selected dataset
        ''' </summary>
        Private Sub OnDeleteDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDeleteDS.Click
            Try
                Me.DeleteDS(Me.SelectedDataset)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' User wants to clear the spatial data cache.
        ''' </summary>
        Private Sub OnClearCache(sender As System.Object, e As System.EventArgs) _
            Handles m_btnClearCache.Click

            Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
            Dim dSizeTot As Double = cache.GetSize() / 1024
            Dim dSizeUnused As Double = cache.GetUnusedSize(Me.m_manSets) / 1024
            Dim strPrompt As String = My.Resources.PROMPT_CACHE_CLEAR
            Dim bSucces As Boolean = True

            Try
                If (dSizeUnused > 0) Then
                    Dim fmsg As New cFeedbackMessage(String.Format(strPrompt, Me.m_uic.StyleGuide.FormatNumber(dSizeTot), Me.m_uic.StyleGuide.FormatNumber(dSizeUnused)), _
                                                     EwEUtils.Core.eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                    Me.m_uic.Core.Messages.SendMessage(fmsg)

                    Select Case fmsg.Reply
                        Case eMessageReply.YES
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear(Me.m_manSets)
                        Case eMessageReply.NO
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                        Case eMessageReply.CANCEL
                    End Select
                Else
                    bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                End If
            Catch ex As Exception
                bSucces = False
            End Try

            ' Repopulate grid to reflect cache sizes
            Me.m_gridDatasets.Fill(Me.m_adt, Me.SelectedDataset)

            Dim dSizeTot2 As Double = cache.GetSize() / 1024
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_CACHECLEARED, Me.m_uic.StyleGuide.FormatNumber(dSizeTot - dSizeTot2)), _
                                    eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)

            ' Reflect new state
            Me.EvaluateCache()

        End Sub

        ''' <summary>
        ''' Event handler for customizing how converters are displayed in this UI.
        ''' </summary>
        Private Sub OnFormatCV(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_cmbConverter.Format

            Dim fmt As New cSpatialConverterFormatter()
            If e.ListItem.Equals(String.Empty) Then
                e.Value = fmt.GetDescriptor(Nothing)
            Else
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        End Sub

        ''' <summary>
        ''' User wants to configure the currently selected converter.
        ''' </summary>
        Private Sub OnConfigCV(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigureCV.Click
            Try
                Me.ConfigConverter(Me.SelectedConverter)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' User has selected a converter for the current adapter and layer.
        ''' </summary>
        Private Sub OnSelectCV(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbConverter.SelectedIndexChanged
            Try
                Dim obj As Object = Me.m_cmbConverter.SelectedItem
                If String.Empty.Equals(obj) Then
                    Me.SelectedConverter = Nothing
                Else
                    Me.SelectedConverter = DirectCast(obj, ISpatialDataConverter)
                End If
                Me.UpdateControls()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnDatScaleTypeChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_rbAbsolute.CheckedChanged, m_rbRelative.CheckedChanged

            Try
                If (TypeOf Me.m_adt Is cSpatialScalarDataAdapterBase) Then
                    Dim ssda As cSpatialScalarDataAdapterBase = DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                    If (Me.m_rbAbsolute.Checked) Then
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapterBase.eScaleType.Absolute
                    Else
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapterBase.eScaleType.Relative
                    End If

                    If Not Me.m_bInUpdate Then
                        ' Invalidate the cached data for this dataset
                        ' ToDo_JS: Make dataset clearing more sublte. 
                        '          This statement deletes cached data for ALL scenarios a dataset is cached for. It should only
                        '          clear the cached data for the current Ecospace scenario. Oof. Ok, at least it works...
                        cSpatialDataCache.DefaultDataCache.Clear(Me.SelectedDataset)
                        ' Reflect new state
                        Me.EvaluateCache()
                    End If
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub OnScaleChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbxScale.TextChanged, m_tbxScale.LostFocus
            Try
                If (TypeOf Me.m_adt Is cSpatialScalarDataAdapterBase) Then
                    Dim ssda As cSpatialScalarDataAdapterBase = DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                    Double.TryParse(Me.m_tbxScale.Text, ssda.DataScale(Me.m_layer.Index))
                End If

                If Not Me.m_bInUpdate Then
                    ' Invalidate the cached data for this dataset
                    cSpatialDataCache.DefaultDataCache.Clear(Me.SelectedDataset)
                End If
                ' Reflect new state
                Me.EvaluateCache()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnCalculateScale(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Try
                ' Stop any indexing
                Me.m_manSets.IndexDataset = Nothing
                Me.UpdateControls()

                Dim ssda As cSpatialScalarDataAdapterBase = DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                Dim iStartTimeStep As Integer = Math.Max(1, Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(Me.SelectedDataset.TimeStart))
                Dim dtStartDate As DateTime = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iStartTimeStep)
                Dim dScale As Double = 1.0
                Dim msg As cMessage = Nothing

                ' Perform calculation
                Select Case ssda.CalculateScaleFromEcopathTimePeriod(iStartTimeStep, dScale)

                    Case cDatasetCompatilibity.eCompatibilityTypes.NotSet
                        msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NOINDEX), _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Information)
                    Case cDatasetCompatilibity.eCompatibilityTypes.Errors, _
                         cDatasetCompatilibity.eCompatibilityTypes.NoTemporal
                        msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NODATA, dtStartDate.ToShortDateString()), _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
                    Case cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
                        msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NOOVERLAP, dtStartDate.ToShortDateString()), _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
                    Case Else
                        ' Only when ok
                        ssda.DataScale(Me.m_layer.Index) = dScale
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapterBase.eScaleType.Relative

                End Select

                ' Got compatibility error message?
                If (msg IsNot Nothing) Then
                    Me.m_uic.Core.Messages.SendMessage(msg)
                End If

                Me.PopulateAdapterControls()
                Me.EvaluateCache()
                Me.m_gridDatasets.UpdateCacheInfo(Me.SelectedDataset)

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnSaveStats(sender As System.Object, e As System.EventArgs) _
            Handles m_btnSaveStats.Click

            ' This is very deliberately hidden functionality!
            Dim ds As ISpatialDataSet = Me.SelectedDataset
            Dim cv As ISpatialDataConverter = Me.SelectedConverter
            Dim sw As StreamWriter = Nothing
            Dim core As cCore = Me.m_uic.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim strFile As String = ""

            If (ds Is Nothing) Or (cv Is Nothing) Then Return

            strFile = Path.Combine(Me.m_uic.Core.OutputPath(), cFileUtils.ToValidFileName(ds.DisplayName & "_stats.csv", False))
            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
                Return
            End If

            Try
                sw = New StreamWriter(strFile)
            Catch ex As Exception
                Return
            End Try

            sw.WriteLine("timestep,date,min,max,mean,stdev")
            Dim iStart As Integer = Math.Max(0, core.AbsoluteTimeToEcospaceTimestep(ds.TimeStart))
            Dim iEnd As Integer = Math.Min(core.AbsoluteTimeToEcospaceTimestep(ds.TimeEnd), core.nEcospaceTimeSteps)
            Dim t As Date = Nothing
            Dim rs As ISpatialRaster = Nothing

            Try

                For i As Integer = iStart To iEnd
                    t = core.EcospaceTimestepToAbsoluteTime(i)
                    If ds.HasDataAtT(t) Then
                        Console.WriteLine("Getting stats for time step " & i & " [" & iStart & ", " & iEnd & "]")
                        If (ds.LockDataAtT(t, bm.CellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                            rs = ds.GetRaster(cv, "")
                            sw.WriteLine("{0},{1},{2},{3},{4},{5}", _
                                              i, cStringUtils.ToCSVField(t.ToShortDateString()), _
                                              cStringUtils.FormatNumber(rs.Min), _
                                              cStringUtils.FormatNumber(rs.Max), _
                                              cStringUtils.FormatNumber(rs.Mean), _
                                              cStringUtils.FormatNumber(rs.StandardDeviation))
                            ds.Unlock()
                        End If
                    End If
                Next
            Catch ex As Exception
                sw.WriteLine(cStringUtils.ToCSVField(ex.Message))
            End Try
            sw.Flush()
            sw.Close()

            Try
                Dim msg As New cMessage("External data statistics saved to " & strFile, eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strFile)
                core.Messages.SendMessage(msg)
            Catch ex As Exception

            End Try

            ' Update
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' Grid callback
        ''' </summary>
        ''' <param name="ds"></param>
        Private Sub OnConfigDS(ds As ISpatialDataSet) Handles m_gridDatasets.OnConfigDS
            Me.ConfigDS(ds)
        End Sub

#End Region ' Control events

#Region " Internals "

        Private Sub UpdateControls()

            Dim bHasContext As Boolean = (Me.m_adt IsNot Nothing) And (Me.m_layer IsNot Nothing)
            Dim ds As ISpatialDataSet = Me.SelectedDataset
            Dim cv As ISpatialDataConverter = Me.SelectedConverter
            Dim bCanConfigDS As Boolean = False
            Dim bCanConfigCV As Boolean = False
            Dim bIsConfigured As Boolean = False
            Dim bIsIndexing As Boolean = Me.m_manSets.IsIndexing(ds)
            Dim bNeedsConverter As Boolean = False

            If (ds IsNot Nothing) Then
                bCanConfigDS = bHasContext And (TypeOf ds Is IConfigurablePlugin)
                bIsConfigured = bHasContext And Me.m_adt.IsConnected(Me.m_layer.Index)
                bNeedsConverter = Not String.IsNullOrWhiteSpace(ds.ConversionFormat)
            End If
            If (cv IsNot Nothing) Then
                bCanConfigCV = bHasContext And (TypeOf cv Is IConfigurablePlugin)
            End If

            Me.m_btnCreateDS.Enabled = Me.m_bHasDatasetTemplates
            Me.m_btnConfigDS.Enabled = bCanConfigDS
            Me.m_btnDeleteDS.Enabled = (ds IsNot Nothing)
            Me.m_btnConfigureCV.Enabled = bCanConfigCV
            Me.m_btnClearCache.Enabled = (Me.m_bHasCachedData = True)

#If DEBUG Then
            Me.m_btnSaveStats.Visible = True
            Me.m_btnSaveStats.Enabled = bIsConfigured
#Else
            Me.m_btnSaveStats.Visible = False
#End If

            If (bHasContext) Then
                Me.m_hdrSource.Text = String.Format(My.Resources.CAPTION_EXTERNAL_DATA_DETAIL, Me.m_layer.Name)
            Else
                Me.m_hdrSource.Text = My.Resources.CAPTION_EXTERNAL_DATA
            End If

            Me.m_gridDatasets.Enabled = bHasContext

            Me.m_lblSelectCV.Enabled = bHasContext And bNeedsConverter
            Me.m_cmbConverter.Enabled = Me.m_lblSelectCV.Enabled

            Me.m_btnCalculate.Enabled = bIsConfigured

        End Sub

        Private Sub FillTemplateDatasetBox()

            Me.m_cmbNewDS.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_man.DatasetTemplates
                Me.m_cmbNewDS.Items.Add(ds)
            Next

            If (Me.m_cmbNewDS.Items.Count = 0) Then
                Me.m_cmbNewDS.Items.Add("")
                Me.m_bHasDatasetTemplates = False
            Else
                Me.m_bHasDatasetTemplates = True
            End If
            Me.m_cmbNewDS.SelectedIndex = 0

        End Sub

        Private Sub FillCompatibleConverterBox(Optional cv As ISpatialDataConverter = Nothing)

            If (cv Is Nothing) Then cv = Me.SelectedConverter
            Me.m_cmbConverter.Items.Clear()
            For Each cvTest As ISpatialDataConverter In Me.m_man.ConverterTemplates(Me.SelectedDataset)
                If (cv Is Nothing) Then cv = cvTest
                Me.m_cmbConverter.Items.Add(cvTest)
            Next
            Me.SelectConverter(cv)

        End Sub

        Private Sub ConfigConverter(cv As ISpatialDataConverter)
            ' NOP
        End Sub

        Private Property SelectedDataset As ISpatialDataSet
            Get
                If (Me.m_adt Is Nothing) Then Return Nothing
                Return Me.m_adt.Dataset(Me.m_layer.Index)
            End Get
            Set(dataset As ISpatialDataSet)

                If (Me.m_adt Is Nothing) Then Return

                ' Apply
                If (Not Object.ReferenceEquals(Me.m_adt.Dataset(Me.m_layer.Index), dataset)) Then
                    Me.m_adt.Dataset(Me.m_layer.Index) = dataset
                    'Me.LayerChanged()
                    Me.FillCompatibleConverterBox(Me.m_adt.Converter(Me.m_layer.Index))
                    Me.m_manSets.IndexDataset = dataset
                    Me.UpdateControls()
                End If

            End Set
        End Property

        Private Sub SelectConverter(converter As ISpatialDataConverter)
            ' Update selection
            Dim iIndex As Integer = 0
            If (converter IsNot Nothing) Then
                For Each item As Object In Me.m_cmbConverter.Items
                    If converter.GetType().Equals(item.GetType()) Then
                        Me.m_cmbConverter.SelectedItem = item
                        Return
                    End If
                Next
            End If
            Me.m_cmbConverter.SelectedItem = Nothing
        End Sub

        Private Property SelectedConverter As ISpatialDataConverter
            Get
                If (Me.m_adt Is Nothing) Then Return Nothing
                Return Me.m_adt.Converter(Me.m_layer.Index)
            End Get
            Set(converter As ISpatialDataConverter)

                If (Me.m_adt Is Nothing) Then Return

                ' Apply
                If (Not Object.ReferenceEquals(Me.m_adt.Converter(Me.m_layer.Index), converter)) Then
                    Me.m_adt.Converter(Me.m_layer.Index) = converter

                    If Not Me.m_bInUpdate Then
                        Me.LayerChanged()
                    End If
                End If

            End Set
        End Property

        Private Sub CreateDS()

            If (Me.m_adt Is Nothing) Then Return

            Dim item As Object = Me.m_cmbNewDS.SelectedItem
            If Not TypeOf (item) Is ISpatialDataSet Then Return

            Dim dsSelected As ISpatialDataSet = DirectCast(item, ISpatialDataSet)
            Dim dsNew As ISpatialDataSet = Nothing

            If (dsSelected Is Nothing) Then Return

            dsNew = CType(Activator.CreateInstance(dsSelected.GetType()), ISpatialDataSet)
            If (dsNew Is Nothing) Then Return

            Try
                dsNew.VarName = Me.m_adt.VarName
                If Me.ConfigDS(dsNew) Then
                    Me.m_manSets.Add(dsNew)
                    Me.m_gridDatasets.Fill(Me.m_adt, dsNew)
                    Me.m_manSets.IndexDataset = dsNew
                End If
            Catch ex As Exception
                cLog.Write(ex, "ucConficAdapter::CreateDS")
            End Try

        End Sub

        Private Function ConfigDS(ds As ISpatialDataSet) As Boolean

            If (ds Is Nothing) Then Return False
            If (Not TypeOf ds Is IConfigurablePlugin) Then Return True

            If (TypeOf ds Is IPlugin) Then
                DirectCast(ds, IPlugin).Initialize(Me.m_uic.Core)
            End If

            Dim dsConf As IConfigurablePlugin = DirectCast(ds, IConfigurablePlugin)
            Dim ctrl As Control = dsConf.GetConfigUI()

            If (ctrl Is Nothing) Then Return dsConf.IsConfigured

            Dim dlg As New dlgConfig()
            dlg.UIContext = Me.UIContext
            dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl)

            Me.EvaluateCache()

            Return (dsConf.IsConfigured)

        End Function

        Public Sub DeleteDS(ds As ISpatialDataSet)
            Me.SelectedDataset = Nothing
            Me.m_manSets.Remove(ds)
            Me.m_gridDatasets.Fill(Me.m_adt)
        End Sub

        Private Sub LayerChanged()

            If (Me.m_uic Is Nothing) Then Return
            If (Me.m_adt Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Me.m_man.Update()
            ' Me.m_uic.Core.onChanged(Me.m_layer)
            Me.UpdateControls()

        End Sub

        Private Sub EvaluateCache()
            Me.m_bHasCachedData = (cSpatialDataCache.DefaultDataCache.GetSize > 0)
            Me.UpdateControls()
        End Sub

#Region " Scalar data adapter "

        Private Sub PopulateAdapterControls()

            ' Do not trigger invalidating updates!
            Me.m_bInUpdate = True

            If (TypeOf Me.m_adt Is cSpatialScalarDataAdapterBase) Then
                Dim ssda As cSpatialScalarDataAdapterBase = DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                Select Case ssda.DataScaleType(Me.m_layer.Index)
                    Case cSpatialScalarDataAdapterBase.eScaleType.Absolute
                        Me.m_rbAbsolute.Checked = True
                    Case cSpatialScalarDataAdapterBase.eScaleType.Relative
                        Me.m_rbRelative.Checked = True
                End Select
                Me.m_tbxScale.Text = ssda.DataScale(Me.m_layer.Index).ToString
                Me.m_plScalarAdapter.Visible = True
            Else
                Me.m_plScalarAdapter.Visible = False
            End If

            Me.m_bInUpdate = False

        End Sub

#End Region ' Scalar data adapter

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls
