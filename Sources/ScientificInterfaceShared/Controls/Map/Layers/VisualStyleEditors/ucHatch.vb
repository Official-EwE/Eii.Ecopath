' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing.Drawing2D

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Control managing a single <see cref="HatchStyle">hatch pattern</see>
    ''' in a <see cref="ucHatchSelect">Hatch select control</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucHatch

        Private m_hbs As HatchStyle = HatchStyle.Cross
        Private m_bSelected As Boolean = False
        Private m_parent As ucHatchSelect = Nothing
        Private m_clrFore As Color = Color.Black
        Private m_clrBack As Color = Color.Transparent

        Public Sub New(parent As ucHatchSelect, hbs As HatchStyle)
            Me.m_parent = parent
            Me.m_hbs = hbs
            Me.InitializeComponent()
        End Sub

        Private Sub ucHatch_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

            Dim rcHatch As Rectangle = New Rectangle(Me.ClientRectangle.X + 2, Me.ClientRectangle.Y + 2, Me.ClientRectangle.Width - 4, Me.ClientRectangle.Height - 4)

            e.Graphics.FillRectangle(New SolidBrush(Color.White), e.ClipRectangle)
            If Me.m_bSelected Then
                e.Graphics.DrawRectangle(New Pen(Color.Black, 2), rcHatch)
            End If
            rcHatch.Inflate(-4, -4)
            e.Graphics.DrawRectangle(Pens.Black, rcHatch)
            e.Graphics.FillRectangle(New HatchBrush(Me.m_hbs, Me.m_clrFore, Me.m_clrBack), rcHatch)
        End Sub

        Public ReadOnly Property HatchStyle() As HatchStyle
            Get
                Return Me.m_hbs
            End Get
        End Property

        Public Property Selected() As Boolean
            Get
                Return Me.m_bSelected
            End Get
            Set(value As Boolean)
                Me.m_bSelected = value
                Me.Invalidate()
            End Set
        End Property

        Public Sub Colours(clrFore As Color, clrBack As Color)
            Me.m_clrFore = clrFore
            Me.m_clrBack = clrBack
            Me.Invalidate()
        End Sub
    End Class

End Namespace
