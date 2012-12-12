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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Namespace Other

    Partial Class ucOptionsFileManagement
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsFileManagement))
            Me.m_cbEcosimRun = New System.Windows.Forms.CheckBox()
            Me.m_cbMonteCarlo = New System.Windows.Forms.CheckBox()
            Me.m_cbAutosaveAll = New System.Windows.Forms.CheckBox()
            Me.m_cbEcospace = New System.Windows.Forms.CheckBox()
            Me.m_cbSpaceASCII = New System.Windows.Forms.CheckBox()
            Me.m_cbSpaceCSV = New System.Windows.Forms.CheckBox()
            Me.m_cbEcosim = New System.Windows.Forms.CheckBox()
            Me.m_cbMSE = New System.Windows.Forms.CheckBox()
            Me.m_cbEcotracer = New System.Windows.Forms.CheckBox()
            Me.m_tbxEcosim = New System.Windows.Forms.TextBox()
            Me.m_tbxMC = New System.Windows.Forms.TextBox()
            Me.m_tbxMSE = New System.Windows.Forms.TextBox()
            Me.m_tbxASCII = New System.Windows.Forms.TextBox()
            Me.m_tbxCSV = New System.Windows.Forms.TextBox()
            Me.m_hdrAutosave = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tbxTracer = New System.Windows.Forms.TextBox()
            Me.m_fieldpickBackup = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_fieldpickOutput = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_tbBackupMask = New System.Windows.Forms.TextBox()
            Me.m_lblBackupFolder = New System.Windows.Forms.Label()
            Me.m_tbOutputMask = New System.Windows.Forms.TextBox()
            Me.m_lblOutput = New System.Windows.Forms.Label()
            Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tbxOutputSample = New System.Windows.Forms.TextBox()
            Me.m_tbxBackupSample = New System.Windows.Forms.TextBox()
            Me.m_cbMSY = New System.Windows.Forms.CheckBox()
            Me.m_tbxMSY = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'm_cbEcosimRun
            '
            resources.ApplyResources(Me.m_cbEcosimRun, "m_cbEcosimRun")
            Me.m_cbEcosimRun.Name = "m_cbEcosimRun"
            Me.m_cbEcosimRun.UseVisualStyleBackColor = True
            '
            'm_cbMonteCarlo
            '
            resources.ApplyResources(Me.m_cbMonteCarlo, "m_cbMonteCarlo")
            Me.m_cbMonteCarlo.Name = "m_cbMonteCarlo"
            Me.m_cbMonteCarlo.UseVisualStyleBackColor = True
            '
            'm_cbAutosaveAll
            '
            resources.ApplyResources(Me.m_cbAutosaveAll, "m_cbAutosaveAll")
            Me.m_cbAutosaveAll.Name = "m_cbAutosaveAll"
            Me.m_cbAutosaveAll.UseVisualStyleBackColor = True
            '
            'm_cbEcospace
            '
            resources.ApplyResources(Me.m_cbEcospace, "m_cbEcospace")
            Me.m_cbEcospace.Name = "m_cbEcospace"
            Me.m_cbEcospace.UseVisualStyleBackColor = True
            '
            'm_cbSpaceASCII
            '
            resources.ApplyResources(Me.m_cbSpaceASCII, "m_cbSpaceASCII")
            Me.m_cbSpaceASCII.Name = "m_cbSpaceASCII"
            Me.m_cbSpaceASCII.UseVisualStyleBackColor = True
            '
            'm_cbSpaceCSV
            '
            resources.ApplyResources(Me.m_cbSpaceCSV, "m_cbSpaceCSV")
            Me.m_cbSpaceCSV.Name = "m_cbSpaceCSV"
            Me.m_cbSpaceCSV.UseVisualStyleBackColor = True
            '
            'm_cbEcosim
            '
            resources.ApplyResources(Me.m_cbEcosim, "m_cbEcosim")
            Me.m_cbEcosim.Name = "m_cbEcosim"
            Me.m_cbEcosim.UseVisualStyleBackColor = True
            '
            'm_cbMSE
            '
            resources.ApplyResources(Me.m_cbMSE, "m_cbMSE")
            Me.m_cbMSE.Name = "m_cbMSE"
            Me.m_cbMSE.UseVisualStyleBackColor = True
            '
            'm_cbEcotracer
            '
            resources.ApplyResources(Me.m_cbEcotracer, "m_cbEcotracer")
            Me.m_cbEcotracer.Name = "m_cbEcotracer"
            Me.m_cbEcotracer.UseVisualStyleBackColor = True
            '
            'm_tbxEcosim
            '
            resources.ApplyResources(Me.m_tbxEcosim, "m_tbxEcosim")
            Me.m_tbxEcosim.Name = "m_tbxEcosim"
            Me.m_tbxEcosim.ReadOnly = True
            Me.m_tbxEcosim.TabStop = False
            '
            'm_tbxMC
            '
            resources.ApplyResources(Me.m_tbxMC, "m_tbxMC")
            Me.m_tbxMC.Name = "m_tbxMC"
            Me.m_tbxMC.ReadOnly = True
            Me.m_tbxMC.TabStop = False
            '
            'm_tbxMSE
            '
            resources.ApplyResources(Me.m_tbxMSE, "m_tbxMSE")
            Me.m_tbxMSE.Name = "m_tbxMSE"
            Me.m_tbxMSE.ReadOnly = True
            Me.m_tbxMSE.TabStop = False
            '
            'm_tbxASCII
            '
            resources.ApplyResources(Me.m_tbxASCII, "m_tbxASCII")
            Me.m_tbxASCII.Name = "m_tbxASCII"
            Me.m_tbxASCII.ReadOnly = True
            Me.m_tbxASCII.TabStop = False
            '
            'm_tbxCSV
            '
            resources.ApplyResources(Me.m_tbxCSV, "m_tbxCSV")
            Me.m_tbxCSV.Name = "m_tbxCSV"
            Me.m_tbxCSV.ReadOnly = True
            Me.m_tbxCSV.TabStop = False
            '
            'm_hdrAutosave
            '
            resources.ApplyResources(Me.m_hdrAutosave, "m_hdrAutosave")
            Me.m_hdrAutosave.CanCollapseParent = False
            Me.m_hdrAutosave.CollapsedParentHeight = 0
            Me.m_hdrAutosave.IsCollapsed = False
            Me.m_hdrAutosave.Name = "m_hdrAutosave"
            '
            'm_tbxTracer
            '
            resources.ApplyResources(Me.m_tbxTracer, "m_tbxTracer")
            Me.m_tbxTracer.Name = "m_tbxTracer"
            Me.m_tbxTracer.ReadOnly = True
            Me.m_tbxTracer.TabStop = False
            '
            'm_fieldpickBackup
            '
            resources.ApplyResources(Me.m_fieldpickBackup, "m_fieldpickBackup")
            Me.m_fieldpickBackup.Fields = Nothing
            Me.m_fieldpickBackup.Label = "Fields"
            Me.m_fieldpickBackup.Name = "m_fieldpickBackup"
            Me.m_fieldpickBackup.ShowDirectoryPicker = True
            Me.m_fieldpickBackup.TypeFormatter = Nothing
            Me.m_fieldpickBackup.UIContext = Nothing
            '
            'm_fieldpickOutput
            '
            resources.ApplyResources(Me.m_fieldpickOutput, "m_fieldpickOutput")
            Me.m_fieldpickOutput.Fields = Nothing
            Me.m_fieldpickOutput.Label = "Fields"
            Me.m_fieldpickOutput.Name = "m_fieldpickOutput"
            Me.m_fieldpickOutput.ShowDirectoryPicker = True
            Me.m_fieldpickOutput.TypeFormatter = Nothing
            Me.m_fieldpickOutput.UIContext = Nothing
            '
            'm_tbBackupMask
            '
            resources.ApplyResources(Me.m_tbBackupMask, "m_tbBackupMask")
            Me.m_tbBackupMask.Name = "m_tbBackupMask"
            '
            'm_lblBackupFolder
            '
            resources.ApplyResources(Me.m_lblBackupFolder, "m_lblBackupFolder")
            Me.m_lblBackupFolder.Name = "m_lblBackupFolder"
            '
            'm_tbOutputMask
            '
            resources.ApplyResources(Me.m_tbOutputMask, "m_tbOutputMask")
            Me.m_tbOutputMask.Name = "m_tbOutputMask"
            '
            'm_lblOutput
            '
            resources.ApplyResources(Me.m_lblOutput, "m_lblOutput")
            Me.m_lblOutput.Name = "m_lblOutput"
            '
            'CEwEHeaderLabel1
            '
            resources.ApplyResources(Me.CEwEHeaderLabel1, "CEwEHeaderLabel1")
            Me.CEwEHeaderLabel1.CanCollapseParent = False
            Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
            Me.CEwEHeaderLabel1.IsCollapsed = False
            Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
            '
            'm_tbxOutputSample
            '
            resources.ApplyResources(Me.m_tbxOutputSample, "m_tbxOutputSample")
            Me.m_tbxOutputSample.Name = "m_tbxOutputSample"
            Me.m_tbxOutputSample.ReadOnly = True
            '
            'm_tbxBackupSample
            '
            resources.ApplyResources(Me.m_tbxBackupSample, "m_tbxBackupSample")
            Me.m_tbxBackupSample.Name = "m_tbxBackupSample"
            Me.m_tbxBackupSample.ReadOnly = True
            '
            'm_cbMSY
            '
            resources.ApplyResources(Me.m_cbMSY, "m_cbMSY")
            Me.m_cbMSY.Name = "m_cbMSY"
            Me.m_cbMSY.UseVisualStyleBackColor = True
            '
            'm_tbxMSY
            '
            resources.ApplyResources(Me.m_tbxMSY, "m_tbxMSY")
            Me.m_tbxMSY.Name = "m_tbxMSY"
            Me.m_tbxMSY.ReadOnly = True
            Me.m_tbxMSY.TabStop = False
            '
            'ucOptionsFileManagement
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_fieldpickBackup)
            Me.Controls.Add(Me.m_fieldpickOutput)
            Me.Controls.Add(Me.m_tbBackupMask)
            Me.Controls.Add(Me.m_lblBackupFolder)
            Me.Controls.Add(Me.m_tbOutputMask)
            Me.Controls.Add(Me.m_lblOutput)
            Me.Controls.Add(Me.m_tbxTracer)
            Me.Controls.Add(Me.m_tbxCSV)
            Me.Controls.Add(Me.m_tbxASCII)
            Me.Controls.Add(Me.m_tbxMSY)
            Me.Controls.Add(Me.m_tbxMSE)
            Me.Controls.Add(Me.m_tbxMC)
            Me.Controls.Add(Me.m_tbxBackupSample)
            Me.Controls.Add(Me.m_tbxOutputSample)
            Me.Controls.Add(Me.m_tbxEcosim)
            Me.Controls.Add(Me.m_cbEcotracer)
            Me.Controls.Add(Me.m_cbEcospace)
            Me.Controls.Add(Me.m_cbEcosim)
            Me.Controls.Add(Me.m_cbMSY)
            Me.Controls.Add(Me.m_cbAutosaveAll)
            Me.Controls.Add(Me.m_cbMSE)
            Me.Controls.Add(Me.m_cbMonteCarlo)
            Me.Controls.Add(Me.m_cbSpaceCSV)
            Me.Controls.Add(Me.m_cbSpaceASCII)
            Me.Controls.Add(Me.m_cbEcosimRun)
            Me.Controls.Add(Me.CEwEHeaderLabel1)
            Me.Controls.Add(Me.m_hdrAutosave)
            Me.Name = "ucOptionsFileManagement"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cbEcosimRun As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrAutosave As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_cbMonteCarlo As System.Windows.Forms.CheckBox
        Private WithEvents m_cbAutosaveAll As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcospace As System.Windows.Forms.CheckBox
        Private WithEvents m_cbSpaceASCII As System.Windows.Forms.CheckBox
        Private WithEvents m_cbSpaceCSV As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcosim As System.Windows.Forms.CheckBox
        Private WithEvents m_cbMSE As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcotracer As System.Windows.Forms.CheckBox
        Private WithEvents m_tbxEcosim As System.Windows.Forms.TextBox
        Private WithEvents m_tbxMC As System.Windows.Forms.TextBox
        Private WithEvents m_tbxMSE As System.Windows.Forms.TextBox
        Private WithEvents m_tbxASCII As System.Windows.Forms.TextBox
        Private WithEvents m_tbxCSV As System.Windows.Forms.TextBox
        Private WithEvents m_tbxTracer As System.Windows.Forms.TextBox
        Private WithEvents m_fieldpickBackup As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_fieldpickOutput As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_tbBackupMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblBackupFolder As System.Windows.Forms.Label
        Private WithEvents m_tbOutputMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblOutput As System.Windows.Forms.Label
        Private WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_tbxOutputSample As System.Windows.Forms.TextBox
        Private WithEvents m_tbxBackupSample As System.Windows.Forms.TextBox
        Private WithEvents m_cbMSY As System.Windows.Forms.CheckBox
        Private WithEvents m_tbxMSY As System.Windows.Forms.TextBox

    End Class

End Namespace
