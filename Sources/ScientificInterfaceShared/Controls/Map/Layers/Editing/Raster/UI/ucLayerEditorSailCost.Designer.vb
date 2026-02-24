' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorSailCost
        Inherits ucLayerEditorRange

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorSailCost))
            Me.m_cmbFleet = New System.Windows.Forms.ComboBox()
            Me.m_lblFleet = New System.Windows.Forms.Label()
            Me.m_btnCalculate = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_cmbFleet
            '
            resources.ApplyResources(Me.m_cmbFleet, "m_cmbFleet")
            Me.m_cmbFleet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbFleet.FormattingEnabled = True
            Me.m_cmbFleet.Name = "m_cmbFleet"
            '
            'm_lblFleet
            '
            resources.ApplyResources(Me.m_lblFleet, "m_lblFleet")
            Me.m_lblFleet.Name = "m_lblFleet"
            '
            'm_btnCalculate
            '
            resources.ApplyResources(Me.m_btnCalculate, "m_btnCalculate")
            Me.m_btnCalculate.Name = "m_btnCalculate"
            Me.m_btnCalculate.UseVisualStyleBackColor = True
            '
            'ucLayerEditorSailCost
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_btnCalculate)
            Me.Controls.Add(Me.m_cmbFleet)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Name = "ucLayerEditorSailCost"
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbFleet, 0)
            Me.Controls.SetChildIndex(Me.m_btnCalculate, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbFleet As System.Windows.Forms.ComboBox
        Private WithEvents m_lblFleet As System.Windows.Forms.Label
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button

    End Class

End Namespace
