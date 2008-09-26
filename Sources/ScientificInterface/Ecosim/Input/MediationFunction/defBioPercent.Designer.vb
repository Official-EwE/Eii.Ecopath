Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class defBioPercent
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(defBioPercent))
            Me.m_btnOK = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_lbAvailableGroupsFleets = New System.Windows.Forms.ListBox
            Me.m_btnAdd = New System.Windows.Forms.Button
            Me.m_btnRemove = New System.Windows.Forms.Button
            Me.epBP = New System.Windows.Forms.ErrorProvider(Me.components)
            Me.m_lblAvailable = New System.Windows.Forms.Label
            Me.m_lblAssigned = New System.Windows.Forms.Label
            Me.m_splitPanels = New System.Windows.Forms.SplitContainer
            Me.m_grid = New ScientificInterface.Ecosim.ucDefBioPercentGrid
            Me.m_bp = New ScientificInterface.Ecosim.ucBioPercent
            CType(Me.epBP, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_splitPanels.Panel1.SuspendLayout()
            Me.m_splitPanels.Panel2.SuspendLayout()
            Me.m_splitPanels.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnOK
            '
            resources.ApplyResources(Me.m_btnOK, "m_btnOK")
            Me.m_btnOK.Name = "m_btnOK"
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            '
            'm_lbAvailableGroupsFleets
            '
            resources.ApplyResources(Me.m_lbAvailableGroupsFleets, "m_lbAvailableGroupsFleets")
            Me.m_lbAvailableGroupsFleets.FormattingEnabled = True
            Me.m_lbAvailableGroupsFleets.Name = "m_lbAvailableGroupsFleets"
            '
            'm_btnAdd
            '
            Me.m_btnAdd.Image = Global.ScientificInterface.My.Resources.Resources.arrow_right
            resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_btnRemove
            '
            Me.m_btnRemove.Image = Global.ScientificInterface.My.Resources.Resources.arrow_left
            resources.ApplyResources(Me.m_btnRemove, "m_btnRemove")
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'epBP
            '
            Me.epBP.ContainerControl = Me
            '
            'm_lblAvailable
            '
            resources.ApplyResources(Me.m_lblAvailable, "m_lblAvailable")
            Me.m_lblAvailable.Name = "m_lblAvailable"
            '
            'm_lblAssigned
            '
            resources.ApplyResources(Me.m_lblAssigned, "m_lblAssigned")
            Me.m_lblAssigned.Name = "m_lblAssigned"
            '
            'm_splitPanels
            '
            resources.ApplyResources(Me.m_splitPanels, "m_splitPanels")
            Me.m_splitPanels.Name = "m_splitPanels"
            '
            'm_splitPanels.Panel1
            '
            Me.m_splitPanels.Panel1.Controls.Add(Me.m_grid)
            '
            'm_splitPanels.Panel2
            '
            Me.m_splitPanels.Panel2.Controls.Add(Me.m_bp)
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
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
            Me.m_grid.FixedColumnWidths = True
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
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
            'm_bp
            '
            resources.ApplyResources(Me.m_bp, "m_bp")
            Me.m_bp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_bp.Name = "m_bp"
            Me.m_bp.Shape = Nothing
            '
            'defBioPercent
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_splitPanels)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_lblAssigned)
            Me.Controls.Add(Me.m_lblAvailable)
            Me.Controls.Add(Me.m_lbAvailableGroupsFleets)
            Me.Controls.Add(Me.m_btnRemove)
            Me.Controls.Add(Me.m_btnAdd)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "defBioPercent"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            CType(Me.epBP, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_splitPanels.Panel1.ResumeLayout(False)
            Me.m_splitPanels.Panel2.ResumeLayout(False)
            Me.m_splitPanels.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_btnOK As System.Windows.Forms.Button
        Friend WithEvents m_btnCancel As System.Windows.Forms.Button
        Friend WithEvents m_lbAvailableGroupsFleets As System.Windows.Forms.ListBox
        Friend WithEvents m_btnAdd As System.Windows.Forms.Button
        Friend WithEvents m_btnRemove As System.Windows.Forms.Button
        Friend WithEvents m_bp As ucBioPercent
        Friend WithEvents epBP As System.Windows.Forms.ErrorProvider
        Friend WithEvents m_lblAvailable As System.Windows.Forms.Label
        Friend WithEvents m_grid As ucDefBioPercentGrid
        Friend WithEvents m_lblAssigned As System.Windows.Forms.Label
        Friend WithEvents m_splitPanels As System.Windows.Forms.SplitContainer

    End Class

End Namespace

