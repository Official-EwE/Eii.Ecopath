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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProgress
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmProgress))
        Me.m_progress = New System.Windows.Forms.ProgressBar()
        Me.m_lblInfo = New System.Windows.Forms.Label()
        Me.m_btnStop = New System.Windows.Forms.Button()
        Me.m_pbIcon = New System.Windows.Forms.PictureBox()
        CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_progress
        '
        resources.ApplyResources(Me.m_progress, "m_progress")
        Me.m_progress.Name = "m_progress"
        '
        'm_lblInfo
        '
        resources.ApplyResources(Me.m_lblInfo, "m_lblInfo")
        Me.m_lblInfo.Name = "m_lblInfo"
        '
        'm_btnStop
        '
        resources.ApplyResources(Me.m_btnStop, "m_btnStop")
        Me.m_btnStop.Name = "m_btnStop"
        Me.m_btnStop.UseVisualStyleBackColor = True
        '
        'm_pbIcon
        '
        resources.ApplyResources(Me.m_pbIcon, "m_pbIcon")
        Me.m_pbIcon.Name = "m_pbIcon"
        Me.m_pbIcon.TabStop = False
        '
        'frmProgress
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ControlBox = False
        Me.Controls.Add(Me.m_pbIcon)
        Me.Controls.Add(Me.m_btnStop)
        Me.Controls.Add(Me.m_lblInfo)
        Me.Controls.Add(Me.m_progress)
        Me.Name = "frmProgress"
        CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btnStop As System.Windows.Forms.Button
    Private WithEvents m_progress As System.Windows.Forms.ProgressBar
    Private WithEvents m_lblInfo As System.Windows.Forms.Label
    Private WithEvents m_pbIcon As System.Windows.Forms.PictureBox
End Class
