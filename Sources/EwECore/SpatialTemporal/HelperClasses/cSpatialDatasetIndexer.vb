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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Helper class that performs the task of indexing one spatial dataset at the time.
    ''' The indexer has a queue of datasets to index.
    ''' </summary>
    Friend Class cSpatialDatasetIndexer

#Region " Private vars "

        ''' <summary>Synclock</summary>
        Private m_sync As New Object()
        ''' <summary>The core to operate on.</summary>
        Private m_core As cCore = Nothing

        ''' <summary>One-item wait queue.</summary>
        Private m_dsNext As ISpatialDataSet = Nothing
        ''' <summary>Currently indexed dataset.</summary>
        Private m_dsCurrent As ISpatialDataSet = Nothing
        ''' <summary>THe worker thread to perform the indexing.</summary>
        Private m_threadIndex As Threading.Thread = Nothing

#End Region ' Private vars

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="core">The core to index against.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a dataset for indexing.
        ''' </summary>
        ''' <param name="ds">The <see cref="ISpatialDataSet"/> to index, or
        ''' nothing to stop indexing.</param>
        ''' -------------------------------------------------------------------
        Public Sub Add(ds As ISpatialDataSet)

            ' Check if there is work to do
            If (ds IsNot Nothing) Then
                ' Check if we really need to do this
                Dim comp As New cDatasetCompatilibity(Me.m_core, ds)
                ' Is set full indexed?
                If (comp.NumIndexed = comp.NumOverlappingTimeSteps) Then
                    ' #Yes: nothing to index for the current scenario
                    ' JS: This could also stop the indexer. Not sure what is the best approach
                    Return
                End If
            End If

            ' Critical section bit
            SyncLock Me.m_sync

                ' Line dataset up as the next one to process
                Me.m_dsNext = ds

                ' Is indexing?
                If (Me.m_dsCurrent IsNot Nothing) Then
                    ' #Yes: tell current dataset to stop indexing graciously
                    Me.m_dsCurrent.StopIndexing()
                Else
                    ' #No: ah, ready for a new dataset to index
                    ' Get the dataset that is lined up next
                    Me.m_dsCurrent = Me.m_dsNext
                    ' Is there more to do?
                    If (Me.m_dsCurrent IsNot Nothing) Then
                        ' #Yes: start thread. Note that the dying thread will move the indexing queue forward
                        Me.m_threadIndex = New Threading.Thread(AddressOf IndexDatasetThread)
                        Me.m_threadIndex.IsBackground = True
                        Me.m_threadIndex.Start()
                    End If
                End If

            End SyncLock

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stop indexing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub [Stop]()
            ' JS140125: let's consider applying an abort timer here
            Me.Add(Nothing)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="ISpatialDataSet"/> currently being indexed.
        ''' </summary>
        ''' <returns>The <see cref="ISpatialDataSet"/> currently being indexed.</returns>
        ''' -------------------------------------------------------------------
        Public Function Current() As ISpatialDataSet
            Return Me.m_dsCurrent
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a dataset is being indexed.
        ''' </summary>
        ''' <param name="ds">The <see cref="ISpatialDataSet"/> to check, or 
        ''' nothing to check if any dataset is being indexed.</param>
        ''' <returns>True if a dataset is being indexed.</returns>
        ''' -------------------------------------------------------------------
        Public Function IsIndexing(ds As ISpatialDataSet) As Boolean
            ' These are atomic thread-safe checks; no need for critical sections
            If (ds Is Nothing) Then Return (Me.m_dsCurrent IsNot Nothing)
            Return Object.ReferenceEquals(Me.m_dsCurrent, ds)
        End Function

#End Region ' Public bits

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Private indexing thread.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub IndexDatasetThread()

            Dim ds As ISpatialDataSet = Me.m_dsCurrent
            Debug.Assert(ds IsNot Nothing)

            Try
                ' Start building index for the Ecospace run time
                Me.m_dsCurrent.BuildIndex(Me.m_core.EcospaceTimestepToAbsoluteTime(1), _
                                          Me.m_core.EcospaceTimestepToAbsoluteTime(Me.m_core.nEcospaceTimeSteps + 1), _
                                          New ISpatialDataSet.BuildIndexUpdateDelegate(AddressOf OnSpatialIndexUpdated))

                Me.m_dsCurrent = Nothing
                Me.m_threadIndex = Nothing
                'Console.WriteLine("Done indexing " & ds.DisplayName)

                ' Done (send just in case)
                Me.OnSpatialIndexUpdated(ds, 1.0!)

            Catch ex As Exception
                cLog.Write(ex, "cSpatialDatasetIndexer::IndexDatasetThread(" & ds.DisplayName & ")")
                'Console.WriteLine(ex.Message)
            End Try

            ' Next
            Me.Add(Me.m_dsNext)

        End Sub

        Private Delegate Sub OnSpatialIndexUpdatedDelegate(ByVal ds As ISpatialDataSet, ByVal sProgress As Single)

        Private Sub OnSpatialIndexUpdated(ByVal ds As ISpatialDataSet, ByVal sProgress As Single)
            If (Me.m_core IsNot Nothing) Then
                Try
                    Dim state As eProgressState = eProgressState.Running
                    Dim strMessage As String = ""

                    If (ds IsNot Nothing) Then
                        strMessage = String.Format(My.Resources.CoreMessages.STATUS_INDEXING_DATASET, ds.DisplayName)
                    End If

                    If sProgress = 0 Then
                        state = eProgressState.Start
                    ElseIf sProgress = 1.0 Then
                        state = eProgressState.Finished
                    End If

                    Dim msg As New cProgressMessage(state, 1, sProgress, strMessage, eMessageType.Progress, eDataTypes.EcospaceSpatialDataConnection)
                    msg.Source = eCoreComponentType.External

                    Me.m_core.Messages.SendMessage(msg)

                Catch ex As Exception
                    ' Hmm
                    Debug.Assert(False, ex.Message)
                End Try
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace
