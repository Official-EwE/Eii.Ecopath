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
Namespace Ecospace.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucConfigAdapter
        Inherits System.Windows.Forms.UserControl

         'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                Me.UIContext = Nothing
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblNewDS = New System.Windows.Forms.Label()
            Me.m_btnCreateDS = New System.Windows.Forms.Button()
            Me.m_lbxExistingDS = New System.Windows.Forms.ListBox()
            Me.m_btnConfigDS = New System.Windows.Forms.Button()
            Me.m_lblSelectCV = New System.Windows.Forms.Label()
            Me.m_lbxExistingConv = New System.Windows.Forms.ListBox()
            Me.m_btnConfigureCV = New System.Windows.Forms.Button()
            Me.m_cmbNewDS = New System.Windows.Forms.ComboBox()
            Me.m_btnDeleteDS = New System.Windows.Forms.Button()
            Me.m_btnClearCache = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_hdrSource
            '
            Me.m_hdrSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrSource.CanCollapseParent = False
            Me.m_hdrSource.CollapsedParentHeight = 0
            Me.m_hdrSource.IsCollapsed = False
            Me.m_hdrSource.Location = New System.Drawing.Point(3, 0)
            Me.m_hdrSource.Name = "m_hdrSource"
            Me.m_hdrSource.Size = New System.Drawing.Size(305, 18)
            Me.m_hdrSource.TabIndex = 0
            Me.m_hdrSource.Text = "External data"
            Me.m_hdrSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblNewDS
            '
            Me.m_lblNewDS.AutoSize = True
            Me.m_lblNewDS.Location = New System.Drawing.Point(3, 24)
            Me.m_lblNewDS.Name = "m_lblNewDS"
            Me.m_lblNewDS.Size = New System.Drawing.Size(64, 13)
            Me.m_lblNewDS.TabIndex = 2
            Me.m_lblNewDS.Text = "&Connection:"
            '
            'm_btnCreateDS
            '
            Me.m_btnCreateDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnCreateDS.Location = New System.Drawing.Point(233, 19)
            Me.m_btnCreateDS.Name = "m_btnCreateDS"
            Me.m_btnCreateDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnCreateDS.TabIndex = 4
            Me.m_btnCreateDS.Text = "&Create..."
            Me.m_btnCreateDS.UseVisualStyleBackColor = True
            '
            'm_lbxExistingDS
            '
            Me.m_lbxExistingDS.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbxExistingDS.FormattingEnabled = True
            Me.m_lbxExistingDS.IntegralHeight = False
            Me.m_lbxExistingDS.Location = New System.Drawing.Point(73, 48)
            Me.m_lbxExistingDS.Name = "m_lbxExistingDS"
            Me.m_lbxExistingDS.Size = New System.Drawing.Size(154, 138)
            Me.m_lbxExistingDS.TabIndex = 3
            '
            'm_btnConfigDS
            '
            Me.m_btnConfigDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConfigDS.Location = New System.Drawing.Point(233, 48)
            Me.m_btnConfigDS.Name = "m_btnConfigDS"
            Me.m_btnConfigDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnConfigDS.TabIndex = 4
            Me.m_btnConfigDS.Text = "C&onfigure..."
            Me.m_btnConfigDS.UseVisualStyleBackColor = True
            '
            'm_lblSelectCV
            '
            Me.m_lblSelectCV.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblSelectCV.AutoSize = True
            Me.m_lblSelectCV.Location = New System.Drawing.Point(3, 226)
            Me.m_lblSelectCV.Name = "m_lblSelectCV"
            Me.m_lblSelectCV.Size = New System.Drawing.Size(56, 13)
            Me.m_lblSelectCV.TabIndex = 2
            Me.m_lblSelectCV.Text = "&Converter:"
            '
            'm_lbxExistingConv
            '
            Me.m_lbxExistingConv.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbxExistingConv.FormattingEnabled = True
            Me.m_lbxExistingConv.IntegralHeight = False
            Me.m_lbxExistingConv.Location = New System.Drawing.Point(73, 221)
            Me.m_lbxExistingConv.Name = "m_lbxExistingConv"
            Me.m_lbxExistingConv.Size = New System.Drawing.Size(154, 95)
            Me.m_lbxExistingConv.TabIndex = 3
            '
            'm_btnConfigureCV
            '
            Me.m_btnConfigureCV.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConfigureCV.Location = New System.Drawing.Point(233, 221)
            Me.m_btnConfigureCV.Name = "m_btnConfigureCV"
            Me.m_btnConfigureCV.Size = New System.Drawing.Size(75, 23)
            Me.m_btnConfigureCV.TabIndex = 4
            Me.m_btnConfigureCV.Text = "C&onfigure..."
            Me.m_btnConfigureCV.UseVisualStyleBackColor = True
            '
            'm_cmbNewDS
            '
            Me.m_cmbNewDS.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbNewDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbNewDS.FormattingEnabled = True
            Me.m_cmbNewDS.Location = New System.Drawing.Point(73, 21)
            Me.m_cmbNewDS.Name = "m_cmbNewDS"
            Me.m_cmbNewDS.Size = New System.Drawing.Size(154, 21)
            Me.m_cmbNewDS.TabIndex = 5
            '
            'm_btnDeleteDS
            '
            Me.m_btnDeleteDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnDeleteDS.Location = New System.Drawing.Point(233, 77)
            Me.m_btnDeleteDS.Name = "m_btnDeleteDS"
            Me.m_btnDeleteDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnDeleteDS.TabIndex = 4
            Me.m_btnDeleteDS.Text = "&Delete..."
            Me.m_btnDeleteDS.UseVisualStyleBackColor = True
            '
            'm_btnClearCache
            '
            Me.m_btnClearCache.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnClearCache.Location = New System.Drawing.Point(233, 163)
            Me.m_btnClearCache.Name = "m_btnClearCache"
            Me.m_btnClearCache.Size = New System.Drawing.Size(75, 23)
            Me.m_btnClearCache.TabIndex = 6
            Me.m_btnClearCache.Text = "&Clear cache"
            Me.m_btnClearCache.UseVisualStyleBackColor = True
            '
            'ucConfigAdapter
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_btnClearCache)
            Me.Controls.Add(Me.m_cmbNewDS)
            Me.Controls.Add(Me.m_btnConfigureCV)
            Me.Controls.Add(Me.m_btnDeleteDS)
            Me.Controls.Add(Me.m_btnConfigDS)
            Me.Controls.Add(Me.m_btnCreateDS)
            Me.Controls.Add(Me.m_lbxExistingConv)
            Me.Controls.Add(Me.m_lbxExistingDS)
            Me.Controls.Add(Me.m_lblSelectCV)
            Me.Controls.Add(Me.m_lblNewDS)
            Me.Controls.Add(Me.m_hdrSource)
            Me.Name = "ucConfigAdapter"
            Me.Size = New System.Drawing.Size(311, 316)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_hdrSource As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_lblNewDS As System.Windows.Forms.Label
        Private WithEvents m_btnCreateDS As System.Windows.Forms.Button
        Private WithEvents m_lbxExistingDS As System.Windows.Forms.ListBox
        Private WithEvents m_btnConfigDS As System.Windows.Forms.Button
        Private WithEvents m_lblSelectCV As System.Windows.Forms.Label
        Private WithEvents m_lbxExistingConv As System.Windows.Forms.ListBox
        Private WithEvents m_btnConfigureCV As System.Windows.Forms.Button
        Private WithEvents m_cmbNewDS As System.Windows.Forms.ComboBox
        Private WithEvents m_btnDeleteDS As System.Windows.Forms.Button
        Private WithEvents m_btnClearCache As System.Windows.Forms.Button

    End Class

End Namespace
