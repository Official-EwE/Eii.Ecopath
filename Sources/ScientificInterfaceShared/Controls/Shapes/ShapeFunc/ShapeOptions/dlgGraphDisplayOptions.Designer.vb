Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgGraphDisplayOptions
        Inherits System.Windows.Forms.Form

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgGraphDisplayOptions))
            Me.m_rbLine = New System.Windows.Forms.RadioButton
            Me.m_rbFill = New System.Windows.Forms.RadioButton
            Me.cbRightClickAutoScale = New System.Windows.Forms.CheckBox
            Me.nupYMax = New System.Windows.Forms.NumericUpDown
            Me.cbAutoScale = New System.Windows.Forms.CheckBox
            Me.lbYMax = New System.Windows.Forms.Label
            Me.m_cbShowScaleAndTitle = New System.Windows.Forms.CheckBox
            Me.m_lblCapInformation = New System.Windows.Forms.Label
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.m_rbDots = New System.Windows.Forms.RadioButton
            CType(Me.nupYMax, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_rbLine
            '
            resources.ApplyResources(Me.m_rbLine, "m_rbLine")
            Me.m_rbLine.Name = "m_rbLine"
            Me.m_rbLine.TabStop = True
            Me.m_rbLine.UseVisualStyleBackColor = True
            '
            'm_rbFill
            '
            resources.ApplyResources(Me.m_rbFill, "m_rbFill")
            Me.m_rbFill.Name = "m_rbFill"
            Me.m_rbFill.TabStop = True
            Me.m_rbFill.UseVisualStyleBackColor = True
            '
            'cbRightClickAutoScale
            '
            resources.ApplyResources(Me.cbRightClickAutoScale, "cbRightClickAutoScale")
            Me.cbRightClickAutoScale.Name = "cbRightClickAutoScale"
            Me.cbRightClickAutoScale.UseVisualStyleBackColor = True
            '
            'nupYMax
            '
            resources.ApplyResources(Me.nupYMax, "nupYMax")
            Me.nupYMax.Name = "nupYMax"
            '
            'cbAutoScale
            '
            resources.ApplyResources(Me.cbAutoScale, "cbAutoScale")
            Me.cbAutoScale.Name = "cbAutoScale"
            Me.cbAutoScale.UseVisualStyleBackColor = True
            '
            'lbYMax
            '
            resources.ApplyResources(Me.lbYMax, "lbYMax")
            Me.lbYMax.Name = "lbYMax"
            '
            'm_cbShowScaleAndTitle
            '
            resources.ApplyResources(Me.m_cbShowScaleAndTitle, "m_cbShowScaleAndTitle")
            Me.m_cbShowScaleAndTitle.Name = "m_cbShowScaleAndTitle"
            Me.m_cbShowScaleAndTitle.UseVisualStyleBackColor = True
            '
            'm_lblCapInformation
            '
            resources.ApplyResources(Me.m_lblCapInformation, "m_lblCapInformation")
            Me.m_lblCapInformation.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblCapInformation.ForeColor = System.Drawing.SystemColors.Window
            Me.m_lblCapInformation.Name = "m_lblCapInformation"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.BackColor = System.Drawing.SystemColors.ControlDark
            Me.Label1.ForeColor = System.Drawing.SystemColors.Window
            Me.Label1.Name = "Label1"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.BackColor = System.Drawing.SystemColors.ControlDark
            Me.Label2.ForeColor = System.Drawing.SystemColors.Window
            Me.Label2.Name = "Label2"
            '
            'm_rbDots
            '
            resources.ApplyResources(Me.m_rbDots, "m_rbDots")
            Me.m_rbDots.Name = "m_rbDots"
            Me.m_rbDots.TabStop = True
            Me.m_rbDots.UseVisualStyleBackColor = True
            '
            'dlgGraphDisplayOptions
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_lblCapInformation)
            Me.Controls.Add(Me.nupYMax)
            Me.Controls.Add(Me.lbYMax)
            Me.Controls.Add(Me.cbRightClickAutoScale)
            Me.Controls.Add(Me.cbAutoScale)
            Me.Controls.Add(Me.m_rbDots)
            Me.Controls.Add(Me.m_rbLine)
            Me.Controls.Add(Me.m_rbFill)
            Me.Controls.Add(Me.m_cbShowScaleAndTitle)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgGraphDisplayOptions"
            CType(Me.nupYMax, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_rbLine As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbFill As System.Windows.Forms.RadioButton
        Friend WithEvents cbRightClickAutoScale As System.Windows.Forms.CheckBox
        Friend WithEvents nupYMax As System.Windows.Forms.NumericUpDown
        Friend WithEvents cbAutoScale As System.Windows.Forms.CheckBox
        Friend WithEvents lbYMax As System.Windows.Forms.Label
        Friend WithEvents m_cbShowScaleAndTitle As System.Windows.Forms.CheckBox
        Friend WithEvents m_lblCapInformation As System.Windows.Forms.Label
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents OK_Button As System.Windows.Forms.Button
        Friend WithEvents Cancel_Button As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents m_rbDots As System.Windows.Forms.RadioButton

    End Class

End Namespace

