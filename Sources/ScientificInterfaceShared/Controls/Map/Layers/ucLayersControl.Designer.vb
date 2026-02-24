' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map

    Partial Class ucLayersControl
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayersControl))
            Me.m_fpItems = New System.Windows.Forms.FlowLayoutPanel()
            Me.SuspendLayout()
            '
            'm_fpItems
            '
            resources.ApplyResources(Me.m_fpItems, "m_fpItems")
            Me.m_fpItems.Name = "m_fpItems"
            '
            'ucLayersControl
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.m_fpItems)
            Me.Name = "ucLayersControl"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_fpItems As System.Windows.Forms.FlowLayoutPanel

    End Class
End Namespace