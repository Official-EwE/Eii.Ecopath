#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' The one access point in EwE to create <see cref="cTimeSeries">cTimeSeries</see>
''' -derived objects, and to translate between time series <see cref="eTimeSeriesType">types</see>
''' and <see cref="eTimeSeriesCategoryType">categories</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTimeSeriesFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Determine the <see cref="eTimeSeriesCategoryType">Time series category</see>
    ''' based on a <see cref="eTimeSeriesType">Time series type</see>. For instance,
    ''' time series types <see cref="eTimeSeriesType.Catches">eTimeSeriesType.Catches</see>
    ''' and <see cref="eTimeSeriesType.CatchesForcing">eTimeSeriesType.CatchesForcing</see>
    ''' are <see cref="eTimeSeriesCategoryType.Fleet">Fleet</see>-related time series.
    ''' </summary>
    ''' <param name="timeSeriesType"></param>
    ''' <remarks>
    ''' This method was added to centralize interpretation of the awkward enumerator 
    ''' <see cref="eTimeSeriesType">eTimeSeriesType</see>.
    ''' </remarks>
    ''' <returns>
    ''' A time series category for the provided time series type.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function TimeSeriesCategory(ByVal timeSeriesType As eTimeSeriesType) As eTimeSeriesCategoryType

        Select Case timeSeriesType

            Case eTimeSeriesType.NotSet
                Return eTimeSeriesCategoryType.NotSet

            Case eTimeSeriesType.TimeForcing
                Return eTimeSeriesCategoryType.Forcing

            Case eTimeSeriesType.FishingEffort
                Return eTimeSeriesCategoryType.Fleet

            Case Else
                Return eTimeSeriesCategoryType.Group

        End Select

        ' Add this for good manners.
        Return eTimeSeriesCategoryType.NotSet
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory method; the only location in EwE where actual <see cref="cTimeSeries">cTimeSeries-derived</see>
    ''' objects are created.
    ''' </summary>
    ''' <param name="timeSeriesType">The <see cref="eTimeSeriesType">type</see> of
    ''' the time series.</param>
    ''' <returns>A Time Series instance, or nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function CreateTimeSeries(ByVal timeSeriesType As eTimeSeriesType, _
            ByVal core As cCore, ByVal iDBID As Integer) As cTimeSeries

        Dim ts As cTimeSeries = Nothing

        Select Case TimeSeriesCategory(timeSeriesType)

            Case eTimeSeriesCategoryType.Forcing
                ts = Nothing ' No can do

            Case eTimeSeriesCategoryType.Fleet
                ts = New cFleetTimeSeries(core, iDBID)

            Case eTimeSeriesCategoryType.Group
                ts = New cGroupTimeSeries(core, iDBID)

            Case eTimeSeriesCategoryType.NotSet
                Debug.Assert(False, String.Format("Unknown category of time series for type {0}", timeSeriesType))

        End Select

        Return ts
    End Function

End Class