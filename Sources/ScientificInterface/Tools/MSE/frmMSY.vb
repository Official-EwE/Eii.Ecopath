#Region " Imports "

Option Strict On
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Utilities
Imports System.Text

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form, implements the main MSY search interface.
''' </summary>
''' ===========================================================================
Public Class frmMSY

    Private m_mse As MSE.cMSEManager
    Private m_nFleets As Integer
    Private MSY() As Single

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_mse = Me.UIContext.Core.MSEManager
        Me.rbValue.Checked = True

    End Sub

    Private Sub btRunMSY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btRunMSY.Click

        Try

            ' Hard-wire run state parameters...for now
            If rbValue.Checked Then
                Me.m_mse.ModelParameters.MSYEvaluateValue = True
            Else
                Me.m_mse.ModelParameters.MSYEvaluateValue = False
            End If
            Me.m_mse.ModelParameters.MSYRunSilent = False
            Me.m_mse.ModelParameters.MSYStartTimeIndex = 2

            'get the number of fleets for the progress updates
            m_nFleets = Me.UIContext.Core.nFleets
            ReDim MSY(Me.UIContext.Core.nFleets)
            Me.updateControls(True)

            'connect and disconnect every time we run the MSY
            Me.m_mse.Connect(Nothing, AddressOf Me.onMSYProgress)
            Me.m_mse.RunMSYSearch(True)
            Me.m_mse.Disconnect()

        Catch ex As Exception

        End Try

        Me.updateControls(False)

    End Sub

    Private Shadows Sub updateControls(ByVal bRunning As Boolean)

        If bRunning Then
            Me.btRunMSY.Enabled = False
            Me.btStop.Enabled = True
        Else
            Me.btRunMSY.Enabled = True
            Me.btStop.Enabled = False

            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format(My.Resources.MSE_ITERATION_HEADER, vbTab))
            For i As Integer = 1 To Me.UIContext.Core.nFleets
                sb.AppendLine(String.Format(My.Resources.MSE_ITERATION_LINE, vbTab, i, Me.StyleGuide.FormatNumber(MSY(i))))
            Next
            Me.txtMSYresults.Text = sb.ToString
        End If

    End Sub

    Private Sub onMSYProgress(ByVal MSYProgress As MSE.cMSYProgressArgs)

        Try
            Me.lbFleet.Text = String.Format(SharedResources.GENERIC_VALUE_FLEET_OF_N, MSYProgress.FleetIndex, m_nFleets)
            Me.lbiter.Text = String.Format(SharedResources.GENERIC_VALUE_ITERATION, MSYProgress.Iteration)
            Me.lbEffort.Text = String.Format(My.Resources.MSE_EFFORT_VALUE, Me.StyleGuide.FormatNumber(MSYProgress.CurrentEffort))
            If MSYProgress.CurrentEffort > 0 Then MSY(MSYProgress.FleetIndex) = MSYProgress.CurrentEffort

            'the DoEvents can be removed once the MSY is running on a thread 
            Application.DoEvents()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btStop.Click
        Me.m_mse.ModelParameters.StopRun = True
    End Sub

    Private Sub btFleetTradeoffs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btFleetTradeoffs.Click

        Try
            'get the number of fleets for the progress updates
            m_nFleets = Me.UIContext.Core.nFleets

            'connect and disconnect every time we run the MSY
            Me.m_mse.Connect(Nothing, AddressOf Me.onMSYProgress)
            Me.m_mse.FleetTradeoffs()
            Me.m_mse.Disconnect()

            MsgBox(SharedResources.GENERIC_LABEL_FINISHED)

        Catch ex As Exception

        End Try

    End Sub

End Class