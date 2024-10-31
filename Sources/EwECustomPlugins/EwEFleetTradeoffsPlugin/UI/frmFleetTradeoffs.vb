Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class frmFleetTradeoffs

    Private m_fpFrom As cEwEFormatProvider = Nothing
    Private m_fpTo As cEwEFormatProvider = Nothing
    Private m_fpStepsize As cEwEFormatProvider = Nothing

    Public Sub New(uic As cUIContext)

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.Text = My.Resources.PLUGIN_TITLE
        Me.m_progress.Visible = False

    End Sub

    Private Property UIContext As cUIContext

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim md As New cVariableMetaData(0, 10, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        Me.m_fpFrom = New cEwEFormatProvider(Me.UIContext, Me.m_tbxFrom, GetType(Single), md)
        Me.m_fpTo = New cEwEFormatProvider(Me.UIContext, Me.m_tbxTo, GetType(Single), md)
        Me.m_fpStepsize = New cEwEFormatProvider(Me.UIContext, Me.m_tbxStep, GetType(Single), md)

        Me.m_fpFrom.Value = 0.9
        Me.m_fpTo.Value = 0.9
        Me.m_fpStepsize.Value = 0.1

        Me.CenterToParent()
    End Sub

    Private Sub OnRun(sender As Object, e As EventArgs) Handles m_btnRun.Click

        Dim core As cCore = Me.UIContext.Core
        Dim manager As cMSEManager = core.MSEManager

        If manager.IsRunning Then Return
        manager.Connect(Nothing, AddressOf OnDetailedProgress)

        Me.m_progress.Visible = True
        Try
            manager.FleetTradeoffs(CSng(Me.m_fpFrom.Value), CSng(Me.m_fpTo.Value), CSng(Me.m_fpStepsize.Value))
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