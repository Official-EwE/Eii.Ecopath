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

Namespace Controls


    Partial Class dlgChangeShape
        Inherits System.Windows.Forms.Form

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgChangeShape))
            Me.m_tbxC = New System.Windows.Forms.TextBox()
            Me.m_lblC = New System.Windows.Forms.Label()
            Me.m_tbxA = New System.Windows.Forms.TextBox()
            Me.m_lblA = New System.Windows.Forms.Label()
            Me.m_tbxD = New System.Windows.Forms.TextBox()
            Me.m_tbxB = New System.Windows.Forms.TextBox()
            Me.m_lblD = New System.Windows.Forms.Label()
            Me.m_lblB = New System.Windows.Forms.Label()
            Me.m_btnOk = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.m_plPreview = New System.Windows.Forms.Panel()
            Me.m_lbShapeFunctionTypes = New System.Windows.Forms.ListBox()
            Me.m_hdrShape = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrParams = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpParams = New System.Windows.Forms.TableLayoutPanel()
            Me.m_tbxMaxValue = New System.Windows.Forms.TextBox()
            Me.m_lblMax = New System.Windows.Forms.Label()
            Me.m_btDefaults = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            Me.m_tlpParams.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tbxC
            '
            resources.ApplyResources(Me.m_tbxC, "m_tbxC")
            Me.m_tbxC.Name = "m_tbxC"
            '
            'm_lblC
            '
            resources.ApplyResources(Me.m_lblC, "m_lblC")
            Me.m_lblC.Name = "m_lblC"
            '
            'm_tbxA
            '
            resources.ApplyResources(Me.m_tbxA, "m_tbxA")
            Me.m_tbxA.Name = "m_tbxA"
            '
            'm_lblA
            '
            resources.ApplyResources(Me.m_lblA, "m_lblA")
            Me.m_lblA.Name = "m_lblA"
            '
            'm_tbxD
            '
            resources.ApplyResources(Me.m_tbxD, "m_tbxD")
            Me.m_tbxD.Name = "m_tbxD"
            '
            'm_tbxB
            '
            resources.ApplyResources(Me.m_tbxB, "m_tbxB")
            Me.m_tbxB.Name = "m_tbxB"
            '
            'm_lblD
            '
            resources.ApplyResources(Me.m_lblD, "m_lblD")
            Me.m_lblD.Name = "m_lblD"
            '
            'm_lblB
            '
            resources.ApplyResources(Me.m_lblB, "m_lblB")
            Me.m_lblB.Name = "m_lblB"
            '
            'm_btnOk
            '
            resources.ApplyResources(Me.m_btnOk, "m_btnOk")
            Me.m_btnOk.Name = "m_btnOk"
            Me.m_btnOk.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_plPreview
            '
            resources.ApplyResources(Me.m_plPreview, "m_plPreview")
            Me.m_plPreview.BackColor = System.Drawing.SystemColors.Window
            Me.m_plPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plPreview.Name = "m_plPreview"
            '
            'm_lbShapeFunctionTypes
            '
            resources.ApplyResources(Me.m_lbShapeFunctionTypes, "m_lbShapeFunctionTypes")
            Me.m_lbShapeFunctionTypes.FormattingEnabled = True
            Me.m_lbShapeFunctionTypes.Name = "m_lbShapeFunctionTypes"
            Me.m_lbShapeFunctionTypes.Sorted = True
            '
            'm_hdrShape
            '
            Me.m_hdrShape.CanCollapseParent = False
            Me.m_hdrShape.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrShape, "m_hdrShape")
            Me.m_hdrShape.IsCollapsed = False
            Me.m_hdrShape.Name = "m_hdrShape"
            '
            'm_hdrParams
            '
            resources.ApplyResources(Me.m_hdrParams, "m_hdrParams")
            Me.m_hdrParams.CanCollapseParent = False
            Me.m_hdrParams.CollapsedParentHeight = 0
            Me.m_hdrParams.IsCollapsed = False
            Me.m_hdrParams.Name = "m_hdrParams"
            '
            'm_tlpParams
            '
            resources.ApplyResources(Me.m_tlpParams, "m_tlpParams")
            Me.m_tlpParams.Controls.Add(Me.m_lblA, 0, 0)
            Me.m_tlpParams.Controls.Add(Me.m_tbxA, 1, 0)
            Me.m_tlpParams.Controls.Add(Me.m_lblB, 0, 1)
            Me.m_tlpParams.Controls.Add(Me.m_tbxB, 1, 1)
            Me.m_tlpParams.Controls.Add(Me.m_tbxC, 1, 2)
            Me.m_tlpParams.Controls.Add(Me.m_lblC, 0, 2)
            Me.m_tlpParams.Controls.Add(Me.m_tbxMaxValue, 1, 4)
            Me.m_tlpParams.Controls.Add(Me.m_tbxD, 1, 3)
            Me.m_tlpParams.Controls.Add(Me.m_lblD, 0, 3)
            Me.m_tlpParams.Controls.Add(Me.m_lblMax, 0, 4)
            Me.m_tlpParams.Name = "m_tlpParams"
            '
            'm_tbxMaxValue
            '
            resources.ApplyResources(Me.m_tbxMaxValue, "m_tbxMaxValue")
            Me.m_tbxMaxValue.Name = "m_tbxMaxValue"
            '
            'm_lblMax
            '
            resources.ApplyResources(Me.m_lblMax, "m_lblMax")
            Me.m_lblMax.Name = "m_lblMax"
            '
            'm_btDefaults
            '
            resources.ApplyResources(Me.m_btDefaults, "m_btDefaults")
            Me.m_btDefaults.Name = "m_btDefaults"
            Me.m_btDefaults.UseVisualStyleBackColor = True
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'm_tbxName
            '
            resources.ApplyResources(Me.m_tbxName, "m_tbxName")
            Me.m_tbxName.Name = "m_tbxName"
            '
            'dlgChangeShape
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tbxName)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_btDefaults)
            Me.Controls.Add(Me.m_tlpParams)
            Me.Controls.Add(Me.m_lbShapeFunctionTypes)
            Me.Controls.Add(Me.m_hdrShape)
            Me.Controls.Add(Me.m_hdrParams)
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_btnOk)
            Me.Controls.Add(Me.m_btnCancel)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgChangeShape"
            Me.ShowInTaskbar = False
            Me.m_tlpParams.ResumeLayout(False)
            Me.m_tlpParams.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tbxC As System.Windows.Forms.TextBox
        Private WithEvents m_lblC As System.Windows.Forms.Label
        Private WithEvents m_tbxA As System.Windows.Forms.TextBox
        Private WithEvents m_lblA As System.Windows.Forms.Label
        Private WithEvents m_tbxD As System.Windows.Forms.TextBox
        Private WithEvents m_tbxB As System.Windows.Forms.TextBox
        Private WithEvents m_lblD As System.Windows.Forms.Label
        Private WithEvents m_lblB As System.Windows.Forms.Label
        Private WithEvents m_btnOk As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_plPreview As System.Windows.Forms.Panel
        Private WithEvents m_hdrParams As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrShape As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_lbShapeFunctionTypes As System.Windows.Forms.ListBox
        Private WithEvents m_tlpParams As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_btDefaults As System.Windows.Forms.Button
        Friend WithEvents m_lblMax As System.Windows.Forms.Label
        Friend WithEvents m_tbxMaxValue As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_tbxName As System.Windows.Forms.TextBox

    End Class

End Namespace

