Option Strict On
Imports EwECore
Imports EwEUtils.SpatialData
Imports EwEPlugin
Imports EwECore.SpatialData
Imports EwEUtils

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

        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing
        Private m_adt As cSpatialDataAdapter = Nothing
        Private m_layer As cEcospaceLayer = Nothing
        Private m_uic As cUIContext = Nothing
        Private m_bHasCachedData As Boolean = False

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Mandatory overrides etc "

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_adt = Nothing
                    Me.m_layer = Nothing
                    Me.m_manSets.Save()
                    Me.m_manSets = Nothing
                    Me.m_man = Nothing
                End If

                Me.m_uic = uic

                If (Me.m_uic IsNot Nothing) Then
                    Me.m_man = Me.m_uic.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
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
        ''' Set the connection to configure.
        ''' </summary>
        ''' <param name="adt"><see cref="cSpatialDataAdapter"/> to configure.</param>
        ''' <param name="layer"><see cref="cEcospaceLayer"/> to configure.</param>
        ''' -------------------------------------------------------------------
        Public Sub SetConnection(adt As cSpatialDataAdapter, layer As cEcospaceLayer)
            ' Store refs
            Me.m_adt = adt
            Me.m_layer = layer
            ' Set initials
            If (adt IsNot Nothing) And (layer IsNot Nothing) Then
                Me.SelectedDS = adt.Dataset(layer.Index)
                Me.SelectedConverter = adt.Converter(layer.Index)
            End If
            ' Done
            Me.UpdateControls()
        End Sub

#End Region ' Public bits

#Region " Control events "

        Private Sub OnDatasetTemplateSelected(sender As Object, e As System.EventArgs) _
            Handles m_cmbNewDS.SelectedIndexChanged
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
            Try
                Me.CreateDS()
            Catch ex As Exception

            End Try
        End Sub

        ''' <summary>
        ''' User has selected a dataset for the current adapter and layer.
        ''' </summary>
        Private Sub OnSelectDS(sender As System.Object, e As System.EventArgs) _
            Handles m_lbxExistingDS.SelectedIndexChanged
            Try
                Dim obj As Object = Me.m_lbxExistingDS.SelectedItem
                If String.Empty.Equals(obj) Then
                    Me.SelectedDS = Nothing
                Else
                    Me.SelectedDS = DirectCast(obj, ISpatialDataSet)
                End If
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        ''' <summary>
        ''' User wants to configure the currently selected dataset.
        ''' </summary>
        Private Sub OnConfigDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigDS.Click

            Me.Cursor = Cursors.WaitCursor
            Try
                Me.ConfigDS(Me.SelectedDS)
                Me.FillExistingDatasetBox()
            Catch ex As Exception

            End Try
            Me.Cursor = Cursors.Default

        End Sub

        ''' <summary>
        ''' User wants to delete the selected dataset
        ''' </summary>
        Private Sub OnDeleteDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDeleteDS.Click
            Try
                Me.DeleteDS(Me.SelectedDS)
            Catch ex As Exception

            End Try
        End Sub

        ''' <summary>
        ''' User wants to clear the spatial data cache.
        ''' </summary>
        Private Sub OnClearCache(sender As System.Object, e As System.EventArgs) _
            Handles m_btnClearCache.Click

            ' ToDo: globalize this

            Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
            Dim dSizeTot As Double = cache.GetSize() / 1024
            Dim dSizeUnused As Double = cache.GetUnusedSize(Me.m_manSets) / 1024
            Dim strPrompt As String = My.Resources.PROMPT_CACHE_CLEAR
            Dim bSucces As Boolean = True

            Try
                If (dSizeUnused > 0) Then
                    Select Case MsgBox(String.Format(strPrompt, Me.m_uic.StyleGuide.FormatNumber(dSizeTot), Me.m_uic.StyleGuide.FormatNumber(dSizeUnused)), _
                                           MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                        Case MsgBoxResult.Yes
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear(Me.m_manSets)
                        Case MsgBoxResult.No
                            bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                        Case MsgBoxResult.Cancel
                    End Select
                Else
                    bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                End If
            Catch ex As Exception
                bSucces = False
            End Try

            Dim dSizeTot2 As Double = cache.GetSize() / 1024
            Dim msg As New cMessage(String.Format("Spatial data cache cleared, freed {0} kb of data", Me.m_uic.StyleGuide.FormatNumber(dSizeTot - dSizeTot2)), eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)

            ' Reflect new state
            Me.EvaluateCache()
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' Event handler for customizing how converters are displayed in this UI.
        ''' </summary>
        Private Sub OnFormatCV(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbxExistingConv.Format
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

            End Try
        End Sub

        ''' <summary>
        ''' User has selected a converter for the current adapter and layer.
        ''' </summary>
        Private Sub OnSelectCV(sender As System.Object, e As System.EventArgs) _
            Handles m_lbxExistingConv.SelectedIndexChanged
            Try
                Dim obj As Object = Me.m_lbxExistingConv.SelectedItem
                If String.Empty.Equals(obj) Then
                    Me.SelectedConverter = Nothing
                Else
                    Me.SelectedConverter = DirectCast(obj, ISpatialDataConverter)
                End If
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Control events

#Region " Internals "

        Private Sub UpdateControls()

            Dim bIsConnected As Boolean = (Me.m_adt IsNot Nothing) And (Me.m_layer IsNot Nothing)
            Dim ds As ISpatialDataSet = Me.SelectedDS
            Dim cv As ISpatialDataConverter = Me.SelectedConverter
            Dim bCanConfigDS As Boolean = False
            Dim bCanConfigCV As Boolean = False

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
            Me.m_lbxExistingConv.Enabled = bIsConnected

        End Sub

        Private Sub FillTemplateDatasetBox()

            Me.m_cmbNewDS.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_man.DatasetTemplates
                Me.m_cmbNewDS.Items.Add(ds)
            Next
            If (Me.m_cmbNewDS.Items.Count > 0) Then
                Me.m_cmbNewDS.SelectedItem = 0
            End If

        End Sub

        Private Sub FillExistingDatasetBox(Optional ds As ISpatialDataSet = Nothing)

            If (ds Is Nothing) Then ds = Me.SelectedDS
            Me.m_lbxExistingDS.Items.Clear()
            Me.m_lbxExistingDS.Items.Add("")
            For i As Integer = 0 To Me.m_manSets.Count - 1
                Me.m_lbxExistingDS.Items.Add(Me.m_manSets(i))
            Next
            Me.SelectedDS = ds

        End Sub

        Private Sub FillExistingConverterBox(Optional cv As ISpatialDataConverter = Nothing)

            If (cv Is Nothing) Then cv = Me.SelectedConverter
            Me.m_lbxExistingConv.Items.Clear()
            Me.m_lbxExistingConv.Items.Add("")
            For Each cvTest As ISpatialDataConverter In Me.m_man.ConverterTemplates
                Me.m_lbxExistingConv.Items.Add(cvTest)
            Next
            Me.SelectedConverter = cv
        End Sub

        Private Sub ConfigConverter(cv As ISpatialDataConverter)
            ' NOP
        End Sub

        Private Property SelectedDS As ISpatialDataSet
            Get
                If (Me.m_adt Is Nothing) Then Return Nothing
                Return Me.m_adt.Dataset(Me.m_layer.Index)
            End Get
            Set(dataset As ISpatialDataSet)

                If (Me.m_adt Is Nothing) Then Return

                ' Update selection
                Dim iIndex As Integer = 0
                If (dataset IsNot Nothing) Then
                    iIndex = Me.m_lbxExistingDS.Items.IndexOf(dataset)
                End If
                Me.m_lbxExistingDS.SelectedIndex = iIndex

                ' Apply
                If (Not Object.ReferenceEquals(Me.m_adt.Dataset(Me.m_layer.Index), dataset)) Then
                    Me.m_adt.Dataset(Me.m_layer.Index) = dataset
                    Me.LayerChanged()
                End If

            End Set
        End Property

        Private Property SelectedConverter As ISpatialDataConverter
            Get
                If (Me.m_adt Is Nothing) Then Return Nothing
                Return Me.m_adt.Converter(Me.m_layer.Index)
            End Get
            Set(converter As ISpatialDataConverter)

                If (Me.m_adt Is Nothing) Then Return

                ' Update selection
                Dim iIndex As Integer = 0
                If (converter IsNot Nothing) Then
                    iIndex = Me.m_lbxExistingConv.Items.IndexOf(converter)
                End If
                Me.m_lbxExistingConv.SelectedIndex = iIndex

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
            If (Me.m_adt Is Nothing) Then Return

            dsNew = CType(Activator.CreateInstance(dsSelected.GetType()), ISpatialDataSet)
            If (dsNew Is Nothing) Then Return

            If Me.ConfigDS(dsNew) Then
                Me.m_manSets.Add(dsNew)
                Me.FillExistingDatasetBox(dsNew)
            End If

        End Sub

        Private Function ConfigDS(ds As ISpatialDataSet) As Boolean

            ' ToDo: Globalize this

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
            Me.SelectedDS = Nothing
            Me.m_manSets.Remove(ds)
            Me.FillExistingDatasetBox()
        End Sub

        Private Sub LayerChanged()

            If (Me.m_uic Is Nothing) Then Return
            If (Me.m_adt Is Nothing) Then Return

            Me.m_man.Update()
            Me.m_uic.Core.onChanged(Me.m_layer)

        End Sub

        Private Sub EvaluateCache()
            Me.m_bHasCachedData = (cSpatialDataCache.DefaultDataCache.GetSize > 0)
        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls
