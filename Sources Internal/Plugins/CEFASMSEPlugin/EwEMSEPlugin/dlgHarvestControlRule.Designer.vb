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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgHarvestControlRule
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgHarvestControlRule))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.m_cbBiomassGroups = New System.Windows.Forms.ComboBox()
        Me.m_cbFMortGroups = New System.Windows.Forms.ComboBox()
        Me.m_lblBiomassGroup = New System.Windows.Forms.Label()
        Me.m_lblFMortGroup = New System.Windows.Forms.Label()
        Me.m_tbxRule = New System.Windows.Forms.TextBox()
        Me.m_cbCostFunctions = New System.Windows.Forms.ComboBox()
        Me.m_lblHRCType = New System.Windows.Forms.Label()
        Me.m_lblBiomassGroupInfo = New System.Windows.Forms.Label()
        Me.m_lblInfoFMortGroup = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.m_lblHCRTypeInfo = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.Name = "OK_Button"
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'm_cbBiomassGroups
        '
        Me.m_cbBiomassGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cbBiomassGroups.FormattingEnabled = True
        resources.ApplyResources(Me.m_cbBiomassGroups, "m_cbBiomassGroups")
        Me.m_cbBiomassGroups.Name = "m_cbBiomassGroups"
        '
        'm_cbFMortGroups
        '
        Me.m_cbFMortGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cbFMortGroups.FormattingEnabled = True
        resources.ApplyResources(Me.m_cbFMortGroups, "m_cbFMortGroups")
        Me.m_cbFMortGroups.Name = "m_cbFMortGroups"
        '
        'm_lblBiomassGroup
        '
        resources.ApplyResources(Me.m_lblBiomassGroup, "m_lblBiomassGroup")
        Me.m_lblBiomassGroup.Name = "m_lblBiomassGroup"
        '
        'm_lblFMortGroup
        '
        resources.ApplyResources(Me.m_lblFMortGroup, "m_lblFMortGroup")
        Me.m_lblFMortGroup.Name = "m_lblFMortGroup"
        '
        'm_tbxRule
        '
        resources.ApplyResources(Me.m_tbxRule, "m_tbxRule")
        Me.m_tbxRule.Name = "m_tbxRule"
        '
        'm_cbCostFunctions
        '
        Me.m_cbCostFunctions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cbCostFunctions.FormattingEnabled = True
        resources.ApplyResources(Me.m_cbCostFunctions, "m_cbCostFunctions")
        Me.m_cbCostFunctions.Name = "m_cbCostFunctions"
        '
        'm_lblHRCType
        '
        resources.ApplyResources(Me.m_lblHRCType, "m_lblHRCType")
        Me.m_lblHRCType.Name = "m_lblHRCType"
        '
        'm_lblBiomassGroupInfo
        '
        resources.ApplyResources(Me.m_lblBiomassGroupInfo, "m_lblBiomassGroupInfo")
        Me.m_lblBiomassGroupInfo.Name = "m_lblBiomassGroupInfo"
        '
        'm_lblInfoFMortGroup
        '
        resources.ApplyResources(Me.m_lblInfoFMortGroup, "m_lblInfoFMortGroup")
        Me.m_lblInfoFMortGroup.Name = "m_lblInfoFMortGroup"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'm_lblHCRTypeInfo
        '
        resources.ApplyResources(Me.m_lblHCRTypeInfo, "m_lblHCRTypeInfo")
        Me.m_lblHCRTypeInfo.Name = "m_lblHCRTypeInfo"
        '
        'dlgHarvestControlRule
        '
        Me.AcceptButton = Me.OK_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.m_lblHCRTypeInfo)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.m_lblInfoFMortGroup)
        Me.Controls.Add(Me.m_lblBiomassGroupInfo)
        Me.Controls.Add(Me.m_lblHRCType)
        Me.Controls.Add(Me.m_cbCostFunctions)
        Me.Controls.Add(Me.m_tbxRule)
        Me.Controls.Add(Me.m_lblFMortGroup)
        Me.Controls.Add(Me.m_lblBiomassGroup)
        Me.Controls.Add(Me.m_cbFMortGroups)
        Me.Controls.Add(Me.m_cbBiomassGroups)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgHarvestControlRule"
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents m_lblBiomassGroup As System.Windows.Forms.Label
    Private WithEvents m_lblFMortGroup As System.Windows.Forms.Label
    Private WithEvents m_lblInfoFMortGroup As System.Windows.Forms.Label
    Private WithEvents m_lblBiomassGroupInfo As System.Windows.Forms.Label
    Private WithEvents m_lblHCRTypeInfo As System.Windows.Forms.Label
    Private WithEvents m_lblHRCType As System.Windows.Forms.Label
    Private WithEvents m_cbCostFunctions As System.Windows.Forms.ComboBox
    Private WithEvents m_cbBiomassGroups As System.Windows.Forms.ComboBox
    Private WithEvents m_cbFMortGroups As System.Windows.Forms.ComboBox
    Private WithEvents m_tbxRule As System.Windows.Forms.TextBox

End Class
