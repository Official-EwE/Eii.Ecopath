Namespace Ecospace.Advection

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
            Me.m_tlpMaps = New System.Windows.Forms.TableLayoutPanel
            Me.m_ucWind = New ScientificInterface.Ecospace.Advection.ucWind
            Me.m_ucMLD = New ScientificInterface.Ecospace.Advection.ucMLD
            Me.m_ucMap = New ScientificInterface.Ecospace.Advection.ucMap
            Me.m_ucUpwelling = New ScientificInterface.Ecospace.Advection.ucUpwelling
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_nudDepth = New System.Windows.Forms.NumericUpDown
            Me.m_lblDepth = New System.Windows.Forms.Label
            Me.m_nudWind = New System.Windows.Forms.NumericUpDown
            Me.m_lblWind = New System.Windows.Forms.Label
            Me.m_lblCursor = New System.Windows.Forms.Label
            Me.m_sliderCursor = New ScientificInterfaceShared.Controls.ucSlider
            Me.m_hdrEditing = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tsControls = New System.Windows.Forms.ToolStrip
            Me.m_tsmiShowOptions = New System.Windows.Forms.ToolStripButton
            Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tslMonth = New System.Windows.Forms.ToolStripLabel
            Me.m_tscmMonth = New System.Windows.Forms.ToolStripComboBox
            Me.m_ucZoomToolbar = New ScientificInterface.Ecospace.ucMapZoomToolbar
            Me.m_tlpMaps.SuspendLayout()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).BeginInit()
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
            Me.m_tlpMaps.Size = New System.Drawing.Size(626, 573)
            Me.m_tlpMaps.TabIndex = 0
            '
            'm_ucWind
            '
            Me.m_ucWind.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucWind.Location = New System.Drawing.Point(316, 0)
            Me.m_ucWind.MapName = "Wind"
            Me.m_ucWind.Margin = New System.Windows.Forms.Padding(3, 0, 0, 3)
            Me.m_ucWind.Name = "m_ucWind"
            Me.m_ucWind.Size = New System.Drawing.Size(310, 283)
            Me.m_ucWind.TabIndex = 0
            Me.m_ucWind.UIContext = Nothing
            '
            'm_ucMLD
            '
            Me.m_ucMLD.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMLD.Location = New System.Drawing.Point(0, 289)
            Me.m_ucMLD.MapName = "Mixed Layer Depths"
            Me.m_ucMLD.Margin = New System.Windows.Forms.Padding(0, 3, 3, 0)
            Me.m_ucMLD.Name = "m_ucMLD"
            Me.m_ucMLD.Size = New System.Drawing.Size(310, 284)
            Me.m_ucMLD.TabIndex = 1
            Me.m_ucMLD.UIContext = Nothing
            '
            'm_ucMap
            '
            Me.m_ucMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMap.Location = New System.Drawing.Point(0, 0)
            Me.m_ucMap.MapName = "Map"
            Me.m_ucMap.Margin = New System.Windows.Forms.Padding(0, 0, 3, 3)
            Me.m_ucMap.Name = "m_ucMap"
            Me.m_ucMap.Size = New System.Drawing.Size(310, 283)
            Me.m_ucMap.TabIndex = 2
            Me.m_ucMap.UIContext = Nothing
            '
            'm_ucUpwelling
            '
            Me.m_ucUpwelling.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucUpwelling.Location = New System.Drawing.Point(316, 289)
            Me.m_ucUpwelling.MapName = "Upwelling"
            Me.m_ucUpwelling.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
            Me.m_ucUpwelling.Name = "m_ucUpwelling"
            Me.m_ucUpwelling.Size = New System.Drawing.Size(310, 284)
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
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudDepth)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDepth)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblWind)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblCursor)
            Me.m_scMain.Panel1.Controls.Add(Me.m_sliderCursor)
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrEditing)
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrOptions)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpMaps)
            Me.m_scMain.Size = New System.Drawing.Size(798, 573)
            Me.m_scMain.SplitterDistance = 168
            Me.m_scMain.TabIndex = 0
            '
            'm_nudDepth
            '
            Me.m_nudDepth.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_nudDepth.DecimalPlaces = 3
            Me.m_nudDepth.Location = New System.Drawing.Point(54, 550)
            Me.m_nudDepth.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudDepth.Name = "m_nudDepth"
            Me.m_nudDepth.Size = New System.Drawing.Size(72, 20)
            Me.m_nudDepth.TabIndex = 4
            Me.m_nudDepth.ThousandsSeparator = True
            Me.m_nudDepth.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_lblDepth
            '
            Me.m_lblDepth.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblDepth.AutoSize = True
            Me.m_lblDepth.Location = New System.Drawing.Point(4, 552)
            Me.m_lblDepth.Name = "m_lblDepth"
            Me.m_lblDepth.Size = New System.Drawing.Size(33, 13)
            Me.m_lblDepth.TabIndex = 3
            Me.m_lblDepth.Text = "&MLD:"
            '
            'm_nudWind
            '
            Me.m_nudWind.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_nudWind.Location = New System.Drawing.Point(54, 524)
            Me.m_nudWind.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudWind.Name = "m_nudWind"
            Me.m_nudWind.Size = New System.Drawing.Size(72, 20)
            Me.m_nudWind.TabIndex = 4
            Me.m_nudWind.ThousandsSeparator = True
            Me.m_nudWind.Value = New Decimal(New Integer() {25, 0, 0, 0})
            '
            'm_lblWind
            '
            Me.m_lblWind.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblWind.AutoSize = True
            Me.m_lblWind.Location = New System.Drawing.Point(4, 526)
            Me.m_lblWind.Name = "m_lblWind"
            Me.m_lblWind.Size = New System.Drawing.Size(35, 13)
            Me.m_lblWind.TabIndex = 3
            Me.m_lblWind.Text = "&Wind:"
            '
            'm_lblCursor
            '
            Me.m_lblCursor.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblCursor.AutoSize = True
            Me.m_lblCursor.Location = New System.Drawing.Point(3, 500)
            Me.m_lblCursor.Name = "m_lblCursor"
            Me.m_lblCursor.Size = New System.Drawing.Size(40, 13)
            Me.m_lblCursor.TabIndex = 2
            Me.m_lblCursor.Text = "&Cursor:"
            '
            'm_sliderCursor
            '
            Me.m_sliderCursor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sliderCursor.Location = New System.Drawing.Point(54, 500)
            Me.m_sliderCursor.Maximum = 5
            Me.m_sliderCursor.Minimum = 1
            Me.m_sliderCursor.Name = "m_sliderCursor"
            Me.m_sliderCursor.Size = New System.Drawing.Size(111, 20)
            Me.m_sliderCursor.TabIndex = 1
            Me.m_sliderCursor.Value = 1
            '
            'm_hdrEditing
            '
            Me.m_hdrEditing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrEditing.Location = New System.Drawing.Point(0, 475)
            Me.m_hdrEditing.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrEditing.Name = "m_hdrEditing"
            Me.m_hdrEditing.Size = New System.Drawing.Size(168, 18)
            Me.m_hdrEditing.TabIndex = 0
            Me.m_hdrEditing.Text = "Editing"
            Me.m_hdrEditing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_hdrOptions
            '
            Me.m_hdrOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrOptions.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrOptions.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrOptions.Name = "m_hdrOptions"
            Me.m_hdrOptions.Size = New System.Drawing.Size(168, 18)
            Me.m_hdrOptions.TabIndex = 0
            Me.m_hdrOptions.Text = "Options"
            Me.m_hdrOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsControls
            '
            Me.m_tsControls.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tsControls.AutoSize = False
            Me.m_tsControls.Dock = System.Windows.Forms.DockStyle.None
            Me.m_tsControls.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiShowOptions, Me.m_sep1, Me.m_tslMonth, Me.m_tscmMonth})
            Me.m_tsControls.Location = New System.Drawing.Point(3, 3)
            Me.m_tsControls.Name = "m_tsControls"
            Me.m_tsControls.Size = New System.Drawing.Size(485, 25)
            Me.m_tsControls.TabIndex = 1
            Me.m_tsControls.Text = "ToolStrip1"
            '
            'm_tsmiShowOptions
            '
            Me.m_tsmiShowOptions.CheckOnClick = True
            Me.m_tsmiShowOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsmiShowOptions.Image = CType(resources.GetObject("m_tsmiShowOptions.Image"), System.Drawing.Image)
            Me.m_tsmiShowOptions.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsmiShowOptions.Name = "m_tsmiShowOptions"
            Me.m_tsmiShowOptions.Size = New System.Drawing.Size(75, 22)
            Me.m_tsmiShowOptions.Text = "Show options"
            '
            'm_sep1
            '
            Me.m_sep1.Name = "m_sep1"
            Me.m_sep1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tslMonth
            '
            Me.m_tslMonth.Name = "m_tslMonth"
            Me.m_tslMonth.Size = New System.Drawing.Size(70, 22)
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
            Me.m_ucZoomToolbar.Location = New System.Drawing.Point(491, 3)
            Me.m_ucZoomToolbar.MinimumSize = New System.Drawing.Size(100, 25)
            Me.m_ucZoomToolbar.Name = "m_ucZoomToolbar"
            Me.m_ucZoomToolbar.PositionMode = ScientificInterface.Ecospace.ucMapZoom.ePositionModeTypes.Center
            Me.m_ucZoomToolbar.Size = New System.Drawing.Size(310, 25)
            Me.m_ucZoomToolbar.TabIndex = 2
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
            Me.Text = "frmAdvection"
            Me.m_tlpMaps.ResumeLayout(False)
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudWind, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tsControls.ResumeLayout(False)
            Me.m_tsControls.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tlpMaps As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_ucMLD As ScientificInterface.Ecospace.Advection.ucMLD
        Private WithEvents m_ucUpwelling As ScientificInterface.Ecospace.Advection.ucUpwelling
        Private WithEvents m_ucWind As ScientificInterface.Ecospace.Advection.ucWind
        Private WithEvents m_ucMap As ScientificInterface.Ecospace.Advection.ucMap
        Private WithEvents m_tsControls As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsmiShowOptions As System.Windows.Forms.ToolStripButton
        Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tslMonth As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tscmMonth As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_ucZoomToolbar As ScientificInterface.Ecospace.ucMapZoomToolbar
        Private WithEvents m_lblCursor As System.Windows.Forms.Label
        Private WithEvents m_sliderCursor As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_lblWind As System.Windows.Forms.Label
        Private WithEvents m_nudWind As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudDepth As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblDepth As System.Windows.Forms.Label
        Private WithEvents m_hdrEditing As ScientificInterfaceShared.Controls.cEwEHeaderLabel

    End Class

End Namespace
