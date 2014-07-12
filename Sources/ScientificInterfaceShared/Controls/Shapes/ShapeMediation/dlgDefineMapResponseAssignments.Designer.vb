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

Imports ScientificInterfaceShared.Controls

Partial Class dlgDefineMapResponseAssignments
    Inherits System.Windows.Forms.Form

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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDefineMapResponseAssignments))
        Me.m_btnOk = New System.Windows.Forms.Button()
        Me.m_lblMaps = New System.Windows.Forms.Label()
        Me.m_tbxXMax = New System.Windows.Forms.TextBox()
        Me.m_lblXMax = New System.Windows.Forms.Label()
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_tbxXMin = New System.Windows.Forms.TextBox()
        Me.m_lblXMin = New System.Windows.Forms.Label()
        Me.m_btnDefaultMinMax = New System.Windows.Forms.Button()
        Me.m_tvMaps = New System.Windows.Forms.TreeView()
        Me.m_lblGroups = New System.Windows.Forms.Label()
        Me.m_btnRemove = New System.Windows.Forms.Button()
        Me.m_btnAdd = New System.Windows.Forms.Button()
        Me.m_txMean = New System.Windows.Forms.TextBox()
        Me.m_txSD = New System.Windows.Forms.TextBox()
        Me.m_lbMean = New System.Windows.Forms.Label()
        Me.m_lbSD = New System.Windows.Forms.Label()
        Me.m_hdrReponse = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lbxGroups = New ScientificInterfaceShared.Controls.cGroupListBox()
        Me.m_hdrConfig = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btChangeShape = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'm_btnOk
        '
        resources.ApplyResources(Me.m_btnOk, "m_btnOk")
        Me.m_btnOk.Name = "m_btnOk"
        '
        'm_lblMaps
        '
        resources.ApplyResources(Me.m_lblMaps, "m_lblMaps")
        Me.m_lblMaps.Name = "m_lblMaps"
        '
        'm_tbxXMax
        '
        resources.ApplyResources(Me.m_tbxXMax, "m_tbxXMax")
        Me.m_tbxXMax.Name = "m_tbxXMax"
        '
        'm_lblXMax
        '
        resources.ApplyResources(Me.m_lblXMax, "m_lblXMax")
        Me.m_lblXMax.Name = "m_lblXMax"
        '
        'm_graph
        '
        resources.ApplyResources(Me.m_graph, "m_graph")
        Me.m_graph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0.0R
        Me.m_graph.ScrollMaxX = 0.0R
        Me.m_graph.ScrollMaxY = 0.0R
        Me.m_graph.ScrollMaxY2 = 0.0R
        Me.m_graph.ScrollMinX = 0.0R
        Me.m_graph.ScrollMinY = 0.0R
        Me.m_graph.ScrollMinY2 = 0.0R
        '
        'm_tbxXMin
        '
        resources.ApplyResources(Me.m_tbxXMin, "m_tbxXMin")
        Me.m_tbxXMin.Name = "m_tbxXMin"
        '
        'm_lblXMin
        '
        resources.ApplyResources(Me.m_lblXMin, "m_lblXMin")
        Me.m_lblXMin.Name = "m_lblXMin"
        '
        'm_btnDefaultMinMax
        '
        resources.ApplyResources(Me.m_btnDefaultMinMax, "m_btnDefaultMinMax")
        Me.m_btnDefaultMinMax.Name = "m_btnDefaultMinMax"
        Me.m_btnDefaultMinMax.UseVisualStyleBackColor = True
        '
        'm_tvMaps
        '
        resources.ApplyResources(Me.m_tvMaps, "m_tvMaps")
        Me.m_tvMaps.FullRowSelect = True
        Me.m_tvMaps.HideSelection = False
        Me.m_tvMaps.Name = "m_tvMaps"
        Me.m_tvMaps.ShowRootLines = False
        '
        'm_lblGroups
        '
        resources.ApplyResources(Me.m_lblGroups, "m_lblGroups")
        Me.m_lblGroups.Name = "m_lblGroups"
        '
        'm_btnRemove
        '
        resources.ApplyResources(Me.m_btnRemove, "m_btnRemove")
        Me.m_btnRemove.Image = Global.ScientificInterfaceShared.My.Resources.Resources.Back
        Me.m_btnRemove.Name = "m_btnRemove"
        Me.m_btnRemove.UseVisualStyleBackColor = True
        '
        'm_btnAdd
        '
        resources.ApplyResources(Me.m_btnAdd, "m_btnAdd")
        Me.m_btnAdd.Image = Global.ScientificInterfaceShared.My.Resources.Resources.forward
        Me.m_btnAdd.Name = "m_btnAdd"
        Me.m_btnAdd.UseVisualStyleBackColor = True
        '
        'm_txMean
        '
        resources.ApplyResources(Me.m_txMean, "m_txMean")
        Me.m_txMean.Name = "m_txMean"
        '
        'm_txSD
        '
        resources.ApplyResources(Me.m_txSD, "m_txSD")
        Me.m_txSD.Name = "m_txSD"
        '
        'm_lbMean
        '
        resources.ApplyResources(Me.m_lbMean, "m_lbMean")
        Me.m_lbMean.Name = "m_lbMean"
        '
        'm_lbSD
        '
        resources.ApplyResources(Me.m_lbSD, "m_lbSD")
        Me.m_lbSD.Name = "m_lbSD"
        '
        'm_hdrReponse
        '
        Me.m_hdrReponse.CanCollapseParent = False
        Me.m_hdrReponse.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrReponse, "m_hdrReponse")
        Me.m_hdrReponse.IsCollapsed = False
        Me.m_hdrReponse.Name = "m_hdrReponse"
        '
        'm_lbxGroups
        '
        Me.m_lbxGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
        Me.m_lbxGroups.AllGroupsItemText = "(All)"
        resources.ApplyResources(Me.m_lbxGroups, "m_lbxGroups")
        Me.m_lbxGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.m_lbxGroups.FormattingEnabled = True
        Me.m_lbxGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.Manual
        Me.m_lbxGroups.IsAllGroupsItemSelected = False
        Me.m_lbxGroups.Name = "m_lbxGroups"
        Me.m_lbxGroups.SelectedGroup = Nothing
        Me.m_lbxGroups.SelectedGroupIndex = -1
        Me.m_lbxGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.m_lbxGroups.ShowAllGroupsItem = False
        Me.m_lbxGroups.SortThreshold = -9999.0!
        '
        'm_hdrConfig
        '
        Me.m_hdrConfig.CanCollapseParent = False
        Me.m_hdrConfig.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrConfig, "m_hdrConfig")
        Me.m_hdrConfig.IsCollapsed = False
        Me.m_hdrConfig.Name = "m_hdrConfig"
        '
        'm_btChangeShape
        '
        resources.ApplyResources(Me.m_btChangeShape, "m_btChangeShape")
        Me.m_btChangeShape.Name = "m_btChangeShape"
        Me.m_btChangeShape.UseVisualStyleBackColor = True
        '
        'dlgDefineMapResponseAssignments
        '
        Me.AcceptButton = Me.m_btnOk
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_btChangeShape)
        Me.Controls.Add(Me.m_lbSD)
        Me.Controls.Add(Me.m_lbMean)
        Me.Controls.Add(Me.m_txSD)
        Me.Controls.Add(Me.m_txMean)
        Me.Controls.Add(Me.m_lblXMin)
        Me.Controls.Add(Me.m_tbxXMax)
        Me.Controls.Add(Me.m_hdrReponse)
        Me.Controls.Add(Me.m_lblXMax)
        Me.Controls.Add(Me.m_lbxGroups)
        Me.Controls.Add(Me.m_graph)
        Me.Controls.Add(Me.m_hdrConfig)
        Me.Controls.Add(Me.m_btnDefaultMinMax)
        Me.Controls.Add(Me.m_btnRemove)
        Me.Controls.Add(Me.m_tbxXMin)
        Me.Controls.Add(Me.m_lblMaps)
        Me.Controls.Add(Me.m_btnAdd)
        Me.Controls.Add(Me.m_btnOk)
        Me.Controls.Add(Me.m_tvMaps)
        Me.Controls.Add(Me.m_lblGroups)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgDefineMapResponseAssignments"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_btnOk As System.Windows.Forms.Button
    Private WithEvents m_lblMaps As System.Windows.Forms.Label
    Private WithEvents m_lblGroups As System.Windows.Forms.Label
    Private WithEvents m_btnRemove As System.Windows.Forms.Button
    Private WithEvents m_btnAdd As System.Windows.Forms.Button
    Private WithEvents m_tvMaps As System.Windows.Forms.TreeView
    Private WithEvents m_lbxGroups As cGroupListBox
    Private WithEvents m_btnDefaultMinMax As System.Windows.Forms.Button
    Private WithEvents m_lblXMin As System.Windows.Forms.Label
    Private WithEvents m_lblXMax As System.Windows.Forms.Label
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tbxXMin As System.Windows.Forms.TextBox
    Private WithEvents m_tbxXMax As System.Windows.Forms.TextBox
    Private WithEvents m_hdrReponse As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrConfig As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_txMean As System.Windows.Forms.TextBox
    Friend WithEvents m_txSD As System.Windows.Forms.TextBox
    Friend WithEvents m_lbMean As System.Windows.Forms.Label
    Friend WithEvents m_lbSD As System.Windows.Forms.Label
    Friend WithEvents m_btChangeShape As System.Windows.Forms.Button

End Class
