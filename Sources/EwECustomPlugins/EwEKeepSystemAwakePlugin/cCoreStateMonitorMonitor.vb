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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Timers
Imports EwECore

#End Region ' Imports

''' <summary>
''' The best class name ever.
''' </summary>
Public Class cCoreStateMonitorMonitor

    Private WithEvents m_sm As cCoreStateMonitor = Nothing
    Private WithEvents m_timer As Timer = Nothing

    Private m_bKeepingAlive As Boolean = False

    Public Sub New(sm As cCoreStateMonitor)
        Me.m_timer = New Timer(10000)
        Me.m_sm = sm
    End Sub

    Public Sub Prod()

        SyncLock Me.m_timer

            ' Re-evaluate if keep alive situation needs to change
            Dim bIsRunning As Boolean = Me.m_sm.IsBusy And My.Settings.KeepMonitorOn

            ' No changes? Ok, abort
            If (bIsRunning = Me.m_bKeepingAlive) Then Return

            If (Not Me.m_bKeepingAlive) Then
                Me.m_timer.Start()
                Me.m_bKeepingAlive = True
            Else
                Me.m_timer.Stop()
                Me.m_bKeepingAlive = False
            End If

            ' As a precaution
            Me.UpdateThreadState()

        End SyncLock

    End Sub

#Region " Event handlers "

    Private Sub OnCoreStateEvent(statemonitor As cCoreStateMonitor) Handles m_sm.CoreExecutionStateEvent
        Me.Prod()
    End Sub

    Private Sub OnTimerElapsed(sender As Object, e As ElapsedEventArgs) Handles m_timer.Elapsed
        Me.UpdateThreadState()
        Me.Prod()
    End Sub

#End Region ' Event handlers

#Region " The magic "

    Private Sub UpdateThreadState()

        If Me.m_bKeepingAlive Then
            cNativeMethods.PreventSleep(My.Settings.KeepMonitorOn)
        Else
            cNativeMethods.AllowSleep()
        End If
    End Sub

#End Region ' The magic

End Class
