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
            Me.m_cbRightClickAutoScale = New System.Windows.Forms.CheckBox
            Me.m_nudMax = New System.Windows.Forms.NumericUpDown
            Me.m_cbAutoScale = New System.Windows.Forms.CheckBox
            Me.m_lblYMax = New System.Windows.Forms.Label
            Me.m_cbShowScaleAndTitle = New System.Windows.Forms.CheckBox
            Me.m_hdrShow = New cEwEHeaderLabel
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_hdrDrawAs = New cEwEHeaderLabel
            Me.m_hdrScaling = New cEwEHeaderLabel
            Me.m_rbDots = New System.Windows.Forms.RadioButton
            CType(Me.m_nudMax, System.ComponentModel.ISupportInitialize).BeginInit()
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
            'm_cbRightClickAutoScale
            '
            resources.ApplyResources(Me.m_cbRightClickAutoScale, "m_cbRightClickAutoScale")
            Me.m_cbRightClickAutoScale.Name = "m_cbRightClickAutoScale"
            Me.m_cbRightClickAutoScale.UseVisualStyleBackColor = True
            '
            'm_nudMax
            '
            resources.ApplyResources(Me.m_nudMax, "m_nudMax")
            Me.m_nudMax.Name = "m_nudMax"
            '
            'm_cbAutoScale
            '
            resources.ApplyResources(Me.m_cbAutoScale, "m_cbAutoScale")
            Me.m_cbAutoScale.Name = "m_cbAutoScale"
            Me.m_cbAutoScale.UseVisualStyleBackColor = True
            '
            'm_lblYMax
            '
            resources.ApplyResources(Me.m_lblYMax, "m_lblYMax")
            Me.m_lblYMax.Name = "m_lblYMax"
            '
            'm_cbShowScaleAndTitle
            '
            resources.ApplyResources(Me.m_cbShowScaleAndTitle, "m_cbShowScaleAndTitle")
            Me.m_cbShowScaleAndTitle.Name = "m_cbShowScaleAndTitle"
            Me.m_cbShowScaleAndTitle.UseVisualStyleBackColor = True
            '
            'm_hdrShow
            '
            resources.ApplyResources(Me.m_hdrShow, "m_hdrShow")
            Me.m_hdrShow.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_hdrShow.ForeColor = System.Drawing.SystemColors.Window
            Me.m_hdrShow.Name = "m_hdrShow"
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
            'm_hdrDrawAs
            '
            resources.ApplyResources(Me.m_hdrDrawAs, "m_hdrDrawAs")
            Me.m_hdrDrawAs.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_hdrDrawAs.ForeColor = System.Drawing.SystemColors.Window
            Me.m_hdrDrawAs.Name = "m_hdrDrawAs"
            '
            'm_hdrScaling
            '
            resources.ApplyResources(Me.m_hdrScaling, "m_hdrScaling")
            Me.m_hdrScaling.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_hdrScaling.ForeColor = System.Drawing.SystemColors.Window
            Me.m_hdrScaling.Name = "m_hdrScaling"
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
            Me.Controls.Add(Me.m_hdrScaling)
            Me.Controls.Add(Me.m_hdrDrawAs)
            Me.Controls.Add(Me.m_hdrShow)
            Me.Controls.Add(Me.m_nudMax)
            Me.Controls.Add(Me.m_lblYMax)
            Me.Controls.Add(Me.m_cbRightClickAutoScale)
            Me.Controls.Add(Me.m_cbAutoScale)
            Me.Controls.Add(Me.m_rbDots)
            Me.Controls.Add(Me.m_rbLine)
            Me.Controls.Add(Me.m_rbFill)
            Me.Controls.Add(Me.m_cbShowScaleAndTitle)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgGraphDisplayOptions"
            CType(Me.m_nudMax, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_rbLine As System.Windows.Forms.RadioButton
        Private WithEvents m_rbFill As System.Windows.Forms.RadioButton
        Private WithEvents m_cbShowScaleAndTitle As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrShow As cEwEHeaderLabel
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_rbDots As System.Windows.Forms.RadioButton
        Private WithEvents m_hdrDrawAs As cEwEHeaderLabel
        Private WithEvents m_hdrScaling As cEwEHeaderLabel
        Private WithEvents m_cbAutoScale As System.Windows.Forms.CheckBox
        Private WithEvents m_cbRightClickAutoScale As System.Windows.Forms.CheckBox
        Private WithEvents m_lblYMax As System.Windows.Forms.Label
        Private WithEvents m_nudMax As System.Windows.Forms.NumericUpDown

    End Class

End Namespace

