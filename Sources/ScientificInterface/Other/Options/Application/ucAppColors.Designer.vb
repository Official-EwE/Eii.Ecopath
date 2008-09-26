Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppColors
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppColors))
            Me.lbItems = New System.Windows.Forms.ListBox
            Me.btnUseDefault = New System.Windows.Forms.Button
            Me.lblItemForeColor = New System.Windows.Forms.Label
            Me.cbItemForeground = New System.Windows.Forms.ComboBox
            Me.lblItemBackColor = New System.Windows.Forms.Label
            Me.btnCustomForeColor = New System.Windows.Forms.Button
            Me.cbItemBackground = New System.Windows.Forms.ComboBox
            Me.btnCustomBackColor = New System.Windows.Forms.Button
            Me.gbpExample = New System.Windows.Forms.GroupBox
            Me.m_lblExample = New System.Windows.Forms.Label
            Me.lblColorHeader = New System.Windows.Forms.Label
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lblSelection = New System.Windows.Forms.Label
            Me.gbpExample.SuspendLayout()
            Me.SuspendLayout()
            '
            'lbItems
            '
            resources.ApplyResources(Me.lbItems, "lbItems")
            Me.lbItems.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lbItems.FormattingEnabled = True
            Me.lbItems.Name = "lbItems"
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
            'cbItemForeground
            '
            resources.ApplyResources(Me.cbItemForeground, "cbItemForeground")
            Me.cbItemForeground.BackColor = System.Drawing.Color.White
            Me.cbItemForeground.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.cbItemForeground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbItemForeground.FormattingEnabled = True
            Me.cbItemForeground.Name = "cbItemForeground"
            '
            'lblItemBackColor
            '
            resources.ApplyResources(Me.lblItemBackColor, "lblItemBackColor")
            Me.lblItemBackColor.Name = "lblItemBackColor"
            '
            'btnCustomForeColor
            '
            resources.ApplyResources(Me.btnCustomForeColor, "btnCustomForeColor")
            Me.btnCustomForeColor.Name = "btnCustomForeColor"
            Me.btnCustomForeColor.UseVisualStyleBackColor = True
            '
            'cbItemBackground
            '
            resources.ApplyResources(Me.cbItemBackground, "cbItemBackground")
            Me.cbItemBackground.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.cbItemBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbItemBackground.FormattingEnabled = True
            Me.cbItemBackground.Name = "cbItemBackground"
            '
            'btnCustomBackColor
            '
            resources.ApplyResources(Me.btnCustomBackColor, "btnCustomBackColor")
            Me.btnCustomBackColor.Name = "btnCustomBackColor"
            Me.btnCustomBackColor.UseVisualStyleBackColor = True
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
            'lblColorHeader
            '
            Me.lblColorHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.lblColorHeader, "lblColorHeader")
            Me.lblColorHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblColorHeader.Name = "lblColorHeader"
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
            'ucAppColors
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_lblSelection)
            Me.Controls.Add(Me.lblColorHeader)
            Me.Controls.Add(Me.gbpExample)
            Me.Controls.Add(Me.btnCustomBackColor)
            Me.Controls.Add(Me.cbItemBackground)
            Me.Controls.Add(Me.btnCustomForeColor)
            Me.Controls.Add(Me.lblItemBackColor)
            Me.Controls.Add(Me.cbItemForeground)
            Me.Controls.Add(Me.lblItemForeColor)
            Me.Controls.Add(Me.btnUseDefault)
            Me.Controls.Add(Me.lbItems)
            Me.Name = "ucAppColors"
            Me.gbpExample.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents lbItems As System.Windows.Forms.ListBox
        Friend WithEvents btnUseDefault As System.Windows.Forms.Button
        Friend WithEvents lblItemForeColor As System.Windows.Forms.Label
        Friend WithEvents cbItemForeground As System.Windows.Forms.ComboBox
        Friend WithEvents lblItemBackColor As System.Windows.Forms.Label
        Friend WithEvents btnCustomForeColor As System.Windows.Forms.Button
        Friend WithEvents cbItemBackground As System.Windows.Forms.ComboBox
        Friend WithEvents btnCustomBackColor As System.Windows.Forms.Button
        Friend WithEvents gbpExample As System.Windows.Forms.GroupBox
        Friend WithEvents lblColorHeader As System.Windows.Forms.Label
        Friend WithEvents m_lblExample As System.Windows.Forms.Label
        Friend WithEvents m_lblDescription As System.Windows.Forms.Label
        Friend WithEvents m_lblSelection As System.Windows.Forms.Label

    End Class
End Namespace

