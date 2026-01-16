' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace Ecospace

    Partial Class dlgDefineRegions
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgDefineRegions))
            Me.m_nudNoRegions = New System.Windows.Forms.NumericUpDown()
            Me.m_btnOK = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.m_lblNoRegions = New System.Windows.Forms.Label()
            Me.m_lblAllocate = New System.Windows.Forms.Label()
            Me.m_dgvMapping = New System.Windows.Forms.DataGridView()
            Me.m_colIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_colName = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_colRegion = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_colPriority = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_rbNone = New System.Windows.Forms.RadioButton()
            Me.m_tlpOptions = New System.Windows.Forms.TableLayoutPanel()
            Me.m_rbFromHabitats = New System.Windows.Forms.RadioButton()
            Me.m_rbFromMPAs = New System.Windows.Forms.RadioButton()
            Me.m_acknowledgements = New ScientificInterfaceShared.ucLogoBar()
            CType(Me.m_nudNoRegions, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_dgvMapping, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpOptions.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_nudNoRegions
            '
            resources.ApplyResources(Me.m_nudNoRegions, "m_nudNoRegions")
            Me.m_nudNoRegions.Name = "m_nudNoRegions"
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
            'm_lblNoRegions
            '
            resources.ApplyResources(Me.m_lblNoRegions, "m_lblNoRegions")
            Me.m_lblNoRegions.Name = "m_lblNoRegions"
            '
            'm_lblAllocate
            '
            resources.ApplyResources(Me.m_lblAllocate, "m_lblAllocate")
            Me.m_lblAllocate.Name = "m_lblAllocate"
            '
            'm_dgvMapping
            '
            Me.m_dgvMapping.AllowUserToAddRows = False
            Me.m_dgvMapping.AllowUserToDeleteRows = False
            Me.m_dgvMapping.AllowUserToResizeRows = False
            resources.ApplyResources(Me.m_dgvMapping, "m_dgvMapping")
            Me.m_dgvMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.m_dgvMapping.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colIndex, Me.m_colName, Me.m_colRegion, Me.m_colPriority})
            Me.m_dgvMapping.MultiSelect = False
            Me.m_dgvMapping.Name = "m_dgvMapping"
            Me.m_dgvMapping.RowHeadersVisible = False
            Me.m_dgvMapping.ShowRowErrors = False
            '
            'm_colIndex
            '
            resources.ApplyResources(Me.m_colIndex, "m_colIndex")
            Me.m_colIndex.Name = "m_colIndex"
            Me.m_colIndex.ReadOnly = True
            '
            'm_colName
            '
            Me.m_colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            resources.ApplyResources(Me.m_colName, "m_colName")
            Me.m_colName.Name = "m_colName"
            Me.m_colName.ReadOnly = True
            '
            'm_colRegion
            '
            resources.ApplyResources(Me.m_colRegion, "m_colRegion")
            Me.m_colRegion.MaxInputLength = 2
            Me.m_colRegion.Name = "m_colRegion"
            '
            'm_colPriority
            '
            resources.ApplyResources(Me.m_colPriority, "m_colPriority")
            Me.m_colPriority.MaxInputLength = 2
            Me.m_colPriority.Name = "m_colPriority"
            '
            'm_rbNone
            '
            resources.ApplyResources(Me.m_rbNone, "m_rbNone")
            Me.m_rbNone.Name = "m_rbNone"
            Me.m_rbNone.TabStop = True
            Me.m_rbNone.UseVisualStyleBackColor = True
            '
            'm_tlpOptions
            '
            resources.ApplyResources(Me.m_tlpOptions, "m_tlpOptions")
            Me.m_tlpOptions.Controls.Add(Me.m_rbNone, 0, 0)
            Me.m_tlpOptions.Controls.Add(Me.m_rbFromHabitats, 1, 0)
            Me.m_tlpOptions.Controls.Add(Me.m_rbFromMPAs, 2, 0)
            Me.m_tlpOptions.Name = "m_tlpOptions"
            '
            'm_rbFromHabitats
            '
            resources.ApplyResources(Me.m_rbFromHabitats, "m_rbFromHabitats")
            Me.m_rbFromHabitats.Name = "m_rbFromHabitats"
            Me.m_rbFromHabitats.TabStop = True
            Me.m_rbFromHabitats.UseVisualStyleBackColor = True
            '
            'm_rbFromMPAs
            '
            resources.ApplyResources(Me.m_rbFromMPAs, "m_rbFromMPAs")
            Me.m_rbFromMPAs.Name = "m_rbFromMPAs"
            Me.m_rbFromMPAs.TabStop = True
            Me.m_rbFromMPAs.UseVisualStyleBackColor = True
            '
            'm_acknowledgements
            '
            resources.ApplyResources(Me.m_acknowledgements, "m_acknowledgements")
            Me.m_acknowledgements.Name = "m_acknowledgements"
            Me.m_acknowledgements.UIContext = Nothing
            '
            'dlgDefineRegions
            '
            Me.AcceptButton = Me.m_btnOK
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_acknowledgements)
            Me.Controls.Add(Me.m_tlpOptions)
            Me.Controls.Add(Me.m_dgvMapping)
            Me.Controls.Add(Me.m_lblAllocate)
            Me.Controls.Add(Me.m_lblNoRegions)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_nudNoRegions)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgDefineRegions"
            Me.ShowInTaskbar = False
            CType(Me.m_nudNoRegions, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_dgvMapping, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpOptions.ResumeLayout(False)
            Me.m_tlpOptions.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudNoRegions As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_lblNoRegions As System.Windows.Forms.Label
        Private WithEvents m_lblAllocate As Label
        Private WithEvents m_tlpOptions As TableLayoutPanel
        Private WithEvents m_rbFromHabitats As RadioButton
        Private WithEvents m_rbFromMPAs As RadioButton
        Private WithEvents m_rbNone As RadioButton
        Private WithEvents m_dgvMapping As DataGridView
        Friend WithEvents m_colIndex As DataGridViewTextBoxColumn
        Friend WithEvents m_colName As DataGridViewTextBoxColumn
        Friend WithEvents m_colRegion As DataGridViewTextBoxColumn
        Friend WithEvents m_colPriority As DataGridViewTextBoxColumn
        Private WithEvents m_acknowledgements As ScientificInterfaceShared.ucLogoBar
    End Class

End Namespace
