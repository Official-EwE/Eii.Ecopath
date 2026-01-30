' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Interface IThreadedProcess

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Terminate a threaded run.
    ''' </summary>
    ''' <param name="WaitTimeInMillSec"></param>
    ''' <returns></returns>
    ''' ---------------------------------------------------------------------------
    Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Block the calling thread until the model has finished running
    ''' </summary>
    ''' <param name="WaitTimeinMillSec">
    ''' Length of time in milliseconds to wait for the process to complete,
    '''  -1 to wait indefinitely,
    '''  0 return immediately with True if the process has completed False if it is still running,
    '''  > 0 (any positive integer) then wait for WaitTimeInMillSec return True if process completed False otherwise.
    ''' </param>
    ''' <returns>True if the process was stop within the wait time, False if it timed out.</returns>
    ''' <remarks>This can be used by an interface to call the model then wait for results before continuing processing.</remarks>
    ''' ---------------------------------------------------------------------------
    Function Wait(Optional WaitTimeInMillSec As Integer = -1) As Boolean

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Set the signaled state to non-signaled. Any thread that calls this method 
    ''' will be blocked until <see cref="ReleaseWait"/> is called.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Sub SetWait()

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Resume all the threats <see cref="SetWait">waiting for a release</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Sub ReleaseWait()

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Get whether the process is running.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    ReadOnly Property IsRunning() As Boolean


    WriteOnly Property MessagePump As cCore.MessagePumpDelegate


End Interface