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

    Public Class ucLayerEditorSailCost

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)
            ' May be cleaning up
            If (Not Me.IsAttached Or Me.Editor Is Nothing) Then Return
            ' Okidoki
            Me.m_btnCalculate.Enabled = editor.IsEditable
        End Sub

        Private Sub OnCalculate(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Me.UIContext.Core.CalcEcospaceCostOfSailing()
        End Sub

        Private Sub OnSmooth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSmooth.Click
            Me.Editor.Smooth()
        End Sub

    End Class

End Namespace
