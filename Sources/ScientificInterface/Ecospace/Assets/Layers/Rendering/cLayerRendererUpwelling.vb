#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Reflection
Imports EwECore.Auxiliary

#End Region 'Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a wind indicator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererUpwelling
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderCell(ByVal g As Graphics, _
                                        ByVal rc As Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags)

            Dim ptfCenter As PointF = Nothing
            Dim sHalfArrow As Single = Nothing
            Dim sValue As Single = 0.0!
            Dim sMax As Single = 1.0!

            If layer.MaxValue > 0.0! Then sMax = layer.MaxValue

            If TypeOf value Is Single Then
                '' Leave a margin
                'rc.Inflate(-2, -2)
                ' Get value to render
                sValue = -CSng(value)
                'Cl2 = 0.01 / CellLength ' ^ 2
                'UpVel(i, j) = UpLoc  'Added for this model  SM.
                'Up.Circle (j + 0.5, i + 0.5 - UpLoc / UpMax), 0.1
                'Up.Line (j + 0.5, i + 0.5)-Step(0, -UpLoc / UpMax)

                ' Calc cell center
                ptfCenter = New PointF(CSng(rc.X + rc.Width / 2), CSng(rc.Y + rc.Height / 2))
                ' Calc arrow size
                sHalfArrow = rc.Height * sValue / (2 * sMax)

                Using p As New Pen(Me.VisualStyle.ForeColour, 0.001!)
                    g.DrawEllipse(p, ptfCenter.X - 2, ptfCenter.Y + sHalfArrow - 2, 3, 3)
                    g.DrawLine(p, _
                               ptfCenter.X, ptfCenter.Y - sHalfArrow, _
                               ptfCenter.X, ptfCenter.Y + sHalfArrow)
                End Using

            End If

        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As EwECore.cEcospaceLayer)

            If Me.IsStyleValid Then
                Me.RenderCell(g, rc, layer, 42.0!, cStyleGuide.eStyleFlags.OK)
            Else
                Me.RenderError(g, rc)
            End If

        End Sub

    End Class

End Namespace
