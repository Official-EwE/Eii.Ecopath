<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgEditGroupTaxon
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditGroupTaxon))
        Me.m_btnAdd = New System.Windows.Forms.Button
        Me.m_btnRemove = New System.Windows.Forms.Button
        Me.m_btnUpdate = New System.Windows.Forms.Button
        Me.m_btnKeep = New System.Windows.Forms.Button
        Me.m_btnMoveDown = New System.Windows.Forms.Button
        Me.m_btnMoveUp = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.OK_Button = New System.Windows.Forms.Button
        Me.m_btnUpdateAll = New System.Windows.Forms.Button
        Me.m_lbTerm = New System.Windows.Forms.Label
        Me.m_tbSearch = New System.Windows.Forms.TextBox
        Me.m_cmbEngine = New System.Windows.Forms.ComboBox
        Me.m_btnConfigure = New System.Windows.Forms.Button
        Me.m_cbIncludeExtent = New System.Windows.Forms.CheckBox
        Me.m_scMain = New System.Windows.Forms.SplitContainer
        Me.m_tcMain = New System.Windows.Forms.TabControl
        Me.m_tpSearch = New System.Windows.Forms.TabPage
        Me.m_lblEngine = New System.Windows.Forms.Label
        Me.m_tpDetails = New System.Windows.Forms.TabPage
        Me.m_lblSelectedGroup = New System.Windows.Forms.Label
        Me.m_lblGroup = New System.Windows.Forms.Label
        Me.m_btnPhylum = New System.Windows.Forms.Button
        Me.m_btnSearchClass = New System.Windows.Forms.Button
        Me.m_btnSearchOrder = New System.Windows.Forms.Button
        Me.m_btnSearchFamily = New System.Windows.Forms.Button
        Me.m_btnSearchGenus = New System.Windows.Forms.Button
        Me.m_btnSearchSpecies = New System.Windows.Forms.Button
        Me.m_btnSearchCommon = New System.Windows.Forms.Button
        Me.m_cmbSpecies = New System.Windows.Forms.ComboBox
        Me.m_cmbGenus = New System.Windows.Forms.ComboBox
        Me.m_cmbFamily = New System.Windows.Forms.ComboBox
        Me.m_cmbOrder = New System.Windows.Forms.ComboBox
        Me.m_cmbPhylum = New System.Windows.Forms.ComboBox
        Me.m_cmbClass = New System.Windows.Forms.ComboBox
        Me.m_lbSpecies = New System.Windows.Forms.Label
        Me.m_lbGenus = New System.Windows.Forms.Label
        Me.m_lbFamily = New System.Windows.Forms.Label
        Me.m_lbOrder = New System.Windows.Forms.Label
        Me.m_lblPhylum = New System.Windows.Forms.Label
        Me.m_tbCommon = New System.Windows.Forms.TextBox
        Me.m_lbClass = New System.Windows.Forms.Label
        Me.m_lbCommon = New System.Windows.Forms.Label
        Me.m_gridGroups = New ScientificInterface.gridEditGroupTaxon
        Me.m_gridResults = New ScientificInterface.gridTaxonSearchResults
        Me.m_hdrOrder = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_hdrEdit = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_hdrExternal = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tcMain.SuspendLayout()
        Me.m_tpSearch.SuspendLayout()
        Me.m_tpDetails.SuspendLayout()
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
        'm_lbTerm
        '
        resources.ApplyResources(Me.m_lbTerm, "m_lbTerm")
        Me.m_lbTerm.Name = "m_lbTerm"
        '
        'm_tbSearch
        '
        resources.ApplyResources(Me.m_tbSearch, "m_tbSearch")
        Me.m_tbSearch.Name = "m_tbSearch"
        '
        'm_cmbEngine
        '
        resources.ApplyResources(Me.m_cmbEngine, "m_cmbEngine")
        Me.m_cmbEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbEngine.FormattingEnabled = True
        Me.m_cmbEngine.Items.AddRange(New Object() {resources.GetString("m_cmbEngine.Items"), resources.GetString("m_cmbEngine.Items1"), resources.GetString("m_cmbEngine.Items2")})
        Me.m_cmbEngine.Name = "m_cmbEngine"
        '
        'm_btnConfigure
        '
        resources.ApplyResources(Me.m_btnConfigure, "m_btnConfigure")
        Me.m_btnConfigure.Name = "m_btnConfigure"
        Me.m_btnConfigure.UseVisualStyleBackColor = True
        '
        'm_cbIncludeExtent
        '
        resources.ApplyResources(Me.m_cbIncludeExtent, "m_cbIncludeExtent")
        Me.m_cbIncludeExtent.Name = "m_cbIncludeExtent"
        Me.m_cbIncludeExtent.UseVisualStyleBackColor = True
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_gridGroups)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_tcMain)
        '
        'm_tcMain
        '
        Me.m_tcMain.Controls.Add(Me.m_tpSearch)
        Me.m_tcMain.Controls.Add(Me.m_tpDetails)
        resources.ApplyResources(Me.m_tcMain, "m_tcMain")
        Me.m_tcMain.Name = "m_tcMain"
        Me.m_tcMain.SelectedIndex = 0
        '
        'm_tpSearch
        '
        Me.m_tpSearch.Controls.Add(Me.m_gridResults)
        Me.m_tpSearch.Controls.Add(Me.m_cbIncludeExtent)
        Me.m_tpSearch.Controls.Add(Me.m_cmbEngine)
        Me.m_tpSearch.Controls.Add(Me.m_tbSearch)
        Me.m_tpSearch.Controls.Add(Me.m_lblEngine)
        Me.m_tpSearch.Controls.Add(Me.m_lbTerm)
        Me.m_tpSearch.Controls.Add(Me.m_btnConfigure)
        resources.ApplyResources(Me.m_tpSearch, "m_tpSearch")
        Me.m_tpSearch.Name = "m_tpSearch"
        '
        'm_lblEngine
        '
        resources.ApplyResources(Me.m_lblEngine, "m_lblEngine")
        Me.m_lblEngine.Name = "m_lblEngine"
        '
        'm_tpDetails
        '
        Me.m_tpDetails.Controls.Add(Me.m_lblSelectedGroup)
        Me.m_tpDetails.Controls.Add(Me.m_lblGroup)
        Me.m_tpDetails.Controls.Add(Me.m_btnPhylum)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchClass)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchOrder)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchFamily)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchGenus)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchSpecies)
        Me.m_tpDetails.Controls.Add(Me.m_btnSearchCommon)
        Me.m_tpDetails.Controls.Add(Me.m_cmbSpecies)
        Me.m_tpDetails.Controls.Add(Me.m_cmbGenus)
        Me.m_tpDetails.Controls.Add(Me.m_cmbFamily)
        Me.m_tpDetails.Controls.Add(Me.m_cmbOrder)
        Me.m_tpDetails.Controls.Add(Me.m_cmbPhylum)
        Me.m_tpDetails.Controls.Add(Me.m_cmbClass)
        Me.m_tpDetails.Controls.Add(Me.m_lbSpecies)
        Me.m_tpDetails.Controls.Add(Me.m_lbGenus)
        Me.m_tpDetails.Controls.Add(Me.m_lbFamily)
        Me.m_tpDetails.Controls.Add(Me.m_lbOrder)
        Me.m_tpDetails.Controls.Add(Me.m_lblPhylum)
        Me.m_tpDetails.Controls.Add(Me.m_tbCommon)
        Me.m_tpDetails.Controls.Add(Me.m_lbClass)
        Me.m_tpDetails.Controls.Add(Me.m_lbCommon)
        resources.ApplyResources(Me.m_tpDetails, "m_tpDetails")
        Me.m_tpDetails.Name = "m_tpDetails"
        Me.m_tpDetails.UseVisualStyleBackColor = True
        '
        'm_lblSelectedGroup
        '
        resources.ApplyResources(Me.m_lblSelectedGroup, "m_lblSelectedGroup")
        Me.m_lblSelectedGroup.Name = "m_lblSelectedGroup"
        '
        'm_lblGroup
        '
        resources.ApplyResources(Me.m_lblGroup, "m_lblGroup")
        Me.m_lblGroup.Name = "m_lblGroup"
        '
        'm_btnPhylum
        '
        resources.ApplyResources(Me.m_btnPhylum, "m_btnPhylum")
        Me.m_btnPhylum.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnPhylum.Name = "m_btnPhylum"
        Me.m_btnPhylum.UseVisualStyleBackColor = True
        '
        'm_btnSearchClass
        '
        resources.ApplyResources(Me.m_btnSearchClass, "m_btnSearchClass")
        Me.m_btnSearchClass.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchClass.Name = "m_btnSearchClass"
        Me.m_btnSearchClass.UseVisualStyleBackColor = True
        '
        'm_btnSearchOrder
        '
        resources.ApplyResources(Me.m_btnSearchOrder, "m_btnSearchOrder")
        Me.m_btnSearchOrder.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchOrder.Name = "m_btnSearchOrder"
        Me.m_btnSearchOrder.UseVisualStyleBackColor = True
        '
        'm_btnSearchFamily
        '
        resources.ApplyResources(Me.m_btnSearchFamily, "m_btnSearchFamily")
        Me.m_btnSearchFamily.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchFamily.Name = "m_btnSearchFamily"
        Me.m_btnSearchFamily.UseVisualStyleBackColor = True
        '
        'm_btnSearchGenus
        '
        resources.ApplyResources(Me.m_btnSearchGenus, "m_btnSearchGenus")
        Me.m_btnSearchGenus.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchGenus.Name = "m_btnSearchGenus"
        Me.m_btnSearchGenus.UseVisualStyleBackColor = True
        '
        'm_btnSearchSpecies
        '
        resources.ApplyResources(Me.m_btnSearchSpecies, "m_btnSearchSpecies")
        Me.m_btnSearchSpecies.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchSpecies.Name = "m_btnSearchSpecies"
        Me.m_btnSearchSpecies.UseVisualStyleBackColor = True
        '
        'm_btnSearchCommon
        '
        resources.ApplyResources(Me.m_btnSearchCommon, "m_btnSearchCommon")
        Me.m_btnSearchCommon.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
        Me.m_btnSearchCommon.Name = "m_btnSearchCommon"
        Me.m_btnSearchCommon.UseVisualStyleBackColor = True
        '
        'm_cmbSpecies
        '
        resources.ApplyResources(Me.m_cmbSpecies, "m_cmbSpecies")
        Me.m_cmbSpecies.FormattingEnabled = True
        Me.m_cmbSpecies.Name = "m_cmbSpecies"
        Me.m_cmbSpecies.Sorted = True
        '
        'm_cmbGenus
        '
        resources.ApplyResources(Me.m_cmbGenus, "m_cmbGenus")
        Me.m_cmbGenus.FormattingEnabled = True
        Me.m_cmbGenus.Name = "m_cmbGenus"
        Me.m_cmbGenus.Sorted = True
        '
        'm_cmbFamily
        '
        resources.ApplyResources(Me.m_cmbFamily, "m_cmbFamily")
        Me.m_cmbFamily.FormattingEnabled = True
        Me.m_cmbFamily.Name = "m_cmbFamily"
        Me.m_cmbFamily.Sorted = True
        '
        'm_cmbOrder
        '
        resources.ApplyResources(Me.m_cmbOrder, "m_cmbOrder")
        Me.m_cmbOrder.FormattingEnabled = True
        Me.m_cmbOrder.Name = "m_cmbOrder"
        Me.m_cmbOrder.Sorted = True
        '
        'm_cmbPhylum
        '
        resources.ApplyResources(Me.m_cmbPhylum, "m_cmbPhylum")
        Me.m_cmbPhylum.FormattingEnabled = True
        Me.m_cmbPhylum.Name = "m_cmbPhylum"
        Me.m_cmbPhylum.Sorted = True
        '
        'm_cmbClass
        '
        resources.ApplyResources(Me.m_cmbClass, "m_cmbClass")
        Me.m_cmbClass.FormattingEnabled = True
        Me.m_cmbClass.Name = "m_cmbClass"
        Me.m_cmbClass.Sorted = True
        '
        'm_lbSpecies
        '
        resources.ApplyResources(Me.m_lbSpecies, "m_lbSpecies")
        Me.m_lbSpecies.Name = "m_lbSpecies"
        '
        'm_lbGenus
        '
        resources.ApplyResources(Me.m_lbGenus, "m_lbGenus")
        Me.m_lbGenus.Name = "m_lbGenus"
        '
        'm_lbFamily
        '
        resources.ApplyResources(Me.m_lbFamily, "m_lbFamily")
        Me.m_lbFamily.Name = "m_lbFamily"
        '
        'm_lbOrder
        '
        resources.ApplyResources(Me.m_lbOrder, "m_lbOrder")
        Me.m_lbOrder.Name = "m_lbOrder"
        '
        'm_lblPhylum
        '
        resources.ApplyResources(Me.m_lblPhylum, "m_lblPhylum")
        Me.m_lblPhylum.Name = "m_lblPhylum"
        '
        'm_tbCommon
        '
        resources.ApplyResources(Me.m_tbCommon, "m_tbCommon")
        Me.m_tbCommon.Name = "m_tbCommon"
        '
        'm_lbClass
        '
        resources.ApplyResources(Me.m_lbClass, "m_lbClass")
        Me.m_lbClass.Name = "m_lbClass"
        '
        'm_lbCommon
        '
        resources.ApplyResources(Me.m_lbCommon, "m_lbCommon")
        Me.m_lbCommon.Name = "m_lbCommon"
        '
        'm_gridGroups
        '
        Me.m_gridGroups.AllowBlockSelect = False
        Me.m_gridGroups.AutoSizeMinHeight = 10
        Me.m_gridGroups.AutoSizeMinWidth = 10
        Me.m_gridGroups.AutoStretchColumnsToFitWidth = False
        Me.m_gridGroups.AutoStretchRowsToFitHeight = False
        Me.m_gridGroups.BackColor = System.Drawing.Color.White
        Me.m_gridGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridGroups.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridGroups.CustomSort = False
        resources.ApplyResources(Me.m_gridGroups, "m_gridGroups")
        Me.m_gridGroups.FixedColumnWidths = False
        Me.m_gridGroups.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridGroups.GridToolTipActive = True
        Me.m_gridGroups.Name = "m_gridGroups"
        Me.m_gridGroups.SelectedGroup = Nothing
        Me.m_gridGroups.SelectedTaxon = Nothing
        Me.m_gridGroups.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridGroups.UIContext = Nothing
        '
        'm_gridResults
        '
        Me.m_gridResults.AllowBlockSelect = True
        resources.ApplyResources(Me.m_gridResults, "m_gridResults")
        Me.m_gridResults.AutoSizeMinHeight = 10
        Me.m_gridResults.AutoSizeMinWidth = 10
        Me.m_gridResults.AutoStretchColumnsToFitWidth = False
        Me.m_gridResults.AutoStretchRowsToFitHeight = False
        Me.m_gridResults.BackColor = System.Drawing.Color.White
        Me.m_gridResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridResults.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridResults.CustomSort = False
        Me.m_gridResults.FixedColumnWidths = False
        Me.m_gridResults.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridResults.GridToolTipActive = True
        Me.m_gridResults.Name = "m_gridResults"
        Me.m_gridResults.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridResults.UIContext = Nothing
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
        'dlgEditGroupTaxon
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_scMain)
        Me.Controls.Add(Me.m_hdrExternal)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.m_hdrEdit)
        Me.Controls.Add(Me.m_hdrOrder)
        Me.Controls.Add(Me.m_btnKeep)
        Me.Controls.Add(Me.m_btnMoveDown)
        Me.Controls.Add(Me.m_btnMoveUp)
        Me.Controls.Add(Me.m_btnUpdateAll)
        Me.Controls.Add(Me.m_btnUpdate)
        Me.Controls.Add(Me.m_btnRemove)
        Me.Controls.Add(Me.m_btnAdd)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgEditGroupTaxon"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.ResumeLayout(False)
        Me.m_tcMain.ResumeLayout(False)
        Me.m_tpSearch.ResumeLayout(False)
        Me.m_tpSearch.PerformLayout()
        Me.m_tpDetails.ResumeLayout(False)
        Me.m_tpDetails.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btnRemove As System.Windows.Forms.Button
    Private WithEvents m_btnUpdate As System.Windows.Forms.Button
    Private WithEvents m_gridGroups As ScientificInterface.gridEditGroupTaxon
    Private WithEvents m_hdrOrder As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnKeep As System.Windows.Forms.Button
    Private WithEvents m_btnMoveDown As System.Windows.Forms.Button
    Private WithEvents m_btnMoveUp As System.Windows.Forms.Button
    Private WithEvents m_hdrEdit As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents Cancel_Button As System.Windows.Forms.Button
    Private WithEvents OK_Button As System.Windows.Forms.Button
    Private WithEvents m_hdrExternal As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnUpdateAll As System.Windows.Forms.Button
    Private WithEvents m_lbTerm As System.Windows.Forms.Label
    Private WithEvents m_tbSearch As System.Windows.Forms.TextBox
    Private WithEvents m_cmbEngine As System.Windows.Forms.ComboBox
    Private WithEvents m_btnConfigure As System.Windows.Forms.Button
    Private WithEvents m_btnAdd As System.Windows.Forms.Button
    Private WithEvents m_cbIncludeExtent As System.Windows.Forms.CheckBox
    Private WithEvents m_tcMain As System.Windows.Forms.TabControl
    Friend WithEvents m_tpSearch As System.Windows.Forms.TabPage
    Friend WithEvents m_tpDetails As System.Windows.Forms.TabPage
    Friend WithEvents m_gridResults As ScientificInterface.gridTaxonSearchResults
    Private WithEvents m_cmbSpecies As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbGenus As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbFamily As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbOrder As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbClass As System.Windows.Forms.ComboBox
    Private WithEvents m_lbSpecies As System.Windows.Forms.Label
    Private WithEvents m_lbGenus As System.Windows.Forms.Label
    Private WithEvents m_lbFamily As System.Windows.Forms.Label
    Private WithEvents m_lbOrder As System.Windows.Forms.Label
    Private WithEvents m_tbCommon As System.Windows.Forms.TextBox
    Private WithEvents m_lbClass As System.Windows.Forms.Label
    Private WithEvents m_lbCommon As System.Windows.Forms.Label
    Private WithEvents m_lblEngine As System.Windows.Forms.Label
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_btnSearchCommon As System.Windows.Forms.Button
    Private WithEvents m_btnSearchSpecies As System.Windows.Forms.Button
    Private WithEvents m_btnSearchGenus As System.Windows.Forms.Button
    Private WithEvents m_btnSearchFamily As System.Windows.Forms.Button
    Private WithEvents m_btnSearchClass As System.Windows.Forms.Button
    Private WithEvents m_btnSearchOrder As System.Windows.Forms.Button
    Private WithEvents m_lblSelectedGroup As System.Windows.Forms.Label
    Private WithEvents m_lblGroup As System.Windows.Forms.Label
    Private WithEvents m_cmbPhylum As System.Windows.Forms.ComboBox
    Private WithEvents m_lblPhylum As System.Windows.Forms.Label
    Private WithEvents m_btnPhylum As System.Windows.Forms.Button

End Class
