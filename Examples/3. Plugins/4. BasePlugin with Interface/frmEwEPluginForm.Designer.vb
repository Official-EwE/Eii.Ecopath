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

Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEwEPlugin
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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
        Me.m_btButton = New System.Windows.Forms.Button()
        Me.m_txtTextbox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnClickMe = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'm_btButton
        '
        Me.m_btButton.Location = New System.Drawing.Point(202, 22)
        Me.m_btButton.Name = "m_btButton"
        Me.m_btButton.Size = New System.Drawing.Size(130, 31)
        Me.m_btButton.TabIndex = 0
        Me.m_btButton.Text = "Pass value to Plugin"
        Me.m_btButton.UseVisualStyleBackColor = True
        '
        'm_txtTextbox
        '
        Me.m_txtTextbox.Location = New System.Drawing.Point(82, 28)
        Me.m_txtTextbox.Name = "m_txtTextbox"
        Me.m_txtTextbox.Size = New System.Drawing.Size(114, 20)
        Me.m_txtTextbox.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Enter value:"
        '
        'btnClickMe
        '
        Me.btnClickMe.Location = New System.Drawing.Point(15, 69)
        Me.btnClickMe.Name = "btnClickMe"
        Me.btnClickMe.Size = New System.Drawing.Size(128, 23)
        Me.btnClickMe.TabIndex = 3
        Me.btnClickMe.Text = "Click me! Click me!"
        Me.btnClickMe.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(169, 69)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(106, 20)
        Me.TextBox1.TabIndex = 4
        '
        'frmEwEPlugin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(573, 351)
        Me.ControlBox = False
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.btnClickMe)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_txtTextbox)
        Me.Controls.Add(Me.m_btButton)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEwEPlugin"
        Me.ShowInTaskbar = False
        Me.TabText = "Basic plug-in"
        Me.Text = "Basic plug-in"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btButton As System.Windows.Forms.Button
    Friend WithEvents m_txtTextbox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnClickMe As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
