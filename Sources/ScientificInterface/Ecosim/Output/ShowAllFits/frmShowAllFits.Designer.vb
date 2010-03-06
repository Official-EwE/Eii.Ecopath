Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmShowAllFits
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShowAllFits))
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_nudMarginTB = New System.Windows.Forms.NumericUpDown
            Me.m_nudMarginLR = New System.Windows.Forms.NumericUpDown
            Me.m_nudDotSize = New System.Windows.Forms.NumericUpDown
            Me.m_nudLineWidth = New System.Windows.Forms.NumericUpDown
            Me.m_nudRowNum = New System.Windows.Forms.NumericUpDown
            Me.m_chkScaleForPrinter = New System.Windows.Forms.CheckBox
            Me.m_clbOptions = New System.Windows.Forms.CheckedListBox
            Me.lblTBMargin = New System.Windows.Forms.Label
            Me.m_lblMarginLR = New System.Windows.Forms.Label
            Me.m_lblDisplayOptions = New System.Windows.Forms.Label
            Me.m_lblGeneral = New System.Windows.Forms.Label
            Me.m_lblRowNum = New System.Windows.Forms.Label
            Me.m_lblLineWidth = New System.Windows.Forms.Label
            Me.m_lblDotSize = New System.Windows.Forms.Label
            Me.plPlots = New System.Windows.Forms.Panel
            Me.m_pbPlots = New System.Windows.Forms.PictureBox
            Me.m_tsMain = New System.Windows.Forms.ToolStrip
            Me.m_tsmiOptions = New System.Windows.Forms.ToolStripButton
            Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsmiChoosePlots = New System.Windows.Forms.ToolStripButton
            Me.m_tsbnScale = New System.Windows.Forms.ToolStripButton
            Me.m_sep2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsddSave = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiSaveAsImage = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiSaveAsCSV = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsddPrint = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiPrint = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiPrintPreview = New System.Windows.Forms.ToolStripMenuItem
            Me.m_printdocAllFits = New System.Drawing.Printing.PrintDocument
            Me.dlgPV = New System.Windows.Forms.PrintPreviewDialog
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            CType(Me.m_nudMarginTB, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMarginLR, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudDotSize, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudLineWidth, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRowNum, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.plPlots.SuspendLayout()
            CType(Me.m_pbPlots, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tsMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            resources.ApplyResources(Me.m_scMain.Panel1, "m_scMain.Panel1")
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudMarginTB)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudMarginLR)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudDotSize)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudLineWidth)
            Me.m_scMain.Panel1.Controls.Add(Me.m_nudRowNum)
            Me.m_scMain.Panel1.Controls.Add(Me.m_chkScaleForPrinter)
            Me.m_scMain.Panel1.Controls.Add(Me.m_clbOptions)
            Me.m_scMain.Panel1.Controls.Add(Me.lblTBMargin)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblMarginLR)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDisplayOptions)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblGeneral)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblRowNum)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblLineWidth)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDotSize)
            '
            'm_scMain.Panel2
            '
            resources.ApplyResources(Me.m_scMain.Panel2, "m_scMain.Panel2")
            Me.m_scMain.Panel2.Controls.Add(Me.plPlots)
            Me.m_scMain.Panel2.Controls.Add(Me.m_tsMain)
            '
            'm_nudMarginTB
            '
            resources.ApplyResources(Me.m_nudMarginTB, "m_nudMarginTB")
            Me.m_nudMarginTB.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudMarginTB.Name = "m_nudMarginTB"
            '
            'm_nudMarginLR
            '
            resources.ApplyResources(Me.m_nudMarginLR, "m_nudMarginLR")
            Me.m_nudMarginLR.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudMarginLR.Name = "m_nudMarginLR"
            '
            'm_nudDotSize
            '
            resources.ApplyResources(Me.m_nudDotSize, "m_nudDotSize")
            Me.m_nudDotSize.DecimalPlaces = 2
            Me.m_nudDotSize.Name = "m_nudDotSize"
            Me.m_nudDotSize.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_nudLineWidth
            '
            resources.ApplyResources(Me.m_nudLineWidth, "m_nudLineWidth")
            Me.m_nudLineWidth.DecimalPlaces = 1
            Me.m_nudLineWidth.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
            Me.m_nudLineWidth.Maximum = New Decimal(New Integer() {5, 0, 0, 0})
            Me.m_nudLineWidth.Minimum = New Decimal(New Integer() {1, 0, 0, 131072})
            Me.m_nudLineWidth.Name = "m_nudLineWidth"
            Me.m_nudLineWidth.Value = New Decimal(New Integer() {1, 0, 0, 131072})
            '
            'm_nudRowNum
            '
            resources.ApplyResources(Me.m_nudRowNum, "m_nudRowNum")
            Me.m_nudRowNum.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudRowNum.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudRowNum.Name = "m_nudRowNum"
            Me.m_nudRowNum.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_chkScaleForPrinter
            '
            resources.ApplyResources(Me.m_chkScaleForPrinter, "m_chkScaleForPrinter")
            Me.m_chkScaleForPrinter.Name = "m_chkScaleForPrinter"
            Me.m_chkScaleForPrinter.UseVisualStyleBackColor = True
            '
            'm_clbOptions
            '
            resources.ApplyResources(Me.m_clbOptions, "m_clbOptions")
            Me.m_clbOptions.CheckOnClick = True
            Me.m_clbOptions.FormattingEnabled = True
            Me.m_clbOptions.Items.AddRange(New Object() {resources.GetString("m_clbOptions.Items"), resources.GetString("m_clbOptions.Items1"), resources.GetString("m_clbOptions.Items2"), resources.GetString("m_clbOptions.Items3"), resources.GetString("m_clbOptions.Items4")})
            Me.m_clbOptions.Name = "m_clbOptions"
            '
            'lblTBMargin
            '
            resources.ApplyResources(Me.lblTBMargin, "lblTBMargin")
            Me.lblTBMargin.Name = "lblTBMargin"
            '
            'm_lblMarginLR
            '
            resources.ApplyResources(Me.m_lblMarginLR, "m_lblMarginLR")
            Me.m_lblMarginLR.Name = "m_lblMarginLR"
            '
            'm_lblDisplayOptions
            '
            resources.ApplyResources(Me.m_lblDisplayOptions, "m_lblDisplayOptions")
            Me.m_lblDisplayOptions.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblDisplayOptions.ForeColor = System.Drawing.SystemColors.Window
            Me.m_lblDisplayOptions.Name = "m_lblDisplayOptions"
            '
            'm_lblGeneral
            '
            resources.ApplyResources(Me.m_lblGeneral, "m_lblGeneral")
            Me.m_lblGeneral.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblGeneral.ForeColor = System.Drawing.SystemColors.Window
            Me.m_lblGeneral.Name = "m_lblGeneral"
            '
            'm_lblRowNum
            '
            resources.ApplyResources(Me.m_lblRowNum, "m_lblRowNum")
            Me.m_lblRowNum.Name = "m_lblRowNum"
            '
            'm_lblLineWidth
            '
            resources.ApplyResources(Me.m_lblLineWidth, "m_lblLineWidth")
            Me.m_lblLineWidth.Name = "m_lblLineWidth"
            '
            'm_lblDotSize
            '
            resources.ApplyResources(Me.m_lblDotSize, "m_lblDotSize")
            Me.m_lblDotSize.Name = "m_lblDotSize"
            '
            'plPlots
            '
            Me.plPlots.BackColor = System.Drawing.SystemColors.Control
            Me.plPlots.Controls.Add(Me.m_pbPlots)
            resources.ApplyResources(Me.plPlots, "plPlots")
            Me.plPlots.Name = "plPlots"
            '
            'm_pbPlots
            '
            Me.m_pbPlots.BackColor = System.Drawing.Color.White
            resources.ApplyResources(Me.m_pbPlots, "m_pbPlots")
            Me.m_pbPlots.Name = "m_pbPlots"
            Me.m_pbPlots.TabStop = False
            '
            'm_tsMain
            '
            Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiOptions, Me.m_sep1, Me.m_tsmiChoosePlots, Me.m_tsbnScale, Me.m_sep2, Me.m_tsddSave, Me.m_tsddPrint})
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.Name = "m_tsMain"
            '
            'm_tsmiOptions
            '
            Me.m_tsmiOptions.Checked = True
            Me.m_tsmiOptions.CheckOnClick = True
            Me.m_tsmiOptions.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiOptions.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            resources.ApplyResources(Me.m_tsmiOptions, "m_tsmiOptions")
            Me.m_tsmiOptions.Name = "m_tsmiOptions"
            '
            'm_sep1
            '
            Me.m_sep1.Name = "m_sep1"
            resources.ApplyResources(Me.m_sep1, "m_sep1")
            '
            'm_tsmiChoosePlots
            '
            Me.m_tsmiChoosePlots.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            resources.ApplyResources(Me.m_tsmiChoosePlots, "m_tsmiChoosePlots")
            Me.m_tsmiChoosePlots.Name = "m_tsmiChoosePlots"
            '
            'm_tsbnScale
            '
            Me.m_tsbnScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnScale, "m_tsbnScale")
            Me.m_tsbnScale.Name = "m_tsbnScale"
            '
            'm_sep2
            '
            Me.m_sep2.Name = "m_sep2"
            resources.ApplyResources(Me.m_sep2, "m_sep2")
            '
            'm_tsddSave
            '
            Me.m_tsddSave.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiSaveAsImage, Me.m_tsmiSaveAsCSV})
            Me.m_tsddSave.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
            resources.ApplyResources(Me.m_tsddSave, "m_tsddSave")
            Me.m_tsddSave.Name = "m_tsddSave"
            '
            'm_tsmiSaveAsImage
            '
            Me.m_tsmiSaveAsImage.Image = Global.ScientificInterface.My.Resources.Resources.InsertPictureHS
            Me.m_tsmiSaveAsImage.Name = "m_tsmiSaveAsImage"
            resources.ApplyResources(Me.m_tsmiSaveAsImage, "m_tsmiSaveAsImage")
            '
            'm_tsmiSaveAsCSV
            '
            Me.m_tsmiSaveAsCSV.Image = Global.ScientificInterface.My.Resources.Resources.ExportXMLHS
            Me.m_tsmiSaveAsCSV.Name = "m_tsmiSaveAsCSV"
            resources.ApplyResources(Me.m_tsmiSaveAsCSV, "m_tsmiSaveAsCSV")
            '
            'm_tsddPrint
            '
            Me.m_tsddPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiPrint, Me.m_tsmiPrintPreview})
            Me.m_tsddPrint.Image = Global.ScientificInterface.My.Resources.Resources.PrintHS
            resources.ApplyResources(Me.m_tsddPrint, "m_tsddPrint")
            Me.m_tsddPrint.Name = "m_tsddPrint"
            '
            'm_tsmiPrint
            '
            Me.m_tsmiPrint.Image = Global.ScientificInterface.My.Resources.Resources.PrintHS
            Me.m_tsmiPrint.Name = "m_tsmiPrint"
            resources.ApplyResources(Me.m_tsmiPrint, "m_tsmiPrint")
            '
            'm_tsmiPrintPreview
            '
            Me.m_tsmiPrintPreview.Image = Global.ScientificInterface.My.Resources.Resources.PrintPreviewHS
            Me.m_tsmiPrintPreview.Name = "m_tsmiPrintPreview"
            resources.ApplyResources(Me.m_tsmiPrintPreview, "m_tsmiPrintPreview")
            '
            'm_printdocAllFits
            '
            '
            'dlgPV
            '
            resources.ApplyResources(Me.dlgPV, "dlgPV")
            Me.dlgPV.Name = "dlgPV"
            '
            'frmShowAllFits
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmShowAllFits"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.Panel2.PerformLayout()
            Me.m_scMain.ResumeLayout(False)
            CType(Me.m_nudMarginTB, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMarginLR, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudDotSize, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudLineWidth, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRowNum, System.ComponentModel.ISupportInitialize).EndInit()
            Me.plPlots.ResumeLayout(False)
            CType(Me.m_pbPlots, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents lblTBMargin As System.Windows.Forms.Label
        Private WithEvents m_lblMarginLR As System.Windows.Forms.Label
        Private WithEvents m_lblDotSize As System.Windows.Forms.Label
        Private WithEvents m_lblLineWidth As System.Windows.Forms.Label
        Private WithEvents plPlots As System.Windows.Forms.Panel
        Private WithEvents m_sep2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_printdocAllFits As System.Drawing.Printing.PrintDocument
        Private WithEvents dlgPV As System.Windows.Forms.PrintPreviewDialog
        Private WithEvents m_lblDisplayOptions As System.Windows.Forms.Label
        Private WithEvents m_clbOptions As System.Windows.Forms.CheckedListBox
        Private WithEvents m_lblGeneral As System.Windows.Forms.Label
        Private WithEvents m_pbPlots As System.Windows.Forms.PictureBox
        Private WithEvents m_nudRowNum As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudLineWidth As System.Windows.Forms.NumericUpDown
        Private WithEvents m_tsMain As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsmiChoosePlots As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnScale As System.Windows.Forms.ToolStripButton
        Private WithEvents m_nudDotSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblRowNum As System.Windows.Forms.Label
        Private WithEvents m_nudMarginLR As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudMarginTB As System.Windows.Forms.NumericUpDown
        Private WithEvents m_chkScaleForPrinter As System.Windows.Forms.CheckBox
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_tsddSave As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiSaveAsImage As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiSaveAsCSV As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsddPrint As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiPrint As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiPrintPreview As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiOptions As System.Windows.Forms.ToolStripButton
        Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
    End Class

End Namespace

