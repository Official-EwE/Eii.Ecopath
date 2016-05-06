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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Controls.Map.Layers

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
            Me.m_btnAllDefault = New System.Windows.Forms.Button()
            Me.m_btnLayerDefault = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_hdDefaults = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.SuspendLayout()
            '
            'm_cmbGroups
            '
            Me.m_cmbGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGroups.FormattingEnabled = True
            Me.m_cmbGroups.Location = New System.Drawing.Point(68, 180)
            Me.m_cmbGroups.MaxDropDownItems = 12
            Me.m_cmbGroups.Name = "m_cmbGroups"
            Me.m_cmbGroups.Size = New System.Drawing.Size(127, 21)
            Me.m_cmbGroups.TabIndex = 8
            '
            'm_lblFleet
            '
            Me.m_lblFleet.AutoSize = True
            Me.m_lblFleet.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblFleet.Location = New System.Drawing.Point(3, 183)
            Me.m_lblFleet.Name = "m_lblFleet"
            Me.m_lblFleet.Size = New System.Drawing.Size(39, 13)
            Me.m_lblFleet.TabIndex = 7
            Me.m_lblFleet.Text = "&Group:"
            '
            'm_btnAllDefault
            '
            Me.m_btnAllDefault.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnAllDefault.Location = New System.Drawing.Point(68, 251)
            Me.m_btnAllDefault.Name = "m_btnAllDefault"
            Me.m_btnAllDefault.Size = New System.Drawing.Size(127, 23)
            Me.m_btnAllDefault.TabIndex = 12
            Me.m_btnAllDefault.Text = "Reset &all layers"
            Me.m_btnAllDefault.UseVisualStyleBackColor = True
            '
            'm_btnLayerDefault
            '
            Me.m_btnLayerDefault.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnLayerDefault.Location = New System.Drawing.Point(68, 222)
            Me.m_btnLayerDefault.Name = "m_btnLayerDefault"
            Me.m_btnLayerDefault.Size = New System.Drawing.Size(127, 23)
            Me.m_btnLayerDefault.TabIndex = 11
            Me.m_btnLayerDefault.Text = "Reset &this layer"
            Me.m_btnLayerDefault.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 138)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(0, 13)
            Me.Label1.TabIndex = 10
            '
            'm_hdDefaults
            '
            Me.m_hdDefaults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdDefaults.CanCollapseParent = False
            Me.m_hdDefaults.CollapsedParentHeight = 0
            Me.m_hdDefaults.IsCollapsed = False
            Me.m_hdDefaults.Location = New System.Drawing.Point(3, 204)
            Me.m_hdDefaults.Name = "m_hdDefaults"
            Me.m_hdDefaults.Size = New System.Drawing.Size(195, 18)
            Me.m_hdDefaults.TabIndex = 9
            Me.m_hdDefaults.Text = "Defaults"
            Me.m_hdDefaults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ucLayerEditorHabitatCapacity
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_hdDefaults)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_btnLayerDefault)
            Me.Controls.Add(Me.m_cmbGroups)
            Me.Controls.Add(Me.m_lblFleet)
            Me.Controls.Add(Me.m_btnAllDefault)
            Me.Name = "ucLayerEditorHabitatCapacity"
            Me.Size = New System.Drawing.Size(203, 282)
            Me.Controls.SetChildIndex(Me.m_btnAllDefault, 0)
            Me.Controls.SetChildIndex(Me.m_lblFleet, 0)
            Me.Controls.SetChildIndex(Me.m_cmbGroups, 0)
            Me.Controls.SetChildIndex(Me.m_btnLayerDefault, 0)
            Me.Controls.SetChildIndex(Me.Label1, 0)
            Me.Controls.SetChildIndex(Me.m_hdDefaults, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbGroups As System.Windows.Forms.ComboBox
        Private WithEvents m_lblFleet As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_hdDefaults As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnAllDefault As System.Windows.Forms.Button
        Private WithEvents m_btnLayerDefault As System.Windows.Forms.Button

    End Class

End Namespace
