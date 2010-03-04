Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorPort
        Inherits ucLayerEditorFleet

        'UserControl overrides dispose to clean up the component list.
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
            Me.m_tlpButtons = New System.Windows.Forms.TableLayoutPanel
            Me.m_btnClear = New System.Windows.Forms.Button
            Me.m_btnSet = New System.Windows.Forms.Button
            Me.m_tlpButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpButtons
            '
            Me.m_tlpButtons.ColumnCount = 2
            Me.m_tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.Controls.Add(Me.m_btnClear, 0, 0)
            Me.m_tlpButtons.Controls.Add(Me.m_btnSet, 1, 0)
            Me.m_tlpButtons.Location = New System.Drawing.Point(49, 48)
            Me.m_tlpButtons.Name = "m_tlpButtons"
            Me.m_tlpButtons.RowCount = 1
            Me.m_tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.Size = New System.Drawing.Size(148, 23)
            Me.m_tlpButtons.TabIndex = 3
            '
            'm_btnClear
            '
            Me.m_btnClear.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.m_btnClear.Location = New System.Drawing.Point(0, 0)
            Me.m_btnClear.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.m_btnClear.Name = "m_btnClear"
            Me.m_btnClear.Size = New System.Drawing.Size(71, 23)
            Me.m_btnClear.TabIndex = 0
            Me.m_btnClear.Text = "&Clear"
            Me.m_btnClear.UseVisualStyleBackColor = True
            '
            'm_btnSet
            '
            Me.m_btnSet.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_btnSet.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.m_btnSet.Location = New System.Drawing.Point(77, 0)
            Me.m_btnSet.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.m_btnSet.Name = "m_btnSet"
            Me.m_btnSet.Size = New System.Drawing.Size(71, 23)
            Me.m_btnSet.TabIndex = 0
            Me.m_btnSet.Text = "&All coasts"
            Me.m_btnSet.UseVisualStyleBackColor = True
            '
            'ucLayerEditorPort
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpButtons)
            Me.Name = "ucLayerEditorPort"
            Me.Size = New System.Drawing.Size(200, 82)
            Me.Controls.SetChildIndex(Me.m_tlpButtons, 0)
            Me.m_tlpButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tlpButtons As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_btnClear As System.Windows.Forms.Button
        Private WithEvents m_btnSet As System.Windows.Forms.Button

    End Class

End Namespace
