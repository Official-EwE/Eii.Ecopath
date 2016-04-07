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

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Namespace Ecotracer

    Partial Class frmEcotracerInput
        Inherits frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotracerInput))
            Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip
            Me.m_plAaargh = New System.Windows.Forms.Panel
            Me.m_hdrGroups = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_lbFFEnv = New System.Windows.Forms.Label
            Me.m_cmbEnvInflowFF = New System.Windows.Forms.ComboBox
            Me.m_tlp = New System.Windows.Forms.TableLayoutPanel
            Me.m_lbCZeroEnv = New System.Windows.Forms.Label
            Me.m_lbCDecayRateEnv = New System.Windows.Forms.Label
            Me.m_lblCInflowEnv = New System.Windows.Forms.Label
            Me.m_lblCDecay = New System.Windows.Forms.Label
            Me.m_tbCDecayRateEnv = New System.Windows.Forms.TextBox
            Me.m_tbCInflowEnv = New System.Windows.Forms.TextBox
            Me.m_tbCLossEnv = New System.Windows.Forms.TextBox
            Me.m_tbCZeroEnv = New System.Windows.Forms.TextBox
            Me.m_grid = New ScientificInterface.Ecotracer.gridEcotracerInput
            Me.m_hdrInit = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tlpGroups = New System.Windows.Forms.TableLayoutPanel
            Me.m_plAaargh.SuspendLayout()
            Me.m_tlp.SuspendLayout()
            Me.m_tlpGroups.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMain.Name = "m_tsMain"
            Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_plAaargh
            '
            Me.m_plAaargh.Controls.Add(Me.m_tlpGroups)
            Me.m_plAaargh.Controls.Add(Me.m_hdrGroups)
            Me.m_plAaargh.Controls.Add(Me.m_lbFFEnv)
            Me.m_plAaargh.Controls.Add(Me.m_cmbEnvInflowFF)
            Me.m_plAaargh.Controls.Add(Me.m_tlp)
            Me.m_plAaargh.Controls.Add(Me.m_hdrInit)
            resources.ApplyResources(Me.m_plAaargh, "m_plAaargh")
            Me.m_plAaargh.Name = "m_plAaargh"
            '
            'm_hdrGroups
            '
            resources.ApplyResources(Me.m_hdrGroups, "m_hdrGroups")
            Me.m_hdrGroups.Name = "m_hdrGroups"
            '
            'm_lbFFEnv
            '
            resources.ApplyResources(Me.m_lbFFEnv, "m_lbFFEnv")
            Me.m_lbFFEnv.Name = "m_lbFFEnv"
            '
            'm_cmbEnvInflowFF
            '
            Me.m_cmbEnvInflowFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbEnvInflowFF.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbEnvInflowFF, "m_cmbEnvInflowFF")
            Me.m_cmbEnvInflowFF.Name = "m_cmbEnvInflowFF"
            '
            'm_tlp
            '
            resources.ApplyResources(Me.m_tlp, "m_tlp")
            Me.m_tlp.Controls.Add(Me.m_lbCZeroEnv, 0, 0)
            Me.m_tlp.Controls.Add(Me.m_lbCDecayRateEnv, 0, 1)
            Me.m_tlp.Controls.Add(Me.m_lblCInflowEnv, 3, 0)
            Me.m_tlp.Controls.Add(Me.m_lblCDecay, 3, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCDecayRateEnv, 1, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCInflowEnv, 4, 0)
            Me.m_tlp.Controls.Add(Me.m_tbCLossEnv, 4, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCZeroEnv, 1, 0)
            Me.m_tlp.Name = "m_tlp"
            '
            'm_lbCZeroEnv
            '
            resources.ApplyResources(Me.m_lbCZeroEnv, "m_lbCZeroEnv")
            Me.m_lbCZeroEnv.Name = "m_lbCZeroEnv"
            '
            'm_lbCDecayRateEnv
            '
            resources.ApplyResources(Me.m_lbCDecayRateEnv, "m_lbCDecayRateEnv")
            Me.m_lbCDecayRateEnv.Name = "m_lbCDecayRateEnv"
            '
            'm_lblCInflowEnv
            '
            resources.ApplyResources(Me.m_lblCInflowEnv, "m_lblCInflowEnv")
            Me.m_lblCInflowEnv.Name = "m_lblCInflowEnv"
            '
            'm_lblCDecay
            '
            resources.ApplyResources(Me.m_lblCDecay, "m_lblCDecay")
            Me.m_lblCDecay.Name = "m_lblCDecay"
            '
            'm_tbCDecayRateEnv
            '
            resources.ApplyResources(Me.m_tbCDecayRateEnv, "m_tbCDecayRateEnv")
            Me.m_tbCDecayRateEnv.Name = "m_tbCDecayRateEnv"
            '
            'm_tbCInflowEnv
            '
            resources.ApplyResources(Me.m_tbCInflowEnv, "m_tbCInflowEnv")
            Me.m_tbCInflowEnv.Name = "m_tbCInflowEnv"
            '
            'm_tbCLossEnv
            '
            resources.ApplyResources(Me.m_tbCLossEnv, "m_tbCLossEnv")
            Me.m_tbCLossEnv.Name = "m_tbCLossEnv"
            '
            'm_tbCZeroEnv
            '
            resources.ApplyResources(Me.m_tbCZeroEnv, "m_tbCZeroEnv")
            Me.m_tbCZeroEnv.Name = "m_tbCZeroEnv"
            '
            'm_grid
            '
            Me.m_grid.AllowBlockSelect = True
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
            Me.m_grid.UIContext = Nothing
            '
            'm_hdrInit
            '
            resources.ApplyResources(Me.m_hdrInit, "m_hdrInit")
            Me.m_hdrInit.Name = "m_hdrInit"
            '
            'm_tlpGroups
            '
            resources.ApplyResources(Me.m_tlpGroups, "m_tlpGroups")
            Me.m_tlpGroups.Controls.Add(Me.m_tsMain, 0, 0)
            Me.m_tlpGroups.Controls.Add(Me.m_grid, 0, 1)
            Me.m_tlpGroups.Name = "m_tlpGroups"
            '
            'frmEcotracerInput
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_plAaargh)
            Me.Name = "frmEcotracerInput"
            Me.m_plAaargh.ResumeLayout(False)
            Me.m_plAaargh.PerformLayout()
            Me.m_tlp.ResumeLayout(False)
            Me.m_tlp.PerformLayout()
            Me.m_tlpGroups.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tsMain As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_plAaargh As System.Windows.Forms.Panel
        Private WithEvents m_hdrGroups As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_lbFFEnv As System.Windows.Forms.Label
        Private WithEvents m_cmbEnvInflowFF As System.Windows.Forms.ComboBox
        Private WithEvents m_tlp As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lbCZeroEnv As System.Windows.Forms.Label
        Private WithEvents m_lbCDecayRateEnv As System.Windows.Forms.Label
        Private WithEvents m_lblCInflowEnv As System.Windows.Forms.Label
        Private WithEvents m_lblCDecay As System.Windows.Forms.Label
        Private WithEvents m_tbCDecayRateEnv As System.Windows.Forms.TextBox
        Private WithEvents m_tbCInflowEnv As System.Windows.Forms.TextBox
        Private WithEvents m_tbCLossEnv As System.Windows.Forms.TextBox
        Private WithEvents m_tbCZeroEnv As System.Windows.Forms.TextBox
        Private WithEvents m_grid As ScientificInterface.Ecotracer.gridEcotracerInput
        Private WithEvents m_hdrInit As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_tlpGroups As System.Windows.Forms.TableLayoutPanel
    End Class

End Namespace
