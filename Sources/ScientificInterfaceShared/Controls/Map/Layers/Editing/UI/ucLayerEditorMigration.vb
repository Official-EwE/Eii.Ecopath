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
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    Public Class ucLayerEditorMigration

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Private Sub OnMonthChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbMonth.SelectedIndexChanged
            Try
                Me.Editor.CellValue = Me.m_cmbMonth.SelectedIndex + 1
            Catch ex As Exception
            End Try
        End Sub

        Private Sub OnGroupChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbGroup.SelectedIndexChanged
            Try
                Me.Editor.Group = Me.m_cmbGroup.SelectedIndex + 1
            Catch ex As Exception
            End Try
        End Sub

        Public Overrides Sub EndEdit(ByVal editor As cLayerEditor)
            If Me.m_chkAutoRotate.Checked Then
                Try
                    Me.Editor.CellValue = CInt(CInt(editor.CellValue) Mod cCore.N_MONTHS) + 1
                Catch ex As Exception

                End Try
            End If
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim group As cEcoPathGroupInput = Nothing
            Dim fmt As New cCoreInterfaceFormatter()

            Me.m_cmbGroup.Items.Clear()

            ' ToDo: this control will not respond to dynamic group name changes
            For iGroup As Integer = 1 To core.nLivingGroups
                group = core.EcoPathGroupInputs(iGroup)
                Me.m_cmbGroup.Items.Add(fmt.GetDescriptor(group))
            Next iGroup

            Me.UpdateContent(Me.Editor)
        End Sub

        Public Overrides Sub UpdateContent(ByVal editor As cLayerEditor)
            MyBase.UpdateContent(editor)

            If (Me.m_cmbGroup Is Nothing) Then Return
            If (Me.m_cmbMonth Is Nothing) Then Return
            If (Me.m_chkAutoRotate Is Nothing) Then Return

            If (editor IsNot Nothing) Then
                Try
                    Me.m_cmbMonth.SelectedIndex = CInt(editor.CellValue) - 1
                    Me.m_cmbGroup.SelectedIndex = CInt(Me.Editor.Group) - 1

                    Me.m_chkAutoRotate.Enabled = editor.IsEditable
                Catch ex As Exception

                End Try
            End If

        End Sub

        Protected Overloads Property Editor() As cLayerEditorMigration
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorMigration)
            End Get
            Set(ByVal editor As cLayerEditorMigration)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorMigration, "ucLayerEditorMigration connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 1
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

    End Class

End Namespace

