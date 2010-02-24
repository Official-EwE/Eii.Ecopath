Public Class frmCEFASSample

    Private m_plugin As cCEFASPluginPoint


    Public Sub New(ByVal Plugin As cCEFASPluginPoint)

        Me.InitializeComponent()

        Me.m_plugin = Plugin

    End Sub

    Private Sub frmEcoSimSample_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        UpdateInterface()
    End Sub

    Private Sub ckMultiThread_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckMultiThread.CheckedChanged
        Me.m_plugin.EcosimDS.bMultiThreaded = Me.ckMultiThread.Checked
    End Sub

    Private Sub btRunEcosim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btRunEcosim.Click

        Me.lstTimesteps.Items.Clear()

        Me.m_plugin.Core.RunEcoSim()

    End Sub

    Private Sub UpdateInterface()

        'update the check box whenever the interface is activated
        If Me.ckMultiThread.Checked <> Me.m_plugin.EcosimDS.bMultiThreaded Then
            Me.ckMultiThread.Checked = Me.m_plugin.EcosimDS.bMultiThreaded
        End If

        Me.txStepsPerMonth.Text = Me.m_plugin.EcosimDS.StepsPerMonth.ToString

    End Sub

    Private Sub txStepsPerMonth_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txStepsPerMonth.TextChanged
        Me.m_plugin.EcosimDS.StepsPerMonth = Integer.Parse(Me.txStepsPerMonth.Text)
    End Sub


    Friend Sub onEcosimMonthlyTimeStep(ByVal TimeStep As Integer)
        Me.lstTimesteps.Items.Add("Month = " & TimeStep.ToString)
        Me.lstTimesteps.Refresh()
    End Sub


    Friend Sub onEcosimSubTimestep(ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object)
        Me.lstTimesteps.Items.Add("Sub timestep in years= " & TimeInYears.ToString)
    End Sub

    Friend Sub onEcosimRunCompleted()
        Me.UpdateInterface()
    End Sub

End Class