Option Strict On
Imports EwECore
Imports EwEUtils.SpatialData
Imports EwEPlugin
Imports EwECore.SpatialData
Imports EwEUtils

Namespace Ecospace.Controls

    Public Class ucConfigAdapter
        Implements IUIElement
        Implements IDisposable

        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing
        Private m_adt As cSpatialDataAdapter = Nothing
        Private m_layer As cEcospaceLayer = Nothing
        Private m_uic As cUIContext = Nothing
        Private m_bHasCachedData As Boolean = False

        Public Sub New()
            Me.InitializeComponent()
        End Sub

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

        Public Sub SetConnection(adt As cSpatialDataAdapter, layer As cEcospaceLayer)
            Me.m_adt = adt
            Me.m_layer = layer
            Me.PopulateControls()
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.UIContext Is Nothing) Then Return
            Me.EvaluateCache()
            Me.FillTemplateDatasetBox()
            Me.UpdateControls()
        End Sub

#Region " Control events "

        Private Sub OnFormatDS(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbxExistingDS.Format, m_cmbNewDS.Format
            Dim fmt As New cSpatialDatasetFormatter()
            If e.ListItem.Equals(String.Empty) Then
                e.Value = fmt.GetDescriptor(Nothing)
            Else
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        End Sub

        Private Sub OnCreateDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCreateDS.Click
            Try
                Me.CreateDS()
            Catch ex As Exception

            End Try
        End Sub

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

        Private Sub OnConfigDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigDS.Click
            Try
                Me.ConfigDS(Me.SelectedDS)
                Me.FillExistingDatasetBox()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnDeleteDS(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDeleteDS.Click
            Try
                Me.DeleteDS(Me.SelectedDS)
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnClearCache(sender As System.Object, e As System.EventArgs) _
            Handles m_btnClearCache.Click

            Dim cache As cSpatialDataCache = cSpatialDataCache.DefaultDataCache
            Dim dSizeTot As Double = cache.GetSize() / 1024
            Dim dSizeUnused As Double = cache.GetUnusedSize(Me.m_manSets) / 1024
            Dim strPrompt As String = My.Resources.PROMPT_CACHE_CLEAR
            Dim bSucces As Boolean = True

            Try
                Select Case MsgBox(String.Format(strPrompt, Me.m_uic.StyleGuide.FormatNumber(dSizeTot), Me.m_uic.StyleGuide.FormatNumber(dSizeUnused)), _
                                   MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Yes
                        bSucces = cSpatialDataCache.DefaultDataCache.Clear(Me.m_manSets)
                    Case MsgBoxResult.No
                        bSucces = cSpatialDataCache.DefaultDataCache.Clear()
                    Case MsgBoxResult.Cancel
                End Select
            Catch ex As Exception
                bSucces = False
            End Try

            ' Reflect new state
            Me.EvaluateCache()
            Me.UpdateControls()

            If Not bSucces Then

            End If
        End Sub

        Private Sub OnFormatCV(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_lbxExistingConv.Format
            Dim fmt As New cSpatialConverterFormatter()
            If e.ListItem.Equals(String.Empty) Then
                e.Value = fmt.GetDescriptor(Nothing)
            Else
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        End Sub

        Private Sub OnConfigCV(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConfigureCV.Click
            Try
                Me.ConfigConverter(Me.SelectedConverter)
            Catch ex As Exception

            End Try
        End Sub

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

            Me.Enabled = (Me.m_adt IsNot Nothing)
            If (Not Me.Enabled) Then Return

            Dim ds As ISpatialDataSet = Me.SelectedDS
            Dim cv As ISpatialDataConverter = Me.SelectedConverter
            Dim bCanConfigDS As Boolean = False
            Dim bCanConfigCV As Boolean = False

            If (ds IsNot Nothing) Then bCanConfigDS = (TypeOf ds Is IConfigurablePlugin)
            If (cv IsNot Nothing) Then bCanConfigCV = (TypeOf cv Is IConfigurablePlugin)

            Me.m_btnCreateDS.Enabled = (Me.m_cmbNewDS.SelectedIndex >= 0)

            Me.m_btnConfigDS.Enabled = bCanConfigDS
            Me.m_btnDeleteDS.Enabled = (ds IsNot Nothing)

            Me.m_btnConfigureCV.Enabled = bCanConfigCV

            Me.m_btnClearCache.Enabled = (Me.m_bHasCachedData = True)

            Dim fmt As New cVarnameTypeFormatter()

            If (Me.m_adt Is Nothing) Then
                Me.m_hdrSource.Text = My.Resources.CAPTION_EXTERNAL_DATA
            Else
                Me.m_hdrSource.Text = String.Format(My.Resources.CAPTION_EXTERNAL_DATA_DETAIL, fmt.GetDescriptor(Me.m_adt.VarName))
            End If

        End Sub

        Private Sub PopulateControls()

            Debug.Assert(Me.m_uic IsNot Nothing)

            Me.FillExistingDatasetBox(Me.SelectedDS)
            Me.FillExistingConverterBox()

        End Sub

        Private Sub FillTemplateDatasetBox()

            Dim pm As cPluginManager = Me.m_uic.Core.PluginManager

            Me.m_cmbNewDS.Items.Clear()
            For Each ds As ISpatialDataSet In Me.m_man.DatasetTemplates
                Me.m_cmbNewDS.Items.Add(ds)
            Next
            If (Me.m_cmbNewDS.Items.Count > 0) Then
                Me.m_cmbNewDS.SelectedIndex = 0
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
                Me.m_adt.Dataset(Me.m_layer.Index) = dataset
                Dim iIndex As Integer = 0
                If (dataset IsNot Nothing) Then
                    iIndex = Me.m_lbxExistingDS.Items.IndexOf(dataset)
                End If
                Me.m_lbxExistingDS.SelectedIndex = iIndex
                Me.LayerChanged()
            End Set
        End Property

        Private Property SelectedConverter As ISpatialDataConverter
            Get
                If (Me.m_adt Is Nothing) Then Return Nothing
                Return Me.m_adt.Converter(Me.m_layer.Index)
            End Get
            Set(converter As ISpatialDataConverter)
                If (Me.m_adt Is Nothing) Then Return
                Me.m_adt.Converter(Me.m_layer.Index) = converter
                Dim iIndex As Integer = 0
                If (converter IsNot Nothing) Then
                    iIndex = Me.m_lbxExistingConv.Items.IndexOf(converter)
                End If
                Me.m_lbxExistingConv.SelectedIndex = iIndex
                Me.LayerChanged()
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

            Me.m_uic.Core.onChanged(Me.m_layer)

        End Sub

        Private Sub EvaluateCache()
            Me.m_bHasCachedData = (cSpatialDataCache.DefaultDataCache.GetSize > 0)
        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls
