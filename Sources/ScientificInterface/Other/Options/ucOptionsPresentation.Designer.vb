' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Imports ScientificInterfaceShared

Namespace Other

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
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblEntering = New System.Windows.Forms.Label()
            Me.m_cbHideModelBar = New System.Windows.Forms.CheckBox()
            Me.m_cbHideStatusBar = New System.Windows.Forms.CheckBox()
            Me.m_cbHideMainMenu = New System.Windows.Forms.CheckBox()
            Me.m_cbCollapseNavPanel = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            '
            'm_hdrCaption
            '
            Me.m_hdrCaption.CanCollapseParent = False
            Me.m_hdrCaption.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.IsCollapsed = False
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
            'ucOptionsPresentation
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
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

    End Class

End Namespace

