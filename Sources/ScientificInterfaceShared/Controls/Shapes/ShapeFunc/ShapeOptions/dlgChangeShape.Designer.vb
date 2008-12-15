Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgChangeShape
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgChangeShape))
            Me.m_txbYBase = New System.Windows.Forms.TextBox
            Me.lbYBase = New System.Windows.Forms.Label
            Me.m_txbYZero = New System.Windows.Forms.TextBox
            Me.lbYZero = New System.Windows.Forms.Label
            Me.m_txbSteep = New System.Windows.Forms.TextBox
            Me.m_txbYEnd = New System.Windows.Forms.TextBox
            Me.lbSteep = New System.Windows.Forms.Label
            Me.lbYEnd = New System.Windows.Forms.Label
            Me.m_lbShape = New System.Windows.Forms.ListBox
            Me.m_lbShapeTypes = New System.Windows.Forms.Label
            Me.m_gbParameters = New System.Windows.Forms.GroupBox
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_plPreview = New System.Windows.Forms.Panel
            Me.m_gbParameters.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_txbYBase
            '
            resources.ApplyResources(Me.m_txbYBase, "m_txbYBase")
            Me.m_txbYBase.Name = "m_txbYBase"
            '
            'lbYBase
            '
            resources.ApplyResources(Me.lbYBase, "lbYBase")
            Me.lbYBase.Name = "lbYBase"
            '
            'm_txbYZero
            '
            resources.ApplyResources(Me.m_txbYZero, "m_txbYZero")
            Me.m_txbYZero.Name = "m_txbYZero"
            '
            'lbYZero
            '
            resources.ApplyResources(Me.lbYZero, "lbYZero")
            Me.lbYZero.Name = "lbYZero"
            '
            'm_txbSteep
            '
            resources.ApplyResources(Me.m_txbSteep, "m_txbSteep")
            Me.m_txbSteep.Name = "m_txbSteep"
            '
            'm_txbYEnd
            '
            resources.ApplyResources(Me.m_txbYEnd, "m_txbYEnd")
            Me.m_txbYEnd.Name = "m_txbYEnd"
            '
            'lbSteep
            '
            resources.ApplyResources(Me.lbSteep, "lbSteep")
            Me.lbSteep.Name = "lbSteep"
            '
            'lbYEnd
            '
            resources.ApplyResources(Me.lbYEnd, "lbYEnd")
            Me.lbYEnd.Name = "lbYEnd"
            '
            'm_lbShape
            '
            Me.m_lbShape.FormattingEnabled = True
            Me.m_lbShape.Items.AddRange(New Object() {resources.GetString("m_lbShape.Items"), resources.GetString("m_lbShape.Items1"), resources.GetString("m_lbShape.Items2"), resources.GetString("m_lbShape.Items3"), resources.GetString("m_lbShape.Items4")})
            resources.ApplyResources(Me.m_lbShape, "m_lbShape")
            Me.m_lbShape.Name = "m_lbShape"
            '
            'm_lbShapeTypes
            '
            resources.ApplyResources(Me.m_lbShapeTypes, "m_lbShapeTypes")
            Me.m_lbShapeTypes.Name = "m_lbShapeTypes"
            '
            'm_gbParameters
            '
            Me.m_gbParameters.Controls.Add(Me.lbYZero)
            Me.m_gbParameters.Controls.Add(Me.lbSteep)
            Me.m_gbParameters.Controls.Add(Me.m_txbYEnd)
            Me.m_gbParameters.Controls.Add(Me.m_txbYBase)
            Me.m_gbParameters.Controls.Add(Me.lbYEnd)
            Me.m_gbParameters.Controls.Add(Me.lbYBase)
            Me.m_gbParameters.Controls.Add(Me.m_txbSteep)
            Me.m_gbParameters.Controls.Add(Me.m_txbYZero)
            resources.ApplyResources(Me.m_gbParameters, "m_gbParameters")
            Me.m_gbParameters.Name = "m_gbParameters"
            Me.m_gbParameters.TabStop = False
            '
            'm_btnOk
            '
            resources.ApplyResources(Me.m_btnOk, "m_btnOk")
            Me.m_btnOk.Name = "m_btnOk"
            Me.m_btnOk.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_plPreview
            '
            resources.ApplyResources(Me.m_plPreview, "m_plPreview")
            Me.m_plPreview.BackColor = System.Drawing.SystemColors.Window
            Me.m_plPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plPreview.Name = "m_plPreview"
            '
            'dlgChangeShape
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_lbShape)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_gbParameters)
            Me.Controls.Add(Me.m_lbShapeTypes)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgChangeShape"
            Me.ShowInTaskbar = False
            Me.m_gbParameters.ResumeLayout(False)
            Me.m_gbParameters.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_txbYBase As System.Windows.Forms.TextBox
        Friend WithEvents lbYBase As System.Windows.Forms.Label
        Friend WithEvents m_txbYZero As System.Windows.Forms.TextBox
        Friend WithEvents lbYZero As System.Windows.Forms.Label
        Friend WithEvents m_txbSteep As System.Windows.Forms.TextBox
        Friend WithEvents m_txbYEnd As System.Windows.Forms.TextBox
        Friend WithEvents lbSteep As System.Windows.Forms.Label
        Friend WithEvents lbYEnd As System.Windows.Forms.Label
        Friend WithEvents m_lbShape As System.Windows.Forms.ListBox
        Friend WithEvents m_lbShapeTypes As System.Windows.Forms.Label
        Friend WithEvents m_gbParameters As System.Windows.Forms.GroupBox
        Friend WithEvents m_btnOk As System.Windows.Forms.Button
        Friend WithEvents m_btnCancel As System.Windows.Forms.Button
        Friend WithEvents m_plPreview As System.Windows.Forms.Panel

    End Class

End Namespace

