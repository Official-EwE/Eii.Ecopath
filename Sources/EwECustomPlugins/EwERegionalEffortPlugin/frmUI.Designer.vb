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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUI
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUI))
        Me.m_cbAutoModeEnabled = New System.Windows.Forms.CheckBox()
        Me.m_btnChoosePath = New System.Windows.Forms.Button()
        Me.m_cbWriteMortalities = New System.Windows.Forms.CheckBox()
        Me.m_hdrLMEEffort = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tbxEffortDistThreshold = New System.Windows.Forms.TextBox()
        Me.m_lblSailingCostThreshold = New System.Windows.Forms.Label()
        Me.m_lblEffortFile = New System.Windows.Forms.Label()
        Me.m_lblZone = New System.Windows.Forms.Label()
        Me.m_hdrValidation = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_cbWriteCatches = New System.Windows.Forms.CheckBox()
        Me.m_cbWriteEffort = New System.Windows.Forms.CheckBox()
        Me.m_hdrZones = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnLoad = New System.Windows.Forms.Button()
        Me.m_tbxEffortFile = New System.Windows.Forms.TextBox()
        Me.m_lblZoneName = New System.Windows.Forms.Label()
        Me.m_lblZoneInfo2 = New System.Windows.Forms.TextBox()
        Me.m_tbxZoneName = New System.Windows.Forms.TextBox()
        Me.m_pbStatus = New System.Windows.Forms.PictureBox()
        Me.m_lblStatus = New System.Windows.Forms.Label()
        Me.m_btnCalc = New System.Windows.Forms.Button()
        Me.m_btnLoadMap = New System.Windows.Forms.Button()
        Me.m_cbOnlyFishBelowCostThreshold = New System.Windows.Forms.CheckBox()
        Me.m_cbNormalizeZonalEffort = New System.Windows.Forms.CheckBox()
        CType(Me.m_pbStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_cbAutoModeEnabled
        '
        resources.ApplyResources(Me.m_cbAutoModeEnabled, "m_cbAutoModeEnabled")
        Me.m_cbAutoModeEnabled.Checked = True
        Me.m_cbAutoModeEnabled.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_cbAutoModeEnabled.Name = "m_cbAutoModeEnabled"
        Me.m_cbAutoModeEnabled.UseVisualStyleBackColor = True
        '
        'm_btnChoosePath
        '
        resources.ApplyResources(Me.m_btnChoosePath, "m_btnChoosePath")
        Me.m_btnChoosePath.Name = "m_btnChoosePath"
        Me.m_btnChoosePath.UseVisualStyleBackColor = True
        '
        'm_cbWriteMortalities
        '
        resources.ApplyResources(Me.m_cbWriteMortalities, "m_cbWriteMortalities")
        Me.m_cbWriteMortalities.Name = "m_cbWriteMortalities"
        Me.m_cbWriteMortalities.UseVisualStyleBackColor = True
        '
        'm_hdrLMEEffort
        '
        resources.ApplyResources(Me.m_hdrLMEEffort, "m_hdrLMEEffort")
        Me.m_hdrLMEEffort.CanCollapseParent = False
        Me.m_hdrLMEEffort.CollapsedParentHeight = 0
        Me.m_hdrLMEEffort.IsCollapsed = False
        Me.m_hdrLMEEffort.Name = "m_hdrLMEEffort"
        '
        'm_tbxEffortDistThreshold
        '
        resources.ApplyResources(Me.m_tbxEffortDistThreshold, "m_tbxEffortDistThreshold")
        Me.m_tbxEffortDistThreshold.Name = "m_tbxEffortDistThreshold"
        '
        'm_lblSailingCostThreshold
        '
        resources.ApplyResources(Me.m_lblSailingCostThreshold, "m_lblSailingCostThreshold")
        Me.m_lblSailingCostThreshold.Name = "m_lblSailingCostThreshold"
        '
        'm_lblEffortFile
        '
        resources.ApplyResources(Me.m_lblEffortFile, "m_lblEffortFile")
        Me.m_lblEffortFile.Name = "m_lblEffortFile"
        '
        'm_lblZone
        '
        resources.ApplyResources(Me.m_lblZone, "m_lblZone")
        Me.m_lblZone.Name = "m_lblZone"
        '
        'm_hdrValidation
        '
        resources.ApplyResources(Me.m_hdrValidation, "m_hdrValidation")
        Me.m_hdrValidation.CanCollapseParent = False
        Me.m_hdrValidation.CollapsedParentHeight = 0
        Me.m_hdrValidation.IsCollapsed = False
        Me.m_hdrValidation.Name = "m_hdrValidation"
        '
        'm_cbWriteCatches
        '
        resources.ApplyResources(Me.m_cbWriteCatches, "m_cbWriteCatches")
        Me.m_cbWriteCatches.Name = "m_cbWriteCatches"
        Me.m_cbWriteCatches.UseVisualStyleBackColor = True
        '
        'm_cbWriteEffort
        '
        resources.ApplyResources(Me.m_cbWriteEffort, "m_cbWriteEffort")
        Me.m_cbWriteEffort.Name = "m_cbWriteEffort"
        Me.m_cbWriteEffort.UseVisualStyleBackColor = True
        '
        'm_hdrZones
        '
        resources.ApplyResources(Me.m_hdrZones, "m_hdrZones")
        Me.m_hdrZones.CanCollapseParent = False
        Me.m_hdrZones.CollapsedParentHeight = 0
        Me.m_hdrZones.IsCollapsed = False
        Me.m_hdrZones.Name = "m_hdrZones"
        '
        'm_btnLoad
        '
        resources.ApplyResources(Me.m_btnLoad, "m_btnLoad")
        Me.m_btnLoad.Name = "m_btnLoad"
        Me.m_btnLoad.UseVisualStyleBackColor = True
        '
        'm_tbxEffortFile
        '
        resources.ApplyResources(Me.m_tbxEffortFile, "m_tbxEffortFile")
        Me.m_tbxEffortFile.Name = "m_tbxEffortFile"
        Me.m_tbxEffortFile.ReadOnly = True
        '
        'm_lblZoneName
        '
        resources.ApplyResources(Me.m_lblZoneName, "m_lblZoneName")
        Me.m_lblZoneName.Name = "m_lblZoneName"
        '
        'm_lblZoneInfo2
        '
        resources.ApplyResources(Me.m_lblZoneInfo2, "m_lblZoneInfo2")
        Me.m_lblZoneInfo2.Name = "m_lblZoneInfo2"
        Me.m_lblZoneInfo2.ReadOnly = True
        '
        'm_tbxZoneName
        '
        resources.ApplyResources(Me.m_tbxZoneName, "m_tbxZoneName")
        Me.m_tbxZoneName.Name = "m_tbxZoneName"
        '
        'm_pbStatus
        '
        resources.ApplyResources(Me.m_pbStatus, "m_pbStatus")
        Me.m_pbStatus.Name = "m_pbStatus"
        Me.m_pbStatus.TabStop = False
        '
        'm_lblStatus
        '
        resources.ApplyResources(Me.m_lblStatus, "m_lblStatus")
        Me.m_lblStatus.Name = "m_lblStatus"
        '
        'm_btnCalc
        '
        resources.ApplyResources(Me.m_btnCalc, "m_btnCalc")
        Me.m_btnCalc.Name = "m_btnCalc"
        Me.m_btnCalc.UseVisualStyleBackColor = True
        '
        'm_btnLoadMap
        '
        resources.ApplyResources(Me.m_btnLoadMap, "m_btnLoadMap")
        Me.m_btnLoadMap.Name = "m_btnLoadMap"
        Me.m_btnLoadMap.UseVisualStyleBackColor = True
        '
        'm_cbOnlyFishBelowCostThreshold
        '
        resources.ApplyResources(Me.m_cbOnlyFishBelowCostThreshold, "m_cbOnlyFishBelowCostThreshold")
        Me.m_cbOnlyFishBelowCostThreshold.Name = "m_cbOnlyFishBelowCostThreshold"
        Me.m_cbOnlyFishBelowCostThreshold.UseVisualStyleBackColor = True
        '
        'm_cbNormalizeZonalEffort
        '
        resources.ApplyResources(Me.m_cbNormalizeZonalEffort, "m_cbNormalizeZonalEffort")
        Me.m_cbNormalizeZonalEffort.Name = "m_cbNormalizeZonalEffort"
        Me.m_cbNormalizeZonalEffort.UseVisualStyleBackColor = True
        '
        'frmUI
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ControlBox = False
        Me.Controls.Add(Me.m_cbNormalizeZonalEffort)
        Me.Controls.Add(Me.m_btnCalc)
        Me.Controls.Add(Me.m_lblStatus)
        Me.Controls.Add(Me.m_pbStatus)
        Me.Controls.Add(Me.m_tbxZoneName)
        Me.Controls.Add(Me.m_lblZoneInfo2)
        Me.Controls.Add(Me.m_lblZoneName)
        Me.Controls.Add(Me.m_tbxEffortFile)
        Me.Controls.Add(Me.m_cbWriteEffort)
        Me.Controls.Add(Me.m_hdrValidation)
        Me.Controls.Add(Me.m_lblZone)
        Me.Controls.Add(Me.m_lblEffortFile)
        Me.Controls.Add(Me.m_lblSailingCostThreshold)
        Me.Controls.Add(Me.m_tbxEffortDistThreshold)
        Me.Controls.Add(Me.m_cbOnlyFishBelowCostThreshold)
        Me.Controls.Add(Me.m_hdrZones)
        Me.Controls.Add(Me.m_hdrLMEEffort)
        Me.Controls.Add(Me.m_btnLoadMap)
        Me.Controls.Add(Me.m_btnLoad)
        Me.Controls.Add(Me.m_btnChoosePath)
        Me.Controls.Add(Me.m_cbWriteCatches)
        Me.Controls.Add(Me.m_cbWriteMortalities)
        Me.Controls.Add(Me.m_cbAutoModeEnabled)
        Me.Name = "frmUI"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        CType(Me.m_pbStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_btnChoosePath As Windows.Forms.Button
    Private WithEvents m_cbAutoModeEnabled As Windows.Forms.CheckBox
    Private WithEvents m_cbWriteMortalities As Windows.Forms.CheckBox
    Private WithEvents m_hdrLMEEffort As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tbxEffortDistThreshold As Windows.Forms.TextBox
    Private WithEvents m_lblEffortFile As Windows.Forms.Label
    Private WithEvents m_lblZone As Windows.Forms.Label
    Private WithEvents m_hdrValidation As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbWriteCatches As Windows.Forms.CheckBox
    Private WithEvents m_cbWriteEffort As Windows.Forms.CheckBox
    Private WithEvents m_hdrZones As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnLoad As Windows.Forms.Button
    Private WithEvents m_tbxEffortFile As Windows.Forms.TextBox
    Private WithEvents m_lblSailingCostThreshold As Windows.Forms.Label
    Private WithEvents m_lblZoneName As Windows.Forms.Label
    Private WithEvents m_lblZoneInfo2 As Windows.Forms.TextBox
    Private WithEvents m_tbxZoneName As Windows.Forms.TextBox
    Private WithEvents m_pbStatus As Windows.Forms.PictureBox
    Private WithEvents m_lblStatus As Windows.Forms.Label
    Private WithEvents m_btnCalc As Windows.Forms.Button
    Private WithEvents m_btnLoadMap As Windows.Forms.Button
    Private WithEvents m_cbOnlyFishBelowCostThreshold As Windows.Forms.CheckBox
    Private WithEvents m_cbNormalizeZonalEffort As Windows.Forms.CheckBox
End Class
