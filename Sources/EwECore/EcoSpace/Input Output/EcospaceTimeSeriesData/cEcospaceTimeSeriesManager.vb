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

    Public Enum eTimeSeriesRecValidations
        isValid
        MalformedString
        InvalidDateFormat
        EmptyRec
    End Enum

    Public Class cEcospaceTimeSeriesManager

        'ToDo 27-July-2016 Add Error messages back to the core instead of just Asserts
        '   Done 28-July-2016 Added core messages if read throws an Exception 
        '   Done 28-July-2016 Also sends message if dates or extents are out of bounds
        '   Done 29-Jul-2016 Validates records and sends message

        'ToDo 29-July-2016 Added message strings to resources

        'ToDo 27-July-2016  Document the file formats (input and output) and how it works

        'ToDo 27-July-2016 Let the user selected the output file name. 
        '   Maybe when the user is selecting the input file have them choose the output file
        '   Use the default filename

        'ToDo 27-July-2016 Added SS output to the UI. Results form... Main Run UI some place?

        'ToDo 27-July-2016 Added Group SS output to Results form

        'ToDo 27-July-2016 remove the DebugDump
        '   Done 29-Jul-2016 

#Region "Public data/properties"

        Public TimeStepFormatString As String = "yyyy-MM-dd"

#End Region


#Region "Private data"


        Private m_dcDataByDate As Dictionary(Of Date, List(Of cEcospaceTimeSeriesRec))
        Private m_core As cCore
        Private m_SpaceData As cEcospaceDataStructures

        'sum of squares by group
        Private m_ss() As Double

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

        Private m_FileName As String
        Private m_OutputFilename As String

#End Region

#Region "Construction Initialization"


        Public Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)

            Me.m_core = Core
            Me.m_SpaceData = EcospaceData

            'Create a new list of cEcospaceTimeSeriesRec
            Me.m_dcDataByDate = New Dictionary(Of Date, List(Of cEcospaceTimeSeriesRec))

        End Sub


        Public Sub InitForRun()

            Try

                'Clear out the results
                Me.m_ss = New Double(Me.m_core.nGroups) {}
                Erpred = New List(Of Double)

                Me.DatSumZ = 0.0
                Me.DatSumZ2 = 0.0

                'Clear out the results part of the cEcospaceTimeSeriesRec objects
                For Each recs As List(Of cEcospaceTimeSeriesRec) In Me.m_dcDataByDate.Values
                    For Each rec As cEcospaceTimeSeriesRec In recs
                        rec.PredictedValue = cCore.NULL_VALUE
                        rec.SS = cCore.NULL_VALUE
                    Next
                Next

            Catch ex As Exception

            End Try

        End Sub


        Private Sub InitForRead()
            'Create a new list of cEcospaceTimeSeriesRec
            Me.m_dcDataByDate = New Dictionary(Of Date, List(Of cEcospaceTimeSeriesRec))
        End Sub

        Public Sub Clear()
            Me.m_dcDataByDate.Clear()
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Added a new cEcospaceTimeSeriesRec record. 
        ''' </summary>
        ''' <param name="TimeSeriesRec"></param>
        ''' <returns></returns>
        Public Function Add(TimeSeriesRec As cEcospaceTimeSeriesRec) As Boolean

            Try
                'Add TimeSeriesRec to the list of cEcospaceTimeSeriesRec objects
                'cEcospaceTimeSeriesRec are stored by date, all the recs with the same date will be in one list
                'Me.ByDate(date,CreateNew:=True) will create a new list if it doesn't already exist
                Me.RecsByDate(TimeSeriesRec.TimeStamp, CreateNew:=True).Add(TimeSeriesRec)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Read the Ecospace time series XYZ formatted file 
        ''' </summary>
        ''' <param name="InputFilename"></param>
        ''' <returns></returns>
        Public Function Load(InputFilename As String, OutputFileName As String) As Boolean
            Dim bReturn As Boolean = True
            Me.m_FileName = InputFilename
            Me.m_OutputFilename = OutputFileName

            If Not IO.File.Exists(InputFilename) Then
                System.Console.WriteLine(Me.ToString + ".Read() file does not exist!")
                Return False
            End If

            Me.InitForRead()

            Try
                Dim reader As New cEcospaceTimeSeriesXYZReader(InputFilename, Me)

                If reader.Read() Then
                    Me.checkDates(reader.StartDate, reader.EndDate)
                    Me.checkExtent(reader.MaxRow, reader.MaxCol)
                    bReturn = True
                End If 'If reader.Read() Then

            Catch ex As Exception
                'cEcospaceTimeSeriesXYZReader.Read() will throw the exception back here is there if there is an internal exception
                Me.m_core.Messages.AddMessage(New cMessage("Ecospace could not load time series data due to error: " + ex.Message,
                                                            EwEUtils.Core.eMessageType.ErrorEncountered,
                                                            EwEUtils.Core.eCoreComponentType.EcoSpace, EwEUtils.Core.eMessageImportance.Warning))
                'Clear out any data that may been read
                Me.m_dcDataByDate.Clear()
                bReturn = False
            End Try

            Me.m_core.Messages.AddMessage(New cMessage("Ecospace Timeseries " + Me.nRecords.ToString + " records loaded.",
                                                       EwEUtils.Core.eMessageType.Any, EwEUtils.Core.eCoreComponentType.EcoSpace,
                                                       EwEUtils.Core.eMessageImportance.Information))

            Me.m_core.Messages.sendAllMessages()

            Return bReturn

        End Function

        ''' <summary>
        ''' Calculate stats for this time step 
        ''' </summary>
        ''' <param name="iTimeStep">Current model time step</param>
        ''' <param name="biomass">Predicted biomass</param>
        ''' <returns></returns>
        Public Function CalculateStats(iTimeStep As Integer, biomass(,,) As Single) As Boolean
            Dim zstat As Double
            Dim TimeStepDate As Date = Me.TimeStepToDate(iTimeStep)
            Try

                'is there records for this model date
                If Me.ContainsDate(TimeStepDate) Then

                    'get a list of all the records for this date
                    For Each Rec As cEcospaceTimeSeriesRec In Me.RecsByDate(TimeStepDate)

                        ' System.Console.WriteLine("Ecospace Timeseries group=" + Rec.iGroupID.ToString + ", Date=" + Rec.TimeStamp.ToShortDateString)

                        'There is no zero value or bounds checking
                        'so trap all the errors until we are doing something better
                        Try

                            'log prediction error
                            zstat = Math.Log(Rec.CellValue / biomass(Rec.Row, Rec.Col, Rec.iGroupID))

                            'save the predicted and calculated SS values back into the record
                            Rec.PredictedValue = biomass(Rec.Row, Rec.Col, Rec.iGroupID)
                            Rec.SS = zstat

                            'By Group
                            Me.m_ss(Rec.iGroupID) += zstat ^ 2

                            'Debug.Assert(Not Double.IsNaN(zstat))
                            If Not Double.IsNaN(zstat) And Not Double.IsInfinity(zstat) Then
                                Me.Erpred.Add(zstat)
                                Me.DatSumZ += zstat
                                Me.DatSumZ2 += zstat ^ 2
                            End If

                            'shouldn't happen!
                            Debug.Assert(Not Double.IsNaN(Me.DatSumZ2))

                        Catch ex As Exception
                            'What to do if a data point throws an exception???
                            System.Console.WriteLine(Me.ToString + ".CalculateStats() Invalid data point.")
                        End Try

                    Next Rec

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'for debugging
                    'Me.dumpDebugData()
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                End If

            Catch ex As Exception
                'This shouldn't happen!
                'If it does it's some kind of a programming error...Really...
                Debug.Assert(False, "Ecospace Time Series failed to calculate stats for timestep " + iTimeStep.ToString)
                Return False
            End Try

            Return True

        End Function


        Public ReadOnly Property SS As Double
            Get
                Return Me.DatSumZ2
            End Get
        End Property

        Public ReadOnly Property SSGroup(igrp As Integer) As Double
            Get
                Return Me.m_ss(igrp)
            End Get
        End Property


        Public Sub RunCompleted()
            Try
                Me.SaveResults()
            Catch ex As Exception

            End Try
        End Sub


        'Public Property OutputFileName As String
        '    Get
        '        Return Me.m_OutputFilename
        '    End Get
        '    Set(value As String)
        '        Me.m_OutputFilename = value
        '    End Set
        'End Property

        Friend ReadOnly Property Core As cCore
            Get
                Return Me.m_core
            End Get
        End Property


#End Region

#Region "Private Methods"


        ''' <summary>
        ''' Get a list of cEcospaceTimeSeriesRec objects for this date. 
        ''' If CreateNew = True add a new list and return it, if CreateNew = False return nothing.   
        ''' </summary>
        ''' <param name="RecDate"></param>
        ''' <param name="CreateNew"></param>
        ''' <returns></returns>
        Private Function RecsByDate(RecDate As Date, Optional CreateNew As Boolean = False) As List(Of cEcospaceTimeSeriesRec)
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

        Public ReadOnly Property nRecords As Integer
            Get
                Return Me.m_dcDataByDate.Count
            End Get
        End Property


        ''' <summary>
        ''' Is there Ecospace time series data loaded
        ''' </summary>
        ''' <returns>True if there is loaded data, False otherwise. Does not test the map bounds or dates.</returns>
        Public ReadOnly Property ContainsData As Boolean
            Get
                If Me.m_dcDataByDate IsNot Nothing Then
                    Return Me.m_dcDataByDate.Count > 0
                End If
                Return False
            End Get
        End Property


        ''' <summary>
        ''' Does the currently loaded data contain this date
        ''' </summary>
        ''' <param name="RecDate"></param>
        ''' <returns></returns>
        Private Function ContainsDate(RecDate As Date) As Boolean

            If m_dcDataByDate.ContainsKey(RecDate) Then
                Return True
            End If
            Return False

        End Function



        ''' <summary>
        ''' Get the calendar date for the current model time step
        ''' </summary>
        ''' <param name="itimestep"></param>
        ''' <returns></returns>
        Private Function TimeStepToDate(itimestep As Integer) As Date
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

        ''' <summary>
        ''' Are the start date and the end date of the input time series data within the model run
        ''' </summary>
        ''' <param name="StartDate"></param>
        ''' <param name="EndDate"></param>
        ''' <returns>True if any part of the dates are in bounds, False otherwise. </returns>
        Private Function checkDates(StartDate As Date, EndDate As Date) As Boolean
            Dim msg As New System.Text.StringBuilder
            Dim bReturn As Boolean = True

            If Me.m_core.EwEModel.FirstYear <> 0 Then
                Dim mSD As New Date(Me.m_core.EwEModel.FirstYear, 1, 1)
                Dim mED As New Date(CInt(Me.m_core.EwEModel.FirstYear + Me.m_SpaceData.TotalTime), 1, 1)
                If StartDate > mED Or EndDate < mSD Then
                    'Failed date bounds
                    msg.Append("Ecospace time series dates " + StartDate.ToShortDateString + " to " + EndDate.ToShortDateString + " do not overlap with current model dates. ")
                    msg.Append("Check Model date or dates in input file.")

                    bReturn = False
                End If
            Else
                'First year = 0 
                'The user has not set a model data
                msg.Append("EwE Model date has not been set. You must set this to use Ecospace time series data.")

                bReturn = False
            End If

            If msg.Length > 0 Then
                Me.m_core.Messages.AddMessage(New cMessage(msg.ToString, EwEUtils.Core.eMessageType.DataValidation, EwEUtils.Core.eCoreComponentType.EcoSpace, EwEUtils.Core.eMessageImportance.Warning))
            End If

            Return bReturn

        End Function


        ''' <summary>
        ''' Check the Extent of the input time series data against the current Ecospace map extent
        ''' </summary>
        ''' <param name="MaxRow"></param>
        ''' <param name="MaxCol"></param>
        ''' <returns>Return True if the row and col are inbounds, False otherwise.</returns>
        Private Function checkExtent(MaxRow As Integer, MaxCol As Integer) As Boolean

            If MaxRow > Me.m_SpaceData.InRow Or MaxCol > Me.m_SpaceData.InCol Then
                'Debug.Assert(False, "Oppss Time Series map exceeds the Ecospace map extent.")
                Dim msg As New System.Text.StringBuilder
                msg.Append("Ecospace time series map extents outside the currently load map.")
                Me.m_core.Messages.AddMessage(New cMessage(msg.ToString, EwEUtils.Core.eMessageType.DataValidation, EwEUtils.Core.eCoreComponentType.EcoSpace, EwEUtils.Core.eMessageImportance.Warning))
                Return False
            End If

            Return True

        End Function

        Private Sub SaveResults()

            If Not Me.ContainsData Then
                'nothing the save
                Exit Sub
            End If

            Dim msg As Text.StringBuilder

            Try
                Dim header As String = "Row,Col,GroupID,Date(yyyy-MM-dd),ObservedValue,PredictedValue,PredictionError(LogN(ObservedValue/PredictedValue)"
                '   Dim outPutFileName As String = Me.getDefaultOutputFileName(Me.m_FileName)
                Dim strm As New IO.StreamWriter(Me.m_OutputFilename)
                strm.WriteLine(header)
                For Each recs As List(Of cEcospaceTimeSeriesRec) In Me.m_dcDataByDate.Values
                    For Each rec As cEcospaceTimeSeriesRec In recs
                        If rec.PredictedValue <> cCore.NULL_VALUE Then
                            strm.WriteLine(rec.ToCSVString)
                        End If
                    Next
                Next

                strm.Close()

            Catch ex As Exception
                EwEUtils.Core.cLog.Write(ex, Me.ToString + ".SaveResults() Exception")
                msg = New Text.StringBuilder
                msg.Append("Ecospace Timeseries exception saving output file " + ex.Message)
            End Try

            If msg IsNot Nothing Then
                Me.m_core.Messages.AddMessage(New cMessage(msg.ToString, EwEUtils.Core.eMessageType.Any,
                    EwEUtils.Core.eCoreComponentType.EcoSpace, EwEUtils.Core.eMessageImportance.Warning))
            End If

        End Sub

        Public Function getDefaultOutputFileName(InputFileName As String) As String
            Me.m_FileName = InputFileName
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.m_FileName), IO.Path.GetFileNameWithoutExtension(Me.m_FileName) + "_SS-Results.csv")
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
