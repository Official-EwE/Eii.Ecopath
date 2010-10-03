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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer for rendering EwE column header cells.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cEwEGridColumnHeaderVisualizer
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

End Namespace