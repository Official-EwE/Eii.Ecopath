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

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Other

    Partial Class ucAutorunOption
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAutorunOption))
            Me.m_cbOption = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            '
            'm_cbOption
            '
            resources.ApplyResources(Me.m_cbOption, "m_cbOption")
            Me.m_cbOption.Name = "m_cbOption"
            Me.m_cbOption.UseVisualStyleBackColor = True
            '
            'ucAutorunOption
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cbOption)
            Me.Name = "ucAutorunOption"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_cbOption As System.Windows.Forms.CheckBox

    End Class

End Namespace
