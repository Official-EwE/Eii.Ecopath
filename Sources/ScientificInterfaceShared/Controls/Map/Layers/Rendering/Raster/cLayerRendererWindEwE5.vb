' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region 'Imports

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells as a wind indicator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererWindEwE5
        Inherits cRasterLayerRenderer

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle)
            If Me.IsStyleValid Then
                Me.RenderCell(g, rc, Nothing, New Single() {5, 5}, cStyleGuide.eStyleFlags.OK)
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
            Dim sScaleX As Single = 0.0!
            Dim sScaleY As Single = 0.0!

            If (layer IsNot Nothing) Then
                If (layer.MaxValue > 0) Then sMax = layer.MaxValue
            End If

            If TypeOf value Is Single() Then
                asValues = DirectCast(value, Single())
                If asValues.Length = 2 Then

                    ' Calc display scale, rounded to two decimals between -1 and 1
                    Try
                        sScaleX = Math.Max(-1, Math.Min(cNumberUtils.FixValue(CSng(Math.Round(asValues(0) / sMax, 2))), 1))
                        sScaleY = Math.Max(-1, Math.Min(cNumberUtils.FixValue(CSng(Math.Round(asValues(1) / sMax, 2))), 1))
                    Catch ex As Exception
                        sScaleX = 0
                        sScaleY = 0
                    End Try

                    cArrowIndicator.DrawArrowDxDy(g, Me.VisualStyle.ForeColour, rc, sScaleX, sScaleY)

                    '' Leave a margin
                    'rc.Inflate(-2, -2)
                    '' Calc center
                    'ptfCenter = New PointF(CSng(rc.X + rc.Width / 2), CSng(rc.Y + rc.Height / 2))
                    '' Calc arrow size
                    'szfHalfArrow = New SizeF(rc.Width * sScaleX / 2.0!, rc.Height * sScaleY / 2.0!)

                    'Using p As New Pen(Me.VisualStyle.ForeColour, 0.001!)

                    '    p.StartCap = LineCap.Round
                    '    p.CustomEndCap = New AdjustableArrowCap(3, 3)

                    '    g.DrawLine(p, _
                    '                   ptfCenter.X - szfHalfArrow.Width, ptfCenter.Y - szfHalfArrow.Height, _
                    '                   ptfCenter.X + szfHalfArrow.Width, ptfCenter.Y + szfHalfArrow.Height)
                    '    'g.DrawEllipse(p, _
                    '    '              ptfCenter.X + szfHalfArrow.Width - rc.Width / 8.0!, _
                    '    '              ptfCenter.Y + szfHalfArrow.Height - rc.Height / 8.0!, _
                    '    '              rc.Width / 4.0!, _
                    '    '              rc.Height / 4.0!)
                    'End Using

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