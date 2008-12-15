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
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.lblPred = New System.Windows.Forms.Label
            Me.lblPrey = New System.Windows.Forms.Label
            Me.lblAvailableFF = New System.Windows.Forms.Label
            Me.lblSearchRate = New System.Windows.Forms.Label
            Me.txbPreyName = New System.Windows.Forms.TextBox
            Me.txbPredName = New System.Windows.Forms.TextBox
            Me.gbMultipliers = New System.Windows.Forms.GroupBox
            Me.rbSearchRate = New System.Windows.Forms.RadioButton
            Me.rbVulArea = New System.Windows.Forms.RadioButton
            Me.rbProdRate = New System.Windows.Forms.RadioButton
            Me.rbArea = New System.Windows.Forms.RadioButton
            Me.rbVul = New System.Windows.Forms.RadioButton
            Me.btnAdd = New System.Windows.Forms.Button
            Me.lvAppliedShapes = New System.Windows.Forms.ListView
            Me.chShape = New System.Windows.Forms.ColumnHeader
            Me.chModifier = New System.Windows.Forms.ColumnHeader
            Me.chIndex = New System.Windows.Forms.ColumnHeader
            Me.lvAllShapes = New System.Windows.Forms.ListView
            Me.btnRemove = New System.Windows.Forms.Button
            Me.lblTitle = New System.Windows.Forms.Label
            Me.lblAppliedFF = New System.Windows.Forms.Label
            Me.TableLayoutPanel1.SuspendLayout()
            Me.gbMultipliers.SuspendLayout()
            Me.SuspendLayout()
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
            'lblPred
            '
            resources.ApplyResources(Me.lblPred, "lblPred")
            Me.lblPred.Name = "lblPred"
            '
            'lblPrey
            '
            resources.ApplyResources(Me.lblPrey, "lblPrey")
            Me.lblPrey.Name = "lblPrey"
            '
            'lblAvailableFF
            '
            resources.ApplyResources(Me.lblAvailableFF, "lblAvailableFF")
            Me.lblAvailableFF.Name = "lblAvailableFF"
            '
            'lblSearchRate
            '
            resources.ApplyResources(Me.lblSearchRate, "lblSearchRate")
            Me.lblSearchRate.Name = "lblSearchRate"
            '
            'txbPreyName
            '
            resources.ApplyResources(Me.txbPreyName, "txbPreyName")
            Me.txbPreyName.Name = "txbPreyName"
            Me.txbPreyName.ReadOnly = True
            '
            'txbPredName
            '
            resources.ApplyResources(Me.txbPredName, "txbPredName")
            Me.txbPredName.Name = "txbPredName"
            Me.txbPredName.ReadOnly = True
            '
            'gbMultipliers
            '
            resources.ApplyResources(Me.gbMultipliers, "gbMultipliers")
            Me.gbMultipliers.Controls.Add(Me.rbSearchRate)
            Me.gbMultipliers.Controls.Add(Me.rbVulArea)
            Me.gbMultipliers.Controls.Add(Me.rbProdRate)
            Me.gbMultipliers.Controls.Add(Me.rbArea)
            Me.gbMultipliers.Controls.Add(Me.rbVul)
            Me.gbMultipliers.Controls.Add(Me.lblSearchRate)
            Me.gbMultipliers.Name = "gbMultipliers"
            Me.gbMultipliers.TabStop = False
            '
            'rbSearchRate
            '
            resources.ApplyResources(Me.rbSearchRate, "rbSearchRate")
            Me.rbSearchRate.Name = "rbSearchRate"
            Me.rbSearchRate.UseVisualStyleBackColor = True
            '
            'rbVulArea
            '
            resources.ApplyResources(Me.rbVulArea, "rbVulArea")
            Me.rbVulArea.Name = "rbVulArea"
            Me.rbVulArea.TabStop = True
            Me.rbVulArea.UseVisualStyleBackColor = True
            '
            'rbProdRate
            '
            resources.ApplyResources(Me.rbProdRate, "rbProdRate")
            Me.rbProdRate.Checked = True
            Me.rbProdRate.Name = "rbProdRate"
            Me.rbProdRate.TabStop = True
            Me.rbProdRate.UseVisualStyleBackColor = True
            '
            'rbArea
            '
            resources.ApplyResources(Me.rbArea, "rbArea")
            Me.rbArea.Name = "rbArea"
            Me.rbArea.TabStop = True
            Me.rbArea.UseVisualStyleBackColor = True
            '
            'rbVul
            '
            resources.ApplyResources(Me.rbVul, "rbVul")
            Me.rbVul.Name = "rbVul"
            Me.rbVul.TabStop = True
            Me.rbVul.UseVisualStyleBackColor = True
            '
            'btnAdd
            '
            Me.btnAdd.Image = Global.ScientificInterface.My.Resources.Resources.arrow_right
            resources.ApplyResources(Me.btnAdd, "btnAdd")
            Me.btnAdd.Name = "btnAdd"
            Me.btnAdd.UseVisualStyleBackColor = True
            '
            'lvAppliedShapes
            '
            resources.ApplyResources(Me.lvAppliedShapes, "lvAppliedShapes")
            Me.lvAppliedShapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chShape, Me.chModifier, Me.chIndex})
            Me.lvAppliedShapes.FullRowSelect = True
            Me.lvAppliedShapes.HideSelection = False
            Me.lvAppliedShapes.MultiSelect = False
            Me.lvAppliedShapes.Name = "lvAppliedShapes"
            Me.lvAppliedShapes.UseCompatibleStateImageBehavior = False
            '
            'chShape
            '
            resources.ApplyResources(Me.chShape, "chShape")
            '
            'chModifier
            '
            resources.ApplyResources(Me.chModifier, "chModifier")
            '
            'chIndex
            '
            resources.ApplyResources(Me.chIndex, "chIndex")
            '
            'lvAllShapes
            '
            resources.ApplyResources(Me.lvAllShapes, "lvAllShapes")
            Me.lvAllShapes.HideSelection = False
            Me.lvAllShapes.MultiSelect = False
            Me.lvAllShapes.Name = "lvAllShapes"
            Me.lvAllShapes.UseCompatibleStateImageBehavior = False
            '
            'btnRemove
            '
            Me.btnRemove.Image = Global.ScientificInterface.My.Resources.Resources.delete
            resources.ApplyResources(Me.btnRemove, "btnRemove")
            Me.btnRemove.Name = "btnRemove"
            Me.btnRemove.UseVisualStyleBackColor = True
            '
            'lblTitle
            '
            resources.ApplyResources(Me.lblTitle, "lblTitle")
            Me.lblTitle.Name = "lblTitle"
            '
            'lblAppliedFF
            '
            resources.ApplyResources(Me.lblAppliedFF, "lblAppliedFF")
            Me.lblAppliedFF.Name = "lblAppliedFF"
            '
            'dlgApplyShape
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.txbPredName)
            Me.Controls.Add(Me.lblPred)
            Me.Controls.Add(Me.txbPreyName)
            Me.Controls.Add(Me.lvAppliedShapes)
            Me.Controls.Add(Me.lvAllShapes)
            Me.Controls.Add(Me.lblPrey)
            Me.Controls.Add(Me.btnRemove)
            Me.Controls.Add(Me.lblTitle)
            Me.Controls.Add(Me.btnAdd)
            Me.Controls.Add(Me.gbMultipliers)
            Me.Controls.Add(Me.lblAppliedFF)
            Me.Controls.Add(Me.lblAvailableFF)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgApplyShape"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.gbMultipliers.ResumeLayout(False)
            Me.gbMultipliers.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents OK_Button As System.Windows.Forms.Button
        Friend WithEvents Cancel_Button As System.Windows.Forms.Button
        Friend WithEvents lblPred As System.Windows.Forms.Label
        Friend WithEvents lblPrey As System.Windows.Forms.Label
        Friend WithEvents lblAvailableFF As System.Windows.Forms.Label
        Friend WithEvents lblSearchRate As System.Windows.Forms.Label
        Friend WithEvents txbPreyName As System.Windows.Forms.TextBox
        Friend WithEvents txbPredName As System.Windows.Forms.TextBox
        Friend WithEvents gbMultipliers As System.Windows.Forms.GroupBox
        Friend WithEvents btnAdd As System.Windows.Forms.Button
        Friend WithEvents lvAppliedShapes As System.Windows.Forms.ListView
        Friend WithEvents lvAllShapes As System.Windows.Forms.ListView
        Friend WithEvents chShape As System.Windows.Forms.ColumnHeader
        Friend WithEvents chModifier As System.Windows.Forms.ColumnHeader
        Friend WithEvents btnRemove As System.Windows.Forms.Button
        Friend WithEvents rbSearchRate As System.Windows.Forms.RadioButton
        Friend WithEvents rbVulArea As System.Windows.Forms.RadioButton
        Friend WithEvents rbArea As System.Windows.Forms.RadioButton
        Friend WithEvents rbVul As System.Windows.Forms.RadioButton
        Friend WithEvents chIndex As System.Windows.Forms.ColumnHeader
        Friend WithEvents rbProdRate As System.Windows.Forms.RadioButton
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents lblAppliedFF As System.Windows.Forms.Label

    End Class

End Namespace

