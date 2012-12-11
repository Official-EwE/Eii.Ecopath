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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base Class to provide thread blocking for the Threaded Manager Classes
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cThreadWaitBase
    Implements IThreadedProcess

#Region " Private vars "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Signal state flag, used by an calling routine to block its thread until the 
    ''' model has completed. Invoked by <see cref="SetWait"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Private m_SignalState As System.Threading.ManualResetEvent

    Private m_bIsRunning As Boolean

#End Region ' Private vars

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Sub New()
        Me.m_SignalState = New System.Threading.ManualResetEvent(True)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <inheritdocs cref="IThreadedProcess.StopRun"/>
    ''' ---------------------------------------------------------------------------
    Public MustOverride Function StopRun(Optional ByVal WaitTimeInMillSec As Integer = -1) As Boolean _
        Implements IThreadedProcess.StopRun

    ''' ---------------------------------------------------------------------------
    ''' <inheritdocs cref="IThreadedProcess.Wait"/>
    ''' ---------------------------------------------------------------------------
    Public Overridable Function Wait(Optional ByVal WaitTimeInMillSec As Integer = -1) As Boolean _
        Implements IThreadedProcess.Wait

        Dim result As Boolean
        Dim waitTime As Integer = WaitTimeInMillSec
        Dim totTime As Integer
        Dim processing As Boolean = True
        Dim n As Integer

        'System.Console.WriteLine("Starting Waiting.")

        'if WaitTimeInMillSec  = -1 wait until completed(WaitOne returns True) no matter how long
        'if WaitTimeInMillSec = 0 then wait for zero time even if WaitOne returns False, process has not completed
        'WaitTimeInMillSec > 0 (any positive integer) then wait for WaitTimeInMillSec or until WaitOne returns True
        If waitTime > 0 Then waitTime = 100

        'Wait is in a loop because
        'm_SignalState is signaled when a thread is running
        'm_SignalState.WaitOne will block the calling thread (the interface) while the signal is set
        'this allows the running thread to keep going.
        'If the running thread calls out to the interface there will be a deadlock, it is block by WaitOne.
        'The loop allows the interface to unblock and process any calls from the thread 
        'then reblock and finish any processing on the thread
        Do
            n += 1

            'remove for Mono compatibilty
            Windows.Forms.Application.DoEvents()
            

            'WaitOne() will return False if it timed out, the process has not completed
            'True if the wait was completed or there was no wait 
            result = Me.m_SignalState.WaitOne(waitTime)
            totTime += waitTime

            If result = True Then processing = False
            If totTime >= WaitTimeInMillSec Then processing = False

        Loop While processing

        'System.Console.WriteLine("Finished waiting " & totTime.ToString & " milliseconds, " & n.ToString & " iterations")
        Return result

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <inheritdocs cref="IThreadedProcess.SetWait"/>
    ''' ---------------------------------------------------------------------------
    Protected Overridable Sub SetWait() _
        Implements IThreadedProcess.SetWait

        'set the isRunning flag
        m_bIsRunning = True
        'puts the ManualResetEvent into a non-signaled state
        'threads calling Wait() will block until ReleaseWait() is called
        Me.m_SignalState.Reset()

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <inheritdocs cref="IThreadedProcess.ReleaseWait"/>
    ''' ---------------------------------------------------------------------------
    Protected Overridable Sub ReleaseWait() _
        Implements IThreadedProcess.ReleaseWait

        m_bIsRunning = False
        'puts the ManualResetEvent into a signaled state
        'Threads that called Wait() will be signaled to proceed
        Me.m_SignalState.Set()

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <inheritdocs cref="IThreadedProcess.IsRunning"/>
    ''' ---------------------------------------------------------------------------
    Public Overridable ReadOnly Property IsRunning() As Boolean _
        Implements IThreadedProcess.IsRunning
        Get
            Return m_bIsRunning
        End Get
    End Property

End Class