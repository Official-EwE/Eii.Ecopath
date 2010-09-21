Imports ScientificInterfaceShared.Controls

Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgEditPedigree
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditPedigree))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_btnKeep = New System.Windows.Forms.Button
            Me.m_btnDelete = New System.Windows.Forms.Button
            Me.m_btnSort = New System.Windows.Forms.Button
            Me.m_btnInsert = New System.Windows.Forms.Button
            Me.m_lblVariable = New System.Windows.Forms.Label
            Me.m_cmbVariable = New System.Windows.Forms.ComboBox
            Me.m_hdrOrder = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrEdit = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_grid = New ScientificInterface.gridEditPedigree
            Me.m_scYupi = New System.Windows.Forms.SplitContainer
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_scYupi.Panel1.SuspendLayout()
            Me.m_scYupi.Panel2.SuspendLayout()
            Me.m_scYupi.SuspendLayout()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'm_btnKeep
            '
            resources.ApplyResources(Me.m_btnKeep, "m_btnKeep")
            Me.m_btnKeep.Name = "m_btnKeep"
            Me.m_btnKeep.UseVisualStyleBackColor = True
            '
            'm_btnDelete
            '
            resources.ApplyResources(Me.m_btnDelete, "m_btnDelete")
            Me.m_btnDelete.Name = "m_btnDelete"
            Me.m_btnDelete.UseVisualStyleBackColor = True
            '
            'm_btnSort
            '
            resources.ApplyResources(Me.m_btnSort, "m_btnSort")
            Me.m_btnSort.Name = "m_btnSort"
            Me.m_btnSort.UseVisualStyleBackColor = True
            '
            'm_btnInsert
            '
            resources.ApplyResources(Me.m_btnInsert, "m_btnInsert")
            Me.m_btnInsert.Name = "m_btnInsert"
            Me.m_btnInsert.UseVisualStyleBackColor = True
            '
            'm_lblVariable
            '
            resources.ApplyResources(Me.m_lblVariable, "m_lblVariable")
            Me.m_lblVariable.Name = "m_lblVariable"
            '
            'm_cmbVariable
            '
            resources.ApplyResources(Me.m_cmbVariable, "m_cmbVariable")
            Me.m_cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbVariable.FormattingEnabled = True
            Me.m_cmbVariable.Name = "m_cmbVariable"
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
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = False
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = True
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.FixedColumnWidths = False
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
            Me.m_grid.TrackPropertySelection = False
            Me.m_grid.UIContext = Nothing
            Me.m_grid.VarName = EwEUtils.Core.eVarNameFlags.NotSet
            '
            'm_scYupi
            '
            resources.ApplyResources(Me.m_scYupi, "m_scYupi")
            Me.m_scYupi.Name = "m_scYupi"
            '
            'm_scYupi.Panel1
            '
            Me.m_scYupi.Panel1.Controls.Add(Me.m_grid)
            '
            'm_scYupi.Panel2
            '
            Me.m_scYupi.Panel2.Controls.Add(Me.m_tbDescription)
            Me.m_scYupi.Panel2.Controls.Add(Me.m_lblDescription)
            '
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_tbDescription
            '
            resources.ApplyResources(Me.m_tbDescription, "m_tbDescription")
            Me.m_tbDescription.Name = "m_tbDescription"
            '
            'dlgEditPedigree
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.m_scYupi)
            Me.Controls.Add(Me.m_cmbVariable)
            Me.Controls.Add(Me.m_lblVariable)
            Me.Controls.Add(Me.m_hdrOrder)
            Me.Controls.Add(Me.m_hdrEdit)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.m_btnDelete)
            Me.Controls.Add(Me.m_btnSort)
            Me.Controls.Add(Me.m_btnInsert)
            Me.Name = "dlgEditPedigree"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.m_scYupi.Panel1.ResumeLayout(False)
            Me.m_scYupi.Panel2.ResumeLayout(False)
            Me.m_scYupi.Panel2.PerformLayout()
            Me.m_scYupi.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_btnSort As System.Windows.Forms.Button
        Private WithEvents m_btnInsert As System.Windows.Forms.Button
        Private WithEvents m_hdrEdit As cEwEHeaderLabel
        Private WithEvents m_hdrOrder As cEwEHeaderLabel
        Private WithEvents m_grid As gridEditPedigree
        Private WithEvents m_lblVariable As System.Windows.Forms.Label
        Private WithEvents m_cmbVariable As System.Windows.Forms.ComboBox
        Private WithEvents m_scYupi As System.Windows.Forms.SplitContainer
        Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Private WithEvents m_lblDescription As System.Windows.Forms.Label

    End Class

End Namespace

