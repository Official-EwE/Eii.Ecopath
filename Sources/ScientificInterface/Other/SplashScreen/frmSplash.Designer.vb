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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Partial Class frmSplash
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplash))
        Me.m_pbIcon = New System.Windows.Forms.PictureBox()
        Me.m_lblEwE = New System.Windows.Forms.Label()
        Me.m_lblReleaseMode = New System.Windows.Forms.Label()
        Me.m_tlpSplash = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblText = New System.Windows.Forms.Label()
        CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpSplash.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_pbIcon
        '
        resources.ApplyResources(Me.m_pbIcon, "m_pbIcon")
        Me.m_pbIcon.BackColor = System.Drawing.Color.Transparent
        Me.m_pbIcon.Name = "m_pbIcon"
        Me.m_pbIcon.TabStop = False
        '
        'm_lblEwE
        '
        resources.ApplyResources(Me.m_lblEwE, "m_lblEwE")
        Me.m_lblEwE.BackColor = System.Drawing.Color.Transparent
        Me.m_lblEwE.Name = "m_lblEwE"
        '
        'm_lblReleaseMode
        '
        resources.ApplyResources(Me.m_lblReleaseMode, "m_lblReleaseMode")
        Me.m_lblReleaseMode.BackColor = System.Drawing.Color.Transparent
        Me.m_lblReleaseMode.Name = "m_lblReleaseMode"
        '
        'm_tlpSplash
        '
        Me.m_tlpSplash.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.m_tlpSplash, "m_tlpSplash")
        Me.m_tlpSplash.Controls.Add(Me.m_pbIcon, 0, 1)
        Me.m_tlpSplash.Controls.Add(Me.m_lblReleaseMode, 0, 3)
        Me.m_tlpSplash.Controls.Add(Me.m_lblEwE, 0, 2)
        Me.m_tlpSplash.Controls.Add(Me.m_lblText, 0, 4)
        Me.m_tlpSplash.Name = "m_tlpSplash"
        '
        'm_lblText
        '
        resources.ApplyResources(Me.m_lblText, "m_lblText")
        Me.m_lblText.Name = "m_lblText"
        '
        'frmSplash
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tlpSplash)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmSplash"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.TransparencyKey = System.Drawing.Color.Transparent
        CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpSplash.ResumeLayout(False)
        Me.m_tlpSplash.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_pbIcon As PictureBox
    Private WithEvents m_lblEwE As Label
    Private WithEvents m_lblReleaseMode As Label
    Private WithEvents m_tlpSplash As TableLayoutPanel
    Private WithEvents m_lblText As Label
End Class
