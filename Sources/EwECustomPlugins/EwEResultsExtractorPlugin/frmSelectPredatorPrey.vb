Imports EwECore
Imports EwEResultsExtractor.frmResults
Imports System.Drawing

Public Class frmSelectPredatorPrey
    Inherits EwEResultsExtractor.CreateCollectionForData

    Public Event FormExited()


    Public Sub New(ByVal i As cSelectionData, ByRef p As cCore)
        MyBase.New(i, p)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Me.m_core = cCore.GetInstance()
        Me.Show()

    End Sub

    Public Overrides Sub PopulateAttachedList(ByVal Predator As String)

        Dim PredIndex As Integer

        'Clear the list
        Me.chklstAttached.Items.Clear()

        If Predator = Nothing Then 'don't do anything

            Exit Sub

        Else 'else populate list

            ' Find the index number to refer to selected predator
            PredIndex = 1
            Do While m_core.EcoSimGroupOutputs(PredIndex).Name IsNot Predator
                PredIndex += 1
            Loop

            'Check which functional groups are prey to given predator and add to prey chklist
            With Me.chklstAttached.Items
                For i As Integer = 1 To m_core.nGroups
                    If m_core.EcoSimGroupOutputs(PredIndex).isPrey(i) Then
                        .Add(m_core.EcoSimGroupOutputs(i).Name)
                    End If
                Next
            End With

        End If

    End Sub

    Private Sub frmSelectPredatorPrey_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If FireChecked = False Then
            frmResults.NextAction()
        End If
        RaiseEvent FormExited()
    End Sub
End Class