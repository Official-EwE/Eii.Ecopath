Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcospaceResults
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcospaceResults))
            Me.plResultsGrid = New System.Windows.Forms.Panel
            Me.gpbOption = New System.Windows.Forms.GroupBox
            Me.cbRegions = New System.Windows.Forms.ComboBox
            Me.cbGears = New System.Windows.Forms.ComboBox
            Me.rbGroup = New System.Windows.Forms.RadioButton
            Me.rbRegion = New System.Windows.Forms.RadioButton
            Me.rbGear = New System.Windows.Forms.RadioButton
            Me.Label1 = New System.Windows.Forms.Label
            Me.txbBegin = New System.Windows.Forms.TextBox
            Me.txbEnd = New System.Windows.Forms.TextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.gpbYear = New System.Windows.Forms.GroupBox
            Me.gpbOption.SuspendLayout()
            Me.gpbYear.SuspendLayout()
            Me.SuspendLayout()
            '
            'plResultsGrid
            '
            resources.ApplyResources(Me.plResultsGrid, "plResultsGrid")
            Me.plResultsGrid.Name = "plResultsGrid"
            '
            'gpbOption
            '
            resources.ApplyResources(Me.gpbOption, "gpbOption")
            Me.gpbOption.Controls.Add(Me.cbRegions)
            Me.gpbOption.Controls.Add(Me.cbGears)
            Me.gpbOption.Controls.Add(Me.rbGroup)
            Me.gpbOption.Controls.Add(Me.rbRegion)
            Me.gpbOption.Controls.Add(Me.rbGear)
            Me.gpbOption.Name = "gpbOption"
            Me.gpbOption.TabStop = False
            '
            'cbRegions
            '
            Me.cbRegions.FormattingEnabled = True
            resources.ApplyResources(Me.cbRegions, "cbRegions")
            Me.cbRegions.Name = "cbRegions"
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
            'rbRegion
            '
            resources.ApplyResources(Me.rbRegion, "rbRegion")
            Me.rbRegion.Name = "rbRegion"
            Me.rbRegion.TabStop = True
            Me.rbRegion.UseVisualStyleBackColor = True
            '
            'rbGear
            '
            resources.ApplyResources(Me.rbGear, "rbGear")
            Me.rbGear.Name = "rbGear"
            Me.rbGear.TabStop = True
            Me.rbGear.UseVisualStyleBackColor = True
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'txbBegin
            '
            resources.ApplyResources(Me.txbBegin, "txbBegin")
            Me.txbBegin.Name = "txbBegin"
            Me.txbBegin.ReadOnly = True
            '
            'txbEnd
            '
            resources.ApplyResources(Me.txbEnd, "txbEnd")
            Me.txbEnd.Name = "txbEnd"
            Me.txbEnd.ReadOnly = True
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'gpbYear
            '
            Me.gpbYear.Controls.Add(Me.txbEnd)
            Me.gpbYear.Controls.Add(Me.Label1)
            Me.gpbYear.Controls.Add(Me.txbBegin)
            Me.gpbYear.Controls.Add(Me.Label2)
            resources.ApplyResources(Me.gpbYear, "gpbYear")
            Me.gpbYear.Name = "gpbYear"
            Me.gpbYear.TabStop = False
            '
            'EcospaceResults
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gpbYear)
            Me.Controls.Add(Me.gpbOption)
            Me.Controls.Add(Me.plResultsGrid)
            Me.Name = "EcospaceResults"
            Me.gpbOption.ResumeLayout(False)
            Me.gpbOption.PerformLayout()
            Me.gpbYear.ResumeLayout(False)
            Me.gpbYear.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plResultsGrid As System.Windows.Forms.Panel
        Friend WithEvents txbEnd As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents txbBegin As System.Windows.Forms.TextBox
        Friend WithEvents gpbOption As System.Windows.Forms.GroupBox
        Friend WithEvents gpbYear As System.Windows.Forms.GroupBox
        Friend WithEvents rbGroup As System.Windows.Forms.RadioButton
        Friend WithEvents rbRegion As System.Windows.Forms.RadioButton
        Friend WithEvents rbGear As System.Windows.Forms.RadioButton
        Friend WithEvents cbRegions As System.Windows.Forms.ComboBox
        Friend WithEvents cbGears As System.Windows.Forms.ComboBox
    End Class

End Namespace

