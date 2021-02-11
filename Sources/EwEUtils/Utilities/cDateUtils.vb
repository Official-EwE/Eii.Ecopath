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

#Region " Imports "

Option Strict On
Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.Net

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing a collection of <see cref="Date">date</see>-related utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDateUtils

#Region " Internals "

        Private Shared s_starttime As DateTime = Date.Now

#End Region ' Internals

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Julian date for a given <see cref="Date">date</see>. If no 
        ''' date is specified, the Julian date for the current time is returned.
        ''' </summary>
        ''' <param name="dt">The date to return the Julian date for.</param>
        ''' <returns>A Julian date.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function DateToJulian(Optional dt As Date = Nothing) As Double
            If dt = Nothing Then dt = Date.Now
            Return dt.ToOADate()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a <see cref="Date">date</see> for a Julian date value.
        ''' </summary>
        ''' <param name="dJulian">The Julian date to return a Date instance for.</param>
        ''' <returns>A Date instance.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function JulianToDate(dJulian As Double) As Date
            Return Date.FromOADate(dJulian)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the textual representation for a month number.
        ''' </summary>
        ''' <param name="iMonth">The month to format [1, 12]</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMonthName(iMonth As Integer,
                                            Optional bFullName As Boolean = True) As String
            Try
                If (iMonth < 1 Or iMonth > 12) Then Return ""

                Dim dt As New DateTime(1, iMonth, 1)
                If bFullName Then
                    Return dt.ToString("MMMM")
                Else
                    Return dt.ToString("MMM")
                End If
            Catch ex As Exception
                Debug.Assert(False, "Month out of range")
            End Try
            Return ""
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the date of Easter for a given year in Gregorian calendars.
        ''' </summary>
        ''' <param name="y">The Gregorian year to calculate easter for [1583, 4099]</param>
        ''' <returns>The date of Easter Sunday.</returns>
        ''' <remarks>
        ''' This algorithm is adapted from http://www.gmarts.org/index.php?go=415, 
        ''' whcih in turn is adapted from a faq document by Claus Tondering
        ''' URL: http://www.pip.dknet.dk/~pip10160/calendar.faq2.txt
        ''' E-mail: c-t@pip.dknet.dk.
        ''' The FAQ algorithm is based in part on the algorithm of Oudin (1940)
        ''' as quoted in "Explanatory Supplement to the Astronomical Almanac",
        ''' P. Kenneth Seidelmann, editor.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function Easter(y As Integer) As Date

            Debug.Assert(y >= 1583 And y <= 4099, "Gregorian calendar Easters apply for years 1583 to 4099 only")

            Dim d As Integer = 0 ' Day
            Dim m As Integer = 0 ' Month
            Dim g As Integer = 0 ' golden year - 1
            Dim c As Integer = 0 ' century
            Dim h As Integer = 0 ' = (23 - Epact) mod 30
            Dim i As Integer = 0 ' no of days from March 21 to Paschal Full Moon
            Dim j As Integer = 0 ' weekday for PFM (0=Sunday, etc)
            Dim p As Integer = 0 ' no of days from March 21 to Sunday on or before PFM
            Dim e As Integer = 0 'extra days to add for method 2 (converting Julian date to Gregorian date)

            g = y Mod 19
            c = y \ 100
            h = (c - c \ 4 - (8 * c + 13) \ 25 + 19 * g + 15) Mod 30
            i = h - (h \ 28) * (1 - (h \ 28) * (29 \ (h + 1)) * ((21 - g) \ 11))
            j = (y + y \ 4 + i + 2 - c + c \ 4) Mod 7
            'return day and month
            p = i - j + e
            ' p can be from -6 to 56 corresponding to dates 22 March to 23 May
            ' (later dates apply to method 2, although 23 May never actually occurs)
            d = 1 + (p + 27 + (p + 6) \ 40) Mod 31
            m = 3 + (p + 26) \ 30

            Return New Date(y, m, d)

        End Function

        Public Enum eNextEvent As Integer
            None
            Easter
            Xmas
            DagVanDeLiefde
        End Enum

        Public Shared Function GetNextEvent(iNumDays As Integer) As eNextEvent

            Dim dtNow As DateTime = New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day)
            Dim dtEaster As DateTime = cDateUtils.Easter(dtNow.Year)
            Dim dtXMas As DateTime = New Date(dtNow.Year, 12, 25)
            Dim dtJoepie As DateTime = New Date(dtNow.Year, 2, 14)

            If (dtEaster >= dtNow) And (dtEaster.Subtract(dtNow).Days <= iNumDays) Then Return eNextEvent.Easter
            If (dtXMas >= dtNow) And (dtXMas.Subtract(dtNow).Days <= iNumDays) Then Return eNextEvent.Xmas
            If (dtJoepie >= dtNow) And (dtJoepie.Subtract(dtNow).Days <= iNumDays / 2) Then Return eNextEvent.DagVanDeLiefde

            Return eNextEvent.None

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of months between two dates.
        ''' </summary>
        ''' <param name="first"></param>
        ''' <param name="second"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://stackoverflow.com/questions/3249968/calculating-number-of-months-between-2-dates
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function MonthDifference(first As DateTime, second As DateTime) As Integer
            Return Math.Abs((first.Month - second.Month) + 12 * (first.Year - second.Year))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the time from an Internet server. The retrieved time can be obtained
        ''' from <see cref="StartTime"/>.
        ''' </summary>
        ''' <param name="server">Full URL to the server to obtain time from.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' https://stackoverflow.com/questions/6435099/how-to-get-datetime-from-the-internet
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetNetworkTime(Optional server As String = "http://www.ecopath.org") As Boolean

            Try
                Using response As WebResponse = WebRequest.Create(server).GetResponse()
                    cDateUtils.StartTime = DateTime.ParseExact(response.Headers("date"), "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal)
                End Using
                Return True

            Catch ex As Exception
                ' Ouch
            End Try
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the time returned by <see cref="GetNetworkTime(String)"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Property StartTime As DateTime
            Get
                Return s_starttime
            End Get
            Private Set(value As DateTime)
                s_starttime = value
            End Set
        End Property
    End Class

End Namespace
