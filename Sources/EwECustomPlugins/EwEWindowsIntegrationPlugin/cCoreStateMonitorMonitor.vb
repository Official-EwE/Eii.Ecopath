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
Imports ScientificInterfaceShared

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' The best class name ever.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cCoreStateMonitorMonitor

#Region " Privates "

    ''' <summary>The monitored <see cref="cCoreStateMonitor"/></summary>
    Private WithEvents m_sm As cCoreStateMonitor = Nothing
    ''' <summary>Timer to (re)set the main thread state.</summary>
    Private WithEvents m_timer As Timer = Nothing

    ''' <summary>Current state for responding to state changes.</summary>
    Private m_bIsActive As Boolean = False
    ''' <summary>Think positive.</summary>
    Private m_bAbleToShiftActiveHours As Boolean = True

#End Region ' Privates

#Region " Construction "

    Public Sub New(sm As cCoreStateMonitor)
        Me.m_timer = New Timer(10000)
        Me.m_sm = sm
    End Sub

#End Region ' Construction

#Region " Public methods "

    Public Property IsEnabled() As Boolean
        Get
            Return My.Settings.Enabled And Me.m_sm.HasEcopathLoaded
        End Get
        Set(value As Boolean)
            If (value <> My.Settings.Enabled) Then
                My.Settings.Enabled = value
                My.Settings.Save()
                Me.RefreshState()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the keep alive state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub RefreshState()

        SyncLock Me.m_timer

            ' Re-evaluate if keep alive situation needs to change
            Dim bNeedsMonitoring As Boolean = False

            If IsEnabled Then
                Dim bNoSleep As Boolean = My.Settings.KeepOSAwake And Me.m_sm.IsBusy
                Dim bNoRestart As Boolean = My.Settings.NoRestart And (Me.m_sm.IsDatasourceModified Or Me.m_sm.IsBusy)
                bNeedsMonitoring = bNoRestart Or bNoSleep
            End If

            ' No changes? Ok, abort
            If (bNeedsMonitoring = Me.m_bIsActive) Then Return

            If (Not Me.m_bIsActive) Then
                Me.m_timer.Start()
                Me.m_bIsActive = True
            Else
                Me.m_timer.Stop()
                Me.m_bIsActive = False
            End If

            ' Check state change
            Me.ApplyStateChange()

        End SyncLock

    End Sub

#End Region ' Public methods

#Region " Event handlers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the core state monitor has changed its state.
    ''' Handled to re-evaluate if the EwE thread needs to be kept alive.
    ''' </summary>
    ''' <param name="statemonitor"></param>
    ''' -----------------------------------------------------------------------
    Private Sub OnCoreStateEvent(statemonitor As cCoreStateMonitor) Handles m_sm.CoreExecutionStateEvent
        Me.RefreshState()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the on-board timer fired. Handled to update 
    ''' the thread state in the system.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------
    Private Sub OnTimerElapsed(sender As Object, e As ElapsedEventArgs) Handles m_timer.Elapsed
        Me.ApplyStateChange()
    End Sub

#End Region ' Event handlers

#Region " The magic "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether Windows is being kept alert and awake
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function IsActive() As Boolean
        Return Me.m_bIsActive
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether active hours were set correctly. User rights and whatnot
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function IsAbleToShiftActiveHours() As Boolean
        Return Me.m_bAbleToShiftActiveHours
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reaffirm the thread state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ApplyStateChange()

        If Me.IsActive And My.Settings.KeepOSAwake Then
            cNativeMethods.PreventSleep(My.Settings.KeepMonitorOn)
        Else
            cNativeMethods.AllowSleep()
        End If

        If Me.IsActive And My.Settings.NoRestart Then
            Me.m_bAbleToShiftActiveHours = cNativeMethods.ShiftActiveHours()
        End If
    End Sub

#End Region ' The magic

End Class
