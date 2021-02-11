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

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorHabitatCapacityComputed
        Inherits ucLayerEditorDefault

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
            Me.m_cmbGroups = New System.Windows.Forms.ComboBox()
            Me.m_lblFleet = New System.Windows.Forms.Label()
            Me.m_btnComputeCap = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'm_cmbGroups
            '
            Me.m_cmbGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroups.FormattingEnabled = True
            Me.m_cmbGroups.Location = New System.Drawing.Point(68, 151)
            Me.m_cmbGroups.MaxDropDownItems = 12
            Me.m_cmbGroups.Name = "m_cmbGroups"
            Me.m_cmbGroups.Size = New System.Drawing.Size(127, 21)
            Me.m_cmbGroups.TabIndex = 8
            '
            'm_lblFleet
            '
            Me.m_lblFleet.AutoSize = True
            Me.m_lblFleet.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblFleet.Location = New System.Drawing.Point(3, 154)
            Me.m_lblFleet.Name = "m_lblFleet"
            Me.m_lblFleet.Size = New System.Drawing.Size(39, 13)
            Me.m_lblFleet.TabIndex = 7
            Me.m_lblFleet.Text = "&Group:"
            '
            'm_btnComputeCap
            '
            Me.m_btnComputeCap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnComputeCap.Location = New System.Drawing.Point(68, 178)
            Me.m_btnComputeCap.Name = "m_btnComputeCap"
            Me.m_btnComputeCap.Size = New System.Drawing.Size(127, 23)
            Me.m_btnComputeCap.TabIndex = 11
            Me.m_btnComputeCap.Text = "&Recompute"
            Me.m_btnComputeCap.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 138)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(0, 13)
            Me.Label1.TabIndex = 10
            '
            'ucLayerEditorHabitatCapacityComputed
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_btnComputeCap)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_cmbGroups)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Name = "ucLayerEditorHabitatCapacityComputed"
            Me.Size = New System.Drawing.Size(203, 209)
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroups, 0)
            Me.Controls.SetChildIndex(Me.Label1, 0)
            Me.Controls.SetChildIndex(Me.m_btnComputeCap, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbGroups As System.Windows.Forms.ComboBox
        Private WithEvents m_lblFleet As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_btnComputeCap As System.Windows.Forms.Button
    End Class

End Namespace
