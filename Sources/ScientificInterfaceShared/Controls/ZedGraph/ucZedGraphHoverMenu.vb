Public Class ucZedGraphHoverMenu

    Public Enum eCommandTypes As Integer
        ZoomIn
        ZoomOut
    End Enum

    Public Delegate Sub OnCommandDelegate(ByVal cmd As eCommandTypes)

    Private m_callback As OnCommandDelegate = Nothing

    Public Sub New(ByVal callback As OnCommandDelegate)
        Me.InitializeComponent()
        Me.m_callback = callback
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.Size = Me.ClientSize
    End Sub

    Private Sub m_btnIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnIn.Click
        Me.InvokeCallback(eCommandTypes.ZoomIn)
    End Sub

    Private Sub m_btnOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOut.Click
        Me.InvokeCallback(eCommandTypes.ZoomOut)
    End Sub

    Private Sub InvokeCallback(ByVal cmd As eCommandTypes)
        If (Me.m_callback Is Nothing) Then Return
        Me.m_callback.Invoke(cmd)
    End Sub

End Class
