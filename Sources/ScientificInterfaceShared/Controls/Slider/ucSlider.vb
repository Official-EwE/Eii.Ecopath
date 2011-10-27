Option Strict On

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Custom slider control that was whipped up 'cause the .NET TrackBar
    ''' is just too ugly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucSlider

        Private m_aValues() As Integer
        Private m_iKnobCurr As Integer = 0
        Private cKNOBSIZE As Integer = 10
        Private m_iValueMax As Integer = 100
        Private m_iValueMin As Integer = 0

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or _
                        ControlStyles.AllPaintingInWmPaint Or _
                        ControlStyles.ResizeRedraw, True)
            Me.NumKnobs = 1
        End Sub

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value for (a knob on) the slider.
        ''' </summary>
        ''' <param name="iIndex">Optional index for the knob to access.</param>
        ''' -------------------------------------------------------------------
        Public Property Value(Optional ByVal iIndex As Integer = 0) As Integer
            Get
                Return Me.m_aValues(iIndex)
            End Get
            Set(ByVal value As Integer)
                value = Math.Max(Me.Minimum, Math.Min(value, Me.Maximum))
                If (value <> Me.m_aValues(iIndex)) Then
                    Me.m_aValues(iIndex) = value
                    Me.Invalidate()
                    RaiseEvent ValueChanged(Me, New System.EventArgs())
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the minimum value that the slider can hold.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Minimum() As Integer
            Get
                Return Me.m_iValueMin
            End Get
            Set(ByVal value As Integer)
                Me.m_iValueMin = Math.Min(Me.Maximum - 1, value)
                Me.Value = Me.Value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the maximum value that the slider can hold.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Maximum() As Integer
            Get
                Return Me.m_iValueMax
            End Get
            Set(ByVal value As Integer)
                Me.m_iValueMax = Math.Max(Me.Minimum + 1, value)
                Me.Value = Me.Value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of knobs that the slider should show. By default
        ''' the slider displays one knob.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property NumKnobs As Integer
            Get
                Return Me.m_aValues.Length
            End Get
            Set(ByVal value As Integer)
                ReDim Preserve Me.m_aValues(Math.Max(1, value) - 1)
                Me.m_iKnobCurr = Math.Min(Me.m_iKnobCurr, Me.m_aValues.Length - 1)
                Me.Invalidate()
            End Set
        End Property

        Public Event ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

#End Region ' Public interfaces

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Filter useful input key presses for a slider.
        ''' </summary>
        ''' <param name="keyData">The key to validate.</param>
        ''' <returns>True if the given key should be considered an input key.</returns>
        ''' -------------------------------------------------------------------
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

        Protected Overrides Sub OnGotFocus(ByVal e As System.EventArgs)
            Me.Invalidate()
            MyBase.OnGotFocus(e)
        End Sub

        Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
            If e.KeyCode = Keys.Left Then Me.Value(Me.m_iKnobCurr) -= 1 : e.Handled = True : e.SuppressKeyPress = True
            If e.KeyCode = Keys.Right Then Me.Value(Me.m_iKnobCurr) += 1 : e.Handled = True : e.SuppressKeyPress = True
            MyBase.OnKeyDown(e)
        End Sub

        Protected Overrides Sub OnLostFocus(ByVal e As System.EventArgs)
            Me.Invalidate()
            MyBase.OnLostFocus(e)
        End Sub

        Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseDown(e)
            Me.Capture = True

            Dim iValue As Integer = Me.GetValueAtPoint(e.Location)
            Dim iDistNearest As Integer = Me.Maximum
            Dim iKnobNearest As Integer = -1

            ' Find nearest knob
            For i As Integer = 0 To Me.m_aValues.Length - 1
                Dim iDistTest As Integer = Math.Abs(iValue - Me.Value(i))
                If (iDistTest < iDistNearest) Then
                    iDistNearest = iDistTest
                    iKnobNearest = i
                End If
            Next

            Me.m_iKnobCurr = iKnobNearest
            Me.Value(Me.m_iKnobCurr) = iValue

        End Sub

        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseMove(e)
            If (Me.Capture = False) Then Return
            Me.Value(Me.m_iKnobCurr) = Me.GetValueAtPoint(e.Location)
        End Sub

        Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseUp(e)
            Me.Capture = False
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

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

            ' Draw knobs
            ' - Make sure current knob is positioned at the end of the list
            Dim lKnobIndexes As New List(Of Integer)
            For i As Integer = 0 To Me.NumKnobs - 1
                lKnobIndexes.Add(i)
            Next
            lKnobIndexes.Remove(Me.m_iKnobCurr)
            lKnobIndexes.Add(Me.m_iKnobCurr)

            For Each i As Integer In lKnobIndexes

                Dim iX0 As Integer = CInt((Me.Value(i) - Me.m_iValueMin) / Me.RenderScale()) + Me.Margin.Left
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

                ' - Body
                ' Is current selected knob?
                If (i = Me.m_iKnobCurr) And (Me.NumKnobs > 1) Then
                    ' #Yes: render with highlighted background
                    e.Graphics.FillPolygon(SystemBrushes.Highlight, aptKnobOutline)
                Else
                    ' #Yes: render as regular control
                    e.Graphics.FillPolygon(SystemBrushes.Control, aptKnobOutline)
                End If
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


            Next

        End Sub

        Protected Overrides Sub OnSizeChanged(ByVal e As System.EventArgs)
            Me.Invalidate()
            MyBase.OnSizeChanged(e)
        End Sub

#End Region ' Events

#Region " Internals "

        Private Function GetValueAtPoint(ByVal ptMouse As Point) As Integer
            Dim sMouseX As Single = CSng(Math.Max(0, Math.Min(Me.Width - cKNOBSIZE - Me.Margin.Left - Me.Margin.Right, ptMouse.X - cKNOBSIZE / 2)))
            Dim iValue As Integer = Me.Minimum + CInt(sMouseX * Me.RenderScale())
            Return iValue
        End Function

#End Region ' Internals

    End Class

End Namespace ' Controls
