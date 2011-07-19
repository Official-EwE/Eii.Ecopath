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
            Me.m_lbYBase = New System.Windows.Forms.Label
            Me.m_txbYZero = New System.Windows.Forms.TextBox
            Me.m_lbYZero = New System.Windows.Forms.Label
            Me.m_txbSteep = New System.Windows.Forms.TextBox
            Me.m_txbYEnd = New System.Windows.Forms.TextBox
            Me.m_lbSteep = New System.Windows.Forms.Label
            Me.m_lbYEnd = New System.Windows.Forms.Label
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_plPreview = New System.Windows.Forms.Panel
            Me.m_lbShapeFunctionTypes = New System.Windows.Forms.ListBox
            Me.m_hdrShape = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrParams = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.SuspendLayout()
            '
            'm_txbYBase
            '
            resources.ApplyResources(Me.m_txbYBase, "m_txbYBase")
            Me.m_txbYBase.Name = "m_txbYBase"
            '
            'm_lbYBase
            '
            resources.ApplyResources(Me.m_lbYBase, "m_lbYBase")
            Me.m_lbYBase.Name = "m_lbYBase"
            '
            'm_txbYZero
            '
            resources.ApplyResources(Me.m_txbYZero, "m_txbYZero")
            Me.m_txbYZero.Name = "m_txbYZero"
            '
            'm_lbYZero
            '
            resources.ApplyResources(Me.m_lbYZero, "m_lbYZero")
            Me.m_lbYZero.Name = "m_lbYZero"
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
            'm_lbSteep
            '
            resources.ApplyResources(Me.m_lbSteep, "m_lbSteep")
            Me.m_lbSteep.Name = "m_lbSteep"
            '
            'm_lbYEnd
            '
            resources.ApplyResources(Me.m_lbYEnd, "m_lbYEnd")
            Me.m_lbYEnd.Name = "m_lbYEnd"
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
            'dlgChangeShape
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_lbShapeFunctionTypes)
            Me.Controls.Add(Me.m_lbYZero)
            Me.Controls.Add(Me.m_lbSteep)
            Me.Controls.Add(Me.m_hdrShape)
            Me.Controls.Add(Me.m_hdrParams)
            Me.Controls.Add(Me.m_txbYEnd)
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_txbYBase)
            Me.Controls.Add(Me.m_lbYEnd)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.m_lbYBase)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_txbSteep)
            Me.Controls.Add(Me.m_txbYZero)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgChangeShape"
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_txbYBase As System.Windows.Forms.TextBox
        Private WithEvents m_lbYBase As System.Windows.Forms.Label
        Private WithEvents m_txbYZero As System.Windows.Forms.TextBox
        Private WithEvents m_lbYZero As System.Windows.Forms.Label
        Private WithEvents m_txbSteep As System.Windows.Forms.TextBox
        Private WithEvents m_txbYEnd As System.Windows.Forms.TextBox
        Private WithEvents m_lbSteep As System.Windows.Forms.Label
        Private WithEvents m_lbYEnd As System.Windows.Forms.Label
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_plPreview As System.Windows.Forms.Panel
        Private WithEvents m_hdrParams As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrShape As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_lbShapeFunctionTypes As System.Windows.Forms.ListBox

    End Class

End Namespace

