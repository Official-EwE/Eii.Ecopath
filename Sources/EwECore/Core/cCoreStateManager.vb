' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwEUtils.Core

' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

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
    Public Function LoadState(ByVal ExecutionState As eCoreExecutionState) As Boolean
        Try
            Dim sm As cCoreStateMonitor = Me.m_core.StateMonitor

            'Try to bring to core up to the requested execution state
            Select Case ExecutionState

                Case eCoreExecutionState.EcopathCompleted
                    If Not sm.HasEcopathLoaded Then Return False
                    If sm.HasEcopathRan Then Return True
                    Return m_core.RunEcoPath()

                Case eCoreExecutionState.EcosimInitialized
                    If Not sm.HasEcosimLoaded Then Return False
                    If sm.HasEcosimInitialized Then Return True
                    If m_core.m_EcoSim.Init(False) Then
                        sm.SetEcoSimInitialized()
                        Return True
                    End If
                    Return False

                Case eCoreExecutionState.EcosimCompleted
                    If Not sm.HasEcosimLoaded Then Return False
                    If sm.HasEcosimRan Then Return True
                    Return m_core.RunEcoSim()

                Case Else
                    ' Not implemented (yet)
            End Select

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadState(" & ExecutionState.ToString & ") Error: " & ex.Message)
        End Try

        Return False

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
