Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmShowAllFits
        Inherits System.Windows.Forms.Form

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShowAllFits))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.btnClose = New System.Windows.Forms.Button
            Me.gpbDisplay = New System.Windows.Forms.GroupBox
            Me.clbOptions = New System.Windows.Forms.CheckedListBox
            Me.gpbGeneral = New System.Windows.Forms.GroupBox
            Me.txbLineWidth = New System.Windows.Forms.TextBox
            Me.txbTBMargin = New System.Windows.Forms.TextBox
            Me.txbLRMargin = New System.Windows.Forms.TextBox
            Me.txbDotSize = New System.Windows.Forms.TextBox
            Me.lblRowNum = New System.Windows.Forms.Label
            Me.btnFModify = New System.Windows.Forms.Button
            Me.cbScaleFP = New System.Windows.Forms.CheckBox
            Me.lblTBMargin = New System.Windows.Forms.Label
            Me.lblLRMargin = New System.Windows.Forms.Label
            Me.lblDotSize = New System.Windows.Forms.Label
            Me.lblLineWidth = New System.Windows.Forms.Label
            Me.txbPlotsPerRow = New System.Windows.Forms.TextBox
            Me.lblFont = New System.Windows.Forms.Label
            Me.plPlots = New System.Windows.Forms.Panel
            Me.pbPlots = New System.Windows.Forms.PictureBox
            Me.tsCommands = New System.Windows.Forms.ToolStrip
            Me.tsBtnHSPlots = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnSaveImage = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnSaveData = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnChangeYScale = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnPrint = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnPrintPreview = New System.Windows.Forms.ToolStripButton
            Me.epInput = New System.Windows.Forms.ErrorProvider(Me.components)
            Me.pdAllFits = New System.Drawing.Printing.PrintDocument
            Me.dlgPV = New System.Windows.Forms.PrintPreviewDialog
            Me.PrintDialog1 = New System.Windows.Forms.PrintDialog
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.gpbDisplay.SuspendLayout()
            Me.gpbGeneral.SuspendLayout()
            Me.plPlots.SuspendLayout()
            CType(Me.pbPlots, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tsCommands.SuspendLayout()
            CType(Me.epInput, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            resources.ApplyResources(Me.SplitContainer1.Panel1, "SplitContainer1.Panel1")
            Me.SplitContainer1.Panel1.Controls.Add(Me.btnClose)
            Me.SplitContainer1.Panel1.Controls.Add(Me.gpbDisplay)
            Me.SplitContainer1.Panel1.Controls.Add(Me.gpbGeneral)
            '
            'SplitContainer1.Panel2
            '
            resources.ApplyResources(Me.SplitContainer1.Panel2, "SplitContainer1.Panel2")
            Me.SplitContainer1.Panel2.Controls.Add(Me.plPlots)
            Me.SplitContainer1.Panel2.Controls.Add(Me.tsCommands)
            '
            'btnClose
            '
            resources.ApplyResources(Me.btnClose, "btnClose")
            Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnClose.Name = "btnClose"
            Me.btnClose.UseVisualStyleBackColor = True
            '
            'gpbDisplay
            '
            resources.ApplyResources(Me.gpbDisplay, "gpbDisplay")
            Me.gpbDisplay.Controls.Add(Me.clbOptions)
            Me.gpbDisplay.Name = "gpbDisplay"
            Me.gpbDisplay.TabStop = False
            '
            'clbOptions
            '
            Me.clbOptions.CheckOnClick = True
            resources.ApplyResources(Me.clbOptions, "clbOptions")
            Me.clbOptions.FormattingEnabled = True
            Me.clbOptions.Items.AddRange(New Object() {resources.GetString("clbOptions.Items"), resources.GetString("clbOptions.Items1"), resources.GetString("clbOptions.Items2"), resources.GetString("clbOptions.Items3"), resources.GetString("clbOptions.Items4")})
            Me.clbOptions.Name = "clbOptions"
            '
            'gpbGeneral
            '
            resources.ApplyResources(Me.gpbGeneral, "gpbGeneral")
            Me.gpbGeneral.Controls.Add(Me.txbLineWidth)
            Me.gpbGeneral.Controls.Add(Me.txbTBMargin)
            Me.gpbGeneral.Controls.Add(Me.txbLRMargin)
            Me.gpbGeneral.Controls.Add(Me.txbDotSize)
            Me.gpbGeneral.Controls.Add(Me.lblRowNum)
            Me.gpbGeneral.Controls.Add(Me.btnFModify)
            Me.gpbGeneral.Controls.Add(Me.cbScaleFP)
            Me.gpbGeneral.Controls.Add(Me.lblTBMargin)
            Me.gpbGeneral.Controls.Add(Me.lblLRMargin)
            Me.gpbGeneral.Controls.Add(Me.lblDotSize)
            Me.gpbGeneral.Controls.Add(Me.lblLineWidth)
            Me.gpbGeneral.Controls.Add(Me.txbPlotsPerRow)
            Me.gpbGeneral.Controls.Add(Me.lblFont)
            Me.gpbGeneral.Name = "gpbGeneral"
            Me.gpbGeneral.TabStop = False
            '
            'txbLineWidth
            '
            resources.ApplyResources(Me.txbLineWidth, "txbLineWidth")
            Me.txbLineWidth.Name = "txbLineWidth"
            '
            'txbTBMargin
            '
            resources.ApplyResources(Me.txbTBMargin, "txbTBMargin")
            Me.txbTBMargin.Name = "txbTBMargin"
            '
            'txbLRMargin
            '
            resources.ApplyResources(Me.txbLRMargin, "txbLRMargin")
            Me.txbLRMargin.Name = "txbLRMargin"
            '
            'txbDotSize
            '
            resources.ApplyResources(Me.txbDotSize, "txbDotSize")
            Me.txbDotSize.Name = "txbDotSize"
            '
            'lblRowNum
            '
            resources.ApplyResources(Me.lblRowNum, "lblRowNum")
            Me.lblRowNum.Name = "lblRowNum"
            '
            'btnFModify
            '
            resources.ApplyResources(Me.btnFModify, "btnFModify")
            Me.btnFModify.Name = "btnFModify"
            Me.btnFModify.UseVisualStyleBackColor = True
            '
            'cbScaleFP
            '
            resources.ApplyResources(Me.cbScaleFP, "cbScaleFP")
            Me.cbScaleFP.Name = "cbScaleFP"
            Me.cbScaleFP.UseVisualStyleBackColor = True
            '
            'lblTBMargin
            '
            resources.ApplyResources(Me.lblTBMargin, "lblTBMargin")
            Me.lblTBMargin.Name = "lblTBMargin"
            '
            'lblLRMargin
            '
            resources.ApplyResources(Me.lblLRMargin, "lblLRMargin")
            Me.lblLRMargin.Name = "lblLRMargin"
            '
            'lblDotSize
            '
            resources.ApplyResources(Me.lblDotSize, "lblDotSize")
            Me.lblDotSize.Name = "lblDotSize"
            '
            'lblLineWidth
            '
            resources.ApplyResources(Me.lblLineWidth, "lblLineWidth")
            Me.lblLineWidth.Name = "lblLineWidth"
            '
            'txbPlotsPerRow
            '
            resources.ApplyResources(Me.txbPlotsPerRow, "txbPlotsPerRow")
            Me.txbPlotsPerRow.Name = "txbPlotsPerRow"
            '
            'lblFont
            '
            resources.ApplyResources(Me.lblFont, "lblFont")
            Me.lblFont.Name = "lblFont"
            '
            'plPlots
            '
            Me.plPlots.BackColor = System.Drawing.SystemColors.Control
            Me.plPlots.Controls.Add(Me.pbPlots)
            resources.ApplyResources(Me.plPlots, "plPlots")
            Me.plPlots.Name = "plPlots"
            '
            'pbPlots
            '
            Me.pbPlots.BackColor = System.Drawing.Color.White
            resources.ApplyResources(Me.pbPlots, "pbPlots")
            Me.pbPlots.Name = "pbPlots"
            Me.pbPlots.TabStop = False
            '
            'tsCommands
            '
            Me.tsCommands.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnHSPlots, Me.ToolStripSeparator1, Me.tsBtnSaveImage, Me.ToolStripSeparator2, Me.tsBtnSaveData, Me.ToolStripSeparator3, Me.tsBtnChangeYScale, Me.ToolStripSeparator4, Me.tsBtnPrint, Me.ToolStripSeparator5, Me.tsBtnPrintPreview})
            resources.ApplyResources(Me.tsCommands, "tsCommands")
            Me.tsCommands.Name = "tsCommands"
            Me.tsCommands.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'tsBtnHSPlots
            '
            Me.tsBtnHSPlots.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnHSPlots, "tsBtnHSPlots")
            Me.tsBtnHSPlots.Name = "tsBtnHSPlots"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsBtnSaveImage
            '
            Me.tsBtnSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnSaveImage, "tsBtnSaveImage")
            Me.tsBtnSaveImage.Name = "tsBtnSaveImage"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
            '
            'tsBtnSaveData
            '
            Me.tsBtnSaveData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnSaveData, "tsBtnSaveData")
            Me.tsBtnSaveData.Name = "tsBtnSaveData"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
            '
            'tsBtnChangeYScale
            '
            Me.tsBtnChangeYScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnChangeYScale, "tsBtnChangeYScale")
            Me.tsBtnChangeYScale.Name = "tsBtnChangeYScale"
            '
            'ToolStripSeparator4
            '
            Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
            resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
            '
            'tsBtnPrint
            '
            Me.tsBtnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnPrint, "tsBtnPrint")
            Me.tsBtnPrint.Name = "tsBtnPrint"
            '
            'ToolStripSeparator5
            '
            Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
            resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
            '
            'tsBtnPrintPreview
            '
            Me.tsBtnPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnPrintPreview, "tsBtnPrintPreview")
            Me.tsBtnPrintPreview.Name = "tsBtnPrintPreview"
            '
            'epInput
            '
            Me.epInput.ContainerControl = Me
            '
            'pdAllFits
            '
            '
            'dlgPV
            '
            resources.ApplyResources(Me.dlgPV, "dlgPV")
            Me.dlgPV.Name = "dlgPV"
            '
            'PrintDialog1
            '
            Me.PrintDialog1.UseEXDialog = True
            '
            'ShowAllFits
            '
            Me.AcceptButton = Me.btnClose
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.btnClose
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "ShowAllFits"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.Panel2.PerformLayout()
            Me.SplitContainer1.ResumeLayout(False)
            Me.gpbDisplay.ResumeLayout(False)
            Me.gpbGeneral.ResumeLayout(False)
            Me.gpbGeneral.PerformLayout()
            Me.plPlots.ResumeLayout(False)
            CType(Me.pbPlots, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tsCommands.ResumeLayout(False)
            Me.tsCommands.PerformLayout()
            CType(Me.epInput, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gpbDisplay As System.Windows.Forms.GroupBox
        Friend WithEvents clbOptions As System.Windows.Forms.CheckedListBox
        Friend WithEvents gpbGeneral As System.Windows.Forms.GroupBox
        Friend WithEvents lblTBMargin As System.Windows.Forms.Label
        Friend WithEvents lblLRMargin As System.Windows.Forms.Label
        Friend WithEvents lblDotSize As System.Windows.Forms.Label
        Friend WithEvents lblLineWidth As System.Windows.Forms.Label
        Friend WithEvents txbPlotsPerRow As System.Windows.Forms.TextBox
        Friend WithEvents lblFont As System.Windows.Forms.Label
        Friend WithEvents tsCommands As System.Windows.Forms.ToolStrip
        Friend WithEvents cbScaleFP As System.Windows.Forms.CheckBox
        Friend WithEvents tsBtnHSPlots As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsBtnSaveImage As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsBtnSaveData As System.Windows.Forms.ToolStripButton
        Friend WithEvents lblRowNum As System.Windows.Forms.Label
        Friend WithEvents btnFModify As System.Windows.Forms.Button
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents plPlots As System.Windows.Forms.Panel
        Friend WithEvents txbLineWidth As System.Windows.Forms.TextBox
        Friend WithEvents txbTBMargin As System.Windows.Forms.TextBox
        Friend WithEvents txbLRMargin As System.Windows.Forms.TextBox
        Friend WithEvents txbDotSize As System.Windows.Forms.TextBox
        Friend WithEvents btnClose As System.Windows.Forms.Button
        Friend WithEvents epInput As System.Windows.Forms.ErrorProvider
        Friend WithEvents pbPlots As System.Windows.Forms.PictureBox
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsBtnChangeYScale As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsBtnPrint As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents tsBtnPrintPreview As System.Windows.Forms.ToolStripButton
        Friend WithEvents pdAllFits As System.Drawing.Printing.PrintDocument
        Friend WithEvents dlgPV As System.Windows.Forms.PrintPreviewDialog
        Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    End Class

End Namespace

