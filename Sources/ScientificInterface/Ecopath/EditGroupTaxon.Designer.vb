<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditGroupTaxon
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditGroupTaxon))
        Me.m_btnAdd = New System.Windows.Forms.Button
        Me.m_btnRemove = New System.Windows.Forms.Button
        Me.m_btnUpdate = New System.Windows.Forms.Button
        Me.m_btnKeep = New System.Windows.Forms.Button
        Me.m_btnMoveDown = New System.Windows.Forms.Button
        Me.m_btnMoveUp = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.OK_Button = New System.Windows.Forms.Button
        Me.m_btnUpdateAll = New System.Windows.Forms.Button
        Me.m_lbCommon = New System.Windows.Forms.Label
        Me.m_tbCommon = New System.Windows.Forms.TextBox
        Me.m_cmbClass = New System.Windows.Forms.ComboBox
        Me.m_lbClass = New System.Windows.Forms.Label
        Me.m_lbOrder = New System.Windows.Forms.Label
        Me.m_cmbOrder = New System.Windows.Forms.ComboBox
        Me.m_lbFamily = New System.Windows.Forms.Label
        Me.m_lbGenus = New System.Windows.Forms.Label
        Me.m_lbSpecies = New System.Windows.Forms.Label
        Me.m_cmbFamily = New System.Windows.Forms.ComboBox
        Me.m_cmbGenus = New System.Windows.Forms.ComboBox
        Me.m_cmbSpecies = New System.Windows.Forms.ComboBox
        Me.m_lbExternalSrc = New System.Windows.Forms.Label
        Me.m_cmbSource = New System.Windows.Forms.ComboBox
        Me.m_btnConfigure = New System.Windows.Forms.Button
        Me.m_btnSearch = New System.Windows.Forms.Button
        Me.m_hdrOrder = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_hdrEdit = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_hdrExternal = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_hdrTaxon = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_grid = New ScientificInterface.gridEditGroupTaxon
        Me.m_cbIncludeExtent = New System.Windows.Forms.CheckBox
        Me.m_hdrSearch = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.SuspendLayout()
        '
        'm_btnAdd
        '
        resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
        Me.m_btnAdd.Name = "m_btnAdd"
        Me.m_btnAdd.UseVisualStyleBackColor = True
        '
        'm_btnRemove
        '
        resources.ApplyResources(Me.m_btnRemove, "m_btnRemove")
        Me.m_btnRemove.Name = "m_btnRemove"
        Me.m_btnRemove.UseVisualStyleBackColor = True
        '
        'm_btnUpdate
        '
        resources.ApplyResources(Me.m_btnUpdate, "m_btnUpdate")
        Me.m_btnUpdate.Name = "m_btnUpdate"
        Me.m_btnUpdate.UseVisualStyleBackColor = True
        '
        'm_btnKeep
        '
        resources.ApplyResources(Me.m_btnKeep, "m_btnKeep")
        Me.m_btnKeep.Name = "m_btnKeep"
        Me.m_btnKeep.UseVisualStyleBackColor = True
        '
        'm_btnMoveDown
        '
        resources.ApplyResources(Me.m_btnMoveDown, "m_btnMoveDown")
        Me.m_btnMoveDown.Name = "m_btnMoveDown"
        Me.m_btnMoveDown.UseVisualStyleBackColor = True
        '
        'm_btnMoveUp
        '
        resources.ApplyResources(Me.m_btnMoveUp, "m_btnMoveUp")
        Me.m_btnMoveUp.Name = "m_btnMoveUp"
        Me.m_btnMoveUp.UseVisualStyleBackColor = True
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.Name = "OK_Button"
        '
        'm_btnUpdateAll
        '
        resources.ApplyResources(Me.m_btnUpdateAll, "m_btnUpdateAll")
        Me.m_btnUpdateAll.Name = "m_btnUpdateAll"
        Me.m_btnUpdateAll.UseVisualStyleBackColor = True
        '
        'm_lbCommon
        '
        resources.ApplyResources(Me.m_lbCommon, "m_lbCommon")
        Me.m_lbCommon.Name = "m_lbCommon"
        '
        'm_tbCommon
        '
        resources.ApplyResources(Me.m_tbCommon, "m_tbCommon")
        Me.m_tbCommon.Name = "m_tbCommon"
        '
        'm_cmbClass
        '
        resources.ApplyResources(Me.m_cmbClass, "m_cmbClass")
        Me.m_cmbClass.FormattingEnabled = True
        Me.m_cmbClass.Name = "m_cmbClass"
        Me.m_cmbClass.Sorted = True
        '
        'm_lbClass
        '
        resources.ApplyResources(Me.m_lbClass, "m_lbClass")
        Me.m_lbClass.Name = "m_lbClass"
        '
        'm_lbOrder
        '
        resources.ApplyResources(Me.m_lbOrder, "m_lbOrder")
        Me.m_lbOrder.Name = "m_lbOrder"
        '
        'm_cmbOrder
        '
        resources.ApplyResources(Me.m_cmbOrder, "m_cmbOrder")
        Me.m_cmbOrder.FormattingEnabled = True
        Me.m_cmbOrder.Name = "m_cmbOrder"
        Me.m_cmbOrder.Sorted = True
        '
        'm_lbFamily
        '
        resources.ApplyResources(Me.m_lbFamily, "m_lbFamily")
        Me.m_lbFamily.Name = "m_lbFamily"
        '
        'm_lbGenus
        '
        resources.ApplyResources(Me.m_lbGenus, "m_lbGenus")
        Me.m_lbGenus.Name = "m_lbGenus"
        '
        'm_lbSpecies
        '
        resources.ApplyResources(Me.m_lbSpecies, "m_lbSpecies")
        Me.m_lbSpecies.Name = "m_lbSpecies"
        '
        'm_cmbFamily
        '
        resources.ApplyResources(Me.m_cmbFamily, "m_cmbFamily")
        Me.m_cmbFamily.FormattingEnabled = True
        Me.m_cmbFamily.Name = "m_cmbFamily"
        Me.m_cmbFamily.Sorted = True
        '
        'm_cmbGenus
        '
        resources.ApplyResources(Me.m_cmbGenus, "m_cmbGenus")
        Me.m_cmbGenus.FormattingEnabled = True
        Me.m_cmbGenus.Name = "m_cmbGenus"
        Me.m_cmbGenus.Sorted = True
        '
        'm_cmbSpecies
        '
        resources.ApplyResources(Me.m_cmbSpecies, "m_cmbSpecies")
        Me.m_cmbSpecies.FormattingEnabled = True
        Me.m_cmbSpecies.Name = "m_cmbSpecies"
        Me.m_cmbSpecies.Sorted = True
        '
        'm_lbExternalSrc
        '
        resources.ApplyResources(Me.m_lbExternalSrc, "m_lbExternalSrc")
        Me.m_lbExternalSrc.Name = "m_lbExternalSrc"
        '
        'm_cmbSource
        '
        resources.ApplyResources(Me.m_cmbSource, "m_cmbSource")
        Me.m_cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbSource.FormattingEnabled = True
        Me.m_cmbSource.Items.AddRange(New Object() {resources.GetString("m_cmbSource.Items"), resources.GetString("m_cmbSource.Items1"), resources.GetString("m_cmbSource.Items2")})
        Me.m_cmbSource.Name = "m_cmbSource"
        '
        'm_btnConfigure
        '
        resources.ApplyResources(Me.m_btnConfigure, "m_btnConfigure")
        Me.m_btnConfigure.Name = "m_btnConfigure"
        Me.m_btnConfigure.UseVisualStyleBackColor = True
        '
        'm_btnSearch
        '
        resources.ApplyResources(Me.m_btnSearch, "m_btnSearch")
        Me.m_btnSearch.Name = "m_btnSearch"
        Me.m_btnSearch.UseVisualStyleBackColor = True
        '
        'm_hdrOrder
        '
        resources.ApplyResources(Me.m_hdrOrder, "m_hdrOrder")
        Me.m_hdrOrder.Name = "m_hdrOrder"
        '
        'm_hdrEdit
        '
        resources.ApplyResources(Me.m_hdrEdit, "m_hdrEdit")
        Me.m_hdrEdit.Name = "m_hdrEdit"
        '
        'm_hdrExternal
        '
        resources.ApplyResources(Me.m_hdrExternal, "m_hdrExternal")
        Me.m_hdrExternal.Name = "m_hdrExternal"
        '
        'm_hdrTaxon
        '
        resources.ApplyResources(Me.m_hdrTaxon, "m_hdrTaxon")
        Me.m_hdrTaxon.Name = "m_hdrTaxon"
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = False
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
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.Name = "m_grid"
        Me.m_grid.SelectedTaxon = Nothing
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
        'm_cbIncludeExtent
        '
        resources.ApplyResources(Me.m_cbIncludeExtent, "m_cbIncludeExtent")
        Me.m_cbIncludeExtent.Name = "m_cbIncludeExtent"
        Me.m_cbIncludeExtent.UseVisualStyleBackColor = True
        '
        'm_hdrSearch
        '
        resources.ApplyResources(Me.m_hdrSearch, "m_hdrSearch")
        Me.m_hdrSearch.Name = "m_hdrSearch"
        '
        'EditGroupTaxon
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_cbIncludeExtent)
        Me.Controls.Add(Me.m_cmbSource)
        Me.Controls.Add(Me.m_cmbSpecies)
        Me.Controls.Add(Me.m_cmbGenus)
        Me.Controls.Add(Me.m_cmbFamily)
        Me.Controls.Add(Me.m_cmbOrder)
        Me.Controls.Add(Me.m_cmbClass)
        Me.Controls.Add(Me.m_lbExternalSrc)
        Me.Controls.Add(Me.m_lbSpecies)
        Me.Controls.Add(Me.m_lbGenus)
        Me.Controls.Add(Me.m_lbFamily)
        Me.Controls.Add(Me.m_lbOrder)
        Me.Controls.Add(Me.m_tbCommon)
        Me.Controls.Add(Me.m_lbClass)
        Me.Controls.Add(Me.m_lbCommon)
        Me.Controls.Add(Me.m_hdrSearch)
        Me.Controls.Add(Me.m_hdrTaxon)
        Me.Controls.Add(Me.m_hdrExternal)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.m_hdrEdit)
        Me.Controls.Add(Me.m_hdrOrder)
        Me.Controls.Add(Me.m_btnKeep)
        Me.Controls.Add(Me.m_btnMoveDown)
        Me.Controls.Add(Me.m_btnMoveUp)
        Me.Controls.Add(Me.m_grid)
        Me.Controls.Add(Me.m_btnConfigure)
        Me.Controls.Add(Me.m_btnSearch)
        Me.Controls.Add(Me.m_btnUpdateAll)
        Me.Controls.Add(Me.m_btnUpdate)
        Me.Controls.Add(Me.m_btnRemove)
        Me.Controls.Add(Me.m_btnAdd)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EditGroupTaxon"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_btnRemove As System.Windows.Forms.Button
    Private WithEvents m_btnUpdate As System.Windows.Forms.Button
    Private WithEvents m_grid As ScientificInterface.gridEditGroupTaxon
    Private WithEvents m_hdrOrder As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnKeep As System.Windows.Forms.Button
    Private WithEvents m_btnMoveDown As System.Windows.Forms.Button
    Private WithEvents m_btnMoveUp As System.Windows.Forms.Button
    Private WithEvents m_hdrEdit As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents Cancel_Button As System.Windows.Forms.Button
    Private WithEvents OK_Button As System.Windows.Forms.Button
    Private WithEvents m_hdrExternal As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnUpdateAll As System.Windows.Forms.Button
    Private WithEvents m_lbCommon As System.Windows.Forms.Label
    Private WithEvents m_tbCommon As System.Windows.Forms.TextBox
    Private WithEvents m_lbClass As System.Windows.Forms.Label
    Private WithEvents m_cmbClass As System.Windows.Forms.ComboBox
    Private WithEvents m_lbOrder As System.Windows.Forms.Label
    Private WithEvents m_cmbOrder As System.Windows.Forms.ComboBox
    Private WithEvents m_lbFamily As System.Windows.Forms.Label
    Private WithEvents m_lbGenus As System.Windows.Forms.Label
    Private WithEvents m_lbSpecies As System.Windows.Forms.Label
    Private WithEvents m_cmbFamily As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbGenus As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbSpecies As System.Windows.Forms.ComboBox
    Private WithEvents m_lbExternalSrc As System.Windows.Forms.Label
    Private WithEvents m_cmbSource As System.Windows.Forms.ComboBox
    Private WithEvents m_btnConfigure As System.Windows.Forms.Button
    Private WithEvents m_btnSearch As System.Windows.Forms.Button
    Private WithEvents m_btnAdd As System.Windows.Forms.Button
    Private WithEvents m_hdrTaxon As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbIncludeExtent As System.Windows.Forms.CheckBox
    Private WithEvents m_hdrSearch As ScientificInterfaceShared.Controls.cEwEHeaderLabel

End Class
