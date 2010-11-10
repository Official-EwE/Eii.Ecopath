Imports ScientificInterfaceShared

Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsPresentation
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsPresentation))
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_lblEntering = New System.Windows.Forms.Label
            Me.m_cbHideModelBar = New System.Windows.Forms.CheckBox
            Me.m_cbHideStatusBar = New System.Windows.Forms.CheckBox
            Me.m_cbHideMainMenu = New System.Windows.Forms.CheckBox
            Me.m_cbCollapseNavPanel = New System.Windows.Forms.CheckBox
            Me.m_cbCollapseRemarkPanel = New System.Windows.Forms.CheckBox
            Me.m_cbCollapseStatusPanel = New System.Windows.Forms.CheckBox
            Me.m_cbRestoreItems = New System.Windows.Forms.CheckBox
            Me.m_lblExiting = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'm_hdrCaption
            '
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.Name = "m_hdrCaption"
            '
            'm_lblEntering
            '
            resources.ApplyResources(Me.m_lblEntering, "m_lblEntering")
            Me.m_lblEntering.Name = "m_lblEntering"
            '
            'm_cbHideModelBar
            '
            resources.ApplyResources(Me.m_cbHideModelBar, "m_cbHideModelBar")
            Me.m_cbHideModelBar.Name = "m_cbHideModelBar"
            Me.m_cbHideModelBar.UseVisualStyleBackColor = True
            '
            'm_cbHideStatusBar
            '
            resources.ApplyResources(Me.m_cbHideStatusBar, "m_cbHideStatusBar")
            Me.m_cbHideStatusBar.Name = "m_cbHideStatusBar"
            Me.m_cbHideStatusBar.UseVisualStyleBackColor = True
            '
            'm_cbHideMainMenu
            '
            resources.ApplyResources(Me.m_cbHideMainMenu, "m_cbHideMainMenu")
            Me.m_cbHideMainMenu.Name = "m_cbHideMainMenu"
            Me.m_cbHideMainMenu.UseVisualStyleBackColor = True
            '
            'm_cbCollapseNavPanel
            '
            resources.ApplyResources(Me.m_cbCollapseNavPanel, "m_cbCollapseNavPanel")
            Me.m_cbCollapseNavPanel.Name = "m_cbCollapseNavPanel"
            Me.m_cbCollapseNavPanel.UseVisualStyleBackColor = True
            '
            'm_cbCollapseRemarkPanel
            '
            resources.ApplyResources(Me.m_cbCollapseRemarkPanel, "m_cbCollapseRemarkPanel")
            Me.m_cbCollapseRemarkPanel.Name = "m_cbCollapseRemarkPanel"
            Me.m_cbCollapseRemarkPanel.UseVisualStyleBackColor = True
            '
            'm_cbCollapseStatusPanel
            '
            resources.ApplyResources(Me.m_cbCollapseStatusPanel, "m_cbCollapseStatusPanel")
            Me.m_cbCollapseStatusPanel.Name = "m_cbCollapseStatusPanel"
            Me.m_cbCollapseStatusPanel.UseVisualStyleBackColor = True
            '
            'm_cbRestoreItems
            '
            resources.ApplyResources(Me.m_cbRestoreItems, "m_cbRestoreItems")
            Me.m_cbRestoreItems.Name = "m_cbRestoreItems"
            Me.m_cbRestoreItems.UseVisualStyleBackColor = True
            '
            'm_lblExiting
            '
            resources.ApplyResources(Me.m_lblExiting, "m_lblExiting")
            Me.m_lblExiting.Name = "m_lblExiting"
            '
            'ucOptionsPresentation
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblExiting)
            Me.Controls.Add(Me.m_cbRestoreItems)
            Me.Controls.Add(Me.m_cbCollapseStatusPanel)
            Me.Controls.Add(Me.m_cbCollapseRemarkPanel)
            Me.Controls.Add(Me.m_cbCollapseNavPanel)
            Me.Controls.Add(Me.m_cbHideMainMenu)
            Me.Controls.Add(Me.m_cbHideStatusBar)
            Me.Controls.Add(Me.m_cbHideModelBar)
            Me.Controls.Add(Me.m_lblEntering)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Name = "ucOptionsPresentation"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_lblEntering As System.Windows.Forms.Label
        Private WithEvents m_cbHideModelBar As System.Windows.Forms.CheckBox
        Private WithEvents m_cbHideStatusBar As System.Windows.Forms.CheckBox
        Private WithEvents m_cbHideMainMenu As System.Windows.Forms.CheckBox
        Private WithEvents m_cbCollapseNavPanel As System.Windows.Forms.CheckBox
        Private WithEvents m_cbCollapseRemarkPanel As System.Windows.Forms.CheckBox
        Private WithEvents m_cbCollapseStatusPanel As System.Windows.Forms.CheckBox
        Private WithEvents m_lblExiting As System.Windows.Forms.Label
        Private WithEvents m_cbRestoreItems As System.Windows.Forms.CheckBox

    End Class

End Namespace

