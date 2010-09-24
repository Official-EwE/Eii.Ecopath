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
        ''' Obtain a lighter variant of a <see cref="Color">color</see>.
        ''' </summary>
        ''' <param name="clr">The colour to obtain a variant colour for.</param>
        ''' <param name="iVariant">The zero-based numbered variant to obtain.</param>
        ''' <returns>A lighter colour variant.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetVariant(ByVal clr As Color, ByVal iVariant As Integer) As Color

            iVariant = Math.Max(0, iVariant + 1)
            Return Color.FromArgb(255, _
                                  CInt(255 - (255 - clr.R) / iVariant), _
                                  CInt(255 - (255 - clr.G) / iVariant), _
                                  CInt(255 - (255 - clr.B) / iVariant))

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

