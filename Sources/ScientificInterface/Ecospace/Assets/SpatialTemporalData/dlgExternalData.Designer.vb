Imports ScientificInterfaceShared.Forms

Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgExternalData
        Inherits Form
        Implements IUIElement

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Me.UIContext = Nothing
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_btnOK = New System.Windows.Forms.Button()
            Me.m_sc = New System.Windows.Forms.SplitContainer()
            Me.m_lbxAdapters = New System.Windows.Forms.ListBox()
            Me.m_config = New ScientificInterface.Ecospace.Controls.ucConfigAdapter()
            CType(Me.m_sc, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_sc.Panel1.SuspendLayout()
            Me.m_sc.Panel2.SuspendLayout()
            Me.m_sc.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnOK
            '
            Me.m_btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnOK.Location = New System.Drawing.Point(457, 296)
            Me.m_btnOK.Name = "m_btnOK"
            Me.m_btnOK.Size = New System.Drawing.Size(75, 23)
            Me.m_btnOK.TabIndex = 2
            Me.m_btnOK.Text = "OK"
            Me.m_btnOK.UseVisualStyleBackColor = True
            '
            'm_sc
            '
            Me.m_sc.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sc.Location = New System.Drawing.Point(12, 12)
            Me.m_sc.Name = "m_sc"
            '
            'm_sc.Panel1
            '
            Me.m_sc.Panel1.Controls.Add(Me.m_lbxAdapters)
            '
            'm_sc.Panel2
            '
            Me.m_sc.Panel2.Controls.Add(Me.m_config)
            Me.m_sc.Size = New System.Drawing.Size(520, 278)
            Me.m_sc.SplitterDistance = 139
            Me.m_sc.TabIndex = 0
            '
            'm_lbxAdapters
            '
            Me.m_lbxAdapters.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lbxAdapters.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbxAdapters.FormattingEnabled = True
            Me.m_lbxAdapters.IntegralHeight = False
            Me.m_lbxAdapters.Location = New System.Drawing.Point(0, 0)
            Me.m_lbxAdapters.Name = "m_lbxAdapters"
            Me.m_lbxAdapters.Size = New System.Drawing.Size(139, 278)
            Me.m_lbxAdapters.TabIndex = 0
            '
            'm_config
            '
            Me.m_config.Adapter = Nothing
            Me.m_config.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_config.Location = New System.Drawing.Point(0, 0)
            Me.m_config.Name = "m_config"
            Me.m_config.Size = New System.Drawing.Size(377, 278)
            Me.m_config.TabIndex = 0
            Me.m_config.UIContext = Nothing
            '
            'dlgExternalData
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(544, 331)
            Me.ControlBox = False
            Me.Controls.Add(Me.m_sc)
            Me.Controls.Add(Me.m_btnOK)
            Me.MinimumSize = New System.Drawing.Size(560, 350)
            Me.Name = "dlgExternalData"
            Me.ShowInTaskbar = False
            Me.Text = "Ecospace external data connections"
            Me.m_sc.Panel1.ResumeLayout(False)
            Me.m_sc.Panel2.ResumeLayout(False)
            CType(Me.m_sc, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_sc.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_sc As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbxAdapters As System.Windows.Forms.ListBox
        Private WithEvents m_config As ScientificInterface.Ecospace.Controls.ucConfigAdapter

    End Class

End Namespace