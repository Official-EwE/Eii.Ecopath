' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms

Partial Class frmMSEAssessGroups
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEAssessGroups))
        Me.m_blocks = New ScientificInterface.Ecosim.ucPolicyColorBlocks()
        Me.SuspendLayout()
        '
        'm_blocks
        '
        resources.ApplyResources(Me.m_blocks, "m_blocks")
        Me.m_blocks.ControlPanelVisible = False
        Me.m_blocks.CurrentColor = System.Drawing.Color.Empty
        Me.m_blocks.Name = "m_blocks"
        Me.m_blocks.ParmBlockCodes = Nothing
        Me.m_blocks.ShowTooltip = True
        Me.m_blocks.UIContext = Nothing
        '
        'frmMSEAssessGroups
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Controls.Add(Me.m_blocks)
        Me.Name = "frmMSEAssessGroups"
        Me.TabText = ""
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_blocks As ScientificInterface.Ecosim.ucPolicyColorBlocks
End Class
