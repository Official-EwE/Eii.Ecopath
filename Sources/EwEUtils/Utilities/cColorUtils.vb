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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System
Imports System.Drawing

#End Region ' Imports

Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing a collection of <see cref="Date">date</see>-related utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cColorUtils

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain a different shade of a <see cref="Color">color</see>.
        ''' </summary>
        ''' <param name="clr">The colour to obtain a variant colour for.</param>
        ''' <param name="sShade">Shade [-1, 1] to obtain. -1 returns black, 1 returns white.</param>
        ''' <returns>A different shade of a colour.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetVariant(ByVal clr As Color, ByVal sShade As Single) As Color

            sShade = Math.Max(-1, Math.Min(1, sShade))

            If (sShade < 0) Then
                Return Color.FromArgb(255, _
                                      CInt(clr.R * (1 + sShade)), _
                                      CInt(clr.G * (1 + sShade)), _
                                      CInt(clr.B * (1 + sShade)))
            Else
                Return Color.FromArgb(255, _
                                      CInt(clr.R + (255 - clr.R) * sShade), _
                                      CInt(clr.G + (255 - clr.G) * sShade), _
                                      CInt(clr.B + (255 - clr.B) * sShade))
            End If

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a integer of format AARRGGBB to a <see cref="Color">color</see> value.
        ''' </summary>
        ''' <param name="iColor">The integer to convert.</param>
        ''' <returns>A color.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IntToColor(ByVal iColor As Integer) As Color
            Return Color.FromArgb((iColor >> 24) And &HFF, (iColor >> 16) And &HFF, (iColor >> 8) And &HFF, iColor And &HFF)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a <see cref="Color">color</see> to an integer of format AARRGGBB.
        ''' </summary>
        ''' <param name="clr">The <see cref="Color">color</see> to convert.</param>
        ''' <returns>An integer of the format AARRGGBB.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ColorToInt(ByVal clr As Color) As Integer
            Return ((clr.A And &HFF) << 24) + ((clr.R And &HFF) << 16) + ((clr.G And &HFF) << 8) + (clr.B And &HFF)
        End Function

    End Class

End Namespace

