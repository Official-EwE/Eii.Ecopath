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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.m_scMap = New System.Windows.Forms.SplitContainer()
            Me.m_zoomContainer = New ScientificInterfaceShared.Controls.Map.ucMapZoom()
            Me.m_tlpLayers = New System.Windows.Forms.TableLayoutPanel()
            Me.m_ucLayers = New ScientificInterfaceShared.Controls.Map.ucLayersControl()
            Me.m_plEditor = New System.Windows.Forms.Panel()
            CType(Me.m_scMap, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMap.Panel1.SuspendLayout()
            Me.m_scMap.Panel2.SuspendLayout()
            Me.m_scMap.SuspendLayout()
            Me.m_tlpLayers.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMap
            '
            Me.m_scMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMap.Location = New System.Drawing.Point(0, 0)
            Me.m_scMap.Name = "m_scMap"
            '
            'm_scMap.Panel1
            '
            Me.m_scMap.Panel1.Controls.Add(Me.m_zoomContainer)
            '
            'm_scMap.Panel2
            '
            Me.m_scMap.Panel2.Controls.Add(Me.m_tlpLayers)
            Me.m_scMap.Size = New System.Drawing.Size(800, 450)
            Me.m_scMap.SplitterDistance = 604
            Me.m_scMap.TabIndex = 2
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
            Me.m_zoomContainer.Size = New System.Drawing.Size(604, 450)
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
            Me.m_tlpLayers.Size = New System.Drawing.Size(192, 450)
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
            Me.m_ucLayers.Size = New System.Drawing.Size(192, 427)
            Me.m_ucLayers.TabIndex = 13
            Me.m_ucLayers.UIContext = Nothing
            '
            'm_plEditor
            '
            Me.m_plEditor.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plEditor.Location = New System.Drawing.Point(0, 427)
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
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmEcospaceComputedCapacity"
            Me.TabText = ""
            Me.Text = "Computed foraging capacity"
            Me.m_scMap.Panel1.ResumeLayout(False)
            Me.m_scMap.Panel2.ResumeLayout(False)
            CType(Me.m_scMap, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMap.ResumeLayout(False)
            Me.m_tlpLayers.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpLayers As TableLayoutPanel
        Private WithEvents m_plEditor As Panel
        Private WithEvents m_ucLayers As Map.ucLayersControl
        Private WithEvents m_zoomContainer As Map.ucMapZoom
        Private WithEvents m_scMap As SplitContainer
    End Class

End Namespace
