' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcospaceFit
    Inherits ScientificInterfaceShared.Forms.frmEwE

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
        Me.components = New System.ComponentModel.Container()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.m_btClear = New System.Windows.Forms.Button()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_grdFit = New EwEEcospaceFitPlugin.gridFit(Me.components)
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.m_grdFit)
        Me.Panel1.Location = New System.Drawing.Point(12, 65)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(742, 401)
        Me.Panel1.TabIndex = 1
        '
        'm_btClear
        '
        Me.m_btClear.Location = New System.Drawing.Point(12, 38)
        Me.m_btClear.Name = "m_btClear"
        Me.m_btClear.Size = New System.Drawing.Size(102, 21)
        Me.m_btClear.TabIndex = 2
        Me.m_btClear.Text = "Clear"
        Me.m_btClear.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(9, 9)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(745, 26)
        Me.CEwEHeaderLabel1.TabIndex = 3
        Me.CEwEHeaderLabel1.Text = "Fit of Ecospace to Ecopath biomass"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_grdFit
        '
        Me.m_grdFit.AllowBlockSelect = True
        Me.m_grdFit.AutoSizeMinHeight = 10
        Me.m_grdFit.AutoSizeMinWidth = 10
        Me.m_grdFit.AutoStretchColumnsToFitWidth = False
        Me.m_grdFit.AutoStretchRowsToFitHeight = False
        Me.m_grdFit.BackColor = System.Drawing.Color.White
        Me.m_grdFit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grdFit.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_grdFit.CustomSort = False
        Me.m_grdFit.DataName = "grid content"
        Me.m_grdFit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_grdFit.FixedColumnWidths = True
        Me.m_grdFit.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grdFit.GridToolTipActive = True
        Me.m_grdFit.IsLayoutSuspended = False
        Me.m_grdFit.Location = New System.Drawing.Point(0, 0)
        Me.m_grdFit.Name = "m_grdFit"
        Me.m_grdFit.Size = New System.Drawing.Size(742, 401)
        Me.m_grdFit.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grdFit.TabIndex = 0
        Me.m_grdFit.UIContext = Nothing
        '
        'frmEcospaceFit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(766, 478)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_btClear)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceFit"
        Me.Text = "Ecospace fit to Ecopath biomass"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents m_grdFit As EwEEcospaceFitPlugin.gridFit
    Friend WithEvents m_btClear As System.Windows.Forms.Button
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
