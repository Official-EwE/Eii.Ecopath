Public Class cApplicationStatusNotifier
    Implements IApplicationStatusDispatcher

    Private Shared __inst__ As cApplicationStatusNotifier

    Private m_dispatcher As IApplicationStatusDispatcher = Nothing

    Public Sub New(ByVal dispatcher As IApplicationStatusDispatcher)
        Debug.Assert(__inst__ Is Nothing)
        Debug.Assert(dispatcher IsNot Nothing)
        __inst__ = Me
        Me.m_dispatcher = dispatcher
    End Sub

    Public Shared Function GetInstance() As cApplicationStatusNotifier
        Return cApplicationStatusNotifier.__inst__
    End Function

    Public Sub SetStatusText(Optional ByVal strText As String = "", _
                             Optional ByVal tsUseWaitCursor As Microsoft.VisualBasic.TriState = Microsoft.VisualBasic.TriState.UseDefault, _
                             Optional ByVal sProgress As Single = 0.0) _
        Implements IApplicationStatusDispatcher.SetStatusText

        Me.m_dispatcher.SetStatusText(strText, tsUseWaitCursor, sProgress)
    End Sub

End Class

Public Interface IApplicationStatusDispatcher

    Sub SetStatusText(Optional ByVal strText As String = "", _
        Optional ByVal tsUseWaitCursor As TriState = TriState.UseDefault, _
        Optional ByVal sProgress As Single = 0.0)

End Interface
