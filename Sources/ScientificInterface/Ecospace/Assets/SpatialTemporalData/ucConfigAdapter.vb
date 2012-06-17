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
Imports EwECore.Ecospace
Imports EwECore.SpatialData
Imports EwEPlugin
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
        ''' <summary>Flag that states whether indexing is in place</summary>
        Private m_bIndexing As Boolean = False

        ''' <summary>Flag that states whether there is any data in the cache</summary>
        Private m_bHasCachedData As Boolean = False

        Private m_mhEcospace As cMessageHandler = Nothing

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
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                    Me.m_mhEcospace = Nothing

                    Me.m_adt = Nothing
                    Me.m_layer = Nothing
                    Me.m_manSets.IndexDataset(Nothing) ' Stop indexing
                    Me.m_manSets.Save()
                    Me.m_manSets = Nothing
                    Me.m_man = Nothing
                End If

                Me.m_uic = uic

                If (Me.m_uic IsNot Nothing) Then
                    ' Set new
                    Me.m_man = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager

                    Me.m_mhEcospace = New cMessageHandler(AddressOf OnCoreMessage, EwEUtils.Core.eCoreComponentType.EcoSpace, eMessageType.DataModified, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhEcospace)

                End If
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Only evaluate cache on load
            Me.EvaluateCache()
            ' Populate all
            Me.FillTemplateDatasetBox()
            Me.FillExistingDatasetBox(Nothing)
            Me.FillExistingConverterBox()

            ' Done
            Me.UpdateControls()

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
                Me.SelectDataset(adt.Dataset(layer.Index))
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
            Handles m_lbxExistingDS.Format, m_cmbNewDS.Format

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
        Private Sub OnSelectDS(sender As System.Object, e As System.EventArgs) _
            Handles m_lbxExistingDS.SelectedIndexChanged
            Try
                Dim obj As Object = Me.m_lbxExistingDS.SelectedItem
                If String.Empty.Equals(obj) Then
                    Me.SelectedDataset = Nothing
                Else
                    Me.SelectedDataset = DirectCast(obj, ISpatialDataSet)
                End If
                Me.UpdateControls()
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
                Me.FillExistingDatasetBox()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
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
                                                     EwEUtils.Core.eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, cFeedbackMessage.eReplyStyle.YES_NO_CANCEL)
                    Me.m_uic.Core.Messages.SendMessage(fmsg)

                    Select Case fmsg.Reply
                        Case cFeedbackMessage.eReply.YES
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear(Me.m_manSets)
                        Case cFeedbackMessage.eReply.NO
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                        Case cFeedbackMessage.eReply.CANCEL
                    End Select
                Else
                    bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                End If
            Catch ex As Exception
                bSucces = False
            End Try

            Dim dSizeTot2 As Double = cache.GetSize() / 1024
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_CACHECLEARED, Me.m_uic.StyleGuide.FormatNumber(dSizeTot - dSizeTot2)), eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)

            ' Reflect new state
            Me.EvaluateCache()
            Me.UpdateControls()

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
                If (TypeOf Me.m_adt Is cSpatialScalarDataAdapter) Then
                    Dim ssda As cSpatialScalarDataAdapter = DirectCast(Me.m_adt, cSpatialScalarDataAdapter)
                    If (Me.m_rbAbsolute.Checked) Then
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapter.eScaleType.Absolute
                    Else
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapter.eScaleType.Relative
                    End If
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub OnScaleChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbxScale.TextChanged
            Try
                If (TypeOf Me.m_adt Is cSpatialScalarDataAdapter) Then
                    Dim ssda As cSpatialScalarDataAdapter = DirectCast(Me.m_adt, cSpatialScalarDataAdapter)
                    Single.TryParse(Me.m_tbxScale.Text, ssda.DataScale(Me.m_layer.Index))
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnCalculateScale(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Try
                ' Stop any indexing
                Me.m_manSets.IndexDataset(Nothing)
                Me.UpdateControls()

                Dim ssda As cSpatialScalarDataAdapter = DirectCast(Me.m_adt, cSpatialScalarDataAdapter)
                Dim iStartTimeStep As Integer = Math.Max(1, Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(Me.SelectedDataset.TimeStart))
                Dim dtStartDate As DateTime = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iStartTimeStep)
                Dim dScale As Double = 1.0
                Dim msg As cMessage = Nothing

                ' Perform calculation
                Select Case ssda.CalculateScaleFromEcopathTimePeriod(iStartTimeStep, dScale)

                    Case cDatasetCompatilibity.eCompatibilityTypes.Errors, _
                         cDatasetCompatilibity.eCompatibilityTypes.NoTemporal
                        msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NODATA, dtStartDate.ToShortDateString()), _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
                    Case cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
                        msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NOOVERLAP, dtStartDate.ToShortDateString()), _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
                    Case Else
                        ' Only when ok
                        ssda.DataScale(Me.m_layer.Index) = CSng(dScale)
                        ssda.DataScaleType(Me.m_layer.Index) = cSpatialScalarDataAdapter.eScaleType.Relative

                End Select

                ' Got compatibility error message?
                If (msg IsNot Nothing) Then
                    Me.m_uic.Core.Messages.SendMessage(msg)
                End If

                Me.PopulateAdapterControls()

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnCoreMessage(ByRef msg As cMessage)

            If msg.DataType = EwEUtils.Core.eDataTypes.EcospaceSpatialDataConnection Then
                Me.UpdateControls()
            End If

        End Sub

#End Region ' Control events

#Region " Internals "

        Private Sub UpdateControls()

            Dim bIsConnected As Boolean = (Me.m_adt IsNot Nothing) And (Me.m_layer IsNot Nothing)
            Dim ds As ISpatialDataSet = Me.SelectedDataset
            Dim cv As ISpatialDataConverter = Me.SelectedConverter
            Dim bCanConfigDS As Boolean = False
            Dim bCanConfigCV As Boolean = False
            Dim bIsConfigured As Boolean = bIsConnected AndAlso Me.m_adt.IsConnected(Me.m_layer.Index)
            Dim bIsIndexing As Boolean = Me.m_manSets.IsIndexing(ds)

            If (ds IsNot Nothing) Then bCanConfigDS = bIsConnected And (TypeOf ds Is IConfigurablePlugin)
            If (cv IsNot Nothing) Then bCanConfigCV = bIsConnected And (TypeOf cv Is IConfigurablePlugin)

            Me.m_btnCreateDS.Enabled = (Me.m_cmbNewDS.SelectedIndex >= 0)
            Me.m_btnConfigDS.Enabled = bCanConfigDS
            Me.m_btnDeleteDS.Enabled = (ds IsNot Nothing)
            Me.m_btnConfigureCV.Enabled = bCanConfigCV
            Me.m_btnClearCache.Enabled = (Me.m_bHasCachedData = True)

            If (bIsConnected) Then
                Me.m_hdrSource.Text = String.Format(My.Resources.CAPTION_EXTERNAL_DATA_DETAIL, Me.m_layer.Name)
            Else
                Me.m_hdrSource.Text = My.Resources.CAPTION_EXTERNAL_DATA
            End If

            Me.m_lbxExistingDS.Enabled = bIsConnected
            Me.m_cmbConverter.Enabled = bIsConnected
            Me.m_btnCalculate.Enabled = bIsConfigured

            ' -- Indexing status --

            Dim strValidate As String = ""
            Dim imgValidate As Image = Nothing

            If (ds IsNot Nothing) Then

                Dim comp As New cDatasetCompatilibity(Me.m_uic.Core, ds)
                strValidate = String.Format(My.Resources.STATUS_SPATIALCOMPATIBILITY, _
                                            Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(comp.FirstTimeStep).ToShortDateString, _
                                            Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(comp.LastTimeStep).ToShortDateString, _
                                            CInt(Math.Ceiling(100 * comp.NumIndexed / Math.Max(1, comp.NumOverlappingTimeSteps))), _
                                            comp.ToString, _
                                            cStringUtils.vbNewline)
                If bIsIndexing Then
                    imgValidate = ScientificInterfaceShared.My.Resources.ani_loader
                ElseIf (comp.Compatibility = cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap) Then
                    imgValidate = ScientificInterfaceShared.My.Resources.OK
                Else
                    imgValidate = ScientificInterfaceShared.My.Resources.Warning
                End If

            End If

            Me.m_pbCompatibility.Image = imgValidate
            Me.m_lblCompatibility.Text = strValidate

        End Sub

        Private Sub FillTemplateDatasetBox()

            Me.m_cmbNewDS.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_man.DatasetTemplates
                Me.m_cmbNewDS.Items.Add(ds)
            Next
            If (Me.m_cmbNewDS.Items.Count > 0) Then
                Me.m_cmbNewDS.SelectedIndex = 0
            End If

        End Sub

        Private Sub FillExistingDatasetBox(Optional ds As ISpatialDataSet = Nothing)

            If (ds Is Nothing) Then ds = Me.SelectedDataset
            Me.m_lbxExistingDS.Items.Clear()
            Me.m_lbxExistingDS.Items.Add("")
            For i As Integer = 0 To Me.m_manSets.Count - 1
                Me.m_lbxExistingDS.Items.Add(Me.m_manSets(i))
            Next
            Me.SelectDataset(ds)

        End Sub

        Private Sub FillExistingConverterBox(Optional cv As ISpatialDataConverter = Nothing)

            If (cv Is Nothing) Then cv = Me.SelectedConverter
            Me.m_cmbConverter.Items.Clear()
            Me.m_cmbConverter.Items.Add("")
            For Each cvTest As ISpatialDataConverter In Me.m_man.ConverterTemplates
                Me.m_cmbConverter.Items.Add(cvTest)
            Next
            Me.SelectConverter(cv)
        End Sub

        Private Sub ConfigConverter(cv As ISpatialDataConverter)
            ' NOP
        End Sub

        Private Sub SelectDataset(dataset As ISpatialDataSet)

            ' Update selection
            Dim iIndex As Integer = 0
            If (dataset IsNot Nothing) Then
                iIndex = Me.m_lbxExistingDS.Items.IndexOf(dataset)
            End If
            Me.m_lbxExistingDS.SelectedIndex = iIndex
            Me.SelectedDataset = dataset

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
                    Me.LayerChanged()
                End If
                Me.m_manSets.IndexDataset(dataset)

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
                    Me.LayerChanged()
                End If

            End Set
        End Property

        Private Sub CreateDS()

            Dim dsSelected As ISpatialDataSet = DirectCast(Me.m_cmbNewDS.SelectedItem, ISpatialDataSet)
            Dim dsNew As ISpatialDataSet = Nothing

            If (dsSelected Is Nothing) Then Return

            dsNew = CType(Activator.CreateInstance(dsSelected.GetType()), ISpatialDataSet)
            If (dsNew Is Nothing) Then Return

            If Me.ConfigDS(dsNew) Then
                Me.m_manSets.Add(dsNew)
                Me.FillExistingDatasetBox(dsNew)
            End If

        End Sub

        Private Function ConfigDS(ds As ISpatialDataSet) As Boolean

            If (ds Is Nothing) Then Return False
            If (Not TypeOf ds Is IConfigurablePlugin) Then Return True

            Dim dsConf As IConfigurablePlugin = DirectCast(ds, IConfigurablePlugin)
            Dim ctrl As Control = dsConf.GetConfigUI()

            If (ctrl Is Nothing) Then Return dsConf.IsConfigured

            Dim dlg As New dlgConfig()
            dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl)

            Return (dsConf.IsConfigured)

        End Function

        Public Sub DeleteDS(ds As ISpatialDataSet)
            Me.SelectedDataset = Nothing
            Me.m_manSets.Remove(ds)
            Me.FillExistingDatasetBox()
        End Sub

        Private Sub LayerChanged()

            If (Me.m_uic Is Nothing) Then Return
            If (Me.m_adt Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Me.m_man.Update()
            Me.m_uic.Core.onChanged(Me.m_layer)

        End Sub

        Private Sub EvaluateCache()
            Me.m_bHasCachedData = (cSpatialDataCache.DefaultDataCache.GetSize > 0)
        End Sub

#Region " Scalar data adapter "

        'Private Sub CalculateScaleFromEcopathTimePeriod()

        '    Dim iIndex As Integer = Me.m_layer.Index

        '    ' Early bail-out
        '    If Not Me.m_adt.IsConnected(iIndex) Then Return

        '    Dim ds As ISpatialDataSet = Me.m_adt.Dataset(Me.m_layer.Index)
        '    Dim cv As ISpatialDataConverter = Me.m_adt.Converter(Me.m_layer.Index)
        '    Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
        '    Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
        '    Dim ldtData As New List(Of DateTime)
        '    Dim iTSMin As Integer = 1
        '    Dim iTSMax As Integer = 1
        '    Dim rs As ISpatialRaster = Nothing

        '    ' Determine time steps with overlap
        '    For i As Integer = 1 To CInt(Me.m_uic.Core.EcospaceModelParameters.NumberOfTimeStepsPerYear)
        '        Dim dt As Date = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(i)
        '        If ds.HasDataAtT(dt) Then
        '            iTSMin = Math.Min(iTSMin, i)
        '            iTSMax = Math.Max(iTSMax, i)
        '            ldtData.Add(dt)
        '        End If
        '    Next

        '    ' Determine compatibility
        '    Dim comp As New cDatasetCompatilibity(Me.m_uic.Core, ds, iTSMin, iTSMax - iTSMin)
        '    Dim msg As cMessage = Nothing

        '    Select Case comp.Compatibility
        '        Case cDatasetCompatilibity.eCompatibilityTypes.Errors, _
        '             cDatasetCompatilibity.eCompatibilityTypes.NoTemporal
        '            msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NODATA, iYear), _
        '                               eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
        '        Case cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
        '            msg = New cMessage(String.Format(My.Resources.PROMPT_SPATIALTEMPORAL_CALC_NOOVERLAP, iYear), _
        '                               eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)

        '    End Select

        '    ' Got compatibility error message?
        '    If (msg IsNot Nothing) Then
        '        ' #Yes: abort
        '        Me.m_uic.Core.Messages.SendMessage(msg)
        '        Return
        '    End If

        '    Dim rst As ISpatialRaster = Nothing
        '    Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        '    Dim dCellSize As Double = bm.CellSize
        '    Dim dMean As Double = 0.0
        '    Dim MapTotValue As Double = 0.0
        '    Dim lNumValCells As Long = 0
        '    Dim NumWaterCells As Long = 0
        '    Dim iInRow As Integer = bm.InRow
        '    Dim iInCol As Integer = bm.InCol

        '    ' Stop any indexing
        '    Me.m_manSets.IndexDataset(Nothing)

        '    cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
        '    Try

        '        For i As Integer = 0 To ldtData.Count - 1
        '            Dim dt As DateTime = ldtData(i)

        '            cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, _
        '                                                      String.Format(My.Resources.STATUS_SPATIALTEMPORAL_CALCULATING, m_layer.Name, ds.DisplayName), _
        '                                                      CSng(i / ldtData.Count))

        '            ' Do the spatial magics
        '            If (ds.LockDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
        '                rst = Me.m_adt.Dataset(iIndex).GetRaster(Me.m_adt.Converter(iIndex), Me.m_layer.Name())

        '                For iRow As Integer = 1 To iInRow
        '                    For iCol As Integer = 1 To iInCol
        '                        If depth.IsWaterCell(iRow, iCol) Then
        '                            Dim dval As Double = rst.Cell(iRow, iCol)
        '                            If (dval <> cCore.NULL_VALUE And dval <> rst.NoData) Then
        '                                NumWaterCells += 1
        '                                MapTotValue += dval
        '                            End If
        '                        End If
        '                    Next
        '                Next

        '                ds.Unlock()
        '            End If

        '        Next
        '    Catch ex As Exception

        '    End Try
        '    cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        '    ' Resume indexing
        '    Me.m_manSets.IndexDataset(Me.SelectedDataset)

        '    If MapTotValue = 0 Then MapTotValue = 1

        '    ' Update format provider
        '    Me.m_tbxScale.Text = (NumWaterCells / MapTotValue).ToString
        '    ' Notify the world
        '    Me.LayerChanged()

        'End Sub

        Private Sub PopulateAdapterControls()
            If (TypeOf Me.m_adt Is cSpatialScalarDataAdapter) Then
                Dim ssda As cSpatialScalarDataAdapter = DirectCast(Me.m_adt, cSpatialScalarDataAdapter)
                Select Case ssda.DataScaleType(Me.m_layer.Index)
                    Case cSpatialScalarDataAdapter.eScaleType.Absolute
                        Me.m_rbAbsolute.Checked = True
                    Case cSpatialScalarDataAdapter.eScaleType.Relative
                        Me.m_rbRelative.Checked = True
                End Select
                Me.m_tbxScale.Text = ssda.DataScale(Me.m_layer.Index).ToString
                Me.m_plScalarAdapter.Visible = True
            Else
                Me.m_plScalarAdapter.Visible = False
            End If
        End Sub

#End Region ' Scalar data adapter

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls
