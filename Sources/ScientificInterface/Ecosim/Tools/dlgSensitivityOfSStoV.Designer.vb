<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgSensitivityOfSStoV
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSensitivityOfSStoV))
        Me.m_ucVulBlocks = New ScientificInterface.Ecosim.ucVulnerabiltyBlocks
        Me.m_gbSearch = New System.Windows.Forms.GroupBox
        Me.m_pbSearch = New System.Windows.Forms.ProgressBar
        Me.m_btnSearch = New System.Windows.Forms.Button
        Me.m_rbSearchPredPrey = New System.Windows.Forms.RadioButton
        Me.m_rbSearchPred = New System.Windows.Forms.RadioButton
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.m_nudNumBlocks = New System.Windows.Forms.NumericUpDown
        Me.m_rbTransferPredPreyCell = New System.Windows.Forms.RadioButton
        Me.m_rbTransferPredRow = New System.Windows.Forms.RadioButton
        Me.m_btnUpdate = New System.Windows.Forms.Button
        Me.m_rbTransferPredCol = New System.Windows.Forms.RadioButton
        Me.m_btnCancel = New System.Windows.Forms.Button
        Me.m_btnOk = New System.Windows.Forms.Button
        Me.m_gbSearch.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.m_nudNumBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_ucVulBlocks
        '
        Me.m_ucVulBlocks.BlockColors = Nothing
        Me.m_ucVulBlocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.m_ucVulBlocks, "m_ucVulBlocks")
        Me.m_ucVulBlocks.Name = "m_ucVulBlocks"
        Me.m_ucVulBlocks.SelectedBlockNum = 0
        '
        'm_gbSearch
        '
        Me.m_gbSearch.Controls.Add(Me.m_pbSearch)
        Me.m_gbSearch.Controls.Add(Me.m_btnSearch)
        Me.m_gbSearch.Controls.Add(Me.m_rbSearchPredPrey)
        Me.m_gbSearch.Controls.Add(Me.m_rbSearchPred)
        resources.ApplyResources(Me.m_gbSearch, "m_gbSearch")
        Me.m_gbSearch.Name = "m_gbSearch"
        Me.m_gbSearch.TabStop = False
        '
        'm_pbSearch
        '
        resources.ApplyResources(Me.m_pbSearch, "m_pbSearch")
        Me.m_pbSearch.Name = "m_pbSearch"
        Me.m_pbSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'm_btnSearch
        '
        resources.ApplyResources(Me.m_btnSearch, "m_btnSearch")
        Me.m_btnSearch.Name = "m_btnSearch"
        Me.m_btnSearch.UseVisualStyleBackColor = True
        '
        'm_rbSearchPredPrey
        '
        resources.ApplyResources(Me.m_rbSearchPredPrey, "m_rbSearchPredPrey")
        Me.m_rbSearchPredPrey.Name = "m_rbSearchPredPrey"
        Me.m_rbSearchPredPrey.UseVisualStyleBackColor = True
        '
        'm_rbSearchPred
        '
        resources.ApplyResources(Me.m_rbSearchPred, "m_rbSearchPred")
        Me.m_rbSearchPred.Checked = True
        Me.m_rbSearchPred.Name = "m_rbSearchPred"
        Me.m_rbSearchPred.TabStop = True
        Me.m_rbSearchPred.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_ucVulBlocks, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.m_gbSearch, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.GroupBox1, 1, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.m_nudNumBlocks)
        Me.GroupBox1.Controls.Add(Me.m_rbTransferPredPreyCell)
        Me.GroupBox1.Controls.Add(Me.m_rbTransferPredRow)
        Me.GroupBox1.Controls.Add(Me.m_btnUpdate)
        Me.GroupBox1.Controls.Add(Me.m_rbTransferPredCol)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_nudNumBlocks
        '
        resources.ApplyResources(Me.m_nudNumBlocks, "m_nudNumBlocks")
        Me.m_nudNumBlocks.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.m_nudNumBlocks.Name = "m_nudNumBlocks"
        Me.m_nudNumBlocks.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'm_rbTransferPredPreyCell
        '
        resources.ApplyResources(Me.m_rbTransferPredPreyCell, "m_rbTransferPredPreyCell")
        Me.m_rbTransferPredPreyCell.Name = "m_rbTransferPredPreyCell"
        Me.m_rbTransferPredPreyCell.UseVisualStyleBackColor = True
        '
        'm_rbTransferPredRow
        '
        resources.ApplyResources(Me.m_rbTransferPredRow, "m_rbTransferPredRow")
        Me.m_rbTransferPredRow.Name = "m_rbTransferPredRow"
        Me.m_rbTransferPredRow.UseVisualStyleBackColor = True
        '
        'm_btnUpdate
        '
        resources.ApplyResources(Me.m_btnUpdate, "m_btnUpdate")
        Me.m_btnUpdate.Name = "m_btnUpdate"
        Me.m_btnUpdate.UseVisualStyleBackColor = True
        '
        'm_rbTransferPredCol
        '
        resources.ApplyResources(Me.m_rbTransferPredCol, "m_rbTransferPredCol")
        Me.m_rbTransferPredCol.Checked = True
        Me.m_rbTransferPredCol.Name = "m_rbTransferPredCol"
        Me.m_rbTransferPredCol.TabStop = True
        Me.m_rbTransferPredCol.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnOk
        '
        resources.ApplyResources(Me.m_btnOk, "m_btnOk")
        Me.m_btnOk.Name = "m_btnOk"
        Me.m_btnOk.UseVisualStyleBackColor = True
        '
        'dlgSensitivityOfSStoV
        '
        Me.AcceptButton = Me.m_btnOk
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOk)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgSensitivityOfSStoV"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.m_gbSearch.ResumeLayout(False)
        Me.m_gbSearch.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.m_nudNumBlocks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_ucVulBlocks As Ecosim.ucVulnerabiltyBlocks
    Friend WithEvents m_gbSearch As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents m_btnSearch As System.Windows.Forms.Button
    Friend WithEvents m_rbSearchPredPrey As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbSearchPred As System.Windows.Forms.RadioButton
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Friend WithEvents m_btnOk As System.Windows.Forms.Button
    Friend WithEvents m_rbTransferPredPreyCell As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbTransferPredRow As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbTransferPredCol As System.Windows.Forms.RadioButton
    Friend WithEvents m_btnUpdate As System.Windows.Forms.Button
    Friend WithEvents m_pbSearch As System.Windows.Forms.ProgressBar
    Friend WithEvents m_nudNumBlocks As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
