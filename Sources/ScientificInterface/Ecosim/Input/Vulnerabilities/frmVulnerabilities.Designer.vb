' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms

Namespace Ecosim

    Partial Class frmVulnerabilities
        Inherits frmEwEGrid

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmVulnerabilities))
            Me.m_tsVUlnerabilities = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbEstimateVs = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnScaleVtoTL = New System.Windows.Forms.ToolStripButton()
            Me.m_tsVUlnerabilities.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsVUlnerabilities
            '
            Me.m_tsVUlnerabilities.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsVUlnerabilities.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbEstimateVs, Me.m_tsbnScaleVtoTL})
            resources.ApplyResources(Me.m_tsVUlnerabilities, "m_tsVUlnerabilities")
            Me.m_tsVUlnerabilities.Name = "m_tsVUlnerabilities"
            Me.m_tsVUlnerabilities.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbEstimateVs
            '
            resources.ApplyResources(Me.m_tsbEstimateVs, "m_tsbEstimateVs")
            Me.m_tsbEstimateVs.Name = "m_tsbEstimateVs"
            '
            'm_tsbnScaleVtoTL
            '
            Me.m_tsbnScaleVtoTL.AutoToolTip = False
            Me.m_tsbnScaleVtoTL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnScaleVtoTL, "m_tsbnScaleVtoTL")
            Me.m_tsbnScaleVtoTL.Name = "m_tsbnScaleVtoTL"
            '
            'frmVulnerabilities
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_tsVUlnerabilities)
            Me.Name = "frmVulnerabilities"
            Me.TabText = ""
            Me.m_tsVUlnerabilities.ResumeLayout(False)
            Me.m_tsVUlnerabilities.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsVUlnerabilities As cEwEToolstrip
        Private WithEvents m_tsbEstimateVs As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnScaleVtoTL As ToolStripButton
    End Class

End Namespace