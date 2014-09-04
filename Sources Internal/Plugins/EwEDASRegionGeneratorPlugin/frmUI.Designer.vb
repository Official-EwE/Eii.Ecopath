<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUI
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUI))
        Me.m_lblFile = New System.Windows.Forms.Label()
        Me.m_tbxFile = New System.Windows.Forms.TextBox()
        Me.m_lblLayers = New System.Windows.Forms.Label()
        Me.m_tbxLayers = New System.Windows.Forms.TextBox()
        Me.m_btnGenerate = New System.Windows.Forms.Button()
        Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_pbxSponsors = New System.Windows.Forms.PictureBox()
        CType(Me.m_pbxSponsors, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'm_lblLayers
        '
        resources.ApplyResources(Me.m_lblLayers, "m_lblLayers")
        Me.m_lblLayers.Name = "m_lblLayers"
        '
        'm_tbxLayers
        '
        resources.ApplyResources(Me.m_tbxLayers, "m_tbxLayers")
        Me.m_tbxLayers.Name = "m_tbxLayers"
        '
        'm_btnGenerate
        '
        resources.ApplyResources(Me.m_btnGenerate, "m_btnGenerate")
        Me.m_btnGenerate.Name = "m_btnGenerate"
        Me.m_btnGenerate.UseVisualStyleBackColor = True
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
        Me.m_pbxSponsors.Name = "m_pbxSponsors"
        Me.m_pbxSponsors.TabStop = False
        '
        'frmUI
        '
        Me.AcceptButton = Me.m_btnGenerate
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_pbxSponsors)
        Me.Controls.Add(Me.m_hdrSponsors)
        Me.Controls.Add(Me.m_btnGenerate)
        Me.Controls.Add(Me.m_tbxLayers)
        Me.Controls.Add(Me.m_lblLayers)
        Me.Controls.Add(Me.m_tbxFile)
        Me.Controls.Add(Me.m_lblFile)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUI"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.m_pbxSponsors, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblFile As System.Windows.Forms.Label
    Private WithEvents m_tbxFile As System.Windows.Forms.TextBox
    Private WithEvents m_lblLayers As System.Windows.Forms.Label
    Private WithEvents m_tbxLayers As System.Windows.Forms.TextBox
    Private WithEvents m_btnGenerate As System.Windows.Forms.Button
    Private WithEvents m_hdrSponsors As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_pbxSponsors As System.Windows.Forms.PictureBox
End Class
