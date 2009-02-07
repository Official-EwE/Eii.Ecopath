'==============================================================================
'
' $Log: FlowDiagram.vb,v $
' Revision 1.5  2009/02/07 17:48:39  jeroens
' cINIFile moved
'
' Revision 1.4  2009/02/05 21:11:55  jeroens
' Labels can be dragged
'
' Revision 1.3  2008/11/21 23:06:15  sherman
' Fixed bugs: 550
' - Added listeners to properties, changed text names, made scaling more rhobust.
'
' Revision 1.2  2008/11/08 23:52:37  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:28  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Win32Api
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports System.Drawing
Imports System.Reflection

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    Public Class FlowDiagram
        : Inherits frmEwE

#Region " Private variables "

        Private components As System.ComponentModel.IContainer = Nothing
        Private m_FDData As FlowDiagramData = Nothing
        Private m_FDDraw As cFlowDiagramRenderer = Nothing

        '' Double buffer Image
        Private m_bmpOffScreen As Bitmap = Nothing
        Private m_gOffScreen As Graphics = Nothing

        Private m_bMouseDown As Boolean = False
        Private WithEvents m_timer As System.Windows.Forms.Timer
        Private WithEvents FDPictBox As System.Windows.Forms.PictureBox
        Private WithEvents m_menuContext As System.Windows.Forms.ContextMenuStrip
        Private WithEvents m_tsiSaveFile As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsiLoadFile As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsiSettings As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsiSaveImage As System.Windows.Forms.ToolStripMenuItem

        Private FDSettingsForm As FlowDiagramSettings = Nothing

#End Region ' Private variables

#Region " Constructor/Destructor "

        Public Sub New()

            InitializeComponent()

            ' This draws the control whenever it is resized
            SetStyle(ControlStyles.ResizeRedraw, True)
            ' This supports mouse movement such as the mouse wheel
            SetStyle(ControlStyles.UserMouse, True)
            ' This allows the control to be transparent
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)

            ' This updates the styles
            Me.UpdateStyles()

            ' Read the flowdiagram data
            m_FDData = New FlowDiagramData()

            'Initialize Draw
            m_FDDraw = New cFlowDiagramRenderer(m_FDData, Me.Height, Me.Width)
            AddHandler m_FDDraw.ForceRedraw, AddressOf RaiseForceRedraw

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath}
        End Sub

        Public Sub New(ByVal text As String)
            Me.New()
            'Set tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text
        End Sub

        Protected Overrides Sub Finalize()
            RemoveHandler m_FDDraw.ForceRedraw, AddressOf RaiseForceRedraw
            Me.CoreComponents = Nothing
            MyBase.Finalize()
        End Sub

#End Region ' Constructor/Destructor 

#Region " Events "

        Private Sub FlowDiagram_Resize(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Resize
            If Not m_FDDraw Is Nothing Then
                m_FDDraw.ResizeGraph(Me.Height, Me.Width)
            End If
        End Sub

#Region " Drawing "

        Private Sub FlowDiagram_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint

            m_bmpOffScreen = New Bitmap(FDPictBox.Width, FDPictBox.Height)
            m_gOffScreen = Graphics.FromImage(m_bmpOffScreen)
            m_gOffScreen.FillRectangle(New System.Drawing.SolidBrush(Color.White), 0, 0, FDPictBox.Width, FDPictBox.Height)

            m_FDDraw.DrawDots(m_gOffScreen, m_FDData)

            m_gOffScreen.Dispose()
            FDPictBox.Image = m_bmpOffScreen

        End Sub

        '' Overrides the paint routine so it elimates the flicker
        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
        End Sub

#End Region ' Drawing

#Region "Mouse Events"

        Private Sub FDPictBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles FDPictBox.MouseDown

            Using g As Graphics = Me.CreateGraphics()
                Me.m_FDDraw.BeginDrag(Me.m_FDData, e.Location, g)
            End Using

        End Sub

        Private Sub FDPictBox_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles FDPictBox.MouseUp
            Me.m_FDDraw.EndDrag(Me.m_FDData, e.Location)
        End Sub

        Private Sub FDPictBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles FDPictBox.MouseMove

            Using g As Graphics = Me.CreateGraphics()
                Me.m_FDDraw.ProcessMouseMove(g, Me.m_FDData, e.Location)
                Me.Invalidate()
            End Using

        End Sub

#End Region

#Region " Context Menu "

        Private Sub OnLoadFromFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsiLoadFile.Click

            Dim ifData As cINIFile = Nothing
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(My.Resources.FILEFILTER_FLOWDIAGRAM, 1)

            If (cmdFO.Result = DialogResult.OK) Then
                Try
                    ifData = New cINIFile(cmdFO.FileName)
                    m_FDDraw.LoadFromFile(ifData, Me.m_FDData)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to load from file '{0}': {1}", cmdFO.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If

        End Sub

        Private Sub OnSaveToFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsiSaveFile.Click

            Dim ifData As cINIFile = Nothing
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_FLOWDIAGRAM, 1)

            If cmdFS.Result = Windows.Forms.DialogResult.OK Then
                Try
                    ifData = New cINIFile(cmdFS.FileName)
                    m_FDDraw.SaveToFile(ifData, Me.m_FDData)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to save to file {0}: {1}", cmdFS.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If
        End Sub

        Private Sub OnSettings(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsiSettings.Click
            FDSettingsForm = New FlowDiagramSettings
            FDSettingsForm.FDPropertyGrid.SelectedObject = m_FDDraw.m_treeGraph
            FDSettingsForm.Show()
        End Sub

        Private Sub OnSaveToImage(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_tsiSaveImage.Click

            Dim fmt As Imaging.ImageFormat = Imaging.ImageFormat.Bmp
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_IMAGE)

            If cmdFS.Result = DialogResult.OK Then

                Select Case cmdFS.FilterIndex
                    Case 0
                        fmt = Imaging.ImageFormat.Bmp
                    Case 1
                        fmt = Imaging.ImageFormat.Jpeg
                    Case 2
                        fmt = Imaging.ImageFormat.Gif
                    Case 3
                        fmt = Imaging.ImageFormat.Png
                    Case 4
                        fmt = Imaging.ImageFormat.Tiff
                End Select

                Try
                    Me.m_bmpOffScreen.Save(cmdFS.FileName, fmt)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to save to file {0}: {1}", cmdFS.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If

        End Sub

#End Region

#End Region ' Events

#Region " Internals "

        Private Sub RaiseForceRedraw()
            Invalidate()
        End Sub

        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FlowDiagram))
            Me.m_timer = New System.Windows.Forms.Timer(Me.components)
            Me.FDPictBox = New System.Windows.Forms.PictureBox
            Me.m_menuContext = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.m_tsiLoadFile = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsiSaveFile = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsiSaveImage = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsiSettings = New System.Windows.Forms.ToolStripMenuItem
            CType(Me.FDPictBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_menuContext.SuspendLayout()
            Me.SuspendLayout()
            '
            'FDPictBox
            '
            Me.FDPictBox.ContextMenuStrip = Me.m_menuContext
            resources.ApplyResources(Me.FDPictBox, "FDPictBox")
            Me.FDPictBox.Name = "FDPictBox"
            Me.FDPictBox.TabStop = False
            '
            'm_menuContext
            '
            Me.m_menuContext.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsiLoadFile, Me.m_tsiSaveFile, Me.m_tsiSaveImage, Me.ToolStripSeparator1, Me.m_tsiSettings})
            Me.m_menuContext.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.m_menuContext, "m_menuContext")
            '
            'm_tsiLoadFile
            '
            Me.m_tsiLoadFile.Name = "m_tsiLoadFile"
            resources.ApplyResources(Me.m_tsiLoadFile, "m_tsiLoadFile")
            '
            'm_tsiSaveFile
            '
            Me.m_tsiSaveFile.Name = "m_tsiSaveFile"
            resources.ApplyResources(Me.m_tsiSaveFile, "m_tsiSaveFile")
            '
            'm_tsiSaveImage
            '
            Me.m_tsiSaveImage.Name = "m_tsiSaveImage"
            resources.ApplyResources(Me.m_tsiSaveImage, "m_tsiSaveImage")
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsiSettings
            '
            Me.m_tsiSettings.Name = "m_tsiSettings"
            resources.ApplyResources(Me.m_tsiSettings, "m_tsiSettings")
            '
            'FlowDiagram
            '
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.FDPictBox)
            Me.Name = "FlowDiagram"
            CType(Me.FDPictBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_menuContext.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

#End Region ' Internals

    End Class


End Namespace