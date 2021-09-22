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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls

Namespace Other

    Partial Class ucOptionsColorRamps
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsColorRamps))
            Me.m_tlpGradients = New System.Windows.Forms.TableLayoutPanel()
            Me.m_lbGradients = New ScientificInterfaceShared.Controls.cFlickerFreeListBox()
            Me.m_hdrSelected = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrGradients = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpDetails = New System.Windows.Forms.TableLayoutPanel()
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnAdd = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDuplicate = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDelete = New System.Windows.Forms.ToolStripButton()
            Me.m_editor = New ScientificInterfaceShared.Controls.ucEditGradient()
            Me.m_tlpGradients.SuspendLayout()
            Me.m_tlpDetails.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpGradients
            '
            resources.ApplyResources(Me.m_tlpGradients, "m_tlpGradients")
            Me.m_tlpGradients.Controls.Add(Me.m_lbGradients, 0, 1)
            Me.m_tlpGradients.Controls.Add(Me.m_hdrSelected, 1, 0)
            Me.m_tlpGradients.Controls.Add(Me.m_hdrGradients, 0, 0)
            Me.m_tlpGradients.Controls.Add(Me.m_tlpDetails, 1, 1)
            Me.m_tlpGradients.Name = "m_tlpGradients"
            '
            'm_lbGradients
            '
            resources.ApplyResources(Me.m_lbGradients, "m_lbGradients")
            Me.m_lbGradients.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGradients.FormattingEnabled = True
            Me.m_lbGradients.Name = "m_lbGradients"
            '
            'm_hdrSelected
            '
            Me.m_hdrSelected.CanCollapseParent = False
            Me.m_hdrSelected.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrSelected, "m_hdrSelected")
            Me.m_hdrSelected.IsCollapsed = False
            Me.m_hdrSelected.Name = "m_hdrSelected"
            '
            'm_hdrGradients
            '
            Me.m_hdrGradients.CanCollapseParent = False
            Me.m_hdrGradients.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrGradients, "m_hdrGradients")
            Me.m_hdrGradients.IsCollapsed = False
            Me.m_hdrGradients.Name = "m_hdrGradients"
            '
            'm_tlpDetails
            '
            resources.ApplyResources(Me.m_tlpDetails, "m_tlpDetails")
            Me.m_tlpDetails.Controls.Add(Me.m_ts, 0, 0)
            Me.m_tlpDetails.Controls.Add(Me.m_editor, 0, 1)
            Me.m_tlpDetails.Name = "m_tlpDetails"
            '
            'm_ts
            '
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnAdd, Me.m_tsbnDuplicate, Me.m_tsbnDelete})
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnAdd
            '
            Me.m_tsbnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnAdd, "m_tsbnAdd")
            Me.m_tsbnAdd.Name = "m_tsbnAdd"
            '
            'm_tsbnDuplicate
            '
            Me.m_tsbnDuplicate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnDuplicate, "m_tsbnDuplicate")
            Me.m_tsbnDuplicate.Name = "m_tsbnDuplicate"
            '
            'm_tsbnDelete
            '
            Me.m_tsbnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnDelete, "m_tsbnDelete")
            Me.m_tsbnDelete.Name = "m_tsbnDelete"
            '
            'm_editor
            '
            resources.ApplyResources(Me.m_editor, "m_editor")
            Me.m_editor.ColorRamp = Nothing
            Me.m_editor.Name = "m_editor"
            '
            'ucOptionsColorRamps
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_tlpGradients)
            Me.Name = "ucOptionsColorRamps"
            Me.m_tlpGradients.ResumeLayout(False)
            Me.m_tlpDetails.ResumeLayout(False)
            Me.m_tlpDetails.PerformLayout()
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpGradients As TableLayoutPanel
        Private WithEvents m_lbGradients As cFlickerFreeListBox
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbnAdd As ToolStripButton
        Private WithEvents m_tsbnDuplicate As ToolStripButton
        Private WithEvents m_tsbnDelete As ToolStripButton
        Private WithEvents m_editor As ucEditGradient
        Private WithEvents m_hdrSelected As cEwEHeaderLabel
        Private WithEvents m_hdrGradients As cEwEHeaderLabel
        Private WithEvents m_tlpDetails As TableLayoutPanel
    End Class
End Namespace

