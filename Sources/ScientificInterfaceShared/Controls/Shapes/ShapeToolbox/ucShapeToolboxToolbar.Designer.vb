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

Namespace Controls


    Partial Class ucShapeToolboxToolbar
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucShapeToolboxToolbar))
            Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbImport = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbExport = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbLoad = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbWeight = New System.Windows.Forms.ToolStripButton()
            Me.m_ts1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbAdd = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbRemove = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbDuplicate = New System.Windows.Forms.ToolStripButton()
            Me.m_ts2 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbSetTo0 = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbSetToBaseline = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbSetToValue = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbResetAll = New System.Windows.Forms.ToolStripButton()
            Me.m_tscmbFilter = New System.Windows.Forms.ToolStripComboBox()
            Me.m_ts3 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnShowExtraData = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnDiscardExtraData = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnFilterCase = New System.Windows.Forms.ToolStripButton()
            Me.m_tstbxFilterName = New System.Windows.Forms.ToolStripTextBox()
            Me.m_tslFilter = New System.Windows.Forms.ToolStripLabel()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbImport, Me.m_tsbExport, Me.m_tsbLoad, Me.m_tsbWeight, Me.m_ts1, Me.m_tsbAdd, Me.m_tsbRemove, Me.m_tsbDuplicate, Me.m_ts2, Me.m_tsbSetTo0, Me.m_tsbSetToBaseline, Me.m_tsbSetToValue, Me.m_tsbResetAll, Me.m_tscmbFilter, Me.m_ts3, Me.m_tsbnShowExtraData, Me.m_tsbnDiscardExtraData, Me.m_tsbnFilterCase, Me.m_tstbxFilterName, Me.m_tslFilter})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_ts.Size = New System.Drawing.Size(1271, 25)
            Me.m_ts.TabIndex = 0
            '
            'm_tsbImport
            '
            Me.m_tsbImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbImport.Image = CType(resources.GetObject("m_tsbImport.Image"), System.Drawing.Image)
            Me.m_tsbImport.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbImport.Name = "m_tsbImport"
            Me.m_tsbImport.Size = New System.Drawing.Size(56, 22)
            Me.m_tsbImport.Text = "&Import..."
            '
            'm_tsbExport
            '
            Me.m_tsbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbExport.Image = CType(resources.GetObject("m_tsbExport.Image"), System.Drawing.Image)
            Me.m_tsbExport.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbExport.Name = "m_tsbExport"
            Me.m_tsbExport.Size = New System.Drawing.Size(53, 22)
            Me.m_tsbExport.Text = "E&xport..."
            '
            'm_tsbLoad
            '
            Me.m_tsbLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbLoad.Image = CType(resources.GetObject("m_tsbLoad.Image"), System.Drawing.Image)
            Me.m_tsbLoad.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbLoad.Name = "m_tsbLoad"
            Me.m_tsbLoad.Size = New System.Drawing.Size(46, 22)
            Me.m_tsbLoad.Text = "&Load..."
            '
            'm_tsbWeight
            '
            Me.m_tsbWeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbWeight.Image = CType(resources.GetObject("m_tsbWeight.Image"), System.Drawing.Image)
            Me.m_tsbWeight.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbWeight.Name = "m_tsbWeight"
            Me.m_tsbWeight.Size = New System.Drawing.Size(58, 22)
            Me.m_tsbWeight.Text = "&Weight..."
            '
            'm_ts1
            '
            Me.m_ts1.Name = "m_ts1"
            Me.m_ts1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbAdd
            '
            Me.m_tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbAdd.Image = CType(resources.GetObject("m_tsbAdd.Image"), System.Drawing.Image)
            Me.m_tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbAdd.Name = "m_tsbAdd"
            Me.m_tsbAdd.Size = New System.Drawing.Size(42, 22)
            Me.m_tsbAdd.Text = "&Add..."
            '
            'm_tsbRemove
            '
            Me.m_tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbRemove.Image = CType(resources.GetObject("m_tsbRemove.Image"), System.Drawing.Image)
            Me.m_tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbRemove.Name = "m_tsbRemove"
            Me.m_tsbRemove.Size = New System.Drawing.Size(63, 22)
            Me.m_tsbRemove.Text = "&Remove..."
            '
            'm_tsbDuplicate
            '
            Me.m_tsbDuplicate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbDuplicate.Image = CType(resources.GetObject("m_tsbDuplicate.Image"), System.Drawing.Image)
            Me.m_tsbDuplicate.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbDuplicate.Name = "m_tsbDuplicate"
            Me.m_tsbDuplicate.Size = New System.Drawing.Size(61, 22)
            Me.m_tsbDuplicate.Text = "&Duplicate"
            '
            'm_ts2
            '
            Me.m_ts2.Name = "m_ts2"
            Me.m_ts2.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbSetTo0
            '
            Me.m_tsbSetTo0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSetTo0.Image = CType(resources.GetObject("m_tsbSetTo0.Image"), System.Drawing.Image)
            Me.m_tsbSetTo0.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSetTo0.Name = "m_tsbSetTo0"
            Me.m_tsbSetTo0.Size = New System.Drawing.Size(50, 22)
            Me.m_tsbSetTo0.Text = "&Set to 0"
            '
            'm_tsbSetToBaseline
            '
            Me.m_tsbSetToBaseline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSetToBaseline.Image = CType(resources.GetObject("m_tsbSetToBaseline.Image"), System.Drawing.Image)
            Me.m_tsbSetToBaseline.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSetToBaseline.Name = "m_tsbSetToBaseline"
            Me.m_tsbSetToBaseline.Size = New System.Drawing.Size(87, 22)
            Me.m_tsbSetToBaseline.Text = "Set to &baseline"
            '
            'm_tsbSetToValue
            '
            Me.m_tsbSetToValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSetToValue.Image = CType(resources.GetObject("m_tsbSetToValue.Image"), System.Drawing.Image)
            Me.m_tsbSetToValue.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSetToValue.Name = "m_tsbSetToValue"
            Me.m_tsbSetToValue.Size = New System.Drawing.Size(81, 22)
            Me.m_tsbSetToValue.Text = "Set to &value..."
            '
            'm_tsbResetAll
            '
            Me.m_tsbResetAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbResetAll.Image = CType(resources.GetObject("m_tsbResetAll.Image"), System.Drawing.Image)
            Me.m_tsbResetAll.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbResetAll.Name = "m_tsbResetAll"
            Me.m_tsbResetAll.Size = New System.Drawing.Size(56, 22)
            Me.m_tsbResetAll.Text = "Reset &All"
            '
            'm_tscmbFilter
            '
            Me.m_tscmbFilter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tscmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmbFilter.DropDownWidth = 200
            Me.m_tscmbFilter.MaxDropDownItems = 12
            Me.m_tscmbFilter.Name = "m_tscmbFilter"
            Me.m_tscmbFilter.Size = New System.Drawing.Size(150, 25)
            Me.m_tscmbFilter.Sorted = True
            '
            'm_ts3
            '
            Me.m_ts3.Name = "m_ts3"
            Me.m_ts3.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbnShowExtraData
            '
            Me.m_tsbnShowExtraData.CheckOnClick = True
            Me.m_tsbnShowExtraData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnShowExtraData.Image = CType(resources.GetObject("m_tsbnShowExtraData.Image"), System.Drawing.Image)
            Me.m_tsbnShowExtraData.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnShowExtraData.Name = "m_tsbnShowExtraData"
            Me.m_tsbnShowExtraData.Size = New System.Drawing.Size(94, 22)
            Me.m_tsbnShowExtraData.Text = "Show extra &data"
            '
            'm_tsbnDiscardExtraData
            '
            Me.m_tsbnDiscardExtraData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnDiscardExtraData.Image = CType(resources.GetObject("m_tsbnDiscardExtraData.Image"), System.Drawing.Image)
            Me.m_tsbnDiscardExtraData.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnDiscardExtraData.Name = "m_tsbnDiscardExtraData"
            Me.m_tsbnDiscardExtraData.Size = New System.Drawing.Size(104, 22)
            Me.m_tsbnDiscardExtraData.Text = "Discard e&xtra data"
            '
            'm_tsbnFilterCase
            '
            Me.m_tsbnFilterCase.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tsbnFilterCase.CheckOnClick = True
            Me.m_tsbnFilterCase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnFilterCase.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnFilterCase.Name = "m_tsbnFilterCase"
            Me.m_tsbnFilterCase.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tsbnFilterCase.Size = New System.Drawing.Size(25, 22)
            Me.m_tsbnFilterCase.Text = "Aa"
            Me.m_tsbnFilterCase.ToolTipText = "Name filter is case-sensitive"
            '
            'm_tstbxFilterName
            '
            Me.m_tstbxFilterName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tstbxFilterName.Name = "m_tstbxFilterName"
            Me.m_tstbxFilterName.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
            Me.m_tstbxFilterName.Size = New System.Drawing.Size(100, 25)
            Me.m_tstbxFilterName.ToolTipText = "Filter by name"
            '
            'm_tslFilter
            '
            Me.m_tslFilter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tslFilter.Image = Global.ScientificInterfaceShared.My.Resources.Resources.FilterHS
            Me.m_tslFilter.Name = "m_tslFilter"
            Me.m_tslFilter.Size = New System.Drawing.Size(26, 22)
            Me.m_tslFilter.Text = ":"
            '
            'ucShapeToolboxToolbar
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ts)
            Me.Name = "ucShapeToolboxToolbar"
            Me.Size = New System.Drawing.Size(1271, 25)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As cEwEToolstrip
        Private WithEvents m_tsbImport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbLoad As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbAdd As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbDuplicate As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbRemove As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbWeight As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbSetToValue As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbResetAll As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbExport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tscmbFilter As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tsbSetToBaseline As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts3 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbnShowExtraData As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnDiscardExtraData As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnFilterCase As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tstbxFilterName As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tslFilter As System.Windows.Forms.ToolStripLabel

    End Class

End Namespace
