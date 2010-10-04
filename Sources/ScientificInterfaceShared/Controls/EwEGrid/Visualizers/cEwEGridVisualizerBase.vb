#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEGridVisualizerBase is a base class visualizer that provides 
    ''' <see cref="cStyleGuide.eStyleFlags">status</see> colour feedback.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class cEwEGridVisualizerBase
        Inherits SourceGrid2.VisualModels.Common

#Region " Private bits "

        ''' <summary>Border width for Highlighted cells</summary>
        Private m_nHighlightBorderWidth As Integer = 4
        ''' <summary>Text indentation level.</summary>
        Private m_iTextIndent As Integer = 0

#End Region ' Private bits 

#Region " Public configuration bits "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the text indentation
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Indentation() As Integer
            Get
                Return Me.m_iTextIndent
            End Get
            Set(ByVal value As Integer)
                Me.m_iTextIndent = Math.Max(0, value)
            End Set
        End Property

#End Region ' Public configuration bits 

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw background using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Background( _
                ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal pos As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal rc As System.Drawing.Rectangle, _
                ByVal status As SourceGrid2.DrawCellStatus)

            If cell Is Nothing Then Return

            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim style As cStyleGuide.eStyleFlags = Me.Style(cell)
            Dim clrBack As Color = Me.BackColor
            Dim clrFore As Color = Nothing ' Not used here

            sg.GetStyleColors(style, clrFore, clrBack)

            ' Does cell have focus?
            If (status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus bk color
                clrBack = FocusBackColor
                ' Is cell selected?
            ElseIf (status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selection bk color
                clrBack = SelectionBackColor
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
                cRemarksIndicator.Paint(sg, rc, e.Graphics, True)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw cell content using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_ImageAndText( _
                ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal pos As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal rc As System.Drawing.Rectangle, _
                ByVal status As SourceGrid2.DrawCellStatus)

            If cell Is Nothing Then Return

            Dim rcBorder As RectangleBorder = Me.Border
            Dim fontCell As Font = Me.GetCellFont()
            Dim rcClient As New Rectangle(rc.X, rc.Y, rc.Width, rc.Height)

            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim style As cStyleGuide.eStyleFlags = Me.Style(cell)
            Dim clrBack As Color = Me.BackColor
            Dim clrFore As Color = Me.ForeColor

            sg.GetStyleColors(style, clrFore, clrBack)

            ' Does cell have focus?
            If (status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus border properties
                rcBorder = Me.FocusBorder
                clrFore = Me.FocusForeColor
                ' Is cell selected?
            ElseIf (status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selected border properties
                rcBorder = Me.SelectionBorder
                clrFore = Me.SelectionForeColor
            End If

            ' Include indentation, if any
            rcClient.X += Me.m_iTextIndent
            rcClient.Width -= Me.m_iTextIndent

            ' Render Image and Text
            Utility.PaintImageAndText(e.Graphics, rcClient, _
                Me.Image, Me.ImageAlignment, Me.ImageStretch, _
                cell.GetDisplayText(pos), _
                Me.StringFormat, Me.AlignTextToImage, rcBorder, _
                clrFore, fontCell)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw cell border using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Border(ByVal cell As SourceGrid2.Cells.ICellVirtual, _
                                                ByVal pos As SourceGrid2.Position, _
                                                ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                ByVal rc As System.Drawing.Rectangle, _
                                                ByVal status As SourceGrid2.DrawCellStatus)

            If cell Is Nothing Then Return

            Dim sg As cStyleGuide = Me.StyleGuide(cell)
            Dim style As cStyleGuide.eStyleFlags = Me.Style(cell)
            Dim clrFore As Color = Me.ForeColor
            Dim rcBorder As RectangleBorder = Me.Border

            ' Does cell have focus?
            If (status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus border properties
                rcBorder = FocusBorder
                ' Is cell selected?
            ElseIf (status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selected border properties
                rcBorder = SelectionBorder
            End If

            ' Need to render highlightboder?
            If ((style And cStyleGuide.eStyleFlags.Highlight) > 0) And (sg IsNot Nothing) Then
                ' #Yes: render highlight border
                rcBorder = New RectangleBorder( _
                    New Border(sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), Me.m_nHighlightBorderWidth))
            End If

            ' Draw the border
            ControlPaint.DrawBorder(e.Graphics, rc, _
                rcBorder.Left.Color, _
                rcBorder.Left.Width, _
                ButtonBorderStyle.Solid, _
                rcBorder.Top.Color, _
                rcBorder.Top.Width, _
                ButtonBorderStyle.Solid, _
                rcBorder.Right.Color, _
                rcBorder.Right.Width, _
                ButtonBorderStyle.Solid, _
                rcBorder.Bottom.Color, _
                rcBorder.Bottom.Width, _
                ButtonBorderStyle.Solid)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the <see cref="cStyleGuide">style guide</see> from a cell, 
        ''' if possible.
        ''' </summary>
        ''' <param name="cell">The cell to query.</param>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property StyleGuide(ByVal cell As SourceGrid2.Cells.ICellVirtual) As cStyleGuide
            Get
                If (TypeOf cell Is IUIElement) Then
                    Dim uic As cUIContext = DirectCast(cell, IUIElement).UIContext
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
        Protected ReadOnly Property Style(ByVal cell As SourceGrid2.Cells.ICellVirtual) As cStyleGuide.eStyleFlags
            Get
                ' Rendering a cell with an associated property?
                If (TypeOf cell Is EwECellBase) Then
                    ' #Yes: obtain cell style
                    Return DirectCast(cell, EwECellBase).Style()
                End If
                Return cStyleGuide.eStyleFlags.OK
            End Get
        End Property

#End Region ' Internals

    End Class

End Namespace

