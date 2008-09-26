<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgEditBasemap
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditBasemap))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.lblRowCount = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.nudRowCount = New System.Windows.Forms.NumericUpDown
        Me.nudColCount = New System.Windows.Forms.NumericUpDown
        Me.lblCellLength = New System.Windows.Forms.Label
        Me.gbDimensions = New System.Windows.Forms.GroupBox
        Me.gbSpatialRef = New System.Windows.Forms.GroupBox
        Me.lblLonTL = New System.Windows.Forms.Label
        Me.lblLatTL = New System.Windows.Forms.Label
        Me.nudLatTL = New System.Windows.Forms.NumericUpDown
        Me.nudLonTL = New System.Windows.Forms.NumericUpDown
        Me.nudCellLength = New System.Windows.Forms.NumericUpDown
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.nudRowCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudColCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbDimensions.SuspendLayout()
        Me.gbSpatialRef.SuspendLayout()
        CType(Me.nudLatTL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudLonTL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudCellLength, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'lblRowCount
        '
        resources.ApplyResources(Me.lblRowCount, "lblRowCount")
        Me.lblRowCount.Name = "lblRowCount"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'nudRowCount
        '
        resources.ApplyResources(Me.nudRowCount, "nudRowCount")
        Me.nudRowCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudRowCount.Name = "nudRowCount"
        Me.nudRowCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'nudColCount
        '
        resources.ApplyResources(Me.nudColCount, "nudColCount")
        Me.nudColCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudColCount.Name = "nudColCount"
        Me.nudColCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'lblCellLength
        '
        resources.ApplyResources(Me.lblCellLength, "lblCellLength")
        Me.lblCellLength.Name = "lblCellLength"
        '
        'gbDimensions
        '
        resources.ApplyResources(Me.gbDimensions, "gbDimensions")
        Me.gbDimensions.Controls.Add(Me.lblRowCount)
        Me.gbDimensions.Controls.Add(Me.nudRowCount)
        Me.gbDimensions.Controls.Add(Me.Label1)
        Me.gbDimensions.Controls.Add(Me.nudColCount)
        Me.gbDimensions.Name = "gbDimensions"
        Me.gbDimensions.TabStop = False
        '
        'gbSpatialRef
        '
        resources.ApplyResources(Me.gbSpatialRef, "gbSpatialRef")
        Me.gbSpatialRef.Controls.Add(Me.lblLonTL)
        Me.gbSpatialRef.Controls.Add(Me.nudCellLength)
        Me.gbSpatialRef.Controls.Add(Me.nudLonTL)
        Me.gbSpatialRef.Controls.Add(Me.nudLatTL)
        Me.gbSpatialRef.Controls.Add(Me.lblLatTL)
        Me.gbSpatialRef.Controls.Add(Me.lblCellLength)
        Me.gbSpatialRef.Name = "gbSpatialRef"
        Me.gbSpatialRef.TabStop = False
        '
        'lblLonTL
        '
        resources.ApplyResources(Me.lblLonTL, "lblLonTL")
        Me.lblLonTL.Name = "lblLonTL"
        '
        'lblLatTL
        '
        resources.ApplyResources(Me.lblLatTL, "lblLatTL")
        Me.lblLatTL.Name = "lblLatTL"
        '
        'nudLatTL
        '
        resources.ApplyResources(Me.nudLatTL, "nudLatTL")
        Me.nudLatTL.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudLatTL.Name = "nudLatTL"
        Me.nudLatTL.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'nudLonTL
        '
        resources.ApplyResources(Me.nudLonTL, "nudLonTL")
        Me.nudLonTL.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudLonTL.Name = "nudLonTL"
        Me.nudLonTL.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'nudCellLength
        '
        resources.ApplyResources(Me.nudCellLength, "nudCellLength")
        Me.nudCellLength.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudCellLength.Name = "nudCellLength"
        Me.nudCellLength.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'dlgEditBasemap
        '
        Me.AcceptButton = Me.OK_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.gbSpatialRef)
        Me.Controls.Add(Me.gbDimensions)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgEditBasemap"
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.nudRowCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudColCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbDimensions.ResumeLayout(False)
        Me.gbDimensions.PerformLayout()
        Me.gbSpatialRef.ResumeLayout(False)
        Me.gbSpatialRef.PerformLayout()
        CType(Me.nudLatTL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudLonTL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudCellLength, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents lblRowCount As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents nudRowCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudColCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblCellLength As System.Windows.Forms.Label
    Friend WithEvents gbDimensions As System.Windows.Forms.GroupBox
    Friend WithEvents gbSpatialRef As System.Windows.Forms.GroupBox
    Friend WithEvents lblLonTL As System.Windows.Forms.Label
    Friend WithEvents lblLatTL As System.Windows.Forms.Label
    Friend WithEvents nudCellLength As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudLonTL As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudLatTL As System.Windows.Forms.NumericUpDown

End Class
