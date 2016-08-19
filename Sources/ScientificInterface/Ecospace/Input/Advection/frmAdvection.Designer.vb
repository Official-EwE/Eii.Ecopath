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

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls.Map

Namespace Ecospace.Advection

    Partial Class frmAdvection
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
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_scMaps = New System.Windows.Forms.SplitContainer()
            Me.m_scOutputMaps = New System.Windows.Forms.SplitContainer()
            Me.m_tlpControls = New System.Windows.Forms.TableLayoutPanel()
            Me.m_tlpComputeControls = New System.Windows.Forms.TableLayoutPanel()
            Me.m_btnStart = New System.Windows.Forms.Button()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_lblWIndEditorPlaceholder = New System.Windows.Forms.Label()
            Me.m_hdrCompute = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrParams = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpParameters = New System.Windows.Forms.TableLayoutPanel()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_txtUpwelling = New System.Windows.Forms.TextBox()
            Me.m_ucZoomToolbar = New ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar()
            Me.m_ucWind = New ScientificInterface.Ecospace.Advection.ucWind()
            Me.m_ucMap = New ScientificInterface.Ecospace.Advection.ucMap()
            Me.m_ucUpwelling = New ScientificInterface.Ecospace.Advection.ucUpwelling()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            CType(Me.m_scMaps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMaps.Panel1.SuspendLayout()
            Me.m_scMaps.Panel2.SuspendLayout()
            Me.m_scMaps.SuspendLayout()
            CType(Me.m_scOutputMaps, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scOutputMaps.Panel1.SuspendLayout()
            Me.m_scOutputMaps.Panel2.SuspendLayout()
            Me.m_scOutputMaps.SuspendLayout()
            Me.m_tlpControls.SuspendLayout()
            Me.m_tlpComputeControls.SuspendLayout()
            Me.m_tlpParameters.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_scMain.Location = New System.Drawing.Point(3, 31)
            Me.m_scMain.Margin = New System.Windows.Forms.Padding(0)
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_scMaps)
            Me.m_scMain.Panel1MinSize = 190
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpControls)
            Me.m_scMain.Size = New System.Drawing.Size(550, 374)
            Me.m_scMain.SplitterDistance = 371
            Me.m_scMain.TabIndex = 0
            '
            'm_scMaps
            '
            Me.m_scMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_scMaps.Name = "m_scMaps"
            Me.m_scMaps.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scMaps.Panel1
            '
            Me.m_scMaps.Panel1.Controls.Add(Me.m_ucWind)
            '
            'm_scMaps.Panel2
            '
            Me.m_scMaps.Panel2.Controls.Add(Me.m_scOutputMaps)
            Me.m_scMaps.Size = New System.Drawing.Size(371, 374)
            Me.m_scMaps.SplitterDistance = 214
            Me.m_scMaps.TabIndex = 0
            '
            'm_scOutputMaps
            '
            Me.m_scOutputMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scOutputMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_scOutputMaps.Name = "m_scOutputMaps"
            '
            'm_scOutputMaps.Panel1
            '
            Me.m_scOutputMaps.Panel1.Controls.Add(Me.m_ucMap)
            '
            'm_scOutputMaps.Panel2
            '
            Me.m_scOutputMaps.Panel2.Controls.Add(Me.m_ucUpwelling)
            Me.m_scOutputMaps.Size = New System.Drawing.Size(371, 156)
            Me.m_scOutputMaps.SplitterDistance = 180
            Me.m_scOutputMaps.TabIndex = 0
            '
            'm_tlpControls
            '
            Me.m_tlpControls.AutoSize = True
            Me.m_tlpControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_tlpControls.ColumnCount = 1
            Me.m_tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpControls.Controls.Add(Me.m_tlpComputeControls, 0, 4)
            Me.m_tlpControls.Controls.Add(Me.m_lblWIndEditorPlaceholder, 0, 0)
            Me.m_tlpControls.Controls.Add(Me.m_hdrCompute, 0, 3)
            Me.m_tlpControls.Controls.Add(Me.m_hdrParams, 0, 1)
            Me.m_tlpControls.Controls.Add(Me.m_tlpParameters, 0, 2)
            Me.m_tlpControls.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpControls.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpControls.Name = "m_tlpControls"
            Me.m_tlpControls.RowCount = 6
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpControls.Size = New System.Drawing.Size(175, 374)
            Me.m_tlpControls.TabIndex = 0
            '
            'm_tlpComputeControls
            '
            Me.m_tlpComputeControls.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpComputeControls.ColumnCount = 2
            Me.m_tlpComputeControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.Controls.Add(Me.m_btnStart, 0, 0)
            Me.m_tlpComputeControls.Controls.Add(Me.m_btnStop, 1, 0)
            Me.m_tlpComputeControls.Location = New System.Drawing.Point(3, 91)
            Me.m_tlpComputeControls.Name = "m_tlpComputeControls"
            Me.m_tlpComputeControls.RowCount = 1
            Me.m_tlpComputeControls.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpComputeControls.Size = New System.Drawing.Size(169, 27)
            Me.m_tlpComputeControls.TabIndex = 10
            '
            'm_btnStart
            '
            Me.m_btnStart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStart.Location = New System.Drawing.Point(0, 0)
            Me.m_btnStart.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.m_btnStart.Name = "m_btnStart"
            Me.m_btnStart.Size = New System.Drawing.Size(81, 23)
            Me.m_btnStart.TabIndex = 0
            Me.m_btnStart.Text = "&Compute"
            Me.m_btnStart.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.Location = New System.Drawing.Point(87, 0)
            Me.m_btnStop.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(82, 23)
            Me.m_btnStop.TabIndex = 1
            Me.m_btnStop.Text = "&Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_lblWIndEditorPlaceholder
            '
            Me.m_lblWIndEditorPlaceholder.AutoSize = True
            Me.m_lblWIndEditorPlaceholder.Location = New System.Drawing.Point(3, 0)
            Me.m_lblWIndEditorPlaceholder.Name = "m_lblWIndEditorPlaceholder"
            Me.m_lblWIndEditorPlaceholder.Size = New System.Drawing.Size(148, 13)
            Me.m_lblWIndEditorPlaceholder.TabIndex = 0
            Me.m_lblWIndEditorPlaceholder.Text = "<wind edit panel placeholder>"
            '
            'm_hdrCompute
            '
            Me.m_hdrCompute.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrCompute.CanCollapseParent = False
            Me.m_hdrCompute.CollapsedParentHeight = 0
            Me.m_hdrCompute.IsCollapsed = False
            Me.m_hdrCompute.Location = New System.Drawing.Point(0, 70)
            Me.m_hdrCompute.Margin = New System.Windows.Forms.Padding(0)
            Me.m_hdrCompute.Name = "m_hdrCompute"
            Me.m_hdrCompute.Size = New System.Drawing.Size(175, 18)
            Me.m_hdrCompute.TabIndex = 1
            Me.m_hdrCompute.Text = "Compute advection velocities"
            Me.m_hdrCompute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_hdrParams
            '
            Me.m_hdrParams.CanCollapseParent = False
            Me.m_hdrParams.CollapsedParentHeight = 0
            Me.m_hdrParams.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_hdrParams.IsCollapsed = False
            Me.m_hdrParams.Location = New System.Drawing.Point(3, 13)
            Me.m_hdrParams.Name = "m_hdrParams"
            Me.m_hdrParams.Size = New System.Drawing.Size(169, 20)
            Me.m_hdrParams.TabIndex = 11
            Me.m_hdrParams.Text = "Model parameters"
            Me.m_hdrParams.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tlpParameters
            '
            Me.m_tlpParameters.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpParameters.ColumnCount = 2
            Me.m_tlpParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpParameters.Controls.Add(Me.Label1, 0, 0)
            Me.m_tlpParameters.Controls.Add(Me.m_txtUpwelling, 1, 0)
            Me.m_tlpParameters.Location = New System.Drawing.Point(3, 36)
            Me.m_tlpParameters.Name = "m_tlpParameters"
            Me.m_tlpParameters.RowCount = 1
            Me.m_tlpParameters.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpParameters.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44.0!))
            Me.m_tlpParameters.Size = New System.Drawing.Size(169, 31)
            Me.m_tlpParameters.TabIndex = 12
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 0)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(78, 26)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Upwelling threshold"
            '
            'm_txtUpwelling
            '
            Me.m_txtUpwelling.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_txtUpwelling.Location = New System.Drawing.Point(87, 0)
            Me.m_txtUpwelling.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
            Me.m_txtUpwelling.Name = "m_txtUpwelling"
            Me.m_txtUpwelling.Size = New System.Drawing.Size(79, 20)
            Me.m_txtUpwelling.TabIndex = 1
            '
            'm_ucZoomToolbar
            '
            Me.m_ucZoomToolbar.AutoSize = True
            Me.m_ucZoomToolbar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucZoomToolbar.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_ucZoomToolbar.Location = New System.Drawing.Point(3, 3)
            Me.m_ucZoomToolbar.MinimumSize = New System.Drawing.Size(100, 25)
            Me.m_ucZoomToolbar.Name = "m_ucZoomToolbar"
            Me.m_ucZoomToolbar.PositionMode = ScientificInterfaceShared.Controls.Map.ucMapZoom.ePositionModeTypes.Center
            Me.m_ucZoomToolbar.Size = New System.Drawing.Size(550, 25)
            Me.m_ucZoomToolbar.TabIndex = 0
            Me.m_ucZoomToolbar.UIContext = Nothing
            '
            'm_ucWind
            '
            Me.m_ucWind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucWind.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucWind.Location = New System.Drawing.Point(0, 0)
            Me.m_ucWind.Margin = New System.Windows.Forms.Padding(3, 0, 0, 3)
            Me.m_ucWind.Name = "m_ucWind"
            Me.m_ucWind.Size = New System.Drawing.Size(371, 214)
            Me.m_ucWind.TabIndex = 0
            Me.m_ucWind.UIContext = Nothing
            '
            'm_ucMap
            '
            Me.m_ucMap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMap.Location = New System.Drawing.Point(0, 0)
            Me.m_ucMap.Margin = New System.Windows.Forms.Padding(0, 0, 3, 3)
            Me.m_ucMap.Name = "m_ucMap"
            Me.m_ucMap.Size = New System.Drawing.Size(180, 156)
            Me.m_ucMap.TabIndex = 0
            Me.m_ucMap.UIContext = Nothing
            '
            'm_ucUpwelling
            '
            Me.m_ucUpwelling.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_ucUpwelling.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucUpwelling.Location = New System.Drawing.Point(0, 0)
            Me.m_ucUpwelling.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
            Me.m_ucUpwelling.Name = "m_ucUpwelling"
            Me.m_ucUpwelling.Size = New System.Drawing.Size(187, 156)
            Me.m_ucUpwelling.TabIndex = 0
            Me.m_ucUpwelling.UIContext = Nothing
            '
            'frmAdvection
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.ClientSize = New System.Drawing.Size(556, 411)
            Me.Controls.Add(Me.m_ucZoomToolbar)
            Me.Controls.Add(Me.m_scMain)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmAdvection"
            Me.Padding = New System.Windows.Forms.Padding(3)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.TabText = ""
            Me.Text = "Advection"
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.Panel2.PerformLayout()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_scMaps.Panel1.ResumeLayout(False)
            Me.m_scMaps.Panel2.ResumeLayout(False)
            CType(Me.m_scMaps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMaps.ResumeLayout(False)
            Me.m_scOutputMaps.Panel1.ResumeLayout(False)
            Me.m_scOutputMaps.Panel2.ResumeLayout(False)
            CType(Me.m_scOutputMaps, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scOutputMaps.ResumeLayout(False)
            Me.m_tlpControls.ResumeLayout(False)
            Me.m_tlpControls.PerformLayout()
            Me.m_tlpComputeControls.ResumeLayout(False)
            Me.m_tlpParameters.ResumeLayout(False)
            Me.m_tlpParameters.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        'Private WithEvents m_ucMLD As ScientificInterface.Ecospace.Advection.ucMLD
        Private WithEvents m_ucUpwelling As ScientificInterface.Ecospace.Advection.ucUpwelling
        Private WithEvents m_ucWind As ScientificInterface.Ecospace.Advection.ucWind
        Private WithEvents m_ucMap As ScientificInterface.Ecospace.Advection.ucMap
        Private WithEvents m_ucZoomToolbar As ucMapZoomToolbar
        Friend WithEvents m_scMaps As SplitContainer
        Friend WithEvents m_scOutputMaps As SplitContainer
        Private WithEvents m_tlpControls As TableLayoutPanel
        Private WithEvents m_tlpComputeControls As TableLayoutPanel
        Private WithEvents m_btnStart As Button
        Private WithEvents m_btnStop As Button
        Private WithEvents m_lblWIndEditorPlaceholder As Label
        Private WithEvents m_hdrCompute As cEwEHeaderLabel
        Friend WithEvents m_hdrParams As cEwEHeaderLabel
        Friend WithEvents m_tlpParameters As TableLayoutPanel
        Friend WithEvents Label1 As Label
        Friend WithEvents m_txtUpwelling As TextBox
    End Class

End Namespace
