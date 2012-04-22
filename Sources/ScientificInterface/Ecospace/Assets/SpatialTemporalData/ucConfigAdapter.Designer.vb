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
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plConnectionConverter = New System.Windows.Forms.Panel()
            Me.m_cmbConverter = New System.Windows.Forms.ComboBox()
            Me.m_btnClearCache = New System.Windows.Forms.Button()
            Me.m_cmbNewDS = New System.Windows.Forms.ComboBox()
            Me.m_btnConfigureCV = New System.Windows.Forms.Button()
            Me.m_btnDeleteDS = New System.Windows.Forms.Button()
            Me.m_btnConfigDS = New System.Windows.Forms.Button()
            Me.m_btnCreateDS = New System.Windows.Forms.Button()
            Me.m_lbxExistingDS = New System.Windows.Forms.ListBox()
            Me.m_lblSelectCV = New System.Windows.Forms.Label()
            Me.m_lblNewDS = New System.Windows.Forms.Label()
            Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_plScalarAdapter = New System.Windows.Forms.Panel()
            Me.m_btnCalculate = New System.Windows.Forms.Button()
            Me.m_tbxScale = New System.Windows.Forms.TextBox()
            Me.m_rbRelative = New System.Windows.Forms.RadioButton()
            Me.m_rbAbsolute = New System.Windows.Forms.RadioButton()
            Me.m_lblScaling = New System.Windows.Forms.Label()
            Me.m_tlpContent.SuspendLayout()
            Me.m_plConnectionConverter.SuspendLayout()
            Me.m_plScalarAdapter.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpContent
            '
            Me.m_tlpContent.ColumnCount = 1
            Me.m_tlpContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpContent.Controls.Add(Me.m_plConnectionConverter, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_plScalarAdapter, 0, 1)
            Me.m_tlpContent.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpContent.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpContent.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tlpContent.Name = "m_tlpContent"
            Me.m_tlpContent.RowCount = 2
            Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpContent.Size = New System.Drawing.Size(311, 316)
            Me.m_tlpContent.TabIndex = 1
            '
            'm_plConnectionConverter
            '
            Me.m_plConnectionConverter.Controls.Add(Me.m_cmbConverter)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnClearCache)
            Me.m_plConnectionConverter.Controls.Add(Me.m_cmbNewDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnConfigureCV)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnDeleteDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnConfigDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_btnCreateDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_lbxExistingDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_lblSelectCV)
            Me.m_plConnectionConverter.Controls.Add(Me.m_lblNewDS)
            Me.m_plConnectionConverter.Controls.Add(Me.m_hdrSource)
            Me.m_plConnectionConverter.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plConnectionConverter.Location = New System.Drawing.Point(0, 0)
            Me.m_plConnectionConverter.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plConnectionConverter.Name = "m_plConnectionConverter"
            Me.m_plConnectionConverter.Size = New System.Drawing.Size(311, 237)
            Me.m_plConnectionConverter.TabIndex = 1
            '
            'm_cmbConverter
            '
            Me.m_cmbConverter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbConverter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbConverter.FormattingEnabled = True
            Me.m_cmbConverter.Location = New System.Drawing.Point(70, 216)
            Me.m_cmbConverter.Name = "m_cmbConverter"
            Me.m_cmbConverter.Size = New System.Drawing.Size(160, 21)
            Me.m_cmbConverter.TabIndex = 20
            '
            'm_btnClearCache
            '
            Me.m_btnClearCache.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnClearCache.Location = New System.Drawing.Point(236, 168)
            Me.m_btnClearCache.Name = "m_btnClearCache"
            Me.m_btnClearCache.Size = New System.Drawing.Size(75, 23)
            Me.m_btnClearCache.TabIndex = 18
            Me.m_btnClearCache.Text = "&Clear cache"
            Me.m_btnClearCache.UseVisualStyleBackColor = True
            '
            'm_cmbNewDS
            '
            Me.m_cmbNewDS.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbNewDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbNewDS.FormattingEnabled = True
            Me.m_cmbNewDS.Location = New System.Drawing.Point(70, 21)
            Me.m_cmbNewDS.Name = "m_cmbNewDS"
            Me.m_cmbNewDS.Size = New System.Drawing.Size(160, 21)
            Me.m_cmbNewDS.TabIndex = 13
            '
            'm_btnConfigureCV
            '
            Me.m_btnConfigureCV.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConfigureCV.Location = New System.Drawing.Point(236, 215)
            Me.m_btnConfigureCV.Name = "m_btnConfigureCV"
            Me.m_btnConfigureCV.Size = New System.Drawing.Size(75, 23)
            Me.m_btnConfigureCV.TabIndex = 21
            Me.m_btnConfigureCV.Text = "C&onfigure..."
            Me.m_btnConfigureCV.UseVisualStyleBackColor = True
            '
            'm_btnDeleteDS
            '
            Me.m_btnDeleteDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnDeleteDS.Location = New System.Drawing.Point(236, 77)
            Me.m_btnDeleteDS.Name = "m_btnDeleteDS"
            Me.m_btnDeleteDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnDeleteDS.TabIndex = 17
            Me.m_btnDeleteDS.Text = "&Delete..."
            Me.m_btnDeleteDS.UseVisualStyleBackColor = True
            '
            'm_btnConfigDS
            '
            Me.m_btnConfigDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConfigDS.Location = New System.Drawing.Point(236, 48)
            Me.m_btnConfigDS.Name = "m_btnConfigDS"
            Me.m_btnConfigDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnConfigDS.TabIndex = 16
            Me.m_btnConfigDS.Text = "C&onfigure..."
            Me.m_btnConfigDS.UseVisualStyleBackColor = True
            '
            'm_btnCreateDS
            '
            Me.m_btnCreateDS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnCreateDS.Location = New System.Drawing.Point(236, 19)
            Me.m_btnCreateDS.Name = "m_btnCreateDS"
            Me.m_btnCreateDS.Size = New System.Drawing.Size(75, 23)
            Me.m_btnCreateDS.TabIndex = 14
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
            Me.m_lbxExistingDS.Location = New System.Drawing.Point(70, 48)
            Me.m_lbxExistingDS.Name = "m_lbxExistingDS"
            Me.m_lbxExistingDS.Size = New System.Drawing.Size(160, 143)
            Me.m_lbxExistingDS.TabIndex = 15
            '
            'm_lblSelectCV
            '
            Me.m_lblSelectCV.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblSelectCV.AutoSize = True
            Me.m_lblSelectCV.Location = New System.Drawing.Point(0, 219)
            Me.m_lblSelectCV.Name = "m_lblSelectCV"
            Me.m_lblSelectCV.Size = New System.Drawing.Size(56, 13)
            Me.m_lblSelectCV.TabIndex = 19
            Me.m_lblSelectCV.Text = "&Converter:"
            '
            'm_lblNewDS
            '
            Me.m_lblNewDS.AutoSize = True
            Me.m_lblNewDS.Location = New System.Drawing.Point(0, 24)
            Me.m_lblNewDS.Name = "m_lblNewDS"
            Me.m_lblNewDS.Size = New System.Drawing.Size(64, 13)
            Me.m_lblNewDS.TabIndex = 12
            Me.m_lblNewDS.Text = "&Connection:"
            '
            'm_hdrSource
            '
            Me.m_hdrSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrSource.CanCollapseParent = False
            Me.m_hdrSource.CollapsedParentHeight = 0
            Me.m_hdrSource.IsCollapsed = False
            Me.m_hdrSource.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrSource.Name = "m_hdrSource"
            Me.m_hdrSource.Size = New System.Drawing.Size(311, 18)
            Me.m_hdrSource.TabIndex = 11
            Me.m_hdrSource.Text = "External data"
            Me.m_hdrSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_plScalarAdapter
            '
            Me.m_plScalarAdapter.Controls.Add(Me.m_lblScaling)
            Me.m_plScalarAdapter.Controls.Add(Me.m_btnCalculate)
            Me.m_plScalarAdapter.Controls.Add(Me.m_tbxScale)
            Me.m_plScalarAdapter.Controls.Add(Me.m_rbRelative)
            Me.m_plScalarAdapter.Controls.Add(Me.m_rbAbsolute)
            Me.m_plScalarAdapter.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plScalarAdapter.Location = New System.Drawing.Point(0, 237)
            Me.m_plScalarAdapter.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plScalarAdapter.Name = "m_plScalarAdapter"
            Me.m_plScalarAdapter.Size = New System.Drawing.Size(311, 79)
            Me.m_plScalarAdapter.TabIndex = 2
            '
            'm_btnCalculate
            '
            Me.m_btnCalculate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnCalculate.Location = New System.Drawing.Point(236, 56)
            Me.m_btnCalculate.Name = "m_btnCalculate"
            Me.m_btnCalculate.Size = New System.Drawing.Size(75, 23)
            Me.m_btnCalculate.TabIndex = 8
            Me.m_btnCalculate.Text = "Calculate"
            Me.m_btnCalculate.UseVisualStyleBackColor = True
            '
            'm_tbxScale
            '
            Me.m_tbxScale.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxScale.Location = New System.Drawing.Point(123, 58)
            Me.m_tbxScale.Name = "m_tbxScale"
            Me.m_tbxScale.Size = New System.Drawing.Size(107, 20)
            Me.m_tbxScale.TabIndex = 7
            '
            'm_rbRelative
            '
            Me.m_rbRelative.AutoSize = True
            Me.m_rbRelative.Location = New System.Drawing.Point(16, 59)
            Me.m_rbRelative.Name = "m_rbRelative"
            Me.m_rbRelative.Size = New System.Drawing.Size(101, 17)
            Me.m_rbRelative.TabIndex = 6
            Me.m_rbRelative.TabStop = True
            Me.m_rbRelative.Text = "&Scale values to:"
            Me.m_rbRelative.UseVisualStyleBackColor = True
            '
            'm_rbAbsolute
            '
            Me.m_rbAbsolute.AutoSize = True
            Me.m_rbAbsolute.Location = New System.Drawing.Point(16, 36)
            Me.m_rbAbsolute.Name = "m_rbAbsolute"
            Me.m_rbAbsolute.Size = New System.Drawing.Size(199, 17)
            Me.m_rbAbsolute.TabIndex = 5
            Me.m_rbAbsolute.TabStop = True
            Me.m_rbAbsolute.Text = "Use external data as &absolute values"
            Me.m_rbAbsolute.UseVisualStyleBackColor = True
            '
            'm_lblScaling
            '
            Me.m_lblScaling.AutoSize = True
            Me.m_lblScaling.Location = New System.Drawing.Point(0, 18)
            Me.m_lblScaling.Name = "m_lblScaling"
            Me.m_lblScaling.Size = New System.Drawing.Size(108, 13)
            Me.m_lblScaling.TabIndex = 9
            Me.m_lblScaling.Text = "External data scaling:"
            '
            'ucConfigAdapter
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpContent)
            Me.Name = "ucConfigAdapter"
            Me.Size = New System.Drawing.Size(311, 316)
            Me.m_tlpContent.ResumeLayout(False)
            Me.m_plConnectionConverter.ResumeLayout(False)
            Me.m_plConnectionConverter.PerformLayout()
            Me.m_plScalarAdapter.ResumeLayout(False)
            Me.m_plScalarAdapter.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plConnectionConverter As System.Windows.Forms.Panel
        Private WithEvents m_cmbConverter As System.Windows.Forms.ComboBox
        Private WithEvents m_btnClearCache As System.Windows.Forms.Button
        Private WithEvents m_cmbNewDS As System.Windows.Forms.ComboBox
        Private WithEvents m_btnConfigureCV As System.Windows.Forms.Button
        Private WithEvents m_btnDeleteDS As System.Windows.Forms.Button
        Private WithEvents m_btnConfigDS As System.Windows.Forms.Button
        Private WithEvents m_btnCreateDS As System.Windows.Forms.Button
        Private WithEvents m_lbxExistingDS As System.Windows.Forms.ListBox
        Private WithEvents m_lblSelectCV As System.Windows.Forms.Label
        Private WithEvents m_lblNewDS As System.Windows.Forms.Label
        Private WithEvents m_hdrSource As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_plScalarAdapter As System.Windows.Forms.Panel
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button
        Private WithEvents m_tbxScale As System.Windows.Forms.TextBox
        Private WithEvents m_rbRelative As System.Windows.Forms.RadioButton
        Private WithEvents m_rbAbsolute As System.Windows.Forms.RadioButton
        Private WithEvents m_lblScaling As System.Windows.Forms.Label

    End Class

End Namespace
