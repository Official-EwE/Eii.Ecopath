Option Strict On
Imports ScientificInterfaceShared.Style
Imports System.Drawing.Imaging

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
                                        ByVal rc As Rectangle, _
                                        Optional ByVal bHorizontal As Boolean = True)

            If (ramp Is Nothing) Then Return

            Dim bmp As New Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb)
            Dim gtmp As Graphics = Graphics.FromImage(bmp)

            If bHorizontal Then
                For i As Integer = 0 To rc.Width - 1
                    Using p As New Pen(ramp.GetColor(i / rc.Width), 1)
                        gtmp.DrawLine(p, i, 0, i, rc.Height - 1)
                    End Using
                Next
            Else
                For i As Integer = 0 To rc.Height - 1
                    Using p As New Pen(ramp.GetColor(i / rc.Height), 1)
                        gtmp.DrawLine(p, 0, rc.Height - i, rc.Width - 1, rc.Height - i)
                    End Using
                Next
            End If

            g.DrawImage(bmp, rc.X, rc.Y)

            gtmp.Dispose()
            bmp.Dispose()

        End Sub

#End Region

    End Class

End Namespace
