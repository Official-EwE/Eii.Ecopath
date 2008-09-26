Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <CLSCompliant(False)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmEggProduction
        Inherits frmEwE

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEggProduction))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.plSketchPad = New System.Windows.Forms.Panel
            Me.tlpSketchpad = New System.Windows.Forms.TableLayoutPanel
            Me.m_sketchPadToolbar = New ScientificInterface.Ecosim.ucSketchPadToolbar
            Me.m_sketchPad = New ScientificInterface.Ecosim.ucForcingSketchPad
            Me.m_shapeToolBox = New ScientificInterface.Ecosim.ucShapeToolbox
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_shapeToolboxToolbar = New ucShapeToolboxToolbar
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.plSketchPad.SuspendLayout()
            Me.tlpSketchpad.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.plSketchPad)
            '
            'SplitContainer1.Panel2
            '
            resources.ApplyResources(Me.SplitContainer1.Panel2, "SplitContainer1.Panel2")
            Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel1)
            '
            'plSketchPad
            '
            Me.plSketchPad.Controls.Add(Me.tlpSketchpad)
            resources.ApplyResources(Me.plSketchPad, "plSketchPad")
            Me.plSketchPad.Name = "plSketchPad"
            '
            'tlpSketchpad
            '
            resources.ApplyResources(Me.tlpSketchpad, "tlpSketchpad")
            Me.tlpSketchpad.Controls.Add(Me.m_sketchPadToolbar, 0, 0)
            Me.tlpSketchpad.Controls.Add(Me.m_sketchPad, 0, 1)
            Me.tlpSketchpad.Name = "tlpSketchpad"
            '
            'm_sketchPadToolbar
            '
            Me.m_sketchPadToolbar.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.m_sketchPadToolbar, "m_sketchPadToolbar")
            Me.m_sketchPadToolbar.Handler = Nothing
            Me.m_sketchPadToolbar.Name = "m_sketchPadToolbar"
            '
            'm_sketchPad
            '
            Me.m_sketchPad.AxisDisplayMode = eAxisDisplayModeTypes.Show
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.Color = System.Drawing.Color.AliceBlue
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_sketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Me.m_sketchPad.YAxisMaxValue = 1.0!
            '
            'm_shapeToolBox
            '
            Me.m_shapeToolBox.Color = System.Drawing.Color.Empty
            Me.m_shapeToolBox.CurSelectedIndex = -1
            resources.ApplyResources(Me.m_shapeToolBox, "m_shapeToolBox")
            Me.m_shapeToolBox.Handler = Nothing
            Me.m_shapeToolBox.Name = "m_shapeToolBox"
            Me.m_shapeToolBox.Selection = Nothing
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolBox, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolboxToolbar, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
            '
            'frmEggProduction
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmEggProduction"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.plSketchPad.ResumeLayout(False)
            Me.tlpSketchpad.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plSketchPad As System.Windows.Forms.Panel
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents tlpSketchpad As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Friend WithEvents m_sketchPad As ucForcingSketchPad
        Friend WithEvents m_shapeToolBox As ucShapeToolbox
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar

    End Class
End Namespace

