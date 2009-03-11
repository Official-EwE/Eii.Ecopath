<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmShapeValue
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShapeValue))
        Me.lblName = New System.Windows.Forms.Label
        Me.lblPoolCode = New System.Windows.Forms.Label
        Me.lblType = New System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.TextBox
        Me.cmbType = New System.Windows.Forms.ComboBox
        Me.cmbPoolCode = New System.Windows.Forms.ComboBox
        Me.lblWeight = New System.Windows.Forms.Label
        Me.txtWeight = New System.Windows.Forms.TextBox
        Me.lbValues = New System.Windows.Forms.Label
        Me.lbNoOfYears = New System.Windows.Forms.Label
        Me.nudNoOfYears = New System.Windows.Forms.NumericUpDown
        Me.tlbAll = New System.Windows.Forms.TableLayoutPanel
        Me.tlpNoOfYears = New System.Windows.Forms.TableLayoutPanel
        Me.btnSetNoOfYears = New System.Windows.Forms.Button
        Me.pnlValueGrid = New System.Windows.Forms.Panel
        Me.lblViewAs = New System.Windows.Forms.Label
        Me.cmbViewAs = New System.Windows.Forms.ComboBox
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblXBase = New System.Windows.Forms.Label
        Me.txtXBase = New System.Windows.Forms.TextBox
        CType(Me.nudNoOfYears, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlbAll.SuspendLayout()
        Me.tlpNoOfYears.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblName
        '
        resources.ApplyResources(Me.lblName, "lblName")
        Me.lblName.Name = "lblName"
        '
        'lblPoolCode
        '
        resources.ApplyResources(Me.lblPoolCode, "lblPoolCode")
        Me.lblPoolCode.Name = "lblPoolCode"
        '
        'lblType
        '
        resources.ApplyResources(Me.lblType, "lblType")
        Me.lblType.Name = "lblType"
        '
        'txtName
        '
        resources.ApplyResources(Me.txtName, "txtName")
        Me.txtName.Name = "txtName"
        '
        'cmbType
        '
        resources.ApplyResources(Me.cmbType, "cmbType")
        Me.cmbType.FormattingEnabled = True
        Me.cmbType.Name = "cmbType"
        '
        'cmbPoolCode
        '
        resources.ApplyResources(Me.cmbPoolCode, "cmbPoolCode")
        Me.cmbPoolCode.FormattingEnabled = True
        Me.cmbPoolCode.Name = "cmbPoolCode"
        '
        'lblWeight
        '
        resources.ApplyResources(Me.lblWeight, "lblWeight")
        Me.lblWeight.Name = "lblWeight"
        '
        'txtWeight
        '
        resources.ApplyResources(Me.txtWeight, "txtWeight")
        Me.txtWeight.Name = "txtWeight"
        '
        'lbValues
        '
        resources.ApplyResources(Me.lbValues, "lbValues")
        Me.lbValues.Name = "lbValues"
        '
        'lbNoOfYears
        '
        resources.ApplyResources(Me.lbNoOfYears, "lbNoOfYears")
        Me.lbNoOfYears.Name = "lbNoOfYears"
        '
        'nudNoOfYears
        '
        resources.ApplyResources(Me.nudNoOfYears, "nudNoOfYears")
        Me.nudNoOfYears.Maximum = New Decimal(New Integer() {9000, 0, 0, 0})
        Me.nudNoOfYears.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudNoOfYears.Name = "nudNoOfYears"
        Me.nudNoOfYears.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'tlbAll
        '
        resources.ApplyResources(Me.tlbAll, "tlbAll")
        Me.tlbAll.Controls.Add(Me.txtName, 1, 0)
        Me.tlbAll.Controls.Add(Me.cmbType, 1, 1)
        Me.tlbAll.Controls.Add(Me.lbValues, 0, 6)
        Me.tlbAll.Controls.Add(Me.lblType, 0, 1)
        Me.tlbAll.Controls.Add(Me.lblPoolCode, 0, 2)
        Me.tlbAll.Controls.Add(Me.cmbPoolCode, 1, 2)
        Me.tlbAll.Controls.Add(Me.txtWeight, 1, 3)
        Me.tlbAll.Controls.Add(Me.lblName, 0, 0)
        Me.tlbAll.Controls.Add(Me.lblWeight, 0, 3)
        Me.tlbAll.Controls.Add(Me.lbNoOfYears, 0, 7)
        Me.tlbAll.Controls.Add(Me.tlpNoOfYears, 1, 7)
        Me.tlbAll.Controls.Add(Me.pnlValueGrid, 1, 6)
        Me.tlbAll.Controls.Add(Me.lblViewAs, 0, 5)
        Me.tlbAll.Controls.Add(Me.cmbViewAs, 1, 5)
        Me.tlbAll.Controls.Add(Me.lblXBase, 0, 4)
        Me.tlbAll.Controls.Add(Me.txtXBase, 1, 4)
        Me.tlbAll.Name = "tlbAll"
        '
        'tlpNoOfYears
        '
        resources.ApplyResources(Me.tlpNoOfYears, "tlpNoOfYears")
        Me.tlpNoOfYears.Controls.Add(Me.nudNoOfYears, 0, 0)
        Me.tlpNoOfYears.Controls.Add(Me.btnSetNoOfYears, 1, 0)
        Me.tlpNoOfYears.Name = "tlpNoOfYears"
        '
        'btnSetNoOfYears
        '
        resources.ApplyResources(Me.btnSetNoOfYears, "btnSetNoOfYears")
        Me.btnSetNoOfYears.Name = "btnSetNoOfYears"
        Me.btnSetNoOfYears.UseVisualStyleBackColor = True
        '
        'pnlValueGrid
        '
        resources.ApplyResources(Me.pnlValueGrid, "pnlValueGrid")
        Me.pnlValueGrid.Name = "pnlValueGrid"
        Me.pnlValueGrid.TabStop = True
        '
        'lblViewAs
        '
        resources.ApplyResources(Me.lblViewAs, "lblViewAs")
        Me.lblViewAs.Name = "lblViewAs"
        '
        'cmbViewAs
        '
        resources.ApplyResources(Me.cmbViewAs, "cmbViewAs")
        Me.cmbViewAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbViewAs.FormattingEnabled = True
        Me.cmbViewAs.Items.AddRange(New Object() {resources.GetString("cmbViewAs.Items"), resources.GetString("cmbViewAs.Items1")})
        Me.cmbViewAs.Name = "cmbViewAs"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Name = "btnOK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblXBase
        '
        resources.ApplyResources(Me.lblXBase, "lblXBase")
        Me.lblXBase.Name = "lblXBase"
        '
        'txtXBase
        '
        resources.ApplyResources(Me.txtXBase, "txtXBase")
        Me.txtXBase.Name = "txtXBase"
        '
        'frmShapeValue
        '
        Me.AcceptButton = Me.btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.tlbAll)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmShapeValue"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.nudNoOfYears, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlbAll.ResumeLayout(False)
        Me.tlbAll.PerformLayout()
        Me.tlpNoOfYears.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents lblName As System.Windows.Forms.Label
    Private WithEvents lblPoolCode As System.Windows.Forms.Label
    Private WithEvents lblType As System.Windows.Forms.Label
    Private WithEvents txtName As System.Windows.Forms.TextBox
    Private WithEvents cmbType As System.Windows.Forms.ComboBox
    Private WithEvents cmbPoolCode As System.Windows.Forms.ComboBox
    Private WithEvents lblWeight As System.Windows.Forms.Label
    Private WithEvents txtWeight As System.Windows.Forms.TextBox
    Private WithEvents lbValues As System.Windows.Forms.Label
    Private WithEvents lbNoOfYears As System.Windows.Forms.Label
    Private WithEvents nudNoOfYears As System.Windows.Forms.NumericUpDown
    Private WithEvents tlbAll As System.Windows.Forms.TableLayoutPanel
    Private WithEvents tlpNoOfYears As System.Windows.Forms.TableLayoutPanel
    Private WithEvents btnSetNoOfYears As System.Windows.Forms.Button
    Private WithEvents pnlValueGrid As System.Windows.Forms.Panel
    Private WithEvents btnOK As System.Windows.Forms.Button
    Private WithEvents btnCancel As System.Windows.Forms.Button
    Private WithEvents lblViewAs As System.Windows.Forms.Label
    Private WithEvents cmbViewAs As System.Windows.Forms.ComboBox
    Private WithEvents lblXBase As System.Windows.Forms.Label
    Private WithEvents txtXBase As System.Windows.Forms.TextBox

End Class

