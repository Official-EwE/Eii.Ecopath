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
Partial Class dlgRandomizeMPA
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgRandomizeMPA))
        Me.m_lblMPA = New System.Windows.Forms.Label()
        Me.m_lblClose = New System.Windows.Forms.Label()
        Me.m_cmbDestMPA = New System.Windows.Forms.ComboBox()
        Me.m_nudPercentage = New System.Windows.Forms.NumericUpDown()
        Me.m_btnCloseCells = New System.Windows.Forms.Button()
        Me.m_cbClosePerRegion = New System.Windows.Forms.CheckBox()
        Me.m_lblFrom = New System.Windows.Forms.Label()
        Me.m_cmbSrcMPA = New System.Windows.Forms.ComboBox()
        CType(Me.m_nudPercentage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblMPA
        '
        resources.ApplyResources(Me.m_lblMPA, "m_lblMPA")
        Me.m_lblMPA.Name = "m_lblMPA"
        '
        'm_lblClose
        '
        resources.ApplyResources(Me.m_lblClose, "m_lblClose")
        Me.m_lblClose.Name = "m_lblClose"
        '
        'm_cmbDestMPA
        '
        resources.ApplyResources(Me.m_cmbDestMPA, "m_cmbDestMPA")
        Me.m_cmbDestMPA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbDestMPA.FormattingEnabled = True
        Me.m_cmbDestMPA.Name = "m_cmbDestMPA"
        '
        'm_nudPercentage
        '
        resources.ApplyResources(Me.m_nudPercentage, "m_nudPercentage")
        Me.m_nudPercentage.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.m_nudPercentage.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.m_nudPercentage.Name = "m_nudPercentage"
        Me.m_nudPercentage.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'm_btnCloseCells
        '
        resources.ApplyResources(Me.m_btnCloseCells, "m_btnCloseCells")
        Me.m_btnCloseCells.Image = Global.EwERandomizeMPAPlugin.My.Resources.Resources.Dice
        Me.m_btnCloseCells.Name = "m_btnCloseCells"
        Me.m_btnCloseCells.UseVisualStyleBackColor = True
        '
        'm_cbClosePerRegion
        '
        resources.ApplyResources(Me.m_cbClosePerRegion, "m_cbClosePerRegion")
        Me.m_cbClosePerRegion.Name = "m_cbClosePerRegion"
        Me.m_cbClosePerRegion.UseVisualStyleBackColor = True
        '
        'm_lblFrom
        '
        resources.ApplyResources(Me.m_lblFrom, "m_lblFrom")
        Me.m_lblFrom.Name = "m_lblFrom"
        '
        'm_cmbSrcMPA
        '
        resources.ApplyResources(Me.m_cmbSrcMPA, "m_cmbSrcMPA")
        Me.m_cmbSrcMPA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbSrcMPA.FormattingEnabled = True
        Me.m_cmbSrcMPA.Name = "m_cmbSrcMPA"
        '
        'dlgRandomizeMPA
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_cbClosePerRegion)
        Me.Controls.Add(Me.m_btnCloseCells)
        Me.Controls.Add(Me.m_nudPercentage)
        Me.Controls.Add(Me.m_cmbSrcMPA)
        Me.Controls.Add(Me.m_cmbDestMPA)
        Me.Controls.Add(Me.m_lblFrom)
        Me.Controls.Add(Me.m_lblClose)
        Me.Controls.Add(Me.m_lblMPA)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgRandomizeMPA"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.m_nudPercentage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_lblMPA As Windows.Forms.Label
    Private WithEvents m_cmbDestMPA As Windows.Forms.ComboBox
    Private WithEvents m_nudPercentage As Windows.Forms.NumericUpDown
    Private WithEvents m_btnCloseCells As Windows.Forms.Button
    Private WithEvents m_lblClose As Windows.Forms.Label
    Private WithEvents m_cbClosePerRegion As Windows.Forms.CheckBox
    Private WithEvents m_lblFrom As Windows.Forms.Label
    Private WithEvents m_cmbSrcMPA As Windows.Forms.ComboBox
End Class
