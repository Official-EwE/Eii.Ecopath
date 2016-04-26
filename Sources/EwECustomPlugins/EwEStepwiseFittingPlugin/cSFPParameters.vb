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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' SFPParameters calculates and stores estimated parameter values. Also stores applied shape of FF to PP
''' </summary>
Public Class cSFPParameters

    Public Enum eAutosaveMode As Integer
        None = 0
        Ecosim
        Aggregated
        All
    End Enum

#Region " Private vars "

    Private m_ts As cTimeSeriesDataset = Nothing
    Private m_core As cCore

    Private m_iCurrentK As Integer
    Private m_iMinK As Integer
    Private m_iMaxK As Integer

    Private m_iMaxSplinePoints As Integer

    'MinSplinePoints set to 2 as 0 causes overestimates and need more spline points than 1
    Private m_iMinSplinePoints As Integer = 2
    Private m_iObservations As Integer

#End Region ' Private vars

    Public Sub New(ByVal c As EwECore.cCore)
        Me.m_core = c
    End Sub

    ''' <summary>
    ''' Calculate the values of estimated parameters from time series dataset and find applied shape
    ''' </summary>
    Public Function CalculateParameters() As Boolean

        If (Me.m_core.ActiveTimeSeriesDatasetIndex < 1) Then
            Me.m_ts = Nothing
            'Console.WriteLine("SFPParameters no longer has a reference to SFPManager time series")
        Else
            Me.m_ts = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
            'Console.WriteLine("SFPParameters now has reference to SFPManager time series")
        End If

        CalculateRangeOfK()
        CalculateMaxSplinePoints()
        CalculateNumberOfObservations()

        'Console.WriteLine("SFPParameters calculated estimated parameters")

        Return True

    End Function

    ''' <summary>
    ''' Calculate the values of Max and Min K from time series dataset of time series of type 0,1,5,6 and 7 
    ''' </summary>
    Private Sub CalculateRangeOfK()

        m_iMinK = 1
        m_iCurrentK = 0

        ' Make fail-safe
        If (Me.m_ts Is Nothing) Then Return

        Dim ts As cTimeSeries
        Dim tsType As eTimeSeriesType
        ' Dim tsname As String
        Dim count As Integer = 0

        For i As Integer = 1 To m_ts.nTimeSeries
            ts = m_ts.TimeSeries(i)
            tsType = ts.TimeSeriesType

            Select Case tsType
                Case eTimeSeriesType.BiomassRel, _
                     eTimeSeriesType.TotalMortality, _
                     eTimeSeriesType.Catches, _
                     eTimeSeriesType.AverageWeight
                    count += 1

                Case eTimeSeriesType.BiomassAbs
                    If EnableAbsoluteBiomass Then
                        count += 1
                    End If

            End Select
        Next

        Me.m_iMinK = count - (count - 1)
        Me.m_iCurrentK = Math.Max(Me.m_iMinK, count - 1)
        ' ToDo: #vars - 1
        Me.m_iMaxK = Me.m_iCurrentK * 10

    End Sub


    ''' <summary>
    ''' Calculate the values of Max Spline Points from time series dataset
    ''' </summary>
    Private Sub CalculateMaxSplinePoints()

        m_iMaxSplinePoints = 0

        ' Make fail-safe
        If (Me.m_ts Is Nothing) Then Return

        Dim years As Integer
        years = m_ts.NumPoints - 1
        Console.WriteLine("Number of years in time series: " & years.ToString)
        If m_iCurrentK > years Then
            m_iMaxSplinePoints = years
        Else
            m_iMaxSplinePoints = m_iCurrentK
        End If
        'Console.WriteLine("Number of Max spline points: " & m_iMaxSplinePoints.ToString)
        'Console.WriteLine("Number of Min spline points: " & m_iMinSplinePoints.ToString)
    End Sub

    ''' <summary>
    ''' Calculate the number of observations/data points from time series dataset that are within a time series of type 0,1,5,6 and 7 
    ''' </summary>
    Private Sub CalculateNumberOfObservations()

        Me.m_iObservations = 0
        ' Dim Num As Integer = 0

        ' Make fail-safe
        If (Me.m_ts Is Nothing) Then Return

        Dim ts As cTimeSeries
        Dim tsType As eTimeSeriesType
        'Go through each time series of the time series dataset
        For i As Integer = 1 To m_ts.nTimeSeries
            'Get a time series
            ts = m_ts.TimeSeries(i)
            'Get the time series type
            tsType = ts.TimeSeriesType
            'If the time series type is 0,1,5,6 or 7 add its datapoints to the total number of observations
            Select Case tsType
                Case eTimeSeriesType.BiomassRel, _
                    eTimeSeriesType.TotalMortality, _
                    eTimeSeriesType.Catches, _
                    eTimeSeriesType.AverageWeight
                    'If the weight type is not 0 add datapoints of time series to the total number of observations
                    If ts.WtType > 0 Then
                        AddToObservations(ts)
                        'Num += TimeSeries.NumPoints
                    End If
                Case eTimeSeriesType.BiomassAbs
                    If EnableAbsoluteBiomass And ts.WtType > 0 Then
                        AddToObservations(ts)
                        'Num += TimeSeries.NumPoints
                    End If
            End Select

        Next
        'Console.WriteLine("Num: " & Num.ToString)
        'Console.WriteLine("Total Number of Observations: " & m_iObservations.ToString)
    End Sub

    ''' <summary>
    ''' Calculate the number of observations/data points within a time series
    ''' </summary>
    Private Sub AddToObservations(ByVal ts As cTimeSeries)
        Dim tsdatapoints As Single()
        Dim count As Integer
        'Go through each data point of the time series
        For j As Integer = 1 To ts.nPoints
            'Get copy of datapoints
            tsdatapoints = ts.ShapeData
            'If the datapoint is not zero add to count
            If tsdatapoints(j) <> 0 Then
                count += 1
            End If
        Next
        'Add number of data points from this time seires to the total number of observations
        m_iObservations += count
        'Console.WriteLine("Number of Observations from : " & ts.Name & " = " & count.ToString)
    End Sub

    Public Property K As Integer
        Get
            Return Me.m_iCurrentK
        End Get
        Set(value As Integer)
            Me.m_iCurrentK = value
        End Set
    End Property

    Public ReadOnly Property MinK As Integer
        Get
            Return Me.m_iMinK
        End Get
    End Property

    Public ReadOnly Property MaxK As Integer
        Get
            Return Me.m_iMaxK
        End Get
    End Property

    Public ReadOnly Property MaxSplinePoints As Integer
        Get
            Return Me.m_iMaxSplinePoints
        End Get
    End Property

    Public ReadOnly Property MinSplinePoints As Integer
        Get
            Return Me.m_iMinSplinePoints
        End Get
    End Property

    Public ReadOnly Property NumberOfObservations As Integer
        Get
            Return Me.m_iObservations
        End Get
    End Property

    ''' <summary>
    ''' Get/set the selected anomaly shape
    ''' </summary>
    Public Property AppliedShape() As cShapeData

    Public Property EnableAbsoluteBiomass As Boolean = False

#Region " Persistent configuration "

    Public Property SStoVSearchMode As EwECore.FitToTimeSeries.eRunType
        Get
            Return CType(My.Settings.SearchSStoVMode, FitToTimeSeries.eRunType)
        End Get
        Set(ByVal value As EwECore.FitToTimeSeries.eRunType)
            My.Settings.SearchSStoVMode = value
        End Set
    End Property

    Public Property AnomalySearchSplineStepSize As Integer
        Get
            Return My.Settings.AnomalySearchSplineStepSize
        End Get
        Set(ByVal value As Integer)
            My.Settings.AnomalySearchSplineStepSize = value
        End Set
    End Property

    Public Property CustomOutputFolder() As String
        Get
            Return My.Settings.CustomOutputPath
        End Get
        Set(ByVal value As String)
            My.Settings.CustomOutputPath = value
        End Set
    End Property

    Public Property AutosaveMode As eAutosaveMode
        Get
            Return CType(My.Settings.AutoSaveMode, eAutosaveMode)
        End Get
        Set(ByVal value As eAutosaveMode)
            My.Settings.AutoSaveMode = value
        End Set
    End Property

#End Region ' Persistent configuration

End Class
