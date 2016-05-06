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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
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
''' Controller for the resilience supply/demand graph.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cDemandSupplyGraph
    Inherits cZedGraphHelper

#Region " Private vars "

    ''' <summary><see cref="cResilienceData"/> instance to work with.</summary>
    Private m_data As cResilienceData = Nothing
    Private m_bAnnual As Boolean = False
    Private m_bFixedScale As Boolean = False

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

        ' Store ref
        Me.m_data = data

        MyBase.Attach(uic, zgc, 1)

        Me.Configure(strTitle)
        Me.ConfigurePane(My.Resources.GRAPH_SD_CAPTION, My.Resources.GRAPH_SD_XAXIS_LABEL, My.Resources.GRAPH_SD_YAXIS_LABEL, False)

        Me.AutoscalePane() = True
        Me.ShowPointValue = True
        Me.IsLegendVisible = False

        Me.SetScaleMode()

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

    Public Property FixedScale As Boolean
        Get
            Return Me.m_bFixedScale
        End Get
        Set(value As Boolean)
            If (Me.m_bFixedScale <> value) Then
                Me.m_bFixedScale = value
                Me.SetScaleMode()
            End If
        End Set
    End Property

    Public Property Annual As Boolean
        Get
            Return Me.m_bAnnual
        End Get
        Set(value As Boolean)
            If (value <> Me.m_bAnnual) Then
                Me.m_bAnnual = value
                Me.SetScaleMode()
            End If
        End Set
    End Property

    Public Property Time As Integer

    Public Sub Reset()
        Me.SetScaleMode()
    End Sub

    Public Sub Refresh()

        Dim data As cResilienceData = Me.m_data
        Dim t As Integer = Me.Time
        Dim pane As ZedGraph.GraphPane = Me.GetPane(1)
        Dim ppl As ZedGraph.PointPairList = Nothing
        Dim pplReg As ZedGraph.PointPairList = Nothing
        Dim li As ZedGraph.LineItem = Nothing
        Dim sg As cStyleGuide = Me.UIContext.StyleGuide
        Dim fmt As New cCoreInterfaceFormatter()
        Dim strScale As String = ""

        ' Regression bit
        Dim x, y As Double
        Dim xmin As Double = Double.MaxValue
        Dim xmax As Double = Double.MinValue
        Dim resilience As Single

        pane.CurveList.Clear()

        If (Not Me.m_data.Calculated) Then Return

        pplReg = New PointPairList()

        If (Me.FixedScale) Then
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
        Else
            pane.XAxis.Scale.MaxAuto = True
            pane.XAxis.Scale.MinAuto = True
            pane.YAxis.Scale.MaxAuto = True
            pane.YAxis.Scale.MinAuto = True
        End If

        ' Add a line for each group. Each line contains one circle
        For iGroup As Integer = 1 To Me.Core.nGroups
            ppl = New ZedGraph.PointPairList()

            If Me.m_data.IsConsumer(iGroup) Then

                If (Me.Annual) Then
                    x = data.GroupSupplyAtY(iGroup, t)
                    y = data.GroupDemandAtY(iGroup, t)
                Else
                    x = data.GroupSupplyAtT(iGroup, t)
                    y = data.GroupDemandAtT(iGroup, t)
                End If

                If (x <> 0 And y <> 0) Then
                    ppl.Add(x, y)

                    li = Me.CreateLineItem(Core.EcoPathGroupInputs(iGroup), ppl)
                    li.Symbol.Type = SymbolType.Circle
                    li.Line.IsVisible = False
                    pane.CurveList.Add(li)

                    ' Regression tracking
                    pplReg.Add(x, y)
                    xmin = Math.Min(xmin, x)
                    xmax = Math.Max(xmax, x)
                End If

            End If
        Next

        ' Add trend line
        ppl = New ZedGraph.PointPairList()
        If Me.Annual Then
            ppl.Add(xmin, data.InterceptAtY(t) + data.SlopeAtY(t) * xmin)
            ppl.Add(xmax, data.InterceptAtY(t) + data.SlopeAtY(t) * xmax)
            resilience = data.ResilienceAtY(t)
        Else
            ppl.Add(xmin, data.InterceptAtT(t) + data.SlopeAtT(t) * xmin)
            ppl.Add(xmax, data.InterceptAtT(t) + data.SlopeAtT(t) * xmax)
            resilience = data.ResilienceAtT(t)
        End If
        li = New ZedGraph.LineItem(My.Resources.GRAPH_SD_TREND, ppl, Drawing.Color.Black, ZedGraph.SymbolType.None)
        li.Line.IsVisible = True
        pane.CurveList.Add(li)

        ' Prepare graph title
        If (Me.Annual) Then
            strScale = SharedResources.GENERAL_LABEL_YEAR
        Else
            strScale = SharedResources.GENERAL_LABEL_MONTH
        End If

        If (Me.Annual) Then
            If (Me.Core.EcosimFirstYear > 0) Then t = Me.Core.EcosimFirstYear - 1 + t
        End If

        pane.Title.Text = cStringUtils.ToSentenceCase(cStringUtils.Localize(My.Resources.GRAPH_SD_CAPTION, strScale, t, sg.FormatNumber(resilience)))

        ' Done
        Me.RescaleAndRedraw()

    End Sub

#End Region ' Public bits

#Region " Internals "

    Private Sub SetScaleMode()

        Dim data As cResilienceData = Me.m_data
        Dim pane As ZedGraph.GraphPane = Me.GetPane(1)

        If (Me.FixedScale) Then
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
        Else
            pane.XAxis.Scale.MaxAuto = True
            pane.XAxis.Scale.MinAuto = True
            pane.YAxis.Scale.MaxAuto = True
            pane.YAxis.Scale.MinAuto = True
        End If

    End Sub

#End Region ' Internals

End Class
