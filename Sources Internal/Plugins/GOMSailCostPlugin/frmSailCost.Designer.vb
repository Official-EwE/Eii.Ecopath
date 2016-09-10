<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSailCost
    Inherits ScientificInterfaceShared.Forms.frmEwE

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSailCost))
        Me.m_lblWarning = New System.Windows.Forms.Label()
        Me.m_chkUseSailCost = New System.Windows.Forms.CheckBox()
        Me.m_lblPath = New System.Windows.Forms.Label()
        Me.m_btnChoosePath = New System.Windows.Forms.Button()
        Me.m_tbxPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lvValidation = New System.Windows.Forms.ListView()
        Me.m_colDriver = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_colFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_colFound = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'm_lblWarning
        '
        resources.ApplyResources(Me.m_lblWarning, "m_lblWarning")
        Me.m_lblWarning.BackColor = System.Drawing.Color.LightYellow
        Me.m_lblWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lblWarning.Name = "m_lblWarning"
        '
        'm_chkUseSailCost
        '
        resources.ApplyResources(Me.m_chkUseSailCost, "m_chkUseSailCost")
        Me.m_chkUseSailCost.Name = "m_chkUseSailCost"
        Me.m_chkUseSailCost.UseVisualStyleBackColor = True
        '
        'm_lblPath
        '
        resources.ApplyResources(Me.m_lblPath, "m_lblPath")
        Me.m_lblPath.Name = "m_lblPath"
        '
        'm_btnChoosePath
        '
        resources.ApplyResources(Me.m_btnChoosePath, "m_btnChoosePath")
        Me.m_btnChoosePath.Name = "m_btnChoosePath"
        Me.m_btnChoosePath.UseVisualStyleBackColor = True
        '
        'm_tbxPath
        '
        resources.ApplyResources(Me.m_tbxPath, "m_tbxPath")
        Me.m_tbxPath.Name = "m_tbxPath"
        Me.m_tbxPath.ReadOnly = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_lvValidation
        '
        resources.ApplyResources(Me.m_lvValidation, "m_lvValidation")
        Me.m_lvValidation.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.m_colDriver, Me.m_colFile, Me.m_colFound})
        Me.m_lvValidation.FullRowSelect = True
        Me.m_lvValidation.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.m_lvValidation.Name = "m_lvValidation"
        Me.m_lvValidation.UseCompatibleStateImageBehavior = False
        Me.m_lvValidation.View = System.Windows.Forms.View.Details
        '
        'm_colDriver
        '
        resources.ApplyResources(Me.m_colDriver, "m_colDriver")
        '
        'm_colFile
        '
        resources.ApplyResources(Me.m_colFile, "m_colFile")
        '
        'm_colFound
        '
        resources.ApplyResources(Me.m_colFound, "m_colFound")
        '
        'frmSailCost
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lvValidation)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_tbxPath)
        Me.Controls.Add(Me.m_btnChoosePath)
        Me.Controls.Add(Me.m_lblPath)
        Me.Controls.Add(Me.m_chkUseSailCost)
        Me.Controls.Add(Me.m_lblWarning)
        Me.Name = "frmSailCost"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblPath As Windows.Forms.Label
    Private WithEvents m_btnChoosePath As Windows.Forms.Button
    Private WithEvents m_lblWarning As Windows.Forms.Label
    Private WithEvents m_tbxPath As Windows.Forms.TextBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Private WithEvents m_lvValidation As Windows.Forms.ListView
    Private WithEvents m_colDriver As Windows.Forms.ColumnHeader
    Private WithEvents m_colFile As Windows.Forms.ColumnHeader
    Private WithEvents m_colFound As Windows.Forms.ColumnHeader
    Private WithEvents m_chkUseSailCost As Windows.Forms.CheckBox
End Class
