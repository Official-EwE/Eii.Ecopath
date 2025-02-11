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

Imports ScientificInterfaceShared

Namespace Other

    Partial Class ucOptionsFonts
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsFonts))
            Me.m_nudFontSize = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblFontSize = New System.Windows.Forms.Label()
            Me.m_hdrFont = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbFontStyle = New System.Windows.Forms.ComboBox()
            Me.m_lblItemFontStyle = New System.Windows.Forms.Label()
            Me.m_cbFontFamily = New System.Windows.Forms.ComboBox()
            Me.m_lblItemForeColor = New System.Windows.Forms.Label()
            Me.m_lbFontTypes = New System.Windows.Forms.ListBox()
            Me.m_lblPreview = New System.Windows.Forms.Label()
            Me.m_lblDescription = New System.Windows.Forms.Label()
            Me.m_plPreview = New System.Windows.Forms.Panel()
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_nudFontSize
            '
            resources.ApplyResources(Me.m_nudFontSize, "m_nudFontSize")
            Me.m_nudFontSize.DecimalPlaces = 2
            Me.m_nudFontSize.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudFontSize.Maximum = New Decimal(New Integer() {24, 0, 0, 0})
            Me.m_nudFontSize.Minimum = New Decimal(New Integer() {4, 0, 0, 0})
            Me.m_nudFontSize.Name = "m_nudFontSize"
            Me.m_nudFontSize.Value = New Decimal(New Integer() {825, 0, 0, 131072})
            '
            'm_lblFontSize
            '
            resources.ApplyResources(Me.m_lblFontSize, "m_lblFontSize")
            Me.m_lblFontSize.Name = "m_lblFontSize"
            '
            'm_hdrFont
            '
            Me.m_hdrFont.CanCollapseParent = False
            Me.m_hdrFont.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrFont, "m_hdrFont")
            Me.m_hdrFont.IsCollapsed = False
            Me.m_hdrFont.Name = "m_hdrFont"
            '
            'm_cbFontStyle
            '
            resources.ApplyResources(Me.m_cbFontStyle, "m_cbFontStyle")
            Me.m_cbFontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbFontStyle.FormattingEnabled = True
            Me.m_cbFontStyle.Items.AddRange(New Object() {resources.GetString("m_cbFontStyle.Items"), resources.GetString("m_cbFontStyle.Items1"), resources.GetString("m_cbFontStyle.Items2"), resources.GetString("m_cbFontStyle.Items3")})
            Me.m_cbFontStyle.Name = "m_cbFontStyle"
            '
            'm_lblItemFontStyle
            '
            resources.ApplyResources(Me.m_lblItemFontStyle, "m_lblItemFontStyle")
            Me.m_lblItemFontStyle.Name = "m_lblItemFontStyle"
            '
            'm_cbFontFamily
            '
            resources.ApplyResources(Me.m_cbFontFamily, "m_cbFontFamily")
            Me.m_cbFontFamily.BackColor = System.Drawing.Color.White
            Me.m_cbFontFamily.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_cbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbFontFamily.FormattingEnabled = True
            Me.m_cbFontFamily.Name = "m_cbFontFamily"
            '
            'm_lblItemForeColor
            '
            resources.ApplyResources(Me.m_lblItemForeColor, "m_lblItemForeColor")
            Me.m_lblItemForeColor.Name = "m_lblItemForeColor"
            '
            'm_lbFontTypes
            '
            resources.ApplyResources(Me.m_lbFontTypes, "m_lbFontTypes")
            Me.m_lbFontTypes.Name = "m_lbFontTypes"
            '
            'm_lblPreview
            '
            resources.ApplyResources(Me.m_lblPreview, "m_lblPreview")
            Me.m_lblPreview.Name = "m_lblPreview"
            '
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_plPreview
            '
            resources.ApplyResources(Me.m_plPreview, "m_plPreview")
            Me.m_plPreview.Name = "m_plPreview"
            '
            'ucOptionsFonts
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_nudFontSize)
            Me.Controls.Add(Me.m_lblFontSize)
            Me.Controls.Add(Me.m_hdrFont)
            Me.Controls.Add(Me.m_cbFontStyle)
            Me.Controls.Add(Me.m_lblPreview)
            Me.Controls.Add(Me.m_lblItemFontStyle)
            Me.Controls.Add(Me.m_cbFontFamily)
            Me.Controls.Add(Me.m_lblItemForeColor)
            Me.Controls.Add(Me.m_lbFontTypes)
            Me.Name = "ucOptionsFonts"
            CType(Me.m_nudFontSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblFontSize As System.Windows.Forms.Label
        Private WithEvents m_hdrFont As cEwEHeaderLabel
        Private WithEvents m_lblItemFontStyle As System.Windows.Forms.Label
        Private WithEvents m_cbFontFamily As System.Windows.Forms.ComboBox
        Private WithEvents m_lblItemForeColor As System.Windows.Forms.Label
        Private WithEvents m_lbFontTypes As System.Windows.Forms.ListBox
        Private WithEvents m_cbFontStyle As System.Windows.Forms.ComboBox
        Private WithEvents m_lblPreview As System.Windows.Forms.Label
        Private WithEvents m_nudFontSize As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_lblDescription As Label
        Private WithEvents m_plPreview As Panel
    End Class
End Namespace

