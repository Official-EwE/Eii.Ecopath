Namespace Import

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucImportPageWelcome
        Inherits System.Windows.Forms.UserControl

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
            Me.m_tlpLogo = New System.Windows.Forms.TableLayoutPanel
            Me.m_pbWelcome = New System.Windows.Forms.PictureBox
            Me.m_lblWelcomeTitle = New System.Windows.Forms.Label
            Me.m_lblWelcomeInstructions = New System.Windows.Forms.Label
            Me.m_tlpLogo.SuspendLayout()
            CType(Me.m_pbWelcome, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_tlpLogo
            '
            Me.m_tlpLogo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_tlpLogo.BackColor = System.Drawing.Color.White
            Me.m_tlpLogo.ColumnCount = 1
            Me.m_tlpLogo.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpLogo.Controls.Add(Me.m_pbWelcome, 0, 0)
            Me.m_tlpLogo.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpLogo.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tlpLogo.Name = "m_tlpLogo"
            Me.m_tlpLogo.RowCount = 2
            Me.m_tlpLogo.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlpLogo.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpLogo.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlpLogo.Size = New System.Drawing.Size(133, 276)
            Me.m_tlpLogo.TabIndex = 9
            '
            'm_pbWelcome
            '
            Me.m_pbWelcome.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_pbWelcome.BackColor = System.Drawing.Color.White
            Me.m_pbWelcome.Image = Global.ScientificInterface.My.Resources.Resources.EwELogo_caption
            Me.m_pbWelcome.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_pbWelcome.Location = New System.Drawing.Point(0, 0)
            Me.m_pbWelcome.Margin = New System.Windows.Forms.Padding(0)
            Me.m_pbWelcome.Name = "m_pbWelcome"
            Me.m_pbWelcome.Size = New System.Drawing.Size(132, 273)
            Me.m_pbWelcome.TabIndex = 7
            Me.m_pbWelcome.TabStop = False
            '
            'm_lblWelcomeTitle
            '
            Me.m_lblWelcomeTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblWelcomeTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!, System.Drawing.FontStyle.Bold)
            Me.m_lblWelcomeTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblWelcomeTitle.Location = New System.Drawing.Point(154, 55)
            Me.m_lblWelcomeTitle.Name = "m_lblWelcomeTitle"
            Me.m_lblWelcomeTitle.Size = New System.Drawing.Size(286, 46)
            Me.m_lblWelcomeTitle.TabIndex = 7
            Me.m_lblWelcomeTitle.Text = "Welcome to Ecopath Database Conversion Wizard"
            '
            'm_lblWelcomeInstructions
            '
            Me.m_lblWelcomeInstructions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblWelcomeInstructions.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblWelcomeInstructions.Location = New System.Drawing.Point(155, 115)
            Me.m_lblWelcomeInstructions.Name = "m_lblWelcomeInstructions"
            Me.m_lblWelcomeInstructions.Size = New System.Drawing.Size(285, 105)
            Me.m_lblWelcomeInstructions.TabIndex = 8
            Me.m_lblWelcomeInstructions.Text = "The database you are opening was created in a previous version of Ecopath with Ec" & _
                "osim.  It must be converted to the format used by this version." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Click Next to" & _
                " proceed."
            '
            'ucImportPage1Welcome
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpLogo)
            Me.Controls.Add(Me.m_lblWelcomeInstructions)
            Me.Controls.Add(Me.m_lblWelcomeTitle)
            Me.Name = "ucImportPage1Welcome"
            Me.Size = New System.Drawing.Size(465, 276)
            Me.m_tlpLogo.ResumeLayout(False)
            CType(Me.m_pbWelcome, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpLogo As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_pbWelcome As System.Windows.Forms.PictureBox
        Private WithEvents m_lblWelcomeTitle As System.Windows.Forms.Label
        Private WithEvents m_lblWelcomeInstructions As System.Windows.Forms.Label

    End Class

End Namespace