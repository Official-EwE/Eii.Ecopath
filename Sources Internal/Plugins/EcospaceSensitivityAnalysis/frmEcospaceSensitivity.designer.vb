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
        Me.components = New System.ComponentModel.Container()
        Me.m_pbTotalProgress = New System.Windows.Forms.ProgressBar()
        Me.m_pbRunProgress = New System.Windows.Forms.ProgressBar()
        Me.m_lbOutputFile = New System.Windows.Forms.Label()
        Me.m_btStopRun = New System.Windows.Forms.Button()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lbBounds = New System.Windows.Forms.Label()
        Me.m_txBounds = New System.Windows.Forms.TextBox()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btRunRemoval = New System.Windows.Forms.Button()
        Me.m_btStopRemoval = New System.Windows.Forms.Button()
        Me.m_ttFiles = New System.Windows.Forms.ToolTip(Me.components)
        Me.CEwEHeaderLabel4 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btRemovalOutput = New System.Windows.Forms.Button()
        Me.m_lbRemoval = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.m_btRun = New System.Windows.Forms.Button()
        Me.m_lvFiles = New System.Windows.Forms.ListView()
        Me.HeaderLayer = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.HeaderFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lbBoundsPar = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btOuputFile = New System.Windows.Forms.Button()
        Me.CEwEHeaderLabel3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'm_pbTotalProgress
        '
        Me.m_pbTotalProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_pbTotalProgress.Location = New System.Drawing.Point(14, 529)
        Me.m_pbTotalProgress.Name = "m_pbTotalProgress"
        Me.m_pbTotalProgress.Size = New System.Drawing.Size(1167, 26)
        Me.m_pbTotalProgress.Step = 1
        Me.m_pbTotalProgress.TabIndex = 1
        '
        'm_pbRunProgress
        '
        Me.m_pbRunProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_pbRunProgress.Location = New System.Drawing.Point(14, 483)
        Me.m_pbRunProgress.Name = "m_pbRunProgress"
        Me.m_pbRunProgress.Size = New System.Drawing.Size(1167, 24)
        Me.m_pbRunProgress.Step = 1
        Me.m_pbRunProgress.TabIndex = 2
        '
        'm_lbOutputFile
        '
        Me.m_lbOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbOutputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lbOutputFile.Location = New System.Drawing.Point(161, 96)
        Me.m_lbOutputFile.Name = "m_lbOutputFile"
        Me.m_lbOutputFile.Size = New System.Drawing.Size(1016, 23)
        Me.m_lbOutputFile.TabIndex = 3
        '
        'm_btStopRun
        '
        Me.m_btStopRun.Location = New System.Drawing.Point(161, 32)
        Me.m_btStopRun.Name = "m_btStopRun"
        Me.m_btStopRun.Size = New System.Drawing.Size(141, 23)
        Me.m_btStopRun.TabIndex = 7
        Me.m_btStopRun.Text = "Stop run"
        Me.m_btStopRun.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Location = New System.Drawing.Point(11, 447)
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        Me.CEwEHeaderLabel2.Size = New System.Drawing.Size(1167, 12)
        Me.CEwEHeaderLabel2.TabIndex = 9
        Me.CEwEHeaderLabel2.Text = "Progress"
        Me.CEwEHeaderLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lbBounds
        '
        Me.m_lbBounds.AutoSize = True
        Me.m_lbBounds.Location = New System.Drawing.Point(14, 133)
        Me.m_lbBounds.Name = "m_lbBounds"
        Me.m_lbBounds.Size = New System.Drawing.Size(82, 13)
        Me.m_lbBounds.TabIndex = 11
        Me.m_lbBounds.Text = "Delta of bounds"
        '
        'm_txBounds
        '
        Me.m_txBounds.Location = New System.Drawing.Point(161, 133)
        Me.m_txBounds.Name = "m_txBounds"
        Me.m_txBounds.Size = New System.Drawing.Size(66, 20)
        Me.m_txBounds.TabIndex = 10
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(11, 317)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(1168, 20)
        Me.CEwEHeaderLabel1.TabIndex = 13
        Me.CEwEHeaderLabel1.Text = "Sensitivity to functional group responses"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btRunRemoval
        '
        Me.m_btRunRemoval.Location = New System.Drawing.Point(14, 340)
        Me.m_btRunRemoval.Name = "m_btRunRemoval"
        Me.m_btRunRemoval.Size = New System.Drawing.Size(141, 23)
        Me.m_btRunRemoval.TabIndex = 14
        Me.m_btRunRemoval.Text = "Run"
        Me.m_btRunRemoval.UseVisualStyleBackColor = True
        '
        'm_btStopRemoval
        '
        Me.m_btStopRemoval.Location = New System.Drawing.Point(161, 340)
        Me.m_btStopRemoval.Name = "m_btStopRemoval"
        Me.m_btStopRemoval.Size = New System.Drawing.Size(141, 23)
        Me.m_btStopRemoval.TabIndex = 15
        Me.m_btStopRemoval.Text = "Stop run"
        Me.m_btStopRemoval.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel4
        '
        Me.CEwEHeaderLabel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel4.CanCollapseParent = False
        Me.CEwEHeaderLabel4.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel4.IsCollapsed = False
        Me.CEwEHeaderLabel4.Location = New System.Drawing.Point(11, 375)
        Me.CEwEHeaderLabel4.Name = "CEwEHeaderLabel4"
        Me.CEwEHeaderLabel4.Size = New System.Drawing.Size(1167, 27)
        Me.CEwEHeaderLabel4.TabIndex = 17
        Me.CEwEHeaderLabel4.Text = "Parameters"
        Me.CEwEHeaderLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btRemovalOutput
        '
        Me.m_btRemovalOutput.Location = New System.Drawing.Point(14, 405)
        Me.m_btRemovalOutput.Name = "m_btRemovalOutput"
        Me.m_btRemovalOutput.Size = New System.Drawing.Size(141, 23)
        Me.m_btRemovalOutput.TabIndex = 19
        Me.m_btRemovalOutput.Text = "Output file..."
        Me.m_btRemovalOutput.UseVisualStyleBackColor = True
        '
        'm_lbRemoval
        '
        Me.m_lbRemoval.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbRemoval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lbRemoval.Location = New System.Drawing.Point(161, 405)
        Me.m_lbRemoval.Name = "m_lbRemoval"
        Me.m_lbRemoval.Size = New System.Drawing.Size(1020, 23)
        Me.m_lbRemoval.TabIndex = 18
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(14, 467)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(116, 13)
        Me.Label2.TabIndex = 20
        Me.Label2.Text = "Ecospace run progress"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 513)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(92, 13)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "Total run progress"
        '
        'm_btRun
        '
        Me.m_btRun.Location = New System.Drawing.Point(14, 32)
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.Size = New System.Drawing.Size(141, 23)
        Me.m_btRun.TabIndex = 0
        Me.m_btRun.Text = "Run"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_lvFiles
        '
        Me.m_lvFiles.Activation = System.Windows.Forms.ItemActivation.TwoClick
        Me.m_lvFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lvFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.HeaderLayer, Me.HeaderFile})
        Me.m_lvFiles.FullRowSelect = True
        Me.m_lvFiles.HideSelection = False
        Me.m_lvFiles.Location = New System.Drawing.Point(14, 181)
        Me.m_lvFiles.MultiSelect = False
        Me.m_lvFiles.Name = "m_lvFiles"
        Me.m_lvFiles.Size = New System.Drawing.Size(1163, 112)
        Me.m_lvFiles.TabIndex = 10
        Me.m_lvFiles.UseCompatibleStateImageBehavior = False
        Me.m_lvFiles.View = System.Windows.Forms.View.Details
        '
        'HeaderLayer
        '
        Me.HeaderLayer.Text = "Driver layer"
        Me.HeaderLayer.Width = 100
        '
        'HeaderFile
        '
        Me.HeaderFile.Text = ".asc file"
        Me.HeaderFile.Width = 400
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 165)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(127, 13)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Select external driver files"
        '
        'm_lbBoundsPar
        '
        Me.m_lbBoundsPar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbBoundsPar.CanCollapseParent = False
        Me.m_lbBoundsPar.CollapsedParentHeight = 0
        Me.m_lbBoundsPar.IsCollapsed = False
        Me.m_lbBoundsPar.Location = New System.Drawing.Point(11, 72)
        Me.m_lbBoundsPar.Name = "m_lbBoundsPar"
        Me.m_lbBoundsPar.Size = New System.Drawing.Size(1167, 21)
        Me.m_lbBoundsPar.TabIndex = 12
        Me.m_lbBoundsPar.Text = "Parameters"
        Me.m_lbBoundsPar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btOuputFile
        '
        Me.m_btOuputFile.Location = New System.Drawing.Point(14, 96)
        Me.m_btOuputFile.Name = "m_btOuputFile"
        Me.m_btOuputFile.Size = New System.Drawing.Size(141, 23)
        Me.m_btOuputFile.TabIndex = 4
        Me.m_btOuputFile.Text = "Output file..."
        Me.m_btOuputFile.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel3
        '
        Me.CEwEHeaderLabel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel3.CanCollapseParent = False
        Me.CEwEHeaderLabel3.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel3.IsCollapsed = False
        Me.CEwEHeaderLabel3.Location = New System.Drawing.Point(11, 10)
        Me.CEwEHeaderLabel3.Name = "CEwEHeaderLabel3"
        Me.CEwEHeaderLabel3.Size = New System.Drawing.Size(1167, 19)
        Me.CEwEHeaderLabel3.TabIndex = 11
        Me.CEwEHeaderLabel3.Text = "Sensitivity to changes in input factors"
        Me.CEwEHeaderLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmEcospaceSensitivity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1191, 579)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lvFiles)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_btOuputFile)
        Me.Controls.Add(Me.CEwEHeaderLabel3)
        Me.Controls.Add(Me.m_lbBoundsPar)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.m_btRemovalOutput)
        Me.Controls.Add(Me.m_btRun)
        Me.Controls.Add(Me.m_lbRemoval)
        Me.Controls.Add(Me.CEwEHeaderLabel4)
        Me.Controls.Add(Me.m_btStopRemoval)
        Me.Controls.Add(Me.m_btRunRemoval)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_lbBounds)
        Me.Controls.Add(Me.m_txBounds)
        Me.Controls.Add(Me.m_lbOutputFile)
        Me.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Controls.Add(Me.m_btStopRun)
        Me.Controls.Add(Me.m_pbRunProgress)
        Me.Controls.Add(Me.m_pbTotalProgress)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceSensitivity"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace Sensitivity"
        Me.Text = "Ecospace Sensitivity Analysis"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_pbTotalProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents m_pbRunProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents m_lbOutputFile As System.Windows.Forms.Label
    Friend WithEvents m_btStopRun As System.Windows.Forms.Button
    Friend WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_lbBounds As System.Windows.Forms.Label
    Friend WithEvents m_txBounds As System.Windows.Forms.TextBox
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_btRunRemoval As System.Windows.Forms.Button
    Friend WithEvents m_btStopRemoval As System.Windows.Forms.Button
    Friend WithEvents m_ttFiles As System.Windows.Forms.ToolTip
    Friend WithEvents CEwEHeaderLabel4 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_btRemovalOutput As System.Windows.Forms.Button
    Friend WithEvents m_lbRemoval As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents m_btRun As System.Windows.Forms.Button
    Friend WithEvents m_lvFiles As System.Windows.Forms.ListView
    Friend WithEvents HeaderLayer As System.Windows.Forms.ColumnHeader
    Friend WithEvents HeaderFile As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents m_lbBoundsPar As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_btOuputFile As System.Windows.Forms.Button
    Friend WithEvents CEwEHeaderLabel3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
