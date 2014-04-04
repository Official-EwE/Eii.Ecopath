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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

Namespace Ecospace

    Partial Class dlgSelectResponse
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSelectResponse))
            Me.OK_Button = New System.Windows.Forms.Button()
            Me.Cancel_Button = New System.Windows.Forms.Button()
            Me.m_lvAllShapes = New System.Windows.Forms.ListView()
            Me.m_lvAppliedShapes = New System.Windows.Forms.ListView()
            Me.m_btnRemove = New System.Windows.Forms.Button()
            Me.m_btnAdd = New System.Windows.Forms.Button()
            Me.m_tlMain = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plButtons = New System.Windows.Forms.Panel()
            Me.m_hdrResp = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrApplied = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tslbFilter = New System.Windows.Forms.ToolStripLabel()
            Me.m_tstbFilter = New System.Windows.Forms.ToolStripTextBox()
            Me.m_tsbnCaseSensitive = New System.Windows.Forms.ToolStripButton()
            Me.m_tlMain.SuspendLayout()
            Me.m_plButtons.SuspendLayout()
            Me.m_tsMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'OK_Button
            '
            Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.OK_Button.Location = New System.Drawing.Point(434, 351)
            Me.OK_Button.Name = "OK_Button"
            Me.OK_Button.Size = New System.Drawing.Size(67, 23)
            Me.OK_Button.TabIndex = 0
            Me.OK_Button.Text = "OK"
            '
            'Cancel_Button
            '
            Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Location = New System.Drawing.Point(507, 351)
            Me.Cancel_Button.Name = "Cancel_Button"
            Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
            Me.Cancel_Button.TabIndex = 1
            Me.Cancel_Button.Text = "Cancel"
            '
            'm_lvAllShapes
            '
            Me.m_lvAllShapes.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lvAllShapes.FullRowSelect = True
            Me.m_lvAllShapes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.m_lvAllShapes.HideSelection = False
            Me.m_lvAllShapes.Location = New System.Drawing.Point(0, 18)
            Me.m_lvAllShapes.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lvAllShapes.MultiSelect = False
            Me.m_lvAllShapes.Name = "m_lvAllShapes"
            Me.m_lvAllShapes.ShowItemToolTips = True
            Me.m_lvAllShapes.Size = New System.Drawing.Size(264, 295)
            Me.m_lvAllShapes.TabIndex = 1
            Me.m_lvAllShapes.UseCompatibleStateImageBehavior = False
            Me.m_lvAllShapes.View = System.Windows.Forms.View.List
            '
            'm_lvAppliedShapes
            '
            Me.m_lvAppliedShapes.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lvAppliedShapes.FullRowSelect = True
            Me.m_lvAppliedShapes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.m_lvAppliedShapes.HideSelection = False
            Me.m_lvAppliedShapes.Location = New System.Drawing.Point(296, 18)
            Me.m_lvAppliedShapes.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lvAppliedShapes.Name = "m_lvAppliedShapes"
            Me.m_lvAppliedShapes.Size = New System.Drawing.Size(264, 295)
            Me.m_lvAppliedShapes.TabIndex = 3
            Me.m_lvAppliedShapes.UseCompatibleStateImageBehavior = False
            '
            'm_btnRemove
            '
            Me.m_btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.m_btnRemove.Image = CType(resources.GetObject("m_btnRemove.Image"), System.Drawing.Image)
            Me.m_btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_btnRemove.Location = New System.Drawing.Point(3, 148)
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.Size = New System.Drawing.Size(26, 23)
            Me.m_btnRemove.TabIndex = 9
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'm_btnAdd
            '
            Me.m_btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.m_btnAdd.Image = CType(resources.GetObject("m_btnAdd.Image"), System.Drawing.Image)
            Me.m_btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_btnAdd.Location = New System.Drawing.Point(3, 119)
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.Size = New System.Drawing.Size(26, 23)
            Me.m_btnAdd.TabIndex = 8
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_tlMain
            '
            Me.m_tlMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlMain.ColumnCount = 3
            Me.m_tlMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.m_tlMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlMain.Controls.Add(Me.m_lvAllShapes, 0, 1)
            Me.m_tlMain.Controls.Add(Me.m_lvAppliedShapes, 2, 1)
            Me.m_tlMain.Controls.Add(Me.m_plButtons, 1, 1)
            Me.m_tlMain.Controls.Add(Me.m_hdrResp, 0, 0)
            Me.m_tlMain.Controls.Add(Me.m_hdrApplied, 2, 0)
            Me.m_tlMain.Location = New System.Drawing.Point(12, 32)
            Me.m_tlMain.Name = "m_tlMain"
            Me.m_tlMain.RowCount = 2
            Me.m_tlMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlMain.Size = New System.Drawing.Size(560, 313)
            Me.m_tlMain.TabIndex = 10
            '
            'm_plButtons
            '
            Me.m_plButtons.Controls.Add(Me.m_btnAdd)
            Me.m_plButtons.Controls.Add(Me.m_btnRemove)
            Me.m_plButtons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plButtons.Location = New System.Drawing.Point(264, 18)
            Me.m_plButtons.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plButtons.Name = "m_plButtons"
            Me.m_plButtons.Size = New System.Drawing.Size(32, 295)
            Me.m_plButtons.TabIndex = 5
            '
            'm_hdrResp
            '
            Me.m_hdrResp.CanCollapseParent = False
            Me.m_hdrResp.CollapsedParentHeight = 0
            Me.m_hdrResp.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_hdrResp.IsCollapsed = False
            Me.m_hdrResp.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrResp.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrResp.Name = "m_hdrResp"
            Me.m_hdrResp.Size = New System.Drawing.Size(264, 18)
            Me.m_hdrResp.TabIndex = 6
            Me.m_hdrResp.Text = "Response functions"
            Me.m_hdrResp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_hdrApplied
            '
            Me.m_hdrApplied.CanCollapseParent = False
            Me.m_hdrApplied.CollapsedParentHeight = 0
            Me.m_hdrApplied.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_hdrApplied.IsCollapsed = False
            Me.m_hdrApplied.Location = New System.Drawing.Point(296, 0)
            Me.m_hdrApplied.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrApplied.Name = "m_hdrApplied"
            Me.m_hdrApplied.Size = New System.Drawing.Size(264, 18)
            Me.m_hdrApplied.TabIndex = 7
            Me.m_hdrApplied.Text = "Applied responses"
            Me.m_hdrApplied.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsMain
            '
            Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslbFilter, Me.m_tstbFilter, Me.m_tsbnCaseSensitive})
            Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
            Me.m_tsMain.Name = "m_tsMain"
            Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_tsMain.Size = New System.Drawing.Size(584, 25)
            Me.m_tsMain.TabIndex = 13
            Me.m_tsMain.Text = "ToolStrip1"
            '
            'm_tslbFilter
            '
            Me.m_tslbFilter.Name = "m_tslbFilter"
            Me.m_tslbFilter.Size = New System.Drawing.Size(36, 22)
            Me.m_tslbFilter.Text = "&Filter:"
            '
            'm_tstbFilter
            '
            Me.m_tstbFilter.Name = "m_tstbFilter"
            Me.m_tstbFilter.Size = New System.Drawing.Size(125, 25)
            '
            'm_tsbnCaseSensitive
            '
            Me.m_tsbnCaseSensitive.AutoToolTip = False
            Me.m_tsbnCaseSensitive.CheckOnClick = True
            Me.m_tsbnCaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnCaseSensitive.Image = CType(resources.GetObject("m_tsbnCaseSensitive.Image"), System.Drawing.Image)
            Me.m_tsbnCaseSensitive.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnCaseSensitive.Name = "m_tsbnCaseSensitive"
            Me.m_tsbnCaseSensitive.Size = New System.Drawing.Size(25, 22)
            Me.m_tsbnCaseSensitive.Text = "Aa"
            Me.m_tsbnCaseSensitive.ToolTipText = "Search case sensitive"
            '
            'dlgSelectResponse
            '
            Me.AcceptButton = Me.OK_Button
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ClientSize = New System.Drawing.Size(584, 386)
            Me.Controls.Add(Me.m_tsMain)
            Me.Controls.Add(Me.m_tlMain)
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.Cancel_Button)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(390, 228)
            Me.Name = "dlgSelectResponse"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Apply environmental response function"
            Me.m_tlMain.ResumeLayout(False)
            Me.m_plButtons.ResumeLayout(False)
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_lvAllShapes As System.Windows.Forms.ListView
        Private WithEvents m_lvAppliedShapes As System.Windows.Forms.ListView
        Private WithEvents m_btnRemove As System.Windows.Forms.Button
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents m_tlMain As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plButtons As System.Windows.Forms.Panel
        Private WithEvents m_hdrResp As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrApplied As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_tslbFilter As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbFilter As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsbnCaseSensitive As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsMain As ScientificInterfaceShared.Controls.cEwEToolstrip

    End Class

End Namespace
