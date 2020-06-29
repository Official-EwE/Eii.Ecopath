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

#End Region ' Imports

Public Class cDietFlowMapRenderer

    Private Class cElementListSorter
        Implements IComparer(Of cTreeMapRenderer.cTreeMapElement)

        Public Function Compare(x As cTreeMapRenderer.cTreeMapElement, y As cTreeMapRenderer.cTreeMapElement) As Integer Implements IComparer(Of cTreeMapRenderer.cTreeMapElement).Compare
            If y Is Nothing Then Return -1
            If x Is Nothing Then Return 1
            If (x.Value < y.Value) Then Return 1
            If (x.Value > y.Value) Then Return -1
            Return 0
        End Function

    End Class

    Private m_uic As cUIContext
    Private m_lPreds As New List(Of Integer)

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic

        Dim core As cCore = Me.m_uic.Core
        For i As Integer = 1 To core.nLivingGroups
            Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
            If grp.IsConsumer Then m_lPreds.Add(i)
        Next

    End Sub

    <Flags>
    Public Enum eDrawMode As Integer
        Name = 1
        Number = 2
        All = Name Or Number
    End Enum

    Public Property DrawCaptions As Boolean = True
    Public Property DrawLabels As Boolean = True
    Public Property LabelDrawMode As eDrawMode
    Public Property DrawBorders As Boolean = False

    Public Sub Draw(g As Graphics, rc As Rectangle)

        If (Me.m_uic Is Nothing) Then Return

        Dim lRects As New List(Of Rectangle)
        Dim core As cCore = Me.m_uic.Core
        Dim sg As cStyleGuide = Me.m_uic.StyleGuide
        Dim fmt As New cCoreInterfaceFormatter()

        ' For now, draw all living groups
        Me.CalcMapAreas(rc, Me.m_lPreds.Count, lRects)

        For j As Integer = 1 To Me.m_lPreds.Count

            Dim renderer As New cTreeMapRenderer(Me.m_uic)
            renderer.DrawLabels = Me.DrawLabels
            renderer.DrawCaption = Me.DrawCaptions

            Dim elements As New List(Of cTreeMapRenderer.cTreeMapElement)
            Dim iPred As Integer = Me.m_lPreds(j - 1)
            Dim pred As cEcoPathGroupInput = core.EcoPathGroupInputs(iPred)
            For i As Integer = 1 To core.nGroups
                Dim prey As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
                Dim dc As Single = pred.DietComp(i)
                If dc > 0 Then
                    Dim elm As New cTreeMapRenderer.cTreeMapElement()
                    Select Case Me.LabelDrawMode
                        Case eDrawMode.Name
                            elm.Label = prey.Name
                        Case eDrawMode.Number
                            elm.Label = CStr(i)
                        Case eDrawMode.All
                            elm.Label = fmt.ToString(prey, )
                    End Select

                    elm.Color = sg.GroupColor(core, i)
                    elm.Value = dc
                    elements.Add(elm)
                End If
            Next i

            elements.Sort(New cElementListSorter())
            renderer.DrawTreemap(elements, pred.Name, g, lRects(j - 1))
        Next

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
                    Dim rect As Rectangle = New Rectangle(CInt(xSize * j + 3), CInt(i * ySize + 3), CInt(xSize - 6), CInt(ySize - 6))
                    lRects.Add(rect)
                End If
            Next
        Next

    End Sub

End Class
