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

Partial Class frmAcknowledgements
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAcknowledgements))
        Me.m_tlpSonsors = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbIPN = New System.Windows.Forms.PictureBox()
        Me.m_pbCicimar = New System.Windows.Forms.PictureBox()
        Me.m_pbConacyt = New System.Windows.Forms.PictureBox()
        Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblAcknowledgements = New System.Windows.Forms.LinkLabel()
        Me.m_tlpSonsors.SuspendLayout()
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tlpSonsors
        '
        resources.ApplyResources(Me.m_tlpSonsors, "m_tlpSonsors")
        Me.m_tlpSonsors.BackColor = System.Drawing.Color.White
        Me.m_tlpSonsors.Controls.Add(Me.m_pbIPN, 0, 0)
        Me.m_tlpSonsors.Controls.Add(Me.m_pbCicimar, 1, 0)
        Me.m_tlpSonsors.Controls.Add(Me.m_pbConacyt, 2, 0)
        Me.m_tlpSonsors.Name = "m_tlpSonsors"
        '
        'm_pbIPN
        '
        Me.m_pbIPN.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.IPN_color
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
        'm_pbConacyt
        '
        Me.m_pbConacyt.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.CONACYT
        resources.ApplyResources(Me.m_pbConacyt, "m_pbConacyt")
        Me.m_pbConacyt.Name = "m_pbConacyt"
        Me.m_pbConacyt.TabStop = False
        '
        'm_hdrSponsors
        '
        resources.ApplyResources(Me.m_hdrSponsors, "m_hdrSponsors")
        Me.m_hdrSponsors.CanCollapseParent = False
        Me.m_hdrSponsors.CollapsedParentHeight = 0
        Me.m_hdrSponsors.IsCollapsed = False
        Me.m_hdrSponsors.Name = "m_hdrSponsors"
        '
        'm_lblAcknowledgements
        '
        Me.m_lblAcknowledgements.ActiveLinkColor = System.Drawing.SystemColors.Highlight
        resources.ApplyResources(Me.m_lblAcknowledgements, "m_lblAcknowledgements")
        Me.m_lblAcknowledgements.LinkColor = System.Drawing.SystemColors.HotTrack
        Me.m_lblAcknowledgements.Name = "m_lblAcknowledgements"
        Me.m_lblAcknowledgements.TabStop = True
        Me.m_lblAcknowledgements.UseCompatibleTextRendering = True
        '
        'frmAcknowledgements
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lblAcknowledgements)
        Me.Controls.Add(Me.m_hdrSponsors)
        Me.Controls.Add(Me.m_tlpSonsors)
        Me.Name = "frmAcknowledgements"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.m_tlpSonsors.ResumeLayout(False)
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_tlpSonsors As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbIPN As System.Windows.Forms.PictureBox
    Private WithEvents m_pbCicimar As System.Windows.Forms.PictureBox
    Private WithEvents m_pbConacyt As System.Windows.Forms.PictureBox
    Private WithEvents m_hdrSponsors As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblAcknowledgements As System.Windows.Forms.LinkLabel
End Class
