Option Strict On
Imports System.Drawing

Namespace Style

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' <para>This class implements a gradient across a number of colours.</para>
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cColorRamp

        ''' <summary>Colour offset start [0..1]</summary>
        Private m_sColorOffsetStart As Single = 0.0
        ''' <summary>Colour offset end [0..1]</summary>
        Private m_sColorOffsetEnd As Single = 1.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the color start offset, which enables selective use of the color ramp without having to modify the ramp.
        ''' </summary>
        ''' <remarks>
        ''' If the start offset exceeds than the end offset, the entire color scheme is reversed.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ColorOffsetStart() As Single
            Get
                Return Me.m_sColorOffsetStart
            End Get
            Set(ByVal s As Single)
                Me.m_sColorOffsetStart = s
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the color end offset, which enables selective use of the color ramp without having to modify the ramp.
        ''' </summary>
        ''' <remarks>
        ''' If the start offset exceeds than the end offset, the entire color scheme is reversed.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property ColorOffsetEnd() As Single
            Get
                Return Me.m_sColorOffsetEnd
            End Get
            Set(ByVal s As Single)
                Me.m_sColorOffsetEnd = s
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a colour for a given value from the ramp.
        ''' </summary>
        ''' <param name="dValue">The value to calculate the colour for. Typically, this variable will range from [0..1]</param>
        ''' <param name="dValueMax">The maximum to scale the value to.</param>
        ''' <returns>The colour for the given value.</returns>
        ''' <remarks>Override this method to implement a specific ColorRamp.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetColor(ByVal dValue As Double, Optional ByVal dValueMax As Double = 1.0) As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recalculates a colour lookup value by applying ColorOffsets.
        ''' </summary>
        ''' <param name="dValue"></param>
        ''' <param name="dValueMax"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function RecalcValue(ByVal dValue As Double, ByVal dValueMax As Double) As Double

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

    End Class

End Namespace
