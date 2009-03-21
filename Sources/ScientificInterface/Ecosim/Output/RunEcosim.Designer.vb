Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RunEcosim
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunEcosim))
            Me.btnRunOrStop = New System.Windows.Forms.Button
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.tslTarget = New System.Windows.Forms.ToolStripLabel
            Me.tscbTarget = New ScientificInterfaceShared.Controls.cCustomToolstripComboBox
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbSetTo0 = New System.Windows.Forms.ToolStripButton
            Me.tsbSetToValue = New System.Windows.Forms.ToolStripButton
            Me.tsbResetFs = New System.Windows.Forms.ToolStripButton
            Me.m_sketchPad = New ucForcingSketchPad
            Me.m_graph = New Ecosim.ucBiomassPlotzgc
            Me.m_spContainer = New System.Windows.Forms.SplitContainer
            Me.ToolStrip1.SuspendLayout()
            Me.m_spContainer.Panel1.SuspendLayout()
            Me.m_spContainer.Panel2.SuspendLayout()
            Me.m_spContainer.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnRunOrStop
            '
            resources.ApplyResources(Me.btnRunOrStop, "btnRunOrStop")
            Me.btnRunOrStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnRunOrStop.Name = "btnRunOrStop"
            Me.btnRunOrStop.UseVisualStyleBackColor = True
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslTarget, Me.tscbTarget, Me.ToolStripSeparator1, Me.tsbSetTo0, Me.tsbSetToValue, Me.tsbResetFs})
            resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
            Me.ToolStrip1.Name = "ToolStrip1"
            '
            'tslTarget
            '
            Me.tslTarget.Name = "tslTarget"
            resources.ApplyResources(Me.tslTarget, "tslTarget")
            '
            'tscbTarget
            '
            resources.ApplyResources(Me.tscbTarget, "tscbTarget")
            Me.tscbTarget.DropDownHeight = 1
            Me.tscbTarget.DropDownStyle = ComboBoxStyle.DropDownList
            Me.tscbTarget.Name = "tscbTarget"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsbSetTo0
            '
            Me.tsbSetTo0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbSetTo0, "tsbSetTo0")
            Me.tsbSetTo0.Name = "tsbSetTo0"
            '
            'tsbSetToValue
            '
            Me.tsbSetToValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbSetToValue, "tsbSetToValue")
            Me.tsbSetToValue.Name = "tsbSetToValue"
            '
            'tsbResetFs
            '
            Me.tsbResetFs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbResetFs, "tsbResetFs")
            Me.tsbResetFs.Name = "tsbResetFs"
            '
            'm_sketchPad
            '
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.XMarkValue = 1.0!
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            'Me.m_sketchPad.RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_sketchPad.YAxisAutoScaleMode = eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = 1.0!
            '
            'm_graph
            '
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.Name = "m_graph"
            '
            'm_spContainer
            '
            resources.ApplyResources(Me.m_spContainer, "m_spContainer")
            Me.m_spContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.m_spContainer.Name = "m_spContainer"
            '
            'm_spContainer.Panel1
            '
            Me.m_spContainer.Panel1.Controls.Add(Me.m_graph)
            '
            'm_spContainer.Panel2
            '
            Me.m_spContainer.Panel2.Controls.Add(Me.ToolStrip1)
            Me.m_spContainer.Panel2.Controls.Add(Me.btnRunOrStop)
            Me.m_spContainer.Panel2.Controls.Add(Me.m_sketchPad)
            '
            'RunEcosim
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_spContainer)
            Me.Name = "RunEcosim"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.m_spContainer.Panel1.ResumeLayout(False)
            Me.m_spContainer.Panel2.ResumeLayout(False)
            Me.m_spContainer.Panel2.PerformLayout()
            Me.m_spContainer.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents btnRunOrStop As System.Windows.Forms.Button
        Private WithEvents m_sketchPad As ucForcingSketchPad
        Private WithEvents m_graph As ucBiomassPlotzgc
        Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Private WithEvents tslTarget As System.Windows.Forms.ToolStripLabel
        Private WithEvents tscbTarget As cCustomToolstripComboBox
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsbResetFs As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetToValue As System.Windows.Forms.ToolStripButton
        Private WithEvents m_spContainer As System.Windows.Forms.SplitContainer

    End Class
End Namespace

