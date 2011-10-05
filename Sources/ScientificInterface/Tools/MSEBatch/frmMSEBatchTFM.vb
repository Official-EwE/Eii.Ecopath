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



End Class