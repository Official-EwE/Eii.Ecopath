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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Controls

    Partial Class ucEditImage
        Inherits ucEditVisualStyle

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.m_glyphSelect = New ScientificInterfaceShared.Controls.ucGlyphSelect()
            Me.btnImport = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_glyphSelect
            '
            Me.m_glyphSelect.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_glyphSelect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_glyphSelect.GlyphSize = New System.Drawing.Size(40, 25)
            Me.m_glyphSelect.Location = New System.Drawing.Point(0, 0)
            Me.m_glyphSelect.MaxImageSize = New System.Drawing.Size(100, 100)
            Me.m_glyphSelect.Name = "m_glyphSelect"
            Me.m_glyphSelect.SelectedImage = Nothing
            Me.m_glyphSelect.SelectedIndex = -1
            Me.m_glyphSelect.Size = New System.Drawing.Size(307, 135)
            Me.m_glyphSelect.TabIndex = 2
            Me.m_glyphSelect.TabStop = False
            '
            'btnImport
            '
            Me.btnImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnImport.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.btnImport.Location = New System.Drawing.Point(247, 141)
            Me.btnImport.Name = "btnImport"
            Me.btnImport.Size = New System.Drawing.Size(60, 23)
            Me.btnImport.TabIndex = 1
            Me.btnImport.Text = "&Import..."
            Me.btnImport.UseVisualStyleBackColor = True
            '
            'ucEditImage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Controls.Add(Me.m_glyphSelect)
            Me.Controls.Add(Me.btnImport)
            Me.Name = "ucEditImage"
            Me.Size = New System.Drawing.Size(307, 164)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_glyphSelect As ucGlyphSelect
        Friend WithEvents btnImport As System.Windows.Forms.Button

    End Class

End Namespace
