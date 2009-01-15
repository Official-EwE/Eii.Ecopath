Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class cFormEcospaceResults
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(cFormEcospaceResults))
            Me.plResultsGrid = New System.Windows.Forms.Panel
            Me.udSumLength = New System.Windows.Forms.NumericUpDown
            Me.tbSumEndTime = New System.Windows.Forms.TextBox
            Me.tbSumStartTime = New System.Windows.Forms.TextBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.cbRegions = New System.Windows.Forms.ComboBox
            Me.cbGears = New System.Windows.Forms.ComboBox
            Me.rbGroup = New System.Windows.Forms.RadioButton
            Me.rbRegion = New System.Windows.Forms.RadioButton
            Me.rbGear = New System.Windows.Forms.RadioButton
            Me.m_lblYear = New System.Windows.Forms.Label
            Me.m_lblShow = New System.Windows.Forms.Label
            Me.Label3 = New System.Windows.Forms.Label
            CType(Me.udSumLength, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'plResultsGrid
            '
            resources.ApplyResources(Me.plResultsGrid, "plResultsGrid")
            Me.plResultsGrid.Name = "plResultsGrid"
            '
            'udSumLength
            '
            resources.ApplyResources(Me.udSumLength, "udSumLength")
            Me.udSumLength.Name = "udSumLength"
            '
            'tbSumEndTime
            '
            resources.ApplyResources(Me.tbSumEndTime, "tbSumEndTime")
            Me.tbSumEndTime.Name = "tbSumEndTime"
            '
            'tbSumStartTime
            '
            resources.ApplyResources(Me.tbSumStartTime, "tbSumStartTime")
            Me.tbSumStartTime.Name = "tbSumStartTime"
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
            'cbRegions
            '
            Me.cbRegions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbRegions.FormattingEnabled = True
            resources.ApplyResources(Me.cbRegions, "cbRegions")
            Me.cbRegions.Name = "cbRegions"
            '
            'cbGears
            '
            Me.cbGears.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
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
            'm_lblYear
            '
            Me.m_lblYear.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.m_lblYear, "m_lblYear")
            Me.m_lblYear.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblYear.Name = "m_lblYear"
            '
            'm_lblShow
            '
            resources.ApplyResources(Me.m_lblShow, "m_lblShow")
            Me.m_lblShow.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblShow.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblShow.Name = "m_lblShow"
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'cFormEcospaceResults
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.m_lblShow)
            Me.Controls.Add(Me.m_lblYear)
            Me.Controls.Add(Me.cbRegions)
            Me.Controls.Add(Me.cbGears)
            Me.Controls.Add(Me.rbGroup)
            Me.Controls.Add(Me.rbRegion)
            Me.Controls.Add(Me.rbGear)
            Me.Controls.Add(Me.udSumLength)
            Me.Controls.Add(Me.tbSumEndTime)
            Me.Controls.Add(Me.tbSumStartTime)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.plResultsGrid)
            Me.Name = "cFormEcospaceResults"
            CType(Me.udSumLength, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents plResultsGrid As System.Windows.Forms.Panel
        Friend WithEvents udSumLength As System.Windows.Forms.NumericUpDown
        Friend WithEvents tbSumEndTime As System.Windows.Forms.TextBox
        Friend WithEvents tbSumStartTime As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents cbRegions As System.Windows.Forms.ComboBox
        Friend WithEvents cbGears As System.Windows.Forms.ComboBox
        Friend WithEvents rbGroup As System.Windows.Forms.RadioButton
        Friend WithEvents rbRegion As System.Windows.Forms.RadioButton
        Friend WithEvents rbGear As System.Windows.Forms.RadioButton
        Friend WithEvents m_lblYear As System.Windows.Forms.Label
        Friend WithEvents m_lblShow As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
    End Class

End Namespace

