Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcotracerOutput
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
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
        Me.m_scMain = New System.Windows.Forms.SplitContainer
        Me.m_hdrGroups = New cEwEHeaderLabel
        Me.m_btnShowHideGroups = New System.Windows.Forms.Button
        Me.m_cmbRegions = New System.Windows.Forms.ComboBox
        Me.m_chkSortGroups = New System.Windows.Forms.CheckBox
        Me.m_hdrPlotOptions = New cEwEHeaderLabel
        Me.m_hdrCommands = New cEwEHeaderLabel
        Me.m_lblRegion = New System.Windows.Forms.Label
        Me.m_rbCB = New System.Windows.Forms.RadioButton
        Me.m_rbConc = New System.Windows.Forms.RadioButton
        Me.m_btnRunSpace = New System.Windows.Forms.Button
        Me.m_btnRunSim = New System.Windows.Forms.Button
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_zgc
        '
        Me.m_zgc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_zgc.Location = New System.Drawing.Point(0, 0)
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        Me.m_zgc.Size = New System.Drawing.Size(762, 504)
        Me.m_zgc.TabIndex = 0
        '
        'm_lbGroups
        '
        Me.m_lbGroups.AllGroupsItemColor = System.Drawing.Color.Black
        Me.m_lbGroups.AllGroupsItemText = "(Environment)"
        Me.m_lbGroups.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.m_lbGroups.FormattingEnabled = True
        Me.m_lbGroups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayVisibleOnly
        Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.AllGroups
        Me.m_lbGroups.IntegralHeight = False
        Me.m_lbGroups.Location = New System.Drawing.Point(0, 18)
        Me.m_lbGroups.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbGroups.Name = "m_lbGroups"
        Me.m_lbGroups.SelectedGroup = Nothing
        Me.m_lbGroups.SelectedGroupIndex = -1
        Me.m_lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.m_lbGroups.Size = New System.Drawing.Size(180, 237)
        Me.m_lbGroups.SortThreshold = -9999.0!
        Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.GroupIndexAsc
        Me.m_lbGroups.TabIndex = 0
        '
        'm_scMain
        '
        Me.m_scMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_scMain.Location = New System.Drawing.Point(0, 1)
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_hdrGroups)
        Me.m_scMain.Panel1.Controls.Add(Me.m_btnShowHideGroups)
        Me.m_scMain.Panel1.Controls.Add(Me.m_cmbRegions)
        Me.m_scMain.Panel1.Controls.Add(Me.m_chkSortGroups)
        Me.m_scMain.Panel1.Controls.Add(Me.m_hdrPlotOptions)
        Me.m_scMain.Panel1.Controls.Add(Me.m_hdrCommands)
        Me.m_scMain.Panel1.Controls.Add(Me.m_lblRegion)
        Me.m_scMain.Panel1.Controls.Add(Me.m_rbCB)
        Me.m_scMain.Panel1.Controls.Add(Me.m_rbConc)
        Me.m_scMain.Panel1.Controls.Add(Me.m_btnRunSpace)
        Me.m_scMain.Panel1.Controls.Add(Me.m_btnRunSim)
        Me.m_scMain.Panel1.Controls.Add(Me.m_lbGroups)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_zgc)
        Me.m_scMain.Size = New System.Drawing.Size(946, 504)
        Me.m_scMain.SplitterDistance = 180
        Me.m_scMain.TabIndex = 2
        '
        'm_hdrGroups
        '
        Me.m_hdrGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrGroups.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_hdrGroups.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.m_hdrGroups.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrGroups.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_hdrGroups.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrGroups.Name = "m_hdrGroups"
        Me.m_hdrGroups.Size = New System.Drawing.Size(180, 18)
        Me.m_hdrGroups.TabIndex = 11
        Me.m_hdrGroups.Text = "Groups"
        Me.m_hdrGroups.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_btnShowHideGroups
        '
        Me.m_btnShowHideGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnShowHideGroups.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnShowHideGroups.Image = SharedResources.Eye_open
        Me.m_btnShowHideGroups.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.m_btnShowHideGroups.Location = New System.Drawing.Point(3, 360)
        Me.m_btnShowHideGroups.Name = "m_btnShowHideGroups"
        Me.m_btnShowHideGroups.Size = New System.Drawing.Size(174, 23)
        Me.m_btnShowHideGroups.TabIndex = 5
        Me.m_btnShowHideGroups.Text = "Choose &groups..."
        Me.m_btnShowHideGroups.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.m_btnShowHideGroups.UseVisualStyleBackColor = True
        '
        'm_cmbRegions
        '
        Me.m_cmbRegions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_cmbRegions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbRegions.FormattingEnabled = True
        Me.m_cmbRegions.Location = New System.Drawing.Point(22, 480)
        Me.m_cmbRegions.Name = "m_cmbRegions"
        Me.m_cmbRegions.Size = New System.Drawing.Size(155, 21)
        Me.m_cmbRegions.TabIndex = 10
        '
        'm_chkSortGroups
        '
        Me.m_chkSortGroups.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_chkSortGroups.AutoSize = True
        Me.m_chkSortGroups.Location = New System.Drawing.Point(3, 389)
        Me.m_chkSortGroups.Name = "m_chkSortGroups"
        Me.m_chkSortGroups.Size = New System.Drawing.Size(80, 17)
        Me.m_chkSortGroups.TabIndex = 6
        Me.m_chkSortGroups.Text = "&Sort groups"
        Me.m_chkSortGroups.UseVisualStyleBackColor = True
        '
        'm_hdrPlotOptions
        '
        Me.m_hdrPlotOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrPlotOptions.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrPlotOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_hdrPlotOptions.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrPlotOptions.Location = New System.Drawing.Point(0, 337)
        Me.m_hdrPlotOptions.Name = "m_hdrPlotOptions"
        Me.m_hdrPlotOptions.Size = New System.Drawing.Size(180, 20)
        Me.m_hdrPlotOptions.TabIndex = 4
        Me.m_hdrPlotOptions.Text = "Plot Options"
        Me.m_hdrPlotOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrCommands
        '
        Me.m_hdrCommands.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrCommands.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrCommands.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_hdrCommands.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrCommands.Location = New System.Drawing.Point(0, 258)
        Me.m_hdrCommands.Name = "m_hdrCommands"
        Me.m_hdrCommands.Size = New System.Drawing.Size(180, 20)
        Me.m_hdrCommands.TabIndex = 1
        Me.m_hdrCommands.Text = "Commands"
        Me.m_hdrCommands.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblRegion
        '
        Me.m_lblRegion.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_lblRegion.AutoSize = True
        Me.m_lblRegion.Location = New System.Drawing.Point(0, 464)
        Me.m_lblRegion.Name = "m_lblRegion"
        Me.m_lblRegion.Size = New System.Drawing.Size(44, 13)
        Me.m_lblRegion.TabIndex = 9
        Me.m_lblRegion.Text = "&Region:"
        '
        'm_rbCB
        '
        Me.m_rbCB.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_rbCB.AutoSize = True
        Me.m_rbCB.Checked = True
        Me.m_rbCB.Location = New System.Drawing.Point(3, 412)
        Me.m_rbCB.Name = "m_rbCB"
        Me.m_rbCB.Size = New System.Drawing.Size(141, 17)
        Me.m_rbCB.TabIndex = 7
        Me.m_rbCB.TabStop = True
        Me.m_rbCB.Text = "Concentration / &Biomass"
        Me.m_rbCB.UseVisualStyleBackColor = True
        '
        'm_rbConc
        '
        Me.m_rbConc.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_rbConc.AutoSize = True
        Me.m_rbConc.Location = New System.Drawing.Point(3, 435)
        Me.m_rbConc.Name = "m_rbConc"
        Me.m_rbConc.Size = New System.Drawing.Size(91, 17)
        Me.m_rbConc.TabIndex = 8
        Me.m_rbConc.Text = "&Concentration"
        Me.m_rbConc.UseVisualStyleBackColor = True
        '
        'm_btnRunSpace
        '
        Me.m_btnRunSpace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnRunSpace.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnRunSpace.Location = New System.Drawing.Point(3, 308)
        Me.m_btnRunSpace.Name = "m_btnRunSpace"
        Me.m_btnRunSpace.Size = New System.Drawing.Size(174, 22)
        Me.m_btnRunSpace.TabIndex = 3
        Me.m_btnRunSpace.Text = "Run Ecosp&ace"
        Me.m_btnRunSpace.UseVisualStyleBackColor = True
        '
        'm_btnRunSim
        '
        Me.m_btnRunSim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnRunSim.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnRunSim.Location = New System.Drawing.Point(3, 281)
        Me.m_btnRunSim.Name = "m_btnRunSim"
        Me.m_btnRunSim.Size = New System.Drawing.Size(174, 21)
        Me.m_btnRunSim.TabIndex = 2
        Me.m_btnRunSim.Text = "Run Ecos&im"
        Me.m_btnRunSim.UseVisualStyleBackColor = True
        '
        'frmEcotracerOutput
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(946, 505)
        Me.Controls.Add(Me.m_scMain)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcotracerOutput"
        Me.Text = "Ecotracer output"
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_hdrCommands As cEwEHeaderLabel
    Private WithEvents m_btnRunSim As System.Windows.Forms.Button
    Private WithEvents m_btnRunSpace As System.Windows.Forms.Button
    Private WithEvents m_hdrPlotOptions As cEwEHeaderLabel
    Private WithEvents m_btnShowHideGroups As System.Windows.Forms.Button
    Private WithEvents m_chkSortGroups As System.Windows.Forms.CheckBox
    Private WithEvents m_rbCB As System.Windows.Forms.RadioButton
    Private WithEvents m_rbConc As System.Windows.Forms.RadioButton
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_cmbRegions As System.Windows.Forms.ComboBox
    Private WithEvents m_lbGroups As cGroupListBox
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_lblRegion As System.Windows.Forms.Label
    Private WithEvents m_hdrGroups As cEwEHeaderLabel
End Class
