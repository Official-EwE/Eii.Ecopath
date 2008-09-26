Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgEditRegions
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditRegions))
            Me.m_grid = New ScientificInterface.Ecospace.gridEditRegions
            Me.m_btnAddRegion = New System.Windows.Forms.Button
            Me.m_btnRemoveRegion = New System.Windows.Forms.Button
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_btnPreserve = New System.Windows.Forms.Button
            Me.spHorz = New ScientificInterfaceShared.Controls.ucFormSeparator
            Me.btnHabToRegion = New System.Windows.Forms.Button
            Me.btnFromCells = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
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
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.RemoveFocusCellOnLeave
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
            'm_btnAddRegion
            '
            resources.ApplyResources(Me.m_btnAddRegion, "m_btnAddRegion")
            Me.m_btnAddRegion.Name = "m_btnAddRegion"
            Me.m_btnAddRegion.UseVisualStyleBackColor = True
            '
            'm_btnRemoveRegion
            '
            resources.ApplyResources(Me.m_btnRemoveRegion, "m_btnRemoveRegion")
            Me.m_btnRemoveRegion.Name = "m_btnRemoveRegion"
            Me.m_btnRemoveRegion.UseVisualStyleBackColor = True
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
            'm_btnPreserve
            '
            resources.ApplyResources(Me.m_btnPreserve, "m_btnPreserve")
            Me.m_btnPreserve.Name = "m_btnPreserve"
            Me.m_btnPreserve.UseVisualStyleBackColor = True
            '
            'spHorz
            '
            resources.ApplyResources(Me.spHorz, "spHorz")
            Me.spHorz.Name = "spHorz"
            Me.spHorz.TabStop = False
            '
            'btnHabToRegion
            '
            resources.ApplyResources(Me.btnHabToRegion, "btnHabToRegion")
            Me.btnHabToRegion.Name = "btnHabToRegion"
            Me.btnHabToRegion.UseVisualStyleBackColor = True
            '
            'btnFromCells
            '
            resources.ApplyResources(Me.btnFromCells, "btnFromCells")
            Me.btnFromCells.Name = "btnFromCells"
            Me.btnFromCells.UseVisualStyleBackColor = True
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'dlgEditRegions
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.btnFromCells)
            Me.Controls.Add(Me.btnHabToRegion)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.spHorz)
            Me.Controls.Add(Me.m_btnPreserve)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.m_btnRemoveRegion)
            Me.Controls.Add(Me.m_btnAddRegion)
            Me.Controls.Add(Me.m_grid)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgEditRegions"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnRemoveRegion As System.Windows.Forms.Button
        Private WithEvents m_btnAddRegion As System.Windows.Forms.Button
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_btnPreserve As System.Windows.Forms.Button
        Private WithEvents spHorz As ucFormSeparator
        Private WithEvents btnFromCells As System.Windows.Forms.Button
        Private WithEvents btnHabToRegion As System.Windows.Forms.Button
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_grid As gridEditRegions

    End Class

End Namespace

