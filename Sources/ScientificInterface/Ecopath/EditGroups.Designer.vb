Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditGroups
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditGroups))
            Me.m_grid = New ScientificInterface.EditGroupsStanzaEwEGrid
            Me.m_btnInsert = New System.Windows.Forms.Button
            Me.m_btnMoveUp = New System.Windows.Forms.Button
            Me.m_btnMoveDown = New System.Windows.Forms.Button
            Me.m_btnDelete = New System.Windows.Forms.Button
            Me.m_btnKeep = New System.Windows.Forms.Button
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_bntColorScale = New System.Windows.Forms.Button
            Me.m_btnColorDefaults = New System.Windows.Forms.Button
            Me.m_lbColours = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_lbOrder = New System.Windows.Forms.Label
            Me.m_btnCustomColour = New System.Windows.Forms.Button
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
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
            Me.m_grid.TrackPropertySelection = True
            '
            'm_btnInsert
            '
            resources.ApplyResources(Me.m_btnInsert, "m_btnInsert")
            Me.m_btnInsert.Name = "m_btnInsert"
            Me.m_btnInsert.UseVisualStyleBackColor = True
            '
            'm_btnMoveUp
            '
            resources.ApplyResources(Me.m_btnMoveUp, "m_btnMoveUp")
            Me.m_btnMoveUp.Name = "m_btnMoveUp"
            Me.m_btnMoveUp.UseVisualStyleBackColor = True
            '
            'm_btnMoveDown
            '
            resources.ApplyResources(Me.m_btnMoveDown, "m_btnMoveDown")
            Me.m_btnMoveDown.Name = "m_btnMoveDown"
            Me.m_btnMoveDown.UseVisualStyleBackColor = True
            '
            'm_btnDelete
            '
            resources.ApplyResources(Me.m_btnDelete, "m_btnDelete")
            Me.m_btnDelete.Name = "m_btnDelete"
            Me.m_btnDelete.UseVisualStyleBackColor = True
            '
            'm_btnKeep
            '
            resources.ApplyResources(Me.m_btnKeep, "m_btnKeep")
            Me.m_btnKeep.Name = "m_btnKeep"
            Me.m_btnKeep.UseVisualStyleBackColor = True
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
            'm_bntColorScale
            '
            resources.ApplyResources(Me.m_bntColorScale, "m_bntColorScale")
            Me.m_bntColorScale.Name = "m_bntColorScale"
            Me.m_bntColorScale.UseVisualStyleBackColor = True
            '
            'm_btnColorDefaults
            '
            resources.ApplyResources(Me.m_btnColorDefaults, "m_btnColorDefaults")
            Me.m_btnColorDefaults.Name = "m_btnColorDefaults"
            Me.m_btnColorDefaults.UseVisualStyleBackColor = True
            '
            'm_lbColours
            '
            resources.ApplyResources(Me.m_lbColours, "m_lbColours")
            Me.m_lbColours.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbColours.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbColours.Name = "m_lbColours"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.BackColor = System.Drawing.SystemColors.ControlDark
            Me.Label1.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.Label1.Name = "Label1"
            '
            'm_lbOrder
            '
            resources.ApplyResources(Me.m_lbOrder, "m_lbOrder")
            Me.m_lbOrder.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbOrder.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbOrder.Name = "m_lbOrder"
            '
            'm_btnCustomColour
            '
            resources.ApplyResources(Me.m_btnCustomColour, "m_btnCustomColour")
            Me.m_btnCustomColour.Name = "m_btnCustomColour"
            Me.m_btnCustomColour.UseVisualStyleBackColor = True
            '
            'EditGroups
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.m_btnCustomColour)
            Me.Controls.Add(Me.m_btnColorDefaults)
            Me.Controls.Add(Me.m_bntColorScale)
            Me.Controls.Add(Me.m_lbOrder)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_lbColours)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.m_btnDelete)
            Me.Controls.Add(Me.m_btnMoveDown)
            Me.Controls.Add(Me.m_btnMoveUp)
            Me.Controls.Add(Me.m_btnInsert)
            Me.Controls.Add(Me.m_grid)
            Me.Name = "EditGroups"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents m_grid As EditGroupsStanzaEwEGrid
        Private WithEvents m_btnInsert As System.Windows.Forms.Button
        Private WithEvents m_btnMoveUp As System.Windows.Forms.Button
        Private WithEvents m_btnMoveDown As System.Windows.Forms.Button
        Private WithEvents m_btnDelete As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_bntColorScale As System.Windows.Forms.Button
        Private WithEvents m_btnColorDefaults As System.Windows.Forms.Button
        Private WithEvents m_lbColours As System.Windows.Forms.Label
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_lbOrder As System.Windows.Forms.Label
        Private WithEvents m_btnCustomColour As System.Windows.Forms.Button

    End Class

End Namespace

