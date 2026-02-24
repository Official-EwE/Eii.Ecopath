' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Control that draws a horizontal or vertical line.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <ToolboxBitmap(GetType(ucFormSeparator), "ucFormSeparator.ico")>
    Public Class ucFormSeparator

        Public Sub New()
            Me.InitializeComponent()
            Me.Horizontal = True
        End Sub

        Public Property Horizontal As Boolean

        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            Using br As New SolidBrush(Me.BackColor)
                e.Graphics.FillRectangle(br, 0, 0, Me.Width, Me.Height)
            End Using

            If Me.Horizontal Then
                Using p As New Pen(System.Drawing.SystemColors.ControlDark, 1)
                    e.Graphics.DrawLine(p, 0, 0, Me.Width, 0)
                End Using
                Using p As New Pen(System.Drawing.SystemColors.ControlLightLight, 1)
                    e.Graphics.DrawLine(p, 0, 1, Me.Width, 1)
                End Using
            Else
                Using p As New Pen(System.Drawing.SystemColors.ControlDark, 1)
                    e.Graphics.DrawLine(p, 0, 0, 0, Me.Height)
                End Using
                Using p As New Pen(System.Drawing.SystemColors.ControlLightLight, 1)
                    e.Graphics.DrawLine(p, 1, Me.Height, 1, Me.Height)
                End Using
            End If

        End Sub

    End Class

End Namespace
