
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core

#End Region

Public Class frmMSEResults

    Private m_EventSource As cMSEEventSource

    Private Sub onGridTypeCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbGroup.CheckedChanged, rbFleet.CheckedChanged
        Try
            Dim rb As RadioButton = DirectCast(sender, RadioButton)

            If rb.Checked Then
                Me.Grid.GridType = DirectCast(rb.Tag, ScientificInterface.gridRiskResults.eGridType)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub frmMSEResults_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        RemoveHandler Me.m_EventSource.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        RemoveHandler Me.m_EventSource.onRunCompleted, AddressOf Me.onRunCompleted

    End Sub

    Private Sub frmMSEResults_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.rbFleet.Tag = ScientificInterface.gridRiskResults.eGridType.Fleet
        Me.rbGroup.Tag = ScientificInterface.gridRiskResults.eGridType.Group

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        AddHandler Me.m_EventSource.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        AddHandler Me.m_EventSource.onRunCompleted, AddressOf Me.onRunCompleted

    End Sub


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        m_EventSource = New cMSEEventSource

        ' Add any initialization after the InitializeComponent() call.

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

#Region "Core interactions"

    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
        Try
            Me.m_EventSource.HandleCoreMessage(msg)
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class