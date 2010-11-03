Option Strict On

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Custom slider control that was whipped up 'cause the .NET TrackBar
    ''' is just too ugly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucSlider

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or _
                        ControlStyles.AllPaintingInWmPaint Or _
                        ControlStyles.ResizeRedraw, True)
        End Sub

#Region " Public interfaces "

        Public Property Value() As Integer
            Get
                Return Me.m_iValue
            End Get
            Set(ByVal value As Integer)
                value = Math.Max(Me.Minimum, Math.Min(value, Me.Maximum))
                If (value <> Me.m_iValue) Then
                    Me.m_iValue = value
                    Me.Invalidate()
                    RaiseEvent ValueChanged(Me, New System.EventArgs())
                End If
            End Set
        End Property

        Public Property Minimum() As Integer
            Get
                Return Me.m_iValueMin
            End Get
            Set(ByVal value As Integer)
                Me.m_iValueMin = Math.Min(Me.Maximum - 1, value)
                Me.Value = Me.Value
            End Set
        End Property

        Public Property Maximum() As Integer
            Get
                Return Me.m_iValueMax
            End Get
            Set(ByVal value As Integer)
                Me.m_iValueMax = Math.Max(Me.Minimum + 1, value)
                Me.Value = Me.Value
            End Set
        End Property

        Public Event ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

#End Region ' Public interfaces

#Region " Internals "

        Private cKNOBSIZE As Integer = 10
        Private m_iValue As Integer = 50
        Private m_iValueMax As Integer = 100
        Private m_iValueMin As Integer = 0

        Protected Overrides Function IsInputKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean
            Select Case keyData
                Case Keys.Left, Keys.Right, Keys.Up, Keys.Down
                    Return True
            End Select
            Return MyBase.IsInputKey(keyData)
        End Function

        Protected Function RenderScale() As Single
            Return CSng((Me.Maximum - Me.Minimum) / Math.Max(1, Me.Width - cKNOBSIZE - Me.Margin.Left - Me.Margin.Right))
        End Function

#End Region ' Internals 

#Region " Events "

        Private Sub ucSlider_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.GotFocus
            Me.Invalidate()
        End Sub

        Private Sub ucSlider_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.Left Then Me.Value -= 1 : e.Handled = True : e.SuppressKeyPress = True
            If e.KeyCode = Keys.Right Then Me.Value += 1 : e.Handled = True : e.SuppressKeyPress = True
        End Sub

        Private Sub ucSlider_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LostFocus
            Me.Invalidate()
        End Sub

        Private Sub ucSlider_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
            Me.Capture = True
            Me.ProcessMousePos(e.Location)
        End Sub

        Private Sub ucSlider_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
            If (Me.Capture = False) Then Return
            Me.ProcessMousePos(e.Location)
        End Sub

        Private Sub ucSlider_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
            Me.Capture = False
        End Sub

        Private Sub ucSlider_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

            Dim iX0 As Integer = CInt((Me.m_iValue - Me.m_iValueMin) / Me.RenderScale()) + Me.Margin.Left
            Dim aptKnobOutline(5) As Point

            '    2
            ' 1 / \ 3
            '  |___|
            ' 0     4
            aptKnobOutline(0) = New Point(iX0, 14)
            aptKnobOutline(1) = New Point(iX0, 8)
            aptKnobOutline(2) = New Point(iX0 + CInt(cKNOBSIZE / 2), CInt(8 - cKNOBSIZE / 2))
            aptKnobOutline(3) = New Point(iX0 + cKNOBSIZE, 8)
            aptKnobOutline(4) = New Point(iX0 + cKNOBSIZE, 14)
            aptKnobOutline(5) = aptKnobOutline(0)

            ' Draw background
            ' - Eradicate!
            e.Graphics.FillRectangle(New SolidBrush(Me.BackColor), e.ClipRectangle)
            ' - Focus rect
            If Me.Focused Then ControlPaint.DrawFocusRectangle(e.Graphics, e.ClipRectangle)

            ' Draw track
            e.Graphics.DrawLine(SystemPens.ControlDark, CInt(Me.Margin.Left + cKNOBSIZE / 2), 9, CInt(Me.Width - Me.Margin.Right - cKNOBSIZE / 2), 9)
            e.Graphics.DrawLine(SystemPens.ControlDarkDark, CInt(Me.Margin.Left + cKNOBSIZE / 2), 10, CInt(Me.Width - Me.Margin.Right - cKNOBSIZE / 2), 10)
            e.Graphics.DrawLine(SystemPens.ControlLight, CInt(Me.Margin.Left + cKNOBSIZE / 2), 11, CInt(Me.Width - Me.Margin.Right - cKNOBSIZE / 2), 11)
            e.Graphics.DrawLine(SystemPens.ControlLightLight, CInt(Me.Margin.Left + cKNOBSIZE / 2), 12, CInt(Me.Width - Me.Margin.Right - cKNOBSIZE / 2), 12)

            ' Draw knob
            ' - Body
            e.Graphics.FillPolygon(SystemBrushes.Control, aptKnobOutline)
            ' - Outline
            e.Graphics.DrawLine(SystemPens.ControlLightLight, aptKnobOutline(0), aptKnobOutline(1))
            e.Graphics.DrawLine(SystemPens.ControlLightLight, aptKnobOutline(1), aptKnobOutline(2))
            e.Graphics.DrawLine(SystemPens.ControlDarkDark, aptKnobOutline(2), aptKnobOutline(3))
            e.Graphics.DrawLine(SystemPens.ControlDarkDark, aptKnobOutline(3), aptKnobOutline(4))
            e.Graphics.DrawLine(SystemPens.ControlDarkDark, aptKnobOutline(4), aptKnobOutline(0))
            ' - Fancy bits
            aptKnobOutline(2).Y += 1
            aptKnobOutline(3).X -= 1
            aptKnobOutline(4).X -= 1 : aptKnobOutline(4).Y -= 1
            aptKnobOutline(0).X += 1 : aptKnobOutline(0).Y -= 1
            e.Graphics.DrawLine(SystemPens.ControlDark, aptKnobOutline(2), aptKnobOutline(3))
            e.Graphics.DrawLine(SystemPens.ControlDark, aptKnobOutline(3), aptKnobOutline(4))
            e.Graphics.DrawLine(SystemPens.ControlDark, aptKnobOutline(4), aptKnobOutline(0))

        End Sub

        Private Sub ucSlider_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            Me.Invalidate()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub ProcessMousePos(ByVal ptMouse As Point)
            Dim sMouseX As Single = CSng(Math.Max(0, Math.Min(Me.Width - cKNOBSIZE - Me.Margin.Left - Me.Margin.Right, ptMouse.X - cKNOBSIZE / 2)))
            Me.Value = Me.Minimum + CInt(sMouseX * Me.RenderScale())
        End Sub
#End Region ' Internals

    End Class

End Namespace ' Controls
