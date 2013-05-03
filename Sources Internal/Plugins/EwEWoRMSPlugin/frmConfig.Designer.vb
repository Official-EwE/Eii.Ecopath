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
        Me.m_plLogo = New System.Windows.Forms.Panel()
        Me.m_lblConnTO = New System.Windows.Forms.Label()
        Me.m_nudConnTO = New System.Windows.Forms.NumericUpDown()
        Me.m_lblSecs1 = New System.Windows.Forms.Label()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_lblReplyTO = New System.Windows.Forms.Label()
        Me.m_lblSecs2 = New System.Windows.Forms.Label()
        Me.m_nudReplyTO = New System.Windows.Forms.NumericUpDown()
        CType(Me.m_nudConnTO, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudReplyTO, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_plLogo
        '
        resources.ApplyResources(Me.m_plLogo, "m_plLogo")
        Me.m_plLogo.BackColor = System.Drawing.Color.White
        Me.m_plLogo.Name = "m_plLogo"
        '
        'm_lblConnTO
        '
        resources.ApplyResources(Me.m_lblConnTO, "m_lblConnTO")
        Me.m_lblConnTO.Name = "m_lblConnTO"
        '
        'm_nudConnTO
        '
        resources.ApplyResources(Me.m_nudConnTO, "m_nudConnTO")
        Me.m_nudConnTO.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.m_nudConnTO.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.m_nudConnTO.Name = "m_nudConnTO"
        Me.m_nudConnTO.Value = New Decimal(New Integer() {60, 0, 0, 0})
        '
        'm_lblSecs1
        '
        resources.ApplyResources(Me.m_lblSecs1, "m_lblSecs1")
        Me.m_lblSecs1.Name = "m_lblSecs1"
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
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_lblReplyTO
        '
        resources.ApplyResources(Me.m_lblReplyTO, "m_lblReplyTO")
        Me.m_lblReplyTO.Name = "m_lblReplyTO"
        '
        'm_lblSecs2
        '
        resources.ApplyResources(Me.m_lblSecs2, "m_lblSecs2")
        Me.m_lblSecs2.Name = "m_lblSecs2"
        '
        'm_nudReplyTO
        '
        resources.ApplyResources(Me.m_nudReplyTO, "m_nudReplyTO")
        Me.m_nudReplyTO.Maximum = New Decimal(New Integer() {600, 0, 0, 0})
        Me.m_nudReplyTO.Minimum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.m_nudReplyTO.Name = "m_nudReplyTO"
        Me.m_nudReplyTO.Value = New Decimal(New Integer() {300, 0, 0, 0})
        '
        'frmConfig
        '
        Me.AcceptButton = Me.m_btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_nudReplyTO)
        Me.Controls.Add(Me.m_lblSecs2)
        Me.Controls.Add(Me.m_nudConnTO)
        Me.Controls.Add(Me.m_lblReplyTO)
        Me.Controls.Add(Me.m_lblSecs1)
        Me.Controls.Add(Me.m_lblConnTO)
        Me.Controls.Add(Me.m_plLogo)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConfig"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.m_nudConnTO, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudReplyTO, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_plLogo As System.Windows.Forms.Panel
    Private WithEvents m_lblConnTO As System.Windows.Forms.Label
    Private WithEvents m_nudConnTO As System.Windows.Forms.NumericUpDown
    Private WithEvents m_lblSecs1 As System.Windows.Forms.Label
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_lblReplyTO As System.Windows.Forms.Label
    Private WithEvents m_lblSecs2 As System.Windows.Forms.Label
    Private WithEvents m_nudReplyTO As System.Windows.Forms.NumericUpDown
End Class
