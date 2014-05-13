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
' Copyright 1991- Ecopath International Initiative, Barcelona, Spain and
'                 Joint Reseach Centre, Ispra, Italy.
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmImport))
        Me.m_lblDrop = New cFileDropLabel
        Me.m_btnImport = New System.Windows.Forms.Button()
        Me.m_tlpOptions = New System.Windows.Forms.TableLayoutPanel()
        Me.m_hdrEnvelopes = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrSpecies = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_clbxSpecies = New System.Windows.Forms.CheckedListBox()
        Me.m_clbxEnvelopes = New System.Windows.Forms.CheckedListBox()
        Me.m_llAquamaps = New System.Windows.Forms.LinkLabel()
        Me.m_tlpOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblDrop
        '
        Me.m_lblDrop.AllowDrop = True
        resources.ApplyResources(Me.m_lblDrop, "m_lblDrop")
        Me.m_lblDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lblDrop.ForeColor = System.Drawing.SystemColors.AppWorkspace
        Me.m_lblDrop.Name = "m_lblDrop"
        '
        'm_btnImport
        '
        resources.ApplyResources(Me.m_btnImport, "m_btnImport")
        Me.m_btnImport.Name = "m_btnImport"
        Me.m_btnImport.UseVisualStyleBackColor = True
        '
        'm_tlpOptions
        '
        resources.ApplyResources(Me.m_tlpOptions, "m_tlpOptions")
        Me.m_tlpOptions.Controls.Add(Me.m_hdrEnvelopes, 1, 0)
        Me.m_tlpOptions.Controls.Add(Me.m_hdrSpecies, 0, 0)
        Me.m_tlpOptions.Controls.Add(Me.m_clbxSpecies, 0, 1)
        Me.m_tlpOptions.Controls.Add(Me.m_clbxEnvelopes, 1, 1)
        Me.m_tlpOptions.Name = "m_tlpOptions"
        '
        'm_hdrEnvelopes
        '
        Me.m_hdrEnvelopes.CanCollapseParent = False
        Me.m_hdrEnvelopes.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrEnvelopes, "m_hdrEnvelopes")
        Me.m_hdrEnvelopes.IsCollapsed = False
        Me.m_hdrEnvelopes.Name = "m_hdrEnvelopes"
        '
        'm_hdrSpecies
        '
        Me.m_hdrSpecies.CanCollapseParent = False
        Me.m_hdrSpecies.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrSpecies, "m_hdrSpecies")
        Me.m_hdrSpecies.IsCollapsed = False
        Me.m_hdrSpecies.Name = "m_hdrSpecies"
        '
        'm_clbxSpecies
        '
        Me.m_clbxSpecies.CheckOnClick = True
        resources.ApplyResources(Me.m_clbxSpecies, "m_clbxSpecies")
        Me.m_clbxSpecies.FormattingEnabled = True
        Me.m_clbxSpecies.Name = "m_clbxSpecies"
        '
        'm_clbxEnvelopes
        '
        Me.m_clbxEnvelopes.CheckOnClick = True
        resources.ApplyResources(Me.m_clbxEnvelopes, "m_clbxEnvelopes")
        Me.m_clbxEnvelopes.FormattingEnabled = True
        Me.m_clbxEnvelopes.Name = "m_clbxEnvelopes"
        '
        'm_llAquamaps
        '
        resources.ApplyResources(Me.m_llAquamaps, "m_llAquamaps")
        Me.m_llAquamaps.Name = "m_llAquamaps"
        Me.m_llAquamaps.TabStop = True
        '
        'frmImport
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_llAquamaps)
        Me.Controls.Add(Me.m_tlpOptions)
        Me.Controls.Add(Me.m_btnImport)
        Me.Controls.Add(Me.m_lblDrop)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmImport"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.m_tlpOptions.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblDrop As cFileDropLabel
    Private WithEvents m_btnImport As System.Windows.Forms.Button
    Private WithEvents m_tlpOptions As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_hdrEnvelopes As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrSpecies As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_clbxSpecies As System.Windows.Forms.CheckedListBox
    Private WithEvents m_clbxEnvelopes As System.Windows.Forms.CheckedListBox
    Private WithEvents m_llAquamaps As System.Windows.Forms.LinkLabel
End Class
