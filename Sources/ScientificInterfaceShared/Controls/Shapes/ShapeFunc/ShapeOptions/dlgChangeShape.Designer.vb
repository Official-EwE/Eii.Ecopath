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
            Me.m_lbShapeTypes = New System.Windows.Forms.Label
            Me.m_btnOk = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_plPreview = New System.Windows.Forms.Panel
            Me.m_hdrParams = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrShape = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tlpParameters = New System.Windows.Forms.TableLayoutPanel
            Me.m_rbBeta = New System.Windows.Forms.RadioButton
            Me.m_rbExponential = New System.Windows.Forms.RadioButton
            Me.m_rbHyperbolic = New System.Windows.Forms.RadioButton
            Me.m_rbSigmoid = New System.Windows.Forms.RadioButton
            Me.m_rbLinear = New System.Windows.Forms.RadioButton
            Me.m_rbOriginal = New System.Windows.Forms.RadioButton
            Me.m_tlpParameters.SuspendLayout()
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
            'm_lbShapeTypes
            '
            resources.ApplyResources(Me.m_lbShapeTypes, "m_lbShapeTypes")
            Me.m_lbShapeTypes.Name = "m_lbShapeTypes"
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
            'm_hdrParams
            '
            resources.ApplyResources(Me.m_hdrParams, "m_hdrParams")
            Me.m_hdrParams.CanCollapseParent = False
            Me.m_hdrParams.CollapsedParentHeight = 0
            Me.m_hdrParams.IsCollapsed = False
            Me.m_hdrParams.Name = "m_hdrParams"
            '
            'm_hdrShape
            '
            Me.m_hdrShape.CanCollapseParent = False
            Me.m_hdrShape.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrShape, "m_hdrShape")
            Me.m_hdrShape.IsCollapsed = False
            Me.m_hdrShape.Name = "m_hdrShape"
            '
            'm_tlpParameters
            '
            resources.ApplyResources(Me.m_tlpParameters, "m_tlpParameters")
            Me.m_tlpParameters.Controls.Add(Me.m_rbBeta, 0, 5)
            Me.m_tlpParameters.Controls.Add(Me.m_rbExponential, 0, 4)
            Me.m_tlpParameters.Controls.Add(Me.m_rbHyperbolic, 0, 3)
            Me.m_tlpParameters.Controls.Add(Me.m_rbSigmoid, 0, 2)
            Me.m_tlpParameters.Controls.Add(Me.m_rbLinear, 0, 1)
            Me.m_tlpParameters.Controls.Add(Me.m_rbOriginal, 0, 0)
            Me.m_tlpParameters.Name = "m_tlpParameters"
            '
            'm_rbBeta
            '
            resources.ApplyResources(Me.m_rbBeta, "m_rbBeta")
            Me.m_rbBeta.Name = "m_rbBeta"
            Me.m_rbBeta.TabStop = True
            Me.m_rbBeta.UseVisualStyleBackColor = True
            '
            'm_rbExponential
            '
            resources.ApplyResources(Me.m_rbExponential, "m_rbExponential")
            Me.m_rbExponential.Name = "m_rbExponential"
            Me.m_rbExponential.TabStop = True
            Me.m_rbExponential.UseVisualStyleBackColor = True
            '
            'm_rbHyperbolic
            '
            resources.ApplyResources(Me.m_rbHyperbolic, "m_rbHyperbolic")
            Me.m_rbHyperbolic.Name = "m_rbHyperbolic"
            Me.m_rbHyperbolic.TabStop = True
            Me.m_rbHyperbolic.UseVisualStyleBackColor = True
            '
            'm_rbSigmoid
            '
            resources.ApplyResources(Me.m_rbSigmoid, "m_rbSigmoid")
            Me.m_rbSigmoid.Name = "m_rbSigmoid"
            Me.m_rbSigmoid.TabStop = True
            Me.m_rbSigmoid.UseVisualStyleBackColor = True
            '
            'm_rbLinear
            '
            resources.ApplyResources(Me.m_rbLinear, "m_rbLinear")
            Me.m_rbLinear.Name = "m_rbLinear"
            Me.m_rbLinear.TabStop = True
            Me.m_rbLinear.UseVisualStyleBackColor = True
            '
            'm_rbOriginal
            '
            resources.ApplyResources(Me.m_rbOriginal, "m_rbOriginal")
            Me.m_rbOriginal.Name = "m_rbOriginal"
            Me.m_rbOriginal.TabStop = True
            Me.m_rbOriginal.UseVisualStyleBackColor = True
            '
            'dlgChangeShape
            '
            Me.AcceptButton = Me.m_btnOk
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tlpParameters)
            Me.Controls.Add(Me.lbYZero)
            Me.Controls.Add(Me.lbSteep)
            Me.Controls.Add(Me.m_hdrShape)
            Me.Controls.Add(Me.m_hdrParams)
            Me.Controls.Add(Me.m_txbYEnd)
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_txbYBase)
            Me.Controls.Add(Me.lbYEnd)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.lbYBase)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_txbSteep)
            Me.Controls.Add(Me.m_txbYZero)
            Me.Controls.Add(Me.m_lbShapeTypes)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgChangeShape"
            Me.ShowInTaskbar = False
            Me.m_tlpParameters.ResumeLayout(False)
            Me.m_tlpParameters.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_txbYBase As System.Windows.Forms.TextBox
        Friend WithEvents lbYBase As System.Windows.Forms.Label
        Friend WithEvents m_txbYZero As System.Windows.Forms.TextBox
        Friend WithEvents lbYZero As System.Windows.Forms.Label
        Friend WithEvents m_txbSteep As System.Windows.Forms.TextBox
        Friend WithEvents m_txbYEnd As System.Windows.Forms.TextBox
        Friend WithEvents lbSteep As System.Windows.Forms.Label
        Friend WithEvents lbYEnd As System.Windows.Forms.Label
        Friend WithEvents m_lbShapeTypes As System.Windows.Forms.Label
        Friend WithEvents m_btnOk As System.Windows.Forms.Button
        Friend WithEvents m_btnCancel As System.Windows.Forms.Button
        Friend WithEvents m_plPreview As System.Windows.Forms.Panel
        Private WithEvents m_hdrParams As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrShape As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Friend WithEvents m_tlpParameters As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_rbHyperbolic As System.Windows.Forms.RadioButton
        Private WithEvents m_rbSigmoid As System.Windows.Forms.RadioButton
        Private WithEvents m_rbLinear As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbOriginal As System.Windows.Forms.RadioButton
        Private WithEvents m_rbBeta As System.Windows.Forms.RadioButton
        Private WithEvents m_rbExponential As System.Windows.Forms.RadioButton

    End Class

End Namespace

