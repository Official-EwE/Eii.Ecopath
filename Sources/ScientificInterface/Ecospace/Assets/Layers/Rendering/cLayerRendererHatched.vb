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
    ''' Layer renderer that draws cells interpreting the cell value as index 
    ''' to the .NET provided <see cref="HatchStyle">hatch patterns</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererHatch
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor Or cVisualStyle.eVisualStyleTypes.BackColor Or cVisualStyle.eVisualStyleTypes.Hatch)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)
            If Me.IsStyleValid Then
                Using br As New HatchBrush(Me.VisualStyle.HatchStyle, Me.VisualStyle.ForeColour, Me.VisualStyle.BackColour)
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
            Return ((Me.VisualStyle.HatchStyle > 0) And (CInt(Me.VisualStyle.HatchStyle) < [Enum].GetValues(GetType(System.Drawing.Drawing2D.HatchStyle)).Length))
        End Function

    End Class

End Namespace
