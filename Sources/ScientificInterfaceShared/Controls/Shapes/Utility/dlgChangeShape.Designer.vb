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
            Me.m_tbxC = New System.Windows.Forms.TextBox
            Me.m_lblC = New System.Windows.Forms.Label
            Me.m_tbxA = New System.Windows.Forms.TextBox
            Me.m_lblA = New System.Windows.Forms.Label
            Me.m_tbxD = New System.Windows.Forms.TextBox
            Me.m_tbxB = New System.Windows.Forms.TextBox
            Me.m_lblD = New System.Windows.Forms.Label
            Me.m_lblB = New System.Windows.Forms.Label
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_plPreview = New System.Windows.Forms.Panel
            Me.m_lbShapeFunctionTypes = New System.Windows.Forms.ListBox
            Me.m_hdrShape = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrParams = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_btnDefaults = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'm_tbxC
            '
            resources.ApplyResources(Me.m_tbxC, "m_tbxC")
            Me.m_tbxC.Name = "m_tbxC"
            '
            'm_lblC
            '
            resources.ApplyResources(Me.m_lblC, "m_lblC")
            Me.m_lblC.Name = "m_lblC"
            '
            'm_tbxA
            '
            resources.ApplyResources(Me.m_tbxA, "m_tbxA")
            Me.m_tbxA.Name = "m_tbxA"
            '
            'm_lblA
            '
            resources.ApplyResources(Me.m_lblA, "m_lblA")
            Me.m_lblA.Name = "m_lblA"
            '
            'm_tbxD
            '
            resources.ApplyResources(Me.m_tbxD, "m_tbxD")
            Me.m_tbxD.Name = "m_tbxD"
            '
            'm_tbxB
            '
            resources.ApplyResources(Me.m_tbxB, "m_tbxB")
            Me.m_tbxB.Name = "m_tbxB"
            '
            'm_lblD
            '
            resources.ApplyResources(Me.m_lblD, "m_lblD")
            Me.m_lblD.Name = "m_lblD"
            '
            'm_lblB
            '
            resources.ApplyResources(Me.m_lblB, "m_lblB")
            Me.m_lblB.Name = "m_lblB"
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
            'm_lbShapeFunctionTypes
            '
            resources.ApplyResources(Me.m_lbShapeFunctionTypes, "m_lbShapeFunctionTypes")
            Me.m_lbShapeFunctionTypes.FormattingEnabled = True
            Me.m_lbShapeFunctionTypes.Name = "m_lbShapeFunctionTypes"
            Me.m_lbShapeFunctionTypes.Sorted = True
            '
            'm_hdrShape
            '
            Me.m_hdrShape.CanCollapseParent = False
            Me.m_hdrShape.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrShape, "m_hdrShape")
            Me.m_hdrShape.IsCollapsed = False
            Me.m_hdrShape.Name = "m_hdrShape"
            '
            'm_hdrParams
            '
            resources.ApplyResources(Me.m_hdrParams, "m_hdrParams")
            Me.m_hdrParams.CanCollapseParent = False
            Me.m_hdrParams.CollapsedParentHeight = 0
            Me.m_hdrParams.IsCollapsed = False
            Me.m_hdrParams.Name = "m_hdrParams"
            '
            'm_btnDefaults
            '
            resources.ApplyResources(Me.m_btnDefaults, "m_btnDefaults")
            Me.m_btnDefaults.Name = "m_btnDefaults"
            Me.m_btnDefaults.UseVisualStyleBackColor = True
            '
            'dlgChangeShape
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_lbShapeFunctionTypes)
            Me.Controls.Add(Me.m_lblA)
            Me.Controls.Add(Me.m_lblD)
            Me.Controls.Add(Me.m_hdrShape)
            Me.Controls.Add(Me.m_hdrParams)
            Me.Controls.Add(Me.m_tbxB)
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_tbxC)
            Me.Controls.Add(Me.m_lblB)
            Me.Controls.Add(Me.m_btnDefaults)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.m_lblC)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_tbxD)
            Me.Controls.Add(Me.m_tbxA)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgChangeShape"
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tbxC As System.Windows.Forms.TextBox
        Private WithEvents m_lblC As System.Windows.Forms.Label
        Private WithEvents m_tbxA As System.Windows.Forms.TextBox
        Private WithEvents m_lblA As System.Windows.Forms.Label
        Private WithEvents m_tbxD As System.Windows.Forms.TextBox
        Private WithEvents m_tbxB As System.Windows.Forms.TextBox
        Private WithEvents m_lblD As System.Windows.Forms.Label
        Private WithEvents m_lblB As System.Windows.Forms.Label
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_plPreview As System.Windows.Forms.Panel
        Private WithEvents m_hdrParams As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrShape As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_lbShapeFunctionTypes As System.Windows.Forms.ListBox
        Private WithEvents m_btnDefaults As System.Windows.Forms.Button

    End Class

End Namespace

