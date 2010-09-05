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

    End Class

End Namespace
