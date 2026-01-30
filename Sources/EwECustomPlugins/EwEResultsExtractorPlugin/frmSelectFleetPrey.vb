' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class frmSelectFleetPrey
    Inherits EwEResultsExtractor.CreateCollectionForData

    Public Event FormExited()

    Public Sub New(i As cSelectionData, p As cCore)
        MyBase.New(i, p)
        ' This call is required by the Windows Form Designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Show()

    End Sub

    Public Overrides Sub PopulateAttachedList(Fleet As String)

        Dim FleetIndex As Integer
        Dim TotalCatch As Single

        'Clear the list
        Me.chklstAttached.Items.Clear()

        If Fleet = Nothing Then 'don't do anything

            Exit Sub

        Else 'else populate list

            ' Find the index number to refer to selected fleet
            FleetIndex = 0
            Do While Me.Core.EcosimFleetOutput(FleetIndex).Name IsNot Fleet
                FleetIndex += 1
            Loop

            'Check which functional groups are prey to given fleet and add to prey chklist
            With Me.chklstAttached.Items
                For i As Integer = 1 To Me.Core.nGroups
                    TotalCatch = 0
                    For p = 0 To Me.Core.nEcosimTimeSteps
                        TotalCatch += Me.Core.EcosimGroupOutputs(i).CatchByFleet(FleetIndex, p)
                    Next
                    If TotalCatch > 0 Then
                        .Add(Me.Core.EcosimGroupOutputs(i).Name)
                    End If
                Next
            End With

        End If

    End Sub

    Private Sub frmSelectFleetPrey_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class