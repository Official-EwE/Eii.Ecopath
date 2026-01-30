' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel



Public Class cDietTreeMapRenderer

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
            Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(i)
            If grp.IsConsumer Then Me.m_lPreds.Add(i)
        Next

    End Sub

    <Flags>
    Public Enum eDrawMode As Integer
        None = 0
        Name = 1
        Number = 2
        All = Name Or Number
    End Enum

    <Browsable(True),
        Category("Appearance"),
        DisplayName("Predator display style"),
        DefaultValue(eDrawMode.Name)>
    Public Property PredatorLabelStyle As eDrawMode = eDrawMode.Name

    <Browsable(True),
        Category("Appearance"),
        DisplayName("Prey display style"),
        DefaultValue(eDrawMode.Number)>
    Public Property PreyLabelStyle As eDrawMode = eDrawMode.Number

    <Browsable(True),
        Category("Appearance"),
        DisplayName("Draw borders"),
        DefaultValue(True)>
    Public Property DrawBorders As Boolean = False

    <Browsable(True),
        Category("Appearance"),
        DisplayName("Padding"),
        DefaultValue(3)>
    Public Property Padding As Integer = 3

    Public Sub Draw(g As Graphics, rc As Rectangle)

        If (Me.m_uic Is Nothing) Then Return

        Dim lRects As New List(Of Rectangle)
        Dim core As cCore = Me.m_uic.Core
        Dim sg As cStyleGuide = Me.m_uic.StyleGuide
        Dim fmt As New cCoreInterfaceFormatter()

        Dim lPreds As New List(Of Integer)
        For i As Integer = 0 To Me.m_lPreds.Count - 1
            If sg.GroupVisible(Me.m_lPreds(i)) Then
                lPreds.Add(Me.m_lPreds(i))
            End If
        Next

        ' For now, draw all living groups
        Me.CalcMapAreas(rc, lPreds.Count, lRects)

        For j As Integer = 0 To lPreds.Count - 1

            Dim renderer As New cTreeMapRenderer(Me.m_uic)
            Dim predLabel As String = ""

            renderer.DrawBorders = Me.DrawBorders
            renderer.DrawCaptions = (Me.PredatorLabelStyle <> eDrawMode.None)
            renderer.DrawDataLabels = (Me.PreyLabelStyle <> eDrawMode.None)

            Dim elements As New List(Of cTreeMapRenderer.cTreeMapElement)
            Dim iPred As Integer = lPreds(j)
            Dim pred As cEcoPathGroupInput = core.EcopathGroupInputs(iPred)

            For i As Integer = 1 To core.nGroups
                Dim prey As cEcoPathGroupInput = core.EcopathGroupInputs(i)
                Dim dc As Single = pred.DietComp(i)
                If dc > 0 Then
                    Dim elm As New cTreeMapRenderer.cTreeMapElement()
                    Select Case Me.PreyLabelStyle
                        Case eDrawMode.None
                            elm.Label = ""
                        Case eDrawMode.Name
                            elm.Label = prey.Name
                        Case eDrawMode.Number
                            elm.Label = CStr(i)
                        Case eDrawMode.All
                            elm.Label = fmt.ToString(prey)
                    End Select

                    elm.Color = sg.GroupColor(core, i)
                    elm.Value = dc
                    elements.Add(elm)
                End If
            Next i

            elements.Sort(New cElementListSorter())

            Select Case Me.PredatorLabelStyle
                Case eDrawMode.None
                    predLabel = ""
                Case eDrawMode.Name
                    predLabel = pred.Name
                Case eDrawMode.Number
                    predLabel = CStr(pred.Index)
                Case eDrawMode.All
                    predLabel = fmt.ToString(pred)
            End Select
            renderer.DrawTreemap(elements, predLabel, g, lRects(j))
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
                    Dim rect As Rectangle = New Rectangle(CInt(xSize * j + Me.Padding), CInt(i * ySize + Me.Padding), CInt(xSize - 2 * Me.Padding), CInt(ySize - 2 * Me.Padding))
                    lRects.Add(rect)
                End If
            Next
        Next

    End Sub

End Class
