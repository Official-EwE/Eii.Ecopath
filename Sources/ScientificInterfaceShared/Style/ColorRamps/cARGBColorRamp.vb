Option Strict On
Imports System.Drawing

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

        Private m_aclr() As Color ' Colors for steps 
        Private m_adPositions() As Double ' Step pos

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the ARGBColorRamp class.
        ''' </summary>
        ''' <param name="aColors">The colour gradients to use.</param>
        ''' <param name="adPositions">The position of each colour gradient, 
        ''' relative to its predessesor.</param>
        ''' <param name="dScale">The factor to which to scale the positions. This
        ''' value cannot be 0.</param>
        ''' <remarks>
        ''' The following snippet illustrates how to create a valid ARGB color ramp:
        ''' <code>
        ''' ' Define a three level colour ramp
        ''' Dim aclr(2) as Color
        ''' Dim adPositions(2) as Integer
        ''' 
        ''' ' Ramp begins with light blue at position 0
        ''' aclr(0) = Color.FromARGB(255, 200, 200, 255)
        ''' adPositions(0) = 0
        ''' ' At 40%, the ramp is a green tone
        ''' aclr(1) = Color.FromARGB(255, 100, 255, 100)
        ''' adPositions(1) = 0.4
        ''' ' At 100% (0.4 + 0.6) the ramp is deep red
        ''' aclr(2) = Color.FromARGB(255, 255, 25, 25)
        ''' adPositions(2) = 0.6
        ''' 
        ''' ' Create the ramp
        ''' Dim crARGB as New ARGBColorRamp(aclr, adPositions)
        ''' 
        ''' ' Now get the value at 50%, let's see what happens...
        ''' Dim clr as Color = crARGB.GetColor(0.5)
        ''' </code>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal aColors() As Color, ByVal adPositions() As Double, Optional ByVal dScale As Double = 1.0#)

            MyBase.New()

            Dim clr As Color = Nothing
            Dim dTotalPos As Double = 0.0

            ' Validate input
            If (aColors Is Nothing) Then Throw New Exception("Missing required parameter aColors")
            If (adPositions Is Nothing) Then Throw New Exception("Missing required parameter adPositions")
            If (aColors.Length <> adPositions.Length) Then Throw New Exception("Number of colors and positions do not match")
            If (dScale <= 0.0#) Then Throw New Exception("Scaling factor must be greater than 0")

            ReDim Me.m_aclr(adPositions.Length - 1)
            ReDim Me.m_adPositions(adPositions.Length - 1)

            For nPos As Integer = 0 To adPositions.Length - 1
                dTotalPos += CDbl(Math.Abs(adPositions(nPos)))
                Me.m_aclr(nPos) = aColors(nPos)
                Me.m_adPositions(nPos) = dTotalPos / dScale
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return an ARGB colour for a given value.
        ''' </summary>
        ''' <param name="dValue">The value to return the colour for.</param>
        ''' <param name="dValueMax">The maximum value to scale the value to. By default, it is assumed that a colour must be retrieved on a scale from [0..1]</param>
        ''' <returns>The colour for a given value.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetColor(ByVal dValue As Double, Optional ByVal dValueMax As Double = 1.0) As Color

            ' Normalize nValue to nValueMax
            Dim nIndex As Integer = 0
            Dim bFound As Boolean = False

            ' Apply color offsets
            dValue = Me.RecalcValue(dValue, dValueMax)
            dValueMax = 1.0

            ' Find first index
            bFound = (dValue <= Me.m_adPositions(0))
            While Not bFound
                nIndex += 1
                bFound = (nIndex = Me.m_adPositions.Length)
                If Not bFound Then
                    bFound = (dValue <= Me.m_adPositions(nIndex))
                End If
            End While

            ' Below first level? Return first colour without interpolating
            If (nIndex = 0) Then Return Me.m_aclr(0)
            ' Past last level? Return formar-last level without interpolating
            If (nIndex = Me.m_adPositions.Length) Then Return Me.m_aclr(nIndex - 1)
            ' Exactly at a known level? Return the level colour withour interpolating
            If dValue = Me.m_adPositions(nIndex) Then Return Me.m_aclr(nIndex)

            ' must interpolate
            Dim c1 As Color = Me.m_aclr(nIndex - 1)
            Dim c2 As Color = Me.m_aclr(nIndex)
            Dim dX As Double = Me.m_adPositions(nIndex) - Me.m_adPositions(nIndex - 1)
            Dim dPosX As Double = dValue - Me.m_adPositions(nIndex - 1)

            Dim dRatio As Double = (dPosX / dX)

            If (dRatio > 1.0) Then
                dRatio = 1.0
            End If

            Return Color.FromArgb(Me.Interpolate(c1.A, c2.A, dRatio), _
                                  Me.Interpolate(c1.R, c2.R, dRatio), _
                                  Me.Interpolate(c1.G, c2.G, dRatio), _
                                  Me.Interpolate(c1.B, c2.B, dRatio))

        End Function

        Private Function Interpolate(ByVal nVal1 As Integer, ByVal nVal2 As Integer, ByVal dRatio As Double) As Integer
            Try
                Return CInt(Math.Round(nVal1 + (nVal2 - nVal1) * dRatio))
            Catch ex As Exception
                Return 0
            End Try
        End Function

        ''' <summary>
        ''' Get/set the colours to use for every <see cref="GradientBreaks">gradient stop</see>.
        ''' </summary>
        Public Property GradientColors As Color()
            Get
                Return Me.m_aclr
            End Get
            Set(ByVal value As Color())
                Me.m_aclr = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the offset to use for every <see cref="GradientColors">gradient color</see>.
        ''' </summary>
        ''' <remarks>
        ''' Note that every offset must be specified as the distance from the previous offset.
        ''' </remarks>
        Public Property GradientBreaks() As Double()
            Get
                Return Me.m_adPositions
            End Get
            Set(ByVal value As Double())
                Me.m_adPositions = value
            End Set
        End Property

    End Class

End Namespace
