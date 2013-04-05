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

Option Strict On
Imports EwECore

Namespace Controls.Map.Layers

    Public Class ucLayerEditorRegion

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim iNumRegions As Integer = Me.UIContext.Core.nRegions

            Me.Editor.CellValueMax = iNumRegions
            Me.m_nudRegion.Maximum = iNumRegions

        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditor)
            MyBase.UpdateContent(editor)
            If (Me.UIContext Is Nothing) Then Return

            Dim iVal As Integer

            ' Sanity check
            If (Me.m_nudRegion Is Nothing) Then Return

            ' Set control value
            iVal = CInt(editor.CellValue)

            Me.m_nudRegion.Value = iVal
            Me.m_nudRegion.Maximum = CDec(editor.CellValueMax)

            ' Whooh

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub OnDrawRegionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudRegion.ValueChanged

            If (Me.UIContext Is Nothing) Then Return

            Me.Editor.CellValue = CInt(Me.m_nudRegion.Value)

        End Sub

#End Region ' Event handlers

    End Class

End Namespace
