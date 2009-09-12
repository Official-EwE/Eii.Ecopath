Option Strict On
Imports System.Drawing.Drawing2D

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Helper class, draws an arrow on a designated 
    ''' </summary>
    ''' ===========================================================================
    Public Class cArrowIndicator

#Region " Private vars "

        ''' <summary>Graphics path holding the entire arrow</summary>
        Private m_gpArrow As GraphicsPath

#End Region ' Private vars

#Region " Singleton "

        Private Shared __inst__ As cArrowIndicator

        Private Shared Function GetInstance() As cArrowIndicator
            If __inst__ Is Nothing Then __inst__ = New cArrowIndicator()
            Return __inst__
        End Function

        Private Sub New()
            Me.Init()
        End Sub

#End Region ' Singleton

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Draws an arrow in the indicated rectangle with an indicated colour, 
        ''' at a given angle with a given relative size.
        ''' </summary>
        ''' <param name="g">Graphics to draw onto.</param>
        ''' <param name="clr">Colour of the arrow to draw.</param>
        ''' <param name="rc">Rectangle to draw the arrow into.</param>
        ''' <param name="sAngle">Clockwise angle for the arrow. 0 is straight up.</param>
        ''' <param name="sSize">Size of the angle. [0, 1], 0 is smallest size, 1 will
        ''' size the arrow to optimally fit in the rectangle with 1 pixel margin.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub DrawArrow(ByVal g As Graphics, ByVal clr As Color, ByVal rc As Rectangle, ByVal sAngle As Single, ByVal sSize As Single)

            Dim matOrg As Matrix = g.Transform
            Dim matArr As New Matrix()
            ' Arrow is 10 px high. Scale with 1 px all around 
            Dim sScale As Single = CSng(Math.Min(rc.Width / 12, rc.Height / 12)) * sSize
            Dim sDX As Single = CSng(Math.Max((rc.Width - rc.Height) / 2, 0))
            Dim sDY As Single = CSng(Math.Max((rc.Height - rc.Width) / 2, 0))

            ' Anything to draw?
            If (sSize > 0) Then

                ' #Yes: Prepare transformation matrix
                ' - Scale arrow to fit rect
                matArr.Scale(sScale, sScale)
                ' - Move arrow to center of rect
                matArr.Translate((6 / sSize) + ((rc.X + sDX) / sScale), (6 / sSize) + ((rc.Y + sDY) / sScale))
                ' - Rotate arrow to given value
                matArr.Rotate(sAngle)

                ' Apply arrow transformation matrix
                g.Transform = matArr
                ' Draw arrow in requested color
                Using p As New Pen(clr)
                    g.DrawPath(p, GetInstance().m_gpArrow)
                End Using
                ' Clean up borrowed DC by restoring original transformation matrix
                g.Transform = matOrg

            End If

        End Sub

#End Region ' Public access

#Region " Internal implementation "

        Private Sub Init()

            Dim gpCap As New GraphicsPath()
            Dim gpArrow As New GraphicsPath()

            '     (-5,0)
            '       /|\  
            '(-2,-3) | (2,-3)
            '      (0,0)
            '        |
            '        |
            '      (5,0)

            ' Create cap
            gpCap.AddLine(-2, -3, 0, -5)
            gpCap.AddLine(2, -3, 0, -5)
            ' Create body
            gpArrow.AddLine(0, -5, 0, 5)
            ' Add cap
            gpArrow.AddPath(gpCap, False)

            ' Keep
            m_gpArrow = gpArrow

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace ' Controls
