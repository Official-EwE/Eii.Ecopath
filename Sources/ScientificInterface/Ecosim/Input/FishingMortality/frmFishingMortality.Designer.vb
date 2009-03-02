Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <CLSCompliant(False)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFishingMortality
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFishingMortality))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.m_sketchPad = New ucForcingSketchPad
            Me.m_shapeToolboxToolbar = New ucShapeToolboxToolbar
            Me.m_shapeToolBox = New ucShapeToolbox
            Me.m_sketchPadToolbar = New ucSketchPadToolbar
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
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
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_sketchPadToolbar)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_sketchPad)
            '
            'SplitContainer1.Panel2
            '
            resources.ApplyResources(Me.SplitContainer1.Panel2, "SplitContainer1.Panel2")
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_shapeToolboxToolbar)
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_shapeToolBox)
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
            'Me.m_sketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_sketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = -9999.0!
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Handler = Nothing
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
            '
            'm_shapeToolBox
            '
            resources.ApplyResources(Me.m_shapeToolBox, "m_shapeToolBox")
            Me.m_shapeToolBox.Color = System.Drawing.Color.Empty
            Me.m_shapeToolBox.CurSelectedIndex = -1
            Me.m_shapeToolBox.Handler = Nothing
            Me.m_shapeToolBox.Name = "m_shapeToolBox"
            Me.m_shapeToolBox.Selection = Nothing
            Me.m_shapeToolBox.YAxisMinValue = -9999.0!
            '
            'm_sketchPadToolbar
            '
            Me.m_sketchPadToolbar.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.m_sketchPadToolbar, "m_sketchPadToolbar")
            Me.m_sketchPadToolbar.Handler = Nothing
            Me.m_sketchPadToolbar.Name = "m_sketchPadToolbar"
            '
            'frmFishingMortality
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmFishingMortality"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents m_shapeToolBox As ucShapeToolbox
        Private WithEvents m_sketchPad As ucForcingSketchPad
        Private WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar
        Private WithEvents m_sketchPadToolbar As ucSketchPadToolbar

    End Class
End Namespace

