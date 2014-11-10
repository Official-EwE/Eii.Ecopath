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
        Me.m_lbOutputFile = New System.Windows.Forms.Label()
        Me.m_btOuputFile = New System.Windows.Forms.Button()
        Me.m_txBounds = New System.Windows.Forms.TextBox()
        Me.m_lbBounds = New System.Windows.Forms.Label()
        Me.m_btStopRun = New System.Windows.Forms.Button()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'm_btRun
        '
        Me.m_btRun.Location = New System.Drawing.Point(14, 25)
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.Size = New System.Drawing.Size(141, 27)
        Me.m_btRun.TabIndex = 0
        Me.m_btRun.Text = "Run bounds testing"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_pbTotalProgress
        '
        Me.m_pbTotalProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_pbTotalProgress.Location = New System.Drawing.Point(11, 327)
        Me.m_pbTotalProgress.Name = "m_pbTotalProgress"
        Me.m_pbTotalProgress.Size = New System.Drawing.Size(563, 26)
        Me.m_pbTotalProgress.Step = 1
        Me.m_pbTotalProgress.TabIndex = 1
        '
        'm_pbRunProgress
        '
        Me.m_pbRunProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_pbRunProgress.Location = New System.Drawing.Point(11, 297)
        Me.m_pbRunProgress.Name = "m_pbRunProgress"
        Me.m_pbRunProgress.Size = New System.Drawing.Size(563, 24)
        Me.m_pbRunProgress.Step = 1
        Me.m_pbRunProgress.TabIndex = 2
        '
        'm_lbOutputFile
        '
        Me.m_lbOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbOutputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lbOutputFile.Location = New System.Drawing.Point(161, 173)
        Me.m_lbOutputFile.Name = "m_lbOutputFile"
        Me.m_lbOutputFile.Size = New System.Drawing.Size(414, 24)
        Me.m_lbOutputFile.TabIndex = 3
        '
        'm_btOuputFile
        '
        Me.m_btOuputFile.Location = New System.Drawing.Point(11, 173)
        Me.m_btOuputFile.Name = "m_btOuputFile"
        Me.m_btOuputFile.Size = New System.Drawing.Size(144, 25)
        Me.m_btOuputFile.TabIndex = 4
        Me.m_btOuputFile.Text = "Output file..."
        Me.m_btOuputFile.UseVisualStyleBackColor = True
        '
        'm_txBounds
        '
        Me.m_txBounds.Location = New System.Drawing.Point(161, 134)
        Me.m_txBounds.Name = "m_txBounds"
        Me.m_txBounds.Size = New System.Drawing.Size(66, 20)
        Me.m_txBounds.TabIndex = 5
        '
        'm_lbBounds
        '
        Me.m_lbBounds.AutoSize = True
        Me.m_lbBounds.Location = New System.Drawing.Point(11, 137)
        Me.m_lbBounds.Name = "m_lbBounds"
        Me.m_lbBounds.Size = New System.Drawing.Size(112, 13)
        Me.m_lbBounds.TabIndex = 6
        Me.m_lbBounds.Text = "Percentage of bounds"
        '
        'm_btStopRun
        '
        Me.m_btStopRun.Location = New System.Drawing.Point(14, 55)
        Me.m_btStopRun.Name = "m_btStopRun"
        Me.m_btStopRun.Size = New System.Drawing.Size(141, 27)
        Me.m_btStopRun.TabIndex = 7
        Me.m_btStopRun.Text = "Stop run"
        Me.m_btStopRun.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(12, 100)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(563, 31)
        Me.CEwEHeaderLabel1.TabIndex = 8
        Me.CEwEHeaderLabel1.Text = "Parameters"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Location = New System.Drawing.Point(11, 270)
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        Me.CEwEHeaderLabel2.Size = New System.Drawing.Size(563, 12)
        Me.CEwEHeaderLabel2.TabIndex = 9
        Me.CEwEHeaderLabel2.Text = "Progress"
        Me.CEwEHeaderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmEcospaceSensitivity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(587, 365)
        Me.ControlBox = False
        Me.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_btStopRun)
        Me.Controls.Add(Me.m_lbBounds)
        Me.Controls.Add(Me.m_txBounds)
        Me.Controls.Add(Me.m_btOuputFile)
        Me.Controls.Add(Me.m_lbOutputFile)
        Me.Controls.Add(Me.m_pbRunProgress)
        Me.Controls.Add(Me.m_pbTotalProgress)
        Me.Controls.Add(Me.m_btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceSensitivity"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace Sensitivity"
        Me.Text = "Ecospace Sensitivity Analysis"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btRun As System.Windows.Forms.Button
    Friend WithEvents m_pbTotalProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents m_pbRunProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents m_lbOutputFile As System.Windows.Forms.Label
    Friend WithEvents m_btOuputFile As System.Windows.Forms.Button
    Friend WithEvents m_txBounds As System.Windows.Forms.TextBox
    Friend WithEvents m_lbBounds As System.Windows.Forms.Label
    Friend WithEvents m_btStopRun As System.Windows.Forms.Button
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
