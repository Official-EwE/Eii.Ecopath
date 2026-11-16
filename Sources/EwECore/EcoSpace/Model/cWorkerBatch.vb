' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

Imports System.Threading
Imports Microsoft.Extensions.Logging

Partial Public Class cEcoSpace

    ''' <summary>
    ''' Owns completion, worker failures, and fleet allocation for one Ecospace worker batch.
    ''' </summary>
    ''' <remarks>
    ''' The calling thread queues workers, waits, and disposes the batch. Workers may concurrently claim fleets.
    ''' Each invocation needs its own batch; independent cores must never share completion or fleet-allocation state.
    ''' </remarks>
    Private Class cWorkerBatch
        Implements IDisposable

        'The initial count keeps the batch open until all workers have been queued.
        Private ReadOnly m_completion As New CountdownEvent(1)
        Private ReadOnly m_errors As New System.Collections.Concurrent.ConcurrentQueue(Of Exception)
        Private ReadOnly m_owner As cEcoSpace
        Private m_closed As Boolean
        Private m_nextFleet As Integer

        Public Sub New(owner As cEcoSpace)
            m_owner = owner
        End Sub

        ''' <summary>
        ''' Schedules work whose completion is always counted and whose failures are collected for the caller.
        ''' </summary>
        ''' <remarks>Queue all workers before calling WaitForCompletion or Dispose.</remarks>
        Public Sub Queue(callback As WaitCallback, arguments As Object, Optional dedicatedThread As Boolean = False)
            m_completion.AddCount()
            Dim work As WaitCallback =
                Sub(state As Object)
                    Try
                        callback(state)
                    Catch ex As Exception
                        m_errors.Enqueue(ex)
                    Finally
                        m_completion.Signal()
                    End Try
                End Sub

            Try
                If dedicatedThread Then
                    Dim worker As New Thread(Sub() work(arguments))
                    worker.Start()
                ElseIf Not ThreadPool.QueueUserWorkItem(work, arguments) Then
                    Throw New InvalidOperationException("Unable to queue Ecospace worker.")
                End If
            Catch
                'Scheduling failed, so no worker will release the count reserved above.
                m_completion.Signal()
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Atomically claims the next one-based fleet index within this batch, returning False when none remain.
        ''' </summary>
        Public Function GetNextFleet(fleetCount As Integer, ByRef fleetIndex As Integer) As Boolean
            fleetIndex = Interlocked.Increment(m_nextFleet)
            Return fleetIndex <= fleetCount
        End Function

        ''' <summary>
        ''' Closes scheduling and waits for every worker before reporting a timeout or collected failures.
        ''' </summary>
        ''' <param name="timeout">Diagnostic threshold in milliseconds, or Timeout.Infinite for an unbounded wait.</param>
        ''' <param name="operation">Operation name included in logs and exceptions.</param>
        ''' <remarks>
        ''' A timeout does not cancel workers or bound this method's duration. A permanently stuck worker keeps the caller blocked.
        ''' </remarks>
        Public Sub WaitForCompletion(timeout As Integer, operation As String)
            Me.CloseBatch()
            Dim timedOut As Boolean = Not m_completion.Wait(timeout)
            If timedOut Then
                m_owner.m_logger.LogError(operation & " timed out. Waiting for workers to finish before accessing model data.")
                'Workers still own model arrays and the completion signal until they finish.
                m_completion.Wait()
            End If

            Dim failure As AggregateException = Nothing
            If Not m_errors.IsEmpty Then
                failure = New AggregateException(operation & " failed.", m_errors.ToArray())
                m_owner.m_logger.LogError(failure, operation & " failed.")
            End If
            If timedOut Then Throw New TimeoutException(operation & " timed out.", failure)
            If failure IsNot Nothing Then Throw failure
        End Sub

        Private Sub CloseBatch()
            If Not m_closed Then
                m_closed = True
                'Release the scheduling count once; remaining counts belong to workers.
                m_completion.Signal()
            End If
        End Sub

        ''' <summary>
        ''' Drains outstanding workers before releasing synchronization resources, including during caller exception unwinding.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.CloseBatch()
            'Also drain workers if scheduling or the calling thread fails before WaitForCompletion.
            m_completion.Wait()
            m_completion.Dispose()
        End Sub
    End Class

End Class
