#Region " Imports "

Option Strict On
Imports System
Imports System.Diagnostics

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing a collection of <see cref="Date">date</see>-related utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDateUtils

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Julian date for a given <see cref="Date">date</see>. If no 
        ''' date is specified, the Julian date for the current time is returned.
        ''' </summary>
        ''' <param name="dt">The date to return the Julian date for.</param>
        ''' <returns>A Julian date.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function DateToJulian(Optional ByVal dt As Date = Nothing) As Double
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
        Public Shared Function JulianToDate(ByVal dJulian As Double) As Date
            Return Date.FromOADate(dJulian)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the textual representation for a month number.
        ''' </summary>
        ''' <param name="iMonth">The month to format [1, 12]</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMonthName(ByVal iMonth As Integer, _
                                            Optional ByVal bFullName As Boolean = True) As String
            Try
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
        Public Shared Function Easter(ByVal y As Integer) As Date

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

    End Class

End Namespace
