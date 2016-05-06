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

Namespace SpatialData

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAttributeNameConfigPage
        Inherits System.Windows.Forms.UserControl

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
            Me.m_lblAttribute = New System.Windows.Forms.Label()
            Me.m_cmbAttribute = New System.Windows.Forms.ComboBox()
            Me.SuspendLayout()
            '
            'm_lblAttribute
            '
            Me.m_lblAttribute.AutoSize = True
            Me.m_lblAttribute.Location = New System.Drawing.Point(3, 6)
            Me.m_lblAttribute.Name = "m_lblAttribute"
            Me.m_lblAttribute.Size = New System.Drawing.Size(49, 13)
            Me.m_lblAttribute.TabIndex = 0
            Me.m_lblAttribute.Text = "&Attribute:"
            '
            'm_cmbAttribute
            '
            Me.m_cmbAttribute.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbAttribute.FormattingEnabled = True
            Me.m_cmbAttribute.Location = New System.Drawing.Point(58, 3)
            Me.m_cmbAttribute.Name = "m_cmbAttribute"
            Me.m_cmbAttribute.Size = New System.Drawing.Size(192, 21)
            Me.m_cmbAttribute.TabIndex = 1
            '
            'ucIsobarConverterConfigPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cmbAttribute)
            Me.Controls.Add(Me.m_lblAttribute)
            Me.Name = "ucIsobarConverterConfigPage"
            Me.Size = New System.Drawing.Size(253, 28)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblAttribute As System.Windows.Forms.Label
        Friend WithEvents m_cmbAttribute As System.Windows.Forms.ComboBox

    End Class

End Namespace
