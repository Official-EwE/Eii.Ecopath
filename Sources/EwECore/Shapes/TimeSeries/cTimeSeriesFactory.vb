' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







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
    ''' Factory method; the only location in EwE where actual <see cref="cTimeSeries">cTimeSeries-derived</see>
    ''' objects are created.
    ''' </summary>
    ''' <param name="timeSeriesType">The <see cref="eTimeSeriesType">type</see> of
    ''' the time series.</param>
    ''' <returns>A Time Series instance, or nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function CreateTimeSeries(timeSeriesType As eTimeSeriesType,
            core As cCore, iDBID As Integer) As cTimeSeries

        Dim ts As cTimeSeries = Nothing

        Select Case cTimeSeries.Category(timeSeriesType)

            Case eTimeSeriesCategoryType.Forcing
                ts = Nothing ' No can do

            Case eTimeSeriesCategoryType.Fleet,
                 eTimeSeriesCategoryType.FleetGroup
                ts = New cFleetTimeSeries(core, iDBID) With {
                    .TimeSeriesType = timeSeriesType
                    }

            Case eTimeSeriesCategoryType.Group
                ts = New cGroupTimeSeries(core, iDBID) With {
                    .TimeSeriesType = timeSeriesType
                    }

            Case eTimeSeriesCategoryType.NotSet
                Debug.Assert(False, String.Format("Unknown category of time series for type {0}", timeSeriesType))

        End Select

        Return ts
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all <see cref="eTimeSeriesType"/> of the same <see cref="eTimeSeriesCategoryType">category</see>
    ''' as the provided <paramref name="type"/>.
    ''' </summary>
    ''' <param name="type">The <see cref="eTimeSeriesType">type</see> to find others for.</param>
    ''' <returns>Well...</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function CompatibleTypes(type As eTimeSeriesType) As eTimeSeriesType()
        Return CompatibleTypes(cTimeSeries.Category(type))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all <see cref="eTimeSeriesType"/> within a give <see cref="eTimeSeriesCategoryType">category</see>.
    ''' </summary>
    ''' <param name="cat">The <see cref="eTimeSeriesCategoryType">category</see> to find others for.</param>
    ''' <returns>Well...</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function CompatibleTypes(cat As eTimeSeriesCategoryType) As eTimeSeriesType()
        Dim lTypes As New List(Of eTimeSeriesType)
        For Each t As eTimeSeriesType In [Enum].GetValues(GetType(eTimeSeriesType))
            If (cTimeSeries.Category(t) = cat) Or (cat = eTimeSeriesCategoryType.NotSet) Then
                lTypes.Add(t)
            End If
        Next
        Return lTypes.ToArray()
    End Function

End Class