' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    Partial Class dlgEditHabitats
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditHabitats))
            Me.m_grid = New ScientificInterface.Ecospace.gridEditHabitats()
            Me.m_btnAddHabitat = New System.Windows.Forms.Button()
            Me.m_btnRemoveHabitat = New System.Windows.Forms.Button()
            Me.OK_Button = New System.Windows.Forms.Button()
            Me.Cancel_Button = New System.Windows.Forms.Button()
            Me.m_btnKeep = New System.Windows.Forms.Button()
            Me.m_hdrOrder = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_btnMoveDown = New System.Windows.Forms.Button()
            Me.m_btnMoveUp = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = True
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
            Me.m_grid.DataName = "grid content"
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.IsLayoutSuspended = False

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
            'm_btnAddHabitat
            '
            resources.ApplyResources(Me.m_btnAddHabitat, "m_btnAddHabitat")
            Me.m_btnAddHabitat.Name = "m_btnAddHabitat"
            Me.m_btnAddHabitat.UseVisualStyleBackColor = True
            '
            'm_btnRemoveHabitat
            '
            resources.ApplyResources(Me.m_btnRemoveHabitat, "m_btnRemoveHabitat")
            Me.m_btnRemoveHabitat.Name = "m_btnRemoveHabitat"
            Me.m_btnRemoveHabitat.UseVisualStyleBackColor = True
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
            'm_hdrOrder
            '
            resources.ApplyResources(Me.m_hdrOrder, "m_hdrOrder")
            Me.m_hdrOrder.CanCollapseParent = False
            Me.m_hdrOrder.CollapsedParentHeight = 0
            Me.m_hdrOrder.IsCollapsed = False
            Me.m_hdrOrder.Name = "m_hdrOrder"
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
            'dlgEditHabitats
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.m_hdrOrder)
            Me.Controls.Add(Me.m_btnMoveDown)
            Me.Controls.Add(Me.m_btnMoveUp)
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_btnKeep)
            Me.Controls.Add(Me.Cancel_Button)
            Me.Controls.Add(Me.m_btnRemoveHabitat)
            Me.Controls.Add(Me.m_btnAddHabitat)
            Me.Controls.Add(Me.m_grid)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgEditHabitats"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_grid As gridEditHabitats
        Private WithEvents m_btnRemoveHabitat As System.Windows.Forms.Button
        Private WithEvents m_btnAddHabitat As System.Windows.Forms.Button
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_btnKeep As System.Windows.Forms.Button
        Private WithEvents m_hdrOrder As cEwEHeaderLabel
        Private WithEvents m_btnMoveDown As Button
        Private WithEvents m_btnMoveUp As Button
    End Class

End Namespace

