' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer for rendering EwE column header cells.
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cEwECheckBoxVisualizer
        Inherits SourceGrid2.VisualModels.CheckBox

        ''' <summary>Border width for Highlighted cells</summary>
        Private m_nHighlightBorderWidth As Integer = 4

        Public Sub New()
            MyBase.New(False)
        End Sub

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw background using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Background(
                cell As SourceGrid2.Cells.ICellVirtual,
                pos As SourceGrid2.Position,
                e As System.Windows.Forms.PaintEventArgs,
                rc As System.Drawing.Rectangle,
                status As SourceGrid2.DrawCellStatus)

            If cell Is Nothing Then Return

            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim style As cStyleGuide.eStyleFlags = Me.Style(cell)
            Dim clrBack As Color = Me.BackColor
            Dim clrFore As Color = Nothing ' Not used here

            If (sg Is Nothing) Then Return

            ' Get style colors, but exclude remarks style because remarks are 
            ' rendered in a different manner
            sg.GetStyleColors(style And Not cStyleGuide.eStyleFlags.Remarks, clrFore, clrBack)

            ' Does cell have focus?
            If (status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus bk color
                clrBack = Me.FocusBackColor
                ' Is cell selected?
            ElseIf (status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selection bk color
                clrBack = Me.SelectionBackColor
            End If

            ' Draw the background
            Using br As New SolidBrush(clrBack)
                e.Graphics.FillRectangle(br, rc)
            End Using

            ' Check if need to render specific styles
            If (style = 0) Then
                ' #No styles to render: done drawing
                Return
            End If

            ' Need to draw remarks indicator?
            If ((style And cStyleGuide.eStyleFlags.Remarks) > 0) And (sg IsNot Nothing) Then
                ' #Yes: draw remarks indicator
                cRemarksIndicator.Paint(sg.ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND), rc, e.Graphics, True, cSystemUtils.IsRightToLeft)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw cell border using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Border(cell As SourceGrid2.Cells.ICellVirtual,
                                                pos As SourceGrid2.Position,
                                                e As System.Windows.Forms.PaintEventArgs,
                                                rc As System.Drawing.Rectangle,
                                                status As SourceGrid2.DrawCellStatus)

            If (cell Is Nothing) Then Return

            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim style As cStyleGuide.eStyleFlags = Me.Style(cell)
            Dim clrFore As Color = Me.ForeColor
            Dim rcBorder As RectangleBorder = Me.Border

            If (sg Is Nothing) Then Return

            ' Does cell have focus?
            If (status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus border properties
                rcBorder = Me.FocusBorder
                ' Is cell selected?
            ElseIf (status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selected border properties
                rcBorder = Me.SelectionBorder
            End If

            ' Need to render highlightboder?
            If ((style And cStyleGuide.eStyleFlags.Highlight) > 0) And (sg IsNot Nothing) Then
                ' #Yes: render highlight border
                rcBorder = New RectangleBorder(
                    New Border(sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), Me.m_nHighlightBorderWidth))
            End If

            ' Draw the border
            ControlPaint.DrawBorder(e.Graphics, rc,
                rcBorder.Left.Color,
                rcBorder.Left.Width,
                ButtonBorderStyle.Solid,
                rcBorder.Top.Color,
                rcBorder.Top.Width,
                ButtonBorderStyle.Solid,
                rcBorder.Right.Color,
                rcBorder.Right.Width,
                ButtonBorderStyle.Solid,
                rcBorder.Bottom.Color,
                rcBorder.Bottom.Width,
                ButtonBorderStyle.Solid)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the <see cref="cStyleGuide">style guide</see> from a cell, 
        ''' if possible.
        ''' </summary>
        ''' <param name="cell">The cell to query.</param>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property StyleGuide(cell As SourceGrid2.Cells.ICellVirtual) As cStyleGuide
            Get
                If (TypeOf cell Is IEwECell) Then
                    Dim uic As cUIContext = DirectCast(cell, IEwECell).UIContext
                    If (uic IsNot Nothing) Then
                        Return uic.StyleGuide
                    End If
                End If
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the <see cref="cStyleGuide.eStyleFlags">style</see> from 
        ''' a given cell, if possible.
        ''' </summary>
        ''' <param name="cell">The cell to query.</param>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property Style(cell As SourceGrid2.Cells.ICellVirtual) As cStyleGuide.eStyleFlags
            Get
                ' Rendering a cell with an associated property?
                If (TypeOf cell Is IEwECell) Then
                    ' #Yes: obtain cell style
                    Return DirectCast(cell, IEwECell).Style()
                End If
                Return cStyleGuide.eStyleFlags.OK
            End Get
        End Property

#End Region ' Internals

    End Class

End Namespace