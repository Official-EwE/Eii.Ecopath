' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Style.cStyleGuide



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
