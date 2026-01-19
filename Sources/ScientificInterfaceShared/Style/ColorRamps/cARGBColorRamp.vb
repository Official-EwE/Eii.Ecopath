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


Imports EwEUtils.UserInterface

Namespace Style

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' <para>Implements a <see cref="cColorRamp">color ramp</see>, where colours are specified in ARGB values.</para>
    ''' </summary>
    ''' <remarks>
    ''' <para>For examples on how to use this class, refer to the following methods:
    ''' <list type="bullet">
    ''' <item><description><see cref="cARGBColorRamp">Constructor</see></description></item>
    ''' </list>
    ''' </para>
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Class cARGBColorRamp
        Inherits cColorRamp

        ''' <summary>Gradient break colours</summary>
        Private m_colors() As VisualColor
        ''' <summary>Gradient break values - ABSOLUTE</summary>
        Private m_breaks() As Double
        ''' <summary>Gradient break values - RELATIVE</summary>
        Private m_relbreaks() As Double

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the cARGBColorRamp class.
        ''' </summary>
        ''' <param name="name">The name of the color ramp.</param>
        ''' <param name="colors">The colour breaks to use.</param>
        ''' <param name="breaks">The position of each colour break, 
        ''' relative to its predessesor.</param>
        ''' <param name="iSystemID">Unique system ID if positive and non-zero.
        ''' System color ramps are not editable and retrievable by ID rather
        ''' than by color and break locations.</param>
        ''' <remarks>
        ''' The following snippet illustrates how to create a valid ARGB color ramp:
        ''' <code>
        ''' ' Define a three level colour ramp
        ''' Dim colors(2) as Color
        ''' Dim positions(2) as Integer
        ''' 
        ''' ' Ramp begins with light blue at position 0
        ''' colors(0) = Color.FromARGB(255, 200, 200, 255)
        ''' positions(0) = 0
        ''' ' At 40%, the ramp is a green tone
        ''' colors(1) = Color.FromARGB(255, 100, 255, 100)
        ''' positions(1) = 0.4
        ''' ' At 100% (0.4 + 0.6) the ramp is deep red
        ''' colors(2) = Color.FromARGB(255, 255, 25, 25)
        ''' positions(2) = 0.6
        ''' 
        ''' ' Create the ramp
        ''' Dim ramp As New ARGBColorRamp("test", colors, positions)
        ''' 
        ''' ' Now get the value at 65%, let's see what happens...
        ''' Dim clr as Color = ramp.GetColor(0.65)
        ''' </code>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(name As String, ByVal colors() As VisualColor, ByVal breaks() As Double, Optional iSystemID As Integer = cCore.NULL_VALUE)

            MyBase.New(iSystemID, iSystemID > 0)

            Me.GradientColors = colors
            Me.GradientBreaks = breaks

            If (String.IsNullOrWhiteSpace(name)) Then name = My.Resources.DEFAULT_COLORRAMP
            Me.Name = name

        End Sub

        Public Sub New(name As String, ByVal colors() As Color, ByVal breaks() As Double, Optional iSystemID As Integer = cCore.NULL_VALUE)
            Me.New(name, cStyleGuide.ToVisualColors(colors), breaks, iSystemID)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a cARGBColorRamp with the colors equally distributed.
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="colors"></param>
        ''' <param name="iSystemID"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(name As String, ByVal colors() As VisualColor, Optional iSystemID As Integer = cCore.NULL_VALUE)

            MyBase.New(iSystemID, iSystemID > 0)

            Dim n As Integer = colors.Length - 1
            Dim breaks(n) As Double
            For i As Integer = 1 To n
                breaks(i) = 1 / n
            Next

            For i As Integer = 0 To n
                Dim c As VisualColor = colors(i)
                If c.A = 0 Then colors(i) = New VisualColor(c.R, c.G, c.B)
            Next

            Me.GradientColors = colors
            Me.GradientBreaks = breaks

            If (String.IsNullOrWhiteSpace(name)) Then name = My.Resources.DEFAULT_COLORRAMP
            Me.Name = name

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copy constructor.
        ''' </summary>
        ''' <param name="ramp">The ramp to duplicate.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ramp As cARGBColorRamp)
            Me.New(ramp.Name, ramp.GradientColors, ramp.GradientBreaks)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return an ARGB colour for a given value.
        ''' </summary>
        ''' <param name="dValue">The value to return the colour for.</param>
        ''' <param name="dValueMax">The maximum value to scale the value to. By default, it is assumed that a colour must be retrieved on a scale from [0..1]</param>
        ''' <returns>The colour for a given value.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetColorInvariant(dValue As Double, Optional dValueMax As Double = 1.0) As VisualColor

            ' Pre
            Debug.Assert(Me.m_relbreaks.Length = Me.m_colors.Length)

            ' Normalize nValue to nValueMax
            Dim nIndex As Integer = 0
            Dim bFound As Boolean = False

            ' Apply color offsets
            dValue = Me.RecalcValue(dValue, dValueMax)
            dValueMax = 1.0

            ' Find first index
            bFound = (dValue <= Me.m_breaks(0))
            While Not bFound
                nIndex += 1
                bFound = (nIndex = Me.m_breaks.Length)
                If Not bFound Then
                    bFound = (dValue <= Me.m_breaks(nIndex))
                End If
            End While

            ' Below first level? Return first colour without interpolating
            If (nIndex = 0) Then Return Me.m_colors(0)
            ' Past last level? Return formar-last level without interpolating
            If (nIndex = Me.m_breaks.Length) Then Return Me.m_colors(nIndex - 1)
            ' Exactly at a known level? Return the level colour withour interpolating
            If dValue = Me.m_breaks(nIndex) Then Return Me.m_colors(nIndex)

            ' must interpolate
            Dim c1 As VisualColor = Me.m_colors(nIndex - 1)
            Dim c2 As VisualColor = Me.m_colors(nIndex)
            Dim dX As Double = Me.m_breaks(nIndex) - Me.m_breaks(nIndex - 1)
            Dim dPosX As Double = dValue - Me.m_breaks(nIndex - 1)

            Dim dRatio As Double = (dPosX / dX)

            If (dRatio > 1.0) Then
                dRatio = 1.0
            End If

            Return New VisualColor(Me.Interpolate(c1.R, c2.R, dRatio), Me.Interpolate(c1.G, c2.G, dRatio), Me.Interpolate(c1.B, c2.B, dRatio), Me.Interpolate(c1.A, c2.A, dRatio))

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the colours to use for every <see cref="GradientBreaks">gradient stop</see>.
        ''' </summary>
        ''' <remarks>
        ''' Note that the number of breaks and colors must match when trying to
        ''' use the gradient.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property GradientColors As VisualColor()
            Get
                Return Me.m_colors
            End Get
            Set(value As VisualColor())
                Me.m_colors = CType(value.Clone(), VisualColor())
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the position for gradient breaks, relative to its predessesor.
        ''' </summary>
        ''' <remarks>
        ''' Note that the number of breaks and colors must match when trying to
        ''' use the gradient.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property GradientBreaks() As Double()
            Get
                Return Me.m_relbreaks
            End Get
            Set(value As Double())
                ReDim Me.m_breaks(value.Length - 1)
                ReDim Me.m_relbreaks(value.Length - 1)
                Dim dTotalPos As Double = 0.0#
                For i As Integer = 0 To value.Length - 1
                    dTotalPos += CDbl(Math.Abs(value(i)))
                    Me.m_breaks(i) = dTotalPos
                    Me.m_relbreaks(i) = value(i)
                Next
            End Set
        End Property

    End Class

End Namespace
