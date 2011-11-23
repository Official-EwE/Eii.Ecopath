Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Imports EwECore.MSE
Imports ZedGraph


Public Class frmMSERunBatch

    Private m_BatchManager As EwECore.MSEBatchManager.cMSEBatchManager
    Private m_MSE As EwECore.MSE.cMSEManager
    Private m_zgh As cZedGraphHelper = New cZedGraphHelper()

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        m_BatchManager = Me.UIContext.Core.MSEBatchManager
        m_MSE = Me.UIContext.Core.MSEManager

        m_zgh.Attach(Me.UIContext, Me.m_ZedGraph)

        Me.m_BatchManager.onMessageDelegate = AddressOf Me.onMSEBatchMessage

    End Sub


    Private Sub btRunBatch_Click(sender As Object, e As System.EventArgs) Handles btRunBatch.Click

        Me.lstMsgs.Items.Clear()

        Me.m_zgh.GetPane(1).CurveList.Clear()

        Me.m_BatchManager.setDefaults()
        Me.m_BatchManager.Connect(AddressOf Me.onProgress)
        Me.m_BatchManager.Run()
        Me.m_BatchManager.DisConnect()

    End Sub

    Private Sub onProgress()
        'For it As Integer = 1 To Me.m_MSE.NumGroups
        Me.plotMean(Me.m_MSE.BiomassStats(4), 1)
        ' Next

    End Sub

    Private Sub plotMean(ByVal StatsData As cMSEStats, ByVal ipane As Integer)
        Dim x As Double, dx As Double
        Dim ppl As PointPairList = Nothing
        Dim li As LineItem = Nothing

        'time varing mean
        ppl = New PointPairList()
        x = Me.UIContext.Core.EcosimFirstYear
        dx = 1 / StatsData.nStepsPerYear
        For iTime As Integer = 1 To StatsData.nTimeSteps
            ppl.Add(x, StatsData.Mean(iTime))
            x += dx
        Next
        li = Me.m_zgh.CreateLineItem("", eLineType.NotSet, Color.Blue, ppl)
        li.Line.Width = 1

        Me.m_zgh.GetPane(ipane).CurveList.Add(li)
        Me.m_zgh.RescaleAndRedraw()

    End Sub

    Private Sub onMSEBatchMessage(msg As String)
        Me.lstMsgs.Items.Add(msg)
    End Sub

End Class