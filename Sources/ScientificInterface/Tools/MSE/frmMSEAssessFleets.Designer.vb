<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEAssessFleets
    Inherits frmEwE

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEAssessFleets))
        Me.m_split = New System.Windows.Forms.SplitContainer
        Me.m_blocks = New ScientificInterface.Ecosim.ucPolicyColorBlocks
        Me.GridFishingCV1 = New ScientificInterface.gridFishingCV
        Me.m_split.Panel1.SuspendLayout()
        Me.m_split.Panel2.SuspendLayout()
        Me.m_split.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_split
        '
        resources.ApplyResources(Me.m_split, "m_split")
        Me.m_split.Name = "m_split"
        '
        'm_split.Panel1
        '
        Me.m_split.Panel1.Controls.Add(Me.m_blocks)
        '
        'm_split.Panel2
        '
        Me.m_split.Panel2.Controls.Add(Me.GridFishingCV1)
        '
        'm_blocks
        '
        Me.m_blocks.ControlPanelVisible = False
        Me.m_blocks.CurColor = System.Drawing.Color.Empty
        resources.ApplyResources(Me.m_blocks, "m_blocks")
        Me.m_blocks.Name = "m_blocks"
        Me.m_blocks.ParmBlockCodes = Nothing
        Me.m_blocks.UIContext = Nothing
        '
        'GridFishingCV1
        '
        Me.GridFishingCV1.AutoSizeMinHeight = 10
        Me.GridFishingCV1.AutoSizeMinWidth = 10
        Me.GridFishingCV1.AutoStretchColumnsToFitWidth = False
        Me.GridFishingCV1.AutoStretchRowsToFitHeight = False
        Me.GridFishingCV1.BackColor = System.Drawing.Color.White
        Me.GridFishingCV1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.GridFishingCV1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.GridFishingCV1.CustomSort = False
        resources.ApplyResources(Me.GridFishingCV1, "GridFishingCV1")
        Me.GridFishingCV1.FixedColumnWidths = False
        Me.GridFishingCV1.FocusStyle = SourceGrid2.FocusStyle.None
        Me.GridFishingCV1.GridToolTipActive = True
        Me.GridFishingCV1.Name = "GridFishingCV1"
        Me.GridFishingCV1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.GridFishingCV1.TrackPropertySelection = True
        Me.GridFishingCV1.UIContext = Nothing
        '
        'frmMSEAssessFleets
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_split)
        Me.Name = "frmMSEAssessFleets"
        Me.m_split.Panel1.ResumeLayout(False)
        Me.m_split.Panel2.ResumeLayout(False)
        Me.m_split.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_blocks As ScientificInterface.Ecosim.ucPolicyColorBlocks
    Private WithEvents GridFishingCV1 As ScientificInterface.gridFishingCV
    Private WithEvents m_split As System.Windows.Forms.SplitContainer
End Class
