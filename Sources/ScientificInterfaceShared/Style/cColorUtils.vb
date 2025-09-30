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

#Region " Imports "

Option Strict On
Imports EwEUtils.UserInterface
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing a collection of <see cref="Date">date</see>-related utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cColorUtils

        Private Shared gRandom As New Random()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain a different shade of a <see cref="VisualColor"/>.
        ''' </summary>
        ''' <param name="clr">The colour to obtain a variant colour for.</param>
        ''' <param name="sShade">Shade [-1, 1] to obtain. -1 returns black, 1 returns white.</param>
        ''' <returns>A different shade of a colour.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetVariant(clr As Color, sShade As Single) As Color

            sShade = Math.Max(-1, Math.Min(1, sShade))

            If (sShade < 0) Then
                Return Color.FromArgb(255,
                                      CInt(clr.R * (1 + sShade)),
                                      CInt(clr.G * (1 + sShade)),
                                      CInt(clr.B * (1 + sShade)))
            Else
                Return Color.FromArgb(255,
                                      CInt(clr.R + (255 - clr.R) * sShade),
                                      CInt(clr.G + (255 - clr.G) * sShade),
                                      CInt(clr.B + (255 - clr.B) * sShade))
            End If

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a integer of format AARRGGBB to a <see cref="VisualColor"/> value.
        ''' </summary>
        ''' <param name="iColor">The integer to convert.</param>
        ''' <returns>A color.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IntToColor(iColor As Integer) As Color
            Return Color.FromArgb((iColor >> 24) And &HFF, (iColor >> 16) And &HFF, (iColor >> 8) And &HFF, iColor And &HFF)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a integer of format AARRGGBB to a <see cref="VisualColor"/> value.
        ''' </summary>
        ''' <param name="iColor">The integer to convert.</param>
        ''' <returns>A color.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IntToColorInvariant(iColor As Integer) As VisualColor
            Return VisualColor.FromArgb((iColor >> 24) And &HFF, (iColor >> 16) And &HFF, (iColor >> 8) And &HFF, iColor And &HFF)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a <see cref="VisualColor"/> to an integer of format AARRGGBB.
        ''' </summary>
        ''' <param name="clr">The <see cref="VisualColor"/> to convert.</param>
        ''' <returns>An integer of the format AARRGGBB.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ColorToInt(clr As Color) As Integer
            Return ((clr.A And &HFF) << 24) + ((clr.R And &HFF) << 16) + ((clr.G And &HFF) << 8) + (clr.B And &HFF)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a <see cref="VisualColor"/> to an integer of format AARRGGBB.
        ''' </summary>
        ''' <param name="clr">The <see cref="VisualColor"/> to convert.</param>
        ''' <returns>An integer of the format AARRGGBB.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ColorInvariantToInt(clr As VisualColor) As Integer
            Return ((clr.A And &HFF) << 24) + ((clr.R And &HFF) << 16) + ((clr.G And &HFF) << 8) + (clr.B And &HFF)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generate a random colour.
        ''' </summary>
        ''' <param name="mix">The colour to mix with.</param>
        ''' <returns>A random color.</returns>
        ''' <remarks>
        ''' From http://stackoverflow.com/questions/43044/algorithm-to-randomly-generate-an-aesthetically-pleasing-color-palette
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function RandomColor(mix As VisualColor) As VisualColor

            Dim red As Integer = gRandom.Next(256)
            Dim green As Integer = gRandom.Next(256)
            Dim blue As Integer = gRandom.Next(256)

            ' mix the color
            If (mix <> Nothing) Then
                red = CInt((red + mix.R) / 2)
                green = CInt((green + mix.G) / 2)
                blue = CInt((blue + mix.B) / 2)
            End If

            Return VisualColor.FromArgb(255, red, green, blue)

        End Function

        Public Shared Function IsDark(clr As Color) As Boolean
            Return IsDarkInvariant(cStyleGuide.ToVisualColor(clr))
        End Function

        Public Shared Function IsDarkInvariant(clr As VisualColor) As Boolean
            Dim soften As Single = clr.A / 255.0!
            Return ((soften * clr.R + soften * clr.G + soften * clr.B) / 3) < 127
        End Function


        Public Shared Function IsLight(clr As Color) As Boolean
            Return Not IsDark(clr)
        End Function

        Public Shared Function IsLightInvariant(clr As VisualColor) As Boolean
            Return Not IsDarkInvariant(clr)
        End Function

        Public Shared Function Inverse(clr As Color) As Color
            Return Color.FromArgb(clr.A, 255 - clr.R, 255 - clr.G, 255 - clr.B)
        End Function

        Public Shared Function InverseInvariant(clr As VisualColor) As VisualColor
            Return VisualColor.FromArgb(clr.A, 255 - clr.R, 255 - clr.G, 255 - clr.B)
        End Function

    End Class

End Namespace

