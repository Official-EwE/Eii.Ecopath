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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Forms

#End Region ' Imports

Partial Class frmTransectSummary
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTransectSummary))
        Me.m_ts = New System.Windows.Forms.ToolStrip()
        Me.m_tscmbTransect = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbnPlay = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnStop = New System.Windows.Forms.ToolStripButton()
        Me.m_tslblPlayTime = New System.Windows.Forms.ToolStripLabel()
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_timerPlay = New System.Windows.Forms.Timer(Me.components)
        Me.m_ts.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_ts
        '
        Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tscmbTransect, Me.ToolStripSeparator1, Me.m_tsbnPlay, Me.m_tsbnStop, Me.m_tslblPlayTime})
        resources.ApplyResources(Me.m_ts, "m_ts")
        Me.m_ts.Name = "m_ts"
        '
        'm_tscmbTransect
        '
        Me.m_tscmbTransect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbTransect.Name = "m_tscmbTransect"
        resources.ApplyResources(Me.m_tscmbTransect, "m_tscmbTransect")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'm_tsbnPlay
        '
        Me.m_tsbnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnPlay, "m_tsbnPlay")
        Me.m_tsbnPlay.Name = "m_tsbnPlay"
        '
        'm_tsbnStop
        '
        Me.m_tsbnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnStop, "m_tsbnStop")
        Me.m_tsbnStop.Name = "m_tsbnStop"
        '
        'm_tslblPlayTime
        '
        Me.m_tslblPlayTime.Name = "m_tslblPlayTime"
        resources.ApplyResources(Me.m_tslblPlayTime, "m_tslblPlayTime")
        '
        'm_graph
        '
        resources.ApplyResources(Me.m_graph, "m_graph")
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0R
        Me.m_graph.ScrollMaxX = 0R
        Me.m_graph.ScrollMaxY = 0R
        Me.m_graph.ScrollMaxY2 = 0R
        Me.m_graph.ScrollMinX = 0R
        Me.m_graph.ScrollMinY = 0R
        Me.m_graph.ScrollMinY2 = 0R
        '
        'm_timerPlay
        '
        Me.m_timerPlay.Interval = 200
        '
        'frmTransectSummary
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_graph)
        Me.Controls.Add(Me.m_ts)
        Me.Name = "frmTransectSummary"
        Me.TabText = ""
        Me.m_ts.ResumeLayout(False)
        Me.m_ts.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_ts As Windows.Forms.ToolStrip
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tscmbTransect As Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripSeparator1 As Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnPlay As Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnStop As Windows.Forms.ToolStripButton
    Private WithEvents m_tslblPlayTime As Windows.Forms.ToolStripLabel
    Private WithEvents m_timerPlay As Windows.Forms.Timer
End Class
