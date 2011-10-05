Public Class frmMSEBatchTFM

    Private m_BatchManager As EwECore.MSEBatchManager.cMSEBatchManager

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.m_grid.UIContext = Me.UIContext
        End Set
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        m_BatchManager = Me.UIContext.Core.MSEBatchManager

        Me.txNTFM.Text = Me.m_BatchManager.Parameters.nTFMIteration.ToString

    End Sub


    Private Sub txNTFM_TextChanged(sender As System.Object, e As System.EventArgs) Handles txNTFM.TextChanged

        Dim newValue As Integer = Integer.Parse(Me.txNTFM.Text)
        If newValue > 0 And newValue <> Me.m_BatchManager.Parameters.nTFMIteration Then
            Me.m_BatchManager.Parameters.nTFMIteration = newValue
        End If

    End Sub



    Private Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click

        Me.m_BatchManager.setDefaults()

    End Sub

    Private Sub UpDwnIter_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles UpDwnIter.Validating

    End Sub

  
    Private Sub UpDwnIter_ValueChanged(sender As System.Object, e As System.EventArgs) Handles UpDwnIter.ValueChanged
        Dim iter As Integer = CInt(Me.UpDwnIter.Value)
        If Me.m_BatchManager Is Nothing Then Exit Sub
        If iter <= Me.m_BatchManager.Parameters.nTFMIteration Then
            Me.m_grid.iCurIter = iter
        End If
    End Sub
End Class