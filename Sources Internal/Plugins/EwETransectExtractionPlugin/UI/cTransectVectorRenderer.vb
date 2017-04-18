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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class cTransectVectorRenderer
    Inherits cVectorLayerRenderer

#Region " Construction / destruction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vs"></param>
    ''' <param name="layerStyleFlags"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal vs As cVisualStyle,
                   Optional ByVal layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
        MyBase.New(vs, layerStyleFlags)
    End Sub

#End Region ' Construction / destruction

    Public Overrides Sub RenderPreview(g As Graphics, rc As Rectangle, Optional iSymbol As Integer = 0)
    End Sub

    Public Overrides Sub Render(g As Graphics, layer As cDisplayLayer, rc As Rectangle, ptfTL As PointF, ptfBR As PointF, style As cStyleGuide.eStyleFlags)

        If (Not TypeOf layer Is cDisplayLayerTransect) Then Return

        Dim bml As cDisplayLayerTransect = DirectCast(layer, cDisplayLayerTransect)
        Dim data As cTransectDatastructures = bml.Data

        If (data Is Nothing) Then Return

        Dim sScaleX As Single = (rc.Width / (ptfBR.X - ptfTL.X))
        Dim sScaleY As Single = (rc.Height / (ptfTL.Y - ptfBR.Y))

        For Each t As cTransect In data.Transects
            Me.RenderTransect(t, g, rc, ptfTL, sScaleX, sScaleY, Color.LightBlue)
        Next
        Me.RenderTransect(data.Selection, g, rc, ptfTL, sScaleX, sScaleY, Color.Orange)

    End Sub

    Private Sub RenderTransect(t As cTransect, g As Graphics, rc As Rectangle, ptfTL As PointF, sScaleX As Single, sScaleY As Single, clr As Color)

        If (t Is Nothing) Then Return

        Dim ptFrom As New PointF((t.Start.X - ptfTL.X) * sScaleX, (ptfTL.Y - t.Start.Y) * sScaleY)
        Dim ptTo As New PointF((t.End.X - ptfTL.X) * sScaleX, (ptfTL.Y - t.End.Y) * sScaleY)

        Using p As New Pen(Color.Black, 5)
            p.StartCap = LineCap.RoundAnchor
            p.EndCap = LineCap.RoundAnchor
            g.DrawLine(p, ptFrom, ptTo)
        End Using

        Using p As New Pen(clr, 3)
            p.StartCap = LineCap.RoundAnchor
            p.EndCap = LineCap.RoundAnchor
            g.DrawLine(p, ptFrom, ptTo)
        End Using

    End Sub

    Public Overrides Function GetDisplayText(value As Object) As String
        Return "Transects"
    End Function

End Class
