#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core

#End Region

Public Class frmMSEResults

    Private m_EventSource As cMSEEventSource

    Public Sub New()
        InitializeComponent()
        m_EventSource = New cMSEEventSource
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.rbFleet.Tag = ScientificInterface.gridRiskResults.eGridType.Fleet
        Me.rbGroup.Tag = ScientificInterface.gridRiskResults.eGridType.Group

        Me.Grid.UIContext = Me.UIContext

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        AddHandler Me.m_EventSource.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        AddHandler Me.m_EventSource.onRunCompleted, AddressOf Me.onRunCompleted

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        RemoveHandler Me.m_EventSource.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        RemoveHandler Me.m_EventSource.onRunCompleted, AddressOf Me.onRunCompleted
        MyBase.OnFormClosed(e)

    End Sub

    ''' <summary>
    ''' Reference levels have changed! For now just update the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub onRefLevelsChanged()
        Try
            Me.Grid.Update()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub onGridTypeCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbGroup.CheckedChanged, rbFleet.CheckedChanged
        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)
            If rb.Checked Then
                Me.Grid.GridType = DirectCast(rb.Tag, ScientificInterface.gridRiskResults.eGridType)
            End If
        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Stats data has changed. For now just update the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub onRunCompleted()
        Try
            Me.Grid.Update()
        Catch ex As Exception

        End Try
    End Sub

#Region " Core interactions "

    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
        Try
            Me.m_EventSource.HandleCoreMessage(msg)
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class