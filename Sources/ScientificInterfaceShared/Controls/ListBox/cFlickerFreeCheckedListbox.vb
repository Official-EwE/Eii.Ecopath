' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A listbox class that does not flicker when redrawing / resizing
    ''' </summary>
    ''' <remarks>
    ''' Code directly converted from http://yacsharpblog.blogspot.com.es/2008/07/listbox-flicker.html
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cFlickerFreeCheckedListBox
        Inherits System.Windows.Forms.CheckedListBox

        Public Sub New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Me.DrawMode = DrawMode.OwnerDrawFixed
        End Sub

        Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
            If Me.Items.Count > 0 Then
                e.DrawBackground()
                e.Graphics.DrawString(Me.Items(e.Index).ToString(), e.Font, New SolidBrush(Me.ForeColor), New PointF(e.Bounds.X, e.Bounds.Y))
            End If
            MyBase.OnDrawItem(e)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            Dim iRegion As New Region(e.ClipRectangle)
            e.Graphics.FillRegion(New SolidBrush(Me.BackColor), iRegion)
            If Me.Items.Count > 0 Then
                For i As Integer = 0 To Me.Items.Count - 1
                    Dim irect As System.Drawing.Rectangle = Me.GetItemRectangle(i)
                    If e.ClipRectangle.IntersectsWith(irect) Then
                        If (Me.SelectionMode = SelectionMode.One AndAlso Me.SelectedIndex = i) OrElse (Me.SelectionMode = SelectionMode.MultiSimple AndAlso Me.SelectedIndices.Contains(i)) OrElse (Me.SelectionMode = SelectionMode.MultiExtended AndAlso Me.SelectedIndices.Contains(i)) Then
                            Me.OnDrawItem(New DrawItemEventArgs(e.Graphics, Me.Font, irect, i, DrawItemState.Selected, Me.ForeColor,
                             Me.BackColor))
                        Else
                            Me.OnDrawItem(New DrawItemEventArgs(e.Graphics, Me.Font, irect, i, DrawItemState.[Default], Me.ForeColor,
                             Me.BackColor))
                        End If
                        iRegion.Complement(irect)
                    End If
                Next
            End If
            MyBase.OnPaint(e)
        End Sub

    End Class

End Namespace
