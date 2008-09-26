Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmMediationFunction
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMediationFunction))
            Me.plSketchPad = New System.Windows.Forms.Panel
            Me.tlpSketchPad = New System.Windows.Forms.TableLayoutPanel
            Me.m_sketchPadToolbar = New ScientificInterface.Ecosim.ucSketchPadToolbar
            Me.m_sketchPad = New ScientificInterface.Ecosim.ucMediationSketchPad
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
            Me.plBiomassPerct = New System.Windows.Forms.Panel
            Me.m_bioPercent = New ScientificInterface.Ecosim.ucBioPercent
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.tsBtnEditBioPert = New System.Windows.Forms.ToolStripButton
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_shapeToolBox = New ScientificInterface.Ecosim.ucShapeToolbox
            Me.m_shapeToolboxToolbar = New ScientificInterface.Ecosim.ucShapeToolboxToolbar
            Me.plSketchPad.SuspendLayout()
            Me.tlpSketchPad.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SplitContainer2.Panel1.SuspendLayout()
            Me.SplitContainer2.Panel2.SuspendLayout()
            Me.SplitContainer2.SuspendLayout()
            Me.plBiomassPerct.SuspendLayout()
            Me.ToolStrip1.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'plSketchPad
            '
            Me.plSketchPad.Controls.Add(Me.tlpSketchPad)
            resources.ApplyResources(Me.plSketchPad, "plSketchPad")
            Me.plSketchPad.Name = "plSketchPad"
            '
            'tlpSketchPad
            '
            resources.ApplyResources(Me.tlpSketchPad, "tlpSketchPad")
            Me.tlpSketchPad.Controls.Add(Me.m_sketchPadToolbar, 0, 0)
            Me.tlpSketchPad.Controls.Add(Me.m_sketchPad, 0, 1)
            Me.tlpSketchPad.Name = "tlpSketchPad"
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
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.AxisDisplayMode = eAxisDisplayModeTypes.Show
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.Color = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_sketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Me.m_sketchPad.YAxisMaxValue = 1.0!
            Me.m_sketchPad.YAxisMinValue = -9999.0!
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel1)
            '
            'SplitContainer2
            '
            Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.SplitContainer2, "SplitContainer2")
            Me.SplitContainer2.Name = "SplitContainer2"
            '
            'SplitContainer2.Panel1
            '
            Me.SplitContainer2.Panel1.Controls.Add(Me.plSketchPad)
            '
            'SplitContainer2.Panel2
            '
            Me.SplitContainer2.Panel2.Controls.Add(Me.plBiomassPerct)
            Me.SplitContainer2.Panel2.Controls.Add(Me.ToolStrip1)
            '
            'plBiomassPerct
            '
            Me.plBiomassPerct.Controls.Add(Me.m_bioPercent)
            resources.ApplyResources(Me.plBiomassPerct, "plBiomassPerct")
            Me.plBiomassPerct.Name = "plBiomassPerct"
            '
            'm_bioPercent
            '
            resources.ApplyResources(Me.m_bioPercent, "m_bioPercent")
            Me.m_bioPercent.Name = "m_bioPercent"
            Me.m_bioPercent.Shape = Nothing
            '
            'ToolStrip1
            '
            Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnEditBioPert})
            resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'tsBtnEditBioPert
            '
            Me.tsBtnEditBioPert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnEditBioPert, "tsBtnEditBioPert")
            Me.tsBtnEditBioPert.Name = "tsBtnEditBioPert"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolBox, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolboxToolbar, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_shapeToolBox
            '
            Me.m_shapeToolBox.Color = System.Drawing.Color.Empty
            Me.m_shapeToolBox.CurSelectedIndex = -1
            resources.ApplyResources(Me.m_shapeToolBox, "m_shapeToolBox")
            Me.m_shapeToolBox.Handler = Nothing
            Me.m_shapeToolBox.Name = "m_shapeToolBox"
            Me.m_shapeToolBox.Selection = Nothing
            Me.m_shapeToolBox.YAxisMinValue = -9999.0!
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Handler = Nothing
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
            '
            'frmMediationFunction
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmMediationFunction"
            Me.plSketchPad.ResumeLayout(False)
            Me.tlpSketchPad.ResumeLayout(False)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.SplitContainer2.Panel1.ResumeLayout(False)
            Me.SplitContainer2.Panel2.ResumeLayout(False)
            Me.SplitContainer2.Panel2.PerformLayout()
            Me.SplitContainer2.ResumeLayout(False)
            Me.plBiomassPerct.ResumeLayout(False)
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plSketchPad As System.Windows.Forms.Panel
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
        Friend WithEvents plBiomassPerct As System.Windows.Forms.Panel
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents tsBtnEditBioPert As System.Windows.Forms.ToolStripButton
        Friend WithEvents tlpSketchPad As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_shapeToolBox As ucShapeToolbox
        Friend WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar
        Friend WithEvents m_bioPercent As ucBioPercent
        Friend WithEvents m_sketchPad As ScientificInterface.Ecosim.ucMediationSketchPad

    End Class
End Namespace

