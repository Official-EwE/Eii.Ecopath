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

#End Region ' Imports 

Namespace UserInterface

    ''' <summary>
    ''' OS-independent font descriptor
    ''' </summary>
    Public Enum VisualFontStyle As Integer
        Regular = 0
        Bold = 1
        Italic = 2
        Underline = 4
        Strikeout = 8
    End Enum

    ''' <summary>
    ''' OS-independent brushes descriptor
    ''' </summary>
    Public Enum VisualHatchStyle As Integer
        '     A pattern of horizontal lines.
        Horizontal = 0
        '     A pattern of vertical lines.
        Vertical = 1
        '     A pattern of lines on a diagonal from upper left to lower right.
        ForwardDiagonal = 2
        '     A pattern of lines on a diagonal from upper right to lower left.
        BackwardDiagonal = 3
        '     Specifies horizontal And vertical lines that cross.
        Cross = 4
        '     A pattern of crisscross diagonal lines.
        DiagonalCross = 5
        '     Specifies a 5-percent hatch. The ratio of foreground color to background color
        '     Is 5:95.
        Percent05 = 6
        '     Specifies a 10-percent hatch. The ratio of foreground color to background color
        '     Is 10:90.
        Percent10 = 7
        '     Specifies a 20-percent hatch. The ratio of foreground color to background color
        '     Is 20:80.
        Percent20 = 8
        '     Specifies a 25-percent hatch. The ratio of foreground color to background color
        '     Is 25:75.
        Percent25 = 9
        '     Specifies a 30-percent hatch. The ratio of foreground color to background color
        '     Is 30:70.
        Percent30 = 10
        '     Specifies a 40-percent hatch. The ratio of foreground color to background color
        '     Is 40:60.
        Percent40 = 11
        '     Specifies a 50-percent hatch. The ratio of foreground color to background color
        '     Is 50:50.
        Percent50 = 12
        '     Specifies a 60-percent hatch. The ratio of foreground color to background color
        '     Is 60:40.
        Percent60 = 13
        '     Specifies a 70-percent hatch. The ratio of foreground color to background color
        '     Is 70:30.
        Percent70 = 14
        '     Specifies a 75-percent hatch. The ratio of foreground color to background color
        '     Is 75:25.
        Percent75 = 15
        '     Specifies a 80-percent hatch. The ratio of foreground color to background color
        '     Is 80:100.
        Percent80 = 16
        '     Specifies a 90-percent hatch. The ratio of foreground color to background color
        '     Is 90:10.
        Percent90 = 17
        '     Specifies diagonal lines that slant to the right from top points to bottom points
        '     And are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal
        '     but are Not antialiased.
        LightDownwardDiagonal = 18
        '     Specifies diagonal lines that slant to the left from top points to bottom points
        '     And are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal
        '     but they are Not antialiased.
        LightUpwardDiagonal = 19
        '     Specifies diagonal lines that slant to the right from top points to bottom points
        '     are spaced 50 percent closer together than And are twice the width of System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal.
        '     This hatch pattern Is Not antialiased.
        DarkDownwardDiagonal = 20
        '     Specifies diagonal lines that slant to the left from top points to bottom points
        '     are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal
        '     And are twice its width but the lines are Not antialiased.
        DarkUpwardDiagonal = 21
        '     Specifies diagonal lines that slant to the right from top points to bottom points
        '     have the same spacing as hatch style System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal
        '     And are triple its width but are Not antialiased.
        WideDownwardDiagonal = 22
        '     Specifies diagonal lines that slant to the left from top points to bottom points
        '     have the same spacing as hatch style System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal
        '     And are triple its width but are Not antialiased.
        WideUpwardDiagonal = 23
        '     Specifies vertical lines that are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.Vertical.
        LightVertical = 24
        '     Specifies horizontal lines that are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.Horizontal.
        LightHorizontal = 25
        '     Specifies vertical lines that are spaced 75 percent closer together than hatch
        '     style System.Drawing.Drawing2D.HatchStyle.Vertical (Or 25 percent closer together
        '     than System.Drawing.Drawing2D.HatchStyle.LightVertical).
        NarrowVertical = 26
        '     Specifies horizontal lines that are spaced 75 percent closer together than hatch
        '     style System.Drawing.Drawing2D.HatchStyle.Horizontal (Or 25 percent closer together
        '     than System.Drawing.Drawing2D.HatchStyle.LightHorizontal).
        NarrowHorizontal = 27
        '     Specifies vertical lines that are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.Vertical
        '     And are twice its width.
        DarkVertical = 28
        '     Specifies horizontal lines that are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.Horizontal
        '     And are twice the width of System.Drawing.Drawing2D.HatchStyle.Horizontal.
        DarkHorizontal = 29
        '     Specifies dashed diagonal lines that slant to the right from top points to bottom
        '     points.
        DashedDownwardDiagonal = 30
        '     Specifies dashed diagonal lines that slant to the left from top points to bottom
        '     points.
        DashedUpwardDiagonal = 31
        '     Specifies dashed horizontal lines.
        DashedHorizontal = 32
        '     Specifies dashed vertical lines.
        DashedVertical = 33
        '     Specifies a hatch that has the appearance of confetti.
        SmallConfetti = 34
        '     Specifies a hatch that has the appearance of confetti And Is composed of larger
        '     pieces than System.Drawing.Drawing2D.HatchStyle.SmallConfetti.
        LargeConfetti = 35
        '     Specifies horizontal lines that are composed of zigzags.
        ZigZag = 36
        '     Specifies horizontal lines that are composed of tildes.
        Wave = 37
        '     Specifies a hatch that has the appearance of layered bricks that slant to the
        '     left from top points to bottom points.
        DiagonalBrick = 38
        '     Specifies a hatch that has the appearance of horizontally layered bricks.
        HorizontalBrick = 39
        '     Specifies a hatch that has the appearance of a woven material.
        Weave = 40
        '     Specifies a hatch that has the appearance of a plaid material.
        Plaid = 41
        '     Specifies a hatch that has the appearance of divots.
        Divot = 42
        '     Specifies horizontal And vertical lines each of which Is composed of dots that
        '     cross.
        DottedGrid = 43
        '     Specifies forward diagonal And backward diagonal lines each of which Is composed
        '     of dots that cross.
        DottedDiamond = 44
        '     Specifies a hatch that has the appearance of diagonally layered shingles that
        '     slant to the right from top points to bottom points.
        Shingle = 45
        '     Specifies a hatch that has the appearance of a trellis.
        Trellis = 46
        '     Specifies a hatch that has the appearance of spheres laid adjacent to one another.
        Sphere = 47
        '     Specifies horizontal And vertical lines that cross And are spaced 50 percent
        '     closer together than hatch style System.Drawing.Drawing2D.HatchStyle.Cross.
        SmallGrid = 48
        '     Specifies a hatch that has the appearance of a checkerboard.
        SmallCheckerBoard = 49
        '     Specifies a hatch that has the appearance of a checkerboard with squares that
        '     are twice the size of System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard.
        LargeCheckerBoard = 50
        '     Specifies forward diagonal And backward diagonal lines that cross but are Not
        '     antialiased.
        OutlinedDiamond = 51
        '     Specifies a hatch that has the appearance of a checkerboard placed diagonally.
        SolidDiamond = 52
        '     Specifies the hatch style System.Drawing.Drawing2D.HatchStyle.Cross.
        LargeGrid = 4
        '     Specifies hatch style System.Drawing.Drawing2D.HatchStyle.Horizontal.
        Min = 0
        '     Specifies hatch style System.Drawing.Drawing2D.HatchStyle.SolidDiamond.
        Max = 4
    End Enum

    ''' <summary>
    ''' OS-independent colour container
    ''' </summary>
    Public Structure VisualColor
        Implements IEquatable(Of VisualColor)

        Public ReadOnly A As Byte
        Public ReadOnly R As Byte
        Public ReadOnly G As Byte
        Public ReadOnly B As Byte

        Public Sub New(a As Byte, r As Byte, g As Byte, b As Byte)
            Me.A = a : Me.R = r : Me.G = g : Me.B = b
        End Sub

        Public Sub New(r As Byte, g As Byte, b As Byte)
            Me.New(255, r, g, b)
        End Sub

        Public Shared Function FromHex(hex As String) As VisualColor
            If String.IsNullOrEmpty(hex) Then Return New VisualColor(0, 0, 0, 0)
            If hex(0) = "#"c Then hex = hex.Substring(1)
            Dim a As Byte = 255
            If hex.Length >= 8 Then
                a = Convert.ToByte(hex.Substring(0, 2), 16)
                hex = hex.Substring(2)
            End If
            Dim r = Convert.ToByte(hex.Substring(0, 2), 16)
            Dim g = Convert.ToByte(hex.Substring(2, 2), 16)
            Dim b = Convert.ToByte(hex.Substring(4, 2), 16)
            Return New VisualColor(a, r, g, b)
        End Function

        Public Function ToHex() As String
            Return $"#{A:X2}{R:X2}{G:X2}{B:X2}"
        End Function

        Public Shared Function FromArgb(r As Byte, g As Byte, b As Byte) As VisualColor
            Return New VisualColor(r, g, b)
        End Function

        Public Shared Function FromArgb(a As Byte, r As Byte, g As Byte, b As Byte) As VisualColor
            Return New VisualColor(a, r, g, b)
        End Function

        ' Integer components (0..255); clamps just in case
        Public Shared Function FromArgb(a As Integer, r As Integer, g As Integer, b As Integer) As VisualColor
            Return FromArgb(ClampByte(a), ClampByte(r), ClampByte(g), ClampByte(b))
        End Function

        ' Packed ARGB (0xAARRGGBB)
        Public Shared Function FromArgb(argb As Integer) As VisualColor
            Dim hasAlpha As Boolean = (argb And &HFF000000UI) <> 0UI
            If hasAlpha Then
                Dim a As Byte = CByte((argb >> 24) And &HFF)
                Dim r As Byte = CByte((argb >> 16) And &HFF)
                Dim g As Byte = CByte((argb >> 8) And &HFF)
                Dim b As Byte = CByte(argb And &HFF)
                Return FromArgb(a, r, g, b)
            Else
                ' Compat for old short/24-bit hex literals like &HB4C5, &H73E6, &H2546F0
                Dim r As Byte = CByte((argb >> 16) And &HFF)
                Dim g As Byte = CByte((argb >> 8) And &HFF)
                Dim b As Byte = CByte(argb And &HFF)
                Return FromArgb(r, g, b)
            End If
        End Function

        Public Shared Function FromArgb(a As Byte, rgb As VisualColor) As VisualColor
            Dim r As Byte = rgb.R
            Dim g As Byte = rgb.G
            Dim b As Byte = rgb.B
            Return FromArgb(a, r, g, b)
        End Function

        ' Convenience: RGB with implicit alpha=255
        Public Shared Function FromRgb(r As Byte, g As Byte, b As Byte) As VisualColor
            Return New VisualColor(r, g, b)
        End Function

        Private Shared Function ClampByte(v As Integer) As Byte
            If v < 0 Then Return 0
            If v > 255 Then Return 255
            Return CByte(v)
        End Function

        ' Equality operator
        Public Shared Operator =(left As VisualColor, right As VisualColor) As Boolean
            Return left.A = right.A AndAlso
               left.R = right.R AndAlso
               left.G = right.G AndAlso
               left.B = right.B
        End Operator

        ' Inequality operator
        Public Shared Operator <>(left As VisualColor, right As VisualColor) As Boolean
            Return Not (left = right)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return (CInt(A) << 24) Or (CInt(R) << 16) Or (CInt(G) << 8) Or CInt(B)
        End Function

        Public Overloads Function Equals(other As VisualColor) As Boolean _
        Implements IEquatable(Of VisualColor).Equals
            Return A = other.A AndAlso R = other.R AndAlso G = other.G AndAlso B = other.B
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is VisualColor AndAlso Equals(DirectCast(obj, VisualColor))
        End Function

    End Structure

End Namespace ' Auxillary
