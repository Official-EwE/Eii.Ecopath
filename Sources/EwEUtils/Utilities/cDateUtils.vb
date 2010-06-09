#Region " Imports "

Option Strict On
Imports System

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
        Public Shared Function GetJulianDate(Optional ByVal dt As Date = Nothing) As Single
            If dt = Nothing Then dt = Date.Now
            Return CSng(dt.ToOADate())
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a <see cref="Date">date</see> for a Julian date value.
        ''' </summary>
        ''' <param name="sJulian">The Julian date to return a Date instance for.</param>
        ''' <returns>A Date instance.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromJulianDate(ByVal sJulian As Single) As Date
            Return Date.FromOADate(CDbl(sJulian))
        End Function

    End Class

End Namespace
