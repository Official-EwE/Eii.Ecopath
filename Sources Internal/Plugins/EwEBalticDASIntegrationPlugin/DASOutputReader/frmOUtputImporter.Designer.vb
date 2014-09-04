Partial Class frmOutputImporter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOutputImporter))
        Me.m_lblFile = New System.Windows.Forms.Label()
        Me.m_tbxFile = New System.Windows.Forms.TextBox()
        Me.m_btnChoose = New System.Windows.Forms.Button()
        Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_pbxSponsors = New System.Windows.Forms.PictureBox()
        Me.m_dgvMappings = New System.Windows.Forms.DataGridView()
        Me.m_colVariable = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colLayer = New System.Windows.Forms.DataGridViewComboBoxColumn()
        CType(Me.m_pbxSponsors, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_dgvMappings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblFile
        '
        resources.ApplyResources(Me.m_lblFile, "m_lblFile")
        Me.m_lblFile.Name = "m_lblFile"
        '
        'm_tbxFile
        '
        resources.ApplyResources(Me.m_tbxFile, "m_tbxFile")
        Me.m_tbxFile.Name = "m_tbxFile"
        '
        'm_btnChoose
        '
        resources.ApplyResources(Me.m_btnChoose, "m_btnChoose")
        Me.m_btnChoose.Name = "m_btnChoose"
        Me.m_btnChoose.UseVisualStyleBackColor = True
        '
        'm_hdrSponsors
        '
        resources.ApplyResources(Me.m_hdrSponsors, "m_hdrSponsors")
        Me.m_hdrSponsors.CanCollapseParent = False
        Me.m_hdrSponsors.CollapsedParentHeight = 0
        Me.m_hdrSponsors.IsCollapsed = False
        Me.m_hdrSponsors.Name = "m_hdrSponsors"
        '
        'm_pbxSponsors
        '
        resources.ApplyResources(Me.m_pbxSponsors, "m_pbxSponsors")
        Me.m_pbxSponsors.BackColor = System.Drawing.Color.White
        Me.m_pbxSponsors.BackgroundImage = Global.EwEBalticDASPlugin.My.Resources.Resources.SU_logo
        Me.m_pbxSponsors.Name = "m_pbxSponsors"
        Me.m_pbxSponsors.TabStop = False
        '
        'm_dgvMappings
        '
        Me.m_dgvMappings.AllowUserToAddRows = False
        Me.m_dgvMappings.AllowUserToDeleteRows = False
        Me.m_dgvMappings.AllowUserToResizeRows = False
        resources.ApplyResources(Me.m_dgvMappings, "m_dgvMappings")
        Me.m_dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvMappings.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colVariable, Me.m_colLayer})
        Me.m_dgvMappings.Name = "m_dgvMappings"
        Me.m_dgvMappings.RowHeadersVisible = False
        '
        'm_colVariable
        '
        Me.m_colVariable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colVariable, "m_colVariable")
        Me.m_colVariable.Name = "m_colVariable"
        Me.m_colVariable.ReadOnly = True
        '
        'm_colLayer
        '
        Me.m_colLayer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colLayer, "m_colLayer")
        Me.m_colLayer.Name = "m_colLayer"
        '
        'frmOutputImporter
        '
        Me.AcceptButton = Me.m_btnChoose
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_dgvMappings)
        Me.Controls.Add(Me.m_pbxSponsors)
        Me.Controls.Add(Me.m_hdrSponsors)
        Me.Controls.Add(Me.m_btnChoose)
        Me.Controls.Add(Me.m_tbxFile)
        Me.Controls.Add(Me.m_lblFile)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOutputImporter"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.m_pbxSponsors, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_dgvMappings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_lblFile As System.Windows.Forms.Label
    Private WithEvents m_tbxFile As System.Windows.Forms.TextBox
    Private WithEvents m_btnChoose As System.Windows.Forms.Button
    Private WithEvents m_hdrSponsors As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_pbxSponsors As System.Windows.Forms.PictureBox
    Private WithEvents m_dgvMappings As System.Windows.Forms.DataGridView
    Friend WithEvents m_colVariable As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colLayer As System.Windows.Forms.DataGridViewComboBoxColumn

End Class
