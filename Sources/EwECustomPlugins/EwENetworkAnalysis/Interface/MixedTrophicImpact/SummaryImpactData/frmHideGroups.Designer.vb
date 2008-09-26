
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHideGroups
    Inherits System.Windows.Forms.Form

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
        Me.gpbDisplayedGrps = New System.Windows.Forms.GroupBox
        Me.lbDisplayedGrps = New System.Windows.Forms.ListBox
        Me.gpbHiddenGrps = New System.Windows.Forms.GroupBox
        Me.lbHiddenGrps = New System.Windows.Forms.ListBox
        Me.btnHideOne = New System.Windows.Forms.Button
        Me.btnShowAll = New System.Windows.Forms.Button
        Me.btnHideAll = New System.Windows.Forms.Button
        Me.btnShowOne = New System.Windows.Forms.Button
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.OK_Button = New System.Windows.Forms.Button
        Me.gpbDisplayedGrps.SuspendLayout()
        Me.gpbHiddenGrps.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpbDisplayedGrps
        '
        Me.gpbDisplayedGrps.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gpbDisplayedGrps.Controls.Add(Me.lbDisplayedGrps)
        Me.gpbDisplayedGrps.Location = New System.Drawing.Point(25, 23)
        Me.gpbDisplayedGrps.Name = "gpbDisplayedGrps"
        Me.gpbDisplayedGrps.Size = New System.Drawing.Size(220, 410)
        Me.gpbDisplayedGrps.TabIndex = 3
        Me.gpbDisplayedGrps.TabStop = False
        Me.gpbDisplayedGrps.Text = "Displayed groups"
        '
        'lbDisplayedGrps
        '
        Me.lbDisplayedGrps.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbDisplayedGrps.FormattingEnabled = True
        Me.lbDisplayedGrps.Location = New System.Drawing.Point(3, 16)
        Me.lbDisplayedGrps.Name = "lbDisplayedGrps"
        Me.lbDisplayedGrps.Size = New System.Drawing.Size(214, 381)
        Me.lbDisplayedGrps.TabIndex = 0
        '
        'gpbHiddenGrps
        '
        Me.gpbHiddenGrps.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gpbHiddenGrps.Controls.Add(Me.lbHiddenGrps)
        Me.gpbHiddenGrps.Location = New System.Drawing.Point(354, 23)
        Me.gpbHiddenGrps.Name = "gpbHiddenGrps"
        Me.gpbHiddenGrps.Size = New System.Drawing.Size(220, 410)
        Me.gpbHiddenGrps.TabIndex = 4
        Me.gpbHiddenGrps.TabStop = False
        Me.gpbHiddenGrps.Text = "Hidden groups"
        '
        'lbHiddenGrps
        '
        Me.lbHiddenGrps.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbHiddenGrps.FormattingEnabled = True
        Me.lbHiddenGrps.Location = New System.Drawing.Point(3, 16)
        Me.lbHiddenGrps.Name = "lbHiddenGrps"
        Me.lbHiddenGrps.Size = New System.Drawing.Size(214, 381)
        Me.lbHiddenGrps.TabIndex = 0
        '
        'btnHideOne
        '
        Me.btnHideOne.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnHideOne.Location = New System.Drawing.Point(273, 83)
        Me.btnHideOne.Name = "btnHideOne"
        Me.btnHideOne.Size = New System.Drawing.Size(49, 23)
        Me.btnHideOne.TabIndex = 5
        Me.btnHideOne.Text = "->"
        Me.btnHideOne.UseVisualStyleBackColor = True
        '
        'btnShowAll
        '
        Me.btnShowAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowAll.Location = New System.Drawing.Point(273, 196)
        Me.btnShowAll.Name = "btnShowAll"
        Me.btnShowAll.Size = New System.Drawing.Size(49, 23)
        Me.btnShowAll.TabIndex = 9
        Me.btnShowAll.Text = "<<-"
        Me.btnShowAll.UseVisualStyleBackColor = True
        '
        'btnHideAll
        '
        Me.btnHideAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnHideAll.Location = New System.Drawing.Point(273, 167)
        Me.btnHideAll.Name = "btnHideAll"
        Me.btnHideAll.Size = New System.Drawing.Size(49, 23)
        Me.btnHideAll.TabIndex = 10
        Me.btnHideAll.Text = "->>"
        Me.btnHideAll.UseVisualStyleBackColor = True
        '
        'btnShowOne
        '
        Me.btnShowOne.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowOne.Location = New System.Drawing.Point(273, 112)
        Me.btnShowOne.Name = "btnShowOne"
        Me.btnShowOne.Size = New System.Drawing.Size(49, 23)
        Me.btnShowOne.TabIndex = 11
        Me.btnShowOne.Text = "<-"
        Me.btnShowOne.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(425, 444)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 12
        '
        'Cancel_Button
        '
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "&Cancel"
        '
        'OK_Button
        '
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "&OK"
        '
        'frmHideGroups
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(597, 485)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.btnShowOne)
        Me.Controls.Add(Me.btnHideAll)
        Me.Controls.Add(Me.btnShowAll)
        Me.Controls.Add(Me.btnHideOne)
        Me.Controls.Add(Me.gpbHiddenGrps)
        Me.Controls.Add(Me.gpbDisplayedGrps)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmHideGroups"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Show/hide groups in mixed trophic impact plot"
        Me.gpbDisplayedGrps.ResumeLayout(False)
        Me.gpbHiddenGrps.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpbDisplayedGrps As System.Windows.Forms.GroupBox
    Friend WithEvents gpbHiddenGrps As System.Windows.Forms.GroupBox
    Friend WithEvents lbDisplayedGrps As System.Windows.Forms.ListBox
    Friend WithEvents lbHiddenGrps As System.Windows.Forms.ListBox
    Friend WithEvents btnHideOne As System.Windows.Forms.Button
    Friend WithEvents btnShowAll As System.Windows.Forms.Button
    Friend WithEvents btnHideAll As System.Windows.Forms.Button
    Friend WithEvents btnShowOne As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
End Class


