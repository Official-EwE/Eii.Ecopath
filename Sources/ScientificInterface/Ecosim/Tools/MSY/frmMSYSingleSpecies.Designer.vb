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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms
Imports ZedGraph

Namespace Ecosim

    Partial Class frmMSYSingleSpecies
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSYSingleSpecies))
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnShowHide = New System.Windows.Forms.ToolStripButton()
            Me.m_tssep1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnGroup = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnFleet = New System.Windows.Forms.ToolStripButton()
            Me.m_tscmbItem = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tssep2 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tslAssessment = New System.Windows.Forms.ToolStripLabel()
            Me.m_tsbnFull = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnStationary = New System.Windows.Forms.ToolStripButton()
            Me.m_tssep3 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tslView = New System.Windows.Forms.ToolStripLabel()
            Me.m_tscmbView = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tsbnSaveOutput = New System.Windows.Forms.ToolStripButton()
            Me.m_graph = New ZedGraph.ZedGraphControl()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnShowHide, Me.m_tssep1, Me.m_tsbnGroup, Me.m_tsbnFleet, Me.m_tscmbItem, Me.m_tssep2, Me.m_tslAssessment, Me.m_tsbnFull, Me.m_tsbnStationary, Me.m_tssep3, Me.m_tslView, Me.m_tscmbView, Me.m_tsbnSaveOutput})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnShowHide
            '
            Me.m_tsbnShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnShowHide, "m_tsbnShowHide")
            Me.m_tsbnShowHide.Name = "m_tsbnShowHide"
            '
            'm_tssep1
            '
            Me.m_tssep1.Name = "m_tssep1"
            resources.ApplyResources(Me.m_tssep1, "m_tssep1")
            '
            'm_tsbnGroup
            '
            Me.m_tsbnGroup.CheckOnClick = True
            Me.m_tsbnGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnGroup, "m_tsbnGroup")
            Me.m_tsbnGroup.Name = "m_tsbnGroup"
            '
            'm_tsbnFleet
            '
            Me.m_tsbnFleet.CheckOnClick = True
            Me.m_tsbnFleet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnFleet, "m_tsbnFleet")
            Me.m_tsbnFleet.Name = "m_tsbnFleet"
            '
            'm_tscmbItem
            '
            Me.m_tscmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmbItem.Name = "m_tscmbItem"
            resources.ApplyResources(Me.m_tscmbItem, "m_tscmbItem")
            '
            'm_tssep2
            '
            Me.m_tssep2.Name = "m_tssep2"
            resources.ApplyResources(Me.m_tssep2, "m_tssep2")
            '
            'm_tslAssessment
            '
            Me.m_tslAssessment.Name = "m_tslAssessment"
            resources.ApplyResources(Me.m_tslAssessment, "m_tslAssessment")
            '
            'm_tsbnFull
            '
            Me.m_tsbnFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnFull, "m_tsbnFull")
            Me.m_tsbnFull.Name = "m_tsbnFull"
            '
            'm_tsbnStationary
            '
            Me.m_tsbnStationary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnStationary, "m_tsbnStationary")
            Me.m_tsbnStationary.Name = "m_tsbnStationary"
            '
            'm_tssep3
            '
            Me.m_tssep3.Name = "m_tssep3"
            resources.ApplyResources(Me.m_tssep3, "m_tssep3")
            '
            'm_tslView
            '
            Me.m_tslView.Name = "m_tslView"
            resources.ApplyResources(Me.m_tslView, "m_tslView")
            '
            'm_tscmbView
            '
            Me.m_tscmbView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmbView.Items.AddRange(New Object() {resources.GetString("m_tscmbView.Items"), resources.GetString("m_tscmbView.Items1")})
            Me.m_tscmbView.Name = "m_tscmbView"
            resources.ApplyResources(Me.m_tscmbView, "m_tscmbView")
            '
            'm_tsbnSaveOutput
            '
            Me.m_tsbnSaveOutput.CheckOnClick = True
            Me.m_tsbnSaveOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnSaveOutput, "m_tsbnSaveOutput")
            Me.m_tsbnSaveOutput.Name = "m_tsbnSaveOutput"
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
            'frmMSYSingleSpecies
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_graph)
            Me.Controls.Add(Me.m_ts)
            Me.Name = "frmMSYSingleSpecies"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbnShowHide As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tssep1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbnGroup As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnFleet As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tscmbItem As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tssep2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tslAssessment As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsbnFull As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnStationary As System.Windows.Forms.ToolStripButton
        Private WithEvents m_graph As ZedGraphControl
        Private WithEvents m_tssep3 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tslView As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tscmbView As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tsbnSaveOutput As System.Windows.Forms.ToolStripButton
    End Class

End Namespace
