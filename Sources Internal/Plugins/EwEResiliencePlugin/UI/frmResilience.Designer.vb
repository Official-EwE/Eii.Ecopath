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
Imports ScientificInterfaceShared.Forms

Partial Class frmResilience
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmResilience))
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        Me.m_btnRunEcosim = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'm_graph
        '
        resources.ApplyResources(Me.m_graph, "m_graph")
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0.0R
        Me.m_graph.ScrollMaxX = 0.0R
        Me.m_graph.ScrollMaxY = 0.0R
        Me.m_graph.ScrollMaxY2 = 0.0R
        Me.m_graph.ScrollMinX = 0.0R
        Me.m_graph.ScrollMinY = 0.0R
        Me.m_graph.ScrollMinY2 = 0.0R
        '
        'm_cbAutosave
        '
        resources.ApplyResources(Me.m_cbAutosave, "m_cbAutosave")
        Me.m_cbAutosave.Name = "m_cbAutosave"
        Me.m_cbAutosave.UseVisualStyleBackColor = True
        '
        'm_btnRunEcosim
        '
        resources.ApplyResources(Me.m_btnRunEcosim, "m_btnRunEcosim")
        Me.m_btnRunEcosim.Name = "m_btnRunEcosim"
        Me.m_btnRunEcosim.UseVisualStyleBackColor = True
        '
        'frmResilience
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_btnRunEcosim)
        Me.Controls.Add(Me.m_cbAutosave)
        Me.Controls.Add(Me.m_graph)
        Me.Name = "frmResilience"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_cbAutosave As System.Windows.Forms.CheckBox
    Private WithEvents m_btnRunEcosim As System.Windows.Forms.Button
End Class
