Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcosimResults
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcosimResults))
            Me.gpbYear = New System.Windows.Forms.GroupBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.udNumTimeSteps = New System.Windows.Forms.NumericUpDown
            Me.txtSumEnd = New System.Windows.Forms.TextBox
            Me.txtSumStart = New System.Windows.Forms.TextBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.gpbOption = New System.Windows.Forms.GroupBox
            Me.cbGears = New System.Windows.Forms.ComboBox
            Me.rbGroup = New System.Windows.Forms.RadioButton
            Me.rbIndices = New System.Windows.Forms.RadioButton
            Me.rbGear = New System.Windows.Forms.RadioButton
            Me.plResultsGrid = New System.Windows.Forms.Panel
            Me.gpbYear.SuspendLayout()
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gpbOption.SuspendLayout()
            Me.SuspendLayout()
            '
            'gpbYear
            '
            Me.gpbYear.Controls.Add(Me.Label3)
            Me.gpbYear.Controls.Add(Me.udNumTimeSteps)
            Me.gpbYear.Controls.Add(Me.txtSumEnd)
            Me.gpbYear.Controls.Add(Me.txtSumStart)
            Me.gpbYear.Controls.Add(Me.Label1)
            Me.gpbYear.Controls.Add(Me.Label2)
            resources.ApplyResources(Me.gpbYear, "gpbYear")
            Me.gpbYear.Name = "gpbYear"
            Me.gpbYear.TabStop = False
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'udNumTimeSteps
            '
            resources.ApplyResources(Me.udNumTimeSteps, "udNumTimeSteps")
            Me.udNumTimeSteps.Name = "udNumTimeSteps"
            '
            'txtSumEnd
            '
            resources.ApplyResources(Me.txtSumEnd, "txtSumEnd")
            Me.txtSumEnd.Name = "txtSumEnd"
            '
            'txtSumStart
            '
            resources.ApplyResources(Me.txtSumStart, "txtSumStart")
            Me.txtSumStart.Name = "txtSumStart"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'gpbOption
            '
            resources.ApplyResources(Me.gpbOption, "gpbOption")
            Me.gpbOption.Controls.Add(Me.cbGears)
            Me.gpbOption.Controls.Add(Me.rbGroup)
            Me.gpbOption.Controls.Add(Me.rbIndices)
            Me.gpbOption.Controls.Add(Me.rbGear)
            Me.gpbOption.Name = "gpbOption"
            Me.gpbOption.TabStop = False
            '
            'cbGears
            '
            Me.cbGears.FormattingEnabled = True
            resources.ApplyResources(Me.cbGears, "cbGears")
            Me.cbGears.Name = "cbGears"
            '
            'rbGroup
            '
            resources.ApplyResources(Me.rbGroup, "rbGroup")
            Me.rbGroup.Name = "rbGroup"
            Me.rbGroup.TabStop = True
            Me.rbGroup.UseVisualStyleBackColor = True
            '
            'rbIndices
            '
            resources.ApplyResources(Me.rbIndices, "rbIndices")
            Me.rbIndices.Name = "rbIndices"
            Me.rbIndices.TabStop = True
            Me.rbIndices.UseVisualStyleBackColor = True
            '
            'rbGear
            '
            resources.ApplyResources(Me.rbGear, "rbGear")
            Me.rbGear.Name = "rbGear"
            Me.rbGear.TabStop = True
            Me.rbGear.UseVisualStyleBackColor = True
            '
            'plResultsGrid
            '
            resources.ApplyResources(Me.plResultsGrid, "plResultsGrid")
            Me.plResultsGrid.Name = "plResultsGrid"
            '
            'EcosimResults
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gpbYear)
            Me.Controls.Add(Me.gpbOption)
            Me.Controls.Add(Me.plResultsGrid)
            Me.Name = "EcosimResults"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.gpbYear.ResumeLayout(False)
            Me.gpbYear.PerformLayout()
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gpbOption.ResumeLayout(False)
            Me.gpbOption.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gpbYear As System.Windows.Forms.GroupBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents gpbOption As System.Windows.Forms.GroupBox
        Friend WithEvents cbGears As System.Windows.Forms.ComboBox
        Friend WithEvents rbGroup As System.Windows.Forms.RadioButton
        Friend WithEvents rbIndices As System.Windows.Forms.RadioButton
        Friend WithEvents rbGear As System.Windows.Forms.RadioButton
        Friend WithEvents plResultsGrid As System.Windows.Forms.Panel
        Friend WithEvents txtSumEnd As System.Windows.Forms.TextBox
        Friend WithEvents txtSumStart As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents udNumTimeSteps As System.Windows.Forms.NumericUpDown
    End Class

End Namespace

