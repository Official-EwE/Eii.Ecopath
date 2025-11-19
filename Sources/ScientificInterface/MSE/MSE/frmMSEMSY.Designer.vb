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

Imports ScientificInterfaceShared.Forms

Partial Class frmMSEMSY
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEMSY))
        Me.m_btnRunMSY = New System.Windows.Forms.Button()
        Me.m_btnStop = New System.Windows.Forms.Button()
        Me.m_txtMSYresults = New System.Windows.Forms.TextBox()
        Me.m_rbValue = New System.Windows.Forms.RadioButton()
        Me.rbCatch = New System.Windows.Forms.RadioButton()
        Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblBase = New System.Windows.Forms.Label()
        Me.m_hdrRun = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lbFleet = New System.Windows.Forms.Label()
        Me.m_lblIter = New System.Windows.Forms.Label()
        Me.m_lblEffort = New System.Windows.Forms.Label()
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.m_tlpControls = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plBase = New System.Windows.Forms.Panel()
        Me.m_plRun = New System.Windows.Forms.Panel()
        Me.m_plInfo = New System.Windows.Forms.Panel()
        Me.m_lblMSY = New System.Windows.Forms.Label()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tlpControls.SuspendLayout()
        Me.m_plBase.SuspendLayout()
        Me.m_plRun.SuspendLayout()
        Me.m_plInfo.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_btnRunMSY
        '
        resources.ApplyResources(Me.m_btnRunMSY, "m_btnRunMSY")
        Me.m_btnRunMSY.Name = "m_btnRunMSY"
        Me.m_btnRunMSY.UseVisualStyleBackColor = True
        '
        'm_btnStop
        '
        resources.ApplyResources(Me.m_btnStop, "m_btnStop")
        Me.m_btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnStop.Name = "m_btnStop"
        Me.m_btnStop.UseVisualStyleBackColor = True
        '
        'm_txtMSYresults
        '
        resources.ApplyResources(Me.m_txtMSYresults, "m_txtMSYresults")
        Me.m_txtMSYresults.Name = "m_txtMSYresults"
        '
        'm_rbValue
        '
        resources.ApplyResources(Me.m_rbValue, "m_rbValue")
        Me.m_rbValue.Name = "m_rbValue"
        Me.m_rbValue.TabStop = True
        Me.m_rbValue.UseVisualStyleBackColor = True
        '
        'rbCatch
        '
        resources.ApplyResources(Me.rbCatch, "rbCatch")
        Me.rbCatch.Name = "rbCatch"
        Me.rbCatch.TabStop = True
        Me.rbCatch.UseVisualStyleBackColor = True
        '
        'm_hdrOptions
        '
        Me.m_hdrOptions.CanCollapseParent = False
        Me.m_hdrOptions.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrOptions, "m_hdrOptions")
        Me.m_hdrOptions.IsCollapsed = False
        Me.m_hdrOptions.Name = "m_hdrOptions"
        '
        'm_lblBase
        '
        resources.ApplyResources(Me.m_lblBase, "m_lblBase")
        Me.m_lblBase.Name = "m_lblBase"
        '
        'm_hdrRun
        '
        Me.m_hdrRun.CanCollapseParent = False
        Me.m_hdrRun.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrRun, "m_hdrRun")
        Me.m_hdrRun.IsCollapsed = False
        Me.m_hdrRun.Name = "m_hdrRun"
        '
        'm_lbFleet
        '
        resources.ApplyResources(Me.m_lbFleet, "m_lbFleet")
        Me.m_lbFleet.Name = "m_lbFleet"
        '
        'm_lblIter
        '
        resources.ApplyResources(Me.m_lblIter, "m_lblIter")
        Me.m_lblIter.Name = "m_lblIter"
        '
        'm_lblEffort
        '
        resources.ApplyResources(Me.m_lblEffort, "m_lblEffort")
        Me.m_lblEffort.Name = "m_lblEffort"
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_tlpControls)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_txtMSYresults)
        '
        'm_tlpControls
        '
        resources.ApplyResources(Me.m_tlpControls, "m_tlpControls")
        Me.m_tlpControls.Controls.Add(Me.m_lblMSY, 0, 5)
        Me.m_tlpControls.Controls.Add(Me.m_hdrOptions, 0, 0)
        Me.m_tlpControls.Controls.Add(Me.m_plBase, 0, 1)
        Me.m_tlpControls.Controls.Add(Me.m_hdrRun, 0, 2)
        Me.m_tlpControls.Controls.Add(Me.m_plRun, 0, 3)
        Me.m_tlpControls.Controls.Add(Me.m_plInfo, 0, 4)
        Me.m_tlpControls.Name = "m_tlpControls"
        '
        'm_plBase
        '
        Me.m_plBase.Controls.Add(Me.m_lblBase)
        Me.m_plBase.Controls.Add(Me.m_rbValue)
        Me.m_plBase.Controls.Add(Me.rbCatch)
        resources.ApplyResources(Me.m_plBase, "m_plBase")
        Me.m_plBase.Name = "m_plBase"
        '
        'm_plRun
        '
        Me.m_plRun.Controls.Add(Me.m_btnRunMSY)
        Me.m_plRun.Controls.Add(Me.m_btnStop)
        resources.ApplyResources(Me.m_plRun, "m_plRun")
        Me.m_plRun.Name = "m_plRun"
        '
        'm_plInfo
        '
        Me.m_plInfo.Controls.Add(Me.m_lbFleet)
        Me.m_plInfo.Controls.Add(Me.m_lblEffort)
        Me.m_plInfo.Controls.Add(Me.m_lblIter)
        resources.ApplyResources(Me.m_plInfo, "m_plInfo")
        Me.m_plInfo.Name = "m_plInfo"
        '
        'm_lblMSY
        '
        resources.ApplyResources(Me.m_lblMSY, "m_lblMSY")
        Me.m_lblMSY.Name = "m_lblMSY"
        '
        'frmMSY
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.m_btnStop
        Me.Controls.Add(Me.m_scMain)
        Me.CoreExecutionState = EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        Me.Name = "frmMSY"
        Me.TabText = ""
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.Panel2.PerformLayout()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.m_tlpControls.ResumeLayout(False)
        Me.m_tlpControls.PerformLayout()
        Me.m_plBase.ResumeLayout(False)
        Me.m_plBase.PerformLayout()
        Me.m_plRun.ResumeLayout(False)
        Me.m_plInfo.ResumeLayout(False)
        Me.m_plInfo.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rbCatch As System.Windows.Forms.RadioButton
    Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_rbValue As System.Windows.Forms.RadioButton
    Private WithEvents m_lblBase As System.Windows.Forms.Label
    Private WithEvents m_btnRunMSY As System.Windows.Forms.Button
    Private WithEvents m_btnStop As System.Windows.Forms.Button
    Private WithEvents m_hdrRun As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lbFleet As System.Windows.Forms.Label
    Private WithEvents m_lblIter As System.Windows.Forms.Label
    Private WithEvents m_lblEffort As System.Windows.Forms.Label
    Private WithEvents m_txtMSYresults As TextBox
    Private WithEvents m_tlpControls As TableLayoutPanel
    Private WithEvents m_plBase As Panel
    Private WithEvents m_plRun As Panel
    Private WithEvents m_lblMSY As Label
    Private WithEvents m_plInfo As Panel
    Private WithEvents m_scMain As SplitContainer
End Class
