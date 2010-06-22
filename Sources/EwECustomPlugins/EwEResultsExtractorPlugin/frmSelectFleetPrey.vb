Imports EwECore

Public Class frmSelectFleetPrey
    Inherits EwEResultsExtractor.CreateCollectionForData

    Public Event FormExited()

    Public Sub New(ByVal i As cSelectionData, ByVal p As cCore)
        MyBase.New(i, p)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Show()

    End Sub

    Public Overrides Sub PopulateAttachedList(ByVal Fleet As String)

        Dim FleetIndex As Integer
        Dim TotalCatch As Single

        'Clear the list
        Me.chklstAttached.Items.Clear()

        If Fleet = Nothing Then 'don't do anything

            Exit Sub

        Else 'else populate list

            ' Find the index number to refer to selected fleet
            FleetIndex = 0
            Do While m_core.EcosimFleetOutput(FleetIndex).Name IsNot Fleet
                FleetIndex += 1
            Loop

            'Check which functional groups are prey to given fleet and add to prey chklist
            With Me.chklstAttached.Items
                For i As Integer = 1 To m_core.nGroups
                    TotalCatch = 0
                    For p = 0 To m_core.nEcosimTimeSteps
                        TotalCatch += m_core.EcoSimGroupOutputs(i).CatchByFleet(FleetIndex, p)
                    Next
                    If TotalCatch > 0 Then
                        .Add(m_core.EcoSimGroupOutputs(i).Name)
                    End If
                Next
            End With

        End If

    End Sub

    Private Sub frmSelectFleetPrey_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class