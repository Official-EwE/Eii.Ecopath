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

#Region " Imports "

Option Explicit On
Option Strict On

Imports ScientificInterfaceShared.Forms
Imports WeifenLuo.WinFormsUI.Docking
Imports ZedGraph
Imports ScientificInterfaceShared.Controls

#End Region

Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmPrebal
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPrebal))
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tslbShow = New System.Windows.Forms.ToolStripLabel()
            Me.m_tsbnShowTL = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnShowName = New System.Windows.Forms.ToolStripButton()
            Me.m_graph = New ZedGraph.ZedGraphControl()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslbShow, Me.m_tsbnShowTL, Me.m_tsbnShowName})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tslbShow
            '
            Me.m_tslbShow.Name = "m_tslbShow"
            resources.ApplyResources(Me.m_tslbShow, "m_tslbShow")
            '
            'm_tsbnShowTL
            '
            Me.m_tsbnShowTL.CheckOnClick = True
            Me.m_tsbnShowTL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnShowTL, "m_tsbnShowTL")
            Me.m_tsbnShowTL.Name = "m_tsbnShowTL"
            '
            'm_tsbnShowName
            '
            Me.m_tsbnShowName.CheckOnClick = True
            Me.m_tsbnShowName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnShowName, "m_tsbnShowName")
            Me.m_tsbnShowName.Name = "m_tsbnShowName"
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
            'frmPrebal
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_graph)
            Me.Controls.Add(Me.m_ts)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Name = "frmPrebal"
            Me.ShowInTaskbar = False
            Me.TabText = ""
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Private WithEvents m_tsbnShowTL As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnShowName As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslbShow As System.Windows.Forms.ToolStripLabel
    End Class

End Namespace
