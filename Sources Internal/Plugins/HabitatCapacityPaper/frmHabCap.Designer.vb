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
' Copyright 1991-2012 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHabCap
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.m_btStop = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lstMessages = New System.Windows.Forms.ListBox()
        Me.m_btRun = New System.Windows.Forms.Button()
        Me.WorkerThread = New System.ComponentModel.BackgroundWorker()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.m_btStop)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.m_lstMessages)
        Me.Panel1.Controls.Add(Me.m_btRun)
        Me.Panel1.Location = New System.Drawing.Point(1, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(689, 389)
        Me.Panel1.TabIndex = 3
        '
        'm_btStop
        '
        Me.m_btStop.Location = New System.Drawing.Point(12, 45)
        Me.m_btStop.Name = "m_btStop"
        Me.m_btStop.Size = New System.Drawing.Size(165, 27)
        Me.m_btStop.TabIndex = 4
        Me.m_btStop.Text = "Stop run"
        Me.m_btStop.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 89)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Messages"
        '
        'm_lstMessages
        '
        Me.m_lstMessages.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lstMessages.FormattingEnabled = True
        Me.m_lstMessages.Location = New System.Drawing.Point(12, 106)
        Me.m_lstMessages.Name = "m_lstMessages"
        Me.m_lstMessages.Size = New System.Drawing.Size(667, 264)
        Me.m_lstMessages.TabIndex = 4
        '
        'm_btRun
        '
        Me.m_btRun.Location = New System.Drawing.Point(12, 12)
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.Size = New System.Drawing.Size(165, 27)
        Me.m_btRun.TabIndex = 3
        Me.m_btRun.Text = "Run Habitat Capacity Analysis"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_bgw
        '
        Me.WorkerThread.WorkerReportsProgress = True
        Me.WorkerThread.WorkerSupportsCancellation = True
        '
        'frmHabCap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(692, 402)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmHabCap"
        Me.ShowInTaskbar = False
        Me.TabText = "Habitat Capacity"
        Me.Text = "Habitat Capacity"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents m_lstMessages As System.Windows.Forms.ListBox
    Friend WithEvents m_btRun As System.Windows.Forms.Button
    Friend WithEvents m_btStop As System.Windows.Forms.Button
    Friend WithEvents WorkerThread As System.ComponentModel.BackgroundWorker
End Class
