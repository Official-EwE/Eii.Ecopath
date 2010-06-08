Imports ScientificInterfaceShared.Controls

Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditFleets
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditFleets))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_btnKeep = New System.Windows.Forms.Button
            Me.m_btnDelete = New System.Windows.Forms.Button
            Me.m_btnMoveDown = New System.Windows.Forms.Button
            Me.m_btnMoveUp = New System.Windows.Forms.Button
            Me.m_btnInsert = New System.Windows.Forms.Button
            Me.m_btnDefaultAll = New System.Windows.Forms.Button
            Me.m_btnDefaultCurrent = New System.Windows.Forms.Button
            Me.m_btnCustom = New System.Windows.Forms.Button
            Me.m_hdrColors = New cEwEHeaderLabel
            Me.m_hdrOrder = New cEwEHeaderLabel
            Me.m_hdrEdit = New cEwEHeaderLabel
            Me.m_grid = New gridEditFleets
            Me.TableLayoutPanel1.SuspendLayout()
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
            'm_btnInsert
            '
            resources.ApplyResources(Me.m_btnInsert, "m_btnInsert")
            Me.m_btnInsert.Name = "m_btnInsert"
            Me.m_btnInsert.UseVisualStyleBackColor = True
            '
            'm_btnDefaultAll
            '
            resources.ApplyResources(Me.m_btnDefaultAll, "m_btnDefaultAll")
            Me.m_btnDefaultAll.Name = "m_btnDefaultAll"
            Me.m_btnDefaultAll.UseVisualStyleBackColor = True
            '
            'm_btnDefaultCurrent
            '
            resources.ApplyResources(Me.m_btnDefaultCurrent, "m_btnDefaultCurrent")
            Me.m_btnDefaultCurrent.Name = "m_btnDefaultCurrent"
            Me.m_btnDefaultCurrent.UseVisualStyleBackColor = True
            '
            'm_btnCustom
            '
            resources.ApplyResources(Me.m_btnCustom, "m_btnCustom")
            Me.m_btnCustom.Name = "m_btnCustom"
            Me.m_btnCustom.UseVisualStyleBackColor = True
            '
            'm_hdrColors
            '
            resources.ApplyResources(Me.m_hdrColors, "m_hdrColors")
            Me.m_hdrColors.Name = "m_hdrColors"
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
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
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
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'EditFleets
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.m_hdrColors)
            Me.Controls.Add(Me.m_hdrOrder)
            Me.Controls.Add(Me.m_hdrEdit)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.m_btnDelete)
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_btnCustom)
            Me.Controls.Add(Me.m_btnDefaultCurrent)
            Me.Controls.Add(Me.m_btnDefaultAll)
            Me.Controls.Add(Me.m_btnMoveDown)
            Me.Controls.Add(Me.m_btnMoveUp)
            Me.Controls.Add(Me.m_btnInsert)
            Me.Name = "EditFleets"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_btnMoveDown As System.Windows.Forms.Button
        Private WithEvents m_btnMoveUp As System.Windows.Forms.Button
        Private WithEvents m_btnInsert As System.Windows.Forms.Button
        Private WithEvents m_hdrEdit As cEwEHeaderLabel
        Private WithEvents m_hdrOrder As cEwEHeaderLabel
        Private WithEvents m_grid As gridEditFleets
        Private WithEvents m_btnDefaultAll As System.Windows.Forms.Button
        Private WithEvents m_hdrColors As cEwEHeaderLabel
        Private WithEvents m_btnDefaultCurrent As System.Windows.Forms.Button
        Private WithEvents m_btnCustom As System.Windows.Forms.Button

    End Class

End Namespace

