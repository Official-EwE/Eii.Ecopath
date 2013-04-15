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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ZedGraphDrawer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.SuspendLayout()
        Me.Dock = DockStyle.Fill
        '
        'm_zgc
        '
        Me.m_zgc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_zgc.Location = New System.Drawing.Point(0, 0)
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        Me.m_zgc.Size = New System.Drawing.Size(1116, 698)
        Me.m_zgc.TabIndex = 1
        '
        'ZedGraphDrawer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_zgc)
        Me.Name = "ZedGraphDrawer"
        Me.Size = New System.Drawing.Size(1116, 698)
        Me.ResumeLayout(False)

    End Sub

    Public WithEvents m_zgc As ZedGraph.ZedGraphControl

End Class
