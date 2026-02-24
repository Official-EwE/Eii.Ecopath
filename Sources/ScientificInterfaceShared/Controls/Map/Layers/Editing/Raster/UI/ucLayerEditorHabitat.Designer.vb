' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style
Imports EwECore.Common

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorHabitat
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorHabitat))
            Me.m_cbUseHabitatAreaCorrection = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            '
            'm_cbUseHabitatAreaCorrection
            '
            resources.ApplyResources(Me.m_cbUseHabitatAreaCorrection, "m_cbUseHabitatAreaCorrection")
            Me.m_cbUseHabitatAreaCorrection.Name = "m_cbUseHabitatAreaCorrection"
            Me.m_cbUseHabitatAreaCorrection.UseVisualStyleBackColor = True
            '
            'ucLayerEditorHabitat
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_cbUseHabitatAreaCorrection)
            Me.Name = "ucLayerEditorHabitat"
            Me.Controls.SetChildIndex(Me.m_cbUseHabitatAreaCorrection, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_cbUseHabitatAreaCorrection As CheckBox
    End Class

End Namespace
