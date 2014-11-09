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
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcospaceSensitivity
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
        Me.m_btRun = New System.Windows.Forms.Button()
        Me.m_pbTotalProgress = New System.Windows.Forms.ProgressBar()
        Me.m_pbRunProgress = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'm_btRun
        '
        Me.m_btRun.Location = New System.Drawing.Point(12, 25)
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.Size = New System.Drawing.Size(147, 24)
        Me.m_btRun.TabIndex = 0
        Me.m_btRun.Text = "Run (For Testing)"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_pbTotalProgress
        '
        Me.m_pbTotalProgress.Location = New System.Drawing.Point(12, 103)
        Me.m_pbTotalProgress.Name = "m_pbTotalProgress"
        Me.m_pbTotalProgress.Size = New System.Drawing.Size(489, 23)
        Me.m_pbTotalProgress.Step = 1
        Me.m_pbTotalProgress.TabIndex = 1
        '
        'm_pbRunProgress
        '
        Me.m_pbRunProgress.Location = New System.Drawing.Point(12, 74)
        Me.m_pbRunProgress.Name = "m_pbRunProgress"
        Me.m_pbRunProgress.Size = New System.Drawing.Size(489, 23)
        Me.m_pbRunProgress.Step = 1
        Me.m_pbRunProgress.TabIndex = 2
        '
        'frmEcospaceSensitivity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(566, 334)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_pbRunProgress)
        Me.Controls.Add(Me.m_pbTotalProgress)
        Me.Controls.Add(Me.m_btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceSensitivity"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace Sensitivity"
        Me.Text = "Ecospace Sensitivity Analysis"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_btRun As System.Windows.Forms.Button
    Friend WithEvents m_pbTotalProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents m_pbRunProgress As System.Windows.Forms.ProgressBar
End Class
