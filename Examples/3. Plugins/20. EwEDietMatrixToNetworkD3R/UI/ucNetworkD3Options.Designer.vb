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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucNetworkD3Options
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucNetworkD3Options))
        Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblGraph = New System.Windows.Forms.Label()
        Me.m_cbUseSymbolicaNames = New System.Windows.Forms.CheckBox()
        Me.m_cbUseClipboard = New System.Windows.Forms.CheckBox()
        Me.m_cmbNetworkType = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'm_hdr
        '
        Me.m_hdr.CanCollapseParent = False
        Me.m_hdr.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdr, "m_hdr")
        Me.m_hdr.IsCollapsed = False
        Me.m_hdr.Name = "m_hdr"
        '
        'm_lblGraph
        '
        resources.ApplyResources(Me.m_lblGraph, "m_lblGraph")
        Me.m_lblGraph.Name = "m_lblGraph"
        '
        'm_cbUseSymbolicaNames
        '
        resources.ApplyResources(Me.m_cbUseSymbolicaNames, "m_cbUseSymbolicaNames")
        Me.m_cbUseSymbolicaNames.Name = "m_cbUseSymbolicaNames"
        Me.m_cbUseSymbolicaNames.UseVisualStyleBackColor = True
        '
        'm_cbUseClipboard
        '
        resources.ApplyResources(Me.m_cbUseClipboard, "m_cbUseClipboard")
        Me.m_cbUseClipboard.Name = "m_cbUseClipboard"
        Me.m_cbUseClipboard.UseVisualStyleBackColor = True
        '
        'm_cmbNetworkType
        '
        Me.m_cmbNetworkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbNetworkType.FormattingEnabled = True
        resources.ApplyResources(Me.m_cmbNetworkType, "m_cmbNetworkType")
        Me.m_cmbNetworkType.Name = "m_cmbNetworkType"
        '
        'ucNetworkD3Options
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Controls.Add(Me.m_cmbNetworkType)
        Me.Controls.Add(Me.m_cbUseClipboard)
        Me.Controls.Add(Me.m_cbUseSymbolicaNames)
        Me.Controls.Add(Me.m_lblGraph)
        Me.Controls.Add(Me.m_hdr)
        Me.Name = "ucNetworkD3Options"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_hdr As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblGraph As Windows.Forms.Label
    Friend WithEvents m_cbUseSymbolicaNames As Windows.Forms.CheckBox
    Private WithEvents m_cbUseClipboard As Windows.Forms.CheckBox
    Friend WithEvents m_cmbNetworkType As Windows.Forms.ComboBox
End Class
