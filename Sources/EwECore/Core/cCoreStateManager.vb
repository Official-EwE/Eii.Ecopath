
''' <summary>
''' Manager class used to bring the core execution state uptodate
''' </summary>
''' <remarks></remarks>
Public Class cCoreStateManager

#Region "Private data"

    Private m_core As cCore

#End Region

#Region "Construction"

    Friend Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub

#End Region

#Region "Public methods"

    ''' <summary>
    ''' Bring the core state up to the requested execution state
    ''' </summary>
    ''' <param name="ExecutionState">State to bring the core up to</param>
    ''' <returns>True if successful. False otherwise.</returns>
    ''' <remarks></remarks>
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

#End Region

#Region "Friend methods: used by the core"


    ''' <summary>
    ''' Copy the Ecopath dietcomp matrix (DC(ngroup,ngroups)) into the Ecosim dietcomp matrix (SimDC(ngroups,ngroups))
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This is not really an Execution State thing... but hey it has to go somewhere...</remarks>
    Friend Function updateDietComp() As Boolean

        Try

            'Only load the dietcomp into ecosim if it is loaded
            If Not m_core.StateMonitor.HasEcosimLoaded Then
                Return False
            End If

            'this will copy diet comp into Ecosim SimDC()
            m_core.m_EcoSim.RemoveImportFromEcosim()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".DietComp() Failed to copy DietComp into Ecosim." & ex.Message)
            Return False
        End Try

        Return True

    End Function

#End Region

End Class
