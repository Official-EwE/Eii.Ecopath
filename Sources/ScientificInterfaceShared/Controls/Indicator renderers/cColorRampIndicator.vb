' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing.Imaging
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
        Public Shared Sub DrawColorRamp(g As Graphics,
                                        ramp As cColorRamp,
                                        rc As RectangleF,
                                        Optional bHorizontal As Boolean = True)

            If (ramp Is Nothing) Then Return
            If (rc.Width <= 0) Or (rc.Height <= 0) Then Return

            Dim bmp As New Bitmap(CInt(rc.Width), CInt(rc.Height), PixelFormat.Format32bppArgb)
            Dim gtmp As Graphics = Graphics.FromImage(bmp)

            If bHorizontal Then
                For i As Integer = 0 To CInt(rc.Width)
                    Using p As New Pen(ramp.GetColor(i / rc.Width), 1)
                        gtmp.DrawLine(p, i, 0, i, rc.Height - 1)
                    End Using
                Next
            Else
                For i As Integer = 0 To CInt(rc.Height)
                    Using p As New Pen(ramp.GetColor(i / rc.Height), 1)
                        gtmp.DrawLine(p, 0, rc.Height - i, rc.Width - 1, rc.Height - i)
                    End Using
                Next
            End If

            g.DrawImage(bmp, rc.X, rc.Y, rc.Width, rc.Height)

            gtmp.Dispose()
            bmp.Dispose()

        End Sub

#End Region

    End Class

End Namespace
