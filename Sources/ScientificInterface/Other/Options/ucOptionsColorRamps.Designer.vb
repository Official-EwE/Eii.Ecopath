' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
            Me.m_tlpDetails = New System.Windows.Forms.TableLayoutPanel()
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnAdd = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDuplicate = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDelete = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnImport = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnExport = New System.Windows.Forms.ToolStripButton()
            Me.m_editor = New ScientificInterfaceShared.Controls.ucEditGradient()
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plDefaults = New System.Windows.Forms.Panel()
            Me.m_btnSetFleetDefault = New System.Windows.Forms.Button()
            Me.m_btnSetEwEDefault = New System.Windows.Forms.Button()
            Me.m_plPreviewFleet = New System.Windows.Forms.Panel()
            Me.m_lblFleetDefault = New System.Windows.Forms.Label()
            Me.m_plPreviewEwE = New System.Windows.Forms.Panel()
            Me.m_lblEwEDefault = New System.Windows.Forms.Label()
            Me.m_hdrTitle = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpGradients.SuspendLayout()
            Me.m_tlpDetails.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.m_tlpContent.SuspendLayout()
            Me.m_plDefaults.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpGradients
            '
            resources.ApplyResources(Me.m_tlpGradients, "m_tlpGradients")
            Me.m_tlpGradients.Controls.Add(Me.m_lbGradients, 0, 0)
            Me.m_tlpGradients.Controls.Add(Me.m_tlpDetails, 1, 0)
            Me.m_tlpGradients.Name = "m_tlpGradients"
            '
            'm_lbGradients
            '
            resources.ApplyResources(Me.m_lbGradients, "m_lbGradients")
            Me.m_lbGradients.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGradients.FormattingEnabled = True
            Me.m_lbGradients.Name = "m_lbGradients"
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
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnAdd, Me.m_tsbnDuplicate, Me.m_tsbnDelete, Me.ToolStripSeparator1, Me.m_tsbnImport, Me.m_tsbnExport})
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
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsbnImport
            '
            Me.m_tsbnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnImport, "m_tsbnImport")
            Me.m_tsbnImport.Name = "m_tsbnImport"
            '
            'm_tsbnExport
            '
            Me.m_tsbnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnExport, "m_tsbnExport")
            Me.m_tsbnExport.Name = "m_tsbnExport"
            '
            'm_editor
            '
            resources.ApplyResources(Me.m_editor, "m_editor")
            Me.m_editor.ColorRamp = Nothing
            Me.m_editor.Name = "m_editor"
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_plDefaults, 0, 2)
            Me.m_tlpContent.Controls.Add(Me.m_tlpGradients, 0, 1)
            Me.m_tlpContent.Controls.Add(Me.m_hdrTitle, 0, 0)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_plDefaults
            '
            Me.m_plDefaults.Controls.Add(Me.m_btnSetFleetDefault)
            Me.m_plDefaults.Controls.Add(Me.m_btnSetEwEDefault)
            Me.m_plDefaults.Controls.Add(Me.m_plPreviewFleet)
            Me.m_plDefaults.Controls.Add(Me.m_lblFleetDefault)
            Me.m_plDefaults.Controls.Add(Me.m_plPreviewEwE)
            Me.m_plDefaults.Controls.Add(Me.m_lblEwEDefault)
            resources.ApplyResources(Me.m_plDefaults, "m_plDefaults")
            Me.m_plDefaults.Name = "m_plDefaults"
            '
            'm_btnSetFleetDefault
            '
            resources.ApplyResources(Me.m_btnSetFleetDefault, "m_btnSetFleetDefault")
            Me.m_btnSetFleetDefault.Name = "m_btnSetFleetDefault"
            Me.m_btnSetFleetDefault.UseVisualStyleBackColor = True
            '
            'm_btnSetEwEDefault
            '
            resources.ApplyResources(Me.m_btnSetEwEDefault, "m_btnSetEwEDefault")
            Me.m_btnSetEwEDefault.Name = "m_btnSetEwEDefault"
            Me.m_btnSetEwEDefault.UseVisualStyleBackColor = True
            '
            'm_plPreviewFleet
            '
            resources.ApplyResources(Me.m_plPreviewFleet, "m_plPreviewFleet")
            Me.m_plPreviewFleet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plPreviewFleet.Name = "m_plPreviewFleet"
            '
            'm_lblFleetDefault
            '
            resources.ApplyResources(Me.m_lblFleetDefault, "m_lblFleetDefault")
            Me.m_lblFleetDefault.Name = "m_lblFleetDefault"
            '
            'm_plPreviewEwE
            '
            resources.ApplyResources(Me.m_plPreviewEwE, "m_plPreviewEwE")
            Me.m_plPreviewEwE.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plPreviewEwE.Name = "m_plPreviewEwE"
            '
            'm_lblEwEDefault
            '
            resources.ApplyResources(Me.m_lblEwEDefault, "m_lblEwEDefault")
            Me.m_lblEwEDefault.Name = "m_lblEwEDefault"
            '
            'm_hdrTitle
            '
            Me.m_hdrTitle.CanCollapseParent = False
            Me.m_hdrTitle.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrTitle, "m_hdrTitle")
            Me.m_hdrTitle.IsCollapsed = False
            Me.m_hdrTitle.Name = "m_hdrTitle"
            '
            'ucOptionsColorRamps
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_tlpContent)
            Me.Name = "ucOptionsColorRamps"
            Me.m_tlpGradients.ResumeLayout(False)
            Me.m_tlpDetails.ResumeLayout(False)
            Me.m_tlpDetails.PerformLayout()
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_plDefaults.ResumeLayout(False)
            Me.m_plDefaults.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpGradients As TableLayoutPanel
        Private WithEvents m_lbGradients As cFlickerFreeListBox
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbnAdd As ToolStripButton
        Private WithEvents m_tsbnDuplicate As ToolStripButton
        Private WithEvents m_tsbnDelete As ToolStripButton
        Private WithEvents m_editor As ucEditGradient
        Private WithEvents m_tlpDetails As TableLayoutPanel
        Private WithEvents m_tlpContent As TableLayoutPanel
        Private WithEvents m_plDefaults As Panel
        Private WithEvents m_btnSetFleetDefault As Button
        Private WithEvents m_btnSetEwEDefault As Button
        Private WithEvents m_lblFleetDefault As Label
        Private WithEvents m_lblEwEDefault As Label
        Private WithEvents m_plPreviewFleet As Panel
        Private WithEvents m_plPreviewEwE As Panel
        Private WithEvents m_hdrTitle As cEwEHeaderLabel
        Private WithEvents m_tsbnImport As ToolStripButton
        Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
        Private WithEvents m_tsbnExport As ToolStripButton
    End Class
End Namespace

