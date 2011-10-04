Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms


Public Class frmMSERunBatch

    Private m_BatchManager As EwECore.MSEBatchManager.cMSEBatchManager

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        m_BatchManager = Me.UIContext.Core.MSEBatchManager

        Me.m_BatchManager.onMessageDelegate = AddressOf Me.onMSEBatchMessage

    End Sub


    Private Sub btRunBatch_Click(sender As Object, e As System.EventArgs) Handles btRunBatch.Click

        Me.lstMsgs.Items.Clear()

        ' Me.m_BatchManager.Parameters.nTFMIteration = Integer.Parse(Me.txNTFMIters.Text)
        Me.m_BatchManager.setDefaults()
        Me.m_BatchManager.Run()

    End Sub

    Private Sub onMSEBatchMessage(msg As String)
        Me.lstMsgs.Items.Add(msg)
    End Sub

End Class