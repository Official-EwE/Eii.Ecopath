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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEcospaceValidation
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceValidation))
        Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plControl = New System.Windows.Forms.Panel()
        Me.m_grid = New gridPredPreyOverlap()
        Me.m_lblTimeStep = New System.Windows.Forms.Label()
        Me.m_nudTimeStep = New System.Windows.Forms.NumericUpDown()
        Me.m_slTimestep = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_tlpMain.SuspendLayout()
        Me.m_plControl.SuspendLayout()
        CType(Me.m_nudTimeStep, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tlpMain
        '
        resources.ApplyResources(Me.m_tlpMain, "m_tlpMain")
        Me.m_tlpMain.Controls.Add(Me.m_plControl, 0, 0)
        Me.m_tlpMain.Controls.Add(Me.m_grid, 0, 1)
        Me.m_tlpMain.Name = "m_tlpMain"
        '
        'm_plControl
        '
        Me.m_plControl.Controls.Add(Me.m_slTimestep)
        Me.m_plControl.Controls.Add(Me.m_nudTimeStep)
        Me.m_plControl.Controls.Add(Me.m_lblTimeStep)
        resources.ApplyResources(Me.m_plControl, "m_plControl")
        Me.m_plControl.Name = "m_plControl"
        '
        'm_grid
        '
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.Name = "m_grid"
        '
        'm_lblTimeStep
        '
        resources.ApplyResources(Me.m_lblTimeStep, "m_lblTimeStep")
        Me.m_lblTimeStep.Name = "m_lblTimeStep"
        '
        'm_nudTimeStep
        '
        resources.ApplyResources(Me.m_nudTimeStep, "m_nudTimeStep")
        Me.m_nudTimeStep.Name = "m_nudTimeStep"
        '
        'm_slTimestep
        '
        resources.ApplyResources(Me.m_slTimestep, "m_slTimestep")
        Me.m_slTimestep.CurrentKnob = 0
        Me.m_slTimestep.Maximum = 100
        Me.m_slTimestep.Minimum = 0
        Me.m_slTimestep.Name = "m_slTimestep"
        Me.m_slTimestep.NumKnobs = 1
        '
        'frmEcospaceValidation
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmEcospaceValidation"
        Me.m_tlpMain.ResumeLayout(False)
        Me.m_plControl.ResumeLayout(False)
        Me.m_plControl.PerformLayout()
        CType(Me.m_nudTimeStep, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tlpMain As Windows.Forms.TableLayoutPanel
    Private WithEvents m_plControl As Windows.Forms.Panel
    Private WithEvents m_slTimestep As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_nudTimeStep As Windows.Forms.NumericUpDown
    Private WithEvents m_lblTimeStep As Windows.Forms.Label
    Private WithEvents m_grid As gridPredPreyOverlap

End Class
