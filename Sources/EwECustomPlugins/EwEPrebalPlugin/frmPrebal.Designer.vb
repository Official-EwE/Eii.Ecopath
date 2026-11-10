' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms
Imports WeifenLuo.WinFormsUI.Docking
Imports ZedGraph
Imports ScientificInterfaceShared.Controls

Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class frmPrebal
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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPrebal))
            Me.m_graph = New ZedGraph.ZedGraphControl()
            Me.SuspendLayout()
            '
            'm_graph
            '
            Me.m_graph.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_graph.Location = New System.Drawing.Point(0, 0)
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0R
            Me.m_graph.ScrollMaxX = 0R
            Me.m_graph.ScrollMaxY = 0R
            Me.m_graph.ScrollMaxY2 = 0R
            Me.m_graph.ScrollMinX = 0R
            Me.m_graph.ScrollMinY = 0R
            Me.m_graph.ScrollMinY2 = 0R
            Me.m_graph.Size = New System.Drawing.Size(516, 391)
            Me.m_graph.TabIndex = 2
            '
            'frmPrebal
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.ClientSize = New System.Drawing.Size(516, 391)
            Me.Controls.Add(Me.m_graph)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Name = "frmPrebal"
            Me.ShowInTaskbar = False
            Me.TabText = ""
            Me.Text = "Pre-bal"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
    End Class

End Namespace
