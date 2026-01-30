' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="iNumYears">Number of years in this time series.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of this time series.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(iNumYears As Integer, timeSeriesType As eTimeSeriesType)
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

    Public Overrides Function IsValid() As Boolean
        ' Of course
        Return True
    End Function

End Class
