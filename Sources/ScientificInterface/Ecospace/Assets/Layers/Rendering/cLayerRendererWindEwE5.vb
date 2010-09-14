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
    ''' Layer renderer that draws cells as a wind indicator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererWindEwE5
        Inherits cLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)
            If Me.IsStyleValid Then
                Me.RenderCell(g, rc, layer, New Single() {5, 5}, cStyleGuide.eStyleFlags.OK)
            Else
                Me.RenderError(g, rc)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draw the cell as an arrow with a given angle and scale.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        ''' <param name="value">A two-dimensional array of singles, 
        ''' holding the angle [0, 360] as the first index, and the scale
        ''' [0, 1] as the second index.</param>
        ''' <param name="style"></param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub RenderCell(ByVal g As Graphics, _
                                        ByVal rc As Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)

            Dim asValues As Single() = Nothing
            Dim ptfCenter As PointF = Nothing
            Dim szfHalfArrow As SizeF = Nothing
            Dim sMax As Single = 1

            If (layer.MaxValue > 0) Then sMax = layer.MaxValue

            If TypeOf value Is Single() Then
                asValues = DirectCast(value, Single())
                If asValues.Length = 2 Then

                    ' Leave a margin
                    rc.Inflate(-2, -2)
                    ' Calc center
                    ptfCenter = New PointF(CSng(rc.X + rc.Width / 2), CSng(rc.Y + rc.Height / 2))
                    ' Calc arrow size
                    szfHalfArrow = New SizeF(rc.Width * asValues(0) / (2 * sMax), rc.Height * asValues(1) / (2 * sMax))

                    Using p As New Pen(Me.VisualStyle.ForeColour, 0.001!)
                        g.DrawLine(p, _
                                   ptfCenter.X - szfHalfArrow.Width, ptfCenter.Y - szfHalfArrow.Height, _
                                   ptfCenter.X + szfHalfArrow.Width, ptfCenter.Y + szfHalfArrow.Height)
                        g.DrawEllipse(p, _
                                      ptfCenter.X + szfHalfArrow.Width - rc.Width / 8.0!, _
                                      ptfCenter.Y + szfHalfArrow.Height - rc.Height / 8.0!, _
                                      rc.Width / 4.0!, _
                                      rc.Height / 4.0!)
                    End Using

                    ' If Depth(i, j + 1) > 0 Then Vxp = Xvloc(i, j) Else Vxp = 0
                    ' If Depth(i + 1, j) > 0 Then Vyp = Yvloc(i, j) Else Vyp = 0
                    ' WF.Circle (j + 0.5 + Vxp / Xmax, i + 0.5 + Vyp / Xmax), 0.03
                    ' WF.Line (j + 0.5, i + 0.5)-Step(Vxp / Xmax, Vyp / Xmax)

                End If
            End If

        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return True
        End Function

    End Class

End Namespace