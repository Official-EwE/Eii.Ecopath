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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Security.AccessControl

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper with utility methods for dealing with numbers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cNumberUtils

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a value has a finite value that can be determined.
        ''' </summary>
        ''' <param name="sValue">Value to evaluate.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IsFinite(ByVal sValue As Single) As Boolean
            If Single.IsInfinity(sValue) Or Single.IsNaN(sValue) Then Return False
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 'Fix' a number by translating NaN, negative and positive infinity
        ''' values to user-defined values.
        ''' </summary>
        ''' <param name="sValue">Value to test.</param>
        ''' <param name="sNaN">Not a number value to substitute.</param>
        ''' <param name="sNegInf">Negative infinity value to substitute.</param>
        ''' <param name="sPosInf">Positive infinity value to substitute.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FixValue(ByVal sValue As Single, _
                                        Optional ByVal sNaN As Single = 0, _
                                        Optional ByVal sNegInf As Single = -1, _
                                        Optional ByVal sPosInf As Single = 1) As Single

            If cNumberUtils.IsFinite(sValue) Then Return sValue

            If Single.IsNegativeInfinity(sValue) Then
                Return sNegInf
            End If
            If Single.IsPositiveInfinity(sValue) Then
                Return sPosInf
            End If
            Return sNaN

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether two numbers can be considered equal.
        ''' </summary>
        ''' <param name="dVal1">First value to compare.</param>
        ''' <param name="dVal2">Second value to compare.</param>
        ''' <param name="dThreshold">Max difference for the two values to be considered equal.</param>
        ''' <returns>True if the two values differ by no more than the given threshold.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Approximates(dVal1 As Double, dVal2 As Double, dThreshold As Double) As Boolean
            Return (Math.Abs(dVal1 - dVal2) <= dThreshold)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether two numbers can be considered equal.
        ''' </summary>
        ''' <param name="sVal1">First value to compare.</param>
        ''' <param name="sVal2">Second value to compare.</param>
        ''' <param name="sThreshold">Max difference for the two values to be considered equal.</param>
        ''' <returns>True if the two values differ by no more than the given threshold.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Approximates(sVal1 As Single, sVal2 As Single, sThreshold As Single) As Boolean
            Return (Math.Abs(sVal1 - sVal2) <= sThreshold)
        End Function

    End Class

End Namespace
