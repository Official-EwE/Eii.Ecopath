Imports ScientificInterfaceShared.Controls

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
            Me.m_lbItems = New System.Windows.Forms.ListBox
            Me.m_btnResetAll = New System.Windows.Forms.Button
            Me.m_lblItemForeColor = New System.Windows.Forms.Label
            Me.m_cmbItemForeground = New System.Windows.Forms.ComboBox
            Me.m_lblItemBackColor = New System.Windows.Forms.Label
            Me.m_btnCustomForeColor = New System.Windows.Forms.Button
            Me.m_cmbItemBackground = New System.Windows.Forms.ComboBox
            Me.m_btnCustomBackColor = New System.Windows.Forms.Button
            Me.m_grpExample = New System.Windows.Forms.GroupBox
            Me.m_lblExample = New System.Windows.Forms.Label
            Me.m_hdrCaption = New cEwEHeaderLabel
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lblSelection = New System.Windows.Forms.Label
            Me.m_lblColorItem = New System.Windows.Forms.Label
            Me.m_grpExample.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lbItems
            '
            resources.ApplyResources(Me.m_lbItems, "m_lbItems")
            Me.m_lbItems.FormattingEnabled = True
            Me.m_lbItems.Name = "m_lbItems"
            Me.m_lbItems.Sorted = True
            '
            'm_btnResetAll
            '
            resources.ApplyResources(Me.m_btnResetAll, "m_btnResetAll")
            Me.m_btnResetAll.Name = "m_btnResetAll"
            Me.m_btnResetAll.UseVisualStyleBackColor = True
            '
            'm_lblItemForeColor
            '
            resources.ApplyResources(Me.m_lblItemForeColor, "m_lblItemForeColor")
            Me.m_lblItemForeColor.Name = "m_lblItemForeColor"
            '
            'm_cmbItemForeground
            '
            resources.ApplyResources(Me.m_cmbItemForeground, "m_cmbItemForeground")
            Me.m_cmbItemForeground.BackColor = System.Drawing.Color.White
            Me.m_cmbItemForeground.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_cmbItemForeground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbItemForeground.FormattingEnabled = True
            Me.m_cmbItemForeground.Name = "m_cmbItemForeground"
            '
            'm_lblItemBackColor
            '
            resources.ApplyResources(Me.m_lblItemBackColor, "m_lblItemBackColor")
            Me.m_lblItemBackColor.Name = "m_lblItemBackColor"
            '
            'm_btnCustomForeColor
            '
            resources.ApplyResources(Me.m_btnCustomForeColor, "m_btnCustomForeColor")
            Me.m_btnCustomForeColor.Name = "m_btnCustomForeColor"
            Me.m_btnCustomForeColor.UseVisualStyleBackColor = True
            '
            'm_cmbItemBackground
            '
            resources.ApplyResources(Me.m_cmbItemBackground, "m_cmbItemBackground")
            Me.m_cmbItemBackground.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_cmbItemBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbItemBackground.FormattingEnabled = True
            Me.m_cmbItemBackground.Name = "m_cmbItemBackground"
            '
            'm_btnCustomBackColor
            '
            resources.ApplyResources(Me.m_btnCustomBackColor, "m_btnCustomBackColor")
            Me.m_btnCustomBackColor.Name = "m_btnCustomBackColor"
            Me.m_btnCustomBackColor.UseVisualStyleBackColor = True
            '
            'm_grpExample
            '
            resources.ApplyResources(Me.m_grpExample, "m_grpExample")
            Me.m_grpExample.Controls.Add(Me.m_lblExample)
            Me.m_grpExample.Name = "m_grpExample"
            Me.m_grpExample.TabStop = False
            '
            'm_lblExample
            '
            resources.ApplyResources(Me.m_lblExample, "m_lblExample")
            Me.m_lblExample.Name = "m_lblExample"
            '
            'm_hdrCaption
            '
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.Name = "m_hdrCaption"
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
            'm_lblColorItem
            '
            resources.ApplyResources(Me.m_lblColorItem, "m_lblColorItem")
            Me.m_lblColorItem.Name = "m_lblColorItem"
            '
            'ucAppColors
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_lblSelection)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Controls.Add(Me.m_grpExample)
            Me.Controls.Add(Me.m_btnCustomBackColor)
            Me.Controls.Add(Me.m_cmbItemBackground)
            Me.Controls.Add(Me.m_btnCustomForeColor)
            Me.Controls.Add(Me.m_lblItemBackColor)
            Me.Controls.Add(Me.m_cmbItemForeground)
            Me.Controls.Add(Me.m_lblColorItem)
            Me.Controls.Add(Me.m_lblItemForeColor)
            Me.Controls.Add(Me.m_btnResetAll)
            Me.Controls.Add(Me.m_lbItems)
            Me.Name = "ucAppColors"
            Me.m_grpExample.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnResetAll As System.Windows.Forms.Button
        Private WithEvents m_lblItemForeColor As System.Windows.Forms.Label
        Private WithEvents m_cmbItemForeground As System.Windows.Forms.ComboBox
        Private WithEvents m_lblItemBackColor As System.Windows.Forms.Label
        Private WithEvents m_btnCustomForeColor As System.Windows.Forms.Button
        Private WithEvents m_cmbItemBackground As System.Windows.Forms.ComboBox
        Private WithEvents m_btnCustomBackColor As System.Windows.Forms.Button
        Private WithEvents m_grpExample As System.Windows.Forms.GroupBox
        Private m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_lblExample As System.Windows.Forms.Label
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents m_lblSelection As System.Windows.Forms.Label
        Private WithEvents m_lbItems As System.Windows.Forms.ListBox
        Private WithEvents m_lblColorItem As System.Windows.Forms.Label

    End Class
End Namespace

