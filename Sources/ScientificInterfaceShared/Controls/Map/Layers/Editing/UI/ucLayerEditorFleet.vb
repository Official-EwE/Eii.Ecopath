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
#Region " Imports "

Option Strict On
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    Public Class ucLayerEditorFleet

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim fleet As cFleetInput = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            Me.m_cmbFleet.Items.Clear()

            ' ToDo: this control will not respond to dynamic fleet name changes
            Me.m_cmbFleet.Items.Add(SharedResources.GENERIC_VALUE_ALL)
            For iGroup As Integer = 1 To core.nFleets
                fleet = core.FleetInputs(iGroup)
                Me.m_cmbFleet.Items.Add(fmt.GetDescriptor(fleet))
            Next iGroup

            ' Update control
            Me.m_cmbFleet.SelectedIndex = Me.FleetIndex

        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)
            Me.m_cmbFleet.Enabled = Me.IsAttached
        End Sub

        Protected Overloads Property Editor() As cLayerEditorFleet
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorFleet)
            End Get
            Set(ByVal editor As cLayerEditorFleet)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorFleet, "ucLayerEditorFleet connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 0
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

        Protected Property FleetIndex() As Integer
            Get
                If (Not Me.IsAttached) Then Return cCore.NULL_VALUE
                Return Me.Editor.Fleet
            End Get
            Set(ByVal value As Integer)
                If (Me.IsAttached) Then
                    If (Me.Editor.Fleet <> value) Then
                        Me.Editor.Fleet = value
                    End If
                End If
            End Set
        End Property

        Private Sub OnFleetSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbFleet.SelectedIndexChanged
            Me.FleetIndex = Me.m_cmbFleet.SelectedIndex
        End Sub

    End Class

End Namespace

