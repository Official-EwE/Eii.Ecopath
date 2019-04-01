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
Imports ZedGraph
Imports System.Text
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to to update the graph that reflects Ecospace biodiversity indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMCHistogramGraphWrapper
    Inherits cZedGraphHelper

#Region " Private variables "

    ''' <summary>Indicator grouping etc as centrally defined in the plug-in.</summary>
    Private m_settings As cIndicatorSettings = Nothing
    ''' <summary>List of Ecopath indicators to show histograms for.</summary>
    Private m_lind As List(Of cEcopathIndicators) = Nothing

    ''' <summary>Current indicator group to display in the graph.</summary>
    Private m_groupCurrent As cIndicatorInfoGroup = Nothing
    ''' <summary>Current indicator to display in the graph.</summary>
    Private m_indCurrent As cIndicatorInfo = Nothing

    Private m_nBins As Integer = 100

#End Region ' Private variables

#Region " Attach + detach "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Attach this class to a zedgraph control.
    ''' </summary>
    ''' <param name="uic"><see cref="cUIContext"/> providing UI contextual information.</param>
    ''' <param name="zgc"><see cref="ZedGraphControl"/> to style and interact with.</param>
    ''' <param name="settings"><see cref="cIndicatorSettings"/> defined centrally in the plug-in.</param>
    ''' -------------------------------------------------------------------
    Public Shadows Sub Attach(uic As ScientificInterfaceShared.Controls.cUIContext,
                              zgc As ZedGraph.ZedGraphControl,
                              settings As cIndicatorSettings,
                              lind As List(Of cEcopathIndicators))

        MyBase.Attach(uic, zgc, 1)
        ' Store important bits
        Me.m_settings = settings

        Me.m_lind = lind

        Me.ShowPointValue = True
    End Sub

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="cZedGraphHelper.Detach"/>
    ''' -------------------------------------------------------------------
    Public Overrides Sub Detach()
        Me.m_settings = Nothing
        MyBase.Detach()
    End Sub

#End Region ' Attach + detach

#Region " Refreshing "

    Public Property NumBins(baseline As cEcopathIndicators) As Integer
        Get
            Return Me.m_nBins
        End Get
        Set(value As Integer)
            If (Me.m_settings Is Nothing) Then Return

            Me.m_nBins = Math.Max(10, Math.Min(100, value))
            Me.RefreshContent(Me.m_indCurrent, Me.m_groupCurrent, baseline)
        End Set
    End Property

    Public Sub RefreshContent(indSingle As cIndicatorInfo, indGroup As cIndicatorInfoGroup, baseline As cEcopathIndicators)

        If (indSingle Is Nothing) And (indGroup Is Nothing) Then Return

        Dim lInfo As New List(Of cIndicatorInfo)
        Dim info As cIndicatorInfo = Nothing
        Dim gp As GraphPane = Nothing
        Dim strLabelPane As String = ""
        Dim strLabelValue As String = ""
        Dim settings As cIndicatorSettings = Me.m_settings
        Dim ind As cEcosimIndicators = Nothing
        Dim pplTrend As PointPairList = Nothing
        Dim sValue As Single = 0
        Dim sXMin As Single = 0
        Dim sXMax As Single = 0

        If (indSingle Is Nothing) Then
            ' Group mode
            Me.m_groupCurrent = indGroup
            Me.m_indCurrent = Nothing

            For i As Integer = 0 To indGroup.NumIndicators - 1
                lInfo.Add(indGroup.Indicator(i))
            Next
            strLabelPane = indGroup.Name
        Else
            ' Indicator mode
            Me.m_groupCurrent = Nothing
            Me.m_indCurrent = indSingle
            lInfo.Add(indSingle)
            strLabelPane = indSingle.Name
        End If

        ' Set master pane title
        Me.Configure(strLabelPane)

        If (lInfo.Count > 0) Then
            ' Create and configure panes
            Me.NumPanes = lInfo.Count
            For iPane As Integer = 1 To Me.NumPanes
                info = lInfo(iPane - 1)
                gp = Me.GetPane(iPane)
                gp.Tag = info
                If String.IsNullOrWhiteSpace(info.Units) Then
                    strLabelValue = info.ValueDescription
                Else
                    strLabelValue = String.Format(SharedResources.GENERIC_LABEL_DETAILED, info.ValueDescription, info.Units)
                End If
                Me.ConfigurePane(info.Name, strLabelValue, My.Resources.HEADER_OCCURRENCE, False, iPane:=iPane)
            Next
        End If

        Try
            ' Next populate all panels
            For iPane As Integer = 1 To Me.NumPanes
                ' Get pane for indicator iInd
                gp = Me.GetPane(iPane)
                gp.CurveList.Clear()

                ' Prepare structures for creating point list for indicator
                info = DirectCast(gp.Tag, cIndicatorInfo)

                pplTrend = New PointPairList()

                Dim sBinWidth As Single = 1
                Dim hist() As Drawing.PointF = Me.Histogram(info, sBinWidth)
                Dim sYMax As Single = 0

                'The X value in the histogram is the max value of the bin, right hand side of the bin
                'So an input value of 1.0 will be in the .X = 1.0 bin
                For ipt As Integer = 1 To hist.Length - 1
                    pplTrend.Add(hist(ipt).X - sBinWidth / 2, hist(ipt).Y)
                    pplTrend.Add(hist(ipt).X + sBinWidth / 2, hist(ipt).Y)
                    sYMax = Math.Max(sYMax, hist(ipt).Y)
                Next

                sXMin = hist(1).X
                sXMax = hist(hist.Length - 1).X

                ' Add baseline (=ecopath indicator)
                If (baseline IsNot Nothing) Then
                    If (baseline.IsComputed) Then
                        Dim pplBaseline As New PointPairList()
                        Dim baseval As Single = info.GetValue(baseline)
                        pplBaseline.Add(baseval, 0)
                        pplBaseline.Add(baseval, sYMax + 1)
                        Dim liBaseline As LineItem = Me.CreateLineItem(My.Resources.HEADER_BASELINE, eSketchDrawModeTypes.NotSet, Drawing.Color.Orange, pplBaseline)
                        Me.PlotLines(New LineItem() {liBaseline}, iPane, bClear:=False)

                        sXMax = Math.Max(baseval, sXMax)
                        sXMin = Math.Min(baseval, sXMin)
                    End If
                End If

                Dim li As LineItem = Me.CreateLineItem(info.Name, eSketchDrawModeTypes.NotSet, Drawing.Color.Gray, pplTrend)
                li.Line.Fill = New Fill(cColorUtils.GetVariant(li.Color, 0.5))
                Me.PlotLines(New LineItem() {li}, iPane, bClear:=False)

                gp.XAxis.Scale.MinAuto = False
                gp.XAxis.Scale.MinGrace = 0
                gp.XAxis.Scale.Min = sXMin - sBinWidth / 2
                gp.XAxis.Scale.MaxAuto = False
                gp.XAxis.Scale.MaxGrace = 0
                gp.XAxis.Scale.Max = sXMax + sBinWidth / 2

                gp.YAxis.Scale.MaxAuto = False
                gp.YAxis.Scale.MaxGrace = 0
                gp.YAxis.Scale.Max = sYMax + 1

                gp.AxisChange()

            Next iPane

        Catch ex As Exception
            ' Ouch
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region ' Refreshing

    Private Function Histogram(info As cIndicatorInfo, ByRef sBinWidth As Single) As Drawing.PointF()

        Dim nValues As Integer = Math.Max(1, Me.m_lind.Count)
        Dim pts(Me.m_nBins) As Drawing.PointF

        Dim sMin As Single = Single.MaxValue
        Dim sMax As Single = Single.MinValue

        If (Me.m_lind.Count > 0) Then
            For Each ind As cEcopathIndicators In Me.m_lind
                Dim sVal As Single = info.GetValue(ind)
                sMin = Math.Min(sVal, sMin)
                sMax = Math.Max(sVal, sMax)
            Next
        Else
            sMin = 0
            sMax = 1
        End If

        Dim sRange As Single = sMax - sMin
        If (sRange > 0) Then
            sBinWidth = sRange / Me.m_nBins
        Else
            'No data in the map so just set a default binwidth 
            'this will dump all the data into the zero bin
            sBinWidth = 1.0F / Me.m_nBins
        End If

        For Each ind As cEcopathIndicators In Me.m_lind
            Dim sVal As Single = info.GetValue(ind)
            Dim ipt As Integer = CInt(Math.Truncate((sVal - sMin) / sBinWidth)) + 1
            ipt = Math.Max(1, Math.Min(Me.m_nBins, ipt))
            pts(ipt).Y += 1
        Next

        For i As Integer = 1 To Me.m_nBins
            pts(i).X = CSng(sMin + sBinWidth * i)
            'pts(i).Y = pts(i).Y / nValues
        Next
        Return pts

    End Function

#Region " Tooltip "

    Protected Overrides Function FormatTooltip(pane As ZedGraph.GraphPane, curve As ZedGraph.CurveItem, iPoint As Integer) As String

        Dim ind As cIndicatorInfo = DirectCast(pane.Tag, cIndicatorInfo)
        Dim sb As New StringBuilder()

        ' Tooltip should show the indicator description, if available, instead of repeating the pane title
        If Not String.IsNullOrEmpty(ind.Description) Then
            sb.Append(ind.Description)
        Else
            sb.Append(curve.Label.Text)
        End If

        Dim strValueBit As String = Me.FormatTooltipValue(pane, curve, iPoint)
        If Not String.IsNullOrEmpty(strValueBit) Then
            If sb.Length > 0 Then sb.AppendLine("")
            sb.Append(strValueBit)
        End If
        Return sb.ToString

    End Function

#End Region ' Tooltip

End Class
