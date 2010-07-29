#Region " Imports "

Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' TimeSeries import class
''' </summary>
''' <remarks>
''' This reminds me so much about programming COBOL that I'm downright terrified...
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesImport
    Inherits cTimeSeries

    Private m_bIsMonthly As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="iNumYears">Number of years in this time series.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of this time series.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal iNumYears As Integer, ByVal timeSeriesType As eTimeSeriesType)
        MyBase.New(Nothing, -1)
        Me.m_timeSeriesType = timeSeriesType
        Me.ResizeData(iNumYears)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to prevent this type of time series to interact with the 
    ''' EwE core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Update() As Boolean
        ' Suppress this
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a time series should be imported as monthly (true) or 
    ''' annual (false) data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property IsMonthly() As Boolean
        Get
            Return Me.m_bIsMonthly
        End Get
        Set(ByVal value As Boolean)
            Me.m_bIsMonthly = value
        End Set
    End Property

End Class
