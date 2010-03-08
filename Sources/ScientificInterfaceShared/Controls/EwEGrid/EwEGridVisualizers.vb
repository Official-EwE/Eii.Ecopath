#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels

#End Region ' Imports

Namespace Controls.EwEGrid

#Region " EwECellVisualizerBase "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' PropertyCellVisualizerBase is a base class visualizer that provides EwE
    ''' colour feedback
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class EwECellVisualizerBase
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
                ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal p_CellPosition As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                ByVal p_Status As SourceGrid2.DrawCellStatus)

            If p_Cell Is Nothing Then Return

            Dim eStyle As cStyleGuide.eStyleFlags = 0
            Dim clrBack As Color = Me.BackColor
            Dim clrFore As Color = Nothing ' Not used here
            Dim sg As cStyleGuide = Me.StyleGuide(p_Cell)

            ' Rendering a cell with an associated property?
            If (TypeOf p_Cell Is EwECellBase) Then
                ' #Yes: obtain rich info
                ' Get the cell
                Dim cell As EwECellBase = DirectCast(p_Cell, EwECellBase)
                ' Get its style
                eStyle = cell.Style()
                ' Get SG colours for this style
                If (sg IsNot Nothing) Then
                    ' ! Note that when obtaining the background color the remarks style is excluded. This
                    ' ! style will not be not reflected via the background colour but instead via a 
                    ' ! dedicated indicator (see below)
                    sg.GetStyleColors(eStyle And (Not cStyleGuide.eStyleFlags.Remarks), clrFore, clrBack)
                End If
            End If

            ' Does cell have focus?
            If (p_Status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus bk color
                clrBack = FocusBackColor
                ' Is cell selected?
            ElseIf (p_Status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selection bk color
                clrBack = SelectionBackColor
            End If

            ' Draw the background
            Using br As New SolidBrush(clrBack)
                e.Graphics.FillRectangle(br, p_ClientRectangle)
            End Using

            ' Check if need to render specific styles
            If (eStyle = 0) Then
                ' #No styles to render: done drawing
                Return
            End If

            ' Need to draw remarks indicator?
            If ((eStyle And cStyleGuide.eStyleFlags.Remarks) > 0) And (sg IsNot Nothing) Then
                ' #Yes: draw remarks indicator
                cRemarksIndicator.Paint(sg, p_ClientRectangle, e.Graphics, True)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw cell content using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_ImageAndText( _
                ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal p_CellPosition As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                ByVal p_Status As SourceGrid2.DrawCellStatus)

            If p_Cell Is Nothing Then Return

            Dim eStyle As cStyleGuide.eStyleFlags = 0
            Dim clrFore As Color = Me.ForeColor
            Dim clrBack As Color = Nothing ' Not used here
            Dim rcBorder As RectangleBorder = Me.Border
            Dim fontCell As Font = Me.GetCellFont()
            Dim rcClient As New Rectangle(p_ClientRectangle.X, p_ClientRectangle.Y, p_ClientRectangle.Width, p_ClientRectangle.Height)
            Dim sg As cStyleGuide = Me.StyleGuide(p_Cell)

            ' Rendering a cell with an associated property?
            If (TypeOf p_Cell Is EwECellBase) Then
                ' #Yes: obtain rich info
                ' Get the cell
                Dim cell As EwECellBase = DirectCast(p_Cell, EwECellBase)
                ' Get its style
                eStyle = cell.Style()
                If (sg IsNot Nothing) Then
                    ' Get SG colours for this style
                    sg.GetStyleColors(eStyle, clrFore, clrBack)
                End If
            End If

            ' Does cell have focus?
            If (p_Status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus border properties
                rcBorder = Me.FocusBorder
                clrFore = Me.FocusForeColor
                ' Is cell selected?
            ElseIf (p_Status = DrawCellStatus.Selected) Then
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
                p_Cell.GetDisplayText(p_CellPosition), _
                Me.StringFormat, Me.AlignTextToImage, rcBorder, _
                clrFore, fontCell)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to draw cell border using EwE color styles
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_Border(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                                                ByVal p_CellPosition As SourceGrid2.Position, _
                                                ByVal e As System.Windows.Forms.PaintEventArgs, _
                                                ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                                                ByVal p_Status As SourceGrid2.DrawCellStatus)

            If p_Cell Is Nothing Then Return

            Dim sg As cStyleGuide = Me.StyleGuide(p_Cell)
            Dim eStyle As cStyleGuide.eStyleFlags = 0
            Dim clrFore As Color = Me.ForeColor
            Dim rcBorder As RectangleBorder = Me.Border

            ' Rendering a cell with an associated property?
            If (TypeOf p_Cell Is EwECellBase) Then
                ' #Yes: obtain rich info
                ' Get the cell
                Dim cell As EwECellBase = DirectCast(p_Cell, EwECellBase)
                ' Get its style
                eStyle = cell.Style()
            End If

            ' Does cell have focus?
            If (p_Status = DrawCellStatus.Focus) Then
                ' #Yes: obtain standard focus border properties
                rcBorder = FocusBorder
                ' Is cell selected?
            ElseIf (p_Status = DrawCellStatus.Selected) Then
                ' #Yes: obtain standard selected border properties
                rcBorder = SelectionBorder
            End If

            ' Need to render highlightboder?
            If ((eStyle And cStyleGuide.eStyleFlags.Highlight) > 0) And (sg IsNot Nothing) Then
                ' #Yes: render highlight border
                rcBorder = New RectangleBorder( _
                    New Border(sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), Me.m_nHighlightBorderWidth))
            End If

            ' Draw the border
            ControlPaint.DrawBorder(e.Graphics, p_ClientRectangle, _
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
        ''' Borrow style guide from parent cell, if possible.
        ''' </summary>
        ''' <param name="p_Cell"></param>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property StyleGuide(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual) As cStyleGuide
            Get
                If (TypeOf p_Cell Is IUIElement) Then
                    Dim uic As cUIContext = DirectCast(p_Cell, IUIElement).UIContext
                    If (uic IsNot Nothing) Then
                        Return uic.StyleGuide
                    End If
                End If
                Return Nothing
            End Get
        End Property

#End Region ' Internals

    End Class

#End Region ' EwECellVisualizerBase

#Region " EwECellVisualizer "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwECellVisualizer implements a EwECellVisualizerBase visualizer
    ''' for rendering EwE property cells
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwECellVisualizer
        : Inherits EwECellVisualizerBase

        Public Shared Generic As New EwECellVisualizer()

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleCenter
            Me.WordWrap = True
            Me.AlignTextToImage = True
        End Sub

    End Class

#End Region ' EwECellVisualizer

#Region " cVisualizerEwERowHeader "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderVisualizer implements a PropertyCellVisualizerBase visualizer
    ''' for rendering EwE row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cVisualizerEwERowHeader
        : Inherits EwECellVisualizerBase

        Public Sub New()
            MyBase.new()
            Me.TextAlignment = ContentAlignment.MiddleLeft
            Me.WordWrap = True
        End Sub

    End Class

#End Region ' cVisualizerEwERowHeader

#Region " EwERowIndexVisualizer "

    <CLSCompliant(False)> _
    Public Class EwERowIndexVisualizer
        : Inherits EwECellVisualizerBase

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleCenter
            Me.WordWrap = True
        End Sub

    End Class

#End Region ' EwERowIndexVisualizer

#Region " EwEColumnHeaderVisualizer "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' PropertyColumnHeaderVisualizer implements a PropertyCellVisualizerBase visualizer
    ''' for rendering EwE column header cells
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwEColumnHeaderVisualizer
        : Inherits SourceGrid2.VisualModels.Header

        Public Sub New()
            MyBase.New(False)
            Me.TextAlignment = ContentAlignment.MiddleCenter
            Me.WordWrap = True
            Me.AlignTextToImage = True
        End Sub

        Protected Overrides Sub DrawCell_Border(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, ByVal p_CellPosition As SourceGrid2.Position, ByVal e As System.Windows.Forms.PaintEventArgs, ByVal p_ClientRectangle As System.Drawing.Rectangle, ByVal p_Status As SourceGrid2.DrawCellStatus)

            Dim border As RectangleBorder = Me.Border
            Dim rc As Rectangle = p_ClientRectangle
            Dim l_BackColor As Color = Me.BackColor

            If (p_Status = DrawCellStatus.Focus) Then
                l_BackColor = FocusBackColor
            ElseIf (p_Status = DrawCellStatus.Selected) Then
                l_BackColor = SelectionBackColor
                l_BackColor = BackColor
            End If

            ' Draw the border
            ControlPaint.DrawBorder(e.Graphics, rc, _
                SystemColors.ButtonHighlight, 1, ButtonBorderStyle.Solid, _
                Color.Transparent, 0, ButtonBorderStyle.Solid, _
                SystemColors.ButtonShadow, 1, ButtonBorderStyle.Solid, _
                SystemColors.ButtonShadow, 1, ButtonBorderStyle.Solid)

        End Sub

    End Class

#End Region ' EwEColumnHeaderVisualizer

#Region " cColorCellVisualizer "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' cColorCellVisualizer is a cell visualizer that provides color feedback.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cColorCellVisualizer
        Inherits SourceGrid2.VisualModels.Common

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to render cell value as a color
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub DrawCell_ImageAndText( _
                ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                ByVal p_CellPosition As SourceGrid2.Position, _
                ByVal e As System.Windows.Forms.PaintEventArgs, _
                ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                ByVal p_Status As SourceGrid2.DrawCellStatus)

            Dim value As Object = p_Cell.GetValue(p_CellPosition)

            If Not (TypeOf value Is Color) Then Return

            Dim clr As Color = DirectCast(value, Color)
            Dim rcColor As New Rectangle(p_ClientRectangle.X + 2, p_ClientRectangle.Y + 2, p_ClientRectangle.Width - 4, p_ClientRectangle.Height - 4)

            ' Draw the background
            Using br As New SolidBrush(clr)
                e.Graphics.FillRectangle(br, rcColor)
            End Using

        End Sub

    End Class

#End Region ' cColorCellVisualizer

End Namespace