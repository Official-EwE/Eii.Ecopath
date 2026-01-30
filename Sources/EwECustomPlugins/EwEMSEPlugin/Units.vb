' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports EwECore



Public Enum eConvertTypes As Integer
    ''' <summary>Do not convert values</summary>
    None = 0
    ''' <summary>Convert Biomass from interface Tonnes^3 (kt) to t/km2</summary>
    ToEcopathBio = 1
    ''' <summary>Convert Biomass from Ecopath t/km2 to Tonnes^3 (kt)</summary>
    ToDisplayBio = 2
End Enum

Public Class Units

    Private Shared _core_ As cCore

    Public Shared Sub Init(theCore As cCore)
        _core_ = theCore
    End Sub

    Public Shared Function Convert(ConversionType As eConvertTypes, Value As Double) As Double

        Try
            Select Case ConversionType
                Case eConvertTypes.None
                    ' Do nothing ;-)
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
