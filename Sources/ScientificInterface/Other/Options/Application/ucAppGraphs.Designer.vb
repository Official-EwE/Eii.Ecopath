Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppGraphs
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppGraphs))
            Me.m_lblHeader = New System.Windows.Forms.Label
            Me.m_lblThumbnailSize = New System.Windows.Forms.Label
            Me.m_nudThumbnailSize = New System.Windows.Forms.NumericUpDown
            Me.m_gbLegends = New System.Windows.Forms.GroupBox
            Me.m_rbLegendNever = New System.Windows.Forms.RadioButton
            Me.m_rbLegendAlways = New System.Windows.Forms.RadioButton
            Me.m_rbLegendSelective = New System.Windows.Forms.RadioButton
            Me.m_gbThumbnails = New System.Windows.Forms.GroupBox
            Me.m_lblThumbnailUnit = New System.Windows.Forms.Label
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbLegends.SuspendLayout()
            Me.m_gbThumbnails.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblHeader
            '
            Me.m_lblHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.m_lblHeader, "m_lblHeader")
            Me.m_lblHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblHeader.Name = "m_lblHeader"
            '
            'm_lblThumbnailSize
            '
            resources.ApplyResources(Me.m_lblThumbnailSize, "m_lblThumbnailSize")
            Me.m_lblThumbnailSize.Name = "m_lblThumbnailSize"
            '
            'm_nudThumbnailSize
            '
            resources.ApplyResources(Me.m_nudThumbnailSize, "m_nudThumbnailSize")
            Me.m_nudThumbnailSize.Maximum = New Decimal(New Integer() {240, 0, 0, 0})
            Me.m_nudThumbnailSize.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
            Me.m_nudThumbnailSize.Name = "m_nudThumbnailSize"
            Me.m_nudThumbnailSize.Value = New Decimal(New Integer() {32, 0, 0, 0})
            '
            'm_gbLegends
            '
            resources.ApplyResources(Me.m_gbLegends, "m_gbLegends")
            Me.m_gbLegends.Controls.Add(Me.m_rbLegendNever)
            Me.m_gbLegends.Controls.Add(Me.m_rbLegendAlways)
            Me.m_gbLegends.Controls.Add(Me.m_rbLegendSelective)
            Me.m_gbLegends.Name = "m_gbLegends"
            Me.m_gbLegends.TabStop = False
            '
            'm_rbLegendNever
            '
            resources.ApplyResources(Me.m_rbLegendNever, "m_rbLegendNever")
            Me.m_rbLegendNever.Name = "m_rbLegendNever"
            Me.m_rbLegendNever.TabStop = True
            Me.m_rbLegendNever.UseVisualStyleBackColor = True
            '
            'm_rbLegendAlways
            '
            resources.ApplyResources(Me.m_rbLegendAlways, "m_rbLegendAlways")
            Me.m_rbLegendAlways.Name = "m_rbLegendAlways"
            Me.m_rbLegendAlways.TabStop = True
            Me.m_rbLegendAlways.UseVisualStyleBackColor = True
            '
            'm_rbLegendSelective
            '
            resources.ApplyResources(Me.m_rbLegendSelective, "m_rbLegendSelective")
            Me.m_rbLegendSelective.Name = "m_rbLegendSelective"
            Me.m_rbLegendSelective.TabStop = True
            Me.m_rbLegendSelective.UseVisualStyleBackColor = True
            '
            'm_gbThumbnails
            '
            resources.ApplyResources(Me.m_gbThumbnails, "m_gbThumbnails")
            Me.m_gbThumbnails.Controls.Add(Me.m_lblThumbnailUnit)
            Me.m_gbThumbnails.Controls.Add(Me.m_lblThumbnailSize)
            Me.m_gbThumbnails.Controls.Add(Me.m_nudThumbnailSize)
            Me.m_gbThumbnails.Name = "m_gbThumbnails"
            Me.m_gbThumbnails.TabStop = False
            '
            'm_lblThumbnailUnit
            '
            resources.ApplyResources(Me.m_lblThumbnailUnit, "m_lblThumbnailUnit")
            Me.m_lblThumbnailUnit.Name = "m_lblThumbnailUnit"
            '
            'ucAppGraphs
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_gbThumbnails)
            Me.Controls.Add(Me.m_gbLegends)
            Me.Controls.Add(Me.m_lblHeader)
            Me.Name = "ucAppGraphs"
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbLegends.ResumeLayout(False)
            Me.m_gbLegends.PerformLayout()
            Me.m_gbThumbnails.ResumeLayout(False)
            Me.m_gbThumbnails.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_nudThumbnailSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblHeader As System.Windows.Forms.Label
        Private WithEvents m_lblThumbnailSize As System.Windows.Forms.Label
        Private WithEvents m_gbLegends As System.Windows.Forms.GroupBox
        Friend WithEvents m_rbLegendNever As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbLegendAlways As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbLegendSelective As System.Windows.Forms.RadioButton
        Private WithEvents m_gbThumbnails As System.Windows.Forms.GroupBox
        Private WithEvents m_lblThumbnailUnit As System.Windows.Forms.Label

    End Class
End Namespace

