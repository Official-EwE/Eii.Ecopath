
''' <summary>
''' Manager class used to bring the core execution state uptodate
''' </summary>
''' <remarks></remarks>
Public Class cCoreStateManager

    Private m_core As cCore

    Friend Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub


    Public Function LoadState(ByVal ExecutionState As EwEUtils.Core.eCoreExecutionState) As Boolean
        Try

            'Try to bring to core up to the requested execution state
            Select Case ExecutionState

                Case EwEUtils.Core.eCoreExecutionState.EcopathCompleted
                    Return m_core.RunEcoPath()

                    ' Case EwEUtils.Core.eCoreExecutionState.EcoSimInitialized
                    'Return m_core.m_EcoSim.Init()

                Case EwEUtils.Core.eCoreExecutionState.EcosimCompleted
                    Return m_core.RunEcoSim()

            End Select

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadState(" & ExecutionState.ToString & ") Error: " & ex.Message)
            Return False
        End Try

    End Function

End Class
