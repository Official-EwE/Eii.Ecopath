Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class frmFleetTradeoffs

    Public Sub New(uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.Text = My.Resources.PLUGIN_TITLE
        Me.m_progress.Visible = False

    End Sub

    Private Property UIContext As cUIContext

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.CenterToParent()
    End Sub

    Private Sub OnRun(sender As Object, e As EventArgs) Handles m_btnRun.Click

        Dim core As cCore = Me.UIContext.Core
        Dim manager As cMSEManager = core.MSEManager

        If manager.IsRunning Then Return
        manager.Connect(Nothing, AddressOf OnDetailedProgress)

        Me.m_progress.Visible = True
        Try
            manager.FleetTradeoffs()
        Catch ex As Exception

        End Try
        Me.m_progress.Visible = False

        manager.Disconnect()
        Me.Close()

    End Sub

    Private Sub OnDetailedProgress(MSYProgress As cMSYProgressArgs)
        Try
            Me.m_progress.Value = 100 * (MSYProgress.FleetIndex / Math.Max(MSYProgress.Iteration, 1))
            Me.m_progress.Refresh()
        Catch ex As Exception

        End Try
    End Sub

End Class