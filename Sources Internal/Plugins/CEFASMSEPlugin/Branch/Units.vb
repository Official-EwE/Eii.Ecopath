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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region ' Imports 

Public Enum eConvertTypes As Integer
    ''' <summary>Convert Biomass from interface Tonnes^3 (kt) to t/km2</summary>
    ToEcopathBio
    ''' <summary>Convert Biomass from Ecopath t/km2 to Tonnes^3 (kt)</summary>
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
