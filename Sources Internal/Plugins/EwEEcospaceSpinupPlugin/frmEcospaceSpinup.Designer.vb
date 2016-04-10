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

Imports ScientificInterfaceShared

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcospaceSpinup
    Inherits ScientificInterfaceShared.Forms.frmEwE

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
        Me.components = New System.ComponentModel.Container()
        Me.m_chkUseSpinup = New System.Windows.Forms.CheckBox()
        Me.m_txSpinUpYears = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_chkUseBaseBio = New System.Windows.Forms.CheckBox()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.m_gridSpinUpDif = New EwEEcospaceSpinupPlugin.gridSpinupDiff(Me.components)
        Me.m_plControls = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        Me.m_plControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_chkUseSpinup
        '
        Me.m_chkUseSpinup.AutoSize = True
        Me.m_chkUseSpinup.Location = New System.Drawing.Point(6, 3)
        Me.m_chkUseSpinup.Name = "m_chkUseSpinup"
        Me.m_chkUseSpinup.Size = New System.Drawing.Size(114, 17)
        Me.m_chkUseSpinup.TabIndex = 0
        Me.m_chkUseSpinup.Text = "Use spin-up period"
        Me.m_chkUseSpinup.UseVisualStyleBackColor = True
        '
        'm_txSpinUpYears
        '
        Me.m_txSpinUpYears.Location = New System.Drawing.Point(123, 47)
        Me.m_txSpinUpYears.Name = "m_txSpinUpYears"
        Me.m_txSpinUpYears.Size = New System.Drawing.Size(83, 20)
        Me.m_txSpinUpYears.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(114, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Spin-up length in years"
        '
        'm_chkUseBaseBio
        '
        Me.m_chkUseBaseBio.AutoSize = True
        Me.m_chkUseBaseBio.Location = New System.Drawing.Point(6, 26)
        Me.m_chkUseBaseBio.Name = "m_chkUseBaseBio"
        Me.m_chkUseBaseBio.Size = New System.Drawing.Size(162, 17)
        Me.m_chkUseBaseBio.TabIndex = 3
        Me.m_chkUseBaseBio.TabStop = False
        Me.m_chkUseBaseBio.Text = "Plot relative to Ecopath base"
        Me.m_chkUseBaseBio.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel1
        '
        Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(12, 9)
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(588, 18)
        Me.CEwEHeaderLabel1.TabIndex = 4
        Me.CEwEHeaderLabel1.Text = "Ecospace spin-up configuartion"
        Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.m_gridSpinUpDif)
        Me.Panel1.Location = New System.Drawing.Point(15, 115)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(585, 285)
        Me.Panel1.TabIndex = 5
        '
        'm_gridSpinUpDif
        '
        Me.m_gridSpinUpDif.AllowBlockSelect = True
        Me.m_gridSpinUpDif.AutoSizeMinHeight = 10
        Me.m_gridSpinUpDif.AutoSizeMinWidth = 10
        Me.m_gridSpinUpDif.AutoStretchColumnsToFitWidth = False
        Me.m_gridSpinUpDif.AutoStretchRowsToFitHeight = False
        Me.m_gridSpinUpDif.BackColor = System.Drawing.Color.White
        Me.m_gridSpinUpDif.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridSpinUpDif.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridSpinUpDif.CustomSort = False
        Me.m_gridSpinUpDif.DataName = "grid content"
        Me.m_gridSpinUpDif.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_gridSpinUpDif.FixedColumnWidths = True
        Me.m_gridSpinUpDif.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridSpinUpDif.GridToolTipActive = True
        Me.m_gridSpinUpDif.IsLayoutSuspended = False
        Me.m_gridSpinUpDif.Location = New System.Drawing.Point(0, 0)
        Me.m_gridSpinUpDif.Name = "m_gridSpinUpDif"
        Me.m_gridSpinUpDif.Size = New System.Drawing.Size(585, 285)
        Me.m_gridSpinUpDif.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridSpinUpDif.TabIndex = 0
        Me.m_gridSpinUpDif.UIContext = Nothing
        '
        'm_plControls
        '
        Me.m_plControls.Controls.Add(Me.m_chkUseSpinup)
        Me.m_plControls.Controls.Add(Me.m_txSpinUpYears)
        Me.m_plControls.Controls.Add(Me.Label1)
        Me.m_plControls.Controls.Add(Me.m_chkUseBaseBio)
        Me.m_plControls.Location = New System.Drawing.Point(15, 30)
        Me.m_plControls.Name = "m_plControls"
        Me.m_plControls.Size = New System.Drawing.Size(315, 79)
        Me.m_plControls.TabIndex = 6
        '
        'frmEcospaceSpinup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(612, 412)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_plControls)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmEcospaceSpinup"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace spin-up"
        Me.Text = "Ecospace spin-up"
        Me.Panel1.ResumeLayout(False)
        Me.m_plControls.ResumeLayout(False)
        Me.m_plControls.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_chkUseSpinup As System.Windows.Forms.CheckBox
    Friend WithEvents m_txSpinUpYears As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents m_chkUseBaseBio As System.Windows.Forms.CheckBox
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents m_gridSpinUpDif As EwEEcospaceSpinupPlugin.gridSpinupDiff
    Friend WithEvents m_plControls As System.Windows.Forms.Panel
End Class
