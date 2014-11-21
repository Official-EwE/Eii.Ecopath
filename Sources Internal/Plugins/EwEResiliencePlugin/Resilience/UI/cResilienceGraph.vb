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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Controller for the resilience graph.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cResilienceGraph
    Inherits cZedGraphHelper

#Region " Private vars "

    ''' <summary><see cref="cResilienceData"/> instance to work with.</summary>
    Private m_data As cResilienceData = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New()
        ' NOP
    End Sub

#End Region ' Construction

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Attach the handler to the graph.
    ''' </summary>
    ''' <param name="uic"><see cref="cUIContext"/>.</param>
    ''' <param name="zgc"><see cref="ZedGraphControl"/>.</param>
    ''' <param name="data"><see cref="cResilienceData"/>.</param>
    ''' <param name="strTitle">The title of the graph.</param>
    ''' -----------------------------------------------------------------------
    Public Shadows Sub Attach(uic As cUIContext, zgc As ZedGraph.ZedGraphControl, data As cResilienceData, strTitle As String)

        MyBase.Attach(uic, zgc, 1)

        ' Store ref
        Me.m_data = data

        Me.Configure(strTitle)
        Me.ConfigurePane(My.Resources.RESIL_LABEL_CAPTION, My.Resources.RESIL_LABEL_XAXIS, My.Resources.RESIL_LABEL_YAXIS, False)
        Me.AutoscalePane() = True
        Me.ShowPointValue = True

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clean-up.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shadows Sub Detach()

        MyBase.Detach()
        Me.m_data = Nothing

    End Sub

#End Region ' Overrides

#Region " Public bits "

    Public Property Annual As Boolean
    Public Property Time As Integer
    Public Property UseDefaultRegression As Boolean = True

    Public Sub Refresh()

        Dim data As cResilienceData = Me.m_data
        Dim pane As ZedGraph.GraphPane = Me.GetPane(1)
        Dim ppl As ZedGraph.PointPairList = Nothing
        Dim pplReg As ZedGraph.PointPairList = Nothing
        Dim li As ZedGraph.LineItem = Nothing
        Dim fmt As New cCoreInterfaceFormatter()
        Dim strScale As String = ""
        Dim grp As cEcoPathGroupInput = Nothing

        ' Regression bit
        Dim x, y As Double
        Dim xmin As Double = Double.MaxValue
        Dim xmax As Double = Double.MinValue
        Dim a As Single = 0
        Dim b As Single = 0
        Dim n As Integer = 0

        pane.CurveList.Clear()

        pplReg = New PointPairList()

        If (Me.Annual) Then
            pane.XAxis.Scale.Min = data.DataboundsY.dmin
            pane.XAxis.Scale.Max = data.DataboundsY.dmax
            pane.YAxis.Scale.Min = data.DataboundsY.smin
            pane.YAxis.Scale.Max = data.DataboundsY.smax
        Else
            pane.XAxis.Scale.Min = data.DataboundsT.dmin
            pane.XAxis.Scale.Max = data.DataboundsT.dmax
            pane.YAxis.Scale.Min = data.DataboundsT.smin
            pane.YAxis.Scale.Max = data.DataboundsT.smax
        End If

        ' Add a line for each group
        For i As Integer = 1 To Me.Core.nGroups
            ppl = New ZedGraph.PointPairList()
            grp = Core.EcoPathGroupInputs(i)

            If grp.IsConsumer Then
                If (Me.Annual) Then
                    x = data.GroupDemandAtY(i, Me.Time)
                    y = data.GroupSupplyAtY(i, Me.Time)
                Else
                    x = data.GroupDemandAtT(i, Me.Time)
                    y = data.GroupSupplyAtT(i, Me.Time)
                End If
                ppl.Add(x, y)
                li = New ZedGraph.LineItem(fmt.GetDescriptor(grp), ppl, Me.StyleGuide.GroupColor(Me.Core, i), ZedGraph.SymbolType.Circle)
                li.Line.IsVisible = False
                pane.CurveList.Add(li)

                ' Regression tracking
                pplReg.Add(x, y)
                xmin = Math.Min(xmin, x)
                xmax = Math.Max(xmax, x)
                n += 1
            End If
        Next

        ' Add trend line
        Me.FindRegression(pplReg, b, a, n)
        ppl = New ZedGraph.PointPairList()
        ppl.Add(xmin, a + b * xmin)
        ppl.Add(xmax, a + b * xmax)
        li = New ZedGraph.LineItem(My.Resources.RESIL_LABEL_TREND, ppl, Drawing.Color.Blue, ZedGraph.SymbolType.None)
        li.Line.IsVisible = True
        pane.CurveList.Add(li)

        ' Prepare graph title
        If (Me.Annual) Then
            strScale = SharedResources.GENERAL_LABEL_ANNUAL
        Else
            strScale = SharedResources.GENERAL_LABEL_MONTHLY
        End If

        Dim t As Integer = Me.Time
        If (Me.Annual) Then
            If (Me.Core.EcosimFirstYear > 0) Then t = Me.Core.EcosimFirstYear - 1 + t
        End If
        pane.Title.Text = String.Format(My.Resources.RESIL_LABEL_CAPTION, strScale, t)

        ' ToDo: fix axis ranges for the plot

        ' Done
        Me.RescaleAndRedraw()

    End Sub

#End Region ' Public bits

End Class
