Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEwEPlugin
    Inherits Dockcontent

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
        Me.btnChangeVariables = New System.Windows.Forms.Button
        Me.btnRunEcosim = New System.Windows.Forms.Button
        Me.btnMakeDataGrid = New System.Windows.Forms.Button
        Me.btnPlotEcosim = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.btnMakeOwnPlugin = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btnChangeVariables
        '
        Me.btnChangeVariables.Location = New System.Drawing.Point(12, 12)
        Me.btnChangeVariables.Name = "btnChangeVariables"
        Me.btnChangeVariables.Size = New System.Drawing.Size(196, 38)
        Me.btnChangeVariables.TabIndex = 0
        Me.btnChangeVariables.Text = "Change Variables"
        Me.btnChangeVariables.UseVisualStyleBackColor = True
        '
        'btnRunEcosim
        '
        Me.btnRunEcosim.Location = New System.Drawing.Point(12, 101)
        Me.btnRunEcosim.Name = "btnRunEcosim"
        Me.btnRunEcosim.Size = New System.Drawing.Size(196, 38)
        Me.btnRunEcosim.TabIndex = 1
        Me.btnRunEcosim.Text = "Run Ecosim"
        Me.btnRunEcosim.UseVisualStyleBackColor = True
        '
        'btnMakeDataGrid
        '
        Me.btnMakeDataGrid.Location = New System.Drawing.Point(12, 145)
        Me.btnMakeDataGrid.Name = "btnMakeDataGrid"
        Me.btnMakeDataGrid.Size = New System.Drawing.Size(196, 38)
        Me.btnMakeDataGrid.TabIndex = 2
        Me.btnMakeDataGrid.Text = "Make data grid"
        Me.btnMakeDataGrid.UseVisualStyleBackColor = True
        '
        'btnPlotEcosim
        '
        Me.btnPlotEcosim.Location = New System.Drawing.Point(12, 189)
        Me.btnPlotEcosim.Name = "btnPlotEcosim"
        Me.btnPlotEcosim.Size = New System.Drawing.Size(196, 38)
        Me.btnPlotEcosim.TabIndex = 3
        Me.btnPlotEcosim.Text = "Plot Ecosim"
        Me.btnPlotEcosim.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Location = New System.Drawing.Point(214, 11)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(347, 470)
        Me.Panel1.TabIndex = 4
        '
        'btnMakeOwnPlugin
        '
        Me.btnMakeOwnPlugin.Location = New System.Drawing.Point(12, 56)
        Me.btnMakeOwnPlugin.Name = "btnMakeOwnPlugin"
        Me.btnMakeOwnPlugin.Size = New System.Drawing.Size(196, 38)
        Me.btnMakeOwnPlugin.TabIndex = 5
        Me.btnMakeOwnPlugin.Text = "Make your own plugin"
        Me.btnMakeOwnPlugin.UseVisualStyleBackColor = True
        '
        'frmEwEPluginForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(573, 493)
        Me.Controls.Add(Me.btnMakeOwnPlugin)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.btnPlotEcosim)
        Me.Controls.Add(Me.btnMakeDataGrid)
        Me.Controls.Add(Me.btnRunEcosim)
        Me.Controls.Add(Me.btnChangeVariables)
        Me.Name = "frmEwEPluginForm"
        Me.TabText = "frmEwEPluginForm"
        Me.Text = "EwE Plugin Examples"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnChangeVariables As System.Windows.Forms.Button
    Friend WithEvents btnRunEcosim As System.Windows.Forms.Button
    Friend WithEvents btnMakeDataGrid As System.Windows.Forms.Button
    Friend WithEvents btnPlotEcosim As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnMakeOwnPlugin As System.Windows.Forms.Button
End Class
