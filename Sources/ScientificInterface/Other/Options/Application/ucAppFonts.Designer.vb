Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppFonts
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppFonts))
            Me.m_lbFontTypes = New System.Windows.Forms.ListBox
            Me.btnUseDefault = New System.Windows.Forms.Button
            Me.lblItemForeColor = New System.Windows.Forms.Label
            Me.m_cbFontFamily = New System.Windows.Forms.ComboBox
            Me.m_lblItemFontStyle = New System.Windows.Forms.Label
            Me.m_cbFontStyle = New System.Windows.Forms.ComboBox
            Me.gbpExample = New System.Windows.Forms.GroupBox
            Me.m_lblExample = New System.Windows.Forms.Label
            Me.m_lblFontHeader = New System.Windows.Forms.Label
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lblSelection = New System.Windows.Forms.Label
            Me.m_lblFontSize = New System.Windows.Forms.Label
            Me.m_nudFontSize = New System.Windows.Forms.NumericUpDown
            Me.gbpExample.SuspendLayout()
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lbFontTypes
            '
            resources.ApplyResources(Me.m_lbFontTypes, "m_lbFontTypes")
            Me.m_lbFontTypes.Name = "m_lbFontTypes"
            '
            'btnUseDefault
            '
            resources.ApplyResources(Me.btnUseDefault, "btnUseDefault")
            Me.btnUseDefault.Name = "btnUseDefault"
            Me.btnUseDefault.UseVisualStyleBackColor = True
            '
            'lblItemForeColor
            '
            resources.ApplyResources(Me.lblItemForeColor, "lblItemForeColor")
            Me.lblItemForeColor.Name = "lblItemForeColor"
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
            'm_lblItemFontStyle
            '
            resources.ApplyResources(Me.m_lblItemFontStyle, "m_lblItemFontStyle")
            Me.m_lblItemFontStyle.Name = "m_lblItemFontStyle"
            '
            'm_cbFontStyle
            '
            resources.ApplyResources(Me.m_cbFontStyle, "m_cbFontStyle")
            Me.m_cbFontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbFontStyle.FormattingEnabled = True
            Me.m_cbFontStyle.Items.AddRange(New Object() {resources.GetString("m_cbFontStyle.Items"), resources.GetString("m_cbFontStyle.Items1"), resources.GetString("m_cbFontStyle.Items2"), resources.GetString("m_cbFontStyle.Items3")})
            Me.m_cbFontStyle.Name = "m_cbFontStyle"
            '
            'gbpExample
            '
            resources.ApplyResources(Me.gbpExample, "gbpExample")
            Me.gbpExample.Controls.Add(Me.m_lblExample)
            Me.gbpExample.Name = "gbpExample"
            Me.gbpExample.TabStop = False
            '
            'm_lblExample
            '
            resources.ApplyResources(Me.m_lblExample, "m_lblExample")
            Me.m_lblExample.Name = "m_lblExample"
            '
            'm_lblFontHeader
            '
            Me.m_lblFontHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.m_lblFontHeader, "m_lblFontHeader")
            Me.m_lblFontHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblFontHeader.Name = "m_lblFontHeader"
            '
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_lblSelection
            '
            resources.ApplyResources(Me.m_lblSelection, "m_lblSelection")
            Me.m_lblSelection.Name = "m_lblSelection"
            '
            'm_lblFontSize
            '
            resources.ApplyResources(Me.m_lblFontSize, "m_lblFontSize")
            Me.m_lblFontSize.Name = "m_lblFontSize"
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
            'ucAppFonts
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudFontSize)
            Me.Controls.Add(Me.m_lblFontSize)
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_lblSelection)
            Me.Controls.Add(Me.m_lblFontHeader)
            Me.Controls.Add(Me.gbpExample)
            Me.Controls.Add(Me.m_cbFontStyle)
            Me.Controls.Add(Me.m_lblItemFontStyle)
            Me.Controls.Add(Me.m_cbFontFamily)
            Me.Controls.Add(Me.lblItemForeColor)
            Me.Controls.Add(Me.btnUseDefault)
            Me.Controls.Add(Me.m_lbFontTypes)
            Me.Name = "ucAppFonts"
            Me.gbpExample.ResumeLayout(False)
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_lbFontTypes As System.Windows.Forms.ListBox
        Friend WithEvents btnUseDefault As System.Windows.Forms.Button
        Friend WithEvents lblItemForeColor As System.Windows.Forms.Label
        Friend WithEvents m_cbFontFamily As System.Windows.Forms.ComboBox
        Friend WithEvents m_lblItemFontStyle As System.Windows.Forms.Label
        Friend WithEvents m_cbFontStyle As System.Windows.Forms.ComboBox
        Friend WithEvents gbpExample As System.Windows.Forms.GroupBox
        Friend WithEvents m_lblFontHeader As System.Windows.Forms.Label
        Friend WithEvents m_lblExample As System.Windows.Forms.Label
        Friend WithEvents m_lblDescription As System.Windows.Forms.Label
        Friend WithEvents m_lblSelection As System.Windows.Forms.Label
        Friend WithEvents m_lblFontSize As System.Windows.Forms.Label
        Friend WithEvents m_nudFontSize As System.Windows.Forms.NumericUpDown

    End Class
End Namespace

