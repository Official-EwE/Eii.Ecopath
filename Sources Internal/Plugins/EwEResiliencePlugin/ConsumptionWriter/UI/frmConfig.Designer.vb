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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Me.m_cbIncludeDetritus = New System.Windows.Forms.CheckBox()
        Me.m_cbIncludeImportAndSum = New System.Windows.Forms.CheckBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        Me.m_ack = New EwEResiliencePlugin.ucAcknowledgements()
        Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.SuspendLayout()
        '
        'm_cbIncludeDetritus
        '
        resources.ApplyResources(Me.m_cbIncludeDetritus, "m_cbIncludeDetritus")
        Me.m_cbIncludeDetritus.Name = "m_cbIncludeDetritus"
        Me.m_cbIncludeDetritus.UseVisualStyleBackColor = True
        '
        'm_cbIncludeImportAndSum
        '
        resources.ApplyResources(Me.m_cbIncludeImportAndSum, "m_cbIncludeImportAndSum")
        Me.m_cbIncludeImportAndSum.Name = "m_cbIncludeImportAndSum"
        Me.m_cbIncludeImportAndSum.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_cbAutosave
        '
        resources.ApplyResources(Me.m_cbAutosave, "m_cbAutosave")
        Me.m_cbAutosave.Name = "m_cbAutosave"
        Me.m_cbAutosave.UseVisualStyleBackColor = True
        '
        'm_ack
        '
        resources.ApplyResources(Me.m_ack, "m_ack")
        Me.m_ack.Name = "m_ack"
        Me.m_ack.UIContext = Nothing
        '
        'm_hdrOptions
        '
        resources.ApplyResources(Me.m_hdrOptions, "m_hdrOptions")
        Me.m_hdrOptions.CanCollapseParent = False
        Me.m_hdrOptions.CollapsedParentHeight = 0
        Me.m_hdrOptions.IsCollapsed = False
        Me.m_hdrOptions.Name = "m_hdrOptions"
        '
        'frmConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrOptions)
        Me.Controls.Add(Me.m_ack)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_cbIncludeImportAndSum)
        Me.Controls.Add(Me.m_cbAutosave)
        Me.Controls.Add(Me.m_cbIncludeDetritus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "frmConfig"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_cbIncludeImportAndSum As System.Windows.Forms.CheckBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_cbIncludeDetritus As System.Windows.Forms.CheckBox
    Private WithEvents m_cbAutosave As System.Windows.Forms.CheckBox
    Private WithEvents m_ack As EwEResiliencePlugin.ucAcknowledgements
    Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
