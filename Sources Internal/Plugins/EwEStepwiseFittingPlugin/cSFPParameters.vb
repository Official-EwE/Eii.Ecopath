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

    Private TimeSeries As cTimeSeriesDataset
    Private m_core As cCore

    Private MaxK As Integer
    'MinK set to 1
    Private MinK As Integer
    Private MaxSplinePoints As Integer
    'MinSplinePoints set to 2 as 0 causes overestimates and need more spline points than 1
    Private MinSplinePoints As Integer = 2
    Private NumberOfObservations As Integer

    Private AppliedShape As cForcingFunction = Nothing

    Private AbsoluteBiomassEnabled As Boolean = False

    Public Sub New(ByVal c As EwECore.cCore)
        Me.m_core = c
        Console.WriteLine("New SFPParameters instance created")
    End Sub

    ''' <summary>
    ''' Calculate the values of estimated parameters from time series dataset and find applied shape
    ''' </summary>
    Public Function CalculateParameters() As Boolean

        If (Me.m_core.ActiveTimeSeriesDatasetIndex < 1) Then
            Me.TimeSeries = Nothing
            Console.WriteLine("SFPParameters no longer has a reference to SFPManager time series")
        Else
            Me.TimeSeries = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
            Console.WriteLine("SFPParameters now has reference to SFPManager time series")
        End If

        CalculateMaxKandMinK()
        CalculateMaxSplinePoints()
        CalculateNumberOfObservations()
        FFAppliedToPP()

        Console.WriteLine("SFPParameters calculated estimated parameters")

        Return True

    End Function

    ''' <summary>
    ''' Calculate the values of Max and Min K from time series dataset of time series of type 0,1,5,6 and 7 
    ''' </summary>
    Private Sub CalculateMaxKandMinK()

        MaxK = 0
        MinK = 1

        ' Make fail-safe
        If (Me.TimeSeries Is Nothing) Then Return

        Dim ts As cTimeSeries
        Dim tsType As eTimeSeriesType
        ' Dim tsname As String
        Dim count As Integer = 0

        For i As Integer = 1 To TimeSeries.nTimeSeries
            ts = TimeSeries.TimeSeries(i)
            tsType = ts.TimeSeriesType

            Select Case tsType
                Case eTimeSeriesType.BiomassRel, _
                     eTimeSeriesType.TotalMortality, _
                     eTimeSeriesType.Catches, _
                     eTimeSeriesType.AverageWeight
                    count += 1

                Case eTimeSeriesType.BiomassAbs
                    If AbsoluteBiomassEnabled Then
                        count += 1
                    End If

            End Select
        Next

        MaxK = count - 1
        MinK = count - (count - 1)
        Console.WriteLine("Number of max k Parameters to estimate: " & MaxK.ToString)
        Console.WriteLine("Number of min k Parameters to estimate: " & MinK.ToString)
    End Sub


    ''' <summary>
    ''' Calculate the values of Max Spline Points from time series dataset
    ''' </summary>
    Private Sub CalculateMaxSplinePoints()

        MaxSplinePoints = 0

        ' Make fail-safe
        If (Me.TimeSeries Is Nothing) Then Return

        Dim years As Integer
        years = TimeSeries.NumPoints - 1
        Console.WriteLine("Number of years in time series: " & years.ToString)
        If MaxK > years Then
            MaxSplinePoints = years
        Else
            MaxSplinePoints = MaxK
        End If
        Console.WriteLine("Number of Max spline points: " & MaxSplinePoints.ToString)
        Console.WriteLine("Number of Min spline points: " & MinSplinePoints.ToString)
    End Sub

    ''' <summary>
    ''' Calculate the number of observations/data points from time series dataset that are within a time series of type 0,1,5,6 and 7 
    ''' </summary>
    Private Sub CalculateNumberOfObservations()

        Me.NumberOfObservations = 0
        ' Dim Num As Integer = 0

        ' Make fail-safe
        If (Me.TimeSeries Is Nothing) Then Return

        Dim ts As cTimeSeries
        Dim tsType As eTimeSeriesType
        'Go through each time series of the time series dataset
        For i As Integer = 1 To TimeSeries.nTimeSeries
            'Get a time series
            ts = TimeSeries.TimeSeries(i)
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
                    If AbsoluteBiomassEnabled And ts.WtType > 0 Then
                        AddToObservations(ts)
                        'Num += TimeSeries.NumPoints
                    End If
            End Select

        Next
        'Console.WriteLine("Num: " & Num.ToString)
        Console.WriteLine("Total Number of Observations: " & NumberOfObservations.ToString)
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
        NumberOfObservations += count
        Console.WriteLine("Number of Observations from : " & ts.Name & " = " & count.ToString)
    End Sub

    ''' <summary>
    ''' Find current FF applied to PP and store in AppliedShape
    ''' </summary>
    Private Sub FFAppliedToPP()

        Dim interactions As cMediatedInteractionManager = m_core.MediatedInteractionManager
        For Each shape As cForcingFunction In m_core.ForcingShapeManager
            If interactions.IsApplied(shape) Then
                AppliedShape = shape
                Console.WriteLine("Applied PP Anomaly Name : " & shape.Name)
                Exit For
            End If
        Next

    End Sub

    Public ReadOnly Property MaxKValue As Integer
        Get
            Return Me.MaxK
        End Get
    End Property

    Public ReadOnly Property MinKValue As Integer
        Get
            Return Me.MinK
        End Get
    End Property

    Public ReadOnly Property MaxSplinePointsValue As Integer
        Get
            Return Me.MaxSplinePoints
        End Get
    End Property

    Public ReadOnly Property MinSplinePointsValue As Integer
        Get
            Return Me.MinSplinePoints
        End Get
    End Property

    Public ReadOnly Property NumberOfObservationsValue As Integer
        Get
            Return Me.NumberOfObservations
        End Get
    End Property

    Public Function GetAppliedShape() As cForcingFunction
        Return AppliedShape
    End Function

    Public Property EnableAbsoluteBiomass As Boolean
        Get
            Return AbsoluteBiomassEnabled
        End Get
        Set(ByVal value As Boolean)
            AbsoluteBiomassEnabled = value
        End Set
    End Property


#Region " Persistent configuration "

    Public Property PredOrPredPreySSToV As Boolean
        Get
            Return My.Settings.PredOrPredPreySSToV
        End Get
        Set(ByVal value As Boolean)
            My.Settings.PredOrPredPreySSToV = value
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
