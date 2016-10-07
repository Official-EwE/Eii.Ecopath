<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgSpaceRun
    Inherits System.Windows.Forms.Form

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
        Me.m_lblFileHist = New System.Windows.Forms.Label()
        Me.m_tbxFileHist = New System.Windows.Forms.TextBox()
        Me.m_lblYearHist = New System.Windows.Forms.Label()
        Me.m_tbxYearHist = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.m_hdrHist = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblFileFore = New System.Windows.Forms.Label()
        Me.m_lblYearFore = New System.Windows.Forms.Label()
        Me.m_tbxFileFore = New System.Windows.Forms.TextBox()
        Me.m_tbxYearFore = New System.Windows.Forms.TextBox()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrOther = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblNoData = New System.Windows.Forms.Label()
        Me.m_tbxNoData = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'm_lblFileHist
        '
        Me.m_lblFileHist.AutoSize = True
        Me.m_lblFileHist.Location = New System.Drawing.Point(12, 62)
        Me.m_lblFileHist.Name = "m_lblFileHist"
        Me.m_lblFileHist.Size = New System.Drawing.Size(26, 13)
        Me.m_lblFileHist.TabIndex = 0
        Me.m_lblFileHist.Text = "&File:"
        '
        'm_tbxFileHist
        '
        Me.m_tbxFileHist.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFileHist.Location = New System.Drawing.Point(66, 59)
        Me.m_tbxFileHist.Name = "m_tbxFileHist"
        Me.m_tbxFileHist.Size = New System.Drawing.Size(254, 20)
        Me.m_tbxFileHist.TabIndex = 1
        '
        'm_lblYearHist
        '
        Me.m_lblYearHist.AutoSize = True
        Me.m_lblYearHist.Location = New System.Drawing.Point(12, 36)
        Me.m_lblYearHist.Name = "m_lblYearHist"
        Me.m_lblYearHist.Size = New System.Drawing.Size(32, 13)
        Me.m_lblYearHist.TabIndex = 0
        Me.m_lblYearHist.Text = "&Year:"
        '
        'm_tbxYearHist
        '
        Me.m_tbxYearHist.Location = New System.Drawing.Point(66, 33)
        Me.m_tbxYearHist.Name = "m_tbxYearHist"
        Me.m_tbxYearHist.Size = New System.Drawing.Size(61, 20)
        Me.m_tbxYearHist.TabIndex = 2
        Me.m_tbxYearHist.Text = "1978"
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(245, 228)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'm_hdrHist
        '
        Me.m_hdrHist.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrHist.CanCollapseParent = False
        Me.m_hdrHist.CollapsedParentHeight = 0
        Me.m_hdrHist.IsCollapsed = False
        Me.m_hdrHist.Location = New System.Drawing.Point(12, 9)
        Me.m_hdrHist.Name = "m_hdrHist"
        Me.m_hdrHist.Size = New System.Drawing.Size(308, 18)
        Me.m_hdrHist.TabIndex = 4
        Me.m_hdrHist.Text = "Historical reporting"
        Me.m_hdrHist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblFileFore
        '
        Me.m_lblFileFore.AutoSize = True
        Me.m_lblFileFore.Location = New System.Drawing.Point(12, 140)
        Me.m_lblFileFore.Name = "m_lblFileFore"
        Me.m_lblFileFore.Size = New System.Drawing.Size(26, 13)
        Me.m_lblFileFore.TabIndex = 0
        Me.m_lblFileFore.Text = "&File:"
        '
        'm_lblYearFore
        '
        Me.m_lblYearFore.AutoSize = True
        Me.m_lblYearFore.Location = New System.Drawing.Point(12, 114)
        Me.m_lblYearFore.Name = "m_lblYearFore"
        Me.m_lblYearFore.Size = New System.Drawing.Size(32, 13)
        Me.m_lblYearFore.TabIndex = 0
        Me.m_lblYearFore.Text = "&Year:"
        '
        'm_tbxFileFore
        '
        Me.m_tbxFileFore.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFileFore.Location = New System.Drawing.Point(66, 137)
        Me.m_tbxFileFore.Name = "m_tbxFileFore"
        Me.m_tbxFileFore.Size = New System.Drawing.Size(254, 20)
        Me.m_tbxFileFore.TabIndex = 1
        '
        'm_tbxYearFore
        '
        Me.m_tbxYearFore.Location = New System.Drawing.Point(66, 111)
        Me.m_tbxYearFore.Name = "m_tbxYearFore"
        Me.m_tbxYearFore.Size = New System.Drawing.Size(61, 20)
        Me.m_tbxYearFore.TabIndex = 2
        Me.m_tbxYearFore.Text = "1978"
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(12, 90)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(308, 18)
        Me.CEwEHeaderLabel1.TabIndex = 4
        Me.CEwEHeaderLabel1.Text = "Forecast reporting"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrOther
        '
        Me.m_hdrOther.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrOther.CanCollapseParent = False
        Me.m_hdrOther.CollapsedParentHeight = 0
        Me.m_hdrOther.IsCollapsed = False
        Me.m_hdrOther.Location = New System.Drawing.Point(12, 168)
        Me.m_hdrOther.Name = "m_hdrOther"
        Me.m_hdrOther.Size = New System.Drawing.Size(308, 18)
        Me.m_hdrOther.TabIndex = 4
        Me.m_hdrOther.Text = "Other"
        Me.m_hdrOther.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblNoData
        '
        Me.m_lblNoData.AutoSize = True
        Me.m_lblNoData.Location = New System.Drawing.Point(12, 192)
        Me.m_lblNoData.Name = "m_lblNoData"
        Me.m_lblNoData.Size = New System.Drawing.Size(48, 13)
        Me.m_lblNoData.TabIndex = 0
        Me.m_lblNoData.Text = "&No data:"
        '
        'm_tbxNoData
        '
        Me.m_tbxNoData.Location = New System.Drawing.Point(66, 189)
        Me.m_tbxNoData.Name = "m_tbxNoData"
        Me.m_tbxNoData.Size = New System.Drawing.Size(254, 20)
        Me.m_tbxNoData.TabIndex = 2
        Me.m_tbxNoData.Text = "1E-20!"
        '
        'dlgSpaceRun
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(332, 263)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrOther)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_hdrHist)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.m_tbxNoData)
        Me.Controls.Add(Me.m_tbxYearFore)
        Me.Controls.Add(Me.m_tbxYearHist)
        Me.Controls.Add(Me.m_tbxFileFore)
        Me.Controls.Add(Me.m_lblNoData)
        Me.Controls.Add(Me.m_lblYearFore)
        Me.Controls.Add(Me.m_tbxFileHist)
        Me.Controls.Add(Me.m_lblFileFore)
        Me.Controls.Add(Me.m_lblYearHist)
        Me.Controls.Add(Me.m_lblFileHist)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "dlgSpaceRun"
        Me.Text = "FishMIP Ecospace run"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As Windows.Forms.Button
    Private WithEvents m_tbxFileHist As Windows.Forms.TextBox
    Private WithEvents m_tbxYearHist As Windows.Forms.TextBox
    Private WithEvents m_hdrHist As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblYearHist As Windows.Forms.Label
    Private WithEvents m_lblFileHist As Windows.Forms.Label
    Private WithEvents m_lblFileFore As Windows.Forms.Label
    Private WithEvents m_lblYearFore As Windows.Forms.Label
    Private WithEvents m_tbxFileFore As Windows.Forms.TextBox
    Private WithEvents m_tbxYearFore As Windows.Forms.TextBox
    Private WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrOther As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblNoData As Windows.Forms.Label
    Private WithEvents m_tbxNoData As Windows.Forms.TextBox
End Class
