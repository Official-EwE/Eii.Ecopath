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
    ''' Layer renderer that draws cells as an arrow with a specific direction
    ''' and scale. The cell value to render provided in 
    ''' <see cref="cLayerRendererArrow.RenderCell">RenderCell</see> should hold a
    ''' two-dimensional array describing these arrow properties.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererArrow
        Inherits cLayerRenderer

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
            Dim sAngle As Single = 0
            Dim sScale As Single = 1.0

            ' Value should be an array of two singles:
            '   value(0) = arrow angle [0, 360]
            '   value(1) = arrow scale [0, 1]

            If TypeOf value Is Single() Then
                asValues = DirectCast(value, Single())
                If asValues.Length > 0 Then
                    sAngle = asValues(0)
                    If asValues.Length > 1 Then
                        sScale = asValues(1)
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