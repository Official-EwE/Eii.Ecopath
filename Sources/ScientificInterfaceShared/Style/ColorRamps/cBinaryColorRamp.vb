' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



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
