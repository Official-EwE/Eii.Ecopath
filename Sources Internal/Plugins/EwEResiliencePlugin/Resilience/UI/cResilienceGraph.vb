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
        Me.ConfigurePane(My.Resources.RESIL_LABEL_CAPTION, "Time (year)", "Resilience", False)
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

    Public Sub Refresh()

        Dim data As cResilienceData = Me.m_data
        Dim pane As ZedGraph.GraphPane = Me.GetPane(1)
        Dim ppl As ZedGraph.PointPairList = Nothing
        Dim li As ZedGraph.LineItem = Nothing
        Dim sg As cStyleGuide = Me.UIContext.StyleGuide

        pane.CurveList.Clear()
        ppl = New PointPairList()
        For i As Integer = 0 To Me.m_data.NumTimeSteps
            ppl.Add(i / 12, Me.m_data.ResilienceAtT(i))
        Next
        li = New ZedGraph.LineItem("Resilience (month)", ppl, Color.Black, SymbolType.None)
        pane.CurveList.Add(li)

        ppl = New PointPairList()
        For i As Integer = 0 To Me.m_data.NumYears - 1
            ppl.Add(i, Me.m_data.ResilienceAtY(i))
        Next
        li = New ZedGraph.LineItem("Resilience (year)", ppl, Color.Blue, SymbolType.Circle)
        li.Line.IsVisible = False
        pane.CurveList.Add(li)

        pane.Title.Text = "Resilience"

        ' Done
        Me.RescaleAndRedraw()

    End Sub

#End Region ' Public bits

End Class
