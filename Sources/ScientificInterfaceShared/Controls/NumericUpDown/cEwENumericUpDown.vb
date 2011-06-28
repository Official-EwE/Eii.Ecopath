#Region " Imports "

Option Strict On
Imports System.Windows.Forms

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Blender-inspired extension of <see cref="NumericUpDown"/> that allows a
    ''' user to click the control and drag the mouse to decrease or
    ''' increase the control value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEwENumericUpDown
        Inherits NumericUpDown

#Region " Private vars "

        ''' <summary>Most recent mouse position while dragging.</summary>
        Private m_ptLast As Point = Nothing

#End Region ' Private vars

#Region " Control overrides "

        ''' <summary>
        ''' Mouse press override. Used to set the capture for possible dragging.
        ''' </summary>
        Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            ' Use control unaffected
            MyBase.OnMouseDown(e)
            Me.Capture = True
            Me.m_ptLast = Me.DistanceFromBounds(e.Location)
        End Sub

        ''' <summary>
        ''' Mouse move override. Changes the value of the control while dragging.
        ''' </summary>
        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Me.Capture And Not Me.ClientRectangle.Contains(e.Location) Then

                Dim ptCurr As Point = Me.DistanceFromBounds(e.Location)
                Dim dx As Integer = (ptCurr.X - Me.m_ptLast.X)
                Dim dy As Integer = (Me.m_ptLast.Y - ptCurr.Y)
                Dim sIncrement As Single = Me.Increment
                
                If My.Computer.Keyboard.CtrlKeyDown Then
                    sIncrement *= 10
                End If

                If My.Computer.Keyboard.ShiftKeyDown Then
                    sIncrement /= CSng(Math.Max(Math.Pow(10, Me.DecimalPlaces), 5))
                End If

                Dim sDist As Single = (dx + dy) * (dx + dy) * sIncrement * CSng(Math.Sign(dx + dy))
                Me.Value = Convert.ToDecimal(Math.Max(Me.Minimum, Math.Min(Me.Maximum, Me.Value + sDist)))

                ' Remember last point
                Me.m_ptLast = ptCurr
            Else
                MyBase.OnMouseMove(e)
            End If
        End Sub

        ''' <summary>
        ''' Mouse up override. Used to cancel mouse capture.
        ''' </summary>
        Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
            Me.Capture = False
            'Me.Cursor = Cursors.Default
            MyBase.OnMouseUp(e)
        End Sub

        Protected Function DistanceFromBounds(ByVal pt As Point) As Point
            Dim rc As Rectangle = Me.ClientRectangle
            Dim dx As Integer
            Dim dy As Integer

            If pt.X < 0 Then
                dx = pt.X
            ElseIf pt.X <= rc.Width Then
                dx = 0
            Else
                dx = pt.X - rc.Width
            End If

            If pt.Y < 0 Then
                dy = pt.Y
            ElseIf pt.Y <= rc.Height Then
                dy = 0
            Else
                dy = pt.Y - rc.Height
            End If
            Return New Point(dx, dy)

        End Function

#End Region ' Control overrides

    End Class

End Namespace
