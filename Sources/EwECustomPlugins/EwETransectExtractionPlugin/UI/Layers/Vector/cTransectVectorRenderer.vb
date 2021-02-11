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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Style.cStyleGuide

#End Region ' Imports

Public Class cTransectVectorRenderer
    Inherits cVectorLayerRenderer

    Private m_sg As cStyleGuide = Nothing

#Region " Construction / destruction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(uic As cUIContext)
        MyBase.New(uic, Nothing, cVisualStyle.eVisualStyleTypes.NotSet)
        Me.m_sg = uic.StyleGuide
    End Sub

#End Region ' Construction / destruction

#Region " Overrides "

    Public Overrides Sub RenderPreview(g As Graphics, rc As RectangleF, Optional iSymbol As Integer = 0)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cVectorLayerRenderer.Render(Graphics, cDisplayLayer, RectangleF, PointF, PointF, eStyleFlags)"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Render(g As Graphics, layer As cDisplayLayer, rc As RectangleF, ptfTL As PointF, ptfBR As PointF, style As cStyleGuide.eStyleFlags)

        Dim m_data As cTransectDatastructures = DirectCast(layer, cTransectVectorDisplay).Data

        Dim sScaleX As Single = (rc.Width / (ptfBR.X - ptfTL.X))
        Dim sScaleY As Single = (rc.Height / (ptfTL.Y - ptfBR.Y))

        For Each t As cTransect In m_data.Transects
            Me.RenderTransect(t, g, rc, ptfTL, sScaleX, sScaleY, Me.m_sg.ApplicationColor(eApplicationColorType.READONLY_BACKGROUND))
        Next
        Me.RenderTransect(m_data.Selection, g, rc, ptfTL, sScaleX, sScaleY, Me.m_sg.ApplicationColor(eApplicationColorType.HIGHLIGHT))

    End Sub

    Public Overrides Function GetDisplayText(value As Object) As String
        Return My.Resources.CAPTION_IN
    End Function

#End Region ' Overrides

    Private Sub RenderTransect(t As cTransect, g As Graphics, rc As RectangleF, ptfTL As PointF, sScaleX As Single, sScaleY As Single, clr As Color)

        If (t Is Nothing) Then Return

        Dim ptFrom As New PointF(rc.X + (t.Start.X - ptfTL.X) * sScaleX, rc.Y + (ptfTL.Y - t.Start.Y) * sScaleY)
        Dim ptTo As New PointF(rc.X + (t.End.X - ptfTL.X) * sScaleX, rc.Y + (ptfTL.Y - t.End.Y) * sScaleY)

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

End Class
