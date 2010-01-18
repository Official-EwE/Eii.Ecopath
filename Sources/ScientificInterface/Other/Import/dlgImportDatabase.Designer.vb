Namespace Import

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgImportDatabase
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgImportDatabase))
            Me.m_sep = New ScientificInterfaceShared.Controls.ucFormSeparator
            Me.m_navigator = New ScientificInterfaceShared.Controls.Wizard.ucWizardNavigation
            Me.m_plWizardContent = New System.Windows.Forms.Panel
            Me.SuspendLayout()
            '
            'm_sep
            '
            resources.ApplyResources(Me.m_sep, "m_sep")
            Me.m_sep.Name = "m_sep"
            '
            'm_navigator
            '
            resources.ApplyResources(Me.m_navigator, "m_navigator")
            Me.m_navigator.Name = "m_navigator"
            '
            'm_plWizardContent
            '
            resources.ApplyResources(Me.m_plWizardContent, "m_plWizardContent")
            Me.m_plWizardContent.Name = "m_plWizardContent"
            '
            'dlgImportDatabase
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_plWizardContent)
            Me.Controls.Add(Me.m_navigator)
            Me.Controls.Add(Me.m_sep)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
            Me.Name = "dlgImportDatabase"
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_sep As ScientificInterfaceShared.Controls.ucFormSeparator
        Private WithEvents m_navigator As ScientificInterfaceShared.Controls.Wizard.ucWizardNavigation
        Private WithEvents m_plWizardContent As System.Windows.Forms.Panel
    End Class

End Namespace