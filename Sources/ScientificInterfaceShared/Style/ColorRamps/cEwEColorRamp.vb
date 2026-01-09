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



Option Explicit On
Imports EwEUtils.UserInterface



Namespace Style

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implements the default EwE5 color ramp that has been used since 2000. This color ramp,
    ''' running from light blue to red, was designed to work well in both digital and
    ''' printed media.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public NotInheritable Class cEwEColorRamp
        Inherits cColorRamp

        Public Sub New(name As String, Optional startoffset As Single = 0, Optional endoffset As Single = 1.0)
            MyBase.New(0, True)
            Me.ColorOffsetStart = startoffset
            Me.ColorOffsetEnd = endoffset
            MyBase.Name = name
        End Sub

        Public Overrides Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return an ARGB colour for a given value.
        ''' </summary>
        ''' <param name="dValue">The value to return the colour for.</param>
        ''' <param name="dValueMax">The maximum value to scale the value to. By default, it is assumed that a colour must be retrieved on a scale from [0..1]</param>
        ''' <returns>The colour for a given value.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetColorInvariant(dValue As Double, Optional dValueMax As Double = 1.0) As VisualColor

            Const sMaxColor As Double = 250

            Dim sRed As Double = 0.0
            Dim sGrn As Double = 0.0
            Dim sBlu As Double = 0.0
            Dim sMid As Double = 0.0

            ' Brutal safety catch
            If (Not dValueMax > 0.0) Then
                Return Nothing
            End If

            ' Apply color offsets
            dValue = Me.RecalcValue(dValue, dValueMax)
            dValueMax = 1.0

            ' Original algorithm (see below, OrigColorGrad) always inverts colours, aargh
            dValue = 1.0 - dValue

            sMid = dValueMax / 2.0

            ' Red
            Select Case dValue
                Case 0 To 2.0 * (dValueMax / 3.0)
                    sRed = 245.0 * ((2.5 * dValueMax / 3.0 - dValue) / sMid)
                Case Else 'dValue >= sMid 
                    sRed = 255.0 * ((dValue - sMid) / sMid)
            End Select

            ' Green
            Select Case dValue
                Case 0 To sMid
                    sGrn = 300.0 * (dValue / sMid)
                Case Else 'dValue >= sMid
                    sGrn = 255.0 + 45.0 * ((dValue - sMid) / sMid)
            End Select

            ' Blue
            Select Case dValue
                Case 0 To dValueMax / 3.0
                    sBlu = 5.0 * (dValue / sMid)
                Case Else 'dValue >= sMid
                    sBlu = 55.0 + 455.0 * ((dValue - sMid) / sMid)
            End Select

            ' Truncate
            sRed = Math.Max(0.0, Math.Min(255.0, Math.Round(sRed)))
            sGrn = Math.Max(0.0, Math.Min(255.0, Math.Round(sGrn)))
            sBlu = Math.Max(0.0, Math.Min(255.0, Math.Round(sBlu)))

            ' Avoid white
            If sBlu > sMaxColor Then
                sRed = Math.Min(sRed, sMaxColor)
                sGrn = Math.Min(sGrn, sMaxColor)
            End If

            Return New VisualColor(CByte(Math.Round(sRed)), CByte(Math.Round(sGrn)), CByte(Math.Round(sBlu)))
        End Function

    End Class

End Namespace
