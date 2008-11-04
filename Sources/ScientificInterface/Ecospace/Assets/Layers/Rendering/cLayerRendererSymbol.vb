'==============================================================================
'
' $Log: cLayerRendererSymbol.vb,v $
' Revision 1.1  2008/11/04 04:41:35  jeroens
' Split into separate files, moved
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D
Imports System.Reflection

#End Region 'Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a coloured symbol, using the 
    ''' foreground colour in the attaced <see cref="cLayerRenderer.VisualStyle">visual style</see>
    ''' to fill the symbol.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererSymbol
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, ByVal rc As Rectangle)
            If Me.IsStyleValid() Then
                rc.Inflate(CInt(-rc.Width * 0.75), CInt(-rc.Height * 0.75))
                Using p As New Pen(Color.White, 3)
                    g.DrawEllipse(p, rc)
                End Using
                Using br As New SolidBrush(Me.VisualStyle.ForeColour)
                    g.FillEllipse(br, rc)
                End Using
                g.DrawEllipse(Pens.Black, rc)
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, ByVal rc As System.Drawing.Rectangle, _
                                        ByVal value As Object, ByVal style As StyleGuide.eStyleFlags)
            Me.RenderPreview(g, rc)
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return True
        End Function

    End Class

End Namespace