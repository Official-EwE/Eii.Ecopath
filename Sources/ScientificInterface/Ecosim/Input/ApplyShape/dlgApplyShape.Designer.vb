Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgApplyShape
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgApplyShape))
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_lblPred = New System.Windows.Forms.Label
            Me.m_lblPrey = New System.Windows.Forms.Label
            Me.m_lblAvailableFF = New System.Windows.Forms.Label
            Me.lblSearchRate = New System.Windows.Forms.Label
            Me.m_txbPreyName = New System.Windows.Forms.TextBox
            Me.m_txbPredName = New System.Windows.Forms.TextBox
            Me.m_gbMultipliers = New System.Windows.Forms.GroupBox
            Me.m_rbSearchRate = New System.Windows.Forms.RadioButton
            Me.m_rbVulArea = New System.Windows.Forms.RadioButton
            Me.m_rbProdRate = New System.Windows.Forms.RadioButton
            Me.m_rbArea = New System.Windows.Forms.RadioButton
            Me.m_rbVul = New System.Windows.Forms.RadioButton
            Me.m_btnAdd = New System.Windows.Forms.Button
            Me.m_lvAppliedShapes = New System.Windows.Forms.ListView
            Me.m_colhdrShape = New System.Windows.Forms.ColumnHeader
            Me.m_colhdrModifier = New System.Windows.Forms.ColumnHeader
            Me.m_colhdrIndex = New System.Windows.Forms.ColumnHeader
            Me.m_lvAllShapes = New System.Windows.Forms.ListView
            Me.m_btnRemove = New System.Windows.Forms.Button
            Me.m_lblTitle = New System.Windows.Forms.Label
            Me.m_lblAppliedFF = New System.Windows.Forms.Label
            Me.m_gbMultipliers.SuspendLayout()
            Me.SuspendLayout()
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
            'm_lblPred
            '
            resources.ApplyResources(Me.m_lblPred, "m_lblPred")
            Me.m_lblPred.Name = "m_lblPred"
            '
            'm_lblPrey
            '
            resources.ApplyResources(Me.m_lblPrey, "m_lblPrey")
            Me.m_lblPrey.Name = "m_lblPrey"
            '
            'm_lblAvailableFF
            '
            resources.ApplyResources(Me.m_lblAvailableFF, "m_lblAvailableFF")
            Me.m_lblAvailableFF.Name = "m_lblAvailableFF"
            '
            'lblSearchRate
            '
            resources.ApplyResources(Me.lblSearchRate, "lblSearchRate")
            Me.lblSearchRate.Name = "lblSearchRate"
            '
            'm_txbPreyName
            '
            resources.ApplyResources(Me.m_txbPreyName, "m_txbPreyName")
            Me.m_txbPreyName.Name = "m_txbPreyName"
            Me.m_txbPreyName.ReadOnly = True
            '
            'm_txbPredName
            '
            resources.ApplyResources(Me.m_txbPredName, "m_txbPredName")
            Me.m_txbPredName.Name = "m_txbPredName"
            Me.m_txbPredName.ReadOnly = True
            '
            'm_gbMultipliers
            '
            resources.ApplyResources(Me.m_gbMultipliers, "m_gbMultipliers")
            Me.m_gbMultipliers.Controls.Add(Me.m_rbSearchRate)
            Me.m_gbMultipliers.Controls.Add(Me.m_rbVulArea)
            Me.m_gbMultipliers.Controls.Add(Me.m_rbProdRate)
            Me.m_gbMultipliers.Controls.Add(Me.m_rbArea)
            Me.m_gbMultipliers.Controls.Add(Me.m_rbVul)
            Me.m_gbMultipliers.Controls.Add(Me.lblSearchRate)
            Me.m_gbMultipliers.Name = "m_gbMultipliers"
            Me.m_gbMultipliers.TabStop = False
            '
            'm_rbSearchRate
            '
            resources.ApplyResources(Me.m_rbSearchRate, "m_rbSearchRate")
            Me.m_rbSearchRate.Name = "m_rbSearchRate"
            Me.m_rbSearchRate.UseVisualStyleBackColor = True
            '
            'm_rbVulArea
            '
            resources.ApplyResources(Me.m_rbVulArea, "m_rbVulArea")
            Me.m_rbVulArea.Name = "m_rbVulArea"
            Me.m_rbVulArea.TabStop = True
            Me.m_rbVulArea.UseVisualStyleBackColor = True
            '
            'm_rbProdRate
            '
            resources.ApplyResources(Me.m_rbProdRate, "m_rbProdRate")
            Me.m_rbProdRate.Checked = True
            Me.m_rbProdRate.Name = "m_rbProdRate"
            Me.m_rbProdRate.TabStop = True
            Me.m_rbProdRate.UseVisualStyleBackColor = True
            '
            'm_rbArea
            '
            resources.ApplyResources(Me.m_rbArea, "m_rbArea")
            Me.m_rbArea.Name = "m_rbArea"
            Me.m_rbArea.TabStop = True
            Me.m_rbArea.UseVisualStyleBackColor = True
            '
            'm_rbVul
            '
            resources.ApplyResources(Me.m_rbVul, "m_rbVul")
            Me.m_rbVul.Name = "m_rbVul"
            Me.m_rbVul.TabStop = True
            Me.m_rbVul.UseVisualStyleBackColor = True
            '
            'm_btnAdd
            '
            Me.m_btnAdd.Image = Global.ScientificInterface.My.Resources.Resources.arrow_right
            resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_lvAppliedShapes
            '
            resources.ApplyResources(Me.m_lvAppliedShapes, "m_lvAppliedShapes")
            Me.m_lvAppliedShapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.m_colhdrShape, Me.m_colhdrModifier, Me.m_colhdrIndex})
            Me.m_lvAppliedShapes.FullRowSelect = True
            Me.m_lvAppliedShapes.HideSelection = False
            Me.m_lvAppliedShapes.MultiSelect = False
            Me.m_lvAppliedShapes.Name = "m_lvAppliedShapes"
            Me.m_lvAppliedShapes.UseCompatibleStateImageBehavior = False
            '
            'm_colhdrShape
            '
            resources.ApplyResources(Me.m_colhdrShape, "m_colhdrShape")
            '
            'm_colhdrModifier
            '
            resources.ApplyResources(Me.m_colhdrModifier, "m_colhdrModifier")
            '
            'm_colhdrIndex
            '
            resources.ApplyResources(Me.m_colhdrIndex, "m_colhdrIndex")
            '
            'm_lvAllShapes
            '
            resources.ApplyResources(Me.m_lvAllShapes, "m_lvAllShapes")
            Me.m_lvAllShapes.HideSelection = False
            Me.m_lvAllShapes.MultiSelect = False
            Me.m_lvAllShapes.Name = "m_lvAllShapes"
            Me.m_lvAllShapes.UseCompatibleStateImageBehavior = False
            '
            'm_btnRemove
            '
            Me.m_btnRemove.Image = Global.ScientificInterface.My.Resources.Resources.DeleteHS
            resources.ApplyResources(Me.m_btnRemove, "m_btnRemove")
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'm_lblTitle
            '
            resources.ApplyResources(Me.m_lblTitle, "m_lblTitle")
            Me.m_lblTitle.Name = "m_lblTitle"
            '
            'm_lblAppliedFF
            '
            resources.ApplyResources(Me.m_lblAppliedFF, "m_lblAppliedFF")
            Me.m_lblAppliedFF.Name = "m_lblAppliedFF"
            '
            'dlgApplyShape
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.Cancel_Button)
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_txbPredName)
            Me.Controls.Add(Me.m_lblPred)
            Me.Controls.Add(Me.m_txbPreyName)
            Me.Controls.Add(Me.m_lvAppliedShapes)
            Me.Controls.Add(Me.m_lvAllShapes)
            Me.Controls.Add(Me.m_lblPrey)
            Me.Controls.Add(Me.m_btnRemove)
            Me.Controls.Add(Me.m_lblTitle)
            Me.Controls.Add(Me.m_btnAdd)
            Me.Controls.Add(Me.m_gbMultipliers)
            Me.Controls.Add(Me.m_lblAppliedFF)
            Me.Controls.Add(Me.m_lblAvailableFF)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgApplyShape"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.m_gbMultipliers.ResumeLayout(False)
            Me.m_gbMultipliers.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_lblPred As System.Windows.Forms.Label
        Private WithEvents m_lblPrey As System.Windows.Forms.Label
        Private WithEvents lblSearchRate As System.Windows.Forms.Label
        Private WithEvents m_txbPreyName As System.Windows.Forms.TextBox
        Private WithEvents m_txbPredName As System.Windows.Forms.TextBox
        Private WithEvents m_gbMultipliers As System.Windows.Forms.GroupBox
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents m_lvAppliedShapes As System.Windows.Forms.ListView
        Private WithEvents m_lvAllShapes As System.Windows.Forms.ListView
        Private WithEvents m_colhdrShape As System.Windows.Forms.ColumnHeader
        Private WithEvents m_colhdrModifier As System.Windows.Forms.ColumnHeader
        Private WithEvents m_btnRemove As System.Windows.Forms.Button
        Private WithEvents m_rbSearchRate As System.Windows.Forms.RadioButton
        Private WithEvents m_rbVulArea As System.Windows.Forms.RadioButton
        Private WithEvents m_rbArea As System.Windows.Forms.RadioButton
        Private WithEvents m_rbVul As System.Windows.Forms.RadioButton
        Private WithEvents m_colhdrIndex As System.Windows.Forms.ColumnHeader
        Private WithEvents m_rbProdRate As System.Windows.Forms.RadioButton
        Private WithEvents m_lblTitle As System.Windows.Forms.Label
        Private WithEvents m_lblAvailableFF As System.Windows.Forms.Label
        Private WithEvents m_lblAppliedFF As System.Windows.Forms.Label

    End Class

End Namespace

