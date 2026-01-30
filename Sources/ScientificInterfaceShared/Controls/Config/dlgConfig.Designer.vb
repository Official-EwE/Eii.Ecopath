' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    Partial Class dlgConfig
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgConfig))
            Me.m_plContent = New System.Windows.Forms.Panel()
            Me.m_btnOK = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.m_btnDefaults = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_plContent
            '
            resources.ApplyResources(Me.m_plContent, "m_plContent")
            Me.m_plContent.Name = "m_plContent"
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
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_btnDefaults
            '
            resources.ApplyResources(Me.m_btnDefaults, "m_btnDefaults")
            Me.m_btnDefaults.Name = "m_btnDefaults"
            Me.m_btnDefaults.UseVisualStyleBackColor = True
            '
            'dlgConfig
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnDefaults)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_plContent)
            Me.Name = "dlgConfig"
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_plContent As System.Windows.Forms.Panel
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_btnDefaults As System.Windows.Forms.Button
    End Class

End Namespace
