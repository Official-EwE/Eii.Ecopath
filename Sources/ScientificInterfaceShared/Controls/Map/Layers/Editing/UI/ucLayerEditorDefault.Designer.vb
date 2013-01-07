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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorDefault
        Inherits ucLayerEditor

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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorDefault))
            Me.m_ucSlider = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_lblCursor = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'm_ucSlider
            '
            resources.ApplyResources(Me.m_ucSlider, "m_ucSlider")
            Me.m_ucSlider.Maximum = 6
            Me.m_ucSlider.Minimum = 1
            Me.m_ucSlider.Name = "m_ucSlider"
            Me.m_ucSlider.Value = 1
            '
            'm_lblCursor
            '
            resources.ApplyResources(Me.m_lblCursor, "m_lblCursor")
            Me.m_lblCursor.Name = "m_lblCursor"
            '
            'ucLayerEditorDefault
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblCursor)
            Me.Controls.Add(Me.m_ucSlider)
            Me.Name = "ucLayerEditorDefault"
            Me.Controls.SetChildIndex(Me.m_ucSlider, 0)
            Me.Controls.SetChildIndex(Me.m_lblCursor, 0)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_ucSlider As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_lblCursor As System.Windows.Forms.Label

    End Class

End Namespace