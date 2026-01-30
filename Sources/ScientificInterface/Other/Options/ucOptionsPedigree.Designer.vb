' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls

Namespace Other

    Partial Class ucOptionsPedigree
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsPedigree))
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbShowPedigreeIndicators = New System.Windows.Forms.CheckBox()
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
            'm_cbShowPedigreeIndicators
            '
            resources.ApplyResources(Me.m_cbShowPedigreeIndicators, "m_cbShowPedigreeIndicators")
            Me.m_cbShowPedigreeIndicators.Name = "m_cbShowPedigreeIndicators"
            Me.m_cbShowPedigreeIndicators.UseVisualStyleBackColor = True
            '
            'ucOptionsPedigree
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_cbShowPedigreeIndicators)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Name = "ucOptionsPedigree"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private m_hdrCaption As cEwEHeaderLabel
        Friend WithEvents m_cbShowPedigreeIndicators As System.Windows.Forms.CheckBox

    End Class
End Namespace

