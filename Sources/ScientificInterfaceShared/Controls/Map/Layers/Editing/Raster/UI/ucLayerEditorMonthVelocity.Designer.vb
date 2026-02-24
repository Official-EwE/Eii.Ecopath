' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorMonthVelocity
        Inherits ucLayerEditorRange

        'UserControl overrides dispose to clean up the component list.
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
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorMonthVelocity))
            Me.m_btnCopy = New System.Windows.Forms.Button()
            Me.m_cmbMonth = New System.Windows.Forms.ComboBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'm_btnCopy
            '
            resources.ApplyResources(Me.m_btnCopy, "m_btnCopy")
            Me.m_btnCopy.Image = Global.ScientificInterfaceShared.My.Resources.Resources.CopyHS
            Me.m_btnCopy.Name = "m_btnCopy"
            Me.m_btnCopy.UseVisualStyleBackColor = True
            '
            'm_cmbMonth
            '
            resources.ApplyResources(Me.m_cmbMonth, "m_cmbMonth")
            Me.m_cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbMonth.FormattingEnabled = True
            Me.m_cmbMonth.Name = "m_cmbMonth"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'ucLayerEditorMonthVelocity
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_cmbMonth)
            Me.Controls.Add(Me.m_btnCopy)
            Me.Name = "ucLayerEditorMonthVelocity"
            Me.Controls.SetChildIndex(Me.m_btnCopy, 0)
            Me.Controls.SetChildIndex(Me.m_cmbMonth, 0)
            Me.Controls.SetChildIndex(Me.Label1, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbMonth As ComboBox
        Friend WithEvents Label1 As Label
        Private WithEvents m_btnCopy As Button
    End Class

End Namespace

