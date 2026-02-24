' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a bitmap, provided in the attached
    ''' <see cref="cLayerRenderer.VisualStyle">visual style</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererBitmap
        Inherits cRasterLayerRenderer

        Public Sub New(uic As cUIContext, vs As cVisualStyle)
            MyBase.New(uic, vs, cVisualStyle.eVisualStyleTypes.Image)
        End Sub

        Public Overrides Sub RenderPreview(g As Graphics,
                                            rc As RectangleF,
                                            Optional iSymbol As Integer = 0)
            If (Me.IsStyleValid) Then
                Me.DrawImageAlpha(g, rc, Me.VisualStyle.Image, 1.0!)
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

        Public Overrides Sub RenderCell(g As System.Drawing.Graphics,
                                        rc As System.Drawing.RectangleF,
                                        layer As cEcospaceLayer,
                                        value As Object,
                                        style As cStyleGuide.eStyleFlags)
            If (Me.IsStyleValid) Then
                Me.DrawImageAlpha(g, rc, Me.VisualStyle.Image, Math.Min(1, Math.Max(0, CSng(value))))
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return (Me.VisualStyle.Image IsNot Nothing)
        End Function

        Public Overrides Function Clone() As cRasterLayerRenderer
            Dim objClone As Object = Nothing
            Dim vs As cVisualStyle = Me.VisualStyle.Clone()
            objClone = Activator.CreateInstance(Me.GetType(), New Object() {Me.UIContext, vs})
            Return DirectCast(objClone, cRasterLayerRenderer)
        End Function

        Private Sub DrawImageAlpha(g As Graphics, rc As RectangleF, img As Image, sAlpha As Single)

            If sAlpha >= 1 Then
                Using br As New TextureBrush(img, WrapMode.Tile)
                    g.FillRectangle(br, rc)
                End Using
            Else
                Dim matrixItems As Single()() = {
                    New Single() {1, 0, 0, 0, 0},
                    New Single() {0, 1, 0, 0, 0},
                    New Single() {0, 0, 1, 0, 0},
                    New Single() {0, 0, 0, sAlpha, 0},
                    New Single() {0, 0, 0, 0, 1}}

                Dim colorMatrix As New ColorMatrix(matrixItems)
                Dim imageAtt As New ImageAttributes()
                imageAtt.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap)

                Using br As New TextureBrush(img, New Rectangle(0, 0, img.Width, img.Height), imageAtt)
                    br.WrapMode = WrapMode.Tile
                    g.FillRectangle(br, rc)
                End Using
            End If

        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return Convert.ToString(value)
        End Function

    End Class

End Namespace
