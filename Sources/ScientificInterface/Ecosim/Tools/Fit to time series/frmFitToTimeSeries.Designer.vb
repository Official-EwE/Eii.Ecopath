Imports ScientificInterfaceShared.Forms
Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated(), CLSCompliant(False)> _
    Partial Class frmFitToTimeSeries
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFitToTimeSeries))
            Me.m_split1 = New System.Windows.Forms.SplitContainer
            Me.m_btnTimeSeriesWeights = New System.Windows.Forms.Button
            Me.m_splitSearch = New System.Windows.Forms.SplitContainer
            Me.m_cbFishingMortalityPenalty = New System.Windows.Forms.CheckBox
            Me.m_grid = New ScientificInterface.Ecosim.gridFitToTimeSeriesGroup
            Me.m_hdrFishingMortality = New cEwEHeaderLabel
            Me.m_tlbSearch = New System.Windows.Forms.TableLayoutPanel
            Me.m_tbResults = New System.Windows.Forms.TextBox
            Me.m_hdrIterations = New cEwEHeaderLabel
            Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel
            Me.m_btnStop = New System.Windows.Forms.Button
            Me.m_btnSearch = New System.Windows.Forms.Button
            Me.m_hdrSearchTypes = New cEwEHeaderLabel
            Me.m_tbxVariance = New System.Windows.Forms.TextBox
            Me.m_cbVulnerabilitySearch = New System.Windows.Forms.CheckBox
            Me.m_cbAnomalySearch = New System.Windows.Forms.CheckBox
            Me.m_lblVarianceVulnerability = New System.Windows.Forms.Label
            Me.tabSearchOptions = New System.Windows.Forms.TabControl
            Me.tpVulnerabilitySearch = New System.Windows.Forms.TabPage
            Me.m_tsVulSearchTools = New System.Windows.Forms.ToolStrip
            Me.m_tsbSensOfSS2V = New System.Windows.Forms.ToolStripButton
            Me.m_tsbSearchGroup = New System.Windows.Forms.ToolStripButton
            Me.m_vulnerabilityBlockCodeSelector = New ScientificInterface.Ecosim.ucParmBlockCodes
            Me.m_vulnerabilityBlockMatrix = New ScientificInterface.Ecosim.ucVulnerabiltyBlocks
            Me.tpAnomalySearch = New System.Windows.Forms.TabPage
            Me.m_nudLastYear = New System.Windows.Forms.NumericUpDown
            Me.m_nudFirstYear = New System.Windows.Forms.NumericUpDown
            Me.m_nudSplinePts = New System.Windows.Forms.NumericUpDown
            Me.m_splitAnomalyShape = New System.Windows.Forms.SplitContainer
            Me.m_sketchPad = New ScientificInterface.Ecosim.ucAnomalySearchSketchPad
            Me.m_hdrAppliedFF = New cEwEHeaderLabel
            Me.m_shapeToolBox = New ScientificInterfaceShared.Controls.ucShapeToolbox
            Me.m_lbFirstYear = New System.Windows.Forms.Label
            Me.m_lbLastYear = New System.Windows.Forms.Label
            Me.m_lbVariancePrimaryProd = New System.Windows.Forms.Label
            Me.m_lbSplinePoints = New System.Windows.Forms.Label
            Me.m_tbVariancePrimaryProd = New System.Windows.Forms.TextBox
            Me.m_hdrSearch = New cEwEHeaderLabel
            Me.m_split1.Panel1.SuspendLayout()
            Me.m_split1.Panel2.SuspendLayout()
            Me.m_split1.SuspendLayout()
            Me.m_splitSearch.Panel1.SuspendLayout()
            Me.m_splitSearch.Panel2.SuspendLayout()
            Me.m_splitSearch.SuspendLayout()
            Me.m_tlbSearch.SuspendLayout()
            Me.TableLayoutPanel2.SuspendLayout()
            Me.tabSearchOptions.SuspendLayout()
            Me.tpVulnerabilitySearch.SuspendLayout()
            Me.m_tsVulSearchTools.SuspendLayout()
            Me.tpAnomalySearch.SuspendLayout()
            CType(Me.m_nudLastYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudFirstYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSplinePts, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_splitAnomalyShape.Panel1.SuspendLayout()
            Me.m_splitAnomalyShape.Panel2.SuspendLayout()
            Me.m_splitAnomalyShape.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_split1
            '
            Me.m_split1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_split1.Location = New System.Drawing.Point(0, 0)
            Me.m_split1.Name = "m_split1"
            '
            'm_split1.Panel1
            '
            Me.m_split1.Panel1.Controls.Add(Me.m_btnTimeSeriesWeights)
            Me.m_split1.Panel1.Controls.Add(Me.m_splitSearch)
            Me.m_split1.Panel1.Controls.Add(Me.m_hdrSearchTypes)
            Me.m_split1.Panel1.Controls.Add(Me.m_tbxVariance)
            Me.m_split1.Panel1.Controls.Add(Me.m_cbVulnerabilitySearch)
            Me.m_split1.Panel1.Controls.Add(Me.m_cbAnomalySearch)
            Me.m_split1.Panel1.Controls.Add(Me.m_lblVarianceVulnerability)
            Me.m_split1.Panel1.Margin = New System.Windows.Forms.Padding(3)
            Me.m_split1.Panel1.Padding = New System.Windows.Forms.Padding(3)
            Me.m_split1.Panel1MinSize = 249
            '
            'm_split1.Panel2
            '
            Me.m_split1.Panel2.Controls.Add(Me.tabSearchOptions)
            Me.m_split1.Panel2.Controls.Add(Me.m_hdrSearch)
            Me.m_split1.Panel2.Padding = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.m_split1.Size = New System.Drawing.Size(784, 633)
            Me.m_split1.SplitterDistance = 249
            Me.m_split1.TabIndex = 0
            '
            'm_btnTimeSeriesWeights
            '
            Me.m_btnTimeSeriesWeights.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnTimeSeriesWeights.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.m_btnTimeSeriesWeights.Location = New System.Drawing.Point(138, 26)
            Me.m_btnTimeSeriesWeights.Name = "m_btnTimeSeriesWeights"
            Me.m_btnTimeSeriesWeights.Size = New System.Drawing.Size(105, 44)
            Me.m_btnTimeSeriesWeights.TabIndex = 20
            Me.m_btnTimeSeriesWeights.Text = "&Time series weights..."
            Me.m_btnTimeSeriesWeights.UseVisualStyleBackColor = True
            '
            'm_splitSearch
            '
            Me.m_splitSearch.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_splitSearch.Location = New System.Drawing.Point(0, 108)
            Me.m_splitSearch.Margin = New System.Windows.Forms.Padding(0)
            Me.m_splitSearch.Name = "m_splitSearch"
            Me.m_splitSearch.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_splitSearch.Panel1
            '
            Me.m_splitSearch.Panel1.Controls.Add(Me.m_cbFishingMortalityPenalty)
            Me.m_splitSearch.Panel1.Controls.Add(Me.m_grid)
            Me.m_splitSearch.Panel1.Controls.Add(Me.m_hdrFishingMortality)
            '
            'm_splitSearch.Panel2
            '
            Me.m_splitSearch.Panel2.Controls.Add(Me.m_tlbSearch)
            Me.m_splitSearch.Size = New System.Drawing.Size(246, 525)
            Me.m_splitSearch.SplitterDistance = 277
            Me.m_splitSearch.TabIndex = 19
            '
            'm_cbFishingMortalityPenalty
            '
            Me.m_cbFishingMortalityPenalty.AutoSize = True
            Me.m_cbFishingMortalityPenalty.Location = New System.Drawing.Point(3, 257)
            Me.m_cbFishingMortalityPenalty.Name = "m_cbFishingMortalityPenalty"
            Me.m_cbFishingMortalityPenalty.Size = New System.Drawing.Size(137, 17)
            Me.m_cbFishingMortalityPenalty.TabIndex = 7
            Me.m_cbFishingMortalityPenalty.Text = "Fishing mortality &penalty"
            Me.m_cbFishingMortalityPenalty.UseVisualStyleBackColor = True
            Me.m_cbFishingMortalityPenalty.Visible = False
            '
            'm_grid
            '
            Me.m_grid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Location = New System.Drawing.Point(0, 23)
            Me.m_grid.Manager = Nothing
            Me.m_grid.Margin = New System.Windows.Forms.Padding(0)
            Me.m_grid.Name = "m_grid"
            Me.m_grid.Size = New System.Drawing.Size(246, 231)
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TabIndex = 6
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'm_hdrFishingMortality
            '
            Me.m_hdrFishingMortality.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrFishingMortality.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrFishingMortality.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrFishingMortality.Name = "m_hdrFishingMortality"
            Me.m_hdrFishingMortality.Size = New System.Drawing.Size(246, 18)
            Me.m_hdrFishingMortality.TabIndex = 5
            Me.m_hdrFishingMortality.Text = "Max fishing mortality"
            '
            'm_tlbSearch
            '
            Me.m_tlbSearch.ColumnCount = 1
            Me.m_tlbSearch.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbSearch.Controls.Add(Me.m_tbResults, 0, 1)
            Me.m_tlbSearch.Controls.Add(Me.m_hdrIterations, 0, 0)
            Me.m_tlbSearch.Controls.Add(Me.TableLayoutPanel2, 0, 2)
            Me.m_tlbSearch.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlbSearch.Location = New System.Drawing.Point(0, 0)
            Me.m_tlbSearch.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tlbSearch.Name = "m_tlbSearch"
            Me.m_tlbSearch.RowCount = 3
            Me.m_tlbSearch.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18.0!))
            Me.m_tlbSearch.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbSearch.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.m_tlbSearch.Size = New System.Drawing.Size(246, 244)
            Me.m_tlbSearch.TabIndex = 0
            '
            'm_tbResults
            '
            Me.m_tbResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tbResults.Location = New System.Drawing.Point(0, 18)
            Me.m_tbResults.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tbResults.Multiline = True
            Me.m_tbResults.Name = "m_tbResults"
            Me.m_tbResults.ReadOnly = True
            Me.m_tbResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.m_tbResults.Size = New System.Drawing.Size(246, 196)
            Me.m_tbResults.TabIndex = 0
            '
            'm_hdrIterations
            '
            Me.m_hdrIterations.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrIterations.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrIterations.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrIterations.Name = "m_hdrIterations"
            Me.m_hdrIterations.Size = New System.Drawing.Size(246, 18)
            Me.m_hdrIterations.TabIndex = 5
            Me.m_hdrIterations.Text = "Iterations"
            '
            'TableLayoutPanel2
            '
            Me.TableLayoutPanel2.ColumnCount = 4
            Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
            Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
            Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel2.Controls.Add(Me.m_btnStop, 1, 0)
            Me.TableLayoutPanel2.Controls.Add(Me.m_btnSearch, 2, 0)
            Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 214)
            Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
            Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
            Me.TableLayoutPanel2.RowCount = 1
            Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel2.Size = New System.Drawing.Size(246, 30)
            Me.TableLayoutPanel2.TabIndex = 6
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.Location = New System.Drawing.Point(51, 3)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(69, 24)
            Me.m_btnStop.TabIndex = 1
            Me.m_btnStop.Text = "Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_btnSearch
            '
            Me.m_btnSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSearch.Location = New System.Drawing.Point(126, 3)
            Me.m_btnSearch.Name = "m_btnSearch"
            Me.m_btnSearch.Size = New System.Drawing.Size(69, 24)
            Me.m_btnSearch.TabIndex = 2
            Me.m_btnSearch.Text = "&Search"
            Me.m_btnSearch.UseVisualStyleBackColor = True
            '
            'm_hdrSearchTypes
            '
            Me.m_hdrSearchTypes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrSearchTypes.Location = New System.Drawing.Point(0, 3)
            Me.m_hdrSearchTypes.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrSearchTypes.Name = "m_hdrSearchTypes"
            Me.m_hdrSearchTypes.Size = New System.Drawing.Size(246, 18)
            Me.m_hdrSearchTypes.TabIndex = 0
            Me.m_hdrSearchTypes.Text = "Search types"
            '
            'm_tbxVariance
            '
            Me.m_tbxVariance.Location = New System.Drawing.Point(79, 54)
            Me.m_tbxVariance.Name = "m_tbxVariance"
            Me.m_tbxVariance.Size = New System.Drawing.Size(41, 20)
            Me.m_tbxVariance.TabIndex = 3
            Me.m_tbxVariance.Text = "0.1"
            '
            'm_cbVulnerabilitySearch
            '
            Me.m_cbVulnerabilitySearch.AutoSize = True
            Me.m_cbVulnerabilitySearch.Checked = True
            Me.m_cbVulnerabilitySearch.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbVulnerabilitySearch.Location = New System.Drawing.Point(3, 30)
            Me.m_cbVulnerabilitySearch.Name = "m_cbVulnerabilitySearch"
            Me.m_cbVulnerabilitySearch.Size = New System.Drawing.Size(117, 17)
            Me.m_cbVulnerabilitySearch.TabIndex = 1
            Me.m_cbVulnerabilitySearch.Text = "&Vulnerability search"
            Me.m_cbVulnerabilitySearch.UseVisualStyleBackColor = True
            '
            'm_cbAnomalySearch
            '
            Me.m_cbAnomalySearch.AutoSize = True
            Me.m_cbAnomalySearch.Location = New System.Drawing.Point(3, 80)
            Me.m_cbAnomalySearch.Name = "m_cbAnomalySearch"
            Me.m_cbAnomalySearch.Size = New System.Drawing.Size(101, 17)
            Me.m_cbAnomalySearch.TabIndex = 4
            Me.m_cbAnomalySearch.Text = "&Anomaly search"
            Me.m_cbAnomalySearch.UseVisualStyleBackColor = True
            '
            'm_lblVarianceVulnerability
            '
            Me.m_lblVarianceVulnerability.AutoSize = True
            Me.m_lblVarianceVulnerability.Location = New System.Drawing.Point(21, 57)
            Me.m_lblVarianceVulnerability.Name = "m_lblVarianceVulnerability"
            Me.m_lblVarianceVulnerability.Size = New System.Drawing.Size(52, 13)
            Me.m_lblVarianceVulnerability.TabIndex = 2
            Me.m_lblVarianceVulnerability.Text = "Va&riance:"
            '
            'tabSearchOptions
            '
            Me.tabSearchOptions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tabSearchOptions.Controls.Add(Me.tpVulnerabilitySearch)
            Me.tabSearchOptions.Controls.Add(Me.tpAnomalySearch)
            Me.tabSearchOptions.Location = New System.Drawing.Point(0, 26)
            Me.tabSearchOptions.Name = "tabSearchOptions"
            Me.tabSearchOptions.SelectedIndex = 0
            Me.tabSearchOptions.Size = New System.Drawing.Size(531, 607)
            Me.tabSearchOptions.TabIndex = 0
            '
            'tpVulnerabilitySearch
            '
            Me.tpVulnerabilitySearch.Controls.Add(Me.m_tsVulSearchTools)
            Me.tpVulnerabilitySearch.Controls.Add(Me.m_vulnerabilityBlockCodeSelector)
            Me.tpVulnerabilitySearch.Controls.Add(Me.m_vulnerabilityBlockMatrix)
            Me.tpVulnerabilitySearch.Location = New System.Drawing.Point(4, 22)
            Me.tpVulnerabilitySearch.Margin = New System.Windows.Forms.Padding(0)
            Me.tpVulnerabilitySearch.Name = "tpVulnerabilitySearch"
            Me.tpVulnerabilitySearch.Padding = New System.Windows.Forms.Padding(3)
            Me.tpVulnerabilitySearch.Size = New System.Drawing.Size(523, 581)
            Me.tpVulnerabilitySearch.TabIndex = 0
            Me.tpVulnerabilitySearch.Text = "Vulnerability Search"
            Me.tpVulnerabilitySearch.UseVisualStyleBackColor = True
            '
            'm_tsVulSearchTools
            '
            Me.m_tsVulSearchTools.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbSensOfSS2V, Me.m_tsbSearchGroup})
            Me.m_tsVulSearchTools.Location = New System.Drawing.Point(3, 3)
            Me.m_tsVulSearchTools.Name = "m_tsVulSearchTools"
            Me.m_tsVulSearchTools.Size = New System.Drawing.Size(517, 25)
            Me.m_tsVulSearchTools.TabIndex = 10
            '
            'm_tsbSensOfSS2V
            '
            Me.m_tsbSensOfSS2V.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
            Me.m_tsbSensOfSS2V.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSensOfSS2V.Name = "m_tsbSensOfSS2V"
            Me.m_tsbSensOfSS2V.Size = New System.Drawing.Size(126, 22)
            Me.m_tsbSensOfSS2V.Text = "Sensitivity of SS to V"
            '
            'm_tsbSearchGroup
            '
            Me.m_tsbSearchGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSearchGroup.Image = CType(resources.GetObject("m_tsbSearchGroup.Image"), System.Drawing.Image)
            Me.m_tsbSearchGroup.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSearchGroup.Name = "m_tsbSearchGroup"
            Me.m_tsbSearchGroup.Size = New System.Drawing.Size(157, 22)
            Me.m_tsbSearchGroup.Text = "Search groups with time series"
            '
            'm_vulnerabilityBlockCodeSelector
            '
            Me.m_vulnerabilityBlockCodeSelector.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_vulnerabilityBlockCodeSelector.Location = New System.Drawing.Point(0, 30)
            Me.m_vulnerabilityBlockCodeSelector.Margin = New System.Windows.Forms.Padding(0)
            Me.m_vulnerabilityBlockCodeSelector.Name = "m_vulnerabilityBlockCodeSelector"
            Me.m_vulnerabilityBlockCodeSelector.NumBlocks = 30
            Me.m_vulnerabilityBlockCodeSelector.SelectedBlock = 15
            Me.m_vulnerabilityBlockCodeSelector.Size = New System.Drawing.Size(524, 52)
            Me.m_vulnerabilityBlockCodeSelector.TabIndex = 1
            Me.m_vulnerabilityBlockCodeSelector.UIContext = Nothing
            '
            'm_vulnerabilityBlockMatrix
            '
            Me.m_vulnerabilityBlockMatrix.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_vulnerabilityBlockMatrix.BlockColors = Nothing
            Me.m_vulnerabilityBlockMatrix.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_vulnerabilityBlockMatrix.Location = New System.Drawing.Point(-1, 83)
            Me.m_vulnerabilityBlockMatrix.Name = "m_vulnerabilityBlockMatrix"
            Me.m_vulnerabilityBlockMatrix.SelectedBlockNum = 0
            Me.m_vulnerabilityBlockMatrix.Size = New System.Drawing.Size(521, 498)
            Me.m_vulnerabilityBlockMatrix.TabIndex = 9
            Me.m_vulnerabilityBlockMatrix.TabStop = False
            Me.m_vulnerabilityBlockMatrix.UIContext = Nothing
            '
            'tpAnomalySearch
            '
            Me.tpAnomalySearch.Controls.Add(Me.m_nudLastYear)
            Me.tpAnomalySearch.Controls.Add(Me.m_nudFirstYear)
            Me.tpAnomalySearch.Controls.Add(Me.m_nudSplinePts)
            Me.tpAnomalySearch.Controls.Add(Me.m_splitAnomalyShape)
            Me.tpAnomalySearch.Controls.Add(Me.m_lbFirstYear)
            Me.tpAnomalySearch.Controls.Add(Me.m_lbLastYear)
            Me.tpAnomalySearch.Controls.Add(Me.m_lbVariancePrimaryProd)
            Me.tpAnomalySearch.Controls.Add(Me.m_lbSplinePoints)
            Me.tpAnomalySearch.Controls.Add(Me.m_tbVariancePrimaryProd)
            Me.tpAnomalySearch.Location = New System.Drawing.Point(4, 22)
            Me.tpAnomalySearch.Name = "tpAnomalySearch"
            Me.tpAnomalySearch.Padding = New System.Windows.Forms.Padding(3)
            Me.tpAnomalySearch.Size = New System.Drawing.Size(523, 581)
            Me.tpAnomalySearch.TabIndex = 1
            Me.tpAnomalySearch.Text = "Anomaly Search"
            Me.tpAnomalySearch.UseVisualStyleBackColor = True
            '
            'm_nudLastYear
            '
            Me.m_nudLastYear.Location = New System.Drawing.Point(63, 29)
            Me.m_nudLastYear.Maximum = New Decimal(New Integer() {25, 0, 0, 0})
            Me.m_nudLastYear.Name = "m_nudLastYear"
            Me.m_nudLastYear.Size = New System.Drawing.Size(60, 20)
            Me.m_nudLastYear.TabIndex = 5
            Me.m_nudLastYear.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_nudFirstYear
            '
            Me.m_nudFirstYear.Location = New System.Drawing.Point(63, 5)
            Me.m_nudFirstYear.Maximum = New Decimal(New Integer() {25, 0, 0, 0})
            Me.m_nudFirstYear.Name = "m_nudFirstYear"
            Me.m_nudFirstYear.Size = New System.Drawing.Size(60, 20)
            Me.m_nudFirstYear.TabIndex = 1
            '
            'm_nudSplinePts
            '
            Me.m_nudSplinePts.Location = New System.Drawing.Point(263, 30)
            Me.m_nudSplinePts.Maximum = New Decimal(New Integer() {25, 0, 0, 0})
            Me.m_nudSplinePts.Name = "m_nudSplinePts"
            Me.m_nudSplinePts.Size = New System.Drawing.Size(52, 20)
            Me.m_nudSplinePts.TabIndex = 7
            '
            'm_splitAnomalyShape
            '
            Me.m_splitAnomalyShape.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_splitAnomalyShape.Location = New System.Drawing.Point(0, 83)
            Me.m_splitAnomalyShape.Margin = New System.Windows.Forms.Padding(0)
            Me.m_splitAnomalyShape.Name = "m_splitAnomalyShape"
            Me.m_splitAnomalyShape.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_splitAnomalyShape.Panel1
            '
            Me.m_splitAnomalyShape.Panel1.Controls.Add(Me.m_sketchPad)
            '
            'm_splitAnomalyShape.Panel2
            '
            Me.m_splitAnomalyShape.Panel2.Controls.Add(Me.m_hdrAppliedFF)
            Me.m_splitAnomalyShape.Panel2.Controls.Add(Me.m_shapeToolBox)
            Me.m_splitAnomalyShape.Size = New System.Drawing.Size(523, 525)
            Me.m_splitAnomalyShape.SplitterDistance = 386
            Me.m_splitAnomalyShape.TabIndex = 16
            '
            'm_sketchPad
            '
            Me.m_sketchPad.BackColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(235, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.m_sketchPad.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_sketchPad.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_sketchPad.DisplayAxis = True
            Me.m_sketchPad.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.FirstYear = 0
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.LastYear = 0
            Me.m_sketchPad.Location = New System.Drawing.Point(0, 0)
            Me.m_sketchPad.Margin = New System.Windows.Forms.Padding(0)
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.NumSplinePoints = 0
            Me.m_sketchPad.NumTSYears = 0
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.ShowXMark = False
            Me.m_sketchPad.Size = New System.Drawing.Size(523, 386)
            Me.m_sketchPad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
            Me.m_sketchPad.TabIndex = 0
            Me.m_sketchPad.UIContext = Nothing
            Me.m_sketchPad.XMarkLabel = ""
            Me.m_sketchPad.XMarkValue = -9999.0!
            Me.m_sketchPad.YAxisAutoScaleMode = ScientificInterfaceShared.Definitions.eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = 1.0!
            Me.m_sketchPad.YMarkLabel = ""
            Me.m_sketchPad.YMarkValue = -9999.0!
            '
            'm_hdrAppliedFF
            '
            Me.m_hdrAppliedFF.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrAppliedFF.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrAppliedFF.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrAppliedFF.Name = "m_hdrAppliedFF"
            Me.m_hdrAppliedFF.Size = New System.Drawing.Size(523, 23)
            Me.m_hdrAppliedFF.TabIndex = 6
            Me.m_hdrAppliedFF.Text = "Applied Forcing Functions"
            '
            'm_shapeToolBox
            '
            Me.m_shapeToolBox.AllowCheckboxes = False
            Me.m_shapeToolBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_shapeToolBox.Color = System.Drawing.Color.Empty
            Me.m_shapeToolBox.Handler = Nothing
            Me.m_shapeToolBox.Location = New System.Drawing.Point(0, 23)
            Me.m_shapeToolBox.Margin = New System.Windows.Forms.Padding(0)
            Me.m_shapeToolBox.Name = "m_shapeToolBox"
            Me.m_shapeToolBox.Selection = New EwECore.cShapeData(-1) {}
            Me.m_shapeToolBox.Size = New System.Drawing.Size(523, 112)
            Me.m_shapeToolBox.TabIndex = 0
            Me.m_shapeToolBox.UIContext = Nothing
            Me.m_shapeToolBox.YAxisMinValue = -9999.0!
            '
            'm_lbFirstYear
            '
            Me.m_lbFirstYear.AutoSize = True
            Me.m_lbFirstYear.Location = New System.Drawing.Point(4, 7)
            Me.m_lbFirstYear.Name = "m_lbFirstYear"
            Me.m_lbFirstYear.Size = New System.Drawing.Size(52, 13)
            Me.m_lbFirstYear.TabIndex = 0
            Me.m_lbFirstYear.Text = "First year:"
            '
            'm_lbLastYear
            '
            Me.m_lbLastYear.AutoSize = True
            Me.m_lbLastYear.Location = New System.Drawing.Point(4, 33)
            Me.m_lbLastYear.Name = "m_lbLastYear"
            Me.m_lbLastYear.Size = New System.Drawing.Size(53, 13)
            Me.m_lbLastYear.TabIndex = 4
            Me.m_lbLastYear.Text = "Last year:"
            '
            'm_lbVariancePrimaryProd
            '
            Me.m_lbVariancePrimaryProd.AutoSize = True
            Me.m_lbVariancePrimaryProd.Location = New System.Drawing.Point(187, 7)
            Me.m_lbVariancePrimaryProd.Name = "m_lbVariancePrimaryProd"
            Me.m_lbVariancePrimaryProd.Size = New System.Drawing.Size(69, 13)
            Me.m_lbVariancePrimaryProd.TabIndex = 2
            Me.m_lbVariancePrimaryProd.Text = "&PP Variance:"
            '
            'm_lbSplinePoints
            '
            Me.m_lbSplinePoints.AutoSize = True
            Me.m_lbSplinePoints.Location = New System.Drawing.Point(187, 33)
            Me.m_lbSplinePoints.Name = "m_lbSplinePoints"
            Me.m_lbSplinePoints.Size = New System.Drawing.Size(70, 13)
            Me.m_lbSplinePoints.TabIndex = 6
            Me.m_lbSplinePoints.Text = "Spline points:"
            '
            'm_tbVariancePrimaryProd
            '
            Me.m_tbVariancePrimaryProd.Location = New System.Drawing.Point(263, 4)
            Me.m_tbVariancePrimaryProd.Name = "m_tbVariancePrimaryProd"
            Me.m_tbVariancePrimaryProd.Size = New System.Drawing.Size(52, 20)
            Me.m_tbVariancePrimaryProd.TabIndex = 3
            Me.m_tbVariancePrimaryProd.Text = "0.1"
            '
            'm_hdrSearch
            '
            Me.m_hdrSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrSearch.Location = New System.Drawing.Point(0, 3)
            Me.m_hdrSearch.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrSearch.Name = "m_hdrSearch"
            Me.m_hdrSearch.Size = New System.Drawing.Size(531, 18)
            Me.m_hdrSearch.TabIndex = 0
            Me.m_hdrSearch.Text = "Search"
            '
            'frmFitToTimeSeries
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(784, 633)
            Me.Controls.Add(Me.m_split1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmFitToTimeSeries"
            Me.TabText = "Fit to time series"
            Me.Text = "Fit to time series"
            Me.m_split1.Panel1.ResumeLayout(False)
            Me.m_split1.Panel1.PerformLayout()
            Me.m_split1.Panel2.ResumeLayout(False)
            Me.m_split1.ResumeLayout(False)
            Me.m_splitSearch.Panel1.ResumeLayout(False)
            Me.m_splitSearch.Panel1.PerformLayout()
            Me.m_splitSearch.Panel2.ResumeLayout(False)
            Me.m_splitSearch.ResumeLayout(False)
            Me.m_tlbSearch.ResumeLayout(False)
            Me.m_tlbSearch.PerformLayout()
            Me.TableLayoutPanel2.ResumeLayout(False)
            Me.tabSearchOptions.ResumeLayout(False)
            Me.tpVulnerabilitySearch.ResumeLayout(False)
            Me.tpVulnerabilitySearch.PerformLayout()
            Me.m_tsVulSearchTools.ResumeLayout(False)
            Me.m_tsVulSearchTools.PerformLayout()
            Me.tpAnomalySearch.ResumeLayout(False)
            Me.tpAnomalySearch.PerformLayout()
            CType(Me.m_nudLastYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudFirstYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSplinePts, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_splitAnomalyShape.Panel1.ResumeLayout(False)
            Me.m_splitAnomalyShape.Panel2.ResumeLayout(False)
            Me.m_splitAnomalyShape.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_split1 As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbSplinePoints As System.Windows.Forms.Label
        Private WithEvents m_btnSearch As System.Windows.Forms.Button
        Private WithEvents m_tbVariancePrimaryProd As System.Windows.Forms.TextBox
        Private WithEvents m_lbVariancePrimaryProd As System.Windows.Forms.Label
        Private WithEvents m_lbLastYear As System.Windows.Forms.Label
        Private WithEvents m_lbFirstYear As System.Windows.Forms.Label
        Private WithEvents m_cbAnomalySearch As System.Windows.Forms.CheckBox
        Private WithEvents m_tbxVariance As System.Windows.Forms.TextBox
        Private WithEvents m_lblVarianceVulnerability As System.Windows.Forms.Label
        Private WithEvents m_cbVulnerabilitySearch As System.Windows.Forms.CheckBox
        Private WithEvents m_vulnerabilityBlockMatrix As ucVulnerabiltyBlocks
        Private WithEvents m_vulnerabilityBlockCodeSelector As ucParmBlockCodes
        Private WithEvents m_splitSearch As System.Windows.Forms.SplitContainer
        Private WithEvents m_tbResults As System.Windows.Forms.TextBox
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents tabSearchOptions As System.Windows.Forms.TabControl
        Private WithEvents tpVulnerabilitySearch As System.Windows.Forms.TabPage
        Private WithEvents tpAnomalySearch As System.Windows.Forms.TabPage
        Private WithEvents m_hdrFishingMortality As cEwEHeaderLabel
        Private WithEvents m_splitAnomalyShape As System.Windows.Forms.SplitContainer
        Private WithEvents m_sketchPad As ucAnomalySearchSketchPad
        Private WithEvents m_shapeToolBox As ucShapeToolbox
        Private WithEvents m_tlbSearch As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_hdrIterations As cEwEHeaderLabel
        Private WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_nudSplinePts As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudLastYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudFirstYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_hdrSearchTypes As cEwEHeaderLabel
        Private WithEvents m_tsVulSearchTools As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbSensOfSS2V As System.Windows.Forms.ToolStripButton
        Private WithEvents m_hdrAppliedFF As cEwEHeaderLabel
        Private WithEvents m_cbFishingMortalityPenalty As System.Windows.Forms.CheckBox
        Private WithEvents m_grid As gridFitToTimeSeriesGroup
        Private WithEvents m_btnTimeSeriesWeights As System.Windows.Forms.Button
        Private WithEvents m_hdrSearch As cEwEHeaderLabel
        Private WithEvents m_tsbSearchGroup As System.Windows.Forms.ToolStripButton
    End Class

End Namespace

