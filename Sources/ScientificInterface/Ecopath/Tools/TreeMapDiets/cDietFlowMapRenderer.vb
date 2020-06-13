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
Imports EwECore
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cDietFlowMapRenderer

    Private m_uic As cUIContext
    Private m_lPreds As New List(Of Integer)

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic

        Dim core As cCore = Me.m_uic.Core
        For i As Integer = 1 To core.nLivingGroups
            Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
            If grp.IsConsumer Then m_lPreds.Add(i)
            If Me.m_lPreds.Count = 1 Then Exit For
        Next

    End Sub

    Public Sub Draw(g As Graphics, rc As Rectangle)

        If (Me.m_uic Is Nothing) Then Return

        Dim lRects As New List(Of Rectangle)
        Dim core As cCore = Me.m_uic.Core
        Dim sg As cStyleGuide = Me.m_uic.StyleGuide

        ' For now, draw all living groups
        Me.CalcMapAreas(rc, Me.m_lPreds.Count, lRects)

        Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)
            For j As Integer = 1 To Me.m_lPreds.Count
                Dim renderer As New cTreeMapRenderer()
                Dim elements As New List(Of cTreeMapRenderer.cTreeMapElement)
                Dim iPred As Integer = Me.m_lPreds(j - 1)
                Dim pred As cEcoPathGroupInput = core.EcoPathGroupInputs(iPred)

                For i As Integer = 1 To core.nGroups
                    Dim prey As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
                    Dim dc As Single = CInt(pred.DietComp(i) * 100)
                    If dc > 0 Then
                        Dim elm As New cTreeMapRenderer.cTreeMapElement()
                        elm.Label = prey.Name
                        elm.Color = sg.GroupColor(core, i)
                        elm.Value = dc
                        elements.Add(elm)
                    End If
                Next i
                renderer.DrawTreemap(elements, g, lRects(j - 1), ft)
            Next
        End Using

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the best layout of diet panels.
    ''' </summary>
    ''' <param name="rc">The area to draw to.</param>
    ''' <param name="iNumPlots">The number of plots to draw.</param>
    ''' <param name="lRects">A list to receive the map rectangles onto <paramref name="rc"/>.</param>
    ''' -------------------------------------------------------------------
    Private Sub CalcMapAreas(rc As Rectangle, iNumPlots As Integer, ByRef lRects As List(Of Rectangle))

        lRects.Clear()

        If (iNumPlots = 0) Then Return

        Dim iNumHorz As Integer = CInt(Math.Ceiling(Math.Sqrt(iNumPlots) * rc.Width / rc.Height))
        Dim iNumVert As Integer = CInt(Math.Ceiling(iNumPlots / Math.Max(1, iNumHorz)))

        Dim xSize As Double = rc.Width / iNumHorz
        Dim ySize As Double = rc.Height / iNumVert

        For i As Integer = 0 To iNumVert - 1
            For j As Integer = 0 To iNumHorz - 1
                Dim iRect As Integer = i * iNumHorz + j
                If iRect < iNumPlots Then
                    Dim rect As Rectangle = New Rectangle(CInt(xSize * j + 1), CInt(i * ySize + 1), CInt(xSize), CInt(ySize))
                    lRects.Add(rect)
                End If
            Next
        Next

    End Sub

End Class
