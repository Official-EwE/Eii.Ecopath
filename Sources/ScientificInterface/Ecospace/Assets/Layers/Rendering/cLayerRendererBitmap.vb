#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D
Imports System.Reflection
Imports EwECore.Auxiliary

#End Region 'Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a bitmap, provided in the attached
    ''' <see cref="cLayerRenderer.VisualStyle">visual style</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererBitmap
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.Image)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)
            If (Me.IsStyleValid) Then
                Using br As New TextureBrush(Me.VisualStyle.Image, WrapMode.Tile)
                    g.FillRectangle(br, rc)
                End Using
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, _
                                        ByVal rc As System.Drawing.Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)
            Me.RenderPreview(g, rc, layer)
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return (Me.VisualStyle.Image IsNot Nothing)
        End Function

        Public Overrides Function Clone() As cLayerRenderer
            Dim objClone As Object = Nothing
            Dim vs As cVisualStyle = Me.VisualStyle.Clone()

            objClone = Activator.CreateInstance(Me.GetType(), New Object() {vs})
            Return DirectCast(objClone, cLayerRenderer)
        End Function

    End Class

End Namespace
