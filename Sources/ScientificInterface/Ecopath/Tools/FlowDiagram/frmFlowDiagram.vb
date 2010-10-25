#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEUtils.Win32Api
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Drawing
Imports System.Reflection
Imports System.IO
Imports System.Drawing.Imaging

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    ''' =======================================================================
    ''' <summary>
    ''' Form presenting the Ecopath Flow Diagram interface.
    ''' </summary>
    ''' =======================================================================
    Public Class FlowDiagram
        : Inherits frmEwE

#Region " Private variables "

        Private components As System.ComponentModel.IContainer = Nothing
        Private m_data As cFlowDiagramData = Nothing
        Private m_doodler As cFlowDiagramRenderer = Nothing
        Private m_tree As cFlowDiagramTree = Nothing

        Private m_bMouseDown As Boolean = False
        Private WithEvents m_pbFlowDiagram As System.Windows.Forms.PictureBox
        Private WithEvents m_scContent As System.Windows.Forms.SplitContainer
        Private WithEvents m_tsFlowDiagram As System.Windows.Forms.ToolStrip
        Private WithEvents m_pgFlowDiagram As System.Windows.Forms.PropertyGrid
        Private WithEvents m_tsmiSave As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsmiLoad As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsmiSaveToImage As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tss1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsmiSettings As System.Windows.Forms.ToolStripButton

#End Region ' Private variables

#Region " Constructor/Destructor "

        Public Sub New()

            Me.InitializeComponent()

            ' This draws the control whenever it is resized
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            ' This supports mouse movement such as the mouse wheel
            Me.SetStyle(ControlStyles.UserMouse, True)

        End Sub

        Public Sub New(ByVal text As String)
            Me.New()
            'Set tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text
        End Sub

#End Region ' Constructor 

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_data = New cFlowDiagramData(Me.UIContext)
            Me.m_tree = New cFlowDiagramTree(Me.m_data)
            Me.m_doodler = New cFlowDiagramRenderer(Me.m_data, Me.m_tree)

            Me.m_pgFlowDiagram.SelectedObject = Me.m_tree
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath}
            Me.UpdateControls()

            AddHandler Me.m_tree.OnChanged, AddressOf OnTreeChanged

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            RemoveHandler Me.m_tree.OnChanged, AddressOf OnTreeChanged
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Overrides

#Region " Events "

#Region " Drawing "

        Private Sub FlowDiagram_Resize(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_pbFlowDiagram.Resize
            Me.m_pbFlowDiagram.Invalidate()
        End Sub

        Private Sub FlowDiagram_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_pbFlowDiagram.Paint

            Dim rc As Rectangle = Me.m_pbFlowDiagram.ClientRectangle
            Me.m_doodler.DrawFlowDiagram(e.Graphics, rc)

        End Sub

        '' Overrides the paint routine so it elimates the flicker
        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
        End Sub

#End Region ' Drawing

#Region " Mouse Events "

        Private Sub FDPictBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseDown

            Using g As Graphics = Me.CreateGraphics()
                Me.m_doodler.BeginDrag(Me.m_pbFlowDiagram.ClientRectangle, e.Location, g)
            End Using

        End Sub

        Private Sub FDPictBox_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseUp
            Me.m_doodler.EndDrag(Me.m_data, e.Location)
        End Sub

        Private Sub FDPictBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseMove

            Using g As Graphics = Me.CreateGraphics()
                Me.m_doodler.ProcessMouseMove(g, Me.m_pbFlowDiagram.ClientRectangle, e.Location)
                Me.m_pbFlowDiagram.Invalidate()
            End Using

        End Sub

#End Region ' Mouse Events

#Region " Tree events (wouldn't that be nice?)"

        Private Sub OnTreeChanged(ByVal sender As cFlowDiagramTree)
            Me.Invalidate()
        End Sub

#End Region ' Tree events

#Region " Commands "

        Private Sub OnLoadFromFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiLoad.Click

            Dim ifData As cINIFile = Nothing
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(SharedResources.FILEFILTER_FLOWDIAGRAM, 1)

            If (cmdFO.Result = DialogResult.OK) Then
                Try
                    ifData = New cINIFile(cmdFO.FileName)
                    m_doodler.LoadFromFile(ifData, Me.m_pbFlowDiagram.ClientRectangle)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to load from file '{0}': {1}", cmdFO.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If

        End Sub

        Private Sub OnSaveToFile(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSave.Click

            Dim ifData As cINIFile = Nothing
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(SharedResources.FILEFILTER_FLOWDIAGRAM, 1)

            If cmdFS.Result = Windows.Forms.DialogResult.OK Then
                Try
                    ifData = New cINIFile(cmdFS.FileName)
                    m_doodler.SaveToFile(ifData, Me.m_pbFlowDiagram.ClientRectangle)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to save to file {0}: {1}", cmdFS.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
            End If
        End Sub

        Private Sub OnSettings(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSettings.Click

            Me.UpdateControls()

        End Sub

        Private Sub OnSaveToImage(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSaveToImage.Click

            Dim fmt As Imaging.ImageFormat = Imaging.ImageFormat.Bmp
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim fs As FileStream = Nothing
            Dim hdc As IntPtr = Nothing ' :)
            Dim mf As Metafile = Nothing
            Dim bmp As Bitmap = New Bitmap(Me.m_pbFlowDiagram.Width, Me.m_pbFlowDiagram.Height, PixelFormat.Format32bppArgb)
            Dim rc As Rectangle = Me.m_pbFlowDiagram.ClientRectangle

            cmdFS.Invoke("EwE6-flow_diagram", SharedResources.FILEFILTER_IMAGE & "|" & SharedResources.FILEFILTER_IMAGE_EMF, 6)
            If cmdFS.Result = DialogResult.OK Then
                Select Case cmdFS.FilterIndex
                    Case 2
                        fmt = Imaging.ImageFormat.Jpeg
                    Case 3
                        fmt = Imaging.ImageFormat.Gif
                    Case 4
                        fmt = Imaging.ImageFormat.Png
                    Case 5
                        fmt = Imaging.ImageFormat.Tiff
                    Case 6
                        fs = New FileStream(cmdFS.FileName, FileMode.Create)
                        Using g As Graphics = Graphics.FromImage(bmp)
                            hdc = g.GetHdc()
                            mf = New Metafile(fs, hdc, EmfType.EmfOnly)
                            g.ReleaseHdc(hdc)
                        End Using
                        Using g As Graphics = Graphics.FromImage(mf)
                            Me.m_doodler.DrawFlowDiagram(g, rc)
                        End Using
                        fs.Close()
                        mf.Dispose()
                        bmp.Dispose()
                        Return
                    Case Else
                        fmt = Imaging.ImageFormat.Bmp
                End Select

                Using g As Graphics = Graphics.FromImage(bmp)
                    Me.m_doodler.DrawFlowDiagram(g, rc)
                End Using

                Try
                    bmp.Save(cmdFS.FileName, fmt)
                Catch ex As Exception
                    ' ToDo: provide error feedback via cCore message
                    ' ToDo: globalize this
                    MsgBox(String.Format("Unable to save to file {0}: {1}", cmdFS.FileName, ex.Message), _
                        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                End Try
                bmp.Dispose()

            End If

        End Sub

#End Region ' Commands

#End Region ' Events

#Region " Internals "

        Private Sub UpdateControls()
            Me.m_scContent.Panel2Collapsed = Not Me.m_tsmiSettings.Checked
            Me.m_pgFlowDiagram.Refresh()
        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FlowDiagram))
            Me.m_pbFlowDiagram = New System.Windows.Forms.PictureBox
            Me.m_scContent = New System.Windows.Forms.SplitContainer
            Me.m_pgFlowDiagram = New System.Windows.Forms.PropertyGrid
            Me.m_tsFlowDiagram = New System.Windows.Forms.ToolStrip
            Me.m_tsmiLoad = New System.Windows.Forms.ToolStripButton
            Me.m_tsmiSave = New System.Windows.Forms.ToolStripButton
            Me.m_tsmiSaveToImage = New System.Windows.Forms.ToolStripButton
            Me.m_tss1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsmiSettings = New System.Windows.Forms.ToolStripButton
            CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scContent.Panel1.SuspendLayout()
            Me.m_scContent.Panel2.SuspendLayout()
            Me.m_scContent.SuspendLayout()
            Me.m_tsFlowDiagram.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_pbFlowDiagram
            '
            Me.m_pbFlowDiagram.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_pbFlowDiagram, "m_pbFlowDiagram")
            Me.m_pbFlowDiagram.Name = "m_pbFlowDiagram"
            Me.m_pbFlowDiagram.TabStop = False
            '
            'm_scContent
            '
            resources.ApplyResources(Me.m_scContent, "m_scContent")
            Me.m_scContent.Name = "m_scContent"
            '
            'm_scContent.Panel1
            '
            Me.m_scContent.Panel1.Controls.Add(Me.m_pbFlowDiagram)
            '
            'm_scContent.Panel2
            '
            Me.m_scContent.Panel2.Controls.Add(Me.m_pgFlowDiagram)
            '
            'm_pgFlowDiagram
            '
            resources.ApplyResources(Me.m_pgFlowDiagram, "m_pgFlowDiagram")
            Me.m_pgFlowDiagram.Name = "m_pgFlowDiagram"
            '
            'm_tsFlowDiagram
            '
            Me.m_tsFlowDiagram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiLoad, Me.m_tsmiSave, Me.m_tsmiSaveToImage, Me.m_tss1, Me.m_tsmiSettings})
            resources.ApplyResources(Me.m_tsFlowDiagram, "m_tsFlowDiagram")
            Me.m_tsFlowDiagram.Name = "m_tsFlowDiagram"
            '
            'm_tsmiLoad
            '
            Me.m_tsmiLoad.Image = SharedResources.openHS
            resources.ApplyResources(Me.m_tsmiLoad, "m_tsmiLoad")
            Me.m_tsmiLoad.Name = "m_tsmiLoad"
            '
            'm_tsmiSave
            '
            Me.m_tsmiSave.Image = SharedResources.saveHS
            resources.ApplyResources(Me.m_tsmiSave, "m_tsmiSave")
            Me.m_tsmiSave.Name = "m_tsmiSave"
            '
            'm_tsmiSaveToImage
            '
            Me.m_tsmiSaveToImage.Image = SharedResources.saveHS
            resources.ApplyResources(Me.m_tsmiSaveToImage, "m_tsmiSaveToImage")
            Me.m_tsmiSaveToImage.Name = "m_tsmiSaveToImage"
            '
            'm_tss1
            '
            Me.m_tss1.Name = "m_tss1"
            resources.ApplyResources(Me.m_tss1, "m_tss1")
            '
            'm_tsmiSettings
            '
            Me.m_tsmiSettings.CheckOnClick = True
            Me.m_tsmiSettings.Image = SharedResources.OptionsHS
            resources.ApplyResources(Me.m_tsmiSettings, "m_tsmiSettings")
            Me.m_tsmiSettings.Name = "m_tsmiSettings"
            '
            'FlowDiagram
            '
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.m_tsFlowDiagram)
            Me.Controls.Add(Me.m_scContent)
            Me.Name = "FlowDiagram"
            CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scContent.Panel1.ResumeLayout(False)
            Me.m_scContent.Panel2.ResumeLayout(False)
            Me.m_scContent.ResumeLayout(False)
            Me.m_tsFlowDiagram.ResumeLayout(False)
            Me.m_tsFlowDiagram.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region ' Internals

    End Class

End Namespace