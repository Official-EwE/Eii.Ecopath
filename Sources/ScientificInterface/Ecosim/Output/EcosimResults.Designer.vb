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
            Me.m_lblNumTimeSteps = New System.Windows.Forms.Label
            Me.udNumTimeSteps = New System.Windows.Forms.NumericUpDown
            Me.txtSumEnd = New System.Windows.Forms.TextBox
            Me.txtSumStart = New System.Windows.Forms.TextBox
            Me.m_lblBegin = New System.Windows.Forms.Label
            Me.m_lblEnd = New System.Windows.Forms.Label
            Me.cbGears = New System.Windows.Forms.ComboBox
            Me.rbGroup = New System.Windows.Forms.RadioButton
            Me.rbIndices = New System.Windows.Forms.RadioButton
            Me.rbGear = New System.Windows.Forms.RadioButton
            Me.plResultsGrid = New System.Windows.Forms.Panel
            Me.m_lblYear = New System.Windows.Forms.Label
            Me.m_lblShow = New System.Windows.Forms.Label
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblNumTimeSteps
            '
            resources.ApplyResources(Me.m_lblNumTimeSteps, "m_lblNumTimeSteps")
            Me.m_lblNumTimeSteps.Name = "m_lblNumTimeSteps"
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
            'm_lblBegin
            '
            resources.ApplyResources(Me.m_lblBegin, "m_lblBegin")
            Me.m_lblBegin.Name = "m_lblBegin"
            '
            'm_lblEnd
            '
            resources.ApplyResources(Me.m_lblEnd, "m_lblEnd")
            Me.m_lblEnd.Name = "m_lblEnd"
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
            'EcosimResults
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblShow)
            Me.Controls.Add(Me.cbGears)
            Me.Controls.Add(Me.m_lblNumTimeSteps)
            Me.Controls.Add(Me.rbGroup)
            Me.Controls.Add(Me.m_lblYear)
            Me.Controls.Add(Me.rbIndices)
            Me.Controls.Add(Me.udNumTimeSteps)
            Me.Controls.Add(Me.rbGear)
            Me.Controls.Add(Me.txtSumEnd)
            Me.Controls.Add(Me.txtSumStart)
            Me.Controls.Add(Me.plResultsGrid)
            Me.Controls.Add(Me.m_lblBegin)
            Me.Controls.Add(Me.m_lblEnd)
            Me.Name = "EcosimResults"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_lblBegin As System.Windows.Forms.Label
        Friend WithEvents m_lblEnd As System.Windows.Forms.Label
        Friend WithEvents rbGroup As System.Windows.Forms.RadioButton
        Friend WithEvents rbIndices As System.Windows.Forms.RadioButton
        Friend WithEvents rbGear As System.Windows.Forms.RadioButton
        Friend WithEvents plResultsGrid As System.Windows.Forms.Panel
        Friend WithEvents txtSumEnd As System.Windows.Forms.TextBox
        Friend WithEvents txtSumStart As System.Windows.Forms.TextBox
        Friend WithEvents m_lblNumTimeSteps As System.Windows.Forms.Label
        Friend WithEvents udNumTimeSteps As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_lblYear As System.Windows.Forms.Label
        Friend WithEvents m_lblShow As System.Windows.Forms.Label
        Private WithEvents cbGears As System.Windows.Forms.ComboBox
    End Class

End Namespace

