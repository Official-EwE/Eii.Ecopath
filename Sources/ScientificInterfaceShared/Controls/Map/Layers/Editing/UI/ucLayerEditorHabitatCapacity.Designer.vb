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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Namespace Controls.Map.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorHabitatCapacity
        Inherits ucLayerEditorRange

        'UserControl overrides dispose to clean up the component list.
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
            Me.m_cmbGroups = New System.Windows.Forms.ComboBox()
            Me.m_lblFleet = New System.Windows.Forms.Label()
            CType(Me.m_pbPreview, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_cmbGroups
            '
            Me.m_cmbGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroups.FormattingEnabled = True
            Me.m_cmbGroups.Location = New System.Drawing.Point(65, 96)
            Me.m_cmbGroups.MaxDropDownItems = 12
            Me.m_cmbGroups.Name = "m_cmbGroups"
            Me.m_cmbGroups.Size = New System.Drawing.Size(132, 21)
            Me.m_cmbGroups.TabIndex = 6
            '
            'm_lblFleet
            '
            Me.m_lblFleet.AutoSize = True
            Me.m_lblFleet.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblFleet.Location = New System.Drawing.Point(3, 99)
            Me.m_lblFleet.Name = "m_lblFleet"
            Me.m_lblFleet.Size = New System.Drawing.Size(39, 13)
            Me.m_lblFleet.TabIndex = 5
            Me.m_lblFleet.Text = "&Group:"
            '
            'ucLayerEditorHabitatCapacity
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbGroups)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Name = "ucLayerEditorHabitatCapacity"
            Me.Size = New System.Drawing.Size(200, 126)
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroups, 0)
            Me.Controls.SetChildIndex(Me.m_pbPreview, 0)
            CType(Me.m_pbPreview, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbGroups As System.Windows.Forms.ComboBox
        Private WithEvents m_lblFleet As System.Windows.Forms.Label

    End Class

End Namespace
