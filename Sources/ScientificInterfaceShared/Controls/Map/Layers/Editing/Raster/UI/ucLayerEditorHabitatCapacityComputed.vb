' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    Public Class ucLayerEditorHabitatCapacityComputed

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim group As cCoreGroupBase = Nothing

            Me.m_cmbGroups.Items.Clear()

            For iGroup As Integer = 1 To core.nGroups
                group = core.EcopathGroupInputs(iGroup)
                Me.m_cmbGroups.Items.Add(group)
            Next iGroup

            ' Update control
            Me.m_cmbGroups.SelectedIndex = Me.GroupIndex - 1

        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditorRaster)
            MyBase.UpdateContent(editor)

            Me.m_cmbGroups.Enabled = Me.IsAttached

        End Sub

        Protected Overloads Property Editor() As cLayerEditorGroup
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorGroup)
            End Get
            Set(editor As cLayerEditorGroup)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorGroup, "ucLayerEditorGroup connected to wrong editor class")
                ' Configure editor
                editor.CellValue = 0
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

        Protected Property GroupIndex() As Integer
            Get
                If (Not Me.IsAttached) Then Return cCore.NULL_VALUE
                Return Me.Editor.Group
            End Get
            Set(value As Integer)
                If (Me.IsAttached) Then
                    If (Me.Editor.Group <> value) Then
                        Me.Editor.Group = value
                    End If
                End If
            End Set
        End Property

#Region " Events "

        Private Sub OnGroupSelectionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbGroups.SelectedIndexChanged
            Me.GroupIndex = Me.m_cmbGroups.SelectedIndex + 1
        End Sub

        Private Sub OnFormatItemText(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_cmbGroups.Format
            Dim io As cCoreInputOutputBase = DirectCast(e.ListItem, cCoreInputOutputBase)
            Dim fmt As New cCoreInterfaceFormatter()
            e.Value = fmt.ToString(io)
        End Sub

        Private Sub OnSetDefaultAllClick(sender As System.Object, e As System.EventArgs)


            Dim ngrps As Integer = Me.UIContext.Core.nGroups

            Try
                Dim capManager As EwECore.cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
                Dim map As cEcospaceLayerHabitatCapacity
                For igrp As Integer = 1 To ngrps
                    map = capManager.LayerHabitatCapacityInput(igrp)
                    map.Reset()
                Next igrp

                Me.UpdateCore()
            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnSetDefaultThis(sender As System.Object, e As System.EventArgs) Handles m_btnComputeCap.Click
            Try
                Dim capManager As EwECore.cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
                capManager.LayerHabitatCapacity(Me.GroupIndex).Reset()
                Me.UpdateCore()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdateCore()
            Me.UIContext.Core.onChanged(Me.UIContext.Core.EcospaceBasemap.LayerHabitatCapacityInput(1))
        End Sub

#End Region ' Internals

    End Class

End Namespace
