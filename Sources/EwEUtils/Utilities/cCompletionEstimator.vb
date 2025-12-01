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
