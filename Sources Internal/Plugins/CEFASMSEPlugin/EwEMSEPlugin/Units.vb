Option Strict On
Option Explicit On

Imports EwECore

Public Enum eConvertTypes
    ''' <summary>
    ''' Convert Biomass from interface Tonnes^3 (kt) to t/km2
    ''' </summary>
    ''' <remarks></remarks>
    ToEcopathBio

    ''' <summary>
    ''' Convert Biomass from Ecopath t/km2 to Tonnes^3 (kt)
    ''' </summary>
    ''' <remarks></remarks>
    ToDisplayBio
End Enum

Public Class Units

    Private Shared _core_ As cCore

    Public Shared Sub Init(theCore As cCore)
        _core_ = theCore
    End Sub

    Public Shared Function Convert(ConversionType As eConvertTypes, Value As Double) As Double

        Try

            Select Case ConversionType

                Case eConvertTypes.ToEcopathBio
                    Return Value / _core_.EwEModel.Area * 1000
                Case eConvertTypes.ToDisplayBio
                    Return Value * _core_.EwEModel.Area / 1000
            End Select

        Catch ex As Exception
            Debug.Assert(False, "Exception converting units. " + ex.Message)
        End Try

        Return Value

    End Function



End Class
