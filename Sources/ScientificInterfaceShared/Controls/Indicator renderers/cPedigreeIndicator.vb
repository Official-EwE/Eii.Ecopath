' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, renders a pedigree indicator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPedigreeIndicator

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Renders a remarks indicator onto a given canvas
        ''' </summary>
        ''' <param name="sg">Style guide to paint with.</param>
        ''' <param name="rcClip">Clip boundary to fit the remarks indicator in</param>
        ''' <param name="g">The canvas to render onto</param>
        ''' <param name="sPedigreeLevel">Pedigree level to render [0, 1]. A value of
        ''' 0 will not render a pedigree indicator.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Paint(sg As cStyleGuide,
                                rcClip As Rectangle,
                                g As Graphics,
                                sPedigreeLevel As Single)

            If (sPedigreeLevel > 0) Then

                Dim rcPedigree As Rectangle = GetPedigreeArea(sg, rcClip)
                Dim clrFill As Color = sg.ApplicationColor(cStyleGuide.eApplicationColorType.PEDIGREE)
                Dim sBarHeight As Single = (rcPedigree.Height - 4) / 4.0!
                Dim rcBar As New RectangleF(rcPedigree.X, rcPedigree.Y + rcClip.Height - sBarHeight - 4, rcPedigree.Width, sBarHeight)

                Using br As New SolidBrush(clrFill)
                    For i As Integer = 0 To CInt(Math.Round(sPedigreeLevel * 3))
                        g.FillRectangle(br, rcBar)
                        rcBar.Y = rcBar.Y - 1 - sBarHeight
                    Next
                End Using

            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, determines the bounding box for a remarks indicator. This method could be handy when
        ''' testing for remarks indicator mouse hits.
        ''' </summary>
        ''' <param name="rcClip">Coordinates of the area to get the remarks indicator bounding box for.</param>
        ''' <returns>The bounding box that fully encapsulates the Remarks indicator.</returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function GetBounds(sg As cStyleGuide, rcClip As Rectangle) As Rectangle
            Return GetPedigreeArea(sg, rcClip)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the pedigree indicator area for a given cell boundary. The current UI culture
        ''' reading order is evaluated to determine the position and layout of the indicator.
        ''' </summary>
        ''' <param name="rcClip">Clip boundary to calculate the indicator area for.</param>
        ''' <returns>A rectangle.</returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function GetPedigreeArea(sg As cStyleGuide,
                                                rcClip As Rectangle) As Rectangle

            If (cSystemUtils.IsRightToLeft) Then
                ' ------.
                '     | |
                '     |=|
                ' ------`
                Return New Rectangle(rcClip.Width - 10, rcClip.Y + 2, 8, rcClip.Height - 4)
            Else
                ' .-----
                ' | |       
                ' |=|
                ' `-----
                Return New Rectangle(rcClip.X + 2, rcClip.Y + 2, 8, rcClip.Height - 4)
            End If
        End Function

    End Class

End Namespace ' Controls
