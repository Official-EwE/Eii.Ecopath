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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Ecospace

    Partial Class frmEcospaceFishery
        Inherits ScientificInterfaceShared.Forms.frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceFishery))
            Me.m_tsMain = New cEwEToolstrip()
            Me.m_tsbnDefineMPAs = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDefineHabitats = New System.Windows.Forms.ToolStripButton()
            Me.m_grid = New ScientificInterface.Ecospace.gridEcospaceFishery()
            Me.m_tsMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefineHabitats, Me.m_tsbnDefineMPAs})
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.Name = "m_tsMain"
            '
            'm_tsbnDefineMPAs
            '
            resources.ApplyResources(Me.m_tsbnDefineMPAs, "m_tsbnDefineMPAs")
            Me.m_tsbnDefineMPAs.Name = "m_tsbnDefineMPAs"
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
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
            Me.m_grid.DataName = "grid content"
            resources.ApplyResources(Me.m_grid, "m_grid")
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
            'm_tsbnDefineHabitats
            '
            resources.ApplyResources(Me.m_tsbnDefineHabitats, "m_tsbnDefineHabitats")
            Me.m_tsbnDefineHabitats.Name = "m_tsbnDefineHabitats"
            '
            'frmEcospaceFishery
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_tsMain)
            Me.Name = "frmEcospaceFishery"
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsMain As cEwEToolstrip
        Private WithEvents m_tsbnDefineMPAs As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnDefineHabitats As System.Windows.Forms.ToolStripButton
        Private WithEvents m_grid As gridEcospaceFishery
    End Class

End Namespace
