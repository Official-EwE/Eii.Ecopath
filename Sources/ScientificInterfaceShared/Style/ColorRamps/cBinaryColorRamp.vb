' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On
Imports EwEUtils.UserInterface



Namespace Style

    Public Class cBinaryColorRamp
        Inherits cColorRamp

        Public Sub New(id As Integer, name As String, colors As VisualColor())
            MyBase.New(id, False)
            Me.Colors = colors
            Me.Name = name
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return an ARGB colour for a given value.
        ''' </summary>
        ''' <param name="dValue">The value to return the colour for.</param>
        ''' <param name="dValueMax">The maximum value to scale the value to. By default, it is assumed that a colour must be retrieved on a scale from [0..1]</param>
        ''' <returns>The colour for a given value.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetColorInvariant(dValue As Double, Optional dValueMax As Double = 1) As VisualColor

            Dim n As Integer = Me.Colors.Length
            Dim iColor As Integer = 0
            If (n > 0) Then
                iColor = CInt(Math.Floor((n - 1) * dValue / dValueMax))
                Return Me.Colors(iColor)
            End If
            Return VisualColor.FromArgb(&HFF000000)
        End Function

        Public ReadOnly Property Colors As VisualColor()

    End Class

End Namespace
