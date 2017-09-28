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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Partial Class ucEcoEngineerConfigUI
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.m_clbGroups = New System.Windows.Forms.CheckedListBox()
        Me.m_tbxA = New System.Windows.Forms.TextBox()
        Me.m_lblX2 = New System.Windows.Forms.Label()
        Me.m_tbxB = New System.Windows.Forms.TextBox()
        Me.m_lblX = New System.Windows.Forms.Label()
        Me.m_tbxC = New System.Windows.Forms.TextBox()
        Me.m_lblName = New System.Windows.Forms.Label()
        Me.m_tbxName = New System.Windows.Forms.TextBox()
        Me.m_lblDescription = New System.Windows.Forms.Label()
        Me.m_tbxDescription = New System.Windows.Forms.TextBox()
        Me.m_tlpFormula = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblFormula = New System.Windows.Forms.Label()
        Me.m_lblDefaults = New System.Windows.Forms.Label()
        Me.m_cmbPreDefinedRules = New System.Windows.Forms.ComboBox()
        Me.m_lblXAxis = New System.Windows.Forms.Label()
        Me.m_lblYAxis = New System.Windows.Forms.Label()
        Me.m_scContent = New System.Windows.Forms.SplitContainer()
        Me.m_sketchpad = New EcoEngineersPlugin.ucEcoEngineerSketchPad()
        Me.m_btnAddPredefined = New System.Windows.Forms.Button()
        Me.m_btnDeletePredefined = New System.Windows.Forms.Button()
        Me.m_tbxFunctionName = New System.Windows.Forms.TextBox()
        Me.m_lblFunction = New System.Windows.Forms.Label()
        Me.m_tcContent = New System.Windows.Forms.TabControl()
        Me.m_tpCalcs = New System.Windows.Forms.TabPage()
        Me.m_tpAcknowledgements = New System.Windows.Forms.TabPage()
        Me.m_flpLogos = New System.Windows.Forms.FlowLayoutPanel()
        Me.m_pbEII = New System.Windows.Forms.PictureBox()
        Me.m_pbMare = New System.Windows.Forms.PictureBox()
        Me.m_tlpFormula.SuspendLayout()
        CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scContent.Panel1.SuspendLayout()
        Me.m_scContent.Panel2.SuspendLayout()
        Me.m_scContent.SuspendLayout()
        Me.m_tcContent.SuspendLayout()
        Me.m_tpCalcs.SuspendLayout()
        Me.m_tpAcknowledgements.SuspendLayout()
        Me.m_flpLogos.SuspendLayout()
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbMare, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_clbGroups
        '
        Me.m_clbGroups.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_clbGroups.FormattingEnabled = True
        Me.m_clbGroups.IntegralHeight = False
        Me.m_clbGroups.Location = New System.Drawing.Point(0, 0)
        Me.m_clbGroups.Margin = New System.Windows.Forms.Padding(0)
        Me.m_clbGroups.Name = "m_clbGroups"
        Me.m_clbGroups.Size = New System.Drawing.Size(120, 349)
        Me.m_clbGroups.TabIndex = 0
        '
        'm_tbxA
        '
        Me.m_tbxA.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxA.Location = New System.Drawing.Point(0, 0)
        Me.m_tbxA.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tbxA.Name = "m_tbxA"
        Me.m_tbxA.Size = New System.Drawing.Size(84, 20)
        Me.m_tbxA.TabIndex = 0
        '
        'm_lblX2
        '
        Me.m_lblX2.AutoSize = True
        Me.m_lblX2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblX2.Location = New System.Drawing.Point(84, 0)
        Me.m_lblX2.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblX2.Name = "m_lblX2"
        Me.m_lblX2.Size = New System.Drawing.Size(27, 20)
        Me.m_lblX2.TabIndex = 1
        Me.m_lblX2.Text = "x² + "
        Me.m_lblX2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbxB
        '
        Me.m_tbxB.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxB.Location = New System.Drawing.Point(111, 0)
        Me.m_tbxB.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tbxB.Name = "m_tbxB"
        Me.m_tbxB.Size = New System.Drawing.Size(84, 20)
        Me.m_tbxB.TabIndex = 2
        '
        'm_lblX
        '
        Me.m_lblX.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblX.Location = New System.Drawing.Point(195, 0)
        Me.m_lblX.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblX.Name = "m_lblX"
        Me.m_lblX.Size = New System.Drawing.Size(26, 20)
        Me.m_lblX.TabIndex = 3
        Me.m_lblX.Text = "x + "
        Me.m_lblX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbxC
        '
        Me.m_tbxC.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tbxC.Location = New System.Drawing.Point(221, 0)
        Me.m_tbxC.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tbxC.Name = "m_tbxC"
        Me.m_tbxC.Size = New System.Drawing.Size(85, 20)
        Me.m_tbxC.TabIndex = 4
        '
        'm_lblName
        '
        Me.m_lblName.AutoSize = True
        Me.m_lblName.Location = New System.Drawing.Point(3, 6)
        Me.m_lblName.Name = "m_lblName"
        Me.m_lblName.Size = New System.Drawing.Size(38, 13)
        Me.m_lblName.TabIndex = 0
        Me.m_lblName.Text = "&Name:"
        '
        'm_tbxName
        '
        Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxName.Location = New System.Drawing.Point(72, 3)
        Me.m_tbxName.Name = "m_tbxName"
        Me.m_tbxName.Size = New System.Drawing.Size(452, 20)
        Me.m_tbxName.TabIndex = 1
        '
        'm_lblDescription
        '
        Me.m_lblDescription.AutoSize = True
        Me.m_lblDescription.Location = New System.Drawing.Point(3, 31)
        Me.m_lblDescription.Name = "m_lblDescription"
        Me.m_lblDescription.Size = New System.Drawing.Size(63, 13)
        Me.m_lblDescription.TabIndex = 2
        Me.m_lblDescription.Text = "&Description:"
        '
        'm_tbxDescription
        '
        Me.m_tbxDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxDescription.Location = New System.Drawing.Point(72, 28)
        Me.m_tbxDescription.Name = "m_tbxDescription"
        Me.m_tbxDescription.Size = New System.Drawing.Size(452, 20)
        Me.m_tbxDescription.TabIndex = 3
        '
        'm_tlpFormula
        '
        Me.m_tlpFormula.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpFormula.ColumnCount = 5
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpFormula.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.m_tlpFormula.Controls.Add(Me.m_tbxA, 0, 0)
        Me.m_tlpFormula.Controls.Add(Me.m_lblX2, 1, 0)
        Me.m_tlpFormula.Controls.Add(Me.m_tbxB, 2, 0)
        Me.m_tlpFormula.Controls.Add(Me.m_lblX, 3, 0)
        Me.m_tlpFormula.Controls.Add(Me.m_tbxC, 4, 0)
        Me.m_tlpFormula.Location = New System.Drawing.Point(73, 56)
        Me.m_tlpFormula.Name = "m_tlpFormula"
        Me.m_tlpFormula.RowCount = 1
        Me.m_tlpFormula.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpFormula.Size = New System.Drawing.Size(306, 20)
        Me.m_tlpFormula.TabIndex = 7
        '
        'm_lblFormula
        '
        Me.m_lblFormula.AutoSize = True
        Me.m_lblFormula.Location = New System.Drawing.Point(3, 60)
        Me.m_lblFormula.Name = "m_lblFormula"
        Me.m_lblFormula.Size = New System.Drawing.Size(47, 13)
        Me.m_lblFormula.TabIndex = 6
        Me.m_lblFormula.Text = "&Formula:"
        '
        'm_lblDefaults
        '
        Me.m_lblDefaults.AutoSize = True
        Me.m_lblDefaults.Location = New System.Drawing.Point(3, 6)
        Me.m_lblDefaults.Name = "m_lblDefaults"
        Me.m_lblDefaults.Size = New System.Drawing.Size(64, 13)
        Me.m_lblDefaults.TabIndex = 0
        Me.m_lblDefaults.Text = "Pre-defined:"
        '
        'm_cmbPreDefinedRules
        '
        Me.m_cmbPreDefinedRules.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_cmbPreDefinedRules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbPreDefinedRules.FormattingEnabled = True
        Me.m_cmbPreDefinedRules.Location = New System.Drawing.Point(73, 3)
        Me.m_cmbPreDefinedRules.Name = "m_cmbPreDefinedRules"
        Me.m_cmbPreDefinedRules.Size = New System.Drawing.Size(275, 21)
        Me.m_cmbPreDefinedRules.Sorted = True
        Me.m_cmbPreDefinedRules.TabIndex = 1
        '
        'm_lblXAxis
        '
        Me.m_lblXAxis.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lblXAxis.Enabled = False
        Me.m_lblXAxis.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_lblXAxis.Location = New System.Drawing.Point(28, 331)
        Me.m_lblXAxis.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblXAxis.Name = "m_lblXAxis"
        Me.m_lblXAxis.Size = New System.Drawing.Size(351, 18)
        Me.m_lblXAxis.TabIndex = 10
        Me.m_lblXAxis.Text = "Ecosystem engineer biomass (g/m²)"
        Me.m_lblXAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'm_lblYAxis
        '
        Me.m_lblYAxis.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_lblYAxis.Enabled = False
        Me.m_lblYAxis.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_lblYAxis.Location = New System.Drawing.Point(7, 81)
        Me.m_lblYAxis.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lblYAxis.Name = "m_lblYAxis"
        Me.m_lblYAxis.Size = New System.Drawing.Size(18, 247)
        Me.m_lblYAxis.TabIndex = 8
        Me.m_lblYAxis.Text = "Architectural complexity (cm³)"
        Me.m_lblYAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'm_scContent
        '
        Me.m_scContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_scContent.Location = New System.Drawing.Point(3, 3)
        Me.m_scContent.Name = "m_scContent"
        '
        'm_scContent.Panel1
        '
        Me.m_scContent.Panel1.Controls.Add(Me.m_clbGroups)
        '
        'm_scContent.Panel2
        '
        Me.m_scContent.Panel2.Controls.Add(Me.m_sketchpad)
        Me.m_scContent.Panel2.Controls.Add(Me.m_btnAddPredefined)
        Me.m_scContent.Panel2.Controls.Add(Me.m_btnDeletePredefined)
        Me.m_scContent.Panel2.Controls.Add(Me.m_cmbPreDefinedRules)
        Me.m_scContent.Panel2.Controls.Add(Me.m_lblYAxis)
        Me.m_scContent.Panel2.Controls.Add(Me.m_tbxFunctionName)
        Me.m_scContent.Panel2.Controls.Add(Me.m_lblDefaults)
        Me.m_scContent.Panel2.Controls.Add(Me.m_lblXAxis)
        Me.m_scContent.Panel2.Controls.Add(Me.m_tlpFormula)
        Me.m_scContent.Panel2.Controls.Add(Me.m_lblFunction)
        Me.m_scContent.Panel2.Controls.Add(Me.m_lblFormula)
        Me.m_scContent.Size = New System.Drawing.Size(506, 349)
        Me.m_scContent.SplitterDistance = 120
        Me.m_scContent.TabIndex = 0
        '
        'm_sketchpad
        '
        Me.m_sketchpad.AllowDragXMark = False
        Me.m_sketchpad.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_sketchpad.AxisTickMarkDisplayMode = ScientificInterfaceShared.Definitions.eAxisTickmarkDisplayModeTypes.Absolute
        Me.m_sketchpad.BackColor = System.Drawing.SystemColors.Window
        Me.m_sketchpad.Cursor = System.Windows.Forms.Cursors.Cross
        Me.m_sketchpad.DisplayAxis = True
        Me.m_sketchpad.Editable = False
        Me.m_sketchpad.Handler = Nothing
        Me.m_sketchpad.IsSeasonal = False
        Me.m_sketchpad.Location = New System.Drawing.Point(31, 88)
        Me.m_sketchpad.Margin = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.m_sketchpad.MaxXValue = 25000.0!
        Me.m_sketchpad.Name = "m_sketchpad"
        Me.m_sketchpad.NumDataPoints = 0
        Me.m_sketchpad.Shape = Nothing
        Me.m_sketchpad.ShapeColor = System.Drawing.Color.DarkTurquoise
        Me.m_sketchpad.ShowValueTooltip = True
        Me.m_sketchpad.ShowXMark = False
        Me.m_sketchpad.ShowYMark = False
        Me.m_sketchpad.Size = New System.Drawing.Size(348, 240)
        Me.m_sketchpad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
        Me.m_sketchpad.TabIndex = 9
        Me.m_sketchpad.UIContext = Nothing
        Me.m_sketchpad.XAxisMaxValue = -9999
        Me.m_sketchpad.XMarkValue = -9999.0!
        Me.m_sketchpad.YAxisAutoScaleMode = ScientificInterfaceShared.Definitions.eAxisAutoScaleModeTypes.[Auto]
        Me.m_sketchpad.YAxisMaxValue = 0.0!
        Me.m_sketchpad.YAxisMinValue = -9999.0!
        Me.m_sketchpad.YMarkLabel = ""
        Me.m_sketchpad.YMarkValue = -9999.0!
        '
        'm_btnAddPredefined
        '
        Me.m_btnAddPredefined.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnAddPredefined.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_btnAddPredefined.Location = New System.Drawing.Point(355, 28)
        Me.m_btnAddPredefined.Name = "m_btnAddPredefined"
        Me.m_btnAddPredefined.Size = New System.Drawing.Size(24, 22)
        Me.m_btnAddPredefined.TabIndex = 5
        Me.m_btnAddPredefined.Text = "+"
        '
        'm_btnDeletePredefined
        '
        Me.m_btnDeletePredefined.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnDeletePredefined.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_btnDeletePredefined.Location = New System.Drawing.Point(355, 4)
        Me.m_btnDeletePredefined.Name = "m_btnDeletePredefined"
        Me.m_btnDeletePredefined.Size = New System.Drawing.Size(24, 21)
        Me.m_btnDeletePredefined.TabIndex = 2
        Me.m_btnDeletePredefined.Text = "x"
        '
        'm_tbxFunctionName
        '
        Me.m_tbxFunctionName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFunctionName.Location = New System.Drawing.Point(73, 30)
        Me.m_tbxFunctionName.Name = "m_tbxFunctionName"
        Me.m_tbxFunctionName.Size = New System.Drawing.Size(278, 20)
        Me.m_tbxFunctionName.TabIndex = 4
        '
        'm_lblFunction
        '
        Me.m_lblFunction.AutoSize = True
        Me.m_lblFunction.Location = New System.Drawing.Point(3, 33)
        Me.m_lblFunction.Name = "m_lblFunction"
        Me.m_lblFunction.Size = New System.Drawing.Size(51, 13)
        Me.m_lblFunction.TabIndex = 3
        Me.m_lblFunction.Text = "&Function:"
        '
        'm_tcContent
        '
        Me.m_tcContent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tcContent.Controls.Add(Me.m_tpCalcs)
        Me.m_tcContent.Controls.Add(Me.m_tpAcknowledgements)
        Me.m_tcContent.Location = New System.Drawing.Point(3, 54)
        Me.m_tcContent.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tcContent.Name = "m_tcContent"
        Me.m_tcContent.SelectedIndex = 0
        Me.m_tcContent.Size = New System.Drawing.Size(520, 381)
        Me.m_tcContent.TabIndex = 5
        '
        'm_tpCalcs
        '
        Me.m_tpCalcs.Controls.Add(Me.m_scContent)
        Me.m_tpCalcs.Location = New System.Drawing.Point(4, 22)
        Me.m_tpCalcs.Name = "m_tpCalcs"
        Me.m_tpCalcs.Padding = New System.Windows.Forms.Padding(3, 3, 3, 3)
        Me.m_tpCalcs.Size = New System.Drawing.Size(512, 355)
        Me.m_tpCalcs.TabIndex = 1
        Me.m_tpCalcs.Text = "Complexity calculations"
        Me.m_tpCalcs.UseVisualStyleBackColor = True
        '
        'm_tpAcknowledgements
        '
        Me.m_tpAcknowledgements.Controls.Add(Me.m_flpLogos)
        Me.m_tpAcknowledgements.Location = New System.Drawing.Point(4, 22)
        Me.m_tpAcknowledgements.Name = "m_tpAcknowledgements"
        Me.m_tpAcknowledgements.Padding = New System.Windows.Forms.Padding(3, 3, 3, 3)
        Me.m_tpAcknowledgements.Size = New System.Drawing.Size(512, 355)
        Me.m_tpAcknowledgements.TabIndex = 0
        Me.m_tpAcknowledgements.Text = "Acknowledgements"
        Me.m_tpAcknowledgements.UseVisualStyleBackColor = True
        '
        'm_flpLogos
        '
        Me.m_flpLogos.Controls.Add(Me.m_pbMare)
        Me.m_flpLogos.Controls.Add(Me.m_pbEII)
        Me.m_flpLogos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_flpLogos.Location = New System.Drawing.Point(3, 3)
        Me.m_flpLogos.Margin = New System.Windows.Forms.Padding(0)
        Me.m_flpLogos.Name = "m_flpLogos"
        Me.m_flpLogos.Size = New System.Drawing.Size(506, 349)
        Me.m_flpLogos.TabIndex = 0
        '
        'm_pbEII
        '
        Me.m_pbEII.BackgroundImage = Global.EcoEngineersPlugin.My.Resources.Resources.EII
        Me.m_pbEII.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.m_pbEII.Location = New System.Drawing.Point(230, 10)
        Me.m_pbEII.Margin = New System.Windows.Forms.Padding(10)
        Me.m_pbEII.Name = "m_pbEII"
        Me.m_pbEII.Size = New System.Drawing.Size(200, 68)
        Me.m_pbEII.TabIndex = 0
        Me.m_pbEII.TabStop = False
        '
        'm_pbMare
        '
        Me.m_pbMare.BackgroundImage = Global.EcoEngineersPlugin.My.Resources.Resources.Ma_Re
        Me.m_pbMare.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.m_pbMare.Location = New System.Drawing.Point(10, 10)
        Me.m_pbMare.Margin = New System.Windows.Forms.Padding(10)
        Me.m_pbMare.Name = "m_pbMare"
        Me.m_pbMare.Size = New System.Drawing.Size(200, 122)
        Me.m_pbMare.TabIndex = 0
        Me.m_pbMare.TabStop = False
        '
        'ucEcoEngineerConfigUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tcContent)
        Me.Controls.Add(Me.m_tbxDescription)
        Me.Controls.Add(Me.m_tbxName)
        Me.Controls.Add(Me.m_lblDescription)
        Me.Controls.Add(Me.m_lblName)
        Me.Name = "ucEcoEngineerConfigUI"
        Me.Size = New System.Drawing.Size(526, 435)
        Me.m_tlpFormula.ResumeLayout(False)
        Me.m_tlpFormula.PerformLayout()
        Me.m_scContent.Panel1.ResumeLayout(False)
        Me.m_scContent.Panel2.ResumeLayout(False)
        Me.m_scContent.Panel2.PerformLayout()
        CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scContent.ResumeLayout(False)
        Me.m_tcContent.ResumeLayout(False)
        Me.m_tpCalcs.ResumeLayout(False)
        Me.m_tpAcknowledgements.ResumeLayout(False)
        Me.m_flpLogos.ResumeLayout(False)
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbMare, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_clbGroups As System.Windows.Forms.CheckedListBox
    Private WithEvents m_tbxA As System.Windows.Forms.TextBox
    Private WithEvents m_lblX2 As System.Windows.Forms.Label
    Private WithEvents m_tbxB As System.Windows.Forms.TextBox
    Private WithEvents m_lblX As System.Windows.Forms.Label
    Private WithEvents m_tbxC As System.Windows.Forms.TextBox
    Private WithEvents m_lblName As System.Windows.Forms.Label
    Private WithEvents m_tbxName As System.Windows.Forms.TextBox
    Private WithEvents m_lblDescription As System.Windows.Forms.Label
    Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
    Private WithEvents m_tlpFormula As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblDefaults As System.Windows.Forms.Label
    Private WithEvents m_cmbPreDefinedRules As System.Windows.Forms.ComboBox
    Private WithEvents m_lblXAxis As System.Windows.Forms.Label
    Private WithEvents m_lblYAxis As System.Windows.Forms.Label
    Private WithEvents m_lblFormula As System.Windows.Forms.Label
    Private WithEvents m_scContent As System.Windows.Forms.SplitContainer
    Private WithEvents m_tbxFunctionName As System.Windows.Forms.TextBox
    Private WithEvents m_lblFunction As System.Windows.Forms.Label
    Private WithEvents m_btnDeletePredefined As System.Windows.Forms.Button
    Private WithEvents m_btnAddPredefined As System.Windows.Forms.Button
    Private WithEvents m_sketchpad As ucEcoEngineerSketchPad
    Private WithEvents m_tcContent As System.Windows.Forms.TabControl
    Private WithEvents m_tpCalcs As System.Windows.Forms.TabPage
    Private WithEvents m_tpAcknowledgements As System.Windows.Forms.TabPage
    Private WithEvents m_flpLogos As System.Windows.Forms.FlowLayoutPanel
    Private WithEvents m_pbEII As System.Windows.Forms.PictureBox
    Private WithEvents m_pbMare As System.Windows.Forms.PictureBox

End Class
