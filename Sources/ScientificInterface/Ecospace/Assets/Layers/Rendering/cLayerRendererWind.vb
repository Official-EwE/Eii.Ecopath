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
    Public Class cLayerRendererWind
        Inherits cLayerRenderer

        Private Const s_R2D As Single = 180.0! / Math.PI

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)
            If Me.IsStyleValid Then
                'g.FillRectangle(Brushes.White, rc)
                Me.RenderCell(g, rc, layer, 45, cStyleGuide.eStyleFlags.OK)
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
            Dim sAngle As Single
            Dim sScale As Single

            If TypeOf value Is Single() Then
                asValues = DirectCast(value, Single())
                If asValues.Length > 0 Then
                    '        'If Depth(i, j + 1) > 0 Then Vxp = Xvloc(i, j) Else Vxp = 0
                    '        'If Depth(i + 1, j) > 0 Then Vyp = Yvloc(i, j) Else Vyp = 0
                    'WF.Circle (j + 0.5 + Vxp / Xmax, i + 0.5 + Vyp / Xmax), 0.03
                    'WF.Line (j + 0.5, i + 0.5)-Step(Vxp / Xmax, Vyp / Xmax)
                    If layer.MaxValue = 0 Then
                        sAngle = 0
                        sScale = 0
                    Else
                        If asValues(1) = 0 Then
                            If asValues(0) < 0 Then sAngle = 0 Else sAngle = 180
                        Else
                            sAngle = CSng(Math.Atan(asValues(0) / asValues(1))) * s_R2D
                        End If
                        sScale = CSng(Math.Sqrt(asValues(0) * asValues(0) + asValues(1) * asValues(1)) / layer.MaxValue)
                    End If
                End If
            End If

            cArrowIndicator.DrawArrow(g, Me.VisualStyle.ForeColour, rc, sAngle, sScale)
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            Return True
        End Function

    End Class

End Namespace