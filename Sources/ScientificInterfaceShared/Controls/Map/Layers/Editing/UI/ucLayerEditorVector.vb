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
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls.Map.Layers

    Public Class ucLayerEditorVector

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            ' Sanity checks
            If (Me.Editor Is Nothing) Then Return
            If (Me.m_nudValue Is Nothing) Then Return

            Me.m_nudValue.Value = editor.CursorSize
            Me.m_nudValue.Enabled = editor.IsEditable

        End Sub

        Public Shadows Property Editor() As cLayerEditorVector
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorVector)
            End Get
            Set(ByVal value As cLayerEditorVector)
                MyBase.Editor = value
            End Set
        End Property

        Private Sub OnValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudValue.ValueChanged

            If Me.Editor Is Nothing Then Return

            Me.Editor.CellValue = CSng(Me.m_nudValue.Value)
            Me.RaiseChangedEvent()

        End Sub

    End Class

End Namespace
