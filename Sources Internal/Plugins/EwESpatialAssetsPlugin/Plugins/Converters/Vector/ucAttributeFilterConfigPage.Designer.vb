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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace SpatialData

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAttributeFilterConfigPage
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
            Me.m_lblFilter = New System.Windows.Forms.Label()
            Me.m_tbxValue = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'm_lblFilter
            '
            Me.m_lblFilter.AutoSize = True
            Me.m_lblFilter.Location = New System.Drawing.Point(3, 6)
            Me.m_lblFilter.Name = "m_lblFilter"
            Me.m_lblFilter.Size = New System.Drawing.Size(32, 13)
            Me.m_lblFilter.TabIndex = 0
            Me.m_lblFilter.Text = "&Filter:"
            '
            'm_tbxValue
            '
            Me.m_tbxValue.Location = New System.Drawing.Point(58, 3)
            Me.m_tbxValue.Name = "m_tbxValue"
            Me.m_tbxValue.Size = New System.Drawing.Size(309, 20)
            Me.m_tbxValue.TabIndex = 2
            '
            'ucAttributeFilterConfigPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tbxValue)
            Me.Controls.Add(Me.m_lblFilter)
            Me.Name = "ucAttributeFilterConfigPage"
            Me.Size = New System.Drawing.Size(370, 29)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblFilter As System.Windows.Forms.Label
        Private WithEvents m_tbxValue As System.Windows.Forms.TextBox

    End Class

End Namespace
