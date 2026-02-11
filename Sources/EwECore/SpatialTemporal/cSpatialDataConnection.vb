' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwEUtils.Utilities

Namespace SpatialData

    ' TODO: inherit from cCoreInputOutputBase to fire off change notifications
    ' Add variables
    ' Variable statuses: scale may be read-only

    Public Class cSpatialDataConnection

        ''' <summary></summary>
        Public Property Dataset As ISpatialDataSet = Nothing

        ''' <summary></summary>
        Public Property Converter As ISpatialDataConverter = Nothing

        ''' <summary></summary>
        Public Property Scale As Single = 1

        ''' <summary></summary>
        Public Property ScaleType As eScaleType = eScaleType.Relative

        ''' <summary></summary>
        Public ReadOnly Property UseDefaultDateStart As Boolean
            Get
                Return cDateUtils.DateEquals(Me.CustomDateStart, Date.MaxValue)
            End Get
        End Property

        ''' <summary>
        ''' Custom start date for bringing in external data.
        ''' If set before the first year of dataset data, the spatial temporal 
        ''' framework will repeat the FIRST YEAR of external data until the
        ''' actual external data is encountered.
        ''' </summary>
        Public Property CustomDateStart As DateTime = Date.MaxValue

        ''' <summary></summary>
        Public ReadOnly Property UseDefaultDateEnd As Boolean
            Get
                Return cDateUtils.DateEquals(Me.CustomDateEnd, Date.MinValue)
            End Get
        End Property

        ''' <summary>
        ''' Custom end date for bringing in external data.
        ''' If set past the last year of dataset data, the spatial temporal 
        ''' framework will keep repeating the LAST YEAR of external data.
        ''' </summary>
        Public Property CustomDateEnd As DateTime = Date.MinValue

        ''' <summary></summary>
        Public Property Adapter As cSpatialDataAdapter = Nothing

        ''' <summary></summary>
        Public Property iLayer As Integer = 1

        ''' <summary></summary>
        Public Sub New()
        End Sub

        ''' <summary></summary>
        Public Overridable Function IsConfigured() As Boolean

            Dim bIsConfigured As Boolean = False

            If (Me.Dataset IsNot Nothing) Then
                If (Me.Dataset.IsConfigured()) Then
                    If Not String.IsNullOrWhiteSpace(Me.Dataset.ConversionFormat) Then
                        If (Me.Converter IsNot Nothing) Then
                            bIsConfigured = bIsConfigured Or Me.Converter.IsConfigured()
                        End If
                    Else
                        bIsConfigured = True
                    End If
                End If
            End If
            Return bIsConfigured

        End Function

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to resolve the start year of external data, based on dataset 
        ''' configuration and optional choices.
        ''' </summary>
        ''' <seealso cref="CustomDateStart"/>
        ''' <seealso cref="UseDefaultDateStart"/>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TimeStart As DateTime
            Get
                If (Me.Dataset Is Nothing) Then Return Nothing
                If (Me.UseDefaultDateStart) Then Return Me.Dataset.TimeStart
                Return Me.CustomDateStart
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to resolve the end year of external data, based on dataset 
        ''' configuration and optional choices.
        ''' </summary>
        ''' <seealso cref="CustomDateStart"/>
        ''' <seealso cref="UseDefaultDateStart"/>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TimeEnd As DateTime
            Get
                If (Me.Dataset Is Nothing) Then Return Nothing
                If (Me.UseDefaultDateEnd) Then Return Me.Dataset.TimeEnd
                Return Me.CustomDateEnd
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Translate a date to 
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="dt"></param>
        ''' <returns>Converts an incoming date to a date point within the applied date range</returns>
        ''' -------------------------------------------------------------------
        Public Function ToDataTime(core As cCore, dt As DateTime) As DateTime

            If (Me.Dataset Is Nothing) Then Return dt

            Dim dtStart As DateTime = Me.TimeStart
            Dim dtEnd As DateTime = Me.TimeEnd

            If (dt < dtStart Or dt > dtEnd) Then Return DateTime.MinValue

            Dim nStepsYear As Integer = core.m_EcospaceData.nTimeStepsPerYear
            Dim iTime As Integer = core.AbsoluteTimeToEcospaceTimestep(dt)

            If dt < Me.Dataset.TimeStart Then

                ' Need to borrow repeating first year point
                Dim iDataStart As Integer = core.AbsoluteTimeToEcospaceTimestep(Me.Dataset.TimeStart)

                Dim iTx As Integer = iTime Mod nStepsYear
                Dim iSx As Integer = iDataStart Mod nStepsYear
                Dim iOffset As Integer = If(iTx < iSx, nStepsYear, 0)
                Dim iDataReal As Integer = (iDataStart \ nStepsYear) * nStepsYear + iTx + iOffset

                Return core.EcospaceTimestepToAbsoluteTime(iDataReal)
            End If

            If dt > Me.Dataset.TimeEnd Then

                ' Need to borrow repeating end year point
                Dim iDataEnd As Integer = core.AbsoluteTimeToEcospaceTimestep(Me.Dataset.TimeEnd) - nStepsYear + 1

                Dim iTx As Integer = iTime Mod nStepsYear
                Dim iSX As Integer = iDataEnd Mod nStepsYear
                Dim iOffset As Integer = If(iTx > iSX, nStepsYear, 0)

                Return core.EcospaceTimestepToAbsoluteTime((iDataEnd \ nStepsYear) * nStepsYear + iTx - iOffset)

            End If

            Return dt

        End Function

#End Region ' Helper methods

    End Class

End Namespace
