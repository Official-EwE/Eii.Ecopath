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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
Option Strict On

Imports EwEUtils.Core

Public Class cMSEStateMonitor

    Private m_plugin As cMSE = Nothing
    Private m_StateCache([Enum].GetValues(GetType(eState)).Length) As TriState

    Public Sub New(plugin As cMSE)
        Me.m_plugin = plugin
        Me.Invalidate()
    End Sub

    Public Enum eState As Byte
        Idle = 0
        HasParams
        HasModels
        HasResults
    End Enum

    Public Function IsStateAvailable(state As eState) As Boolean

        Dim bHasState As Boolean = True

        If Me.m_StateCache(state) <> TriState.UseDefault Then
            Return Me.m_StateCache(state) = TriState.True
        End If

        Select Case state

            Case eState.Idle
                bHasState = True

            Case eState.HasParams
                bHasState = Me.IsStateAvailable(eState.Idle) And _
                    Me.m_plugin.IsInputStructureAvailable(False) And _
                    Me.m_plugin.IsInputDataCompatible()

            Case eState.HasModels
                bHasState = Me.IsStateAvailable(eState.HasParams) And _
                    (Me.m_plugin.NumModelsAvailable > 0)

            Case eState.HasResults
                bHasState = Me.IsStateAvailable(eState.HasModels) And _
                    True ' ToDo_JS: determine this properly
        End Select

        If bHasState Then
            Me.m_StateCache(state) = TriState.True
        Else
            Me.m_StateCache(state) = TriState.False
        End If

        Return bHasState

    End Function

    Public Sub Invalidate()
        For i As Integer = 0 To Me.m_StateCache.Count - 1
            Me.m_StateCache(i) = TriState.UseDefault
        Next
    End Sub

End Class
