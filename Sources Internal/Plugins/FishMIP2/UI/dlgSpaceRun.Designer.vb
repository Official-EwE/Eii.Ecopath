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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSpaceRun))
        Me.m_lblFileHist = New System.Windows.Forms.Label()
        Me.m_tbxFileHist = New System.Windows.Forms.TextBox()
        Me.m_lblYearHist = New System.Windows.Forms.Label()
        Me.m_tbxYearHist = New System.Windows.Forms.TextBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_lblFileFore = New System.Windows.Forms.Label()
        Me.m_lblYearFore = New System.Windows.Forms.Label()
        Me.m_tbxFileFore = New System.Windows.Forms.TextBox()
        Me.m_tbxYearFore = New System.Windows.Forms.TextBox()
        Me.m_lblNoData = New System.Windows.Forms.Label()
        Me.m_tbxNoData = New System.Windows.Forms.TextBox()
        Me.m_tbxEnd = New System.Windows.Forms.TextBox()
        Me.m_tbxStart = New System.Windows.Forms.TextBox()
        Me.m_pbAlert = New System.Windows.Forms.PictureBox()
        Me.m_lblError = New System.Windows.Forms.Label()
        Me.m_hdrOther = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrHist = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        CType(Me.m_pbAlert, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblFileHist
        '
        resources.ApplyResources(Me.m_lblFileHist, "m_lblFileHist")
        Me.m_lblFileHist.Name = "m_lblFileHist"
        '
        'm_tbxFileHist
        '
        resources.ApplyResources(Me.m_tbxFileHist, "m_tbxFileHist")
        Me.m_tbxFileHist.Name = "m_tbxFileHist"
        '
        'm_lblYearHist
        '
        resources.ApplyResources(Me.m_lblYearHist, "m_lblYearHist")
        Me.m_lblYearHist.Name = "m_lblYearHist"
        '
        'm_tbxYearHist
        '
        resources.ApplyResources(Me.m_tbxYearHist, "m_tbxYearHist")
        Me.m_tbxYearHist.Name = "m_tbxYearHist"
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_lblFileFore
        '
        resources.ApplyResources(Me.m_lblFileFore, "m_lblFileFore")
        Me.m_lblFileFore.Name = "m_lblFileFore"
        '
        'm_lblYearFore
        '
        resources.ApplyResources(Me.m_lblYearFore, "m_lblYearFore")
        Me.m_lblYearFore.Name = "m_lblYearFore"
        '
        'm_tbxFileFore
        '
        resources.ApplyResources(Me.m_tbxFileFore, "m_tbxFileFore")
        Me.m_tbxFileFore.Name = "m_tbxFileFore"
        '
        'm_tbxYearFore
        '
        resources.ApplyResources(Me.m_tbxYearFore, "m_tbxYearFore")
        Me.m_tbxYearFore.Name = "m_tbxYearFore"
        '
        'm_lblNoData
        '
        resources.ApplyResources(Me.m_lblNoData, "m_lblNoData")
        Me.m_lblNoData.Name = "m_lblNoData"
        '
        'm_tbxNoData
        '
        resources.ApplyResources(Me.m_tbxNoData, "m_tbxNoData")
        Me.m_tbxNoData.Name = "m_tbxNoData"
        '
        'm_tbxEnd
        '
        resources.ApplyResources(Me.m_tbxEnd, "m_tbxEnd")
        Me.m_tbxEnd.Name = "m_tbxEnd"
        Me.m_tbxEnd.ReadOnly = True
        Me.m_tbxEnd.TabStop = False
        '
        'm_tbxStart
        '
        resources.ApplyResources(Me.m_tbxStart, "m_tbxStart")
        Me.m_tbxStart.Name = "m_tbxStart"
        Me.m_tbxStart.ReadOnly = True
        Me.m_tbxStart.TabStop = False
        '
        'm_pbAlert
        '
        resources.ApplyResources(Me.m_pbAlert, "m_pbAlert")
        Me.m_pbAlert.Name = "m_pbAlert"
        Me.m_pbAlert.TabStop = False
        '
        'm_lblError
        '
        resources.ApplyResources(Me.m_lblError, "m_lblError")
        Me.m_lblError.ForeColor = System.Drawing.Color.OrangeRed
        Me.m_lblError.Name = "m_lblError"
        '
        'm_hdrOther
        '
        resources.ApplyResources(Me.m_hdrOther, "m_hdrOther")
        Me.m_hdrOther.CanCollapseParent = False
        Me.m_hdrOther.CollapsedParentHeight = 0
        Me.m_hdrOther.IsCollapsed = False
        Me.m_hdrOther.Name = "m_hdrOther"
        '
        'CEwEHeaderLabel1
        '
        resources.ApplyResources(Me.CEwEHeaderLabel1, "CEwEHeaderLabel1")
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        '
        'm_hdrHist
        '
        resources.ApplyResources(Me.m_hdrHist, "m_hdrHist")
        Me.m_hdrHist.CanCollapseParent = False
        Me.m_hdrHist.CollapsedParentHeight = 0
        Me.m_hdrHist.IsCollapsed = False
        Me.m_hdrHist.Name = "m_hdrHist"
        '
        'dlgSpaceRun
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lblError)
        Me.Controls.Add(Me.m_pbAlert)
        Me.Controls.Add(Me.m_hdrOther)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_hdrHist)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_tbxNoData)
        Me.Controls.Add(Me.m_tbxEnd)
        Me.Controls.Add(Me.m_tbxYearFore)
        Me.Controls.Add(Me.m_tbxStart)
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
        Me.ShowInTaskbar = False
        CType(Me.m_pbAlert, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_btnOK As Windows.Forms.Button
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
    Private WithEvents m_tbxEnd As Windows.Forms.TextBox
    Private WithEvents m_tbxStart As Windows.Forms.TextBox
    Private WithEvents m_pbAlert As Windows.Forms.PictureBox
    Private WithEvents m_lblError As Windows.Forms.Label
End Class
