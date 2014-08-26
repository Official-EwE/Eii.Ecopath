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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

' ToDo: Use format provider for scale box
' ToDo: Populate dataset details panel
' ToDo: Respond to configuration / name changes
' ToDo: Enable varname hierarchy in TreeView

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
    Public Class dlgApplyConnection
        Implements IUIElement
        Implements IDisposable

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing

        ''' <summary>Selected data adapter</summary>
        Private m_adt As cSpatialDataAdapter = Nothing
        ''' <summary>Selected layer index</summary>
        Private m_iLayer As Integer = -1

        ''' <summary>Flag to break looped layer change updates/notifications</summary>
        Private m_bInUpdate As Boolean = False
        Private m_bIsChanged As Boolean = False
        Private m_bIsScaling As Boolean = False

        Private m_iNumConn As Integer = 0

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(uic As cUIContext, adt As cSpatialDataAdapter, layer As cEcospaceLayer)

            Me.InitializeComponent()
            Me.m_adt = adt
            Me.m_iLayer = layer.Index

            Me.m_bIsScaling = (TypeOf adt Is cSpatialScalarDataAdapterBase)

            ' Count number of configured connections
            For i As Integer = 1 To cSpatialDataStructures.cMAX_CONN
                If adt.Dataset(layer.Index, i) IsNot Nothing Then
                    Me.m_iNumConn += 1
                End If
            Next
            Me.UIContext = uic
            Me.Text = String.Format(Me.Text, layer.Name)

        End Sub

#End Region ' Constructor

#Region " Form overrides "

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

                    ' Disconnect from data objects first; we do not want disconnecting UI elements from screwing up the last configuration
                    Me.m_adt = Nothing
                    Me.m_iLayer = Nothing

                    Me.m_lbSourceDatasets.UIContext = Nothing
                    Me.m_gridConnections.UIContext = Nothing

                    Me.m_manSets.Save()
                    Me.m_manSets = Nothing
                    Me.m_man = Nothing
                End If

                Me.m_uic = uic

                If (Me.m_uic IsNot Nothing) Then
                    ' Set new
                    Me.m_man = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
                    Me.m_lbSourceDatasets.UIContext = Me.m_uic
                    Me.m_gridConnections.UIContext = Me.m_uic
                End If
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_bInUpdate = True

            ' Kick!
            Me.FillSourceDatasetBox()

            ' Dynamic makeup
            Me.m_tsbnFilter.Image = SharedResources.FilterHS
            Me.m_tsbnDefineConnections.Image = SharedResources.Database

            ' Start listening to grid events
            AddHandler Me.m_gridConnections.OnSelectionChanged, AddressOf OnSelectDS

            ' Populate
            For i As Integer = 1 To Me.m_iNumConn
                Me.m_gridConnections.Add(Me.m_adt.Dataset(m_iLayer, i), (i = Me.m_iNumConn))
            Next

            Me.m_bInUpdate = False
            Me.CenterToParent()

            Me.UpdateDatasetPanel()
            Me.UpdateConversionPanel()
            Me.UpdateScalingPanel()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            If Me.m_bIsChanged Then
                Me.m_man.Update()
                Me.m_bIsChanged = False
            End If

            RemoveHandler Me.m_gridConnections.OnSelectionChanged, AddressOf OnSelectDS
            Me.UIContext = Nothing
            MyBase.OnFormClosed(e)

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                    components = Nothing
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Form overrides

#Region " Control events "

#Region " Manage datasets "

        Private Sub OnManageConnections(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnDefineConnections.Click
            Try
                Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand("EditSpatialDatasets")
                If (cmd IsNot Nothing) Then
                    cmd.Invoke()
                    Me.FillSourceDatasetBox()
                    Me.UpdateControls()
                End If
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Manage connections 

#Region " Candidate connections "

        Private Sub OnDatasetFilterChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnFilter.CheckedChanged
            Try
                Me.FillSourceDatasetBox()
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnAddDataset(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAdd.Click, m_lbSourceDatasets.DoubleClick

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            Try
                Dim ds As ISpatialDataSet = Me.m_lbSourceDatasets.SelectedDataset
                If (ds IsNot Nothing) And (Me.m_iNumConn < cSpatialDataStructures.cMAX_CONN) Then
                    Me.m_bInUpdate = True
                    Me.m_iNumConn += 1
                    Me.m_adt.Dataset(m_iLayer, Me.m_iNumConn) = ds
                    Me.LayerChanged()
                    Me.m_bInUpdate = False

                    Me.m_gridConnections.Add(ds, True)
                End If
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
            Me.UpdateControls()

        End Sub

        Private Sub OnRemoveDataset(sender As System.Object, e As System.EventArgs) _
            Handles m_btnRemove.Click

            Dim iConn As Integer = Me.m_gridConnections.SelectedRow

            If (iConn < 1 Or iConn > Me.m_iNumConn) Then Return

            Me.m_bInUpdate = True
            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            Try
                For i As Integer = iConn To Me.m_iNumConn - 1
                    Me.m_adt.Dataset(Me.m_iLayer, i) = Me.m_adt.Dataset(Me.m_iLayer, i + 1)
                    Me.m_adt.Converter(Me.m_iLayer, i) = Me.m_adt.Converter(Me.m_iLayer, i + 1)
                    If (TypeOf m_adt Is cSpatialScalarDataAdapterBase) Then
                        With DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                            .DataScale(Me.m_iLayer, i) = .DataScale(Me.m_iLayer, i + 1)
                            .DataScaleType(Me.m_iLayer, i) = .DataScaleType(Me.m_iLayer, i + 1)
                        End With
                    End If
                Next
                Me.m_adt.Dataset(Me.m_iLayer, Me.m_iNumConn) = Nothing

                Me.m_iNumConn -= 1
                Me.LayerChanged()

                Me.m_gridConnections.Remove(Me.m_gridConnections.SelectedDataset)
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
            Me.UpdateControls()
            Me.m_bInUpdate = False

        End Sub

        Private Sub OnDatasetSelected(sender As System.Object, e As System.EventArgs) _
            Handles m_lbSourceDatasets.SelectedIndexChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Candidate connections

#Region " Selected connections "

        ''' <summary>
        ''' User has selected a dataset for the current adapter and layer.
        ''' </summary>
        Private Sub OnSelectDS(selection As SourceGrid2.CellVirtualCollection)

            If Me.m_bInUpdate Then Return
            Try
                Me.m_bInUpdate = True
                Me.SelectedDataset = Me.m_gridConnections.SelectedDataset
                Me.m_bInUpdate = False
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

#End Region ' Selected connections

#Region " Dataset config "

        ''' <summary>
        ''' User wants to configure the currently selected dataset.
        ''' </summary>
        Private Sub OnConfigDS(sender As System.Object, e As System.EventArgs) Handles m_btnConfigDS.Click

            Me.Cursor = Cursors.WaitCursor
            Try
                Dim iRow As Integer = Me.m_gridConnections.SelectedRow
                Me.ConfigDS(Me.SelectedDataset)
                Me.m_gridConnections.RefreshContent()
                Me.m_gridConnections.SelectRow(iRow)
                'Me.m_manSets.IndexDataset = Me.SelectedDataset
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucConficAdapter::OnConfigureDS")
            End Try
            Me.Cursor = Cursors.Default
            Me.LayerChanged()

        End Sub

#End Region ' Dataset config

#Region " Converters "

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
            Handles m_btnConfigCV.Click, m_btnConfigDS.Click
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

#End Region ' Converters

#Region " Scaling "

        Private Sub OnDatScaleTypeChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_rbAbsolute.CheckedChanged, m_rbRelative.CheckedChanged

            Dim iLayer As Integer = m_iLayer
            Dim iConn As Integer = Me.SelectedConnectionIndex

            If (Me.m_bInUpdate) Then Return
            If (iConn = -1) Then Return

            Try
                If (Me.m_bIsScaling) Then
                    With DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                        If (Me.m_rbAbsolute.Checked) Then
                            .DataScaleType(iLayer, iConn) = cSpatialScalarDataAdapterBase.eScaleType.Absolute
                        Else
                            .DataScaleType(iLayer, iConn) = cSpatialScalarDataAdapterBase.eScaleType.Relative
                        End If
                        Double.TryParse(Me.m_tbxScale.Text, .DataScale(iLayer, iConn))
                    End With

                    ' Invalidate the cached data for this dataset
                    ' ToDo_JS: Make dataset clearing more sublte. 
                    '          This statement deletes cached data for ALL scenarios a dataset is cached for. It should only
                    '          clear the cached data for the current Ecospace scenario. Oof. Ok, at least it works...
                    cSpatialDataCache.DefaultDataCache.Clear(Me.SelectedDataset)

                    Me.LayerChanged()

                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub OnEnterScaleTextBox(sender As Object, e As System.EventArgs) _
            Handles m_tbxScale.Enter
            Try
                Me.m_rbRelative.Checked = True
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub OnScaleChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbxScale.TextChanged, m_tbxScale.LostFocus

            If Me.m_bInUpdate Then Return

            Try
                If (Me.m_bIsScaling) Then
                    With DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                        ' ToDo: use format provider here
                        Double.TryParse(Me.m_tbxScale.Text, .DataScale(Me.m_iLayer, Me.SelectedConnectionIndex))
                    End With
                End If

                ' Invalidate the cached data for this dataset
                cSpatialDataCache.DefaultDataCache.Clear(Me.SelectedDataset)

                Me.LayerChanged()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub OnCalculateScale(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCalculate.Click

            Me.m_bInUpdate = True

            Try

                Debug.Assert(Me.m_bIsScaling)
                Debug.Assert(Me.SelectedConnectionIndex <> -1)

                ' Wait for indexing to stop
                'While Me.SelectedDataset.IsIndexing()
                ' JS: at least try to stop the indexing process
                Me.m_manSets.IndexDataset = Nothing
                'End While

                Me.UpdateControls()
                Dim ssda As cSpatialScalarDataAdapterBase = DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)

                Dim iStartTimeStep As Integer = Math.Max(1, Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(Me.SelectedDataset.TimeStart))
                Dim dtStartDate As DateTime = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iStartTimeStep)
                Dim dScale As Double = 1.0
                Dim msg As cMessage = Nothing
                Dim iConn As Integer = Me.SelectedConnectionIndex

                ' Perform calculation
                Select Case ssda.CalculateScaleFromEcopathTimePeriod(m_iLayer, iStartTimeStep, iConn, dScale)

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
                        With DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                            .DataScale(Me.m_iLayer, iConn) = dScale
                            .DataScaleType(Me.m_iLayer, iConn) = cSpatialScalarDataAdapterBase.eScaleType.Relative
                        End With

                End Select

                ' Got compatibility error message?
                If (msg IsNot Nothing) Then
                    Me.m_uic.Core.Messages.SendMessage(msg)
                End If

                Me.UpdateScalingPanel()

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

            Me.m_bInUpdate = False
            Me.UpdateControls()

        End Sub

#End Region ' Scaling

#Region " Other "

        Private Sub OnOK(sender As Object, e As System.EventArgs) Handles m_btnOK.Click

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

#End Region ' Other

#End Region ' Control events

#Region " Internals "

        Private Sub FillSourceDatasetBox()

            Dim vnFilter As eVarNameFlags = eVarNameFlags.NotSet
            If (Me.m_tsbnFilter.Checked) Then
                vnFilter = Me.m_adt.VarName
            End If
            Me.m_lbSourceDatasets.Filter = vnFilter

        End Sub

        Private Sub UpdateControls()

            Dim iLayer As Integer = m_iLayer
            Dim iConn As Integer = Me.SelectedConnectionIndex
            Dim bHasConnectionSelected As Boolean = (iConn <> -1)
            Dim ds As ISpatialDataSet = Nothing
            Dim cv As ISpatialDataConverter = Nothing
            Dim bCanConfigDS As Boolean = False
            Dim bCanConfigCV As Boolean = False
            Dim bIsConfigured As Boolean = False
            Dim bNeedsConverter As Boolean = False
            Dim bNeedsScaling As Boolean = False
            Dim bCanAddDS As Boolean = False
            Dim bCanRemoveDS As Boolean = False
            Dim comp As cDatasetCompatilibity.eCompatibilityTypes = cDatasetCompatilibity.eCompatibilityTypes.NotSet

            If (bHasConnectionSelected) Then
                ds = Me.m_adt.Dataset(iLayer, iConn)
                cv = Me.m_adt.Converter(iLayer, iConn)
            End If

            If (ds IsNot Nothing) Then

                bCanConfigDS = (TypeOf ds Is IConfigurable)
                bNeedsConverter = Not String.IsNullOrWhiteSpace(ds.ConversionFormat)
                bNeedsScaling = Me.m_bIsScaling
                bCanRemoveDS = True

                If (ds.IsConfigured) And (Not bNeedsConverter) Then
                    bIsConfigured = True
                Else
                    bIsConfigured = False
                    If (cv IsNot Nothing) Then
                        If cv.IsCompatible(ds) Then
                            bIsConfigured = cv.IsConfigured
                        End If
                    End If
                End If

                If (bIsConfigured And bNeedsScaling) Then
                    Dim worker As New cDatasetCompatilibity(Me.m_uic.Core, ds)
                    comp = worker.Compatibility
                End If
            End If


            bCanAddDS = (Me.m_gridConnections.RowsCount < cSpatialDataStructures.cMAX_CONN) And (Me.SourceDataset IsNot Nothing)

            If (cv IsNot Nothing) Then
                bCanConfigCV = bHasConnectionSelected And (TypeOf cv Is IConfigurable)
            End If

            Me.m_btnConfigDS.Enabled = bCanConfigDS

            Me.m_plDataset.Visible = bHasConnectionSelected

            Me.m_plConversion.Enabled = bHasConnectionSelected And bNeedsConverter
            Me.m_plConversion.Visible = bNeedsConverter
            Me.m_btnConfigCV.Enabled = bCanConfigCV
            Me.m_cmbConverter.Enabled = (Me.m_cmbConverter.Items.Count > 1)

            Me.m_plScalarAdapter.Enabled = bHasConnectionSelected And bNeedsScaling
            Me.m_plScalarAdapter.Visible = bNeedsScaling
            ' Allow calc of scaling even if spatial compatibility has not been assessed yet, for indexing may have been turned off
            Me.m_btnCalculate.Enabled = bNeedsScaling And bIsConfigured

            Me.m_btnAdd.Enabled = bCanAddDS
            Me.m_btnRemove.Enabled = bCanRemoveDS

        End Sub

        Private Sub UpdateDatasetPanel()

            Dim iConn As Integer = Me.SelectedConnectionIndex
            Dim ds As ISpatialDataSet = Nothing

            If (iConn > 0) Then

                Try

                    ds = Me.m_adt.Dataset(Me.m_iLayer, iConn)
                    Me.m_lblDatasetInfo.Text = ds.DisplayName

                    Dim comp As New cDatasetCompatilibity(Me.UIContext.Core, ds)
                    Dim fmt As New cSpatialDatasetCompatibilityFormatter()
                    Me.m_lblCompatibility.Text = fmt.Summary(comp)
                    Me.m_pbCompat.Image = cStyleGuide.GetImage(comp)

                Catch ex As Exception

                End Try

            Else
                Me.m_lblDatasetInfo.Text = ""
                Me.m_lblCompatibility.Text = ""
                Me.m_pbCompat.Image = Nothing
            End If

        End Sub

        ''' <summary>
        ''' Fill UI with converters compatible with the selected dataset.
        ''' </summary>
        Private Sub UpdateConversionPanel()

            Dim iConn As Integer = Me.SelectedConnectionIndex
            Dim ds As ISpatialDataSet = Nothing

            Me.m_cmbConverter.Items.Clear()

            If (iConn > 0) Then
                ds = Me.m_adt.Dataset(Me.m_iLayer, iConn)
                For Each cvTest As ISpatialDataConverter In Me.m_man.ConverterTemplates(ds)
                    If (Me.m_adt.Converter(Me.m_iLayer, iConn) Is Nothing) Then Me.m_adt.Converter(Me.m_iLayer, iConn) = cvTest
                    Me.m_cmbConverter.Items.Add(cvTest)
                Next
                Me.SelectConverter(Me.m_adt.Converter(Me.m_iLayer, iConn))
            End If

        End Sub

        ''' <summary>
        ''' Get the dataset selected that can be added to the current layer
        ''' </summary>
        Private ReadOnly Property SourceDataset As ISpatialDataSet
            Get
                Return Me.m_lbSourceDatasets.SelectedDataset
            End Get
        End Property

        Private Function ConfigConverter(cv As ISpatialDataConverter) As Boolean

            If (cv Is Nothing) Then Return False
            If (Not TypeOf cv Is IConfigurable) Then Return True

            If (TypeOf cv Is IPlugin) Then
                DirectCast(cv, IPlugin).Initialize(Me.m_uic.Core)
            End If

            Dim cvConf As IConfigurable = DirectCast(cv, IConfigurable)
            Dim ctrl As Control = cvConf.GetConfigUI()

            If (ctrl Is Nothing) Then Return cvConf.IsConfigured

            Dim dlg As New dlgConfig()
            dlg.UIContext = Me.UIContext
            dlg.ShowDialog(Me.FindForm, "Configure conversion", ctrl)

            Return (cvConf.IsConfigured)
        End Function

        Private Property SelectedDataset As ISpatialDataSet
            Get
                Return Me.m_gridConnections.SelectedDataset
            End Get
            Set(dataset As ISpatialDataSet)

                ' Apply
                Dim iConn As Integer = Me.SelectedConnectionIndex
                If (iConn > 0) Then
                    Me.m_adt.Dataset(Me.m_iLayer, iConn) = dataset
                    Me.m_manSets.IndexDataset = dataset
                End If

                Me.UpdateDatasetPanel()
                Me.UpdateConversionPanel()
                Me.UpdateScalingPanel()
                Me.UpdateControls()

            End Set
        End Property

        ''' <summary>
        ''' Get the one-based connection index
        ''' </summary>
        Private ReadOnly Property SelectedConnectionIndex As Integer
            Get
                Dim iRow As Integer = Me.m_gridConnections.SelectedRow
                If (iRow < 1) Or (iRow > Me.m_iNumConn) Then Return -1
                Return iRow
            End Get
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
                Return Me.m_adt.Converter(m_iLayer, Me.SelectedConnectionIndex)
            End Get
            Set(converter As ISpatialDataConverter)

                ' Apply
                If (Not Object.ReferenceEquals(Me.m_adt.Converter(m_iLayer, Me.SelectedConnectionIndex), converter)) Then
                    Me.m_adt.Converter(m_iLayer, Me.SelectedConnectionIndex) = converter
                    Me.LayerChanged()
                End If

            End Set
        End Property

        Private Function ConfigDS(ds As ISpatialDataSet) As Boolean

            If (ds Is Nothing) Then Return False
            If (Not TypeOf ds Is IConfigurable) Then Return True

            If (TypeOf ds Is IPlugin) Then
                DirectCast(ds, IPlugin).Initialize(Me.m_uic.Core)
            End If

            Dim dsConf As IConfigurable = DirectCast(ds, IConfigurable)
            Dim ctrl As Control = dsConf.GetConfigUI()

            If (ctrl Is Nothing) Then Return dsConf.IsConfigured

            Dim dlg As New dlgConfig()
            dlg.UIContext = Me.UIContext
            dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl)

            Return (dsConf.IsConfigured)

        End Function

        Private Sub LayerChanged()
            Me.m_bIsChanged = True
        End Sub

#Region " Scalar data adapter "

        Private Sub UpdateScalingPanel()

            If (Not Me.m_bIsScaling) Then Return

            Dim iConn As Integer = Me.SelectedConnectionIndex
            If (iConn <= 0) Then Return

            Dim bInUpdate As Boolean = Me.m_bInUpdate
            Me.m_bInUpdate = True

            With DirectCast(Me.m_adt, cSpatialScalarDataAdapterBase)
                Select Case .DataScaleType(Me.m_iLayer, iConn)
                    Case cSpatialScalarDataAdapterBase.eScaleType.Absolute
                        Me.m_rbAbsolute.Checked = True
                    Case cSpatialScalarDataAdapterBase.eScaleType.Relative
                        Me.m_rbRelative.Checked = True
                End Select
                ' ToDo: use format provider here
                Me.m_tbxScale.Text = CStr(.DataScale(Me.m_iLayer, iConn))
            End With

            Me.m_bInUpdate = bInUpdate

        End Sub

#End Region ' Scalar data adapter

#End Region ' Internals

#Region " Disabled bits that may come in handy again "

#If 0 Then

        Private Sub OnSaveStats(sender As System.Object, e As System.EventArgs)

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
#End If ' 0

#End Region ' Disabled bits that may come in handy again

    End Class

End Namespace ' Ecospace.Controls
