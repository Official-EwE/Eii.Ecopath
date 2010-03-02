#Region " Imports "

Option Strict On

Imports ZedGraph
Imports EwECore
Imports System.Drawing.Drawing2D

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exploratory kite diagram in a ZedGraph.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cZedGraphKiteHelper
        Inherits cZedGraphHelper

#Region " Private vars "

        Private m_lScaleCircles As New List(Of LineItem)

#End Region ' Private vars

#Region " Public interfaces "

        Public Shadows Function ConfigurePane(ByVal strTitle As String, _
                                              ByVal bShowLegend As Boolean, _
                                              Optional ByVal legendPos As ZedGraph.LegendPos = ZedGraph.LegendPos.TopCenter, _
                                              Optional ByVal iPane As Integer = 1) As ZedGraph.GraphPane

            Dim gp As GraphPane = MyBase.ConfigurePane(strTitle, "", Nothing, "", Nothing, bShowLegend, legendPos, iPane)

            gp.XAxis.Cross = 0
            gp.YAxis.Cross = 0

            gp.XAxis.MajorTic.IsAllTics = True
            gp.XAxis.MinorTic.IsAllTics = True
            gp.YAxis.MajorTic.IsAllTics = True
            gp.YAxis.MinorTic.IsAllTics = True
            gp.XAxis.Scale.IsVisible = True
            gp.YAxis.Scale.IsVisible = True

            Return gp

        End Function

        Public Overridable Sub ClearScaleCircles(Optional ByVal iPane As Integer = -1)

            ' Render the simulated polar decorations:
            Dim gp As GraphPane = Me.GetPane(iPane)
            For Each cu As CurveItem In Me.m_lScaleCircles
                gp.CurveList.Remove(cu)
            Next
            Me.m_lScaleCircles.Clear()

        End Sub

        Public Overridable Sub SetScaleCircles(Optional ByVal iPane As Integer = -1)

            ' Render the simulated polar decorations:
            Dim gp As GraphPane = Me.GetPane(iPane)
            Dim dTickSize As Double = gp.XAxis.Scale.MajorStep
            Dim iNumScaleCircles As Integer = CInt(Math.Floor(gp.XAxis.Scale.Max / dTickSize))
            Dim rpl As RadarPointList = Nothing
            Dim circle As LineItem = Nothing

            If Me.m_lScaleCircles.Count > 0 Then
                Me.ClearScaleCircles(iPane)
            End If

            For j As Integer = 1 To iNumScaleCircles

                rpl = New RadarPointList()
                For i As Integer = 0 To 20 : rpl.Add(j * dTickSize, 1) : Next i

                circle = New LineItem("", rpl, Color.Gray, SymbolType.None)
                circle.Line.IsSmooth = True
                circle.Line.SmoothTension = 0.6F
                circle.Line.Style = DashStyle.Custom
                circle.Line.DashOff = 2
                circle.Line.DashOn = 4

                Me.m_lScaleCircles.Add(circle)
                gp.CurveList.Insert(0, circle)

            Next

        End Sub

        Public Shadows Function CreateLineItem(ByVal iGroup As Integer, _
                                               ByVal asValues() As Single) As LineItem

            Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
            Return Me.CreateLineItem(group.Name, Me.StyleGuide.GroupColor(Me.Core, group.Index), asValues)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="clr"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shadows Function CreateLineItem(ByVal strName As String, _
                                               ByVal clr As Color, _
                                               ByVal asValues() As Single) As LineItem

            Dim rpl As New RadarPointList()
            For i As Integer = 0 To asValues.Length - 1
                rpl.Add(asValues(i), 1.0#)
            Next
            Return New LineItem(strName, rpl, clr, SymbolType.None)

        End Function

        Public Shadows Sub PlotLines(ByVal lines() As ZedGraph.LineItem, Optional ByVal iPane As Integer = 1)
            Me.ClearScaleCircles(iPane)
            MyBase.PlotLines(lines, iPane, True, True, False)
            Me.SetScaleCircles(iPane)
        End Sub

        Public Overrides Sub RescaleAndRedraw(Optional ByVal iPane As Integer = -1)
            MyBase.RescaleAndRedraw(iPane)
        End Sub

#End Region ' Public interfaces

#Region " Internals "

        ''' <summary>
        ''' Overridden to prevent summing of radial items.
        ''' </summary>
        ''' <param name="liOffset"></param>
        ''' <param name="lTarget"></param>
        ''' <remarks></remarks>
        Protected Overrides Sub SumLines(ByVal liOffset As ZedGraph.LineItem, ByVal lTarget As ZedGraph.LineItem)

        End Sub

#End Region ' Internals

    End Class

End Namespace
