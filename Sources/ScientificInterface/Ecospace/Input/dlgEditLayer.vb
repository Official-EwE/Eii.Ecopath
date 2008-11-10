'==============================================================================
'
' $Log: dlgEditLayer.vb,v $
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
        Public Sub New(ByRef layer As cLayer, ByVal layerDepth As cLayer, ByVal openType As eOpenDialogTypes)
            InitializeComponent()

            ' Who needs sanity?
            Debug.Assert(layer IsNot Nothing)

            ' Set the references
            Me.m_core = cCore.GetInstance()

            Me.m_layerOriginal = layer
            Me.m_layerWork = New cLayer(layer) ' Work on a clone

            ' Hook up preview Layer to user control
            Me.m_ucZoomControl = New ucZoomBaseMap()
            Me.m_ucZoomControl.Dock = DockStyle.Fill

            Me.m_ucPreview = Me.m_ucZoomControl.Map()
            Me.m_ucPreview.Basemap = Me.m_core.EcospaceBasemap
            Me.m_ucPreview.AddLayer(Me.m_layerWork)
            If ((Not Object.ReferenceEquals(layer, layerDepth)) And _
                (Not Object.ReferenceEquals(layerDepth, Nothing))) Then
                Me.m_ucPreview.AddLayer(layerDepth)
            End If

            ' Add basemap to Panel
            Me.pnBasemap.Controls.Add(Me.m_ucZoomControl)
            Me.m_ucZoomControl.PositionMode = ucZoomBaseMap.ePositionModeTypes.Center
            Me.m_ucZoomControl.Zoom(ucZoomBaseMap.eZoomTypes.ZoomReset)

            ' Show your stuff
            Me.LoadLayer()
            Me.UpdateControls()
            Me.DrawPreview()

            Me.m_tcLayerView.SelectedIndex = CInt(openType)
        End Sub

#End Region ' Constructors

#Region " Local events "

        Private Sub DataLayerDialog_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            Me.m_fpName = Nothing
            Me.m_fpWeight = Nothing
            Me.m_fpDescription = Nothing
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
            If Not Me.ApplyChanges() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub Apply_Button_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Apply_Button.Click
            Me.ApplyChanges()
        End Sub

        Private Sub OnVisualStyleChanged(ByVal sender As Controls.ucEditVisualStyle)
            ' Update work layer Visual Style
            Me.m_ucEditVisualStyle.Apply(Me.m_layerWork.Renderer.VisualStyle)
            Me.m_layerWork.Update(cLayer.eChangeFlags.VisualStyle)
        End Sub

        Private Sub OnImportData(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnDataImport.Click
            Me.ImportData()
        End Sub

        Private Sub OnExportData(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnDataExport.Click
            Me.ExportData()
        End Sub

#End Region ' Local events

#Region " Internal implementation "

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

            If (HasUniqueSource()) Then
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
                AddHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged

            End If

            Me.m_grid.Layer = Me.m_layerWork

            Me.m_tlpDetails.ResumeLayout()

        End Sub

        Private Sub DrawPreview()
            m_ucPreview.Refresh()
        End Sub

        Private Sub UpdateControls()
            'Dim bHasName As Boolean = Not String.IsNullOrEmpty(Me.txtName.Text)
            'Me.OK_Button.Enabled = bHasName
            'Me.Apply_Button.Enabled = bHasName
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

#Region " Data handling - To be elaborated "

        Private Sub ImportData()

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("ImportLayerData")

            If cmd IsNot Nothing Then
                cmd.Tag = Me.m_layerWork
                cmd.Invoke()
            End If

        End Sub

        Private Sub ExportData()

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_CSV)
            If cmdFS.Result = Windows.Forms.DialogResult.OK Then
                Me.SaveCSVFile(cmdFS.FileName)
            End If

        End Sub

        Private Function SaveCSVFile(ByVal strFile As String) As Boolean

            Dim tw As TextWriter = New StreamWriter(strFile)
            Dim strLine As String = ""
            Dim sb As New StringBuilder
            Dim irow, icol As Integer
            Dim data As cEcospaceLayer = Me.m_layerWork.Data

            irow = 1
            For irow = 1 To data.InRow
                For icol = 1 To data.InCol
                    If icol > 1 Then sb.Append(", ")
                    sb.Append(data.Cell(irow, icol))
                Next icol
                sb.AppendLine()
            Next irow

            tw.Write(sb.ToString())
            tw.Close()
            Return True

        End Function

#End Region ' Data handling - To be elaborated

    End Class

End Namespace