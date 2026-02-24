' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorDefault
        Inherits ucLayerEditor

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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorDefault))
            Me.m_ucSlider = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_lblCursor = New System.Windows.Forms.Label()
            Me.m_lblName = New System.Windows.Forms.Label()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            Me.m_lblUnits = New System.Windows.Forms.Label()
            Me.m_tbxUnits = New System.Windows.Forms.TextBox()
            Me.m_tbxMax = New System.Windows.Forms.TextBox()
            Me.m_lblMax = New System.Windows.Forms.Label()
            Me.m_plLegend = New System.Windows.Forms.Panel()
            Me.m_lblMin = New System.Windows.Forms.Label()
            Me.m_tbxMin = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'm_ucSlider
            '
            resources.ApplyResources(Me.m_ucSlider, "m_ucSlider")
            Me.m_ucSlider.CurrentKnob = 0
            Me.m_ucSlider.Maximum = 6
            Me.m_ucSlider.Minimum = 1
            Me.m_ucSlider.Name = "m_ucSlider"
            Me.m_ucSlider.NumKnobs = 1
            '
            'm_lblCursor
            '
            resources.ApplyResources(Me.m_lblCursor, "m_lblCursor")
            Me.m_lblCursor.Name = "m_lblCursor"
            '
            'm_lblName
            '
            resources.ApplyResources(Me.m_lblName, "m_lblName")
            Me.m_lblName.Name = "m_lblName"
            '
            'm_tbxName
            '
            resources.ApplyResources(Me.m_tbxName, "m_tbxName")
            Me.m_tbxName.Name = "m_tbxName"
            '
            'm_lblUnits
            '
            resources.ApplyResources(Me.m_lblUnits, "m_lblUnits")
            Me.m_lblUnits.Name = "m_lblUnits"
            '
            'm_tbxUnits
            '
            resources.ApplyResources(Me.m_tbxUnits, "m_tbxUnits")
            Me.m_tbxUnits.Name = "m_tbxUnits"
            '
            'm_tbxMax
            '
            resources.ApplyResources(Me.m_tbxMax, "m_tbxMax")
            Me.m_tbxMax.Name = "m_tbxMax"
            Me.m_tbxMax.ReadOnly = True
            '
            'm_lblMax
            '
            resources.ApplyResources(Me.m_lblMax, "m_lblMax")
            Me.m_lblMax.Name = "m_lblMax"
            '
            'm_plLegend
            '
            resources.ApplyResources(Me.m_plLegend, "m_plLegend")
            Me.m_plLegend.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plLegend.Name = "m_plLegend"
            '
            'm_lblMin
            '
            resources.ApplyResources(Me.m_lblMin, "m_lblMin")
            Me.m_lblMin.Name = "m_lblMin"
            '
            'm_tbxMin
            '
            resources.ApplyResources(Me.m_tbxMin, "m_tbxMin")
            Me.m_tbxMin.Name = "m_tbxMin"
            Me.m_tbxMin.ReadOnly = True
            '
            'ucLayerEditorDefault
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_plLegend)
            Me.Controls.Add(Me.m_tbxMax)
            Me.Controls.Add(Me.m_tbxMin)
            Me.Controls.Add(Me.m_tbxUnits)
            Me.Controls.Add(Me.m_tbxName)
            Me.Controls.Add(Me.m_lblMin)
            Me.Controls.Add(Me.m_lblMax)
            Me.Controls.Add(Me.m_lblUnits)
            Me.Controls.Add(Me.m_lblName)
            Me.Controls.Add(Me.m_lblCursor)
            Me.Controls.Add(Me.m_ucSlider)
            Me.Name = "ucLayerEditorDefault"
            Me.Controls.SetChildIndex(Me.m_ucSlider, 0)
            Me.Controls.SetChildIndex(Me.m_lblCursor, 0)
            Me.Controls.SetChildIndex(Me.m_lblName, 0)
            Me.Controls.SetChildIndex(Me.m_lblUnits, 0)
            Me.Controls.SetChildIndex(Me.m_lblMax, 0)
            Me.Controls.SetChildIndex(Me.m_lblMin, 0)
            Me.Controls.SetChildIndex(Me.m_tbxName, 0)
            Me.Controls.SetChildIndex(Me.m_tbxUnits, 0)
            Me.Controls.SetChildIndex(Me.m_tbxMin, 0)
            Me.Controls.SetChildIndex(Me.m_tbxMax, 0)
            Me.Controls.SetChildIndex(Me.m_plLegend, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_ucSlider As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_lblCursor As System.Windows.Forms.Label
        Private WithEvents m_lblName As System.Windows.Forms.Label
        Private WithEvents m_tbxName As System.Windows.Forms.TextBox
        Private WithEvents m_lblUnits As System.Windows.Forms.Label
        Private WithEvents m_tbxUnits As System.Windows.Forms.TextBox
        Private WithEvents m_tbxMax As System.Windows.Forms.TextBox
        Private WithEvents m_lblMax As System.Windows.Forms.Label
        Private WithEvents m_plLegend As System.Windows.Forms.Panel
        Private WithEvents m_lblMin As Label
        Private WithEvents m_tbxMin As TextBox
    End Class

End Namespace