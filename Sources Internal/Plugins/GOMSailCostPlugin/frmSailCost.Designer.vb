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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSailCost
    Inherits ScientificInterfaceShared.Forms.frmEwE

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSailCost))
        Me.m_lblWarning = New System.Windows.Forms.Label()
        Me.m_chkUseSailCost = New System.Windows.Forms.CheckBox()
        Me.m_lblPath = New System.Windows.Forms.Label()
        Me.m_btnChoosePath = New System.Windows.Forms.Button()
        Me.m_tbxPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lvValidation = New System.Windows.Forms.ListView()
        Me.m_colDriver = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_colFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_colFound = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_cbUseMortalitiesWriter = New System.Windows.Forms.CheckBox()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrLMEEffort = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblRunMode = New System.Windows.Forms.Label()
        Me.m_rbRunModeOrg = New System.Windows.Forms.RadioButton()
        Me.m_rbRunModeFixed = New System.Windows.Forms.RadioButton()
        Me.m_tbxRunModeFixedYear = New System.Windows.Forms.TextBox()
        Me.m_rbRunModeNone = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'm_lblWarning
        '
        resources.ApplyResources(Me.m_lblWarning, "m_lblWarning")
        Me.m_lblWarning.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.m_lblWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lblWarning.ForeColor = System.Drawing.Color.Red
        Me.m_lblWarning.Name = "m_lblWarning"
        '
        'm_chkUseSailCost
        '
        resources.ApplyResources(Me.m_chkUseSailCost, "m_chkUseSailCost")
        Me.m_chkUseSailCost.Name = "m_chkUseSailCost"
        Me.m_chkUseSailCost.UseVisualStyleBackColor = True
        '
        'm_lblPath
        '
        resources.ApplyResources(Me.m_lblPath, "m_lblPath")
        Me.m_lblPath.Name = "m_lblPath"
        '
        'm_btnChoosePath
        '
        resources.ApplyResources(Me.m_btnChoosePath, "m_btnChoosePath")
        Me.m_btnChoosePath.Name = "m_btnChoosePath"
        Me.m_btnChoosePath.UseVisualStyleBackColor = True
        '
        'm_tbxPath
        '
        resources.ApplyResources(Me.m_tbxPath, "m_tbxPath")
        Me.m_tbxPath.Name = "m_tbxPath"
        Me.m_tbxPath.ReadOnly = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_lvValidation
        '
        resources.ApplyResources(Me.m_lvValidation, "m_lvValidation")
        Me.m_lvValidation.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.m_colDriver, Me.m_colFile, Me.m_colFound})
        Me.m_lvValidation.FullRowSelect = True
        Me.m_lvValidation.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.m_lvValidation.Name = "m_lvValidation"
        Me.m_lvValidation.UseCompatibleStateImageBehavior = False
        Me.m_lvValidation.View = System.Windows.Forms.View.Details
        '
        'm_colDriver
        '
        resources.ApplyResources(Me.m_colDriver, "m_colDriver")
        '
        'm_colFile
        '
        resources.ApplyResources(Me.m_colFile, "m_colFile")
        '
        'm_colFound
        '
        resources.ApplyResources(Me.m_colFound, "m_colFound")
        '
        'm_cbUseMortalitiesWriter
        '
        resources.ApplyResources(Me.m_cbUseMortalitiesWriter, "m_cbUseMortalitiesWriter")
        Me.m_cbUseMortalitiesWriter.Name = "m_cbUseMortalitiesWriter"
        Me.m_cbUseMortalitiesWriter.UseVisualStyleBackColor = True
        '
        'CEwEHeaderLabel1
        '
        resources.ApplyResources(Me.CEwEHeaderLabel1, "CEwEHeaderLabel1")
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        '
        'm_hdrLMEEffort
        '
        resources.ApplyResources(Me.m_hdrLMEEffort, "m_hdrLMEEffort")
        Me.m_hdrLMEEffort.CanCollapseParent = False
        Me.m_hdrLMEEffort.CollapsedParentHeight = 0
        Me.m_hdrLMEEffort.IsCollapsed = False
        Me.m_hdrLMEEffort.Name = "m_hdrLMEEffort"
        '
        'm_lblRunMode
        '
        resources.ApplyResources(Me.m_lblRunMode, "m_lblRunMode")
        Me.m_lblRunMode.Name = "m_lblRunMode"
        '
        'm_rbRunModeOrg
        '
        resources.ApplyResources(Me.m_rbRunModeOrg, "m_rbRunModeOrg")
        Me.m_rbRunModeOrg.Checked = True
        Me.m_rbRunModeOrg.Name = "m_rbRunModeOrg"
        Me.m_rbRunModeOrg.TabStop = True
        Me.m_rbRunModeOrg.Tag = "0"
        Me.m_rbRunModeOrg.UseVisualStyleBackColor = True
        '
        'm_rbRunModeFixed
        '
        resources.ApplyResources(Me.m_rbRunModeFixed, "m_rbRunModeFixed")
        Me.m_rbRunModeFixed.Name = "m_rbRunModeFixed"
        Me.m_rbRunModeFixed.TabStop = True
        Me.m_rbRunModeFixed.Tag = "1"
        Me.m_rbRunModeFixed.UseVisualStyleBackColor = True
        '
        'm_tbxRunModeFixedYear
        '
        resources.ApplyResources(Me.m_tbxRunModeFixedYear, "m_tbxRunModeFixedYear")
        Me.m_tbxRunModeFixedYear.Name = "m_tbxRunModeFixedYear"
        '
        'm_rbRunModeNone
        '
        resources.ApplyResources(Me.m_rbRunModeNone, "m_rbRunModeNone")
        Me.m_rbRunModeNone.Name = "m_rbRunModeNone"
        Me.m_rbRunModeNone.TabStop = True
        Me.m_rbRunModeNone.Tag = "2"
        Me.m_rbRunModeNone.UseVisualStyleBackColor = True
        '
        'frmSailCost
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tbxRunModeFixedYear)
        Me.Controls.Add(Me.m_rbRunModeNone)
        Me.Controls.Add(Me.m_rbRunModeFixed)
        Me.Controls.Add(Me.m_rbRunModeOrg)
        Me.Controls.Add(Me.m_lblRunMode)
        Me.Controls.Add(Me.m_hdrLMEEffort)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.m_lvValidation)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_tbxPath)
        Me.Controls.Add(Me.m_btnChoosePath)
        Me.Controls.Add(Me.m_lblPath)
        Me.Controls.Add(Me.m_cbUseMortalitiesWriter)
        Me.Controls.Add(Me.m_chkUseSailCost)
        Me.Controls.Add(Me.m_lblWarning)
        Me.Name = "frmSailCost"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblPath As Windows.Forms.Label
    Private WithEvents m_btnChoosePath As Windows.Forms.Button
    Private WithEvents m_lblWarning As Windows.Forms.Label
    Private WithEvents m_tbxPath As Windows.Forms.TextBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Private WithEvents m_lvValidation As Windows.Forms.ListView
    Private WithEvents m_colDriver As Windows.Forms.ColumnHeader
    Private WithEvents m_colFile As Windows.Forms.ColumnHeader
    Private WithEvents m_colFound As Windows.Forms.ColumnHeader
    Private WithEvents m_chkUseSailCost As Windows.Forms.CheckBox
    Private WithEvents m_cbUseMortalitiesWriter As Windows.Forms.CheckBox
    Friend WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrLMEEffort As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblRunMode As Windows.Forms.Label
    Friend WithEvents m_rbRunModeOrg As Windows.Forms.RadioButton
    Friend WithEvents m_rbRunModeFixed As Windows.Forms.RadioButton
    Friend WithEvents m_tbxRunModeFixedYear As Windows.Forms.TextBox
    Private WithEvents m_rbRunModeNone As Windows.Forms.RadioButton
End Class
