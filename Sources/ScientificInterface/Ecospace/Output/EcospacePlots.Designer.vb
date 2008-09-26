Namespace Ecospace
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcospacePlots
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
            Me.spcHorizontal = New System.Windows.Forms.SplitContainer
            Me.tlpGraphSelection = New System.Windows.Forms.TableLayoutPanel
            Me.lbRegionBox = New System.Windows.Forms.ListBox
            Me.lblRegion = New System.Windows.Forms.Label
            Me.lbSpecFltBox = New System.Windows.Forms.ListBox
            Me.lblSpeciesFlt = New System.Windows.Forms.Label
            Me.lblDataType = New System.Windows.Forms.Label
            Me.lbDataTypeBox = New System.Windows.Forms.ListBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.spcHorizontal.Panel1.SuspendLayout()
            Me.spcHorizontal.Panel2.SuspendLayout()
            Me.spcHorizontal.SuspendLayout()
            Me.tlpGraphSelection.SuspendLayout()
            Me.SuspendLayout()
            '
            'spcHorizontal
            '
            Me.spcHorizontal.Dock = System.Windows.Forms.DockStyle.Fill
            Me.spcHorizontal.Location = New System.Drawing.Point(0, 0)
            Me.spcHorizontal.Name = "spcHorizontal"
            Me.spcHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'spcHorizontal.Panel1
            '
            Me.spcHorizontal.Panel1.Controls.Add(Me.tlpGraphSelection)
            '
            'spcHorizontal.Panel2
            '
            Me.spcHorizontal.Panel2.Controls.Add(Me.m_zgc)
            Me.spcHorizontal.Size = New System.Drawing.Size(779, 516)
            Me.spcHorizontal.SplitterDistance = 145
            Me.spcHorizontal.TabIndex = 1
            '
            'tlpGraphSelection
            '
            Me.tlpGraphSelection.ColumnCount = 3
            Me.tlpGraphSelection.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33148!))
            Me.tlpGraphSelection.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33592!))
            Me.tlpGraphSelection.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3326!))
            Me.tlpGraphSelection.Controls.Add(Me.lbRegionBox, 0, 1)
            Me.tlpGraphSelection.Controls.Add(Me.lblRegion, 0, 0)
            Me.tlpGraphSelection.Controls.Add(Me.lbSpecFltBox, 2, 1)
            Me.tlpGraphSelection.Controls.Add(Me.lblSpeciesFlt, 2, 0)
            Me.tlpGraphSelection.Controls.Add(Me.lblDataType, 1, 0)
            Me.tlpGraphSelection.Controls.Add(Me.lbDataTypeBox, 1, 1)
            Me.tlpGraphSelection.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tlpGraphSelection.Location = New System.Drawing.Point(0, 0)
            Me.tlpGraphSelection.Name = "tlpGraphSelection"
            Me.tlpGraphSelection.RowCount = 2
            Me.tlpGraphSelection.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.tlpGraphSelection.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpGraphSelection.Size = New System.Drawing.Size(779, 145)
            Me.tlpGraphSelection.TabIndex = 0
            '
            'lbRegionBox
            '
            Me.lbRegionBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbRegionBox.FormattingEnabled = True
            Me.lbRegionBox.Location = New System.Drawing.Point(3, 23)
            Me.lbRegionBox.Name = "lbRegionBox"
            Me.lbRegionBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.lbRegionBox.Size = New System.Drawing.Size(253, 108)
            Me.lbRegionBox.TabIndex = 49
            '
            'lblRegion
            '
            Me.lblRegion.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblRegion.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblRegion.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblRegion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblRegion.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblRegion.Location = New System.Drawing.Point(3, 0)
            Me.lblRegion.Name = "lblRegion"
            Me.lblRegion.Size = New System.Drawing.Size(253, 20)
            Me.lblRegion.TabIndex = 35
            Me.lblRegion.Text = "(1) Region"
            Me.lblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lbSpecFltBox
            '
            Me.lbSpecFltBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbSpecFltBox.FormattingEnabled = True
            Me.lbSpecFltBox.Location = New System.Drawing.Point(521, 23)
            Me.lbSpecFltBox.Name = "lbSpecFltBox"
            Me.lbSpecFltBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.lbSpecFltBox.Size = New System.Drawing.Size(255, 108)
            Me.lbSpecFltBox.TabIndex = 47
            '
            'lblSpeciesFlt
            '
            Me.lblSpeciesFlt.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblSpeciesFlt.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblSpeciesFlt.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblSpeciesFlt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblSpeciesFlt.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblSpeciesFlt.Location = New System.Drawing.Point(521, 0)
            Me.lblSpeciesFlt.Name = "lblSpeciesFlt"
            Me.lblSpeciesFlt.Size = New System.Drawing.Size(255, 20)
            Me.lblSpeciesFlt.TabIndex = 42
            Me.lblSpeciesFlt.Text = "(3) Species/fleet"
            Me.lblSpeciesFlt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lblDataType
            '
            Me.lblDataType.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblDataType.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblDataType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblDataType.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblDataType.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblDataType.Location = New System.Drawing.Point(262, 0)
            Me.lblDataType.Name = "lblDataType"
            Me.lblDataType.Size = New System.Drawing.Size(253, 20)
            Me.lblDataType.TabIndex = 41
            Me.lblDataType.Text = "(2) Data type"
            Me.lblDataType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'lbDataTypeBox
            '
            Me.lbDataTypeBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbDataTypeBox.FormattingEnabled = True
            Me.lbDataTypeBox.Location = New System.Drawing.Point(262, 23)
            Me.lbDataTypeBox.Name = "lbDataTypeBox"
            Me.lbDataTypeBox.Size = New System.Drawing.Size(253, 108)
            Me.lbDataTypeBox.TabIndex = 48
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
            Me.m_zgc.Size = New System.Drawing.Size(779, 367)
            Me.m_zgc.TabIndex = 0
            '
            'EcospacePlots
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(779, 516)
            Me.Controls.Add(Me.spcHorizontal)
            Me.Name = "EcospacePlots"
            Me.Text = "EcospacePlots"
            Me.spcHorizontal.Panel1.ResumeLayout(False)
            Me.spcHorizontal.Panel2.ResumeLayout(False)
            Me.spcHorizontal.ResumeLayout(False)
            Me.tlpGraphSelection.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents spcHorizontal As System.Windows.Forms.SplitContainer
        Friend WithEvents tlpGraphSelection As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_zgc As ZedGraph.ZedGraphControl
        Friend WithEvents lblRegion As System.Windows.Forms.Label
        Friend WithEvents lbDataTypeBox As System.Windows.Forms.ListBox
        Friend WithEvents lbSpecFltBox As System.Windows.Forms.ListBox
        Friend WithEvents lblDataType As System.Windows.Forms.Label
        Friend WithEvents lbRegionBox As System.Windows.Forms.ListBox
        Friend WithEvents lblSpeciesFlt As System.Windows.Forms.Label
    End Class
End Namespace