Imports ScientificInterfaceShared.Forms

Namespace Ecospace.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgDefineExternalSpatialData
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDefineExternalSpatialData))
            Me.m_btnAdd = New System.Windows.Forms.Button()
            Me.m_btnDelete = New System.Windows.Forms.Button()
            Me.m_gridDatasets = New ScientificInterface.Ecospace.Controls.gridDefineExternalSpatialData()
            Me.m_cmbNewDS = New System.Windows.Forms.ComboBox()
            Me.m_btnOK = New System.Windows.Forms.Button()
            Me.m_btnConfigure = New System.Windows.Forms.Button()
            Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbEnableIndexing = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            '
            'm_btnAdd
            '
            resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_btnDelete
            '
            resources.ApplyResources(Me.m_btnDelete, "m_btnDelete")
            Me.m_btnDelete.Name = "m_btnDelete"
            Me.m_btnDelete.UseVisualStyleBackColor = True
            '
            'm_gridDatasets
            '
            Me.m_gridDatasets.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridDatasets, "m_gridDatasets")
            Me.m_gridDatasets.AutoSizeMinHeight = 10
            Me.m_gridDatasets.AutoSizeMinWidth = 10
            Me.m_gridDatasets.AutoStretchColumnsToFitWidth = True
            Me.m_gridDatasets.AutoStretchRowsToFitHeight = False
            Me.m_gridDatasets.BackColor = System.Drawing.Color.White
            Me.m_gridDatasets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridDatasets.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridDatasets.CustomSort = False
            Me.m_gridDatasets.DataName = "grid content"
            Me.m_gridDatasets.FixedColumnWidths = False
            Me.m_gridDatasets.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridDatasets.GridToolTipActive = True
            Me.m_gridDatasets.IsLayoutSuspended = False
            Me.m_gridDatasets.Name = "m_gridDatasets"
            Me.m_gridDatasets.SelectedDataset = Nothing
            Me.m_gridDatasets.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                Or SourceGrid2.GridSpecialKeys.Delete) _
                Or SourceGrid2.GridSpecialKeys.Arrows) _
                Or SourceGrid2.GridSpecialKeys.Tab) _
                Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                Or SourceGrid2.GridSpecialKeys.Enter) _
                Or SourceGrid2.GridSpecialKeys.Escape) _
                Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridDatasets.UIContext = Nothing
            '
            'm_cmbNewDS
            '
            resources.ApplyResources(Me.m_cmbNewDS, "m_cmbNewDS")
            Me.m_cmbNewDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbNewDS.FormattingEnabled = True
            Me.m_cmbNewDS.Name = "m_cmbNewDS"
            '
            'm_btnOK
            '
            resources.ApplyResources(Me.m_btnOK, "m_btnOK")
            Me.m_btnOK.Name = "m_btnOK"
            Me.m_btnOK.UseVisualStyleBackColor = True
            '
            'm_btnConfigure
            '
            resources.ApplyResources(Me.m_btnConfigure, "m_btnConfigure")
            Me.m_btnConfigure.Name = "m_btnConfigure"
            Me.m_btnConfigure.UseVisualStyleBackColor = True
            '
            'm_hdr
            '
            resources.ApplyResources(Me.m_hdr, "m_hdr")
            Me.m_hdr.CanCollapseParent = False
            Me.m_hdr.CollapsedParentHeight = 0
            Me.m_hdr.IsCollapsed = False
            Me.m_hdr.Name = "m_hdr"
            '
            'm_cbEnableIndexing
            '
            resources.ApplyResources(Me.m_cbEnableIndexing, "m_cbEnableIndexing")
            Me.m_cbEnableIndexing.Name = "m_cbEnableIndexing"
            Me.m_cbEnableIndexing.UseVisualStyleBackColor = True
            '
            'dlgDefineExternalSpatialData
            '
            Me.AcceptButton = Me.m_btnOK
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ControlBox = False
            Me.Controls.Add(Me.m_cbEnableIndexing)
            Me.Controls.Add(Me.m_hdr)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_cmbNewDS)
            Me.Controls.Add(Me.m_gridDatasets)
            Me.Controls.Add(Me.m_btnConfigure)
            Me.Controls.Add(Me.m_btnDelete)
            Me.Controls.Add(Me.m_btnAdd)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgDefineExternalSpatialData"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_gridDatasets As gridDefineExternalSpatialData
        Private WithEvents m_cmbNewDS As System.Windows.Forms.ComboBox
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_btnConfigure As System.Windows.Forms.Button
        Private WithEvents m_hdr As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_cbEnableIndexing As System.Windows.Forms.CheckBox
    End Class

End Namespace
