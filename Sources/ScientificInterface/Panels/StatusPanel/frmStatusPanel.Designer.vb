' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms

Partial Class frmStatusPanel
    Inherits frmEwEDockContent

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStatusPanel))
        Me.m_tvStatus = New ScientificInterfaceShared.Controls.cNavigateTreeview()
        Me.SuspendLayout()
        '
        'm_tvStatus
        '
        resources.ApplyResources(Me.m_tvStatus, "m_tvStatus")
        Me.m_tvStatus.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText
        Me.m_tvStatus.FullRowSelect = True
        Me.m_tvStatus.HideSelection = False
        Me.m_tvStatus.Name = "m_tvStatus"
        Me.m_tvStatus.ShowImages = True
        Me.m_tvStatus.ShowLines = False
        Me.m_tvStatus.ShowTime = False
        '
        'frmStatusPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CloseButtonVisible = False
        Me.Controls.Add(Me.m_tvStatus)
        Me.DockAreas = CType((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom), WeifenLuo.WinFormsUI.Docking.DockAreas)
        Me.DoubleBuffered = True
        Me.HideOnClose = True
        Me.Name = "frmStatusPanel"
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tvStatus As ScientificInterfaceShared.Controls.cNavigateTreeview

End Class

