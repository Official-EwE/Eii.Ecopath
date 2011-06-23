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
            'Me.Cursor = Cursors.SizeAll
            Me.m_ptLast = e.Location
        End Sub

        ''' <summary>
        ''' Mouse move override. Changes the value of the control while dragging.
        ''' </summary>
        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Me.Capture Then

                Dim dx As Integer = (e.Location.X - Me.m_ptLast.X)
                Dim dy As Integer = (Me.m_ptLast.Y - e.Location.Y)
                Dim sDist As Single = (dx + dy) * Me.Increment

                If My.Computer.Keyboard.CtrlKeyDown Then
                    sDist *= 10
                End If
                If My.Computer.Keyboard.ShiftKeyDown Then
                    sDist /= CSng(Math.Pow(10, Me.DecimalPlaces))
                End If

                ' Remember last point
                Me.m_ptLast = e.Location

                Me.Value = Convert.ToDecimal(Math.Max(Me.Minimum, Math.Min(Me.Maximum, Me.Value + sDist)))
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

#End Region ' Control overrides

    End Class

End Namespace
