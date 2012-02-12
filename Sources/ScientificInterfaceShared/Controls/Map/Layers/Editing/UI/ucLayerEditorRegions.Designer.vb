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
Namespace Controls.Map.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorRegion
        Inherits ucLayerEditorDefault

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
            Me.m_lblNoRegions = New System.Windows.Forms.Label()
            Me.m_nudNoRegions = New System.Windows.Forms.NumericUpDown()
            Me.m_lblRegion = New System.Windows.Forms.Label()
            Me.m_nudRegion = New System.Windows.Forms.NumericUpDown()
            Me.m_btnFromCell = New System.Windows.Forms.Button()
            Me.m_btnFromMPAs = New System.Windows.Forms.Button()
            Me.m_btnFromHabitats = New System.Windows.Forms.Button()
            Me.m_lblClusterSize = New System.Windows.Forms.Label()
            Me.m_nudClusterSize = New System.Windows.Forms.NumericUpDown()
            Me.m_lblCreateRegions = New System.Windows.Forms.Label()
            CType(Me.m_nudNoRegions, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRegion, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudClusterSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblNoRegions
            '
            Me.m_lblNoRegions.AutoSize = True
            Me.m_lblNoRegions.Location = New System.Drawing.Point(3, 67)
            Me.m_lblNoRegions.Name = "m_lblNoRegions"
            Me.m_lblNoRegions.Size = New System.Drawing.Size(73, 13)
            Me.m_lblNoRegions.TabIndex = 4
            Me.m_lblNoRegions.Text = "&No of regions:"
            '
            'm_nudNoRegions
            '
            Me.m_nudNoRegions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudNoRegions.Location = New System.Drawing.Point(95, 65)
            Me.m_nudNoRegions.Name = "m_nudNoRegions"
            Me.m_nudNoRegions.Size = New System.Drawing.Size(102, 20)
            Me.m_nudNoRegions.TabIndex = 5
            '
            'm_lblRegion
            '
            Me.m_lblRegion.AutoSize = True
            Me.m_lblRegion.Location = New System.Drawing.Point(3, 46)
            Me.m_lblRegion.Name = "m_lblRegion"
            Me.m_lblRegion.Size = New System.Drawing.Size(44, 13)
            Me.m_lblRegion.TabIndex = 2
            Me.m_lblRegion.Text = "&Region:"
            '
            'm_nudRegion
            '
            Me.m_nudRegion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudRegion.Location = New System.Drawing.Point(95, 39)
            Me.m_nudRegion.Name = "m_nudRegion"
            Me.m_nudRegion.Size = New System.Drawing.Size(102, 20)
            Me.m_nudRegion.TabIndex = 3
            '
            'm_btnFromCell
            '
            Me.m_btnFromCell.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnFromCell.Location = New System.Drawing.Point(6, 168)
            Me.m_btnFromCell.Name = "m_btnFromCell"
            Me.m_btnFromCell.Size = New System.Drawing.Size(191, 23)
            Me.m_btnFromCell.TabIndex = 7
            Me.m_btnFromCell.Text = "For each &cell"
            Me.m_btnFromCell.UseVisualStyleBackColor = True
            '
            'm_btnFromMPAs
            '
            Me.m_btnFromMPAs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnFromMPAs.Location = New System.Drawing.Point(6, 110)
            Me.m_btnFromMPAs.Name = "m_btnFromMPAs"
            Me.m_btnFromMPAs.Size = New System.Drawing.Size(191, 23)
            Me.m_btnFromMPAs.TabIndex = 8
            Me.m_btnFromMPAs.Text = "From &MPAs"
            Me.m_btnFromMPAs.UseVisualStyleBackColor = True
            '
            'm_btnFromHabitats
            '
            Me.m_btnFromHabitats.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnFromHabitats.Location = New System.Drawing.Point(6, 139)
            Me.m_btnFromHabitats.Name = "m_btnFromHabitats"
            Me.m_btnFromHabitats.Size = New System.Drawing.Size(191, 23)
            Me.m_btnFromHabitats.TabIndex = 9
            Me.m_btnFromHabitats.Text = "From &habitats"
            Me.m_btnFromHabitats.UseVisualStyleBackColor = True
            '
            'm_lblClusterSize
            '
            Me.m_lblClusterSize.AutoSize = True
            Me.m_lblClusterSize.Location = New System.Drawing.Point(8, 199)
            Me.m_lblClusterSize.Name = "m_lblClusterSize"
            Me.m_lblClusterSize.Size = New System.Drawing.Size(63, 13)
            Me.m_lblClusterSize.TabIndex = 10
            Me.m_lblClusterSize.Text = "Cluster &size:"
            '
            'm_nudClusterSize
            '
            Me.m_nudClusterSize.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudClusterSize.Location = New System.Drawing.Point(95, 197)
            Me.m_nudClusterSize.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudClusterSize.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudClusterSize.Name = "m_nudClusterSize"
            Me.m_nudClusterSize.Size = New System.Drawing.Size(102, 20)
            Me.m_nudClusterSize.TabIndex = 11
            Me.m_nudClusterSize.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_lblCreateRegions
            '
            Me.m_lblCreateRegions.AutoSize = True
            Me.m_lblCreateRegions.Location = New System.Drawing.Point(3, 93)
            Me.m_lblCreateRegions.Name = "m_lblCreateRegions"
            Me.m_lblCreateRegions.Size = New System.Drawing.Size(78, 13)
            Me.m_lblCreateRegions.TabIndex = 12
            Me.m_lblCreateRegions.Text = "Create regions:"
            '
            'ucLayerEditorRegion
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblCreateRegions)
            Me.Controls.Add(Me.m_nudClusterSize)
            Me.Controls.Add(Me.m_lblClusterSize)
            Me.Controls.Add(Me.m_btnFromHabitats)
            Me.Controls.Add(Me.m_btnFromMPAs)
            Me.Controls.Add(Me.m_btnFromCell)
            Me.Controls.Add(Me.m_lblRegion)
            Me.Controls.Add(Me.m_lblNoRegions)
            Me.Controls.Add(Me.m_nudRegion)
            Me.Controls.Add(Me.m_nudNoRegions)
            Me.Name = "ucLayerEditorRegion"
            Me.Size = New System.Drawing.Size(200, 230)
            Me.Controls.SetChildIndex(Me.m_nudNoRegions, 0)
            Me.Controls.SetChildIndex(Me.m_nudRegion, 0)
            Me.Controls.SetChildIndex(Me.m_lblNoRegions, 0)
            Me.Controls.SetChildIndex(Me.m_lblRegion, 0)
            Me.Controls.SetChildIndex(Me.m_btnFromCell, 0)
            Me.Controls.SetChildIndex(Me.m_btnFromMPAs, 0)
            Me.Controls.SetChildIndex(Me.m_btnFromHabitats, 0)
            Me.Controls.SetChildIndex(Me.m_lblClusterSize, 0)
            Me.Controls.SetChildIndex(Me.m_nudClusterSize, 0)
            Me.Controls.SetChildIndex(Me.m_lblCreateRegions, 0)
            CType(Me.m_nudNoRegions, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRegion, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudClusterSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblNoRegions As System.Windows.Forms.Label
        Private WithEvents m_nudNoRegions As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblRegion As System.Windows.Forms.Label
        Private WithEvents m_nudRegion As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnFromCell As System.Windows.Forms.Button
        Private WithEvents m_btnFromMPAs As System.Windows.Forms.Button
        Private WithEvents m_btnFromHabitats As System.Windows.Forms.Button
        Friend WithEvents m_lblClusterSize As System.Windows.Forms.Label
        Friend WithEvents m_nudClusterSize As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblCreateRegions As System.Windows.Forms.Label

    End Class

End Namespace
