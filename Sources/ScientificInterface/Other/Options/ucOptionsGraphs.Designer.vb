Imports ScientificInterfaceShared

Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucOptionsGraphs
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsGraphs))
            Me.m_hdr1 = New cEwEHeaderLabel
            Me.m_lblThumbnailSize = New System.Windows.Forms.Label
            Me.m_nudThumbnailSize = New System.Windows.Forms.NumericUpDown
            Me.m_gbLegends = New System.Windows.Forms.GroupBox
            Me.m_rbLegendAlways = New System.Windows.Forms.RadioButton
            Me.m_rbLegendSelective = New System.Windows.Forms.RadioButton
            Me.m_gbThumbnails = New System.Windows.Forms.GroupBox
            Me.m_lblThumbnailUnit = New System.Windows.Forms.Label
            Me.m_nudFontSize = New System.Windows.Forms.NumericUpDown
            Me.m_lblFontSize = New System.Windows.Forms.Label
            Me.m_lblExample = New System.Windows.Forms.Label
            Me.m_hdr2 = New cEwEHeaderLabel
            Me.m_gbxExample = New System.Windows.Forms.GroupBox
            Me.m_cbFontStyle = New System.Windows.Forms.ComboBox
            Me.m_lblItemFontStyle = New System.Windows.Forms.Label
            Me.m_cbFontFamily = New System.Windows.Forms.ComboBox
            Me.lblItemForeColor = New System.Windows.Forms.Label
            Me.m_btnResetFonts = New System.Windows.Forms.Button
            Me.m_lbFontTypes = New System.Windows.Forms.ListBox
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbLegends.SuspendLayout()
            Me.m_gbThumbnails.SuspendLayout()
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbxExample.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_hdr1
            '
            resources.ApplyResources(Me.m_hdr1, "m_hdr1")
            Me.m_hdr1.Name = "m_hdr1"
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
            Me.m_gbLegends.Controls.Add(Me.m_rbLegendAlways)
            Me.m_gbLegends.Controls.Add(Me.m_rbLegendSelective)
            Me.m_gbLegends.Name = "m_gbLegends"
            Me.m_gbLegends.TabStop = False
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
            'm_nudFontSize
            '
            resources.ApplyResources(Me.m_nudFontSize, "m_nudFontSize")
            Me.m_nudFontSize.DecimalPlaces = 2
            Me.m_nudFontSize.Maximum = New Decimal(New Integer() {24, 0, 0, 0})
            Me.m_nudFontSize.Minimum = New Decimal(New Integer() {4, 0, 0, 0})
            Me.m_nudFontSize.Name = "m_nudFontSize"
            Me.m_nudFontSize.Value = New Decimal(New Integer() {825, 0, 0, 131072})
            '
            'm_lblFontSize
            '
            resources.ApplyResources(Me.m_lblFontSize, "m_lblFontSize")
            Me.m_lblFontSize.Name = "m_lblFontSize"
            '
            'm_lblExample
            '
            resources.ApplyResources(Me.m_lblExample, "m_lblExample")
            Me.m_lblExample.Name = "m_lblExample"
            '
            'm_hdr2
            '
            resources.ApplyResources(Me.m_hdr2, "m_hdr2")
            Me.m_hdr2.Name = "m_hdr2"
            '
            'm_gbxExample
            '
            resources.ApplyResources(Me.m_gbxExample, "m_gbxExample")
            Me.m_gbxExample.Controls.Add(Me.m_lblExample)
            Me.m_gbxExample.Name = "m_gbxExample"
            Me.m_gbxExample.TabStop = False
            '
            'm_cbFontStyle
            '
            resources.ApplyResources(Me.m_cbFontStyle, "m_cbFontStyle")
            Me.m_cbFontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbFontStyle.FormattingEnabled = True
            Me.m_cbFontStyle.Items.AddRange(New Object() {resources.GetString("m_cbFontStyle.Items"), resources.GetString("m_cbFontStyle.Items1"), resources.GetString("m_cbFontStyle.Items2"), resources.GetString("m_cbFontStyle.Items3")})
            Me.m_cbFontStyle.Name = "m_cbFontStyle"
            '
            'm_lblItemFontStyle
            '
            resources.ApplyResources(Me.m_lblItemFontStyle, "m_lblItemFontStyle")
            Me.m_lblItemFontStyle.Name = "m_lblItemFontStyle"
            '
            'm_cbFontFamily
            '
            resources.ApplyResources(Me.m_cbFontFamily, "m_cbFontFamily")
            Me.m_cbFontFamily.BackColor = System.Drawing.Color.White
            Me.m_cbFontFamily.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_cbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbFontFamily.FormattingEnabled = True
            Me.m_cbFontFamily.Name = "m_cbFontFamily"
            '
            'lblItemForeColor
            '
            resources.ApplyResources(Me.lblItemForeColor, "lblItemForeColor")
            Me.lblItemForeColor.Name = "lblItemForeColor"
            '
            'm_btnResetFonts
            '
            resources.ApplyResources(Me.m_btnResetFonts, "m_btnResetFonts")
            Me.m_btnResetFonts.Name = "m_btnResetFonts"
            Me.m_btnResetFonts.UseVisualStyleBackColor = True
            '
            'm_lbFontTypes
            '
            resources.ApplyResources(Me.m_lbFontTypes, "m_lbFontTypes")
            Me.m_lbFontTypes.Name = "m_lbFontTypes"
            '
            'ucAppGraphs
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudFontSize)
            Me.Controls.Add(Me.m_lblFontSize)
            Me.Controls.Add(Me.m_hdr2)
            Me.Controls.Add(Me.m_gbxExample)
            Me.Controls.Add(Me.m_cbFontStyle)
            Me.Controls.Add(Me.m_lblItemFontStyle)
            Me.Controls.Add(Me.m_cbFontFamily)
            Me.Controls.Add(Me.lblItemForeColor)
            Me.Controls.Add(Me.m_btnResetFonts)
            Me.Controls.Add(Me.m_lbFontTypes)
            Me.Controls.Add(Me.m_gbThumbnails)
            Me.Controls.Add(Me.m_gbLegends)
            Me.Controls.Add(Me.m_hdr1)
            Me.Name = "ucAppGraphs"
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbLegends.ResumeLayout(False)
            Me.m_gbLegends.PerformLayout()
            Me.m_gbThumbnails.ResumeLayout(False)
            Me.m_gbThumbnails.PerformLayout()
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbxExample.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudThumbnailSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_hdr1 As cEwEHeaderLabel
        Private WithEvents m_lblThumbnailSize As System.Windows.Forms.Label
        Private WithEvents m_gbLegends As System.Windows.Forms.GroupBox
        Private WithEvents m_rbLegendAlways As System.Windows.Forms.RadioButton
        Private WithEvents m_rbLegendSelective As System.Windows.Forms.RadioButton
        Private WithEvents m_gbThumbnails As System.Windows.Forms.GroupBox
        Private WithEvents m_lblThumbnailUnit As System.Windows.Forms.Label
        Private WithEvents m_nudFontSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblFontSize As System.Windows.Forms.Label
        Private WithEvents m_lblExample As System.Windows.Forms.Label
        Private WithEvents m_hdr2 As cEwEHeaderLabel
        Private WithEvents m_gbxExample As System.Windows.Forms.GroupBox
        Private WithEvents m_lblItemFontStyle As System.Windows.Forms.Label
        Private WithEvents m_cbFontFamily As System.Windows.Forms.ComboBox
        Private WithEvents lblItemForeColor As System.Windows.Forms.Label
        Private WithEvents m_btnResetFonts As System.Windows.Forms.Button
        Private WithEvents m_lbFontTypes As System.Windows.Forms.ListBox
        Private WithEvents m_cbFontStyle As System.Windows.Forms.ComboBox

    End Class
End Namespace

