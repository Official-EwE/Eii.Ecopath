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

Option Strict On
Imports System.Drawing
Imports EwECore
Imports EwEUtils.Core

Namespace Style

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' <para>This class implements a gradient across a number of colours.</para>
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cColorRamp

        Public Sub New(id As Integer, bIsSystemRamp As Boolean)
            Me.ID = id
            Me.IsSystemRamp = bIsSystemRamp
        End Sub

        ''' <summary>
        ''' Stock gradient ID, loosely maintained by the gradient classes in this assembly.
        ''' </summary>
        Public ReadOnly Property ID As Integer = 0

        Public MustOverride Property Name As String

        Public ReadOnly Property IsSystemRamp() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the color start offset, which enables selective use of the color ramp without having to modify the ramp.
        ''' </summary>
        ''' <remarks>
        ''' If the start offset exceeds than the end offset, the entire color scheme is reversed.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ColorOffsetStart() As Single = 0.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the color end offset, which enables selective use of the color ramp without having to modify the ramp.
        ''' </summary>
        ''' <remarks>
        ''' If the start offset exceeds than the end offset, the entire color scheme is reversed.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ColorOffsetEnd() As Single = 1.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a colour for a given value from the ramp.
        ''' </summary>
        ''' <param name="dValue">The value to calculate the colour for. Typically, this variable will range from [0..1]</param>
        ''' <param name="dValueMax">The maximum to scale the value to.</param>
        ''' <returns>The colour for the given value.</returns>
        ''' <remarks>Override this method to implement a specific ColorRamp.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetColor(dValue As Double, Optional dValueMax As Double = 1.0) As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recalculates a colour lookup value by applying ColorOffsets.
        ''' </summary>
        ''' <param name="dValue"></param>
        ''' <param name="dValueMax"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function RecalcValue(dValue As Double, dValueMax As Double) As Double

            Dim dLow As Double = Math.Min(Me.ColorOffsetStart, Me.ColorOffsetEnd)
            Dim dHigh As Double = Math.Max(Me.ColorOffsetStart, Me.ColorOffsetEnd)

            If (dValueMax = 0) Then dValueMax = 1.0

            ' Apply color offsets
            dValue = (dLow + (dHigh - dLow) * Math.Min(1.0, Math.Max(0, dValue / dValueMax)))

            ' Reverse?
            If (Me.ColorOffsetStart > Me.ColorOffsetEnd) Then
                dValue = 1.0 - dValue
            End If

            Return dValue

        End Function

        Protected Function Interpolate(nVal1 As Integer, nVal2 As Integer, dRatio As Double) As Integer
            Try
                Return CInt(Math.Round(nVal1 + (nVal2 - nVal1) * dRatio))
            Catch ex As Exception
                Return 0
            End Try
        End Function

    End Class

End Namespace
