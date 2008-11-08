'==============================================================================
'
' $Log: FlowDiagram.vb,v $
' Revision 1.2  2008/11/08 23:52:37  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:28  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.13  2008/09/09 14:44:52  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.12  2008/07/08 23:22:44  sherman
' Fixed max TL toggle and closing of Settings Dialogue bug: 422
'
' Revision 1.11  2008/07/04 16:13:12  jeroens
' Added header
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports WeifenLuo.WinFormsUI.Docking
Imports System.Reflection
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    Public Class FlowDiagram
        : Inherits DockContent

        Private components As System.ComponentModel.IContainer
        'Friend WithEvents FDData As New FlowDiagramData
        Dim m_FDData As FlowDiagramData = Nothing
        'Dim FDDataIO As New FlowDiagramDataIO
        Dim m_FDDraw As Draw = Nothing

        '' Double buffer Image
        Dim m_bmpOffScreen As Bitmap = Nothing
        Dim m_gOffScreen As Graphics = Nothing
        Dim m_gOnScreen As Graphics = Nothing

        Dim m_bMouseDown As Boolean = False
        Friend WithEvents Timer1 As System.Windows.Forms.Timer
        Friend WithEvents FDPictBox As System.Windows.Forms.PictureBox
        Friend WithEvents ctxmnuRightClick As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents SettingsToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents SaveAsImageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

        Dim FDSettingsForm As FlowDiagramSettings

        Public Sub New()

            ' This call is required by the Component Designer.
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
            m_FDDraw = New Draw(m_FDData, Me.Height, Me.Width)

        End Sub

        Public Sub New(ByVal text As String)
            Me.New()
            'Set tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text
        End Sub

        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FlowDiagram))
            Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
            Me.FDPictBox = New System.Windows.Forms.PictureBox
            Me.ctxmnuRightClick = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.SettingsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
            Me.SaveAsImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            CType(Me.FDPictBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ctxmnuRightClick.SuspendLayout()
            Me.SuspendLayout()
            '
            'FDPictBox
            '
            Me.FDPictBox.ContextMenuStrip = Me.ctxmnuRightClick
            resources.ApplyResources(Me.FDPictBox, "FDPictBox")
            Me.FDPictBox.Name = "FDPictBox"
            Me.FDPictBox.TabStop = False
            '
            'ctxmnuRightClick
            '
            Me.ctxmnuRightClick.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadToolStripMenuItem, Me.SaveToolStripMenuItem, Me.SaveAsImageToolStripMenuItem, Me.ToolStripSeparator1, Me.SettingsToolStripMenuItem1})
            Me.ctxmnuRightClick.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.ctxmnuRightClick, "ctxmnuRightClick")
            '
            'SaveToolStripMenuItem
            '
            Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
            resources.ApplyResources(Me.SaveToolStripMenuItem, "SaveToolStripMenuItem")
            '
            'LoadToolStripMenuItem
            '
            Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
            resources.ApplyResources(Me.LoadToolStripMenuItem, "LoadToolStripMenuItem")
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'SettingsToolStripMenuItem1
            '
            Me.SettingsToolStripMenuItem1.Name = "SettingsToolStripMenuItem1"
            resources.ApplyResources(Me.SettingsToolStripMenuItem1, "SettingsToolStripMenuItem1")
            '
            'SaveAsImageToolStripMenuItem
            '
            Me.SaveAsImageToolStripMenuItem.Name = "SaveAsImageToolStripMenuItem"
            resources.ApplyResources(Me.SaveAsImageToolStripMenuItem, "SaveAsImageToolStripMenuItem")
            '
            'FlowDiagram
            '
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.FDPictBox)
            Me.Name = "FlowDiagram"
            CType(Me.FDPictBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ctxmnuRightClick.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

#Region "Drawing Routines"

        'Protected Overrides Sub OnPaintBackground(ByVal pevent As System.Windows.Forms.PaintEventArgs)
        '    MyBase.OnPaintBackground(pevent)
        'End Sub

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
        End Sub 'OnPaintBackground

#End Region

#Region "Mouse Events"


        Private Sub FDPictBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles FDPictBox.MouseDown
            m_FDDraw.setHighlightNodeLock = True
        End Sub

        Private Sub FDPictBox_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles FDPictBox.MouseUp
            m_FDDraw.setHighlightNodeLock = False
        End Sub

        Private Sub FDPictBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles FDPictBox.MouseMove
            m_FDDraw.CheckMouseOver(m_FDData, e.Location)

            Invalidate()
        End Sub

#End Region

        Private Sub FlowDiagram_Resize(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles Me.Resize
            If Not m_FDDraw Is Nothing Then
                m_FDDraw.resizeGraph(Me.Height, Me.Width)
            End If
        End Sub

#Region " Context Menu Access "

        Private Sub OnLoadFromFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles LoadToolStripMenuItem.Click

            Dim ifData As INIFile = Nothing
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(My.Resources.FILEFILTER_FLOWDIAGRAM, 2)

            If (cmdFO.Result = DialogResult.OK) Then
                Try
                    ifData = New INIFile(cmdFO.FileName)
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
                Handles SaveToolStripMenuItem.Click

            Dim ifData As INIFile = Nothing
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_FLOWDIAGRAM, 2)

            If cmdFS.Result = Windows.Forms.DialogResult.OK Then
                Try
                    ifData = New INIFile(cmdFS.FileName)
                    m_FDDraw.SaveToFile(ifData, Me.m_FDData)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to save to file {0}: {1}", cmdFS.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If
        End Sub

        Private Sub OnSettings(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem1.Click
            FDSettingsForm = New FlowDiagramSettings
            FDSettingsForm.FDPropertyGrid.SelectedObject = m_FDDraw.m_treeGraph
            FDSettingsForm.Show()
        End Sub

        Private Sub OnSaveToImage(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles SaveAsImageToolStripMenuItem.Click

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

    End Class


End Namespace