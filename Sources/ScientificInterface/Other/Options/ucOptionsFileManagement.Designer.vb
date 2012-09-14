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
            Me.SuspendLayout()
            '
            'm_cbEcosimRun
            '
            Me.m_cbEcosimRun.AutoSize = True
            Me.m_cbEcosimRun.Location = New System.Drawing.Point(41, 204)
            Me.m_cbEcosimRun.Name = "m_cbEcosimRun"
            Me.m_cbEcosimRun.Size = New System.Drawing.Size(79, 17)
            Me.m_cbEcosimRun.TabIndex = 12
            Me.m_cbEcosimRun.Text = "&Run results"
            Me.m_cbEcosimRun.UseVisualStyleBackColor = True
            '
            'm_cbMonteCarlo
            '
            Me.m_cbMonteCarlo.AutoSize = True
            Me.m_cbMonteCarlo.Location = New System.Drawing.Point(41, 227)
            Me.m_cbMonteCarlo.Name = "m_cbMonteCarlo"
            Me.m_cbMonteCarlo.Size = New System.Drawing.Size(83, 17)
            Me.m_cbMonteCarlo.TabIndex = 14
            Me.m_cbMonteCarlo.Text = "&Monte Carlo"
            Me.m_cbMonteCarlo.UseVisualStyleBackColor = True
            '
            'm_cbAutosaveAll
            '
            Me.m_cbAutosaveAll.AutoSize = True
            Me.m_cbAutosaveAll.Location = New System.Drawing.Point(6, 158)
            Me.m_cbAutosaveAll.Name = "m_cbAutosaveAll"
            Me.m_cbAutosaveAll.Size = New System.Drawing.Size(87, 17)
            Me.m_cbAutosaveAll.TabIndex = 10
            Me.m_cbAutosaveAll.Text = "&Auto-save all"
            Me.m_cbAutosaveAll.UseVisualStyleBackColor = True
            '
            'm_cbEcospace
            '
            Me.m_cbEcospace.AutoSize = True
            Me.m_cbEcospace.Location = New System.Drawing.Point(23, 278)
            Me.m_cbEcospace.Name = "m_cbEcospace"
            Me.m_cbEcospace.Size = New System.Drawing.Size(74, 17)
            Me.m_cbEcospace.TabIndex = 18
            Me.m_cbEcospace.Text = "&Ecospace"
            Me.m_cbEcospace.UseVisualStyleBackColor = True
            '
            'm_cbSpaceASCII
            '
            Me.m_cbSpaceASCII.AutoSize = True
            Me.m_cbSpaceASCII.Location = New System.Drawing.Point(41, 301)
            Me.m_cbSpaceASCII.Name = "m_cbSpaceASCII"
            Me.m_cbSpaceASCII.Size = New System.Drawing.Size(81, 17)
            Me.m_cbSpaceASCII.TabIndex = 19
            Me.m_cbSpaceASCII.Text = "ASCII &maps"
            Me.m_cbSpaceASCII.UseVisualStyleBackColor = True
            '
            'm_cbSpaceCSV
            '
            Me.m_cbSpaceCSV.AutoSize = True
            Me.m_cbSpaceCSV.Location = New System.Drawing.Point(41, 324)
            Me.m_cbSpaceCSV.Name = "m_cbSpaceCSV"
            Me.m_cbSpaceCSV.Size = New System.Drawing.Size(75, 17)
            Me.m_cbSpaceCSV.TabIndex = 21
            Me.m_cbSpaceCSV.Text = "&CSV maps"
            Me.m_cbSpaceCSV.UseVisualStyleBackColor = True
            '
            'm_cbEcosim
            '
            Me.m_cbEcosim.AutoSize = True
            Me.m_cbEcosim.Location = New System.Drawing.Point(23, 181)
            Me.m_cbEcosim.Name = "m_cbEcosim"
            Me.m_cbEcosim.Size = New System.Drawing.Size(60, 17)
            Me.m_cbEcosim.TabIndex = 11
            Me.m_cbEcosim.Text = "&Ecosim"
            Me.m_cbEcosim.UseVisualStyleBackColor = True
            '
            'm_cbMSE
            '
            Me.m_cbMSE.AutoSize = True
            Me.m_cbMSE.Location = New System.Drawing.Point(41, 250)
            Me.m_cbMSE.Name = "m_cbMSE"
            Me.m_cbMSE.Size = New System.Drawing.Size(49, 17)
            Me.m_cbMSE.TabIndex = 16
            Me.m_cbMSE.Text = "&MSE"
            Me.m_cbMSE.UseVisualStyleBackColor = True
            '
            'm_cbEcotracer
            '
            Me.m_cbEcotracer.AutoSize = True
            Me.m_cbEcotracer.Location = New System.Drawing.Point(23, 347)
            Me.m_cbEcotracer.Name = "m_cbEcotracer"
            Me.m_cbEcotracer.Size = New System.Drawing.Size(72, 17)
            Me.m_cbEcotracer.TabIndex = 23
            Me.m_cbEcotracer.Text = "&Ecotracer"
            Me.m_cbEcotracer.UseVisualStyleBackColor = True
            '
            'm_tbxEcosim
            '
            Me.m_tbxEcosim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxEcosim.Location = New System.Drawing.Point(158, 202)
            Me.m_tbxEcosim.Name = "m_tbxEcosim"
            Me.m_tbxEcosim.ReadOnly = True
            Me.m_tbxEcosim.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxEcosim.TabIndex = 13
            Me.m_tbxEcosim.TabStop = False
            '
            'm_tbxMC
            '
            Me.m_tbxMC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxMC.Location = New System.Drawing.Point(158, 225)
            Me.m_tbxMC.Name = "m_tbxMC"
            Me.m_tbxMC.ReadOnly = True
            Me.m_tbxMC.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxMC.TabIndex = 15
            Me.m_tbxMC.TabStop = False
            '
            'm_tbxMSE
            '
            Me.m_tbxMSE.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxMSE.Location = New System.Drawing.Point(158, 248)
            Me.m_tbxMSE.Name = "m_tbxMSE"
            Me.m_tbxMSE.ReadOnly = True
            Me.m_tbxMSE.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxMSE.TabIndex = 17
            Me.m_tbxMSE.TabStop = False
            '
            'm_tbxASCII
            '
            Me.m_tbxASCII.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxASCII.Location = New System.Drawing.Point(158, 299)
            Me.m_tbxASCII.Name = "m_tbxASCII"
            Me.m_tbxASCII.ReadOnly = True
            Me.m_tbxASCII.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxASCII.TabIndex = 20
            Me.m_tbxASCII.TabStop = False
            '
            'm_tbxCSV
            '
            Me.m_tbxCSV.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxCSV.Location = New System.Drawing.Point(158, 322)
            Me.m_tbxCSV.Name = "m_tbxCSV"
            Me.m_tbxCSV.ReadOnly = True
            Me.m_tbxCSV.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxCSV.TabIndex = 22
            Me.m_tbxCSV.TabStop = False
            '
            'm_hdrAutosave
            '
            Me.m_hdrAutosave.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrAutosave.CanCollapseParent = False
            Me.m_hdrAutosave.CollapsedParentHeight = 0
            Me.m_hdrAutosave.IsCollapsed = False
            Me.m_hdrAutosave.Location = New System.Drawing.Point(0, 131)
            Me.m_hdrAutosave.Name = "m_hdrAutosave"
            Me.m_hdrAutosave.Size = New System.Drawing.Size(553, 18)
            Me.m_hdrAutosave.TabIndex = 9
            Me.m_hdrAutosave.Text = "Auto-save results"
            Me.m_hdrAutosave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tbxTracer
            '
            Me.m_tbxTracer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxTracer.Location = New System.Drawing.Point(158, 345)
            Me.m_tbxTracer.Name = "m_tbxTracer"
            Me.m_tbxTracer.ReadOnly = True
            Me.m_tbxTracer.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxTracer.TabIndex = 24
            Me.m_tbxTracer.TabStop = False
            '
            'm_fieldpickBackup
            '
            Me.m_fieldpickBackup.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_fieldpickBackup.Fields = Nothing
            Me.m_fieldpickBackup.Label = "Fields"
            Me.m_fieldpickBackup.Location = New System.Drawing.Point(481, 74)
            Me.m_fieldpickBackup.Name = "m_fieldpickBackup"
            Me.m_fieldpickBackup.ShowDirectoryPicker = True
            Me.m_fieldpickBackup.Size = New System.Drawing.Size(75, 21)
            Me.m_fieldpickBackup.TabIndex = 7
            Me.m_fieldpickBackup.TypeFormatter = Nothing
            Me.m_fieldpickBackup.UIContext = Nothing
            '
            'm_fieldpickOutput
            '
            Me.m_fieldpickOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_fieldpickOutput.Fields = Nothing
            Me.m_fieldpickOutput.Label = "Fields"
            Me.m_fieldpickOutput.Location = New System.Drawing.Point(481, 21)
            Me.m_fieldpickOutput.Name = "m_fieldpickOutput"
            Me.m_fieldpickOutput.ShowDirectoryPicker = True
            Me.m_fieldpickOutput.Size = New System.Drawing.Size(72, 21)
            Me.m_fieldpickOutput.TabIndex = 3
            Me.m_fieldpickOutput.TypeFormatter = Nothing
            Me.m_fieldpickOutput.UIContext = Nothing
            '
            'm_tbBackupMask
            '
            Me.m_tbBackupMask.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbBackupMask.Location = New System.Drawing.Point(158, 74)
            Me.m_tbBackupMask.Name = "m_tbBackupMask"
            Me.m_tbBackupMask.Size = New System.Drawing.Size(317, 20)
            Me.m_tbBackupMask.TabIndex = 6
            '
            'm_lblBackupFolder
            '
            Me.m_lblBackupFolder.AutoSize = True
            Me.m_lblBackupFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblBackupFolder.Location = New System.Drawing.Point(3, 77)
            Me.m_lblBackupFolder.Name = "m_lblBackupFolder"
            Me.m_lblBackupFolder.Size = New System.Drawing.Size(100, 13)
            Me.m_lblBackupFolder.TabIndex = 5
            Me.m_lblBackupFolder.Text = "&Back-up models as:"
            '
            'm_tbOutputMask
            '
            Me.m_tbOutputMask.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbOutputMask.Location = New System.Drawing.Point(158, 21)
            Me.m_tbOutputMask.Name = "m_tbOutputMask"
            Me.m_tbOutputMask.Size = New System.Drawing.Size(321, 20)
            Me.m_tbOutputMask.TabIndex = 2
            '
            'm_lblOutput
            '
            Me.m_lblOutput.AutoSize = True
            Me.m_lblOutput.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblOutput.Location = New System.Drawing.Point(3, 24)
            Me.m_lblOutput.Name = "m_lblOutput"
            Me.m_lblOutput.Size = New System.Drawing.Size(82, 13)
            Me.m_lblOutput.TabIndex = 1
            Me.m_lblOutput.Text = "&Output location:"
            '
            'CEwEHeaderLabel1
            '
            Me.CEwEHeaderLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CEwEHeaderLabel1.CanCollapseParent = False
            Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
            Me.CEwEHeaderLabel1.IsCollapsed = False
            Me.CEwEHeaderLabel1.Location = New System.Drawing.Point(0, 0)
            Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
            Me.CEwEHeaderLabel1.Size = New System.Drawing.Size(553, 18)
            Me.CEwEHeaderLabel1.TabIndex = 0
            Me.CEwEHeaderLabel1.Text = "File management options"
            Me.CEwEHeaderLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tbxOutputSample
            '
            Me.m_tbxOutputSample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxOutputSample.Location = New System.Drawing.Point(158, 47)
            Me.m_tbxOutputSample.Name = "m_tbxOutputSample"
            Me.m_tbxOutputSample.ReadOnly = True
            Me.m_tbxOutputSample.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxOutputSample.TabIndex = 4
            '
            'm_tbxBackupSample
            '
            Me.m_tbxBackupSample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxBackupSample.Location = New System.Drawing.Point(158, 100)
            Me.m_tbxBackupSample.Name = "m_tbxBackupSample"
            Me.m_tbxBackupSample.ReadOnly = True
            Me.m_tbxBackupSample.Size = New System.Drawing.Size(395, 20)
            Me.m_tbxBackupSample.TabIndex = 8
            '
            'ucOptionsAutosave
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
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
            Me.Controls.Add(Me.m_tbxMSE)
            Me.Controls.Add(Me.m_tbxMC)
            Me.Controls.Add(Me.m_tbxBackupSample)
            Me.Controls.Add(Me.m_tbxOutputSample)
            Me.Controls.Add(Me.m_tbxEcosim)
            Me.Controls.Add(Me.m_cbEcotracer)
            Me.Controls.Add(Me.m_cbEcospace)
            Me.Controls.Add(Me.m_cbEcosim)
            Me.Controls.Add(Me.m_cbAutosaveAll)
            Me.Controls.Add(Me.m_cbMSE)
            Me.Controls.Add(Me.m_cbMonteCarlo)
            Me.Controls.Add(Me.m_cbSpaceCSV)
            Me.Controls.Add(Me.m_cbSpaceASCII)
            Me.Controls.Add(Me.m_cbEcosimRun)
            Me.Controls.Add(Me.CEwEHeaderLabel1)
            Me.Controls.Add(Me.m_hdrAutosave)
            Me.Name = "ucOptionsAutosave"
            Me.Size = New System.Drawing.Size(553, 370)
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

    End Class

End Namespace
