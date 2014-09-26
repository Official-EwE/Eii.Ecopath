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
Imports System.Drawing

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

            ' JS 25jan14: A fundamental weakness in the earlier implementation of 
            ' the indexing process is that its ability to index, and more importantly, 
            ' its ability to abort indexing when needed, totally relies on the 
            ' implementation of indexing logic within individual datasets. If the 
            ' indexing process of a dataset somehow deadlocks, the ability to run the 
            ' spatial temporal framework is stalled, and user interfaces may be 
            ' deadlocked. This is not good. As a solution, control over the indexing 
            ' process has been moved to this class. 

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
                    ' #Yes: Stop processing current dataset
                    Me.m_dsCurrent = Nothing
                Else
                    ' #No: ah, ready for a new dataset to index
                    ' Get the dataset that is lined up next
                    Me.m_dsCurrent = Me.m_dsNext
                    Me.m_dsNext = Nothing
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
            Dim iTS As Integer = 1
            Dim nTS As Integer = Me.m_core.nEcospaceTimeSteps
            Dim dt As DateTime
            Dim ptfTL As New PointF(-180, 90)
            Dim ptfBR As New PointF(180, -90)
            Dim c As ISpatialDataCache = Nothing
            Dim strMessage As String = ""
            Dim bDone As Boolean = False

            If (ds IsNot Nothing) Then
                If (ds.IsConfigured) Then

                    c = ds.Cache

                    Try
                        strMessage = String.Format(My.Resources.CoreMessages.STATUS_INDEXING_DATASET, ds.DisplayName)
                        Me.OnSpatialIndexUpdated(strMessage, eProgressState.Start, 0)

                        While Not bDone

                            dt = Me.m_core.EcospaceTimestepToAbsoluteTime(iTS)
                            If (ds.HasDataAtT(dt)) Then
                                If (ds.IndexStatusAtT(dt) = ISpatialDataSet.eIndexStatus.NotIndexed) Then
                                    ' ToDo: Every dataset call should be subject to a timeout
                                    ds.UpdateIndexAtT(dt)
                                    Dim comp As New cDatasetCompatilibity(Me.m_core, ds)
                                    Me.OnSpatialIndexUpdated(strMessage, eProgressState.Running, CSng(comp.NumIndexed / comp.NumOverlappingTimeSteps))
                                End If
                            End If

                            ' Next
                            iTS += 1
                            bDone = (Not Object.ReferenceEquals(Me.m_dsCurrent, ds)) Or _
                                    (iTS > Me.m_core.nEcospaceTimeSteps)
                        End While

                    Catch ex As Threading.ThreadAbortException
                        ' NOP
                    Catch ex As Exception
                        cLog.Write(ex, "cSpatialDatasetIndexer::IndexDatasetThread(" & ds.DisplayName & ")")
                        'Console.WriteLine(ex.Message)
                    Finally
                        ' Cleanup: restore cache
                        ds.Cache = c
                        ' Flag that this indexing is done
                        Me.m_dsCurrent = Nothing
                        ' Done threading
                        Me.m_threadIndex = Nothing
                        ' Done (send just in case)
                        Me.OnSpatialIndexUpdated("", eProgressState.Finished, 1.0!)
                    End Try
                End If
            End If

            ' Next, if any
            Me.Add(Me.m_dsNext)

        End Sub

        Private Sub OnSpatialIndexUpdated(ByVal strMessage As String, _
                                          ByVal state As eProgressState, _
                                          ByVal sProgress As Single)

            If (Me.m_core IsNot Nothing) Then
                Try
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
