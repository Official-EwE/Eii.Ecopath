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
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        Me.m_zgc = New ZedGraph.ZedGraphControl()
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plGraph = New System.Windows.Forms.Panel()
        Me.m_slider = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_cbAnnual = New System.Windows.Forms.CheckBox()
        Me.m_plSponsors = New System.Windows.Forms.Panel()
        Me.m_tlpSonsors = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbIPN = New System.Windows.Forms.PictureBox()
        Me.m_pbCicimar = New System.Windows.Forms.PictureBox()
        Me.m_pbAuci = New System.Windows.Forms.PictureBox()
        Me.m_pbConacyt = New System.Windows.Forms.PictureBox()
        Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tlpContent.SuspendLayout()
        Me.m_plGraph.SuspendLayout()
        Me.m_plSponsors.SuspendLayout()
        Me.m_tlpSonsors.SuspendLayout()
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbAuci, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'm_tlpContent
        '
        resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
        Me.m_tlpContent.Controls.Add(Me.m_plGraph, 0, 0)
        Me.m_tlpContent.Controls.Add(Me.m_plSponsors, 0, 1)
        Me.m_tlpContent.Name = "m_tlpContent"
        '
        'm_plGraph
        '
        Me.m_plGraph.Controls.Add(Me.m_slider)
        Me.m_plGraph.Controls.Add(Me.m_zgc)
        Me.m_plGraph.Controls.Add(Me.m_cbAnnual)
        Me.m_plGraph.Controls.Add(Me.m_cbAutosave)
        resources.ApplyResources(Me.m_plGraph, "m_plGraph")
        Me.m_plGraph.Name = "m_plGraph"
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
        'm_plSponsors
        '
        Me.m_plSponsors.Controls.Add(Me.m_tlpSonsors)
        Me.m_plSponsors.Controls.Add(Me.m_hdrSponsors)
        resources.ApplyResources(Me.m_plSponsors, "m_plSponsors")
        Me.m_plSponsors.Name = "m_plSponsors"
        '
        'm_tlpSonsors
        '
        resources.ApplyResources(Me.m_tlpSonsors, "m_tlpSonsors")
        Me.m_tlpSonsors.BackColor = System.Drawing.Color.White
        Me.m_tlpSonsors.Controls.Add(Me.m_pbIPN, 0, 0)
        Me.m_tlpSonsors.Controls.Add(Me.m_pbCicimar, 1, 0)
        Me.m_tlpSonsors.Controls.Add(Me.m_pbAuci, 2, 0)
        Me.m_tlpSonsors.Controls.Add(Me.m_pbConacyt, 3, 0)
        Me.m_tlpSonsors.Name = "m_tlpSonsors"
        '
        'm_pbIPN
        '
        Me.m_pbIPN.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.EscudoIPN1
        resources.ApplyResources(Me.m_pbIPN, "m_pbIPN")
        Me.m_pbIPN.Name = "m_pbIPN"
        Me.m_pbIPN.TabStop = False
        '
        'm_pbCicimar
        '
        Me.m_pbCicimar.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.cicimar_color
        resources.ApplyResources(Me.m_pbCicimar, "m_pbCicimar")
        Me.m_pbCicimar.Name = "m_pbCicimar"
        Me.m_pbCicimar.TabStop = False
        '
        'm_pbAuci
        '
        Me.m_pbAuci.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.AUCI
        resources.ApplyResources(Me.m_pbAuci, "m_pbAuci")
        Me.m_pbAuci.Name = "m_pbAuci"
        Me.m_pbAuci.TabStop = False
        '
        'm_pbConacyt
        '
        Me.m_pbConacyt.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.CONACYT
        resources.ApplyResources(Me.m_pbConacyt, "m_pbConacyt")
        Me.m_pbConacyt.Name = "m_pbConacyt"
        Me.m_pbConacyt.TabStop = False
        '
        'm_hdrSponsors
        '
        Me.m_hdrSponsors.CanCollapseParent = True
        Me.m_hdrSponsors.CollapsedParentHeight = 18
        resources.ApplyResources(Me.m_hdrSponsors, "m_hdrSponsors")
        Me.m_hdrSponsors.IsCollapsed = False
        Me.m_hdrSponsors.Name = "m_hdrSponsors"
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
        Me.m_plSponsors.ResumeLayout(False)
        Me.m_tlpSonsors.ResumeLayout(False)
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbAuci, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_cbAutosave As System.Windows.Forms.CheckBox
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_plGraph As System.Windows.Forms.Panel
    Private WithEvents m_plSponsors As System.Windows.Forms.Panel
    Private WithEvents m_tlpSonsors As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbIPN As System.Windows.Forms.PictureBox
    Private WithEvents m_pbCicimar As System.Windows.Forms.PictureBox
    Private WithEvents m_pbAuci As System.Windows.Forms.PictureBox
    Private WithEvents m_pbConacyt As System.Windows.Forms.PictureBox
    Private WithEvents m_hdrSponsors As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_slider As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_cbAnnual As System.Windows.Forms.CheckBox
End Class
