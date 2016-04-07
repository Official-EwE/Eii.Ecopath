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
            Me.m_tlpMaps = New System.Windows.Forms.TableLayoutPanel()
            Me.m_ucWind = New ScientificInterface.Ecospace.Advection.ucWind()
            Me.m_ucMLD = New ScientificInterface.Ecospace.Advection.ucMLD()
            Me.m_ucMap = New ScientificInterface.Ecospace.Advection.ucMap()
            Me.m_ucUpwelling = New ScientificInterface.Ecospace.Advection.ucUpwelling()
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_btnEditUpwelling = New System.Windows.Forms.Button()
            Me.m_btnEditMLD = New System.Windows.Forms.Button()
            Me.m_btnEditWind = New System.Windows.Forms.Button()
            Me.m_nudYVelocity = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblYVelocity = New System.Windows.Forms.Label()
            Me.m_nudXVelocity = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblXVelocity = New System.Windows.Forms.Label()
            Me.m_tlpComputeControls = New System.Windows.Forms.TableLayoutPanel()
            Me.m_btnStart = New System.Windows.Forms.Button()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_btnRevert = New System.Windows.Forms.Button()
            Me.m_nudSorWv = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblSorWv = New System.Windows.Forms.Label()
            Me.m_nudCoriolis = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblCoriolis = New System.Windows.Forms.Label()
            Me.m_nudUpwell = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudMLD = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblUpwelling = New System.Windows.Forms.Label()
            Me.m_lblDepth = New System.Windows.Forms.Label()
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
            Me.m_ucZoomToolbar = New ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar()
            Me.m_tlpMaps.SuspendLayout()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            CType(Me.m_nudYVelocity, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudXVelocity, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpComputeControls.SuspendLayout()
            CType(Me.m_nudSorWv, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudCoriolis, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudUpwell, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMLD, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudWind, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tsControls.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpMaps
            '
            Me.m_tlpMaps.ColumnCount = 2
            Me.m_tlpMaps.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.Controls.Add(Me.m_ucWind, 1, 0)
            Me.m_tlpMaps.Controls.Add(Me.m_ucMLD, 0, 1)
            Me.m_tlpMaps.Controls.Add(Me.m_ucMap, 0, 0)
            Me.m_tlpMaps.Controls.Add(Me.m_ucUpwelling, 1, 1)
            Me.m_tlpMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpMaps.Name = "m_tlpMaps"
            Me.m_tlpMaps.RowCount = 2
            Me.m_tlpMaps.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.Size = New System.Drawing.Size(604, 573)
            Me.m_tlpMaps.TabIndex = 0
            '
            'm_ucWind
            '
            Me.m_ucWind.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucWind.Location = New System.Drawing.Point(305, 0)
            Me.m_ucWind.Margin = New System.Windows.Forms.Padding(3, 0, 0, 3)
            Me.m_ucWind.Name = "m_ucWind"
            Me.m_ucWind.Size = New System.Drawing.Size(299, 283)
            Me.m_ucWind.TabIndex = 1
            Me.m_ucWind.UIContext = Nothing
            '
            'm_ucMLD
            '
            Me.m_ucMLD.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMLD.Location = New System.Drawing.Point(0, 289)
            Me.m_ucMLD.Margin = New System.Windows.Forms.Padding(0, 3, 3, 0)
            Me.m_ucMLD.Name = "m_ucMLD"
            Me.m_ucMLD.Size = New System.Drawing.Size(299, 284)
            Me.m_ucMLD.TabIndex = 2
            Me.m_ucMLD.UIContext = Nothing
            '
            'm_ucMap
            '
            Me.m_ucMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMap.Location = New System.Drawing.Point(0, 0)
            Me.m_ucMap.Margin = New System.Windows.Forms.Padding(0, 0, 3, 3)
            Me.m_ucMap.Name = "m_ucMap"
            Me.m_ucMap.Size = New System.Drawing.Size(299, 283)
            Me.m_ucMap.TabIndex = 0
            Me.m_ucMap.UIContext = Nothing
            '
            'm_ucUpwelling
            '
            Me.m_ucUpwelling.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucUpwelling.Location = New System.Drawing.Point(305, 289)
            Me.m_ucUpwelling.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
            Me.m_ucUpwelling.Name = "m_ucUpwelling"
            Me.m_ucUpwelling.Size = New System.Drawing.Size(299, 284)
            Me.m_ucUpwelling.TabIndex = 3
            Me.m_ucUpwelling.UIContext = Nothing
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
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnEditUpwelling)
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnEditMLD)
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnEditWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudYVelocity)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblYVelocity)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudXVelocity)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblXVelocity)
            Me.m_scMain.Panel1.Controls.Add(Me.m_tlpComputeControls)
            Me.m_scMain.Panel1.Controls.Add(Me.m_btnRevert)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudSorWv)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblSorWv)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudCoriolis)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblCoriolis)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudUpwell)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudMLD)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblUpwelling)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDepth)
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
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpMaps)
            Me.m_scMain.Size = New System.Drawing.Size(798, 573)
            Me.m_scMain.SplitterDistance = 190
            Me.m_scMain.TabIndex = 0
            '
            'm_btnEditUpwelling
            '
            Me.m_btnEditUpwelling.Location = New System.Drawing.Point(139, 300)
            Me.m_btnEditUpwelling.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.m_btnEditUpwelling.Name = "m_btnEditUpwelling"
            Me.m_btnEditUpwelling.Size = New System.Drawing.Size(50, 23)
            Me.m_btnEditUpwelling.TabIndex = 22
            Me.m_btnEditUpwelling.Text = "&Edit..."
            Me.m_btnEditUpwelling.UseVisualStyleBackColor = True
            '
            'm_btnEditMLD
            '
            Me.m_btnEditMLD.Location = New System.Drawing.Point(139, 271)
            Me.m_btnEditMLD.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.m_btnEditMLD.Name = "m_btnEditMLD"
            Me.m_btnEditMLD.Size = New System.Drawing.Size(50, 23)
            Me.m_btnEditMLD.TabIndex = 19
            Me.m_btnEditMLD.Text = "&Edit..."
            Me.m_btnEditMLD.UseVisualStyleBackColor = True
            '
            'm_btnEditWind
            '
            Me.m_btnEditWind.Location = New System.Drawing.Point(139, 242)
            Me.m_btnEditWind.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.m_btnEditWind.Name = "m_btnEditWind"
            Me.m_btnEditWind.Size = New System.Drawing.Size(50, 23)
            Me.m_btnEditWind.TabIndex = 16
            Me.m_btnEditWind.Text = "&Edit..."
            Me.m_btnEditWind.UseVisualStyleBackColor = True
            '
            'm_nudYVelocity
            '
            Me.m_nudYVelocity.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudYVelocity.Location = New System.Drawing.Point(54, 50)
            Me.m_nudYVelocity.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudYVelocity.Minimum = New Decimal(New Integer() {1000, 0, 0, -2147483648})
            Me.m_nudYVelocity.Name = "m_nudYVelocity"
            Me.m_nudYVelocity.Size = New System.Drawing.Size(78, 20)
            Me.m_nudYVelocity.TabIndex = 4
            '
            'm_lblYVelocity
            '
            Me.m_lblYVelocity.AutoSize = True
            Me.m_lblYVelocity.Location = New System.Drawing.Point(3, 52)
            Me.m_lblYVelocity.Name = "m_lblYVelocity"
            Me.m_lblYVelocity.Size = New System.Drawing.Size(35, 13)
            Me.m_lblYVelocity.TabIndex = 3
            Me.m_lblYVelocity.Text = "Vel &Y:"
            '
            'm_nudXVelocity
            '
            Me.m_nudXVelocity.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudXVelocity.Location = New System.Drawing.Point(54, 24)
            Me.m_nudXVelocity.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudXVelocity.Minimum = New Decimal(New Integer() {1000, 0, 0, -2147483648})
            Me.m_nudXVelocity.Name = "m_nudXVelocity"
            Me.m_nudXVelocity.Size = New System.Drawing.Size(78, 20)
            Me.m_nudXVelocity.TabIndex = 2
            '
            'm_lblXVelocity
            '
            Me.m_lblXVelocity.AutoSize = True
            Me.m_lblXVelocity.Location = New System.Drawing.Point(3, 26)
            Me.m_lblXVelocity.Name = "m_lblXVelocity"
            Me.m_lblXVelocity.Size = New System.Drawing.Size(35, 13)
            Me.m_lblXVelocity.TabIndex = 1
            Me.m_lblXVelocity.Text = "Vel &X:"
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
            Me.m_tlpComputeControls.Location = New System.Drawing.Point(1, 137)
            Me.m_tlpComputeControls.Name = "m_tlpComputeControls"
            Me.m_tlpComputeControls.RowCount = 1
            Me.m_tlpComputeControls.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.Size = New System.Drawing.Size(189, 23)
            Me.m_tlpComputeControls.TabIndex = 9
            '
            'm_btnStart
            '
            Me.m_btnStart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStart.Location = New System.Drawing.Point(0, 0)
            Me.m_btnStart.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.m_btnStart.Name = "m_btnStart"
            Me.m_btnStart.Size = New System.Drawing.Size(91, 23)
            Me.m_btnStart.TabIndex = 0
            Me.m_btnStart.Text = "&Compute"
            Me.m_btnStart.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.Location = New System.Drawing.Point(97, 0)
            Me.m_btnStop.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(92, 23)
            Me.m_btnStop.TabIndex = 1
            Me.m_btnStop.Text = "&Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_btnRevert
            '
            Me.m_btnRevert.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnRevert.Location = New System.Drawing.Point(1, 163)
            Me.m_btnRevert.Margin = New System.Windows.Forms.Padding(0)
            Me.m_btnRevert.Name = "m_btnRevert"
            Me.m_btnRevert.Size = New System.Drawing.Size(189, 23)
            Me.m_btnRevert.TabIndex = 10
            Me.m_btnRevert.Text = "&Revert"
            Me.m_btnRevert.UseVisualStyleBackColor = True
            '
            'm_nudSorWv
            '
            Me.m_nudSorWv.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
            Me.m_nudSorWv.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudSorWv.Location = New System.Drawing.Point(55, 102)
            Me.m_nudSorWv.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSorWv.Minimum = New Decimal(New Integer() {1, 0, 0, -2147483648})
            Me.m_nudSorWv.Name = "m_nudSorWv"
            Me.m_nudSorWv.Size = New System.Drawing.Size(78, 20)
            Me.m_nudSorWv.TabIndex = 8
            Me.m_nudSorWv.Value = New Decimal(New Integer() {5, 0, 0, 65536})
            '
            'm_lblSorWv
            '
            Me.m_lblSorWv.AutoSize = True
            Me.m_lblSorWv.Location = New System.Drawing.Point(4, 104)
            Me.m_lblSorWv.Name = "m_lblSorWv"
            Me.m_lblSorWv.Size = New System.Drawing.Size(43, 13)
            Me.m_lblSorWv.TabIndex = 7
            Me.m_lblSorWv.Text = "&SorWv:"
            '
            'm_nudCoriolis
            '
            Me.m_nudCoriolis.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
            Me.m_nudCoriolis.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudCoriolis.Location = New System.Drawing.Point(55, 76)
            Me.m_nudCoriolis.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudCoriolis.Minimum = New Decimal(New Integer() {1, 0, 0, -2147483648})
            Me.m_nudCoriolis.Name = "m_nudCoriolis"
            Me.m_nudCoriolis.Size = New System.Drawing.Size(78, 20)
            Me.m_nudCoriolis.TabIndex = 6
            Me.m_nudCoriolis.Value = New Decimal(New Integer() {5, 0, 0, 65536})
            '
            'm_lblCoriolis
            '
            Me.m_lblCoriolis.AutoSize = True
            Me.m_lblCoriolis.Location = New System.Drawing.Point(4, 78)
            Me.m_lblCoriolis.Name = "m_lblCoriolis"
            Me.m_lblCoriolis.Size = New System.Drawing.Size(43, 13)
            Me.m_lblCoriolis.TabIndex = 5
            Me.m_lblCoriolis.Text = "&Coriolis:"
            '
            'm_nudUpwell
            '
            Me.m_nudUpwell.Increment = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudUpwell.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudUpwell.Location = New System.Drawing.Point(54, 302)
            Me.m_nudUpwell.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudUpwell.Minimum = New Decimal(New Integer() {1000, 0, 0, -2147483648})
            Me.m_nudUpwell.Name = "m_nudUpwell"
            Me.m_nudUpwell.Size = New System.Drawing.Size(78, 20)
            Me.m_nudUpwell.TabIndex = 21
            Me.m_nudUpwell.ThousandsSeparator = True
            '
            'm_nudMLD
            '
            Me.m_nudMLD.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudMLD.Location = New System.Drawing.Point(55, 273)
            Me.m_nudMLD.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudMLD.Name = "m_nudMLD"
            Me.m_nudMLD.Size = New System.Drawing.Size(78, 20)
            Me.m_nudMLD.TabIndex = 18
            Me.m_nudMLD.ThousandsSeparator = True
            Me.m_nudMLD.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_lblUpwelling
            '
            Me.m_lblUpwelling.AutoSize = True
            Me.m_lblUpwelling.Location = New System.Drawing.Point(4, 304)
            Me.m_lblUpwelling.Name = "m_lblUpwelling"
            Me.m_lblUpwelling.Size = New System.Drawing.Size(45, 13)
            Me.m_lblUpwelling.TabIndex = 20
            Me.m_lblUpwelling.Text = "&Upwell.:"
            '
            'm_lblDepth
            '
            Me.m_lblDepth.AutoSize = True
            Me.m_lblDepth.Location = New System.Drawing.Point(5, 275)
            Me.m_lblDepth.Name = "m_lblDepth"
            Me.m_lblDepth.Size = New System.Drawing.Size(33, 13)
            Me.m_lblDepth.TabIndex = 17
            Me.m_lblDepth.Text = "&MLD:"
            '
            'm_nudWind
            '
            Me.m_nudWind.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudWind.Location = New System.Drawing.Point(55, 244)
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
            Me.m_lblWind.Location = New System.Drawing.Point(4, 246)
            Me.m_lblWind.Name = "m_lblWind"
            Me.m_lblWind.Size = New System.Drawing.Size(35, 13)
            Me.m_lblWind.TabIndex = 14
            Me.m_lblWind.Text = "&Wind:"
            '
            'm_lblCursor
            '
            Me.m_lblCursor.AutoSize = True
            Me.m_lblCursor.Location = New System.Drawing.Point(4, 221)
            Me.m_lblCursor.Name = "m_lblCursor"
            Me.m_lblCursor.Size = New System.Drawing.Size(40, 13)
            Me.m_lblCursor.TabIndex = 12
            Me.m_lblCursor.Text = "&Cursor:"
            '
            'm_sliderCursor
            '
            Me.m_sliderCursor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sliderCursor.CurrentKnob = 0
            Me.m_sliderCursor.Location = New System.Drawing.Point(55, 218)
            Me.m_sliderCursor.Maximum = 5
            Me.m_sliderCursor.Minimum = 1
            Me.m_sliderCursor.Name = "m_sliderCursor"
            Me.m_sliderCursor.NumKnobs = 1
            Me.m_sliderCursor.Size = New System.Drawing.Size(133, 20)
            Me.m_sliderCursor.TabIndex = 13
            '
            'm_hdrEditing
            '
            Me.m_hdrEditing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrEditing.CanCollapseParent = False
            Me.m_hdrEditing.CollapsedParentHeight = 0
            Me.m_hdrEditing.IsCollapsed = False
            Me.m_hdrEditing.Location = New System.Drawing.Point(1, 197)
            Me.m_hdrEditing.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrEditing.Name = "m_hdrEditing"
            Me.m_hdrEditing.Size = New System.Drawing.Size(190, 18)
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
            Me.m_hdrCompute.Size = New System.Drawing.Size(190, 18)
            Me.m_hdrCompute.TabIndex = 0
            Me.m_hdrCompute.Text = "Compute velocities"
            Me.m_hdrCompute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsControls
            '
            Me.m_tsControls.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tsControls.AutoSize = False
            Me.m_tsControls.Dock = System.Windows.Forms.DockStyle.None
            Me.m_tsControls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsControls.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiToggleOptions, Me.m_sep1, Me.m_tslMonth, Me.m_tscmMonth})
            Me.m_tsControls.Location = New System.Drawing.Point(3, 3)
            Me.m_tsControls.Name = "m_tsControls"
            Me.m_tsControls.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_tsControls.Size = New System.Drawing.Size(485, 25)
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
            'm_ucZoomToolbar
            '
            Me.m_ucZoomToolbar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_ucZoomToolbar.AutoSize = True
            Me.m_ucZoomToolbar.Location = New System.Drawing.Point(422, 3)
            Me.m_ucZoomToolbar.MinimumSize = New System.Drawing.Size(100, 25)
            Me.m_ucZoomToolbar.Name = "m_ucZoomToolbar"
            Me.m_ucZoomToolbar.PositionMode = ScientificInterfaceShared.Controls.Map.ucMapZoom.ePositionModeTypes.Center
            Me.m_ucZoomToolbar.Size = New System.Drawing.Size(379, 27)
            Me.m_ucZoomToolbar.TabIndex = 1
            Me.m_ucZoomToolbar.UIContext = Nothing
            '
            'frmAdvection
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(804, 607)
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
            Me.m_tlpMaps.ResumeLayout(False)
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            CType(Me.m_nudYVelocity, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudXVelocity, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpComputeControls.ResumeLayout(False)
            CType(Me.m_nudSorWv, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudCoriolis, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudUpwell, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMLD, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudWind, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tsControls.ResumeLayout(False)
            Me.m_tsControls.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tlpMaps As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_hdrCompute As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_ucMLD As ScientificInterface.Ecospace.Advection.ucMLD
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
        Private WithEvents m_lblDepth As System.Windows.Forms.Label
        Private WithEvents m_hdrEditing As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Friend WithEvents m_lblCoriolis As System.Windows.Forms.Label
        Private WithEvents m_btnStart As System.Windows.Forms.Button
        Private WithEvents m_tlpComputeControls As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_btnRevert As System.Windows.Forms.Button
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents m_lblYVelocity As System.Windows.Forms.Label
        Private WithEvents m_lblXVelocity As System.Windows.Forms.Label
        Private WithEvents m_lblSorWv As System.Windows.Forms.Label
        Private WithEvents m_lblUpwelling As System.Windows.Forms.Label
        Private WithEvents m_nudWind As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudMLD As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudCoriolis As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudYVelocity As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudXVelocity As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudSorWv As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudUpwell As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_btnEditUpwelling As System.Windows.Forms.Button
        Private WithEvents m_btnEditMLD As System.Windows.Forms.Button
        Private WithEvents m_btnEditWind As System.Windows.Forms.Button

    End Class

End Namespace
