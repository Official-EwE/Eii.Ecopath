Option Strict On
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Helper class, draws a colour ramp.
    ''' </summary>
    ''' ===========================================================================
    Public Class cColorRampIndicator

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Draws an arrow in the indicated rectangle with an indicated colour, 
        ''' at a given angle with a given relative size.
        ''' </summary>
        ''' <param name="g">Graphics to draw onto.</param>
        ''' <param name="ramp">The <see cref="cColorRamp"/> to draw.</param>
        ''' <param name="rc">Area to draw ramp onto.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub DrawColorRamp(ByVal g As Graphics, _
                                        ByVal ramp As cColorRamp, _
                                        ByVal rc As Rectangle)

            For i As Integer = 0 To rc.Width - 1
                Using p As New Pen(ramp.GetColor(i / rc.Width), 1)
                    g.DrawLine(p, rc.X + i, rc.Y, rc.X + i, rc.Y + rc.Height - 1)
                End Using
            Next

        End Sub

#End Region

    End Class

End Namespace
