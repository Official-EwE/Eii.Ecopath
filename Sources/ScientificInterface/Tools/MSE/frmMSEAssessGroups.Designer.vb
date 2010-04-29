<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEAssessGroups
    Inherits frmEwE

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEAssessGroups))
        Me.m_blocks = New ScientificInterface.Ecosim.ucPolicyColorBlocks
        Me.SuspendLayout()
        '
        'm_blocks
        '
        Me.m_blocks.ControlPanelVisible = False
        Me.m_blocks.CurColor = System.Drawing.Color.Empty
        resources.ApplyResources(Me.m_blocks, "m_blocks")
        Me.m_blocks.Name = "m_blocks"
        Me.m_blocks.ParmBlockCodes = Nothing
        Me.m_blocks.UIContext = Nothing
        '
        'frmMSEAssessGroups
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_blocks)
        Me.Name = "frmMSEAssessGroups"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_blocks As ScientificInterface.Ecosim.ucPolicyColorBlocks
End Class
