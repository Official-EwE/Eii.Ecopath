Public Class frmMSEBatchTFM

    Private m_BatchManager As EwECore.MSEBatchManager.cMSEBatchManager

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.Grid.UIContext = Me.UIContext

        m_BatchManager = Me.UIContext.Core.MSEBatchManager

        Me.txNTFM.Text = Me.m_BatchManager.Parameters.nTFMIteration.ToString


    End Sub


    Private Sub txNTFM_TextChanged(sender As System.Object, e As System.EventArgs) Handles txNTFM.TextChanged

        Dim newValue As Integer = Integer.Parse(Me.txNTFM.Text)
        If newValue > 0 And newValue <> Me.m_BatchManager.Parameters.nTFMIteration Then
            Me.m_BatchManager.Parameters.nTFMIteration = newValue
        End If

    End Sub



End Class