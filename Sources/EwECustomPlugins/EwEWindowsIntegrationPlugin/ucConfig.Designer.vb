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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucConfig
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucConfig))
        Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_cbKeepOSAwake = New System.Windows.Forms.CheckBox()
        Me.m_lblPrompt = New System.Windows.Forms.Label()
        Me.m_cbKeepMonitorOn = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'm_hdr
        '
        Me.m_hdr.CanCollapseParent = False
        Me.m_hdr.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdr, "m_hdr")
        Me.m_hdr.IsCollapsed = False
        Me.m_hdr.Name = "m_hdr"
        '
        'm_cbKeepOSAwake
        '
        resources.ApplyResources(Me.m_cbKeepOSAwake, "m_cbKeepOSAwake")
        Me.m_cbKeepOSAwake.Name = "m_cbKeepOSAwake"
        Me.m_cbKeepOSAwake.UseVisualStyleBackColor = True
        '
        'm_lblPrompt
        '
        resources.ApplyResources(Me.m_lblPrompt, "m_lblPrompt")
        Me.m_lblPrompt.Name = "m_lblPrompt"
        '
        'm_cbKeepMonitorOn
        '
        resources.ApplyResources(Me.m_cbKeepMonitorOn, "m_cbKeepMonitorOn")
        Me.m_cbKeepMonitorOn.Name = "m_cbKeepMonitorOn"
        Me.m_cbKeepMonitorOn.UseVisualStyleBackColor = True
        '
        'ucConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblPrompt)
        Me.Controls.Add(Me.m_cbKeepMonitorOn)
        Me.Controls.Add(Me.m_cbKeepOSAwake)
        Me.Controls.Add(Me.m_hdr)
        Me.Name = "ucConfig"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_cbKeepOSAwake As Windows.Forms.CheckBox
    Private WithEvents m_hdr As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblPrompt As Windows.Forms.Label
    Private WithEvents m_cbKeepMonitorOn As Windows.Forms.CheckBox
End Class
