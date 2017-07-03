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
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
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
        Me.ConfigurePane(My.Resources.GRAPH_RES_CAPTION, My.Resources.GRAPH_RES_XAXIS_LABEL, My.Resources.GRAPH_RES_YAXIS_LABEL, False)
        Me.AutoscalePane() = True
        Me.ShowPointValue = True
        Me.IsLegendVisible = True

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

    Public Sub Refresh()

        Dim data As cResilienceData = Me.m_data
        Dim pane As ZedGraph.GraphPane = Me.GetPane(1)
        Dim ppl As ZedGraph.PointPairList = Nothing
        Dim li As ZedGraph.LineItem = Nothing
        Dim sg As cStyleGuide = Me.UIContext.StyleGuide

        pane.CurveList.Clear()

        If (Not Me.m_data.Calculated) Then Return

        ppl = New PointPairList()
        For iTimeStep As Integer = 1 To Me.m_data.NumTimeSteps
            ppl.Add(iTimeStep / 12, Me.m_data.ResilienceAtT(iTimeStep))
        Next
        li = New ZedGraph.LineItem(My.Resources.GRAPH_RES_LINE_MONTHLY, ppl, Color.Black, SymbolType.None)
        pane.CurveList.Add(li)

        ppl = New PointPairList()
        For iYear As Integer = 1 To Me.m_data.NumYears
            ppl.Add(If(iYear = 1, 1 / cCore.N_MONTHS, iYear - 1), Me.m_data.ResilienceAtY(iYear))
            ppl.Add(iYear, Me.m_data.ResilienceAtY(iYear))
        Next
        li = New ZedGraph.LineItem(My.Resources.GRAPH_RES_LINE_ANNUAL, ppl, Color.Blue, SymbolType.None)
        pane.CurveList.Add(li)

        pane.Title.Text = My.Resources.GRAPH_RES_CAPTION

        ' Done
        Me.RescaleAndRedraw()

    End Sub

#End Region ' Public bits

End Class
