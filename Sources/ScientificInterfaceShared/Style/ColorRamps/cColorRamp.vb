' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.UserInterface





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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stock gradient ID, loosely maintained by the gradient classes in this assembly.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ID As Integer = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the name of the color ramp.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Name As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gets if this color ramp is defined by EwE and cannot be modified.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsSystemRamp() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Gets if this color ramp can be edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable ReadOnly Property IsEditable() As Boolean
            Get
                Return (Me.ID = -9999) And Not Me.IsSystemRamp
            End Get
        End Property

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

        ''' <summary>
        ''' Facade
        ''' </summary>
        ''' <param name="dValue"></param>
        ''' <param name="dValueMax"></param>
        ''' <returns></returns>
        Public Function GetColor(dValue As Double, Optional dValueMax As Double = 1.0) As Color
            Return cStyleGuide.FromVisualColor(Me.GetColorInvariant(dValue, dValueMax))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a colour for a given value from the ramp.
        ''' </summary>
        ''' <param name="dValue">The value to calculate the colour for. Typically, this variable will range from [0..1]</param>
        ''' <param name="dValueMax">The maximum to scale the value to.</param>
        ''' <returns>The colour for the given value.</returns>
        ''' <remarks>Override this method to implement a specific ColorRamp.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetColorInvariant(dValue As Double, Optional dValueMax As Double = 1.0) As VisualColor


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

        Protected Function Interpolate(nVal1 As Integer, nVal2 As Integer, dRatio As Double) As Byte
            Try
                Return CByte(CInt(Math.Round(nVal1 + (nVal2 - nVal1) * dRatio)))
            Catch ex As Exception
                Return 0
            End Try
        End Function


    End Class

End Namespace
