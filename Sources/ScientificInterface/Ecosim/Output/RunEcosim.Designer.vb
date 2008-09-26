Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterface.Controls

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated(), CLSCompliant(False)> _
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
            Me.gpbFF = New System.Windows.Forms.GroupBox
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.tslTarget = New System.Windows.Forms.ToolStripLabel
            Me.tscbTarget = New ScientificInterfaceShared.Controls.CustomToolstripComboBox
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbSetTo0 = New System.Windows.Forms.ToolStripButton
            Me.tsbSetToValue = New System.Windows.Forms.ToolStripButton
            Me.tsbResetFs = New System.Windows.Forms.ToolStripButton
            Me.m_sketchPad = New ScientificInterface.Ecosim.ucForcingSketchPad
            Me.plBPlot = New System.Windows.Forms.Panel
            Me.gpbRun = New System.Windows.Forms.GroupBox
            Me.gpbFF.SuspendLayout()
            Me.ToolStrip1.SuspendLayout()
            Me.gpbRun.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnRunOrStop
            '
            resources.ApplyResources(Me.btnRunOrStop, "btnRunOrStop")
            Me.btnRunOrStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnRunOrStop.Name = "btnRunOrStop"
            Me.btnRunOrStop.UseVisualStyleBackColor = True
            '
            'gpbFF
            '
            resources.ApplyResources(Me.gpbFF, "gpbFF")
            Me.gpbFF.Controls.Add(Me.ToolStrip1)
            Me.gpbFF.Controls.Add(Me.m_sketchPad)
            Me.gpbFF.Name = "gpbFF"
            Me.gpbFF.TabStop = False
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
            Me.m_sketchPad.AxisDisplayMode = ScientificInterface.eAxisDisplayModeTypes.Show
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.BaselineValue = 1.0!
            Me.m_sketchPad.Color = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.RightClickAutoScaleMode = ScientificInterface.eRightClickAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.SketchDrawMode = ScientificInterface.eSketchDrawModeTypes.Fill
            Me.m_sketchPad.YAxisAutoScaleMode = ScientificInterface.eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = 1.0!
            '
            'plBPlot
            '
            resources.ApplyResources(Me.plBPlot, "plBPlot")
            Me.plBPlot.Name = "plBPlot"
            '
            'gpbRun
            '
            resources.ApplyResources(Me.gpbRun, "gpbRun")
            Me.gpbRun.Controls.Add(Me.btnRunOrStop)
            Me.gpbRun.Name = "gpbRun"
            Me.gpbRun.TabStop = False
            '
            'RunEcosim
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gpbRun)
            Me.Controls.Add(Me.plBPlot)
            Me.Controls.Add(Me.gpbFF)
            Me.Name = "RunEcosim"
            Me.gpbFF.ResumeLayout(False)
            Me.gpbFF.PerformLayout()
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.gpbRun.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents btnRunOrStop As System.Windows.Forms.Button
        Friend WithEvents gpbFF As System.Windows.Forms.GroupBox
        Friend WithEvents m_sketchPad As ucForcingSketchPad
        Friend WithEvents plBPlot As System.Windows.Forms.Panel
        Friend WithEvents gpbRun As System.Windows.Forms.GroupBox
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents tslTarget As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tscbTarget As CustomToolstripComboBox
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsbResetFs As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsbSetToValue As System.Windows.Forms.ToolStripButton

    End Class
End Namespace

