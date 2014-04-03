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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorDefault

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity checks
            If (editor Is Nothing) Then Return
            If (Me.m_ucSlider Is Nothing) Then Return

            Dim bEnabled As Boolean = editor.IsEditable

            Me.m_ucSlider.Value = editor.CursorSize
            Me.m_ucSlider.Enabled = bEnabled
            Me.m_lblCursor.Enabled = bEnabled

            If (Me.Layer IsNot Nothing) Then
                Me.m_tbxName.Text = Me.Layer.Name
                Me.m_tbxMin.Text = cStringUtils.FormatNumber(Me.Layer.Data.MinValue)
                Me.m_tbxMax.Text = cStringUtils.FormatNumber(Me.Layer.Data.MaxValue)
            End If

        End Sub

        Private Sub OnSliderValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_ucSlider.ValueChanged

            If Me.Editor Is Nothing Then Return

            Me.Editor.CursorSize = CInt(Me.m_ucSlider.Value)
            Me.RaiseChangedEvent()

        End Sub

    End Class

End Namespace
