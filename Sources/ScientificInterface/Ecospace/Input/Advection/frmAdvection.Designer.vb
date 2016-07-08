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

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls.Map

Namespace Ecospace.Advection

    Partial Class frmAdvection
        Inherits frmEwE

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAdvection))
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_btPhysicsModel = New System.Windows.Forms.Button()
            Me.m_btnEditWind = New System.Windows.Forms.Button()
            Me.m_tlpComputeControls = New System.Windows.Forms.TableLayoutPanel()
            Me.m_btnStart = New System.Windows.Forms.Button()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_btnRevert = New System.Windows.Forms.Button()
            Me.m_nudWind = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblWind = New System.Windows.Forms.Label()
            Me.m_lblCursor = New System.Windows.Forms.Label()
            Me.m_sliderCursor = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_hdrEditing = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrCompute = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tsControls = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsmiToggleOptions = New System.Windows.Forms.ToolStripButton()
            Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tslMonth = New System.Windows.Forms.ToolStripLabel()
            Me.m_tscmMonth = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tsbtCopyMonth = New System.Windows.Forms.ToolStripButton()
            Me.m_ucZoomToolbar = New ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar()
            Me.m_scMaps = New System.Windows.Forms.SplitContainer()
            Me.m_scOutputMaps = New System.Windows.Forms.SplitContainer()
            Me.m_ucWind = New ScientificInterface.Ecospace.Advection.ucWind()
            Me.m_ucMap = New ScientificInterface.Ecospace.Advection.ucMap()
            Me.m_ucUpwelling = New ScientificInterface.Ecospace.Advection.ucUpwelling()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpComputeControls.SuspendLayout()
            CType(Me.m_nudWind, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tsControls.SuspendLayout()
            CType(Me.m_scMaps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMaps.Panel1.SuspendLayout()
            Me.m_scMaps.Panel2.SuspendLayout()
            Me.m_scMaps.SuspendLayout()
            CType(Me.m_scOutputMaps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scOutputMaps.Panel1.SuspendLayout()
            Me.m_scOutputMaps.Panel2.SuspendLayout()
            Me.m_scOutputMaps.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_scMain.Location = New System.Drawing.Point(3, 31)
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_btPhysicsModel)
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnEditWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_tlpComputeControls)
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnRevert)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblCursor)
            Me.m_scMain.Panel1.Controls.Add(Me.m_sliderCursor)
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrEditing)
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrCompute)
            Me.m_scMain.Panel1MinSize = 190
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_scMaps)
            Me.m_scMain.Size = New System.Drawing.Size(1080, 636)
            Me.m_scMain.SplitterDistance = 257
            Me.m_scMain.TabIndex = 0
            '
            'm_btPhysicsModel
            '
            Me.m_btPhysicsModel.Location = New System.Drawing.Point(17, 363)
            Me.m_btPhysicsModel.Name = "m_btPhysicsModel"
            Me.m_btPhysicsModel.Size = New System.Drawing.Size(139, 23)
            Me.m_btPhysicsModel.TabIndex = 2
            Me.m_btPhysicsModel.Text = "Run new Physics Model"
            Me.m_btPhysicsModel.UseVisualStyleBackColor = True
            '
            'm_btnEditWind
            '
            Me.m_btnEditWind.Location = New System.Drawing.Point(139, 132)
            Me.m_btnEditWind.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.m_btnEditWind.Name = "m_btnEditWind"
            Me.m_btnEditWind.Size = New System.Drawing.Size(50, 23)
            Me.m_btnEditWind.TabIndex = 16
            Me.m_btnEditWind.Text = "&Edit..."
            Me.m_btnEditWind.UseVisualStyleBackColor = True
            '
            'm_tlpComputeControls
            '
            Me.m_tlpComputeControls.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpComputeControls.ColumnCount = 2
            Me.m_tlpComputeControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.Controls.Add(Me.m_btnStart, 0, 0)
            Me.m_tlpComputeControls.Controls.Add(Me.m_btnStop, 1, 0)
            Me.m_tlpComputeControls.Location = New System.Drawing.Point(1, 21)
            Me.m_tlpComputeControls.Name = "m_tlpComputeControls"
            Me.m_tlpComputeControls.RowCount = 1
            Me.m_tlpComputeControls.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.Size = New System.Drawing.Size(256, 27)
            Me.m_tlpComputeControls.TabIndex = 9
            '
            'm_btnStart
            '
            Me.m_btnStart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStart.Location = New System.Drawing.Point(0, 0)
            Me.m_btnStart.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.m_btnStart.Name = "m_btnStart"
            Me.m_btnStart.Size = New System.Drawing.Size(125, 23)
            Me.m_btnStart.TabIndex = 0
            Me.m_btnStart.Text = "&Compute"
            Me.m_btnStart.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.Location = New System.Drawing.Point(131, 0)
            Me.m_btnStop.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(125, 23)
            Me.m_btnStop.TabIndex = 1
            Me.m_btnStop.Text = "&Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_btnRevert
            '
            Me.m_btnRevert.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnRevert.Location = New System.Drawing.Point(1, 51)
            Me.m_btnRevert.Margin = New System.Windows.Forms.Padding(0)
            Me.m_btnRevert.Name = "m_btnRevert"
            Me.m_btnRevert.Size = New System.Drawing.Size(256, 27)
            Me.m_btnRevert.TabIndex = 10
            Me.m_btnRevert.Text = "&Revert"
            Me.m_btnRevert.UseVisualStyleBackColor = True
            '
            'm_nudWind
            '
            Me.m_nudWind.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudWind.Location = New System.Drawing.Point(55, 134)
            Me.m_nudWind.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudWind.Name = "m_nudWind"
            Me.m_nudWind.Size = New System.Drawing.Size(78, 20)
            Me.m_nudWind.TabIndex = 15
            Me.m_nudWind.ThousandsSeparator = True
            Me.m_nudWind.Value = New Decimal(New Integer() {25, 0, 0, 0})
            '
            'm_lblWind
            '
            Me.m_lblWind.AutoSize = True
            Me.m_lblWind.Location = New System.Drawing.Point(4, 136)
            Me.m_lblWind.Name = "m_lblWind"
            Me.m_lblWind.Size = New System.Drawing.Size(35, 13)
            Me.m_lblWind.TabIndex = 14
            Me.m_lblWind.Text = "&Wind:"
            '
            'm_lblCursor
            '
            Me.m_lblCursor.AutoSize = True
            Me.m_lblCursor.Location = New System.Drawing.Point(4, 111)
            Me.m_lblCursor.Name = "m_lblCursor"
            Me.m_lblCursor.Size = New System.Drawing.Size(40, 13)
            Me.m_lblCursor.TabIndex = 12
            Me.m_lblCursor.Text = "&Cursor:"
            '
            'm_sliderCursor
            '
            Me.m_sliderCursor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sliderCursor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_sliderCursor.CurrentKnob = 0
            Me.m_sliderCursor.Location = New System.Drawing.Point(55, 108)
            Me.m_sliderCursor.Maximum = 5
            Me.m_sliderCursor.Minimum = 1
            Me.m_sliderCursor.Name = "m_sliderCursor"
            Me.m_sliderCursor.NumKnobs = 1
            Me.m_sliderCursor.Size = New System.Drawing.Size(200, 20)
            Me.m_sliderCursor.TabIndex = 13
            '
            'm_hdrEditing
            '
            Me.m_hdrEditing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrEditing.CanCollapseParent = False
            Me.m_hdrEditing.CollapsedParentHeight = 0
            Me.m_hdrEditing.IsCollapsed = False
            Me.m_hdrEditing.Location = New System.Drawing.Point(1, 87)
            Me.m_hdrEditing.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrEditing.Name = "m_hdrEditing"
            Me.m_hdrEditing.Size = New System.Drawing.Size(257, 18)
            Me.m_hdrEditing.TabIndex = 11
            Me.m_hdrEditing.Text = "Editing"
            Me.m_hdrEditing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_hdrCompute
            '
            Me.m_hdrCompute.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrCompute.CanCollapseParent = False
            Me.m_hdrCompute.CollapsedParentHeight = 0
            Me.m_hdrCompute.IsCollapsed = False
            Me.m_hdrCompute.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrCompute.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrCompute.Name = "m_hdrCompute"
            Me.m_hdrCompute.Size = New System.Drawing.Size(257, 18)
            Me.m_hdrCompute.TabIndex = 0
            Me.m_hdrCompute.Text = "Compute advection velocities"
            Me.m_hdrCompute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsControls
            '
            Me.m_tsControls.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tsControls.AutoSize = False
            Me.m_tsControls.Dock = System.Windows.Forms.DockStyle.None
            Me.m_tsControls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsControls.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiToggleOptions, Me.m_sep1, Me.m_tslMonth, Me.m_tscmMonth, Me.m_tsbtCopyMonth})
            Me.m_tsControls.Location = New System.Drawing.Point(3, 3)
            Me.m_tsControls.Name = "m_tsControls"
            Me.m_tsControls.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_tsControls.Size = New System.Drawing.Size(767, 25)
            Me.m_tsControls.TabIndex = 0
            Me.m_tsControls.Text = "ToolStrip1"
            '
            'm_tsmiToggleOptions
            '
            Me.m_tsmiToggleOptions.CheckOnClick = True
            Me.m_tsmiToggleOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsmiToggleOptions.Image = CType(resources.GetObject("m_tsmiToggleOptions.Image"), System.Drawing.Image)
            Me.m_tsmiToggleOptions.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsmiToggleOptions.Name = "m_tsmiToggleOptions"
            Me.m_tsmiToggleOptions.Size = New System.Drawing.Size(83, 22)
            Me.m_tsmiToggleOptions.Text = "Show options"
            '
            'm_sep1
            '
            Me.m_sep1.Name = "m_sep1"
            Me.m_sep1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tslMonth
            '
            Me.m_tslMonth.Name = "m_tslMonth"
            Me.m_tslMonth.Size = New System.Drawing.Size(78, 22)
            Me.m_tslMonth.Text = "Show month:"
            '
            'm_tscmMonth
            '
            Me.m_tscmMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmMonth.Name = "m_tscmMonth"
            Me.m_tscmMonth.Size = New System.Drawing.Size(75, 25)
            '
            'm_tsbtCopyMonth
            '
            Me.m_tsbtCopyMonth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbtCopyMonth.Image = CType(resources.GetObject("m_tsbtCopyMonth.Image"), System.Drawing.Image)
            Me.m_tsbtCopyMonth.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbtCopyMonth.Name = "m_tsbtCopyMonth"
            Me.m_tsbtCopyMonth.Size = New System.Drawing.Size(119, 22)
            Me.m_tsbtCopyMonth.Text = "Copy current month"
            Me.m_tsbtCopyMonth.ToolTipText = "Copy wind pattern from current month to all months"
            '
            'm_ucZoomToolbar
            '
            Me.m_ucZoomToolbar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_ucZoomToolbar.AutoSize = True
            Me.m_ucZoomToolbar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucZoomToolbar.Location = New System.Drawing.Point(983, 3)
            Me.m_ucZoomToolbar.MinimumSize = New System.Drawing.Size(100, 25)
            Me.m_ucZoomToolbar.Name = "m_ucZoomToolbar"
            Me.m_ucZoomToolbar.PositionMode = ScientificInterfaceShared.Controls.Map.ucMapZoom.ePositionModeTypes.Center
            Me.m_ucZoomToolbar.Size = New System.Drawing.Size(100, 27)
            Me.m_ucZoomToolbar.TabIndex = 1
            Me.m_ucZoomToolbar.UIContext = Nothing
            '
            'm_scMaps
            '
            Me.m_scMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_scMaps.Name = "m_scMaps"
            Me.m_scMaps.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scMaps.Panel1
            '
            Me.m_scMaps.Panel1.Controls.Add(Me.m_ucWind)
            '
            'm_scMaps.Panel2
            '
            Me.m_scMaps.Panel2.Controls.Add(Me.m_scOutputMaps)
            Me.m_scMaps.Size = New System.Drawing.Size(819, 636)
            Me.m_scMaps.SplitterDistance = 273
            Me.m_scMaps.TabIndex = 0
            '
            'm_scOutputMaps
            '
            Me.m_scOutputMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scOutputMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_scOutputMaps.Name = "m_scOutputMaps"
            '
            'm_scOutputMaps.Panel1
            '
            Me.m_scOutputMaps.Panel1.Controls.Add(Me.m_ucMap)
            '
            'm_scOutputMaps.Panel2
            '
            Me.m_scOutputMaps.Panel2.Controls.Add(Me.m_ucUpwelling)
            Me.m_scOutputMaps.Size = New System.Drawing.Size(819, 359)
            Me.m_scOutputMaps.SplitterDistance = 402
            Me.m_scOutputMaps.TabIndex = 0
            '
            'm_ucWind
            '
            Me.m_ucWind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucWind.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucWind.Location = New System.Drawing.Point(0, 0)
            Me.m_ucWind.Margin = New System.Windows.Forms.Padding(3, 0, 0, 3)
            Me.m_ucWind.Name = "m_ucWind"
            Me.m_ucWind.Size = New System.Drawing.Size(819, 273)
            Me.m_ucWind.TabIndex = 1
            Me.m_ucWind.UIContext = Nothing
            '
            'm_ucMap
            '
            Me.m_ucMap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMap.Location = New System.Drawing.Point(0, 0)
            Me.m_ucMap.Margin = New System.Windows.Forms.Padding(0, 0, 3, 3)
            Me.m_ucMap.Name = "m_ucMap"
            Me.m_ucMap.Size = New System.Drawing.Size(402, 359)
            Me.m_ucMap.TabIndex = 0
            Me.m_ucMap.UIContext = Nothing
            '
            'm_ucUpwelling
            '
            Me.m_ucUpwelling.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucUpwelling.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucUpwelling.Location = New System.Drawing.Point(0, 0)
            Me.m_ucUpwelling.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
            Me.m_ucUpwelling.Name = "m_ucUpwelling"
            Me.m_ucUpwelling.Size = New System.Drawing.Size(413, 359)
            Me.m_ucUpwelling.TabIndex = 3
            Me.m_ucUpwelling.UIContext = Nothing
            '
            'frmAdvection
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.ClientSize = New System.Drawing.Size(1086, 670)
            Me.Controls.Add(Me.m_ucZoomToolbar)
            Me.Controls.Add(Me.m_tsControls)
            Me.Controls.Add(Me.m_scMain)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmAdvection"
            Me.Padding = New System.Windows.Forms.Padding(3)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.TabText = ""
            Me.Text = "Advection"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_tlpComputeControls.ResumeLayout(False)
            CType(Me.m_nudWind, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tsControls.ResumeLayout(False)
            Me.m_tsControls.PerformLayout()
            Me.m_scMaps.Panel1.ResumeLayout(False)
            Me.m_scMaps.Panel2.ResumeLayout(False)
            CType(Me.m_scMaps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMaps.ResumeLayout(False)
            Me.m_scOutputMaps.Panel1.ResumeLayout(False)
            Me.m_scOutputMaps.Panel2.ResumeLayout(False)
            CType(Me.m_scOutputMaps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scOutputMaps.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_hdrCompute As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        'Private WithEvents m_ucMLD As ScientificInterface.Ecospace.Advection.ucMLD
        Private WithEvents m_ucUpwelling As ScientificInterface.Ecospace.Advection.ucUpwelling
        Private WithEvents m_ucWind As ScientificInterface.Ecospace.Advection.ucWind
        Private WithEvents m_ucMap As ScientificInterface.Ecospace.Advection.ucMap
        Private WithEvents m_tsControls As cEwEToolstrip
        Private WithEvents m_tsmiToggleOptions As System.Windows.Forms.ToolStripButton
        Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tslMonth As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tscmMonth As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_ucZoomToolbar As ucMapZoomToolbar
        Private WithEvents m_lblCursor As System.Windows.Forms.Label
        Private WithEvents m_sliderCursor As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_lblWind As System.Windows.Forms.Label
        Private WithEvents m_hdrEditing As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnStart As System.Windows.Forms.Button
        Private WithEvents m_tlpComputeControls As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_btnRevert As System.Windows.Forms.Button
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents m_nudWind As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_btnEditWind As System.Windows.Forms.Button
        Friend WithEvents m_btPhysicsModel As System.Windows.Forms.Button
        Friend WithEvents m_tsbtCopyMonth As System.Windows.Forms.ToolStripButton
        Friend WithEvents m_scMaps As SplitContainer
        Friend WithEvents m_scOutputMaps As SplitContainer
    End Class

End Namespace
