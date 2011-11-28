#Region " Imports "

Option Strict On
Imports System.Globalization
Imports System.Threading

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory class for customizing cursors.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cCursorFactory

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a custom cursor, combining a given cursor and an overlay image.
        ''' </summary>
        ''' <param name="crsBase"></param>
        ''' <param name="imgAdd"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetCursorOverlay(ByVal crsBase As Cursor, imgAdd As Image) As Cursor

            If (imgAdd Is Nothing) Then Return crsBase

            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture

            ' Hotspot always at center of new cursor. Not handy!
            Dim rcBase As New Rectangle(0, 0, crsBase.Size.Width, crsBase.Size.Height)
            Dim ptOffset As Point = New Point(rcBase.Width - crsBase.HotSpot.X, rcBase.Height - crsBase.HotSpot.Y)
            rcBase.Offset(ptOffset)

            Dim crsOut As Cursor = Nothing
            Dim rcOut As New Rectangle(0, 0, 2 * CInt(Math.Max(rcBase.Width, rcBase.Width / 2 + imgAdd.Width)), 2 * CInt(Math.Max(rcBase.Height, rcBase.Height / 2 + imgAdd.Height)))
            Dim bmp As New Bitmap(rcOut.Width, rcOut.Height, Imaging.PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(bmp)
                ' Draw cursor, positioned at hotspot
                crsBase.Draw(g, rcBase)
                ' ToDo: this is hack, need to properly position overlay
                If ci.TextInfo.IsRightToLeft Then
                    g.DrawImage(imgAdd, New Rectangle(0, CInt(rcOut.Height - imgAdd.Height - 1), imgAdd.Width, imgAdd.Height))
                Else
                    g.DrawImage(imgAdd, New Rectangle(CInt(rcOut.Width - imgAdd.Width - 1), CInt(rcOut.Height - imgAdd.Height - 1), imgAdd.Width, imgAdd.Height))
                End If
            End Using

            crsOut = New Cursor(bmp.GetHicon())
            bmp.Dispose()

            Return crsOut

        End Function

    End Class

End Namespace

