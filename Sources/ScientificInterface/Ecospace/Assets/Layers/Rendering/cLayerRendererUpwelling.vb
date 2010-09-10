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

            'Cl2 = 0.01 / CellLength ' ^ 2
            'UpVel(i, j) = UpLoc  'Added for this model  SM.
            'Up.Circle (j + 0.5, i + 0.5 - UpLoc / UpMax), 0.1
            'Up.Line (j + 0.5, i + 0.5)-Step(0, -UpLoc / UpMax)

            Dim ptfCenter As PointF = Nothing
            Dim sHalfArrow As Single = Nothing
            Dim sValue As Single = 0.0!
            Dim sMax As Single = 1.0!
            Dim iR As Integer = 0
            Dim iG As Integer = 0
            Dim iB As Integer = 0

            If layer.MaxValue > 0.0! Then sMax = layer.MaxValue

            If TypeOf value Is Single Then
                ' Get value to render
                sValue = -CSng(value)

                ' Calc cell center
                ptfCenter = New PointF(CSng(rc.X + rc.Width / 2), CSng(rc.Y + rc.Height / 2))
                ' Calc arrow size
                sHalfArrow = rc.Height * sValue / (2 * sMax)

                ' Has a value to draw?
                If (sValue <> 0.0!) Then
                    ' #Yes: render a Green (up) or Blue (down) upwelling arrow
                    iG = CInt(IIf(sValue > 0, 150, 0))
                    iB = CInt(IIf(sValue > 0, 0, 150))
                    Using p As New Pen(Color.FromArgb(255, iR, iG, iB), 0.001!)
                        g.DrawLine(p, _
                                   ptfCenter.X, ptfCenter.Y - sHalfArrow, _
                                   ptfCenter.X, ptfCenter.Y + sHalfArrow)
                        g.DrawEllipse(p, _
                                      ptfCenter.X - rc.Width / 8.0!, _
                                      ptfCenter.Y + sHalfArrow - rc.Height / 8.0!, _
                                      rc.Width / 4.0!, rc.Height / 4.0!)
                    End Using
                Else
                    Using p As New Pen(Me.VisualStyle.ForeColour, 0.001!)
                        g.DrawLine(p, _
                                   ptfCenter.X - rc.Width / 4.0!, ptfCenter.Y, _
                                   ptfCenter.X + rc.Width / 4.0!, ptfCenter.Y)
                    End Using
                End If



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
