#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Globalization
Imports System.Threading
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Label control for showing header labels on EwE forms.
    ''' </summary>
    ''' <remarks>
    ''' This control overrides a series of visual properties to style a standard
    ''' label control as a EwE header label.
    ''' </remarks>
    ''' ===========================================================================
    Public Class cEwEHeaderLabel
        Inherits Label

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            Me.Text = "Header"
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="Label.DefaultSize">default size</see> for a new
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides ReadOnly Property DefaultSize() As System.Drawing.Size
            Get
                Return New Size(100, 18)
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.BackColor">background color</see> of a 
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property BackColor() As System.Drawing.Color
            Get
                Return SystemColors.ActiveCaption
            End Get
            Set(ByVal value As System.Drawing.Color)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.ForeColor">foreground color</see> of a 
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property ForeColor() As System.Drawing.Color
            Get
                If Me.Enabled Then
                    Return SystemColors.ActiveCaptionText
                Else
                    Return SystemColors.InactiveCaptionText
                End If
            End Get
            Set(ByVal value As System.Drawing.Color)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.Font">font</see> of a cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property Font() As System.Drawing.Font
            Get
                Return MyBase.Font
            End Get
            Set(ByVal value As System.Drawing.Font)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.AutoSize">auto size</see> behaviour of a 
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property AutoSize() As Boolean
            Get
                Return False
            End Get
            Set(ByVal value As Boolean)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.TextAlign">text alignment</see> behaviour 
        ''' of a cEwEGroupLabel control.
        ''' </summary>
        ''' <remarks>
        ''' This property takes the <see cref="RightToLeft">reading order</see> into
        ''' consideration.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property TextAlign() As System.Drawing.ContentAlignment
            Get
                If Me.RightToLeft = RightToLeft.Yes Then
                    Return ContentAlignment.MiddleRight
                Else
                    Return ContentAlignment.MiddleLeft
                End If
            End Get
            Set(ByVal value As System.Drawing.ContentAlignment)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.BorderStyle">border style</see> of a 
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property BorderStyle() As System.Windows.Forms.BorderStyle
            Get
                Return BorderStyle.None
            End Get
            Set(ByVal value As System.Windows.Forms.BorderStyle)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="Label.PreferredHeight">preferred height</see> of a 
        ''' new cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PreferredHeight() As Integer
            Get
                Return 18
            End Get
        End Property

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Doodledidoodle.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

            Dim rcText As Rectangle = Me.ClientRectangle
            Dim rcImage As Rectangle = Me.ClientRectangle
            Dim bRightToLeft As Boolean = False
            Dim fmt As StringFormat = cDrawingUtils.ContentAlignmentToStringFormat(Me.TextAlign)

            Select Case Me.RightToLeft
                Case RightToLeft.Inherit
                    ' ToDo: use style guide here somehow
                    bRightToLeft = Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft
                Case RightToLeft.Yes
                    bRightToLeft = True
                Case RightToLeft.No
                    bRightToLeft = False
                Case Else
                    ' Huh?!
                    Debug.Assert(False)
            End Select

            Using br As New SolidBrush(Me.BackColor)
                e.Graphics.FillRectangle(br, Me.ClientRectangle)
            End Using

            If (Me.Image IsNot Nothing) Then
                rcImage.Width = Math.Min(Image.Width, Me.ClientRectangle.Width - Me.Padding.Horizontal)
                rcText.Width -= rcImage.Width

                If (bRightToLeft) Then
                    rcImage.X += (rcText.Width + Me.Padding.Horizontal)
                Else
                    rcText.X += (rcImage.Width + Me.Padding.Horizontal)
                End If
                Me.DrawImage(e.Graphics, Me.Image, rcImage, Me.ImageAlign)
            End If

            Using br As New SolidBrush(Me.ForeColor)
                fmt.Trimming = StringTrimming.None
                e.Graphics.DrawString(Me.Text, Me.Font, br, rcText, fmt)
            End Using

        End Sub

#End Region ' Internals

    End Class

End Namespace
