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

Imports ScientificInterfaceShared

Namespace Other

    Partial Class ucOptionsGraphsCharts
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsGraphsCharts))
            Me.m_hdr1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblThumbnailSize = New System.Windows.Forms.Label()
            Me.m_nudThumbnailSize = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_rbLegendAlways = New System.Windows.Forms.RadioButton()
            Me.m_rbLegendSelective = New System.Windows.Forms.RadioButton()
            Me.m_lblThumbnailUnit = New System.Windows.Forms.Label()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_nudChartSymbols = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblSizeOfSymbols = New System.Windows.Forms.Label()
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudChartSymbols, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_hdr1
            '
            Me.m_hdr1.CanCollapseParent = False
            Me.m_hdr1.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdr1, "m_hdr1")
            Me.m_hdr1.IsCollapsed = False
            Me.m_hdr1.Name = "m_hdr1"
            '
            'm_lblThumbnailSize
            '
            resources.ApplyResources(Me.m_lblThumbnailSize, "m_lblThumbnailSize")
            Me.m_lblThumbnailSize.Name = "m_lblThumbnailSize"
            '
            'm_nudThumbnailSize
            '
            Me.m_nudThumbnailSize.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            resources.ApplyResources(Me.m_nudThumbnailSize, "m_nudThumbnailSize")
            Me.m_nudThumbnailSize.Maximum = New Decimal(New Integer() {240, 0, 0, 0})
            Me.m_nudThumbnailSize.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
            Me.m_nudThumbnailSize.Name = "m_nudThumbnailSize"
            Me.m_nudThumbnailSize.Value = New Decimal(New Integer() {32, 0, 0, 0})
            '
            'm_rbLegendAlways
            '
            resources.ApplyResources(Me.m_rbLegendAlways, "m_rbLegendAlways")
            Me.m_rbLegendAlways.Name = "m_rbLegendAlways"
            Me.m_rbLegendAlways.TabStop = True
            Me.m_rbLegendAlways.UseVisualStyleBackColor = True
            '
            'm_rbLegendSelective
            '
            resources.ApplyResources(Me.m_rbLegendSelective, "m_rbLegendSelective")
            Me.m_rbLegendSelective.Name = "m_rbLegendSelective"
            Me.m_rbLegendSelective.TabStop = True
            Me.m_rbLegendSelective.UseVisualStyleBackColor = True
            '
            'm_lblThumbnailUnit
            '
            resources.ApplyResources(Me.m_lblThumbnailUnit, "m_lblThumbnailUnit")
            Me.m_lblThumbnailUnit.Name = "m_lblThumbnailUnit"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'm_nudChartSymbols
            '
            Me.m_nudChartSymbols.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            resources.ApplyResources(Me.m_nudChartSymbols, "m_nudChartSymbols")
            Me.m_nudChartSymbols.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
            Me.m_nudChartSymbols.Minimum = New Decimal(New Integer() {4, 0, 0, 0})
            Me.m_nudChartSymbols.Name = "m_nudChartSymbols"
            Me.m_nudChartSymbols.Value = New Decimal(New Integer() {5, 0, 0, 0})
            '
            'm_lblSizeOfSymbols
            '
            resources.ApplyResources(Me.m_lblSizeOfSymbols, "m_lblSizeOfSymbols")
            Me.m_lblSizeOfSymbols.Name = "m_lblSizeOfSymbols"
            '
            'ucOptionsGraphsCharts
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_nudChartSymbols)
            Me.Controls.Add(Me.m_lblSizeOfSymbols)
            Me.Controls.Add(Me.m_lblThumbnailUnit)
            Me.Controls.Add(Me.m_rbLegendAlways)
            Me.Controls.Add(Me.m_nudThumbnailSize)
            Me.Controls.Add(Me.m_lblThumbnailSize)
            Me.Controls.Add(Me.m_rbLegendSelective)
            Me.Controls.Add(Me.m_hdr1)
            Me.Name = "ucOptionsGraphsCharts"
            CType(Me.m_nudThumbnailSize, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudChartSymbols, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_hdr1 As cEwEHeaderLabel
        Private WithEvents m_lblThumbnailSize As System.Windows.Forms.Label
        Private WithEvents m_rbLegendAlways As System.Windows.Forms.RadioButton
        Private WithEvents m_rbLegendSelective As System.Windows.Forms.RadioButton
        Private WithEvents m_lblThumbnailUnit As System.Windows.Forms.Label
        Private WithEvents m_nudThumbnailSize As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents Label1 As Label
        Private WithEvents m_nudChartSymbols As cEwENumericUpDown
        Private WithEvents m_lblSizeOfSymbols As Label
    End Class
End Namespace

