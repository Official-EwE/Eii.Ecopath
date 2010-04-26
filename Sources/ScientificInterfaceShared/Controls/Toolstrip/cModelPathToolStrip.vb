#Region " Imports "

Option Strict On
Imports System.Text

#End Region ' Imports

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Toolstrip displaying the EwE model path in the toolstrip background.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class does not support rendering in vertical toolstrips.</para>
    ''' <para>This class supports right-to-left reading orders.</para>
    ''' </remarks>
    ''' ===========================================================================
    Public Class cModelPathToolStrip
        Inherits cEwEToolstrip

#Region " Private vars "

        ''' <summary>Path text to display.</summary>
        Private m_strPath As String = ""
        ''' <summary>Flags for displaying label</summary>
        Private m_sfmt As New StringFormat(StringFormatFlags.NoWrap Or StringFormatFlags.FitBlackBox Or StringFormatFlags.LineLimit)
        ''' <summary>Area (in toolstrip client coordinates) for displaying path.</summary>
        Private m_rcLabel As Rectangle = Nothing
        ''' <summary>Formatted path text to display.</summary>
        Private m_strLabel As String = ""
        ''' <summary>Flag stating whether the mouse was last hovering over the path label.</summary>
        Private m_bLabelHover As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            MyBase.New()
            Me.m_sfmt.LineAlignment = StringAlignment.Center
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event to let the world know that the path label area was clicked. I mean,
        ''' come on, you WANT to know such things, no?
        ''' </summary>
        ''' <param name="sender">Sender of the event.</param>
        ''' <param name="e">Event parameters.</param>
        ''' -----------------------------------------------------------------------
        Public Event OnPathAreaClicked(ByVal sender As Object, ByVal e As EventArgs)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the path text to display.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Path() As String
            Get
                Return Me.m_strPath
            End Get
            Set(ByVal strPath As String)
                Me.m_strPath = strPath
                Me.ResetText()
            End Set
        End Property

#End Region ' Public access

#Region " Event overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to send a path area click event.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnMouseClick(ByVal e As MouseEventArgs)
            MyBase.OnMouseClick(e)
            If Me.IsLabelHover() Then
                RaiseEvent OnPathAreaClicked(Me, New EventArgs())
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to track label mouse hover.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnMouseMove(ByVal mea As MouseEventArgs)
            MyBase.OnMouseMove(mea)
            ' Detect if mouse is over label
            Dim bLabelHover As Boolean = Me.m_rcLabel.Contains(Me.PointToClient(MousePosition))
            If (bLabelHover <> Me.m_bLabelHover) Then
                Me.IsLabelHover = bLabelHover
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to turn off mouse hover feedback.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
            MyBase.OnMouseLeave(e)
            Me.IsLabelHover = False
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render the path text onto the available control background.
        ''' </summary>
        ''' <remarks>
        ''' This will recalculate the label text and placement if necessary.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim br As Brush = Nothing

            If String.IsNullOrEmpty(Me.m_strPath) Then Return

            If String.IsNullOrEmpty(Me.m_strLabel) Then
                Me.RecalculateLabelTextAndPlacement()
            End If

            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                Me.m_sfmt.Alignment = StringAlignment.Near
            Else
                Me.m_sfmt.Alignment = StringAlignment.Far
            End If

            If (Me.m_rcLabel.Width > 0) Then
                br = DirectCast(IIf(Me.IsLabelHover, SystemBrushes.ControlText, SystemBrushes.ControlLight), Brush)
                e.Graphics.DrawString(Me.m_strLabel, Me.Font, br, Me.m_rcLabel, Me.m_sfmt)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Toolstrip has been resized; overridden to force the path text to be 
        ''' recalculated next time it will be rendered.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            Me.ResetText()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Available background space may have changed; overridden to force the path 
        ''' text to be recalculated next time it will be rendered.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnControlAdded(ByVal e As ControlEventArgs)
            MyBase.OnControlAdded(e)
            Me.ResetText()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Available background space may have changed; overridden to force the path 
        ''' text to be recalculated next time it will be rendered.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnControlremoved(ByVal e As ControlEventArgs)
            MyBase.OnControlAdded(e)
            Me.ResetText()
        End Sub

#End Region ' Event overrides

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to reset the formatted label. Also invalidates the control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub ResetText()
            'MyBase.ResetText() ' Do not call base version
            Me.m_strLabel = ""
            Me.Invalidate()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Recalculate label text and placement.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub RecalculateLabelTextAndPlacement()

            Dim iMin As Integer = 0
            Dim iMax As Integer = Me.Width
            Dim strTemp As String = String.Copy(Me.m_strPath)
            Dim sbTemp As New StringBuilder

            For Each tsi As ToolStripItem In Me.Items
                If Not tsi.IsOnOverflow And tsi.Available Then
                    If tsi.Alignment = ToolStripItemAlignment.Left Then
                        iMin = Math.Max(tsi.Bounds.Right, iMin)
                    Else
                        iMax = Math.Min(tsi.Bounds.Left, iMax)
                    End If
                End If
            Next

            ' Calc conservative rect
            Me.m_rcLabel = New Rectangle(iMin + 2, 2, iMax - iMin - 4, Me.ClientRectangle.Height - 4)

            If (Me.m_rcLabel.Width > 0) Then
                ' -10 to counter odd calculation effects that I do not understand
                '   Issues seem font based, but no clue as to why the last few chars are sometimes 
                '   not properly included in the label calculations
                TextRenderer.MeasureText(strTemp, Me.Font, New Size(Me.m_rcLabel.Width - 10, Me.m_rcLabel.Height), _
                                         TextFormatFlags.Internal Or _
                                         TextFormatFlags.PathEllipsis Or _
                                         TextFormatFlags.ModifyString)

                ' Chop off Nothing characters which will occur when string is shortened.
                '   These chars are recognized and handled well by the String class, but 
                '   Graphics.DrawString may still render such chars and characters beyond it.
                For Each c As Char In strTemp
                    If c = Nothing Then
                        Exit For
                    End If
                    sbTemp.Append(c)
                Next
            End If

            Me.m_strLabel = sbTemp.ToString

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether to display label hover feedback.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property IsLabelHover() As Boolean
            Get
                Return Me.m_bLabelHover
            End Get
            Set(ByVal value As Boolean)
                If (value = Me.m_bLabelHover) Then Return
                Me.m_bLabelHover = value
                Me.Invalidate()
            End Set
        End Property

#End Region ' Internals

    End Class

End Namespace
