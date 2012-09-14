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
Namespace Ecospace

    Partial Class dlgSelectResponse
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSelectResponse))
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_lvAllShapes = New System.Windows.Forms.ListView
            Me.m_lblResponseFunctions = New System.Windows.Forms.Label
            Me.m_lvAppliedShapes = New System.Windows.Forms.ListView
            Me.m_lblAppliedResponse = New System.Windows.Forms.Label
            Me.m_btnRemove = New System.Windows.Forms.Button
            Me.m_btnAdd = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'OK_Button
            '
            Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.OK_Button.Location = New System.Drawing.Point(234, 261)
            Me.OK_Button.Name = "OK_Button"
            Me.OK_Button.Size = New System.Drawing.Size(67, 23)
            Me.OK_Button.TabIndex = 0
            Me.OK_Button.Text = "OK"
            '
            'Cancel_Button
            '
            Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Location = New System.Drawing.Point(307, 261)
            Me.Cancel_Button.Name = "Cancel_Button"
            Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
            Me.Cancel_Button.TabIndex = 1
            Me.Cancel_Button.Text = "Cancel"
            '
            'm_lvAllShapes
            '
            Me.m_lvAllShapes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lvAllShapes.FullRowSelect = True
            Me.m_lvAllShapes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.m_lvAllShapes.HideSelection = False
            Me.m_lvAllShapes.Location = New System.Drawing.Point(12, 25)
            Me.m_lvAllShapes.MultiSelect = False
            Me.m_lvAllShapes.Name = "m_lvAllShapes"
            Me.m_lvAllShapes.Size = New System.Drawing.Size(160, 230)
            Me.m_lvAllShapes.TabIndex = 1
            Me.m_lvAllShapes.UseCompatibleStateImageBehavior = False
            Me.m_lvAllShapes.View = System.Windows.Forms.View.Details
            '
            'm_lblResponseFunctions
            '
            Me.m_lblResponseFunctions.AutoSize = True
            Me.m_lblResponseFunctions.Location = New System.Drawing.Point(12, 9)
            Me.m_lblResponseFunctions.Name = "m_lblResponseFunctions"
            Me.m_lblResponseFunctions.Size = New System.Drawing.Size(104, 13)
            Me.m_lblResponseFunctions.TabIndex = 2
            Me.m_lblResponseFunctions.Text = "&Response functions:"
            '
            'm_lvAppliedShapes
            '
            Me.m_lvAppliedShapes.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lvAppliedShapes.FullRowSelect = True
            Me.m_lvAppliedShapes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.m_lvAppliedShapes.HideSelection = False
            Me.m_lvAppliedShapes.Location = New System.Drawing.Point(210, 25)
            Me.m_lvAppliedShapes.Name = "m_lvAppliedShapes"
            Me.m_lvAppliedShapes.Size = New System.Drawing.Size(162, 230)
            Me.m_lvAppliedShapes.TabIndex = 3
            Me.m_lvAppliedShapes.UseCompatibleStateImageBehavior = False
            '
            'm_lblAppliedResponse
            '
            Me.m_lblAppliedResponse.AutoSize = True
            Me.m_lblAppliedResponse.Location = New System.Drawing.Point(207, 9)
            Me.m_lblAppliedResponse.Name = "m_lblAppliedResponse"
            Me.m_lblAppliedResponse.Size = New System.Drawing.Size(91, 13)
            Me.m_lblAppliedResponse.TabIndex = 4
            Me.m_lblAppliedResponse.Text = "&Applied response:"
            '
            'm_btnRemove
            '
            Me.m_btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.m_btnRemove.Image = CType(resources.GetObject("m_btnRemove.Image"), System.Drawing.Image)
            Me.m_btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_btnRemove.Location = New System.Drawing.Point(178, 145)
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.Size = New System.Drawing.Size(26, 23)
            Me.m_btnRemove.TabIndex = 9
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'm_btnAdd
            '
            Me.m_btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.m_btnAdd.Image = CType(resources.GetObject("m_btnAdd.Image"), System.Drawing.Image)
            Me.m_btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_btnAdd.Location = New System.Drawing.Point(178, 116)
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.Size = New System.Drawing.Size(26, 23)
            Me.m_btnAdd.TabIndex = 8
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'dlgSelectResponse
            '
            Me.AcceptButton = Me.OK_Button
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ClientSize = New System.Drawing.Size(384, 296)
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_btnRemove)
            Me.Controls.Add(Me.Cancel_Button)
            Me.Controls.Add(Me.m_btnAdd)
            Me.Controls.Add(Me.m_lblAppliedResponse)
            Me.Controls.Add(Me.m_lvAppliedShapes)
            Me.Controls.Add(Me.m_lblResponseFunctions)
            Me.Controls.Add(Me.m_lvAllShapes)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(390, 228)
            Me.Name = "dlgSelectResponse"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Apply environmental response function"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_lvAllShapes As System.Windows.Forms.ListView
        Private WithEvents m_lvAppliedShapes As System.Windows.Forms.ListView
        Private WithEvents m_btnRemove As System.Windows.Forms.Button
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents m_lblResponseFunctions As System.Windows.Forms.Label
        Private WithEvents m_lblAppliedResponse As System.Windows.Forms.Label
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button

    End Class

End Namespace
