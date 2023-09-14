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
'    Scottish Association for Marine Science, Oban, Scotland
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
Imports EwEUtils.SystemUtilities

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

    Private m_iK As Integer
    Private m_iCorrectK As Integer
    Private m_iMinK As Integer
    Private m_iMaxK As Integer

    'MinSplinePoints set to 2 as 0 causes overestimates and need more spline points than 1
    Private m_iMinSplinePoints As Integer = 2
    Private m_iMaxSplinePoints As Integer = 0
    Private m_iObservations As Integer = 0

#End Region ' Private vars

#Region " Construction "

    Public Sub New(c As cCore)
        Me.Core = c
        Me.NumThreads = cSystemUtils.ProcessorCount
    End Sub

#End Region ' Construction

#Region " Public bits "

    Public ReadOnly Property Core As cCore

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the values of estimated parameters from time series dataset and find applied shape
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CalculateParameters(ByVal iPrefK As Integer) As Boolean

        ' Sanity check
        Debug.Assert(Me.TimeSeriesDataset = Me.Core.ActiveTimeSeriesDatasetIndex)

        Me.CalculateOptimalK(iPrefK)
        Me.CalculateMaxSplinePoints()
        Me.UpdateNumberOfObservations()
        Me.CalculateMaxK()

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of K to use. This value falls between <see cref="MinK"/>
    ''' and <see cref="MaxK"/>, and should not really exceed <see cref="CorrectK"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property K As Integer
        Get
            Return Math.Min(Math.Max(Me.MinK, Me.m_iK), Me.MaxK)
        End Get
        Set(value As Integer)
            Me.m_iK = Math.Min(Math.Max(Me.MinK, value), Me.MaxK)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the lower limit for <see cref="K"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MinK As Integer
        Get
            Return Me.m_iMinK
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the upper limit for <see cref="K"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxK As Integer
        Get
            Return Me.m_iMaxK
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the correct value for <see cref="K"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property CorrectK As Integer
        Get
            Return Me.m_iCorrectK
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the selected anomaly shape
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property AnomalyShapeIndex As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the selected anomaly shape
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property VulCap As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether absolute biomass timeseries should be included here.
    ''' </summary>
    ''' <seealso cref="HasAbsoluteBiomassTimeSeries"/>
    ''' -----------------------------------------------------------------------
    Public Property EnableAbsoluteBiomassTimeSeries As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns if absolute biomass timeseries are available.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasAbsoluteBiomassTimeSeries() As Boolean
        For Each ts As cTimeSeries In Me.GetRelevantTimeSeries()
            If ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then Return True
        Next
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of threads to use. Should be between 1 and <see cref="MaxThreads"/>,
    ''' but this policy is not enforced.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumThreads As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the max number of parallel threads to run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxThreads As Integer
        Get
            Return cSystemUtils.ProcessorCount * 2
        End Get
    End Property

    Public Property ModelFileName As String = ""
    Public Property EcosimScenario As Integer = 0
    Public Property TimeSeriesDataset As Integer = 0

#End Region ' Public bits

#Region " Run preparation and offloading "

    Private m_weights(1000) As Single
    Private m_enabled(1000) As Boolean

    Public Sub PrepareForRun(outputfolder As String)

        Me.IterationOutputFolder = outputfolder

        Array.Clear(Me.m_weights, 0, Me.m_weights.Length)

        If (Me.TimeSeriesDataset <= 0) Then Return
        Dim dataset As cTimeSeriesDataset = Me.Core.TimeSeriesDataset(Me.TimeSeriesDataset)

        'Go through each time series of the time series dataset
        For i As Integer = 1 To dataset.nTimeSeries
            With dataset.TimeSeries(i)
                Me.m_weights(i) = .WtType
                Me.m_enabled(i) = .Enabled
            End With
        Next

        ' Make sure this transfers over from core to core, even if the source core has not been saved yet.
        Me.VulCap = Me.Core.EcosimModelParameters.VulnerabilityCap

    End Sub

    Public ReadOnly Property TimeSeriesWeight(its As Integer) As Single
        Get
            If (its < 0 Or its >= Me.m_weights.Length) Then Return 0
            Return Me.m_weights(its)
        End Get
    End Property

    Public ReadOnly Property TimeSeriesEnabled(its As Integer) As Boolean
        Get
            If (its < 0 Or its >= Me.m_enabled.Length) Then Return True
            Return Me.m_enabled(its)
        End Get
    End Property

    Public Property IterationOutputFolder As String = ""
    Public Property SaveHeaders As Boolean = False

#End Region ' Run preparation and offloading

#Region " Persistent configuration "

    Public Property VulSearchMode As ISFPIteration.eVulSearchMode
        Get
            Return CType(My.Settings.VulSearchMode, ISFPIteration.eVulSearchMode)
        End Get
        Set(ByVal value As ISFPIteration.eVulSearchMode)
            My.Settings.VulSearchMode = value
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

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, digs up all relevant, enabled and non-zero weighted 
    ''' reference time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetRelevantTimeSeries() As cTimeSeries()

        Dim lTS As New List(Of cTimeSeries)
        lTS.AddRange(Me.Core.EcosimGroupTimeseries)
        lTS.AddRange(Me.Core.EcosimFleetTimeseries)

        Dim l As New List(Of cTimeSeries)
        For Each ts As cTimeSeries In lTS
            If (ts.Enabled And ts.WtType > 0) Then
                If ts.IsReference Then
                    If ts.TimeSeriesType = eTimeSeriesType.BiomassAbs And Me.EnableAbsoluteBiomassTimeSeries Then
                        l.Add(ts)
                    Else
                        l.Add(ts)
                    End If
                End If

            End If
        Next
        Return l.ToArray()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the values of Min K and K from the <see cref="GetRelevantTimeSeries()">
    ''' relevant time series</see>.
    ''' </summary>
    ''' <param name="iPrefK">The <see cref="K"/> to maintain.</param>
    ''' -----------------------------------------------------------------------
    Private Sub CalculateOptimalK(ByVal iPrefK As Integer)

        Me.m_iK = 0
        Me.m_iMinK = 1

        Dim count As Integer = Me.GetRelevantTimeSeries().Length
        Me.m_iCorrectK = Math.Max(0, count - 1)

        If (iPrefK <= 0) Then
            Me.m_iK = count - 1
        Else
            Me.m_iK = iPrefK
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the values of Max Spline Points from time series dataset
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CalculateMaxSplinePoints()

        Me.m_iMaxSplinePoints = 0

        If (Me.TimeSeriesDataset <= 0) Then Return
        Dim dataset As cTimeSeriesDataset = Me.Core.TimeSeriesDataset(Me.TimeSeriesDataset)
        If (dataset Is Nothing) Then Return

        Dim years As Integer = dataset.NumPoints - 1
        Me.m_iMaxSplinePoints = Math.Min(Me.m_iK, years)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the <see cref="NumberOfObservations">number of observations/data points</see> 
    ''' across all <see cref="GetRelevantTimeSeries()">relevant time series</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateNumberOfObservations()

        Me.m_iObservations = 0
        For Each ts As cTimeSeries In Me.GetRelevantTimeSeries()
            Me.m_iObservations += Me.CountNoOfObservations(ts)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the value of Max K from the number of observations. This is
    ''' funky but allows for experimenting below the 'correct' value of K
    ''' </summary>
    ''' <seealso cref="CorrectK"/>
    ''' <seealso cref="K"/>
    ''' <seealso cref="MinK"/>
    ''' -----------------------------------------------------------------------
    Private Sub CalculateMaxK()
        Me.m_iMaxK = Me.m_iObservations - 1
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of observations/data points within a time series
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function CountNoOfObservations(ByVal ts As cTimeSeries) As Integer

        ' Sanity check
        Debug.Assert(ts.Enabled And ts.WtType > 0)

        Dim tsdatapoints As Single()
        Dim count As Integer

        'Go through each data point of the time series
        For j As Integer = 1 To ts.nPoints
            'Get copy of datapoints
            tsdatapoints = ts.ShapeData
            'If the datapoint is not zero add to count
            If (tsdatapoints(j) <> 0) And (tsdatapoints(j) <> cCore.NULL_VALUE) Then
                count += 1
            End If
        Next

        Return count

    End Function

#End Region ' Internals

End Class
