Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorMigration
        Inherits ucLayerEditor

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorMigration))
            Me.m_lbMonth = New System.Windows.Forms.Label
            Me.m_cmbMonth = New System.Windows.Forms.ComboBox
            Me.m_chkAutoRotate = New System.Windows.Forms.CheckBox
            Me.m_lblGroup = New System.Windows.Forms.Label
            Me.m_cmbGroup = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'm_lbMonth
            '
            resources.ApplyResources(Me.m_lbMonth, "m_lbMonth")
            Me.m_lbMonth.Name = "m_lbMonth"
            '
            'm_cmbMonth
            '
            resources.ApplyResources(Me.m_cmbMonth, "m_cmbMonth")
            Me.m_cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbMonth.FormattingEnabled = True
            Me.m_cmbMonth.Items.AddRange(New Object() {resources.GetString("m_cmbMonth.Items"), resources.GetString("m_cmbMonth.Items1"), resources.GetString("m_cmbMonth.Items2"), resources.GetString("m_cmbMonth.Items3"), resources.GetString("m_cmbMonth.Items4"), resources.GetString("m_cmbMonth.Items5"), resources.GetString("m_cmbMonth.Items6"), resources.GetString("m_cmbMonth.Items7"), resources.GetString("m_cmbMonth.Items8"), resources.GetString("m_cmbMonth.Items9"), resources.GetString("m_cmbMonth.Items10"), resources.GetString("m_cmbMonth.Items11")})
            Me.m_cmbMonth.Name = "m_cmbMonth"
            '
            'm_chkAutoRotate
            '
            resources.ApplyResources(Me.m_chkAutoRotate, "m_chkAutoRotate")
            Me.m_chkAutoRotate.Checked = True
            Me.m_chkAutoRotate.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_chkAutoRotate.Name = "m_chkAutoRotate"
            Me.m_chkAutoRotate.UseVisualStyleBackColor = True
            '
            'm_lblGroup
            '
            resources.ApplyResources(Me.m_lblGroup, "m_lblGroup")
            Me.m_lblGroup.Name = "m_lblGroup"
            '
            'm_cmbGroup
            '
            resources.ApplyResources(Me.m_cmbGroup, "m_cmbGroup")
            Me.m_cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroup.FormattingEnabled = True
            Me.m_cmbGroup.Items.AddRange(New Object() {resources.GetString("m_cmbGroup.Items"), resources.GetString("m_cmbGroup.Items1"), resources.GetString("m_cmbGroup.Items2"), resources.GetString("m_cmbGroup.Items3"), resources.GetString("m_cmbGroup.Items4"), resources.GetString("m_cmbGroup.Items5"), resources.GetString("m_cmbGroup.Items6"), resources.GetString("m_cmbGroup.Items7"), resources.GetString("m_cmbGroup.Items8"), resources.GetString("m_cmbGroup.Items9"), resources.GetString("m_cmbGroup.Items10"), resources.GetString("m_cmbGroup.Items11")})
            Me.m_cmbGroup.Name = "m_cmbGroup"
            '
            'ucLayerEditorMigration
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbMonth)
            Me.Controls.Add(Me.m_chkAutoRotate)
            Me.Controls.Add(Me.m_cmbGroup)
            Me.Controls.Add(Me.m_lbMonth)
            Me.Controls.Add(Me.m_lblGroup)
            Me.Name = "ucLayerEditorMigration"
            Me.Controls.SetChildIndex(Me.m_lblGroup, 0)
            Me.Controls.SetChildIndex(Me.m_lbMonth, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroup, 0)
            Me.Controls.SetChildIndex(Me.m_chkAutoRotate, 0)
            Me.Controls.SetChildIndex(Me.m_cmbMonth, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lbMonth As System.Windows.Forms.Label
        Friend WithEvents m_cmbMonth As System.Windows.Forms.ComboBox
        Friend WithEvents m_chkAutoRotate As System.Windows.Forms.CheckBox
        Private WithEvents m_lblGroup As System.Windows.Forms.Label
        Friend WithEvents m_cmbGroup As System.Windows.Forms.ComboBox

    End Class

End Namespace
