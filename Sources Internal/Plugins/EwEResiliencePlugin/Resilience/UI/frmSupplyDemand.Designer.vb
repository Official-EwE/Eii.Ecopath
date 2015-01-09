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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSupplyDemand))
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        Me.m_zgc = New ZedGraph.ZedGraphControl()
        Me.m_slider = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_cbAnnual = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'm_cbAutosave
        '
        resources.ApplyResources(Me.m_cbAutosave, "m_cbAutosave")
        Me.m_cbAutosave.Name = "m_cbAutosave"
        Me.m_cbAutosave.UseVisualStyleBackColor = True
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
        'm_cbAnnual
        '
        resources.ApplyResources(Me.m_cbAnnual, "m_cbAnnual")
        Me.m_cbAnnual.Name = "m_cbAnnual"
        Me.m_cbAnnual.UseVisualStyleBackColor = True
        '
        'frmSupplyDemand
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_slider)
        Me.Controls.Add(Me.m_cbAnnual)
        Me.Controls.Add(Me.m_zgc)
        Me.Controls.Add(Me.m_cbAutosave)
        Me.Name = "frmSupplyDemand"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_cbAutosave As System.Windows.Forms.CheckBox
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_slider As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_cbAnnual As System.Windows.Forms.CheckBox
End Class
