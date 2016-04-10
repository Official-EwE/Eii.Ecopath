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
Partial Class frmEcospaceMonteCarlo
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
        Me.m_btOutput = New System.Windows.Forms.Button()
        Me.m_btStop = New System.Windows.Forms.Button()
        Me.m_txBeforeStart = New System.Windows.Forms.TextBox()
        Me.m_txBeforeNYears = New System.Windows.Forms.TextBox()
        Me.m_txAfterStart = New System.Windows.Forms.TextBox()
        Me.m_txAfterNYears = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.m_lbOutputFile = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'm_btOutput
        '
        Me.m_btOutput.Location = New System.Drawing.Point(26, 23)
        Me.m_btOutput.Name = "m_btOutput"
        Me.m_btOutput.Size = New System.Drawing.Size(158, 23)
        Me.m_btOutput.TabIndex = 0
        Me.m_btOutput.Text = "Select output file..."
        Me.m_btOutput.UseVisualStyleBackColor = True
        '
        'm_btStop
        '
        Me.m_btStop.Location = New System.Drawing.Point(26, 189)
        Me.m_btStop.Name = "m_btStop"
        Me.m_btStop.Size = New System.Drawing.Size(158, 23)
        Me.m_btStop.TabIndex = 1
        Me.m_btStop.Text = "Stop run"
        Me.m_btStop.UseVisualStyleBackColor = True
        '
        'm_txBeforeStart
        '
        Me.m_txBeforeStart.Location = New System.Drawing.Point(113, 115)
        Me.m_txBeforeStart.Name = "m_txBeforeStart"
        Me.m_txBeforeStart.Size = New System.Drawing.Size(68, 20)
        Me.m_txBeforeStart.TabIndex = 2
        '
        'm_txBeforeNYears
        '
        Me.m_txBeforeNYears.Location = New System.Drawing.Point(187, 115)
        Me.m_txBeforeNYears.Name = "m_txBeforeNYears"
        Me.m_txBeforeNYears.Size = New System.Drawing.Size(68, 20)
        Me.m_txBeforeNYears.TabIndex = 3
        '
        'm_txAfterStart
        '
        Me.m_txAfterStart.Location = New System.Drawing.Point(113, 141)
        Me.m_txAfterStart.Name = "m_txAfterStart"
        Me.m_txAfterStart.Size = New System.Drawing.Size(68, 20)
        Me.m_txAfterStart.TabIndex = 4
        '
        'm_txAfterNYears
        '
        Me.m_txAfterNYears.Location = New System.Drawing.Point(187, 141)
        Me.m_txAfterNYears.Name = "m_txAfterNYears"
        Me.m_txAfterNYears.Size = New System.Drawing.Size(68, 20)
        Me.m_txAfterNYears.TabIndex = 5
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(26, 115)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Before project"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(26, 146)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "After project"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(110, 99)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(52, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Start year"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(184, 99)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Run length"
        '
        'm_lbOutputFile
        '
        Me.m_lbOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbOutputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lbOutputFile.Location = New System.Drawing.Point(26, 49)
        Me.m_lbOutputFile.Name = "m_lbOutputFile"
        Me.m_lbOutputFile.Size = New System.Drawing.Size(519, 21)
        Me.m_lbOutputFile.TabIndex = 10
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(26, 215)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(250, 74)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "WARNING: Stop run will stop the current run, but the Monte Carlo form will still " & _
    "not be enabled. To re-start the run close the Monte Carlo form and re-open it." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & _
    ""
        '
        'frmEcospaceMonteCarlo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(566, 334)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.m_lbOutputFile)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_txAfterNYears)
        Me.Controls.Add(Me.m_txAfterStart)
        Me.Controls.Add(Me.m_txBeforeNYears)
        Me.Controls.Add(Me.m_txBeforeStart)
        Me.Controls.Add(Me.m_btStop)
        Me.Controls.Add(Me.m_btOutput)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceMonteCarlo"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace RBT MonteCarlo"
        Me.Text = "Ecospace RBT MonteCarlo"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btOutput As System.Windows.Forms.Button
    Friend WithEvents m_btStop As System.Windows.Forms.Button
    Friend WithEvents m_txBeforeStart As System.Windows.Forms.TextBox
    Friend WithEvents m_txBeforeNYears As System.Windows.Forms.TextBox
    Friend WithEvents m_txAfterStart As System.Windows.Forms.TextBox
    Friend WithEvents m_txAfterNYears As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents m_lbOutputFile As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
End Class
