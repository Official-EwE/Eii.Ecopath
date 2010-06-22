Imports EwECore

Public Class frmSelectPreyPredator
    Inherits EwEResultsExtractor.CreateCollectionForData

    Public Event FormExited()

    Public Sub New(ByVal i As cSelectionData, ByRef p As cCore)
        MyBase.New(i, p)
        InitializeComponent()

        Me.Show()

    End Sub

    Public Overrides Sub PopulateAttachedList(ByVal Prey As String)
        Dim PreyIndex As Integer

        'Clear the list
        Me.chklstAttached.Items.Clear()

        If Prey = Nothing Then 'don't do anything

            Exit Sub

        Else 'else populate list

            ' Find the index number to refer to selected predator
            PreyIndex = 1
            Do While m_core.EcoSimGroupOutputs(PreyIndex).Name IsNot Prey
                PreyIndex += 1
            Loop

            'Check which functional groups are prey to given predator and add to prey chklist
            With Me.chklstAttached.Items
                For i As Integer = 1 To m_core.nGroups
                    If m_core.EcoSimGroupOutputs(i).isPrey(PreyIndex) Then
                        .Add(m_core.EcoSimGroupOutputs(i).Name)
                    End If
                Next
            End With

        End If

    End Sub

    Private Sub frmSelectPreyPredator_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If frmResults.FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class