<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgDefineMapResponseAssignments
    Inherits System.Windows.Forms.Form

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
        Me.components = New System.ComponentModel.Container
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.lvMaps = New System.Windows.Forms.ListView
        Me.Label1 = New System.Windows.Forms.Label
        Me.txXMax = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.ZedGraph = New ZedGraph.ZedGraphControl
        Me.txXMin = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.btDefaultMinMax = New System.Windows.Forms.Button
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(654, 294)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'lvMaps
        '
        Me.lvMaps.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lvMaps.Location = New System.Drawing.Point(12, 29)
        Me.lvMaps.Name = "lvMaps"
        Me.lvMaps.Size = New System.Drawing.Size(161, 259)
        Me.lvMaps.TabIndex = 1
        Me.lvMaps.UseCompatibleStateImageBehavior = False
        Me.lvMaps.View = System.Windows.Forms.View.List
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(117, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Input maps for capacity"
        '
        'txXMax
        '
        Me.txXMax.Location = New System.Drawing.Point(565, 26)
        Me.txXMax.Name = "txXMax"
        Me.txXMax.Size = New System.Drawing.Size(85, 20)
        Me.txXMax.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(428, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(131, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Response function X max."
        '
        'ZedGraph
        '
        Me.ZedGraph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ZedGraph.Location = New System.Drawing.Point(190, 52)
        Me.ZedGraph.Name = "ZedGraph"
        Me.ZedGraph.ScrollGrace = 0
        Me.ZedGraph.ScrollMaxX = 0
        Me.ZedGraph.ScrollMaxY = 0
        Me.ZedGraph.ScrollMaxY2 = 0
        Me.ZedGraph.ScrollMinX = 0
        Me.ZedGraph.ScrollMinY = 0
        Me.ZedGraph.ScrollMinY2 = 0
        Me.ZedGraph.Size = New System.Drawing.Size(610, 236)
        Me.ZedGraph.TabIndex = 5
        '
        'txXMin
        '
        Me.txXMin.Location = New System.Drawing.Point(321, 26)
        Me.txXMin.Name = "txXMin"
        Me.txXMin.Size = New System.Drawing.Size(82, 20)
        Me.txXMin.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(187, 29)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(128, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Response function X min."
        '
        'btDefaultMinMax
        '
        Me.btDefaultMinMax.Location = New System.Drawing.Point(694, 23)
        Me.btDefaultMinMax.Name = "btDefaultMinMax"
        Me.btDefaultMinMax.Size = New System.Drawing.Size(103, 23)
        Me.btDefaultMinMax.TabIndex = 8
        Me.btDefaultMinMax.Text = "Default min. max."
        Me.btDefaultMinMax.UseVisualStyleBackColor = True
        '
        'dlgDefineMapResponseAssignments
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(812, 335)
        Me.Controls.Add(Me.btDefaultMinMax)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txXMin)
        Me.Controls.Add(Me.ZedGraph)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txXMax)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lvMaps)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgDefineMapResponseAssignments"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Response to input map"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents lvMaps As System.Windows.Forms.ListView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txXMax As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ZedGraph As ZedGraph.ZedGraphControl
    Friend WithEvents txXMin As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btDefaultMinMax As System.Windows.Forms.Button

End Class
