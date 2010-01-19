<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucDefaults
    Inherits System.Windows.Forms.UserControl

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
        Me.m_pgDefaults = New System.Windows.Forms.PropertyGrid
        Me.m_scMain = New System.Windows.Forms.SplitContainer
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.m_lbProducer = New ucUnitDefault
        Me.m_lbProcessing = New ucUnitDefault
        Me.m_lbDistribution = New ucUnitDefault
        Me.m_lbMarket = New ucUnitDefault
        Me.m_lbConsumer = New ucUnitDefault
        Me.m_lnkProd2Proc = New ucLinkDefault
        Me.m_lnkProc2Dist = New ucLinkDefault
        Me.m_lnkDist2Mkt = New ucLinkDefault
        Me.m_lnkMkt2Cons = New ucLinkDefault
        Me.m_cbDefault = New System.Windows.Forms.ComboBox
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_pgDefaults
        '
        Me.m_pgDefaults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_pgDefaults.Location = New System.Drawing.Point(3, 30)
        Me.m_pgDefaults.Name = "m_pgDefaults"
        Me.m_pgDefaults.Size = New System.Drawing.Size(257, 450)
        Me.m_pgDefaults.TabIndex = 1
        '
        'm_scMain
        '
        Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_scMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_scMain.Location = New System.Drawing.Point(0, 0)
        Me.m_scMain.Margin = New System.Windows.Forms.Padding(0)
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.TableLayoutPanel1)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_cbDefault)
        Me.m_scMain.Panel2.Controls.Add(Me.m_pgDefaults)
        Me.m_scMain.Size = New System.Drawing.Size(452, 487)
        Me.m_scMain.SplitterDistance = 181
        Me.m_scMain.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbProducer, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbProcessing, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbDistribution, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbMarket, 1, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbConsumer, 1, 9)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lnkProd2Proc, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lnkProc2Dist, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lnkDist2Mkt, 1, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lnkMkt2Cons, 1, 8)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 11
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(177, 483)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'm_lbProducer
        '
        Me.m_lbProducer.BackColor = System.Drawing.SystemColors.Window
        Me.m_lbProducer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbProducer.Location = New System.Drawing.Point(38, 60)
        Me.m_lbProducer.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbProducer.Name = "m_lbProducer"
        Me.m_lbProducer.ObjDefault = Nothing
        Me.m_lbProducer.Selected = False
        Me.m_lbProducer.Size = New System.Drawing.Size(100, 40)
        Me.m_lbProducer.TabIndex = 0
        '
        'm_lbProcessing
        '
        Me.m_lbProcessing.BackColor = System.Drawing.SystemColors.Window
        Me.m_lbProcessing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbProcessing.Location = New System.Drawing.Point(38, 140)
        Me.m_lbProcessing.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbProcessing.Name = "m_lbProcessing"
        Me.m_lbProcessing.ObjDefault = Nothing
        Me.m_lbProcessing.Selected = False
        Me.m_lbProcessing.Size = New System.Drawing.Size(100, 40)
        Me.m_lbProcessing.TabIndex = 0
        '
        'm_lbDistribution
        '
        Me.m_lbDistribution.BackColor = System.Drawing.SystemColors.Window
        Me.m_lbDistribution.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbDistribution.Location = New System.Drawing.Point(38, 220)
        Me.m_lbDistribution.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbDistribution.Name = "m_lbDistribution"
        Me.m_lbDistribution.ObjDefault = Nothing
        Me.m_lbDistribution.Selected = False
        Me.m_lbDistribution.Size = New System.Drawing.Size(100, 40)
        Me.m_lbDistribution.TabIndex = 0
        '
        'm_lbMarket
        '
        Me.m_lbMarket.BackColor = System.Drawing.SystemColors.Window
        Me.m_lbMarket.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbMarket.Location = New System.Drawing.Point(38, 300)
        Me.m_lbMarket.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbMarket.Name = "m_lbMarket"
        Me.m_lbMarket.ObjDefault = Nothing
        Me.m_lbMarket.Selected = False
        Me.m_lbMarket.Size = New System.Drawing.Size(100, 40)
        Me.m_lbMarket.TabIndex = 0
        '
        'm_lbConsumer
        '
        Me.m_lbConsumer.BackColor = System.Drawing.SystemColors.Window
        Me.m_lbConsumer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lbConsumer.Location = New System.Drawing.Point(38, 380)
        Me.m_lbConsumer.Margin = New System.Windows.Forms.Padding(0)
        Me.m_lbConsumer.Name = "m_lbConsumer"
        Me.m_lbConsumer.ObjDefault = Nothing
        Me.m_lbConsumer.Selected = False
        Me.m_lbConsumer.Size = New System.Drawing.Size(100, 40)
        Me.m_lbConsumer.TabIndex = 0
        '
        'm_lnkProd2Proc
        '
        Me.m_lnkProd2Proc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lnkProd2Proc.Location = New System.Drawing.Point(41, 103)
        Me.m_lnkProd2Proc.Name = "m_lnkProd2Proc"
        Me.m_lnkProd2Proc.ObjDefault = Nothing
        Me.m_lnkProd2Proc.Selected = False
        Me.m_lnkProd2Proc.Size = New System.Drawing.Size(94, 34)
        Me.m_lnkProd2Proc.TabIndex = 1
        '
        'm_lnkProc2Dist
        '
        Me.m_lnkProc2Dist.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lnkProc2Dist.Location = New System.Drawing.Point(41, 183)
        Me.m_lnkProc2Dist.Name = "m_lnkProc2Dist"
        Me.m_lnkProc2Dist.ObjDefault = Nothing
        Me.m_lnkProc2Dist.Selected = False
        Me.m_lnkProc2Dist.Size = New System.Drawing.Size(94, 34)
        Me.m_lnkProc2Dist.TabIndex = 1
        '
        'm_lnkDist2Mkt
        '
        Me.m_lnkDist2Mkt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lnkDist2Mkt.Location = New System.Drawing.Point(41, 263)
        Me.m_lnkDist2Mkt.Name = "m_lnkDist2Mkt"
        Me.m_lnkDist2Mkt.ObjDefault = Nothing
        Me.m_lnkDist2Mkt.Selected = False
        Me.m_lnkDist2Mkt.Size = New System.Drawing.Size(94, 34)
        Me.m_lnkDist2Mkt.TabIndex = 1
        '
        'm_lnkMkt2Cons
        '
        Me.m_lnkMkt2Cons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lnkMkt2Cons.Location = New System.Drawing.Point(41, 343)
        Me.m_lnkMkt2Cons.Name = "m_lnkMkt2Cons"
        Me.m_lnkMkt2Cons.ObjDefault = Nothing
        Me.m_lnkMkt2Cons.Selected = False
        Me.m_lnkMkt2Cons.Size = New System.Drawing.Size(94, 34)
        Me.m_lnkMkt2Cons.TabIndex = 1
        '
        'm_cbDefault
        '
        Me.m_cbDefault.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_cbDefault.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cbDefault.FormattingEnabled = True
        Me.m_cbDefault.Location = New System.Drawing.Point(3, 3)
        Me.m_cbDefault.Name = "m_cbDefault"
        Me.m_cbDefault.Size = New System.Drawing.Size(257, 21)
        Me.m_cbDefault.TabIndex = 0
        '
        'ucDefaults
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scMain)
        Me.Name = "ucDefaults"
        Me.Size = New System.Drawing.Size(452, 487)
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_pgDefaults As System.Windows.Forms.PropertyGrid
    Friend WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents m_lbProducer As ucUnitDefault
    Friend WithEvents m_cbDefault As System.Windows.Forms.ComboBox
    Friend WithEvents m_lbProcessing As ucUnitDefault
    Friend WithEvents m_lbDistribution As ucUnitDefault
    Friend WithEvents m_lbMarket As ucUnitDefault
    Friend WithEvents m_lbConsumer As ucUnitDefault
    Friend WithEvents m_lnkProd2Proc As ucLinkDefault
    Friend WithEvents m_lnkProc2Dist As ucLinkDefault
    Friend WithEvents m_lnkDist2Mkt As ucLinkDefault
    Friend WithEvents m_lnkMkt2Cons As ucLinkDefault
End Class
