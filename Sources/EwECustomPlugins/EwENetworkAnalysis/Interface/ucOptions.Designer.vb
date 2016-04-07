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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptions
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_cbUseTimeout = New System.Windows.Forms.CheckBox()
        Me.m_lblTimeout = New System.Windows.Forms.Label()
        Me.m_nudTimeOut = New System.Windows.Forms.NumericUpDown()
        Me.m_lblTimeOutUnit = New System.Windows.Forms.Label()
        CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_hdrOptions
        '
        Me.m_hdrOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrOptions.CanCollapseParent = False
        Me.m_hdrOptions.CollapsedParentHeight = 0
        Me.m_hdrOptions.IsCollapsed = False
        Me.m_hdrOptions.Location = New System.Drawing.Point(3, 4)
        Me.m_hdrOptions.Name = "m_hdrOptions"
        Me.m_hdrOptions.Size = New System.Drawing.Size(161, 18)
        Me.m_hdrOptions.TabIndex = 0
        Me.m_hdrOptions.Text = "Generic options"
        Me.m_hdrOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_cbUseTimeout
        '
        Me.m_cbUseTimeout.AutoSize = True
        Me.m_cbUseTimeout.Location = New System.Drawing.Point(6, 32)
        Me.m_cbUseTimeout.Name = "m_cbUseTimeout"
        Me.m_cbUseTimeout.Size = New System.Drawing.Size(156, 17)
        Me.m_cbUseTimeout.TabIndex = 1
        Me.m_cbUseTimeout.Text = "&Use timeout for calculations"
        Me.m_cbUseTimeout.UseVisualStyleBackColor = True
        '
        'm_lblTimeout
        '
        Me.m_lblTimeout.AutoSize = True
        Me.m_lblTimeout.Location = New System.Drawing.Point(22, 57)
        Me.m_lblTimeout.Name = "m_lblTimeout"
        Me.m_lblTimeout.Size = New System.Drawing.Size(51, 13)
        Me.m_lblTimeout.TabIndex = 2
        Me.m_lblTimeout.Text = "Time out:"
        '
        'm_nudTimeOut
        '
        Me.m_nudTimeOut.Location = New System.Drawing.Point(79, 55)
        Me.m_nudTimeOut.Maximum = New Decimal(New Integer() {360, 0, 0, 0})
        Me.m_nudTimeOut.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.m_nudTimeOut.Name = "m_nudTimeOut"
        Me.m_nudTimeOut.Size = New System.Drawing.Size(49, 20)
        Me.m_nudTimeOut.TabIndex = 3
        Me.m_nudTimeOut.Value = New Decimal(New Integer() {30, 0, 0, 0})
        '
        'm_lblTimeOutUnit
        '
        Me.m_lblTimeOutUnit.AutoSize = True
        Me.m_lblTimeOutUnit.Location = New System.Drawing.Point(134, 57)
        Me.m_lblTimeOutUnit.Name = "m_lblTimeOutUnit"
        Me.m_lblTimeOutUnit.Size = New System.Drawing.Size(31, 13)
        Me.m_lblTimeOutUnit.TabIndex = 4
        Me.m_lblTimeOutUnit.Text = "mins."
        '
        'ucOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblTimeOutUnit)
        Me.Controls.Add(Me.m_nudTimeOut)
        Me.Controls.Add(Me.m_lblTimeout)
        Me.Controls.Add(Me.m_cbUseTimeout)
        Me.Controls.Add(Me.m_hdrOptions)
        Me.Name = "ucOptions"
        Me.Size = New System.Drawing.Size(167, 83)
        CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbUseTimeout As System.Windows.Forms.CheckBox
    Private WithEvents m_lblTimeout As System.Windows.Forms.Label
    Private WithEvents m_nudTimeOut As System.Windows.Forms.NumericUpDown
    Private WithEvents m_lblTimeOutUnit As System.Windows.Forms.Label

End Class
