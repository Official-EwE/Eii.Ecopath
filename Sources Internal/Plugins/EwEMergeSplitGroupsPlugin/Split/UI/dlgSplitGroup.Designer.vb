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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgSplitGroup
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSplitGroup))
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_lblTarget = New System.Windows.Forms.Label()
        Me.m_cmbSource = New System.Windows.Forms.ComboBox()
        Me.m_tbxSplit1 = New System.Windows.Forms.TextBox()
        Me.m_tbxB1 = New System.Windows.Forms.TextBox()
        Me.m_tbxSplit2 = New System.Windows.Forms.TextBox()
        Me.m_tbxB2 = New System.Windows.Forms.TextBox()
        Me.m_sliderB = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_lblBiomass = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblName = New System.Windows.Forms.Label()
        Me.m_hdrSplit1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrSplit2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_lbxTaxa1 = New System.Windows.Forms.ListBox()
        Me.m_lbxTaxa2 = New System.Windows.Forms.ListBox()
        Me.m_plMoveTaxa = New System.Windows.Forms.Panel()
        Me.m_btn2to1 = New System.Windows.Forms.Button()
        Me.m_btn1to2 = New System.Windows.Forms.Button()
        Me.m_lblStartAge = New System.Windows.Forms.Label()
        Me.m_tbxAge1 = New System.Windows.Forms.TextBox()
        Me.m_tbxAge2 = New System.Windows.Forms.TextBox()
        Me.m_tlpLogo = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbLogo = New System.Windows.Forms.PictureBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.m_plMoveTaxa.SuspendLayout()
        Me.m_tlpLogo.SuspendLayout()
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        'm_lblTarget
        '
        resources.ApplyResources(Me.m_lblTarget, "m_lblTarget")
        Me.m_lblTarget.Name = "m_lblTarget"
        '
        'm_cmbSource
        '
        resources.ApplyResources(Me.m_cmbSource, "m_cmbSource")
        Me.m_cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbSource.FormattingEnabled = True
        Me.m_cmbSource.Name = "m_cmbSource"
        '
        'm_tbxSplit1
        '
        resources.ApplyResources(Me.m_tbxSplit1, "m_tbxSplit1")
        Me.m_tbxSplit1.Name = "m_tbxSplit1"
        '
        'm_tbxB1
        '
        resources.ApplyResources(Me.m_tbxB1, "m_tbxB1")
        Me.m_tbxB1.Name = "m_tbxB1"
        '
        'm_tbxSplit2
        '
        resources.ApplyResources(Me.m_tbxSplit2, "m_tbxSplit2")
        Me.m_tbxSplit2.Name = "m_tbxSplit2"
        '
        'm_tbxB2
        '
        resources.ApplyResources(Me.m_tbxB2, "m_tbxB2")
        Me.m_tbxB2.Name = "m_tbxB2"
        '
        'm_sliderB
        '
        resources.ApplyResources(Me.m_sliderB, "m_sliderB")
        Me.m_sliderB.CurrentKnob = 0
        Me.m_sliderB.Maximum = 1000
        Me.m_sliderB.Minimum = 0
        Me.m_sliderB.Name = "m_sliderB"
        Me.m_sliderB.NumKnobs = 1
        '
        'm_lblBiomass
        '
        resources.ApplyResources(Me.m_lblBiomass, "m_lblBiomass")
        Me.m_lblBiomass.Name = "m_lblBiomass"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxSplit1, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_sliderB, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblBiomass, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxSplit2, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxB1, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblName, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxB2, 3, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_hdrSplit1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.m_hdrSplit2, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbxTaxa1, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lbxTaxa2, 3, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_plMoveTaxa, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.m_lblStartAge, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxAge1, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tbxAge2, 3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'm_lblName
        '
        resources.ApplyResources(Me.m_lblName, "m_lblName")
        Me.m_lblName.Name = "m_lblName"
        '
        'm_hdrSplit1
        '
        Me.m_hdrSplit1.CanCollapseParent = False
        Me.m_hdrSplit1.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrSplit1, "m_hdrSplit1")
        Me.m_hdrSplit1.IsCollapsed = False
        Me.m_hdrSplit1.Name = "m_hdrSplit1"
        '
        'm_hdrSplit2
        '
        Me.m_hdrSplit2.CanCollapseParent = False
        Me.m_hdrSplit2.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrSplit2, "m_hdrSplit2")
        Me.m_hdrSplit2.IsCollapsed = False
        Me.m_hdrSplit2.Name = "m_hdrSplit2"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'm_lbxTaxa1
        '
        resources.ApplyResources(Me.m_lbxTaxa1, "m_lbxTaxa1")
        Me.m_lbxTaxa1.FormattingEnabled = True
        Me.m_lbxTaxa1.Name = "m_lbxTaxa1"
        Me.m_lbxTaxa1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        '
        'm_lbxTaxa2
        '
        resources.ApplyResources(Me.m_lbxTaxa2, "m_lbxTaxa2")
        Me.m_lbxTaxa2.FormattingEnabled = True
        Me.m_lbxTaxa2.Name = "m_lbxTaxa2"
        Me.m_lbxTaxa2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        '
        'm_plMoveTaxa
        '
        resources.ApplyResources(Me.m_plMoveTaxa, "m_plMoveTaxa")
        Me.m_plMoveTaxa.Controls.Add(Me.m_btn2to1)
        Me.m_plMoveTaxa.Controls.Add(Me.m_btn1to2)
        Me.m_plMoveTaxa.Name = "m_plMoveTaxa"
        '
        'm_btn2to1
        '
        resources.ApplyResources(Me.m_btn2to1, "m_btn2to1")
        Me.m_btn2to1.Name = "m_btn2to1"
        Me.m_btn2to1.UseVisualStyleBackColor = True
        '
        'm_btn1to2
        '
        resources.ApplyResources(Me.m_btn1to2, "m_btn1to2")
        Me.m_btn1to2.Name = "m_btn1to2"
        Me.m_btn1to2.UseVisualStyleBackColor = True
        '
        'm_lblStartAge
        '
        resources.ApplyResources(Me.m_lblStartAge, "m_lblStartAge")
        Me.m_lblStartAge.Name = "m_lblStartAge"
        '
        'm_tbxAge1
        '
        resources.ApplyResources(Me.m_tbxAge1, "m_tbxAge1")
        Me.m_tbxAge1.Name = "m_tbxAge1"
        '
        'm_tbxAge2
        '
        resources.ApplyResources(Me.m_tbxAge2, "m_tbxAge2")
        Me.m_tbxAge2.Name = "m_tbxAge2"
        '
        'm_tlpLogo
        '
        Me.m_tlpLogo.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_tlpLogo, "m_tlpLogo")
        Me.m_tlpLogo.Controls.Add(Me.m_pbLogo, 1, 1)
        Me.m_tlpLogo.Name = "m_tlpLogo"
        '
        'm_pbLogo
        '
        Me.m_pbLogo.BackgroundImage = Global.EwEMergeSplitGroupsPlugin.My.Resources.Resources.geomar_logo_en_print
        resources.ApplyResources(Me.m_pbLogo, "m_pbLogo")
        Me.m_pbLogo.Name = "m_pbLogo"
        Me.m_pbLogo.TabStop = False
        '
        'dlgSplitGroup
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tlpLogo)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_cmbSource)
        Me.Controls.Add(Me.m_lblTarget)
        Me.Name = "dlgSplitGroup"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.m_plMoveTaxa.ResumeLayout(False)
        Me.m_tlpLogo.ResumeLayout(False)
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_btnOK As Windows.Forms.Button
    Private WithEvents m_btnCancel As Windows.Forms.Button
    Private WithEvents m_lblTarget As Windows.Forms.Label
    Private WithEvents m_cmbSource As Windows.Forms.ComboBox
    Friend WithEvents m_tbxSplit1 As Windows.Forms.TextBox
    Private WithEvents m_tbxB1 As Windows.Forms.TextBox
    Friend WithEvents m_tbxSplit2 As Windows.Forms.TextBox
    Private WithEvents m_tbxB2 As Windows.Forms.TextBox
    Private WithEvents m_sliderB As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_lblBiomass As Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblName As Windows.Forms.Label
    Friend WithEvents m_hdrSplit1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrSplit2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents m_lbxTaxa1 As Windows.Forms.ListBox
    Friend WithEvents m_lbxTaxa2 As Windows.Forms.ListBox
    Private WithEvents m_plMoveTaxa As Windows.Forms.Panel
    Private WithEvents m_btn2to1 As Windows.Forms.Button
    Private WithEvents m_btn1to2 As Windows.Forms.Button
    Private WithEvents m_lblStartAge As Windows.Forms.Label
    Friend WithEvents m_tbxAge1 As Windows.Forms.TextBox
    Private WithEvents m_tbxAge2 As Windows.Forms.TextBox
    Private WithEvents m_tlpLogo As Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbLogo As Windows.Forms.PictureBox
End Class
