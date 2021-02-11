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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Namespace Ecospace.Basemap


    Partial Class frmEcospaceComputedCapacity
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
        Protected Overrides Sub Dispose(disposing As Boolean)
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceComputedCapacity))
            Me.ToolStrip1 = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnRecompute = New System.Windows.Forms.ToolStripButton()
            Me.m_scMap = New System.Windows.Forms.SplitContainer()
            Me.m_zoomContainer = New ScientificInterfaceShared.Controls.Map.ucMapZoom()
            Me.m_tlpLayers = New System.Windows.Forms.TableLayoutPanel()
            Me.m_ucLayers = New ScientificInterfaceShared.Controls.Map.ucLayersControl()
            Me.m_plEditor = New System.Windows.Forms.Panel()
            Me.ToolStrip1.SuspendLayout()
            CType(Me.m_scMap, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMap.Panel1.SuspendLayout()
            Me.m_scMap.Panel2.SuspendLayout()
            Me.m_scMap.SuspendLayout()
            Me.m_tlpLayers.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStrip1
            '
            Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnRecompute})
            Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
            Me.ToolStrip1.TabIndex = 0
            Me.ToolStrip1.Text = "ToolStrip1"
            '
            'm_tsbnRecompute
            '
            Me.m_tsbnRecompute.Image = CType(resources.GetObject("m_tsbnRecompute.Image"), System.Drawing.Image)
            Me.m_tsbnRecompute.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnRecompute.Name = "m_tsbnRecompute"
            Me.m_tsbnRecompute.Size = New System.Drawing.Size(124, 22)
            Me.m_tsbnRecompute.Text = "&Compute capacity"
            '
            'm_scMap
            '
            Me.m_scMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMap.Location = New System.Drawing.Point(0, 25)
            Me.m_scMap.Name = "m_scMap"
            '
            'm_scMap.Panel1
            '
            Me.m_scMap.Panel1.Controls.Add(Me.m_zoomContainer)
            '
            'm_scMap.Panel2
            '
            Me.m_scMap.Panel2.Controls.Add(Me.m_tlpLayers)
            Me.m_scMap.Size = New System.Drawing.Size(800, 425)
            Me.m_scMap.SplitterDistance = 604
            Me.m_scMap.TabIndex = 3
            '
            'm_zoomContainer
            '
            Me.m_zoomContainer.AutoScroll = True
            Me.m_zoomContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_zoomContainer.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_zoomContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_zoomContainer.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_zoomContainer.Location = New System.Drawing.Point(0, 0)
            Me.m_zoomContainer.Margin = New System.Windows.Forms.Padding(0)
            Me.m_zoomContainer.Name = "m_zoomContainer"
            Me.m_zoomContainer.Size = New System.Drawing.Size(604, 425)
            Me.m_zoomContainer.TabIndex = 4
            Me.m_zoomContainer.UIContext = Nothing
            '
            'm_tlpLayers
            '
            Me.m_tlpLayers.ColumnCount = 1
            Me.m_tlpLayers.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpLayers.Controls.Add(Me.m_ucLayers, 0, 0)
            Me.m_tlpLayers.Controls.Add(Me.m_plEditor, 0, 1)
            Me.m_tlpLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpLayers.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpLayers.Name = "m_tlpLayers"
            Me.m_tlpLayers.RowCount = 2
            Me.m_tlpLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpLayers.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlpLayers.Size = New System.Drawing.Size(192, 425)
            Me.m_tlpLayers.TabIndex = 0
            '
            'm_ucLayers
            '
            Me.m_ucLayers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucLayers.BackColor = System.Drawing.SystemColors.Control
            Me.m_ucLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucLayers.Location = New System.Drawing.Point(0, 0)
            Me.m_ucLayers.Margin = New System.Windows.Forms.Padding(0)
            Me.m_ucLayers.Name = "m_ucLayers"
            Me.m_ucLayers.Size = New System.Drawing.Size(192, 402)
            Me.m_ucLayers.TabIndex = 13
            Me.m_ucLayers.UIContext = Nothing
            '
            'm_plEditor
            '
            Me.m_plEditor.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plEditor.Location = New System.Drawing.Point(0, 402)
            Me.m_plEditor.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plEditor.Name = "m_plEditor"
            Me.m_plEditor.Size = New System.Drawing.Size(192, 23)
            Me.m_plEditor.TabIndex = 12
            '
            'frmEcospaceComputedCapacity
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(800, 450)
            Me.Controls.Add(Me.m_scMap)
            Me.Controls.Add(Me.ToolStrip1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "frmEcospaceComputedCapacity"
            Me.TabText = ""
            Me.Text = "Computed foraging capacity"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.m_scMap.Panel1.ResumeLayout(False)
            Me.m_scMap.Panel2.ResumeLayout(False)
            CType(Me.m_scMap, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMap.ResumeLayout(False)
            Me.m_tlpLayers.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents ToolStrip1 As cEwEToolstrip
        Private WithEvents m_tsbnRecompute As ToolStripButton
        Private WithEvents m_scMap As SplitContainer
        Private WithEvents m_zoomContainer As Map.ucMapZoom
        Private WithEvents m_tlpLayers As TableLayoutPanel
        Private WithEvents m_ucLayers As Map.ucLayersControl
        Private WithEvents m_plEditor As Panel
    End Class

End Namespace
