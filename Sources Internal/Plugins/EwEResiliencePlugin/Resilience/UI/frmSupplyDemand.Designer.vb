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

Partial Class frmSupplyDemand
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSupplyDemand))
        Me.m_zgc = New ZedGraph.ZedGraphControl()
        Me.m_slider = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tsbnAutosave = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveNow = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbnDynamicScales = New System.Windows.Forms.ToolStripButton()
        Me.m_cbAnnual = New System.Windows.Forms.CheckBox()
        Me.m_nudTime = New System.Windows.Forms.NumericUpDown()
        Me.m_lblTime = New System.Windows.Forms.Label()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_nudTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        'm_slider
        '
        resources.ApplyResources(Me.m_slider, "m_slider")
        Me.m_slider.CurrentKnob = 0
        Me.m_slider.Maximum = 100
        Me.m_slider.Minimum = 0
        Me.m_slider.Name = "m_slider"
        Me.m_slider.NumKnobs = 1
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnAutosave, Me.m_tsbnSaveNow, Me.ToolStripSeparator1, Me.m_tsbnDynamicScales})
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
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'm_tsbnDynamicScales
        '
        Me.m_tsbnDynamicScales.CheckOnClick = True
        resources.ApplyResources(Me.m_tsbnDynamicScales, "m_tsbnDynamicScales")
        Me.m_tsbnDynamicScales.Name = "m_tsbnDynamicScales"
        '
        'm_cbAnnual
        '
        resources.ApplyResources(Me.m_cbAnnual, "m_cbAnnual")
        Me.m_cbAnnual.Name = "m_cbAnnual"
        Me.m_cbAnnual.UseVisualStyleBackColor = True
        '
        'm_nudTime
        '
        resources.ApplyResources(Me.m_nudTime, "m_nudTime")
        Me.m_nudTime.Name = "m_nudTime"
        '
        'm_lblTime
        '
        resources.ApplyResources(Me.m_lblTime, "m_lblTime")
        Me.m_lblTime.Name = "m_lblTime"
        '
        'frmSupplyDemand
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lblTime)
        Me.Controls.Add(Me.m_nudTime)
        Me.Controls.Add(Me.m_cbAnnual)
        Me.Controls.Add(Me.m_tsMain)
        Me.Controls.Add(Me.m_slider)
        Me.Controls.Add(Me.m_zgc)
        Me.Name = "frmSupplyDemand"
        Me.TabText = ""
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        CType(Me.m_nudTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_slider As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_tsMain As ScientificInterfaceShared.Controls.cEwEToolstrip
    Private WithEvents m_tsbnAutosave As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnSaveNow As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnDynamicScales As System.Windows.Forms.ToolStripButton
    Private WithEvents m_nudTime As System.Windows.Forms.NumericUpDown
    Private WithEvents m_lblTime As System.Windows.Forms.Label
    Private WithEvents m_cbAnnual As System.Windows.Forms.CheckBox
End Class
