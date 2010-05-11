#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.ComponentModel

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
            ' NOP
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the fixed <see cref="Label.BackColor">background color</see> of a 
        ''' cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
        Public Overrides Property BackColor() As System.Drawing.Color
            Get
                Return SystemColors.ButtonShadow
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
                    Return SystemColors.ButtonFace
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
                Return New Font(MyBase.Font, FontStyle.Bold)
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
                If Me.RightToLeft = Forms.RightToLeft.Yes Then
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
                Return Forms.BorderStyle.None
            End Get
            Set(ByVal value As System.Windows.Forms.BorderStyle)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the interited <see cref="Label.RightToLeft">reading order</see>
        ''' of a cEwEGroupLabel control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Browsable(False)> _
         Public Overrides Property RightToLeft() As System.Windows.Forms.RightToLeft
            Get
                Return Forms.RightToLeft.Inherit
            End Get
            Set(ByVal value As System.Windows.Forms.RightToLeft)
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

    End Class

End Namespace
