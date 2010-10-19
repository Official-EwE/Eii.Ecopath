Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ApplyEP
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ApplyEP))
            Me.m_grid = New ScientificInterface.Ecosim.ApplyEPEwEGrid
            Me.m_splitContent = New System.Windows.Forms.SplitContainer
            Me.m_lvShapes = New cSmoothListView
            Me.m_lblNoStanza = New System.Windows.Forms.Label
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel
            Me.m_tsSet = New System.Windows.Forms.ToolStrip
            Me.m_tsbSet = New System.Windows.Forms.ToolStripButton
            Me.m_tscEggProdShapes = New System.Windows.Forms.ToolStripComboBox
            Me.m_tlbSet = New System.Windows.Forms.ToolStripLabel
            Me.m_splitContent.Panel1.SuspendLayout()
            Me.m_splitContent.Panel2.SuspendLayout()
            Me.m_splitContent.SuspendLayout()
            Me.m_tlpContent.SuspendLayout()
            Me.m_tsSet.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_grid
            '
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.RowResize) _
                        Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.ClearSelection) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.FixedColumnWidths = True
            Me.m_grid.Name = "m_grid"
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            '
            'm_splitContent
            '
            resources.ApplyResources(Me.m_splitContent, "m_splitContent")
            Me.m_splitContent.Name = "m_splitContent"
            '
            'm_splitContent.Panel1
            '
            Me.m_splitContent.Panel1.Controls.Add(Me.m_lvShapes)
            '
            'm_splitContent.Panel2
            '
            Me.m_splitContent.Panel2.Controls.Add(Me.m_grid)
            '
            'm_lvShapes
            '
            Me.m_lvShapes.BackColor = System.Drawing.SystemColors.Window
            resources.ApplyResources(Me.m_lvShapes, "m_lvShapes")
            Me.m_lvShapes.LabelEdit = True
            Me.m_lvShapes.MultiSelect = False
            Me.m_lvShapes.Name = "m_lvShapes"
            Me.m_lvShapes.UseCompatibleStateImageBehavior = False
            '
            'm_lblNoStanza
            '
            resources.ApplyResources(Me.m_lblNoStanza, "m_lblNoStanza")
            Me.m_lblNoStanza.Name = "m_lblNoStanza"
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_splitContent, 0, 1)
            Me.m_tlpContent.Controls.Add(Me.m_tsSet, 0, 0)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_tsSet
            '
            resources.ApplyResources(Me.m_tsSet, "m_tsSet")
            Me.m_tsSet.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbSet, Me.m_tscEggProdShapes, Me.m_tlbSet})
            Me.m_tsSet.Name = "m_tsSet"
            Me.m_tsSet.Stretch = True
            '
            'm_tsbSet
            '
            Me.m_tsbSet.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tsbSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbSet.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
            resources.ApplyResources(Me.m_tsbSet, "m_tsbSet")
            Me.m_tsbSet.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tsbSet.Name = "m_tsbSet"
            '
            'm_tscEggProdShapes
            '
            Me.m_tscEggProdShapes.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tscEggProdShapes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscEggProdShapes.Name = "m_tscEggProdShapes"
            resources.ApplyResources(Me.m_tscEggProdShapes, "m_tscEggProdShapes")
            '
            'm_tlbSet
            '
            Me.m_tlbSet.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tlbSet.Name = "m_tlbSet"
            resources.ApplyResources(Me.m_tlbSet, "m_tlbSet")
            '
            'ApplyEP
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpContent)
            Me.Controls.Add(Me.m_lblNoStanza)
            Me.Name = "ApplyEP"
            Me.m_splitContent.Panel1.ResumeLayout(False)
            Me.m_splitContent.Panel2.ResumeLayout(False)
            Me.m_splitContent.ResumeLayout(False)
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_tsSet.ResumeLayout(False)
            Me.m_tsSet.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_grid As ApplyEPEwEGrid
        Private WithEvents m_lvShapes As cSmoothListView
        Private WithEvents m_splitContent As System.Windows.Forms.SplitContainer
        Private WithEvents m_lblNoStanza As System.Windows.Forms.Label
        Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tsSet As System.Windows.Forms.ToolStrip
        Private WithEvents m_tscEggProdShapes As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tlbSet As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsbSet As System.Windows.Forms.ToolStripButton

    End Class
End Namespace

