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

Option Strict On
Imports EwEUtils.Core

''' <summary>
''' Class that holds and manages all time series-related data in the EwE core.
''' </summary>
Public Class cTimeSeriesDataStructures

    Public Const ANNUAL_DATA_MONTH As Integer = 6

    Private m_SimData As cEcosimDatastructures
    Private m_PathData As cEcopathDataStructures

    Public nGroups As Integer
    Public nFleets As Integer

    ' ------------------------------------------------
    ' Dataset structures
    ' ------------------------------------------------

    Public ActiveDatasetIndex As Integer = cCore.NULL_VALUE

    ''' <summary>Number of available datasets.</summary>
    Public nDatasets As Integer = 0
    ''' <summary>Dataset database IDs</summary>
    Public DatasetDBID() As Integer
    ''' <summary>Names of available datasets.</summary>
    Public DatasetName() As String
    ''' <summary>Authors of available datasets.</summary>
    Public DatasetAuthor() As String
    Public DatasetContact() As String
    ''' <summary>Descriptions of available datasets.</summary>
    Public DatasetDescription() As String
    ''' <summary>Number of time series contained in each dataset.</summary>
    Public DatasetNumTimeSeries() As Integer
    Public DatasetFirstYear() As Integer
    ''' <summary>The number of data points in the dataset.</summary>
    Public DatasetNumPoints() As Integer

    Public DataSetIntervals() As eTSDataSetInterval

    ' ------------------------------------------------
    ' Interface structures
    ' ------------------------------------------------

    ''' <summary>Number of time series in the model.</summary>
    Public nTimeSeries As Integer

    ''' <summary>Maximum number of years across all time series.</summary>
    Public nMaxYears As Integer
    ''' <summary>Database ID for each time series.</summary>
    Public TimeSeriesDBID() As Integer
    ''' <summary>Name of each time series.</summary>
    Public TimeSeriesName() As String
    ''' <summary>Array of flags indicating which a time series must be applied.</summary>
    Public TimeSeriesEnabled() As Boolean
    ''' <summary>Type of each time series.</summary>
    Public TimeSeriesType() As eTimeSeriesType
    ''' <summary>Index of the core object that each time series links to. The type
    ''' of the core object is implied by <see cref="TimeSeriesType">TimeSeriesType</see>.</summary>
    Public TimeSeriesPool() As Integer
    ''' <summary>Index of the core object of a secundary time series target, if applicable. The type
    ''' of the core object is implied by <see cref="TimeSeriesType">TimeSeriesType</see>.</summary>
    Public TimeSeriesPoolSec() As Integer
    ''' <summary>Weight type for each time series.</summary>
    Public TimeSeriesWeight() As Single
    ''' <summary>CV for each time series.</summary>
    Public TimeSeriesCV() As Single
    ''' <summary>Time values for each time series, indexed as (iTime, iSeries).</summary>
    Public TimeSeriesValues(,) As Single
    Public TimeSeriesDatSS() As Single
    Public TimeSeriesDatQ() As Single
    Public TimeSeriesEDatQ() As Single 'exp(sDatQ)

    ''' <summary>Weighted Sum of Squared Prediction Error by time series data set sumof(log(observed(i)/predicted(i))^2) * [timeseries weight(i)].</summary>
    Public TimeSeriesSSPredErr() As Single

    ' ------------------------------------------------
    ' Applied structures
    ' ------------------------------------------------

    ''' <summary>Number of applied time series.</summary>
    Public AppliedNdatType As Integer

    ''' <summary>Number of datum points across all applied time series.</summary>
    Public AppliedDatPoints As Integer


    Public AppliedAICTimeSeries As Integer

    ''' <summary><see cref="eTimeSeriesType">Type</see> of each applied time series.</summary>
    Public AppliedDatType() As eTimeSeriesType
    ''' <summary>Index of the core object that each applied time series links to. The type
    ''' of the core object is implied by <see cref="AppliedDatType">DatType</see>.</summary>
    Public AppliedDatPool() As Integer
    ''' <summary>Index of the second core object that each applied time series links to. The type
    ''' of the core object is implied by <see cref="AppliedDatType">DatType</see>.</summary>
    Public AppliedDatPoolSec() As Integer
    ''' <summary>Weight type for each applied time series.</summary>
    Public AppliedWtType() As Single
    ' ''' <summary>Annual values for each applied time series, indexed as (iYear, iSeries).</summary>
    Public AppliedDatVal(,) As Single
    ''' <summary>Year of the datum point.</summary>
    Public AppliedDatYear() As Integer
    ''' <summary>Sum of squares contribution (x time series)</summary>
    Public AppliedDatSS() As Single

    ''' <summary>Time interval of the currently selected dataset (monthly or annual).</summary>
    Public AppliedDataSetInterval As eTSDataSetInterval

    ''' <summary>Sum of Squared Prediction Error by time series data set sumof(log(observed(i)/predicted(i))^2) * [timeseries weight(i)].</summary>
    Public AppliedSSPredErr() As Single

    ''' <summary>mean(sumof(log(observed/predicted))) by data type</summary>
    Public AppliedDatQ() As Single
    Public AppliedeDatQ() As Single

    Public PoolForceBB(,) As Single
    ''' <summary>Forced fishing mortalities. By Group,Time</summary>
    Public PoolForceZ(,) As Single
    ''' <summary>Forced catch. By Group,Time</summary>
    Public PoolForceCatch(,) As Single
    Public ForcedFs(,) As Single

    ''' <summary>Proportion of total catch that is discarded. By Fleet,Group,Time</summary>
    Public PoolForceDiscardProp(,,) As Single
    ''' <summary>Proportion of discards that incur mortality. By Fleet,Group,Time</summary>
    Public PoolForceDiscardMort(,,) As Single
    ''' <summary>Catchabilities. By Fleet,Group,Time</summary>
    Public PoolForceCatchabilities(,,) As Single

    ' - Money -
    ''' <summary>Ecopath off-vessel price. By Fleet,Group,Time</summary>
    Public PoolForceOffVesselPriceAbs(,,) As Single
    ''' <summary>Ecopath off-vessel price multiplier. By Fleet,Group,Time</summary>
    Public PoolForceOffVesselPriceRel(,,) As Single
    ''' <summary>Ecopath CUPE cost multiplier. By Fleet,Time</summary>
    Public PoolForceEffortCostAbs(,) As Single
    ''' <summary>Ecopath CUPE cost multiplier. By Fleet,Time</summary>
    Public PoolForceEffortCostRel(,) As Single
    ''' <summary>Ecopath sail cost multiplier. By Fleet,Time</summary>
    Public PoolForceSailCostRel(,) As Single
    ''' <summary>Ecopath sail cost (abs). By Fleet,Time</summary>
    Public PoolForceSailCostAbs(,) As Single
    ''' <summary>Ecopath Fixed cost (abs). By Fleet,Time</summary>
    Public PoolForceFixedCostAbs(,) As Single
    ''' <summary>Ecopath Fixed cost multiplier. By Fleet,Time</summary>
    Public PoolForceFixedCostRel(,) As Single

    ''' <summary>
    ''' Index to the current year/datatype
    ''' </summary>
    ''' <remarks>This is increment for each data type each time the stats are collected. Once a year.</remarks>
    Public Iobs As Integer
    Public Wt() As Single

    ''' <summary>
    ''' Value predicted by the model (log). 
    ''' Used by cF2TSModel fit to time series to tell how much change each parameter tweak made.
    ''' </summary>
    Public Yhat() As Single

    ''' <summary>log(observed/predicted) by observation</summary>
    Public Erpred() As Single

    Public Sub New(EcopathData As cEcopathDataStructures, EcosimData As cEcosimDatastructures)
        Me.m_PathData = EcopathData
        Me.m_SimData = EcosimData
    End Sub

    ''' <summary>
    ''' Clear all time series data and free memory
    ''' </summary>
    Friend Sub Clear()
        Me.nDatasets = 0
        Me.ActiveDatasetIndex = cCore.NULL_VALUE
        Me.RedimTimeSeriesDatasets()
        Me.ClearTimeSeries()
    End Sub

    ''' <summary>
    ''' Number of years in the reference data set
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property nYears As Integer
        Get
            Dim n As Integer
            Select Case Me.AppliedDataSetInterval
                Case eTSDataSetInterval.Annual
                    n = Me.AppliedDatPoints
                Case eTSDataSetInterval.TimeStep
                    n = Me.AppliedDatPoints \ cCore.N_MONTHS
                Case Else
                    Debug.Assert(False, Me.ToString + " Undefined DataSetInterval type.")
            End Select
            Return n
        End Get
    End Property

    Public Function toForcingTimeStep(iModelTimeStep As Integer, iModelYear As Integer) As Integer

        Dim its As Integer
        If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
            its = iModelYear
        ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
            its = iModelTimeStep
        End If

        'jb let the forcing index exceed the originaly loaded forcing data
        'If the run length was extened then the data in the forcing arrays will be zero
        Return its

    End Function


    Public Function isTimeStepValid(iModelTimeStep As Integer) As Boolean
        ' System.Console.WriteLine(iModelTimeStep.ToString)
        If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
            If (iModelTimeStep / cCore.N_MONTHS) <= Me.nYears Then Return True
        ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
            If iModelTimeStep <= Me.AppliedDatPoints Then Return True
        End If

        Return False

    End Function

    Private Sub ClearForcing()

        If (Me.PoolForceBB IsNot Nothing) Then

            Array.Clear(Me.PoolForceBB, 0, Me.PoolForceBB.Length)
            Array.Clear(Me.PoolForceCatch, 0, Me.PoolForceCatch.Length)
            Array.Clear(Me.PoolForceZ, 0, Me.PoolForceZ.Length)

            Array.Clear(Me.PoolForceDiscardMort, 0, Me.PoolForceDiscardMort.Length)
            Array.Clear(Me.PoolForceDiscardProp, 0, Me.PoolForceDiscardProp.Length)
            Array.Clear(Me.PoolForceCatchabilities, 0, Me.PoolForceCatchabilities.Length)
            Array.Clear(Me.PoolForceCatchabilities, 0, Me.PoolForceCatchabilities.Length)

            ' Also reset the F is forced flag
            For igrp As Integer = 0 To Me.nGroups
                For ipt As Integer = 0 To Me.AppliedDatPoints * cCore.N_MONTHS
                    Me.ForcedFs(igrp, ipt) = cCore.NULL_VALUE
                Next
            Next

            Me.InitForcedDiscards()
        End If

    End Sub

    Friend Sub RedimTimeSeriesDatasets()

        ReDim Me.DatasetDBID(Me.nDatasets)
        ReDim Me.DatasetName(Me.nDatasets)
        ReDim Me.DatasetDescription(Me.nDatasets)
        ReDim Me.DatasetAuthor(Me.nDatasets)
        ReDim Me.DatasetContact(Me.nDatasets)
        ReDim Me.DatasetFirstYear(Me.nDatasets)
        ReDim Me.DatasetNumPoints(Me.nDatasets)
        ReDim Me.DatasetNumTimeSeries(Me.nDatasets)

        ReDim Me.DataSetIntervals(Me.nDatasets)

    End Sub

    Friend Sub ClearTimeSeries()

        Me.nTimeSeries = 0
        Me.nMaxYears = 0
        Me.AppliedDatPoints = 0
        Me.AppliedNdatType = 0

        Me.RedimTimeSeries()
        Me.RedimEnabledTimeSeries()

        'jb 16-Feb-2010 Fixed bug that caused RedimForcingData() to throw a redim preserve exception when loading a second model that contained more groups
        'RedimForcingData() tries to preserve existing data if the number of timesteps changed  
        'this clears out the existing forcing data which forces RedimForcingData() to allocate new memory instead of trying to preserve the existing values
        Erase Me.PoolForceBB
        Erase Me.PoolForceCatch
        Erase Me.PoolForceZ

        Erase Me.PoolForceDiscardProp
        Erase Me.PoolForceDiscardMort
        Erase Me.PoolForceCatchabilities
        Erase Me.PoolForceOffVesselPriceAbs
        Erase Me.PoolForceOffVesselPriceRel
        Erase Me.PoolForceEffortCostRel
        Erase Me.PoolForceEffortCostAbs
        Erase Me.PoolForceSailCostAbs
        Erase Me.PoolForceSailCostRel
        Erase Me.PoolForceFixedCostAbs
        Erase Me.PoolForceFixedCostRel

    End Sub

    Friend Sub RedimTimeSeries()

        Debug.Assert(Me.nTimeSeries >= 0, Me.ToString & ".RedimTimeSeries() nNumTimeSeries cannot be negative")
        Debug.Assert(Me.nMaxYears >= 0, Me.ToString & ".RedimTimeSeries() NdatYear cannot be negative")

        ' Redim interface time series arrays
        ReDim Me.TimeSeriesDBID(Me.nTimeSeries)
        ReDim Me.TimeSeriesName(Me.nTimeSeries)
        ReDim Me.TimeSeriesEnabled(Me.nTimeSeries)
        ReDim Me.TimeSeriesPool(Me.nTimeSeries)
        ReDim Me.TimeSeriesPoolSec(Me.nTimeSeries)
        ReDim Me.TimeSeriesWeight(Me.nTimeSeries)
        ReDim Me.TimeSeriesCV(Me.nTimeSeries)
        ReDim Me.TimeSeriesType(Me.nTimeSeries)
        ReDim Me.TimeSeriesValues(Me.nMaxYears + 1, Me.nTimeSeries)
        ReDim Me.TimeSeriesDatSS(Me.nTimeSeries)
        ReDim Me.TimeSeriesSSPredErr(Me.nTimeSeries)

        ReDim Me.TimeSeriesDatQ(Me.nTimeSeries)
        ReDim Me.TimeSeriesEDatQ(Me.nTimeSeries)

        ReDim Me.AppliedDatSS(Me.nTimeSeries)
        ReDim Me.AppliedDatQ(Me.nTimeSeries)
        ReDim Me.AppliedeDatQ(Me.nTimeSeries)
        ReDim Me.AppliedSSPredErr(Me.nTimeSeries)

    End Sub

    Public Sub RedimEnabledTimeSeries()

        Debug.Assert(Me.AppliedNdatType >= 0, Me.ToString & ".RedimAppliedTimeSeries() NdatType cannot be negative")
        Debug.Assert(Me.AppliedDatPoints >= 0, Me.ToString & ".RedimAppliedTimeSeries() NdatYear cannot be negative")

        ' Redim applied time series arrays
        ReDim Me.AppliedDatPool(Me.AppliedNdatType)
        ReDim Me.AppliedDatPoolSec(Me.AppliedNdatType)
        ReDim Me.AppliedDatType(Me.AppliedNdatType)
        ReDim Me.AppliedWtType(Me.AppliedNdatType)
        ReDim Me.AppliedDatSS(Me.AppliedNdatType)
        ReDim Me.AppliedDatQ(Me.AppliedNdatType)
        ReDim Me.AppliedeDatQ(Me.AppliedNdatType)

        ReDim Me.AppliedDatYear(Me.AppliedDatPoints)
        ReDim Me.AppliedDatVal(Me.AppliedDatPoints + 1, Me.AppliedNdatType)

    End Sub

    ''' <summary>
    ''' Is there reference data for this model timestep
    ''' </summary>
    ''' <param name="iCumTimeStep"></param>
    ''' <param name="iMonth"></param>
    ''' <param name="iYear"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasData(iCumTimeStep As Integer, iMonth As Integer, iYear As Integer) As Boolean
        Dim breturn As Boolean = False

        If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
            If iYear <= Me.nYears And iMonth = ANNUAL_DATA_MONTH Then
                'Annual data
                'Within the time period
                'On the 6 month
                breturn = True
            End If

        ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then

            If iCumTimeStep <= Me.AppliedDatPoints Then
                breturn = True
            End If
        End If

        Return breturn

    End Function

    ''' <summary>
    ''' Get the reference data timestep index for this model timestep
    ''' </summary>
    ''' <param name="iIndexToSet"> Index of the data in DatVal(,)</param>
    ''' <param name="iCumTimeStep">Model cumulative timestep 1 - n timestep</param>
    ''' <param name="iMonth">Model month 1 - 12</param>
    ''' <param name="iYear">Model year 1 - n years</param>
    ''' <returns>True if there is reference data for this model timestep</returns>
    ''' <remarks></remarks>
    Public Function setRefDataIndex(ByRef iIndexToSet As Integer, iCumTimeStep As Integer,
                                    iMonth As Integer, iYear As Integer) As Boolean
        Dim breturn As Boolean = False
        iIndexToSet = cCore.NULL_VALUE

        If Me.HasData(iCumTimeStep, iMonth, iYear) Then
            If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
                'Set the time step index to the data
                iIndexToSet = iYear
                breturn = True

            ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then

                iIndexToSet = iCumTimeStep
                breturn = True
            End If
        End If

        Return breturn

    End Function

    ''' <summary>
    ''' Set whether a given group is biomass forced through time series.
    ''' </summary>
    ''' <param name="IsBiomassForced"></param>
    Public Sub SetBiomassForcing(IsBiomassForced() As Boolean)

        Try
            ' Abort if not initialized properly
            If (IsBiomassForced.Length <> Me.nGroups + 1) Then Return

            ' Set all group forcing to false
            Array.Clear(IsBiomassForced, 0, IsBiomassForced.Length)

            ' Abort if no time series loaded
            If (Me.PoolForceBB Is Nothing) Then Return

            For igrp As Integer = 1 To Me.nGroups
                Dim iDatPt As Integer = 1
                While Not IsBiomassForced(igrp) And iDatPt <= Me.AppliedDatPoints
                    IsBiomassForced(igrp) = (Me.PoolForceBB(igrp, iDatPt) > 0)
                    iDatPt += 1
                End While
            Next igrp

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".SetBiomassForcing() something went really wrong!")
            cLog.Write(ex, "cTimeSeriesDataStructures.SetBiomassForcing()")
        End Try

    End Sub

    ''' <summary>
    ''' Set whether a given group is discard forced through time series.
    ''' </summary>
    ''' <param name="IsDiscardForced"></param>
    Public Sub SetDiscardForcing(IsDiscardForced() As Boolean)

        Try
            ' Abort if not initialized properly
            If (IsDiscardForced.Length <> Me.nGroups + 1) Then Return

            ' Set all group forcing to false
            Array.Clear(IsDiscardForced, 0, IsDiscardForced.Length)

            ' Abort if no time series loaded
            If (Me.PoolForceDiscardMort Is Nothing) Then Return

            For igrp As Integer = 1 To Me.nGroups
                Dim iflt As Integer = 1
                While Not IsDiscardForced(igrp) And iflt <= Me.nFleets
                    Dim iDatPt As Integer = 1
                    While Not IsDiscardForced(igrp) And iDatPt <= Me.AppliedDatPoints
                        IsDiscardForced(igrp) = (Me.PoolForceDiscardMort(iflt, igrp, iDatPt) > 0) Or (Me.PoolForceDiscardProp(iflt, igrp, iDatPt) > 0)
                        iDatPt += 1
                    End While
                    iflt += 1
                End While
            Next igrp

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".SetBiomassForcing() something went really wrong!")
            cLog.Write(ex, "cTimeSeriesDataStructures.SetBiomassForcing()")
        End Try

    End Sub

    ''' <summary>
    ''' Redim time series forcing data PoolForceBB(nGroups, nYears),PoolForceZ(nGroups, nYears) and PoolForceCatch(nGroups, nYears)
    ''' </summary>
    ''' <param name="RunLengthYears">Ecosim run length in years</param>
    ''' <remarks></remarks>
    Public Sub redimForcingData(RunLengthYears As Integer)

        Try
            'What is the max number of datapoints that will be needed for this Ecosim run length
            'If the Ecosim run length is greater than the forcing data
            'Then we need to increase the number of forcing data points 
            'leaving the extra data points with zeros/no data
            Dim npoints As Integer
            If Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
                npoints = Math.Max(Me.AppliedDatPoints, RunLengthYears * cCore.N_MONTHS)
            Else
                npoints = Math.Max(Me.AppliedDatPoints, RunLengthYears)
            End If

            If Me.PoolForceBB Is Nothing Then
                'This is a first time initialization of the forcing data
                'Create the arrays
                'Populate the discard arrays with -9999, not a valid data point
                ReDim Me.PoolForceBB(Me.nGroups, npoints)
                ReDim Me.PoolForceZ(Me.nGroups, npoints)
                ReDim Me.PoolForceCatch(Me.nGroups, npoints)

                ReDim Me.ForcedFs(Me.nGroups, RunLengthYears * cCore.N_MONTHS)

                For igrp As Integer = 0 To Me.nGroups
                    For ipt As Integer = 0 To RunLengthYears * cCore.N_MONTHS
                        Me.ForcedFs(igrp, ipt) = cCore.NULL_VALUE
                    Next
                Next

                ReDim Me.PoolForceDiscardMort(Me.nFleets, Me.nGroups, npoints)
                ReDim Me.PoolForceDiscardProp(Me.nFleets, Me.nGroups, npoints)
                ReDim Me.PoolForceCatchabilities(Me.nFleets, Me.nGroups, npoints)

                ReDim Me.PoolForceOffVesselPriceAbs(Me.nFleets, Me.nGroups, npoints)
                ReDim Me.PoolForceOffVesselPriceRel(Me.nFleets, Me.nGroups, npoints)
                ReDim Me.PoolForceEffortCostAbs(Me.nFleets, npoints)
                ReDim Me.PoolForceEffortCostRel(Me.nFleets, npoints)
                ReDim Me.PoolForceSailCostAbs(Me.nFleets, npoints)
                ReDim Me.PoolForceSailCostRel(Me.nFleets, npoints)
                ReDim Me.PoolForceFixedCostAbs(Me.nFleets, npoints)
                ReDim Me.PoolForceFixedCostRel(Me.nFleets, npoints)

                Me.InitForcedDiscards()
                Return
            End If

            If npoints > Me.AppliedDatPoints Then
                'number of years the model is running for is greater then the forcing data
                'preserve the existing forcing data 
                ReDim Preserve Me.PoolForceBB(Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceZ(Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceCatch(Me.nGroups, npoints)
                '   ReDim Preserve ForcedFs(nGroups, npoints)

                ReDim Preserve Me.PoolForceDiscardMort(Me.nFleets, Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceDiscardProp(Me.nFleets, Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceCatchabilities(Me.nFleets, Me.nGroups, npoints)

                ReDim Preserve Me.PoolForceOffVesselPriceAbs(Me.nFleets, Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceOffVesselPriceRel(Me.nFleets, Me.nGroups, npoints)
                ReDim Preserve Me.PoolForceEffortCostAbs(Me.nFleets, npoints)
                ReDim Preserve Me.PoolForceEffortCostRel(Me.nFleets, npoints)
                ReDim Preserve Me.PoolForceSailCostRel(Me.nFleets, npoints)
                ReDim Preserve Me.PoolForceSailCostAbs(Me.nFleets, npoints)
                ReDim Preserve Me.PoolForceFixedCostAbs(Me.nFleets, npoints)
                ReDim Preserve Me.PoolForceFixedCostRel(Me.nFleets, npoints)

                ReDim Preserve Me.ForcedFs(Me.nGroups, RunLengthYears * cCore.N_MONTHS)

                For igrp As Integer = 0 To Me.nGroups
                    For ipt As Integer = Me.AppliedDatPoints * cCore.N_MONTHS + 1 To RunLengthYears * cCore.N_MONTHS
                        Me.ForcedFs(igrp, ipt) = cCore.NULL_VALUE
                    Next
                Next

            End If

            If RunLengthYears > Me.m_SimData.NumYears Then
                'Special case 
                'The code has extended the Ecosim run length(in years) 
                'The Fishing Policy Search does this
                'Set the discard forcing data in the extended period to -9999, not valid data
                Dim n As Integer

                If Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
                    n = Math.Max(Me.AppliedDatPoints, Me.m_SimData.NumYears * cCore.N_MONTHS)
                Else
                    n = Math.Max(Me.AppliedDatPoints, Me.m_SimData.NumYears)
                End If

                For iflt As Integer = 0 To Me.nFleets
                    For ipt As Integer = n + 1 To npoints
                        For igrp As Integer = 0 To Me.nGroups
                            Me.PoolForceDiscardMort(iflt, igrp, ipt) = cCore.NULL_VALUE
                            Me.PoolForceDiscardProp(iflt, igrp, ipt) = cCore.NULL_VALUE
                            Me.PoolForceCatchabilities(iflt, igrp, ipt) = cCore.NULL_VALUE
                            Me.PoolForceOffVesselPriceAbs(iflt, igrp, ipt) = 0
                            Me.PoolForceOffVesselPriceRel(iflt, igrp, ipt) = 0
                        Next
                        Me.PoolForceEffortCostAbs(iflt, ipt) = 0
                        Me.PoolForceEffortCostRel(iflt, ipt) = 0
                        Me.PoolForceSailCostRel(iflt, ipt) = 0
                        Me.PoolForceSailCostAbs(iflt, ipt) = 0
                        Me.PoolForceFixedCostAbs(iflt, ipt) = 0
                        Me.PoolForceFixedCostRel(iflt, ipt) = 0
                    Next
                Next
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Apply all flagged time series to the Ecosim model.
    ''' </summary>
    Friend Sub loadEnabled(Optional iTSIndex As Integer = -1)

        Dim iTS As Integer = -1
        Dim iTSEnable As Integer = -1
        Dim bFound As Boolean = False

        ' Single TS index given?
        If (iTSIndex > 0) Then
            ' Try to reload applied data for a single TS
            iTSEnable = 0
            iTS = 0

            ' Determine Applied index 
            While iTS < Math.Min(iTSIndex, Me.nTimeSeries)
                ' Try next
                iTS += 1
                ' Is an applied TS?
                If Me.TimeSeriesEnabled(iTS) Then
                    ' #Yes: count it
                    iTSEnable += 1
                    ' Check if found
                    bFound = (iTSIndex = iTS)
                End If
            End While

            If bFound Then
                ' Sanity check
                If (iTSEnable <= Me.AppliedNdatType) Then
                    Me.LoadEnabledTS(iTS, iTSEnable)
                    Return
                End If
            End If
        End If

        ' Default: reload all enabled
        Me.LoadEnabledTimeSeries()

    End Sub

    Protected Sub LoadEnabledTimeSeries()
        Dim iTS As Integer = 0
        Dim iYear As Integer = 0
        Dim iTSEnable As Integer = 0

        Me.AppliedNdatType = 0
        Me.AppliedDatPoints = Me.nMaxYears
        Me.AppliedAICTimeSeries = 0

        ' Determine no. of time series to enable
        For iTS = 1 To Me.nTimeSeries
            If Me.TimeSeriesEnabled(iTS) Then Me.AppliedNdatType += 1
        Next iTS

        Me.RedimEnabledTimeSeries()

        If Me.nTimeSeries > 0 Then

            Dim dt As Double = 1
            If Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then dt = 1 / cCore.N_MONTHS
            Me.AppliedDatYear(1) = Me.DatasetFirstYear(Me.ActiveDatasetIndex)
            For iYear = 2 To Me.AppliedDatPoints
                'DatYear(iYear) = DatYear(iYear - 1) + 1
                'Year for each datum point
                Me.AppliedDatYear(iYear) = CInt(Math.Truncate(Me.AppliedDatYear(1) + (iYear - 1) * dt))
            Next

            For iTS = 1 To Me.nTimeSeries
                If Me.TimeSeriesEnabled(iTS) Then
                    iTSEnable += 1
                    Me.LoadEnabledTS(iTS, iTSEnable)

                    'count up the number of time series use for the AIC
                    If Me.UseForAIC(Me.AppliedDatType(iTSEnable)) Then
                        Me.AppliedAICTimeSeries += 1
                    End If

                End If
            Next iTS
        End If
        '' JS24Jan24: Invoked to re-assess ForcedFs
        'Me.DoDatValCalculations()

    End Sub

    Private Sub LoadEnabledTS(iTS As Integer, iTSEnable As Integer)
        Debug.Assert(Me.TimeSeriesEnabled(iTS))

        Me.AppliedDatPool(iTSEnable) = Me.TimeSeriesPool(iTS)
        Me.AppliedDatPoolSec(iTSEnable) = Me.TimeSeriesPoolSec(iTS)
        Me.AppliedDatType(iTSEnable) = Me.TimeSeriesType(iTS)
        Me.AppliedWtType(iTSEnable) = Me.TimeSeriesWeight(iTS)
        For iYear As Integer = 0 To Me.AppliedDatPoints
            Me.AppliedDatVal(iYear, iTSEnable) = Me.TimeSeriesValues(iYear, iTS)
        Next iYear

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether a <see cref="eTimeSeriesType">timeseries type</see>
    ''' contributes to AIC calculations.
    ''' </summary>
    ''' <param name="TimeSeriesType"></param>
    ''' <returns>True if a timeseries type contributes to AIC Calculations</returns>
    ''' -----------------------------------------------------------------------
    Friend Function UseForAIC(TimeSeriesType As eTimeSeriesType) As Boolean
        '' JS 24Jan24: Should not all reference time series contribute? We now have a conventient function for that
        'Return cTimeSeries.IsReference(TimeSeriesType)
        Return (TimeSeriesType = eTimeSeriesType.BiomassAbs) Or
               (TimeSeriesType = eTimeSeriesType.BiomassRel) Or
               (TimeSeriesType = eTimeSeriesType.Catches) Or
               (TimeSeriesType = eTimeSeriesType.CatchesForcing) Or
               (TimeSeriesType = eTimeSeriesType.CatchesRel) Or
               (TimeSeriesType = eTimeSeriesType.TotalMortality)
    End Function

    Friend Sub Update()

        Dim iTS As Integer = 0
        Dim iTSenabled As Integer = 0

        For iTS = 1 To Me.nTimeSeries
            If Me.TimeSeriesEnabled(iTS) Then
                iTSenabled += 1 'DatSS and DatQ are indexed from one
                Me.TimeSeriesDatSS(iTS) = Me.AppliedDatSS(iTSenabled)
                Me.TimeSeriesDatQ(iTS) = Me.AppliedDatQ(iTSenabled)
                Me.TimeSeriesEDatQ(iTS) = Me.AppliedeDatQ(iTSenabled)
                Me.TimeSeriesSSPredErr(iTS) = Me.AppliedSSPredErr(iTSenabled)
            Else
                Me.TimeSeriesDatSS(iTS) = 0.0!
                Me.TimeSeriesDatQ(iTS) = 0.0!
                Me.TimeSeriesEDatQ(iTS) = 0.0!
                Me.TimeSeriesSSPredErr(iTS) = 0.0!
            End If
        Next iTS

    End Sub


    Public Sub LoadForcingData()
        'Forcing data is loaded from the database into the same data structures as the other time series data DatVal(ipoint,itype)
        'This allocates arrays for each forcing type PoolForceBB(group,point),PoolForceZ(group,point) and PoolForceCatch(group,point)
        'and loads the data from DatVal(ipoint,itype) into the arrays used by the core
        Try
            'redimForcingData() will expand the forcing data to cover the number of ecosim years
            'while preserving the currently loaded data
            Me.redimForcingData(Me.m_SimData.NumYears)

            Me.InitForcedDiscards()
            'Load the data from DatVal(ipoint,itype) into the core arrays PoolForceBB(group,point)...
            Me.DoDatValCalculations()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Load data from datval() into forcing arrays used by the models. Calculate the 
    ''' </summary>
    ''' <remarks>This needs to be called after the time series data is loaded to update other data arrays.</remarks>
    Public Sub DoDatValCalculations()

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'CAUTION
        'jb Ecosim.SetFFromGear() needs to be call after this 
        'this works now because SetFFromGear() gets called when ecosim is initialized after the scenario is loaded
        'if this is moved to the interface SetFFromGear() will no longer be called
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        Dim bDisFailedValidation As Boolean = False
        Dim iDatPt As Integer
        Dim iDType As Integer
        Dim K As Integer
        Dim Tim As Integer
        Dim iGrp As Integer
        Dim iFlt As Integer
        Dim HoldIobs As Integer = Me.Iobs

        Me.Iobs = 0

        'clear out the FishForced flag
        Me.m_SimData.clearFishForced()

        Me.ClearForcing()

        Try

            For iDatPt = 1 To Me.AppliedDatPoints
                For iDType = 1 To Me.AppliedNdatType

                    Dim value As Single = Me.AppliedDatVal(iDatPt, iDType)

                    Select Case Me.AppliedDatType(iDType)

                        Case eTimeSeriesType.BiomassRel
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp < nGroups) Then
                                If value > 0 Then Me.Iobs = Me.Iobs + 1
                            End If

                        Case eTimeSeriesType.FishingMortalityRef
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp < nGroups) Then
                                If value > 0 Then Me.Iobs = Me.Iobs + 1
                            End If

                        Case eTimeSeriesType.BiomassAbs
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp < nGroups) Then
                                If value > 0 Then Me.Iobs = Me.Iobs + 1
                                Me.PoolForceBB(iGrp, iDatPt) = 0
                            End If

                        Case eTimeSeriesType.BiomassForcing 'pool biomass forcing
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp < nGroups) Then
                                Me.PoolForceBB(iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.TimeForcing 'time forcing data
                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            'jb Time forcing data (Shapes) are handled through the Shape manager and not loaded with the Time series in EwE6
                            'this is the code from EwE5
                            '        If DatPool(j) > ForcingShapes + 3 Then
                            '            ForcingShapes = DatPool(j) - 3
                            'ReDim Preserve ForcingTitle(ForcingShapes) As String
                            'ReDim Preserve SeasonTitle(3) As String
                            'ReDim Preserve zscale(ForcePoints, ForcingShapes + 3) As Single
                            '            ReDim Preserve tval(ForcingShapes + 3)
                            '        End If
                            'If DatPool(j) > 3 And DatPool(j) <= ForcingShapes + 3 Then 'a valid long term shape
                            '    ForcingTitle(DatPool(j) - 3) = DatName(j)
                            '    For K = 1 To 12
                            '        Tim = 12 * (DatYear(i) - DatYear(1)) + K    ': If Tim > 1200 Then Tim = 1200
                            '        zscale(Tim, DatPool(j)) = DatVal(i, j)
                            '    Next
                            'End If
                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        Case eTimeSeriesType.FishingEffort 'effort data by gear type
                            iFlt = Me.AppliedDatPool(iDType)
                            If iFlt > 0 And iFlt <= Me.nFleets Then
                                If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
                                    For K = 1 To 12
                                        Tim = 12 * (Me.AppliedDatYear(iDatPt) - Me.AppliedDatYear(1)) + K    ': If Tim > 1200 Then Tim = 1200
                                        Me.m_SimData.FishRateGear(iFlt, Tim) = value
                                    Next
                                ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
                                    Me.m_SimData.FishRateGear(iFlt, iDatPt) = value
                                End If
                            End If

                        Case eTimeSeriesType.FishingMortality 'F by group
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp <= nGroups) Then
                                Me.m_SimData.FisForced(Me.AppliedDatPool(iDType)) = True
                                If Me.AppliedDataSetInterval = eTSDataSetInterval.Annual Then
                                    For K = 1 To 12
                                        Tim = 12 * (Me.AppliedDatYear(iDatPt) - Me.AppliedDatYear(1)) + K
                                        Me.ForcedFs(iGrp, Tim) = value
                                        Me.m_SimData.FishRateNo(iGrp, Tim) = value
                                        If Me.m_SimData.FishRateMax(iGrp) < Me.m_SimData.FishRateNo(iGrp, Tim) Then
                                            Me.m_SimData.FishRateMax(iGrp) = CSng(Me.m_SimData.FishRateNo(iGrp, Tim) * 1.01)
                                        End If
                                    Next
                                ElseIf Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
                                    Me.ForcedFs(iGrp, iDatPt) = value
                                    Me.m_SimData.FishRateNo(iGrp, iDatPt) = value
                                    If Me.m_SimData.FishRateMax(iGrp) < Me.m_SimData.FishRateNo(iGrp, iDatPt) Then
                                        Me.m_SimData.FishRateMax(iGrp) = CSng(Me.m_SimData.FishRateNo(iGrp, iDatPt) * 1.01)
                                    End If
                                End If
                            End If

                        Case eTimeSeriesType.TotalMortality, eTimeSeriesType.ConstantTotalMortality 'Z by group
                            iGrp = Me.AppliedDatPool(iDType)
                            If (iGrp > 0 And iGrp <= nGroups) Then
                                If Math.Abs(Me.AppliedDatVal(iDatPt, iDType)) > 0 Then Me.Iobs = Me.Iobs + 1 'now also with forced Z
                                If Me.AppliedDatType(iDType) = eTimeSeriesType.ConstantTotalMortality Then
                                    Me.PoolForceZ(iGrp, iDatPt) = value
                                Else
                                    Me.PoolForceZ(iGrp, iDatPt) = 0
                                End If
                            End If

                        Case eTimeSeriesType.Catches, eTimeSeriesType.CatchesForcing, eTimeSeriesType.CatchesRel  'Catches, -6 is forced

                            If Math.Abs(Me.AppliedDatVal(iDatPt, iDType)) > 0 Then Me.Iobs = Me.Iobs + 1 '....Added by SM for Catch Fitting.
                            If Me.AppliedDatType(iDType) = eTimeSeriesType.CatchesForcing Then
                                Me.PoolForceCatch(Me.AppliedDatPool(iDType), iDatPt) = value
                            Else
                                Me.PoolForceCatch(Me.AppliedDatPool(iDType), iDatPt) = 0
                            End If

                            'Martell playing here!
                        Case eTimeSeriesType.AverageWeight 'Mean Body Weight data for split pool groups
                            'jb EwE6 does not have split pools! I'm not sure if this also applies to multi stanza groups??
                            If Me.AppliedDatVal(iDatPt, iDType) > 0 Then Me.Iobs = Me.Iobs + 1

                        Case eTimeSeriesType.DiscardMortality
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            ' JS 27Sep23: Group 0 allowed for this TS type
                            If (iGrp >= 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                If value > 1.0 Then
                                    value = 1.0
                                    bDisFailedValidation = True
                                End If
                                Me.PoolForceDiscardMort(iFlt, iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.DiscardProportion
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            ' JS 27Sep23: Group 0 allowed for this TS type
                            If (iGrp >= 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                If value > 1.0 Then
                                    value = 1.0
                                    bDisFailedValidation = True
                                End If
                                Me.PoolForceDiscardProp(iFlt, iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.Discards, eTimeSeriesType.Landings
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            If (iGrp > 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                Me.Iobs = Me.Iobs + 1
                            End If

                        Case eTimeSeriesType.DiscardsTotalAbs, eTimeSeriesType.DiscardsTotalRel
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            If (iGrp > 0 And iGrp <= nGroups) Then
                                Me.Iobs = Me.Iobs + 1
                            End If

                        Case eTimeSeriesType.Catchabilities
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            If (iGrp > 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                Me.PoolForceCatchabilities(iFlt, iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.OffVesselPrice
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            If (iGrp > 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                Me.PoolForceOffVesselPriceAbs(iFlt, iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.OffVesselPriceRel
                            iFlt = Me.AppliedDatPool(iDType)
                            iGrp = Me.AppliedDatPoolSec(iDType)
                            ' JS 01Oct23: Group 0 allowed for this TS type to vary all off-vessel prices by the same order
                            If (iGrp >= 0 And iFlt > 0 And iGrp <= nGroups And iFlt <= nFleets) Then
                                Me.PoolForceOffVesselPriceRel(iFlt, iGrp, iDatPt) = value
                            End If

                        Case eTimeSeriesType.EffortCost
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceEffortCostAbs(iFlt, iDatPt) = value
                            End If

                        Case eTimeSeriesType.EffortCostRel
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceEffortCostRel(iFlt, iDatPt) = value
                            End If

                        Case eTimeSeriesType.SailCost
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceSailCostAbs(iFlt, iDatPt) = value
                            End If

                        Case eTimeSeriesType.SailCostRel
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceSailCostRel(iFlt, iDatPt) = value
                            End If

                        Case eTimeSeriesType.FixedCost
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceFixedCostAbs(iFlt, iDatPt) = value
                            End If

                        Case eTimeSeriesType.FixedCostRel
                            iFlt = Me.AppliedDatPool(iDType)
                            If (iFlt > 0 And iFlt <= nFleets) Then
                                Me.PoolForceFixedCostRel(iFlt, iDatPt) = value
                            End If

                    End Select
                    '      End If 'If IsDatShown(j) = True Then
                Next
            Next
            iDType = 0
            For iDatPt = 1 To Me.AppliedNdatType
                If Me.AppliedWtType(iDatPt) > 0 Then iDType = iDType + 1
            Next

            'jb was????? 
            ' If ReadingCsvFile Or j = 0 Then
            If iDType = 0 Then
                For iDatPt = 1 To Me.AppliedNdatType
                    If Me.AppliedWtType(iDatPt) = 0 And (Me.AppliedDatType(iDatPt) = 0 Or Me.AppliedDatType(iDatPt) = 1 Or Me.AppliedDatType(iDatPt) = 5 Or
                                               Math.Abs(Me.AppliedDatType(iDatPt)) = 6 Or Me.AppliedDatType(iDatPt) = 7) Then Me.AppliedWtType(iDatPt) = 1
                Next
            End If

            If Me.Iobs = 0 Then Me.Iobs = HoldIobs

            If bDisFailedValidation Then
                cLog.Write("Time series Discard Mortality Rate or Discard Proportion contained values > 1.0. These values cap a 1.0")
            End If

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'CAUTION
            'jb Ecosim.SetFFromGear() needs to be called after this 
            'this works now because SetFFromGear() gets called when ecosim is initialized after the scenario is loaded
            'if this is moved to the interface SetFFromGear() will no longer be called
            'EwE5 reset fishing rates by group to values predicted from effort except for forced groups
            ' SetFFromGear()
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".DoDatValCalculations(). ", ex)
        End Try

    End Sub

    ''' <summary>
    ''' Is there a timeseries of a data type loaded for a group/fleet
    ''' </summary>
    ''' <param name="TSDataType">Type of data to test for</param>
    ''' <param name="iGroupIndex">Index of Group or Fleet</param>
    ''' <returns>True if there is data loaded for this datatype, group</returns>
    Friend Function DataLoadedForTypeGroup(TSDataType As eTimeSeriesType, iGroupIndex As Integer) As Boolean

        Try
            For its As Integer = 1 To Me.AppliedNdatType
                If Me.AppliedDatType(its) = TSDataType Then
                    If Me.AppliedDatPool(its) = iGroupIndex Then
                        Return True
                    End If
                End If
            Next
            Return False
        Catch ex As Exception
            cLog.Write(ex)
        End Try

        Return False

    End Function

    Private Sub InitForcedDiscards()

        Dim nSimPoints As Integer
        If Me.AppliedDataSetInterval = eTSDataSetInterval.TimeStep Then
            nSimPoints = Math.Max(Me.AppliedDatPoints, Me.m_SimData.NumYears * cCore.N_MONTHS)
        Else
            nSimPoints = Math.Max(Me.AppliedDatPoints, Me.m_SimData.NumYears)
        End If

        'jb 27-Oct-2016 I'm not sure about this 
        'set all points past the reference data to the default Ecopath values!
        For iflt As Integer = 0 To Me.nFleets
            For igrp As Integer = 0 To Me.nGroups

                For ipt As Integer = 0 To nSimPoints
                    Me.PoolForceDiscardMort(iflt, igrp, ipt) = cCore.NULL_VALUE
                    Me.PoolForceDiscardProp(iflt, igrp, ipt) = cCore.NULL_VALUE
                Next

            Next
        Next

    End Sub

End Class

#Region "Obsolete "

#If 0 Then


'jb 12-July-2016 Removed the cEcospaceTimeSeriesDataStructures with the implementation of Ecosim biomass forcing time series in Ecospace
'Just use the Cores cTimeSeriesDataStructures object until we need something more advanced  
''' <summary>
''' Time series reference data for Ecospace
''' </summary>
''' <remarks></remarks>
Public Class cEcospaceTimeSeriesDataStructures
    Inherits cTimeSeriesDataStructures

    ' ------------------------------------------------
    ' Interface structures
    ' ------------------------------------------------
    Public iSPRegion() As Integer

    ' ------------------------------------------------
    ' Applied structures used by the models
    ' ------------------------------------------------
    Public SPRegion() As Integer


    Friend Overloads Sub RedimTimeSeries()
        MyBase.RedimTimeSeries()

        ReDim iSPRegion(nTimeSeries)

    End Sub


    Friend Overloads Sub RedimAppliedTimeSeries()
        MyBase.RedimEnabledTimeSeries()

        ReDim iSPRegion(NdatType)

    End Sub

    ''' <summary>
    ''' EwE5 DoSpaceDatValCalculations
    ''' </summary>
    ''' <remarks></remarks>
    Friend Overloads Sub DoDatValCalculations(ByRef EcospaceData As cEcospaceDataStructures)

    End Sub

    ''' <summary>
    ''' Enable all flagged time series to the Ecosim model.
    ''' </summary>
    Friend Overloads Sub Apply(ByRef EcospaceData As cEcospaceDataStructures)

        'load the the user selected data into the data used by the model
        MyBase.LoadEnabledTimeSeries()

        'load the data from datval() into the ecosim data 
        DoDatValCalculations(EcospaceData)

    End Sub

End Class

#End If

#End Region
