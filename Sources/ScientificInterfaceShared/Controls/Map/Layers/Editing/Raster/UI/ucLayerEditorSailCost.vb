' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    Public Class ucLayerEditorSailCost

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Not Me.IsAttached) Then Return

            ' Initialize group combo 
            Dim core As cCore = Me.UIContext.Core
            Dim fleet As cEcopathFleetInput = Nothing

            Me.m_cmbFleet.Items.Clear()
            For i As Integer = 1 To core.nFleets
                fleet = core.EcopathFleetInputs(i)
                Me.m_cmbFleet.Items.Add(fleet)
            Next i

            ' Update control
            Me.m_cmbFleet.SelectedIndex = Math.Max(0, Me.FleetIndex - 1)

        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditorRaster)
            MyBase.UpdateContent(editor)
            ' May be cleaning up
            If (Not Me.IsAttached Or Me.Editor Is Nothing) Then Return
            ' Okidoki
            Me.m_btnCalculate.Enabled = editor.IsEditable
        End Sub

        Private Sub OnCalculate(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCalculate.Click
            Me.UIContext.Core.CalcEcospaceCostOfSailing()
        End Sub

        Protected Overloads Property Editor() As cLayerEditorSailCost
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorSailCost)
            End Get
            Set(editor As cLayerEditorSailCost)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorSailCost, "ucLayerEditorSailCost connected to wrong editor class")
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
            Set(value As Integer)
                If (Me.IsAttached) Then
                    If (Me.Editor.Fleet <> value) Then
                        Me.Editor.Fleet = value
                    End If
                End If
            End Set
        End Property

        Private Sub OnFormatItemText(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_cmbFleet.Format
            Dim io As cCoreInputOutputBase = DirectCast(e.ListItem, cCoreInputOutputBase)
            Dim fmt As New cCoreInterfaceFormatter()
            e.Value = fmt.ToString(io)
        End Sub

        Private Sub OnFleetSelectionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbFleet.SelectedIndexChanged
            Dim item As Object = Me.m_cmbFleet.SelectedItem
            Me.FleetIndex = DirectCast(item, cCoreInputOutputBase).Index
        End Sub

    End Class

End Namespace
