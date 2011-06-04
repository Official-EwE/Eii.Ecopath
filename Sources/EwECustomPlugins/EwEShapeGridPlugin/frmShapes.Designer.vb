Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

''' <summary>
''' Form to implement shape grids.
''' </summary>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShapes
    Inherits frmEwEGrid

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShapes))
        Me.m_plGrid = New System.Windows.Forms.Panel
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip
        Me.m_tsbnTimeSeries = New System.Windows.Forms.ToolStripButton
        Me.m_tsSep1 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbnLongTerm = New System.Windows.Forms.ToolStripButton
        Me.m_tsbnSeasonal = New System.Windows.Forms.ToolStripButton
        Me.m_tsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_plGrid
        '
        resources.ApplyResources(Me.m_plGrid, "m_plGrid")
        Me.m_plGrid.Name = "m_plGrid"
        '
        'm_tsMain
        '
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnTimeSeries, Me.m_tsSep1, Me.m_tsbnLongTerm, Me.m_tsbnSeasonal})
        resources.ApplyResources(Me.m_tsMain, "m_tsMain")
        Me.m_tsMain.Name = "m_tsMain"
        '
        'm_tsbnTimeSeries
        '
        Me.m_tsbnTimeSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnTimeSeries, "m_tsbnTimeSeries")
        Me.m_tsbnTimeSeries.Name = "m_tsbnTimeSeries"
        '
        'm_tsSep1
        '
        Me.m_tsSep1.Name = "m_tsSep1"
        resources.ApplyResources(Me.m_tsSep1, "m_tsSep1")
        '
        'm_tsbnLongTerm
        '
        resources.ApplyResources(Me.m_tsbnLongTerm, "m_tsbnLongTerm")
        Me.m_tsbnLongTerm.Name = "m_tsbnLongTerm"
        '
        'm_tsbnSeasonal
        '
        resources.ApplyResources(Me.m_tsbnSeasonal, "m_tsbnSeasonal")
        Me.m_tsbnSeasonal.Name = "m_tsbnSeasonal"
        '
        'frmShapes
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_plGrid)
        Me.Controls.Add(Me.m_tsMain)
        Me.Name = "frmShapes"
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tsMain As cEwEToolstrip
    Private WithEvents m_plGrid As System.Windows.Forms.Panel
    Private WithEvents m_tsbnTimeSeries As System.Windows.Forms.ToolStripButton
    Private m_tsSep1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnLongTerm As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnSeasonal As System.Windows.Forms.ToolStripButton
End Class
