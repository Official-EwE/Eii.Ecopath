
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEBatch
    Inherits DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btSelect = New System.Windows.Forms.Button
        Me.lbCommandFile = New System.Windows.Forms.Label
        Me.lbCommandFileDir = New System.Windows.Forms.Label
        Me.btRun = New System.Windows.Forms.Button
        Me.lstOutput = New System.Windows.Forms.ListBox
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.CEwEHeaderLabel3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.btStop = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btSelect
        '
        Me.btSelect.Location = New System.Drawing.Point(16, 41)
        Me.btSelect.Name = "btSelect"
        Me.btSelect.Size = New System.Drawing.Size(181, 21)
        Me.btSelect.TabIndex = 1
        Me.btSelect.Text = "Select command file..."
        Me.btSelect.UseVisualStyleBackColor = True
        '
        'lbCommandFile
        '
        Me.lbCommandFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbCommandFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lbCommandFile.Location = New System.Drawing.Point(16, 76)
        Me.lbCommandFile.Name = "lbCommandFile"
        Me.lbCommandFile.Size = New System.Drawing.Size(462, 17)
        Me.lbCommandFile.TabIndex = 2
        '
        'lbCommandFileDir
        '
        Me.lbCommandFileDir.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbCommandFileDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lbCommandFileDir.Location = New System.Drawing.Point(16, 103)
        Me.lbCommandFileDir.Name = "lbCommandFileDir"
        Me.lbCommandFileDir.Size = New System.Drawing.Size(462, 17)
        Me.lbCommandFileDir.TabIndex = 3
        '
        'btRun
        '
        Me.btRun.Location = New System.Drawing.Point(16, 162)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(85, 21)
        Me.btRun.TabIndex = 4
        Me.btRun.Text = "Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'lstOutput
        '
        Me.lstOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstOutput.FormattingEnabled = True
        Me.lstOutput.HorizontalScrollbar = True
        Me.lstOutput.Location = New System.Drawing.Point(16, 222)
        Me.lstOutput.Name = "lstOutput"
        Me.lstOutput.Size = New System.Drawing.Size(462, 199)
        Me.lstOutput.TabIndex = 5
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(5, 9)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(481, 19)
        Me.CEwEHeaderLabel1.TabIndex = 6
        Me.CEwEHeaderLabel1.Text = "MSE batch command file"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel2.Location = New System.Drawing.Point(5, 134)
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        Me.CEwEHeaderLabel2.Size = New System.Drawing.Size(481, 19)
        Me.CEwEHeaderLabel2.TabIndex = 7
        Me.CEwEHeaderLabel2.Text = "Run"
        Me.CEwEHeaderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CEwEHeaderLabel3
        '
        Me.CEwEHeaderLabel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel3.Location = New System.Drawing.Point(5, 196)
        Me.CEwEHeaderLabel3.Name = "CEwEHeaderLabel3"
        Me.CEwEHeaderLabel3.Size = New System.Drawing.Size(481, 19)
        Me.CEwEHeaderLabel3.TabIndex = 8
        Me.CEwEHeaderLabel3.Text = "Output"
        Me.CEwEHeaderLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btStop
        '
        Me.btStop.Location = New System.Drawing.Point(112, 162)
        Me.btStop.Name = "btStop"
        Me.btStop.Size = New System.Drawing.Size(85, 21)
        Me.btStop.TabIndex = 9
        Me.btStop.Text = "Stop run"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'frmMSEBatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(490, 433)
        Me.Controls.Add(Me.btStop)
        Me.Controls.Add(Me.CEwEHeaderLabel3)
        Me.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.lstOutput)
        Me.Controls.Add(Me.btRun)
        Me.Controls.Add(Me.lbCommandFileDir)
        Me.Controls.Add(Me.lbCommandFile)
        Me.Controls.Add(Me.btSelect)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSEBatch"
        Me.Text = "MSE batch command file loader"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btSelect As System.Windows.Forms.Button
    Friend WithEvents lbCommandFile As System.Windows.Forms.Label
    Friend WithEvents lbCommandFileDir As System.Windows.Forms.Label
    Friend WithEvents btRun As System.Windows.Forms.Button
    Friend WithEvents lstOutput As System.Windows.Forms.ListBox
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents CEwEHeaderLabel3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents btStop As System.Windows.Forms.Button
End Class
