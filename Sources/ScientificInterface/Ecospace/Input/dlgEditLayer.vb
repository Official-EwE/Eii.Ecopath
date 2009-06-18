'==============================================================================
'
' $Log: dlgEditLayer.vb,v $
' Revision 1.10  2009/06/18 04:57:16  jeroens
' Fixed update bug after layer import
'
' Revision 1.9  2009/05/15 14:19:08  jeroens
' Work layer disposed
'
' Revision 1.8  2009/05/11 01:50:48  jeroens
' Renamed command classes
'
' Revision 1.7  2009/03/19 16:02:26  jeroens
' Added FormatProvider.Release
'
' Revision 1.6  2008/11/20 15:18:29  jeroens
' Layer ReadOnly state properly handled
'
' Revision 1.5  2008/11/10 22:12:02  jeroens
' Uses import and export dialogs
'
' Revision 1.4  2008/11/10 02:25:15  jeroens
' Uses external command to import data
'
' Revision 1.3  2008/11/10 01:49:57  jeroens
' Renamed resources
'
' Revision 1.2  2008/11/08 23:53:05  jeroens
' Renamed file commands
'
' Revision 1.1  2008/11/04 04:48:35  jeroens
' Renamed
'
' Revision 1.2  2008/10/10 18:04:02  jeroens
' Updated to renamed layers classes
'
' Revision 1.1  2008/09/26 07:31:59  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.IO
Imports System.Text
Imports SAUPUtil.SAUPFile
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.SAUPData
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Document purpose of this dialog
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgEditLayer

        Private m_core As cCore = Nothing

        ''' <summary>Original layer this dialog was invoked for.</summary>
        Private m_layerOriginal As cLayer = Nothing
        Private m_layerDepth As cLayer = Nothing
        Private m_openType As eOpenDialogTypes

        ''' <summary>Work layer (a copy of the original) for this dialog to work on.</summary>
        Private m_layerWork As cLayer = Nothing
        '''' <summary>Basemap zoom wrapper</summary>
        Private m_ucZoomControl As ucZoomBaseMap = Nothing
        '''' <summary>Preview pane</summary>
        Private m_ucPreview As ucBaseMap = Nothing
        ''' <summary>Editor to transmogrify the representation of the layer.</summary>
        Private m_ucEditVisualStyle As ucEditVisualStyle = Nothing

        Private m_fpName As cEwEFormatProvider = Nothing
        Private m_fpWeight As cEwEFormatProvider = Nothing
        Private m_fpDescription As cEwEFormatProvider = Nothing

        Public Enum eOpenDialogTypes As Integer
            Appearance = 0
            Data
        End Enum

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="layer"></param>
        ''' <param name="layerDepth"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByRef layer As cLayer, ByVal layerDepth As cLayer, ByVal opentype As eOpenDialogTypes)
            InitializeComponent()

            ' Who needs sanity?
            Debug.Assert(layer IsNot Nothing)

            ' Set the references
            Me.m_core = cCore.GetInstance()

            Me.m_layerOriginal = layer
            Me.m_layerDepth = layerDepth
            Me.m_openType = opentype

            Me.m_layerWork = New cLayer(layer) ' Work on a clone

        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Hook up preview Layer to user control
            Me.m_ucZoomControl = New ucZoomBaseMap()
            Me.m_ucZoomControl.Dock = DockStyle.Fill

            Me.m_ucPreview = Me.m_ucZoomControl.Map()
            Me.m_ucPreview.Basemap = Me.m_core.EcospaceBasemap
            Me.m_ucPreview.AddLayer(Me.m_layerWork)
            If ((Not Object.ReferenceEquals(Me.m_layerOriginal, Me.m_layerDepth)) And _
                (Not Object.ReferenceEquals(Me.m_layerDepth, Nothing))) Then
                Me.m_ucPreview.AddLayer(Me.m_layerDepth)
            End If

            ' Add basemap to Panel
            Me.pnBasemap.Controls.Add(Me.m_ucZoomControl)
            Me.m_ucZoomControl.PositionMode = ucZoomBaseMap.ePositionModeTypes.Center
            Me.m_ucZoomControl.Zoom(ucZoomBaseMap.eZoomTypes.ZoomReset)

            ' Show your stuff
            Me.LoadLayer()
            Me.UpdateControls()
            Me.DrawPreview()

            Me.m_tcLayerView.SelectedIndex = CInt(Me.m_openType)

            If Me.m_ucEditVisualStyle IsNot Nothing Then
                AddHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

            If Me.m_ucEditVisualStyle IsNot Nothing Then
                RemoveHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

            Me.m_fpName.Release()
            Me.m_fpWeight.Release()
            Me.m_fpDescription.Release()

            Me.m_layerDepth = Nothing
            Me.m_layerOriginal = Nothing
            Me.m_layerWork.Dispose()
            Me.m_layerWork = Nothing

            MyBase.OnFormClosing(e)

        End Sub

#End Region ' Overrides

#Region " Local events "

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            If Not Me.ApplyChanges() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub Apply_Button_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Apply_Button.Click

            Me.ApplyChanges()

        End Sub

        Private Sub OnVisualStyleChanged(ByVal sender As Controls.ucEditVisualStyle)

            ' Update work layer Visual Style
            Me.m_ucEditVisualStyle.Apply(Me.m_layerWork.Renderer.VisualStyle)
            Me.m_layerWork.Update(cLayer.eChangeFlags.VisualStyle)

        End Sub

        Private Sub OnImportData(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnDataImport.Click

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = cmdh.GetCommand("ImportLayerData")

            If cmd IsNot Nothing Then
                cmd.Tag = New cLayer() {Me.m_layerWork}
                cmd.Invoke()
            End If
            Me.m_grid.RefreshContent()

        End Sub

        Private Sub OnExportData(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnDataExport.Click

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = cmdh.GetCommand("ExportLayerData")

            If cmd IsNot Nothing Then
                cmd.Tag = New cLayer() {Me.m_layerWork}
                cmd.Invoke()
            End If

        End Sub

#End Region ' Local events

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Diagnostic method, states if a layer has a unique core variable 
        ''' link. Layers with unique sources support extra's that can be stored
        ''' in the database such as remarks and visual styles.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function HasUniqueSource() As Boolean
            If (Me.m_layerOriginal.Source Is Nothing) Then Return False
            If (TypeOf Me.m_layerOriginal.Source Is cEcospaceBasemap) Then Return False
            Return True
        End Function

        Private Sub LoadLayer()

            Dim vs As cVisualStyle = Me.m_layerWork.Renderer.VisualStyle
            Dim src As cCoreInputOutputBase = Me.m_layerWork.Source

            Me.m_tlpDetails.SuspendLayout()

            Me.m_lblWeight.Visible = False
            Me.m_nudWeight.Visible = False
            Me.m_lblDescription.Visible = False
            Me.m_tbDescription.Visible = False

            Me.m_fpName = New cEwEFormatProvider(Me.m_tbNameValue, GetType(String))
            Me.m_fpWeight = New cEwEFormatProvider(Me.m_nudWeight, GetType(Single))
            Me.m_fpDescription = New cEwEFormatProvider(Me.m_tbNameValue, GetType(String))

            If (Me.HasUniqueSource()) Then
                Me.m_fpName.Enabled = True
                Me.m_tbRemarks.Text = src.Remark
                Me.m_tbRemarks.Enabled = True

                If TypeOf src Is cEcospaceLayerImportance Then
                    Me.m_lblWeight.Visible = True
                    Me.m_nudWeight.Visible = True
                    Me.m_lblDescription.Visible = True
                    Me.m_tbDescription.Visible = True

                    Me.m_fpWeight.Value = src.GetVariable(eVarNameFlags.ImportanceWeight)
                    Me.m_fpDescription.Value = src.GetVariable(eVarNameFlags.Description)
                End If
            Else
                Me.m_fpName.Enabled = False

                Me.m_tbRemarks.Text = "Remarks not supported for this layer"
                Me.m_tbRemarks.Enabled = False
            End If
            Me.m_fpName.Value = m_layerWork.Name

            Me.m_ucEditVisualStyle = ucEditVisualStyle.GetEditor(vs, Me.m_layerWork.Renderer.VisualStyleFlags)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                Me.m_ucEditVisualStyle.Dock = DockStyle.Fill
                Me.m_plEditVisualStyle.Controls.Add(Me.m_ucEditVisualStyle)
            End If

            Me.m_grid.Layer = Me.m_layerWork

            Me.m_tlpDetails.ResumeLayout()

        End Sub

        Private Sub DrawPreview()
            m_ucPreview.Refresh()
        End Sub

        Private Sub UpdateControls()

            Dim bEditable As Boolean = True

            If (Me.m_layerOriginal.Editor IsNot Nothing) Then
                bEditable = (Me.m_layerOriginal.Editor.IsReadOnly = False)
            End If

            Me.m_btnDataImport.Enabled = bEditable

        End Sub

        Private Function ApplyChanges() As Boolean

            Dim cf As cLayer.eChangeFlags = 0
            Dim src As cCoreInputOutputBase = Me.m_layerOriginal.Source

            If (HasUniqueSource()) Then

                Dim pm As cPropertyManager = cPropertyManager.GetInstance()
                Dim p As cProperty = pm.GetProperty(Me.m_layerOriginal.Source, eVarNameFlags.Name)

                If (p IsNot Nothing) Then
                    p.SetRemark(Me.m_tbRemarks.Text)
                    p.SetValue(CStr(Me.m_fpName.Value))
                End If

                If TypeOf Me.m_layerOriginal.Source Is cEcospaceLayerImportance Then
                    src.SetVariable(eVarNameFlags.ImportanceWeight, Me.m_fpWeight.Value)
                    src.SetVariable(eVarNameFlags.Description, Me.m_fpDescription.Value)
                End If

            End If

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                ' Apply changes
                Me.m_ucEditVisualStyle.Apply(Me.m_layerOriginal.Renderer.VisualStyle)
                cf = cf Or cLayer.eChangeFlags.VisualStyle
            End If

            If Me.m_grid.Apply(Me.m_layerOriginal) Then
                cf = cf Or cLayer.eChangeFlags.Map
            End If

            ' Fire layer changed notification
            Me.m_layerOriginal.Update(cf)

        End Function

#End Region ' Internal implementation

    End Class

End Namespace