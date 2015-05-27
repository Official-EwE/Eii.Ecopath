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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfig
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Me.m_cbIncludeDetritus = New System.Windows.Forms.CheckBox()
        Me.m_cbIncludeImportAndSum = New System.Windows.Forms.CheckBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        Me.m_pbIPN = New System.Windows.Forms.PictureBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbCicimar = New System.Windows.Forms.PictureBox()
        Me.m_pbConacyt = New System.Windows.Forms.PictureBox()
        Me.m_hdrSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_cbIncludeDetritus
        '
        resources.ApplyResources(Me.m_cbIncludeDetritus, "m_cbIncludeDetritus")
        Me.m_cbIncludeDetritus.Name = "m_cbIncludeDetritus"
        Me.m_cbIncludeDetritus.UseVisualStyleBackColor = True
        '
        'm_cbIncludeImportAndSum
        '
        resources.ApplyResources(Me.m_cbIncludeImportAndSum, "m_cbIncludeImportAndSum")
        Me.m_cbIncludeImportAndSum.Name = "m_cbIncludeImportAndSum"
        Me.m_cbIncludeImportAndSum.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_cbAutosave
        '
        resources.ApplyResources(Me.m_cbAutosave, "m_cbAutosave")
        Me.m_cbAutosave.Name = "m_cbAutosave"
        Me.m_cbAutosave.UseVisualStyleBackColor = True
        '
        'm_pbIPN
        '
        Me.m_pbIPN.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.IPN_color
        resources.ApplyResources(Me.m_pbIPN, "m_pbIPN")
        Me.m_pbIPN.Name = "m_pbIPN"
        Me.m_pbIPN.TabStop = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.White
        Me.TableLayoutPanel1.Controls.Add(Me.m_pbIPN, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_pbCicimar, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_pbConacyt, 2, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
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
        'frmConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrSponsors)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_cbIncludeImportAndSum)
        Me.Controls.Add(Me.m_cbAutosave)
        Me.Controls.Add(Me.m_cbIncludeDetritus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "frmConfig"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_cbIncludeImportAndSum As System.Windows.Forms.CheckBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_cbIncludeDetritus As System.Windows.Forms.CheckBox
    Private WithEvents m_cbAutosave As System.Windows.Forms.CheckBox
    Private WithEvents m_pbIPN As System.Windows.Forms.PictureBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbCicimar As System.Windows.Forms.PictureBox
    Private WithEvents m_pbConacyt As System.Windows.Forms.PictureBox
    Private WithEvents m_hdrSponsors As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
