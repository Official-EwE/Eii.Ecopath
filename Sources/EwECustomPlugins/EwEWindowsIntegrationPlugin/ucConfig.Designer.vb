' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
        Me.m_cbNoRestart = New System.Windows.Forms.CheckBox()
        Me.m_cbEnabled = New System.Windows.Forms.CheckBox()
        Me.m_lblError = New System.Windows.Forms.Label()
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
        'm_cbNoRestart
        '
        resources.ApplyResources(Me.m_cbNoRestart, "m_cbNoRestart")
        Me.m_cbNoRestart.Name = "m_cbNoRestart"
        Me.m_cbNoRestart.UseVisualStyleBackColor = True
        '
        'm_cbEnabled
        '
        resources.ApplyResources(Me.m_cbEnabled, "m_cbEnabled")
        Me.m_cbEnabled.Name = "m_cbEnabled"
        Me.m_cbEnabled.UseVisualStyleBackColor = True
        '
        'm_lblError
        '
        resources.ApplyResources(Me.m_lblError, "m_lblError")
        Me.m_lblError.Name = "m_lblError"
        '
        'ucConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblError)
        Me.Controls.Add(Me.m_cbEnabled)
        Me.Controls.Add(Me.m_cbNoRestart)
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
    Private WithEvents m_cbNoRestart As Windows.Forms.CheckBox
    Private WithEvents m_cbEnabled As Windows.Forms.CheckBox
    Private WithEvents m_lblError As Windows.Forms.Label
End Class
