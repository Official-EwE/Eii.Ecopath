' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class frmSelectPreyPredator
    Inherits EwEResultsExtractor.CreateCollectionForData

    Public Event FormExited()

    Public Sub New(i As cSelectionData, ByRef p As cCore)
        MyBase.New(i, p)
        Me.InitializeComponent()

        Me.Show()

    End Sub

    Public Overrides Sub PopulateAttachedList(Prey As String)
        Dim PreyIndex As Integer

        'Clear the list
        Me.chklstAttached.Items.Clear()

        If Prey = Nothing Then 'don't do anything

            Exit Sub

        Else 'else populate list

            ' Find the index number to refer to selected predator
            PreyIndex = 1
            Do While Me.Core.EcosimGroupOutputs(PreyIndex).Name IsNot Prey
                PreyIndex += 1
            Loop

            'Check which functional groups are prey to given predator and add to prey chklist
            With Me.chklstAttached.Items
                For i As Integer = 1 To Me.Core.nGroups
                    If Me.Core.EcopathGroupInputs(i).IsPrey(PreyIndex) Then
                        .Add(Me.Core.EcosimGroupOutputs(i).Name)
                    End If
                Next
            End With

        End If

    End Sub

    Private Sub frmSelectPreyPredator_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class