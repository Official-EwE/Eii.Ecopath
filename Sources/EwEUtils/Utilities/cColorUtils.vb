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
        ''' Obtain a lighter variant of a colour.
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
    End Class

End Namespace

