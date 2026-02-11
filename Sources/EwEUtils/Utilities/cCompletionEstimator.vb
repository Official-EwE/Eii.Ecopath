' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Utilities

    ''' <summary>
    ''' A utility class to estimate the completion of a long-term simulation
    ''' </summary>
    Public Class cCompletionEstimator

        Private ReadOnly TimestepStart As Integer = 0
        Private ReadOnly Timesteps As Integer = 0
        Private ReadOnly Timer As New Stopwatch()

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="iTimestepStart"></param>
        ''' <param name="nTimesteps"></param>
        Public Sub New(iTimestepStart As Integer, nTimesteps As Integer)
            Me.TimestepStart = iTimestepStart
            Me.Timesteps = nTimesteps
            Me.Timer.Start()
        End Sub

        Public Function ETA(iTimestepNow As Integer) As DateTime

            Dim timeNow = Date.Now
            iTimestepNow = Math.Max(TimestepStart, Math.Min(Timesteps, iTimestepNow))

            Dim dTimeFractionElapsed As Double = Math.Ceiling(Me.Timer.Elapsed.TotalSeconds) / Math.Max(1, (iTimestepNow - Me.TimestepStart))
            Dim lSecondsRemaining As Long = CLng(dTimeFractionElapsed * (Me.Timesteps - iTimestepNow))

            Return timeNow.AddSeconds(lSecondsRemaining)

        End Function

    End Class

End Namespace
