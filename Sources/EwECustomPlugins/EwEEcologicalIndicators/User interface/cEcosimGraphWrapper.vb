' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to to update the graph that reflects Ecospace biodiversity indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcosimGraphWrapper
    Inherits cZedGraphHelper

#Region " Private variables "

    ''' <summary>Indicator grouping etc as centrally defined in the plug-in.</summary>
    Private m_settings As cIndicatorSettings = Nothing
    ''' <summary>List of Ecosim indicators.</summary>
    Private m_lind As List(Of cEcosimIndicators) = Nothing

    ''' <summary>Current indicator group to display in the graph.</summary>
    Private m_groupCurrent As cIndicatorInfoGroup = Nothing
    ''' <summary>Current indicator to display in the graph.</summary>
    Private m_indCurrent As cIndicatorInfo = Nothing

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
                                lind As List(Of cEcosimIndicators))
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

    Public Sub RefreshContent(indSingle As cIndicatorInfo, indGroup As cIndicatorInfoGroup)

        Dim lInfo As New List(Of cIndicatorInfo)
        Dim info As cIndicatorInfo = Nothing
        Dim gp As GraphPane = Nothing
        Dim strLabelPane As String = ""
        Dim fmt As New EwECore.Style.cUnits(Me.UIContext.Core)
        Dim strLabelTime As String = fmt.ToString("[time]")
        Dim strLabelValue As String = ""
        Dim settings As cIndicatorSettings = Me.m_settings
        Dim ind As cEcosimIndicators = Nothing
        Dim ppl As PointPairList = Nothing
        Dim sValue As Single = 0

        If (indSingle Is Nothing) Then
            ' Group mode
            If Not ReferenceEquals(indGroup, Me.m_groupCurrent) Then
                For i As Integer = 0 To indGroup.NumIndicators - 1
                    lInfo.Add(indGroup.Indicator(i))
                Next
            End If
            strLabelPane = indGroup.Name
            ' Set master pane title
            Me.Configure(strLabelPane)
        Else
            ' Indicator mode
            If Not ReferenceEquals(indSingle, Me.m_indCurrent) Then
                lInfo.Add(indSingle)
            End If
            strLabelPane = indSingle.Name
            ' Set master pane title
            Me.Configure("")
        End If


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
                ' Make indicator panel pretty
                Me.ConfigurePane(info.Name, strLabelTime, strLabelValue, False, iPane:=iPane)
            Next
        End If

        Try
            ' Next populate all panels
            For iPane As Integer = 1 To Me.NumPanes

                Dim sXMin As Single = 0
                Dim sXMax As Single = 0
                Dim sYMin As Single = 0

                ' Get pane for indicator iInd
                gp = Me.GetPane(iPane)
                ' Prepare for determining axis range
                sXMin = Single.MaxValue : sXMax = Single.MinValue
                ' Prepare structures for creating point list for indicator
                info = DirectCast(gp.Tag, cIndicatorInfo)
                ' Check metadata
                sYMin = 0
                If (info.Metadata IsNot Nothing) Then
                    If (info.Metadata.Min >= 0) Then sYMin = info.Metadata.Min
                End If

                ppl = New PointPairList()
                Try
                    ' For all times
                    For iTime As Integer = 0 To Me.m_lind.Count - 1
                        ' Get indicator
                        ind = Me.m_lind(iTime)
                        ' Get indicator value
                        sValue = info.GetValue(ind)
                        ' Has a positive non-zero value?
                        If (sValue >= 0) Then
                            ' #Si: add point and update value min/max range
                            Dim pt As New PointPair(Me.Core.EcosimFirstYear + (ind.Time / cCore.N_MONTHS), sValue)
                            ppl.Add(pt)
                            sXMax = CSng(Math.Max(sXMax, pt.X))
                            sXMin = CSng(Math.Min(sXMin, pt.X))
                        End If
                    Next

                    ' No points added?
                    If ppl.Count = 0 Then
                        ' #Oui: clear the list
                        gp.CurveList.Clear()
                    Else
                        ' #Non: plot the line and configure the axis min/max range
                        Me.PlotLines(New LineItem() {Me.CreateLineItem(info.Name, eSketchDrawModeTypes.Line, Drawing.Color.Blue, ppl, info)}, iPane)
                        gp.XAxis.Scale.Min = sXMin
                        gp.XAxis.Scale.Max = sXMax

                        gp.YAxis.Scale.Min = sYMin
                        gp.YAxis.Scale.MinGrace = 0
                        gp.YAxis.Scale.MinAuto = False

                    End If

                    gp.AxisChange()

                Catch ex As Exception
                    ' Whoah
                    Debug.Assert(False, ex.Message)
                End Try

            Next iPane

        Catch ex As Exception
            ' Ouch
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region ' Refreshing

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
