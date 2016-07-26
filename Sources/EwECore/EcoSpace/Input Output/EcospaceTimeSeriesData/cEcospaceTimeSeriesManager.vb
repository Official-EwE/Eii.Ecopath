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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Option Explicit On
Imports EwECore
Imports EwEUtils


Namespace EcospaceTimeSeries

    Public Class cEcospaceTimeSeriesManager

#Region "Private data"


        Private m_dcDataByDate As Dictionary(Of Date, List(Of cEcospaceTimeSeriesRec))
        Private m_core As cCore
        Private m_SpaceData As cEcospaceDataStructures

        'sum of squares by group
        Private m_ss() As Single

        'Naming convection for SS variables follows
        'EwE5 and Ecosim.AccumulateDataInfo() and PlotDataInfo()

        'stored log error, one record for each cell/timestep
        'log(obs/pred)
        Private Erpred As List(Of Double)

        'sum of log error
        'sumof(log(obs/pred))
        Private DatSumZ As Double

        'squared sum of log error
        'sumof(log(obs/pred)^2)
        Private DatSumZ2 As Double

#End Region

#Region "Construction Initialization"


        Public Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)

            Me.m_core = Core
            Me.m_SpaceData = EcospaceData

        End Sub


        Private Sub Init()
            Me.m_dcDataByDate = New Dictionary(Of Date, List(Of cEcospaceTimeSeriesRec))
            Me.m_ss = New Single(Me.m_core.nGroups) {}
            Erpred = New List(Of Double)

            Me.DatSumZ = 0.0
            Me.DatSumZ2 = 0.0

        End Sub


#End Region

#Region "Public Methods"


        Public Function Add(TimeSeriesRec As cEcospaceTimeSeriesRec) As Boolean

            Try
                Me.ByDate(TimeSeriesRec.TimeStamp, CreateNew:=True).Add(TimeSeriesRec)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function


        Public Function Read(Filename As String) As Boolean

            Me.Init()

            Try
                If IO.File.Exists(Filename) Then
                    Dim reader As New cEcospaceTimeSeriesXYZReader(Filename, Me)
                    If reader.Read() Then
                        Me.checkDates(reader.StartDate, reader.EndDate)
                        Me.checkExtent(reader.MaxRow, reader.MaxCol)
                    End If
                End If
            Catch ex As Exception

            End Try
            Return False
        End Function


        Public Function CalculateStats(iTimeStep As Integer, biomass(,,) As Single) As Boolean
            Dim zstat As Double
            Dim TimeStepDate As Date = Me.getDate(iTimeStep)
            Try

                If Me.ContainsDate(TimeStepDate) Then

                    'get a list of all the records for this date
                    For Each Rec As cEcospaceTimeSeriesRec In Me.ByDate(TimeStepDate)

                        'There is no zero value or bounds checking
                        'so trap all the errors until we are doing something better
                        Try
                            'Nope but this will work for testing
                            Me.m_ss(Rec.iGroupID) += CSng((biomass(Rec.iGroupID, Rec.Row, Rec.Col) - Rec.CellValue) ^ 2)

                            'log prediction error
                            zstat = Math.Log(Rec.CellValue / biomass(Rec.iGroupID, Rec.Row, Rec.Col))

                            Me.Erpred.Add(zstat)
                            Me.DatSumZ += zstat
                            Me.DatSumZ2 += zstat ^ 2
                        Catch ex As Exception
                            System.Console.WriteLine(Me.ToString + ".CalculateStats() Invalid data point.")
                        End Try

                    Next Rec

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'for debugging
                    Me.dumpDebugData()
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                End If

            Catch ex As Exception
                EwEUtils.Core.cLog.Write(ex, "Ecospace Time Series failed to calculate stats for timestep " + iTimeStep.ToString)
            End Try

            Return True

        End Function

#End Region

#Region "Private Methods"


        Private Function ByDate(RecDate As Date, Optional CreateNew As Boolean = False) As List(Of cEcospaceTimeSeriesRec)
            If m_dcDataByDate.ContainsKey(RecDate) Then
                Return m_dcDataByDate.Item(RecDate)
            End If

            If CreateNew Then
                Dim recs As New List(Of cEcospaceTimeSeriesRec)
                m_dcDataByDate.Add(RecDate, recs)
                Return recs
            End If

            Return Nothing

        End Function



        Public ReadOnly Property ContainsData As Boolean
            Get
                If Me.m_dcDataByDate IsNot Nothing Then
                    Return Me.m_dcDataByDate.Count > 0
                End If
                Return False
            End Get
        End Property

        Private Function ContainsDate(RecDate As Date) As Boolean

            If m_dcDataByDate.ContainsKey(RecDate) Then
                Return True
            End If
            Return False

        End Function

        Private Function getDate(itimestep As Integer) As Date
            'convert Ecospace time step into date
            Dim stYear As Integer
            If Me.m_core.EwEModel.FirstYear <> 0 Then
                stYear = Me.m_core.EwEModel.FirstYear
            Else
                stYear = 1
            End If

            Dim StartDate As New Date(stYear, 1, 1)
            Dim nmonths As Integer = CInt(Math.Truncate((itimestep - 1) * Me.m_SpaceData.TimeStep * 12))
            Dim tsDate As Date = StartDate.AddMonths(CInt(nmonths))
            Return tsDate

        End Function


        Private Function checkDates(StartDate As Date, EndDate As Date) As Boolean
            If Me.m_core.EwEModel.FirstYear <> 0 Then
                Dim mSD As New Date(Me.m_core.EwEModel.FirstYear, 1, 1)
                Dim mED As New Date(CInt(Me.m_core.EwEModel.FirstYear + Me.m_SpaceData.TotalTime), 1, 1)
                If StartDate > mED Or EndDate < mSD Then
                    'Failed 
                    Debug.Assert(False, "Oppss Time series dates out of bounds. Check the Model date.")
                    Return False
                End If
            Else
                Debug.Assert(False, "Oppss Model date has not been set")
                Return False
            End If
            Return True
        End Function

        Private Function checkExtent(MaxRow As Integer, MaxCol As Integer) As Boolean

            If MaxRow > Me.m_SpaceData.InRow Or MaxCol > Me.m_SpaceData.InCol Then
                Debug.Assert(False, "Oppss Time Series map exceeds the Ecospace map extent.")
                Return False
            End If

            Return True

        End Function


        Private Sub dumpDebugData()
            'dump data to console window for debugging
            System.Console.WriteLine("sum of log(obs/pred)=" + Me.DatSumZ2.ToString)
            For igrp As Integer = 1 To Me.m_core.nGroups
                If Me.m_ss(igrp) > 0 Then
                    System.Console.WriteLine("Group=" + igrp.ToString + ", SS=" + Me.m_ss(igrp).ToString)
                End If
            Next
        End Sub

#End Region

    End Class

End Namespace
