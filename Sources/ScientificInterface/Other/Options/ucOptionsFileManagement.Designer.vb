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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
            Me.m_hdrAutosave = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_fieldpickBackup = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_fieldpickOutput = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_tbBackupMask = New System.Windows.Forms.TextBox()
            Me.m_lblBackupFolder = New System.Windows.Forms.Label()
            Me.m_tbOutputMask = New System.Windows.Forms.TextBox()
            Me.m_lblOutput = New System.Windows.Forms.Label()
            Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tbxOutputSample = New System.Windows.Forms.TextBox()
            Me.m_tbxBackupSample = New System.Windows.Forms.TextBox()
            Me.m_plAutoSave = New System.Windows.Forms.Panel()
            Me.SuspendLayout()
            '
            'm_hdrAutosave
            '
            resources.ApplyResources(Me.m_hdrAutosave, "m_hdrAutosave")
            Me.m_hdrAutosave.CanCollapseParent = False
            Me.m_hdrAutosave.CollapsedParentHeight = 0
            Me.m_hdrAutosave.IsCollapsed = False
            Me.m_hdrAutosave.Name = "m_hdrAutosave"
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
            'm_plAutoSave
            '
            resources.ApplyResources(Me.m_plAutoSave, "m_plAutoSave")
            Me.m_plAutoSave.Name = "m_plAutoSave"
            '
            'ucOptionsFileManagement
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_plAutoSave)
            Me.Controls.Add(Me.m_fieldpickBackup)
            Me.Controls.Add(Me.m_fieldpickOutput)
            Me.Controls.Add(Me.m_tbBackupMask)
            Me.Controls.Add(Me.m_lblBackupFolder)
            Me.Controls.Add(Me.m_tbOutputMask)
            Me.Controls.Add(Me.m_lblOutput)
            Me.Controls.Add(Me.m_tbxBackupSample)
            Me.Controls.Add(Me.m_tbxOutputSample)
            Me.Controls.Add(Me.CEwEHeaderLabel1)
            Me.Controls.Add(Me.m_hdrAutosave)
            Me.Name = "ucOptionsFileManagement"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_hdrAutosave As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_fieldpickBackup As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_fieldpickOutput As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_tbBackupMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblBackupFolder As System.Windows.Forms.Label
        Private WithEvents m_tbOutputMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblOutput As System.Windows.Forms.Label
        Private WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_tbxOutputSample As System.Windows.Forms.TextBox
        Private WithEvents m_tbxBackupSample As System.Windows.Forms.TextBox
        Private WithEvents m_plAutoSave As System.Windows.Forms.Panel

    End Class

End Namespace
