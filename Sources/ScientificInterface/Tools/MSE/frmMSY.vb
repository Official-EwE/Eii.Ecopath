
Imports EwECore

Public Class frmMSY

    Private m_mse As MSE.cMSEManager
    Private m_core As cCore
    Private m_nFleets As Integer
    Private MSY() As Single

    Private Sub frmMSY_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.m_core = cCore.GetInstance
        m_mse = Me.m_core.MSEManager
    End Sub


    Private Sub btRunMSY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btRunMSY.Click

        Try
            'get the number of fleets for the progress updates
            m_nFleets = Me.m_core.nFleets
            ReDim MSY(m_core.nFleets)
            Me.updateControls(True)

            'connect and disconnect every time we run the MSY
            Me.m_mse.Connect(Nothing, AddressOf Me.onMSYProgress)
            Me.m_mse.RunMSYSearch()
            Me.m_mse.Disconnect()

        Catch ex As Exception

        End Try

        Me.updateControls(False)

    End Sub

    Private Sub updateControls(ByVal running As Boolean)

        If running Then
            Me.btRunMSY.Enabled = False
            Me.btStop.Enabled = True
        Else
            Me.btRunMSY.Enabled = True
            Me.btStop.Enabled = False
            Dim iStr As String = "Fleet" & vbTab & "MSY Effort" & vbCrLf

            For i As Integer = 1 To Me.m_core.nFleets
                iStr = iStr & i.ToString & vbTab & MSY(i).ToString & vbCrLf

            Next
            Me.txtMSYresults.Text = iStr
        End If
    End Sub


    Private Sub onMSYProgress(ByVal MSYProgress As MSE.cMSYProgressArgs)
        ' Dim MSY(m_core.nFleets) As Single


        Try
            Me.lbFleet.Text = "Fleet " & MSYProgress.FleetIndex.ToString & " of " & m_nFleets.ToString
            Me.lbiter.Text = "Iteration " & MSYProgress.Iteration.ToString
            Me.lbEffort.Text = "Effort " & MSYProgress.CurrentEffort.ToString
            If MSYProgress.CurrentEffort > 0 Then MSY(MSYProgress.FleetIndex) = MSYProgress.CurrentEffort

            'the DoEvents can be removed once the MSY is running on a thread 
            Application.DoEvents()
        Catch ex As Exception

        End Try

    End Sub


    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btStop.Click
        Me.m_mse.ModelParameters.StopRun = True
    End Sub

    Private Sub btFleetTradeoffs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btFleetTradeoffs.Click

        Try
            'get the number of fleets for the progress updates
            m_nFleets = Me.m_core.nFleets
            'ReDim MSY(m_core.nFleets)
            'Me.updateControls(True)

            'connect and disconnect every time we run the MSY
            Me.m_mse.Connect(Nothing, AddressOf Me.onMSYProgress)
            Me.m_mse.FleetTradeoffs()
            Me.m_mse.Disconnect()

            MsgBox("Done")

        Catch ex As Exception

        End Try

        'Me.updateControls(False)



    End Sub
End Class