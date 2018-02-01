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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucNetworkD3Options
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
        Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'm_hdr
        '
        Me.m_hdr.CanCollapseParent = False
        Me.m_hdr.CollapsedParentHeight = 0
        Me.m_hdr.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdr.IsCollapsed = False
        Me.m_hdr.Location = New System.Drawing.Point(0, 0)
        Me.m_hdr.Name = "m_hdr"
        Me.m_hdr.Size = New System.Drawing.Size(243, 18)
        Me.m_hdr.TabIndex = 0
        Me.m_hdr.Text = "NetworkD3 options"
        Me.m_hdr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ucNetworkD3Options
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_hdr)
        Me.Name = "ucNetworkD3Options"
        Me.Size = New System.Drawing.Size(243, 210)
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_hdr As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
