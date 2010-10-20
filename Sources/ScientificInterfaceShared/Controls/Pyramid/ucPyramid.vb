Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' User control for rendering one or more pyramids.
    ''' </summary>
    ''' <remarks>
    ''' Does not function yet; needs to be implemented.
    ''' </remarks>
    ''' =======================================================================
    Public Class ucPyramid

        Private m_pyramid As cPyramid = Nothing

        Public Enum eRenderModeTypes As Byte
            ''' <summary>Render two dimensional</summary>
            Render2D
            ''' <summary>Render three dimensional</summary>
            Render3D
        End Enum

        Public Property Pyramid() As cPyramid
            Get
                Return Me.m_pyramid
            End Get
            Set(ByVal value As cPyramid)
                Me.m_pyramid = value
            End Set
        End Property

        Public Sub Plot(ByVal g As Graphics, ByVal rc As Rectangle, ByVal sScale As Single)

            If (Me.m_pyramid Is Nothing) Then Return
            If (Me.m_pyramid.IsValid = False) Then Return

            'Dim sScale As Single = Math.Min(rc.Width / Me.m_sWidth, rc.Height / Me.m_sHeight)

            Dim ptTop As New Point(rc.Left + CInt(rc.Width / 2), rc.Top + CInt(rc.Height / 2 - (sScale * Me.m_pyramid.Height / 2)))
            Dim ptBL As New Point(rc.Left + CInt((rc.Width / 2) - (sScale * Me.m_pyramid.Width / 2)), rc.Top + CInt(rc.Height / 2 + (sScale * Me.m_pyramid.Height / 2)))
            Dim ptBR As New Point(rc.Left + CInt((rc.Width / 2) + (sScale * Me.m_pyramid.Width / 2)), rc.Top + CInt(rc.Height / 2 + (sScale * Me.m_pyramid.Height / 2)))

            ' Plot pyramid
            g.DrawLine(Pens.Black, ptTop, ptBR)
            g.DrawLine(Pens.Black, ptBR, ptBL)
            g.DrawLine(Pens.Black, ptBL, ptTop)

            ' Plot TLs
            For iTL As Integer = 1 To Me.m_pyramid.NumTL - 1
                Dim sYValue As Single = Me.m_pyramid.Height * Me.m_pyramid.Value(iTL)
                Dim ptL As New Point(ptBL.X, ptTop.Y + CInt(sScale * sYValue))
                Dim ptR As New Point(ptBR.X, ptL.Y)
                g.DrawLine(Pens.Red, ptL, ptR)
            Next

            ' Plot scale box
            Dim sVal As Single = CSng(Math.Pow(10, Math.Floor(Math.Log10(Me.m_pyramid.SumB))))
            Dim sScaleBoxSize As Single = CSng(Math.Sqrt(sVal) * sScale)

            While sScaleBoxSize > CSng(Math.Max(rc.Width, rc.Height) / 4)
                sVal /= 10.0!
                sScaleBoxSize /= 10.0!
            End While

            g.DrawRectangle(Pens.Black, rc.Left, rc.Top, sScaleBoxSize, sScaleBoxSize)
            g.DrawString(String.Format("{0} {1}", sVal, Me.m_pyramid.Units), SystemFonts.DialogFont, Brushes.Black, rc.Left, rc.Top + sScaleBoxSize)

        End Sub

    End Class

End Namespace
