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
    ''' Layer renderer that draws cell values as a background colour scaled
    ''' across a colour gradient based on the cell value in relation to the 
    ''' layer min/max value range.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererGradient
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)
            If Me.IsStyleValid() Then
                Using br As New SolidBrush(Me.VisualStyle.ForeColour)
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
            Return True
        End Function

    End Class

End Namespace
