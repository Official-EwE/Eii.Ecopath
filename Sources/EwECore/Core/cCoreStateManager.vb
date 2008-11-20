
''' <summary>
''' Manager class used to bring the core execution state uptodate
''' </summary>
''' <remarks></remarks>
Public Class cCoreStateManager

    Private m_core As cCore

    Friend Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub


    Public Sub LoadState(ByVal ExecutionState As EwEUtils.Core.eCoreExecutionState)
        Try

        Catch ex As Exception

        End Try
    End Sub

End Class
