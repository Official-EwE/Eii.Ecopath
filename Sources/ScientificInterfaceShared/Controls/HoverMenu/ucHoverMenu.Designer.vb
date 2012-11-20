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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Namespace Controls

    Partial Class ucHoverMenu
        Inherits UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnZoomIn = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomOut = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnZoomReset = New System.Windows.Forms.ToolStripButton()
            Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnShowLegends = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnShowAxisLabels = New System.Windows.Forms.ToolStripButton()
            Me.m_sep2 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnExport = New System.Windows.Forms.ToolStripButton()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.BackColor = System.Drawing.SystemColors.Control
            Me.m_ts.CanOverflow = False
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnZoomIn, Me.m_tsbnZoomOut, Me.m_tsbnZoomReset, Me.m_sep1, Me.m_tsbnShowLegends, Me.m_tsbnShowAxisLabels, Me.m_sep2, Me.m_tsbnExport})
            Me.m_ts.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_ts.Size = New System.Drawing.Size(800, 23)
            Me.m_ts.TabIndex = 0
            '
            'm_tsbnZoomIn
            '
            Me.m_tsbnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomIn.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomInHS
            Me.m_tsbnZoomIn.Name = "m_tsbnZoomIn"
            Me.m_tsbnZoomIn.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnZoomIn.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomIn.Text = "Zoom in"
            '
            'm_tsbnZoomOut
            '
            Me.m_tsbnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomOut.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomOutHS
            Me.m_tsbnZoomOut.Name = "m_tsbnZoomOut"
            Me.m_tsbnZoomOut.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnZoomOut.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomOut.Text = "Zoom out"
            '
            'm_tsbnZoomReset
            '
            Me.m_tsbnZoomReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomReset.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomHS
            Me.m_tsbnZoomReset.Name = "m_tsbnZoomReset"
            Me.m_tsbnZoomReset.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnZoomReset.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomReset.Text = "Reset all zoom"
            '
            'm_sep1
            '
            Me.m_sep1.AutoSize = False
            Me.m_sep1.Name = "m_sep1"
            Me.m_sep1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_sep1.Size = New System.Drawing.Size(6, 23)
            Me.m_sep1.Visible = False
            '
            'm_tsbnShowLegends
            '
            Me.m_tsbnShowLegends.Checked = True
            Me.m_tsbnShowLegends.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsbnShowLegends.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnShowLegends.Image = Global.ScientificInterfaceShared.My.Resources.Resources.LegendHS
            Me.m_tsbnShowLegends.Margin = New System.Windows.Forms.Padding(2, 1, 2, 2)
            Me.m_tsbnShowLegends.Name = "m_tsbnShowLegends"
            Me.m_tsbnShowLegends.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnShowLegends.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnShowLegends.Text = "Show legends"
            '
            'm_tsbnShowAxisLabels
            '
            Me.m_tsbnShowAxisLabels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnShowAxisLabels.Image = Global.ScientificInterfaceShared.My.Resources.Resources.tag
            Me.m_tsbnShowAxisLabels.Margin = New System.Windows.Forms.Padding(2, 1, 2, 2)
            Me.m_tsbnShowAxisLabels.Name = "m_tsbnShowAxisLabels"
            Me.m_tsbnShowAxisLabels.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnShowAxisLabels.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnShowAxisLabels.Text = "Show axis labels"
            '
            'm_sep2
            '
            Me.m_sep2.AutoSize = False
            Me.m_sep2.Name = "m_sep2"
            Me.m_sep2.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_sep2.Size = New System.Drawing.Size(6, 23)
            Me.m_sep2.Visible = False
            '
            'm_tsbnExport
            '
            Me.m_tsbnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnExport.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ExportXMLHS
            Me.m_tsbnExport.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnExport.Name = "m_tsbnExport"
            Me.m_tsbnExport.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnExport.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnExport.Text = "Export to CSV..."
            '
            'ucHoverMenu
            '
            Me.AutoSize = True
            Me.BackColor = System.Drawing.SystemColors.ButtonFace
            Me.Controls.Add(Me.m_ts)
            Me.Name = "ucHoverMenu"
            Me.Size = New System.Drawing.Size(800, 23)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbnZoomIn As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomOut As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomReset As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnExport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnShowLegends As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnShowAxisLabels As System.Windows.Forms.ToolStripButton
        Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_sep2 As System.Windows.Forms.ToolStripSeparator
    End Class

End Namespace ' Controls