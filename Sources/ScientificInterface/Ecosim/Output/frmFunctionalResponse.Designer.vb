Imports ZedGraph

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFunctionalResponse
        Inherits frmEwE

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
            Me.m_lblPredator = New System.Windows.Forms.Label
            Me.m_cmbPredator = New System.Windows.Forms.ComboBox
            Me.m_lblPrey = New System.Windows.Forms.Label
            Me.m_lbPrey = New System.Windows.Forms.ListBox
            Me.m_graph = New ZedGraphControl
            Me.SuspendLayout()
            '
            'm_lblPredator
            '
            Me.m_lblPredator.AutoSize = True
            Me.m_lblPredator.Location = New System.Drawing.Point(12, 15)
            Me.m_lblPredator.Name = "m_lblPredator"
            Me.m_lblPredator.Size = New System.Drawing.Size(50, 13)
            Me.m_lblPredator.TabIndex = 0
            Me.m_lblPredator.Text = "&Predator:"
            '
            'm_cmbPredator
            '
            Me.m_cmbPredator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbPredator.FormattingEnabled = True
            Me.m_cmbPredator.Location = New System.Drawing.Point(68, 12)
            Me.m_cmbPredator.Name = "m_cmbPredator"
            Me.m_cmbPredator.Size = New System.Drawing.Size(121, 21)
            Me.m_cmbPredator.TabIndex = 1
            '
            'm_lblPrey
            '
            Me.m_lblPrey.AutoSize = True
            Me.m_lblPrey.Location = New System.Drawing.Point(12, 39)
            Me.m_lblPrey.Name = "m_lblPrey"
            Me.m_lblPrey.Size = New System.Drawing.Size(31, 13)
            Me.m_lblPrey.TabIndex = 2
            Me.m_lblPrey.Text = "Prey:"
            '
            'm_lbPrey
            '
            Me.m_lbPrey.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lbPrey.FormattingEnabled = True
            Me.m_lbPrey.IntegralHeight = False
            Me.m_lbPrey.Location = New System.Drawing.Point(68, 39)
            Me.m_lbPrey.Name = "m_lbPrey"
            Me.m_lbPrey.Size = New System.Drawing.Size(121, 322)
            Me.m_lbPrey.TabIndex = 3
            '
            'm_graph
            '
            Me.m_graph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_graph.Location = New System.Drawing.Point(195, 12)
            Me.m_graph.Name = "m_graph"
            Me.m_graph.Size = New System.Drawing.Size(485, 349)
            Me.m_graph.TabIndex = 4
            '
            'frmFunctionalResponse
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(692, 373)
            Me.Controls.Add(Me.m_graph)
            Me.Controls.Add(Me.m_lbPrey)
            Me.Controls.Add(Me.m_lblPrey)
            Me.Controls.Add(Me.m_cmbPredator)
            Me.Controls.Add(Me.m_lblPredator)
            Me.Name = "frmFunctionalResponse"
            Me.Text = "frmFunctionalResponse"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblPredator As System.Windows.Forms.Label
        Private WithEvents m_cmbPredator As System.Windows.Forms.ComboBox
        Private WithEvents m_lblPrey As System.Windows.Forms.Label
        Private WithEvents m_lbPrey As System.Windows.Forms.ListBox
        Private WithEvents m_graph As ZedGraphControl
    End Class

End Namespace
