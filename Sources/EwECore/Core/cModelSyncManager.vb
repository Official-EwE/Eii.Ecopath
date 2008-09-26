
''' <summary>
''' Synchronize events in the core that effect more than one model or data structure
''' </summary>
''' <remarks></remarks>
Public Class cModelSyncManager

    Private m_core As cCore

    Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub

    ''' <summary>
    ''' Copy the Ecopath dietcomp matrix (DC(ngroup,ngroups)) into the Ecosim dietcomp matrix (SimDC(ngroups,ngroups))
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DietComp() As Boolean

        Try

            'this is kind of hack 
            'if Ecosim has not been initialized then don't do the data copy....
            If m_core.m_EcoSimData.SimDC Is Nothing Then
                Return False
            End If

            'this will copy diet comp into Ecosim
            m_core.m_EcoSim.RemoveImportFromEcosim()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".DietComp() Failed to copy DietComp into Ecosim." & ex.Message)
            Return False
        End Try

        Return True

    End Function


End Class
