Namespace Ecospace

    Partial Class frmEcospaceHabitatFishery
        Inherits frmEwEGrid

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceHabitatFishery))
            Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnQuickHelp = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnDefineHabitats = New System.Windows.Forms.ToolStripButton()
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_lblInfo = New System.Windows.Forms.Label()
            Me.m_grid = New ScientificInterface.Ecospace.gridEcospaceMPAEnforcement()
            Me.m_tsMain.SuspendLayout()
            Me.m_tlpContent.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnQuickHelp, Me.ToolStripSeparator1, Me.m_tsbnDefineHabitats})
            Me.m_tsMain.Name = "m_tsMain"
            Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnQuickHelp
            '
            Me.m_tsbnQuickHelp.AutoToolTip = False
            Me.m_tsbnQuickHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnQuickHelp, "m_tsbnQuickHelp")
            Me.m_tsbnQuickHelp.Name = "m_tsbnQuickHelp"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsbnDefineHabitats
            '
            resources.ApplyResources(Me.m_tsbnDefineHabitats, "m_tsbnDefineHabitats")
            Me.m_tsbnDefineHabitats.Name = "m_tsbnDefineHabitats"
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_tsMain, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_grid, 0, 2)
            Me.m_tlpContent.Controls.Add(Me.m_lblInfo, 0, 1)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_lblInfo
            '
            resources.ApplyResources(Me.m_lblInfo, "m_lblInfo")
            Me.m_lblInfo.Name = "m_lblInfo"
            '
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = True
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
            Me.m_grid.DataName = "grid content"
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.IsLayoutSuspended = False
            Me.m_grid.IsOutputGrid = False
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
            Me.m_grid.UIContext = Nothing
            '
            'frmEcospaceHabitatFishery
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tlpContent)
            Me.Name = "frmEcospaceHabitatFishery"
            Me.ShowInTaskbar = False
            Me.TabText = ""
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_tlpContent.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents m_tsbnDefineHabitats As ToolStripButton
        Private WithEvents m_tlpContent As TableLayoutPanel
        Private WithEvents m_tsMain As cEwEToolstrip
        Private WithEvents m_lblInfo As Label
        Private WithEvents m_grid As gridEcospaceMPAEnforcement
        Private WithEvents m_tsbnQuickHelp As ToolStripButton
        Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    End Class

End Namespace
