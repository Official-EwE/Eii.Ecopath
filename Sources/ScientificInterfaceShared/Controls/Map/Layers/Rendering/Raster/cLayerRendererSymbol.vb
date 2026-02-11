' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a coloured symbol, using the 
    ''' foreground colour in the attaced <see cref="cLayerRenderer.VisualStyle">visual style</see>
    ''' to fill the symbol.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererSymbol
        Inherits cRasterLayerRenderer

        Public Sub New(uic As cUIContext, vs As cVisualStyle)
            MyBase.New(uic, vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(g As Graphics,
                                            rc As RectangleF,
                                            Optional iSymbol As Integer = 0)

            Me.RenderSymbol(g, rc, cStyleGuide.FromVisualColor(Me.VisualStyle.ForeColour))

        End Sub

        Public Overrides Sub RenderCell(g As System.Drawing.Graphics,
                                        rc As System.Drawing.RectangleF,
                                        layer As cEcospaceLayer,
                                        value As Object,
                                        style As cStyleGuide.eStyleFlags)

            If (CBool(value)) Then Me.RenderPreview(g, rc)

        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return ""
        End Function

        Protected Overrides Function IsStyleValid() As Boolean
            Return True
        End Function

#Region " Internals "

        Protected Sub RenderSymbol(g As Graphics,
                                   rc As RectangleF,
                                   colorFill As Color)
            If Me.IsStyleValid() Then
                rc.Inflate(CInt(-rc.Width * 0.1), CInt(-rc.Height * 0.1))

                ' JS 05Sep16: center symbol
                Dim sz As Single = Math.Min(rc.Width, rc.Height)
                Dim rcSymbol As New RectangleF(rc.X + CInt((rc.Width - sz) / 2), rc.Y + CInt((rc.Height - sz) / 2), sz, sz)

                Using p As New Pen(Color.White, 3)
                    g.DrawEllipse(p, rcSymbol)
                End Using
                Using br As New SolidBrush(colorFill)
                    g.FillEllipse(br, rcSymbol)
                End Using
                g.DrawEllipse(Pens.Black, rcSymbol)
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace