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

Imports ScientificInterfaceShared.Controls

Namespace Other

    Partial Class ucOptionsSpatialTemporal
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsSpatialTemporal))
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblCurrent = New System.Windows.Forms.Label()
            Me.m_rbDefault = New System.Windows.Forms.RadioButton()
            Me.m_btnVisitFolder = New System.Windows.Forms.Button()
            Me.m_rbCustom = New System.Windows.Forms.RadioButton()
            Me.m_tbxCustom = New System.Windows.Forms.TextBox()
            Me.m_btnChoose = New System.Windows.Forms.Button()
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
            'm_lblCurrent
            '
            resources.ApplyResources(Me.m_lblCurrent, "m_lblCurrent")
            Me.m_lblCurrent.Name = "m_lblCurrent"
            '
            'm_rbDefault
            '
            resources.ApplyResources(Me.m_rbDefault, "m_rbDefault")
            Me.m_rbDefault.Name = "m_rbDefault"
            Me.m_rbDefault.TabStop = True
            Me.m_rbDefault.UseVisualStyleBackColor = True
            '
            'm_btnVisitFolder
            '
            resources.ApplyResources(Me.m_btnVisitFolder, "m_btnVisitFolder")
            Me.m_btnVisitFolder.Name = "m_btnVisitFolder"
            Me.m_btnVisitFolder.UseVisualStyleBackColor = True
            '
            'm_rbCustom
            '
            resources.ApplyResources(Me.m_rbCustom, "m_rbCustom")
            Me.m_rbCustom.Name = "m_rbCustom"
            Me.m_rbCustom.TabStop = True
            Me.m_rbCustom.UseVisualStyleBackColor = True
            '
            'm_tbxCustom
            '
            resources.ApplyResources(Me.m_tbxCustom, "m_tbxCustom")
            Me.m_tbxCustom.Name = "m_tbxCustom"
            Me.m_tbxCustom.ReadOnly = True
            '
            'm_btnChoose
            '
            resources.ApplyResources(Me.m_btnChoose, "m_btnChoose")
            Me.m_btnChoose.Name = "m_btnChoose"
            Me.m_btnChoose.UseVisualStyleBackColor = True
            '
            'ucOptionsSpatialTemporal
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_btnChoose)
            Me.Controls.Add(Me.m_btnVisitFolder)
            Me.Controls.Add(Me.m_tbxCustom)
            Me.Controls.Add(Me.m_rbCustom)
            Me.Controls.Add(Me.m_rbDefault)
            Me.Controls.Add(Me.m_lblCurrent)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Name = "ucOptionsSpatialTemporal"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_lblCurrent As System.Windows.Forms.Label
        Private WithEvents m_rbDefault As System.Windows.Forms.RadioButton
        Private WithEvents m_btnVisitFolder As System.Windows.Forms.Button
        Private WithEvents m_rbCustom As System.Windows.Forms.RadioButton
        Private WithEvents m_tbxCustom As System.Windows.Forms.TextBox
        Private WithEvents m_btnChoose As System.Windows.Forms.Button

    End Class
End Namespace

