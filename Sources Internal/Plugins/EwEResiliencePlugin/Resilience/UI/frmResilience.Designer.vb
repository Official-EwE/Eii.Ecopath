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
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plGraph = New System.Windows.Forms.Panel()
        Me.m_zgc = New ZedGraph.ZedGraphControl()
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tsbnAutosave = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveNow = New System.Windows.Forms.ToolStripButton()
        Me.m_tlpContent.SuspendLayout()
        Me.m_plGraph.SuspendLayout()
        Me.m_tsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tlpContent
        '
        resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
        Me.m_tlpContent.Controls.Add(Me.m_plGraph, 0, 0)
        Me.m_tlpContent.Name = "m_tlpContent"
        '
        'm_plGraph
        '
        Me.m_plGraph.Controls.Add(Me.m_zgc)
        Me.m_plGraph.Controls.Add(Me.m_tsMain)
        resources.ApplyResources(Me.m_plGraph, "m_plGraph")
        Me.m_plGraph.Name = "m_plGraph"
        '
        'm_zgc
        '
        resources.ApplyResources(Me.m_zgc, "m_zgc")
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0.0R
        Me.m_zgc.ScrollMaxX = 0.0R
        Me.m_zgc.ScrollMaxY = 0.0R
        Me.m_zgc.ScrollMaxY2 = 0.0R
        Me.m_zgc.ScrollMinX = 0.0R
        Me.m_zgc.ScrollMinY = 0.0R
        Me.m_zgc.ScrollMinY2 = 0.0R
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnAutosave, Me.m_tsbnSaveNow})
        resources.ApplyResources(Me.m_tsMain, "m_tsMain")
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'm_tsbnAutosave
        '
        Me.m_tsbnAutosave.CheckOnClick = True
        resources.ApplyResources(Me.m_tsbnAutosave, "m_tsbnAutosave")
        Me.m_tsbnAutosave.Name = "m_tsbnAutosave"
        '
        'm_tsbnSaveNow
        '
        resources.ApplyResources(Me.m_tsbnSaveNow, "m_tsbnSaveNow")
        Me.m_tsbnSaveNow.Name = "m_tsbnSaveNow"
        '
        'frmResilience
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tlpContent)
        Me.Name = "frmResilience"
        Me.m_tlpContent.ResumeLayout(False)
        Me.m_plGraph.ResumeLayout(False)
        Me.m_plGraph.PerformLayout()
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_plGraph As System.Windows.Forms.Panel
    Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_tsMain As cEwEToolstrip
    Private WithEvents m_tsbnAutosave As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnSaveNow As System.Windows.Forms.ToolStripButton
End Class
