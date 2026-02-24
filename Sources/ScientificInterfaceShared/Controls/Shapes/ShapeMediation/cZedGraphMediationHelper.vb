' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports ZedGraph

Namespace Controls

    ''' <summary>
    ''' Derived Zedgraph helper class that just overrides the ToolTip formating for the EnvironmentalResponse graphs
    ''' </summary>
    ''' <remarks></remarks>

    Public Class cZedGraphMediationHelper
        Inherits cZedGraphHelper

        Public Enum eEnvResponseLineType As Integer
            Response
            Histogram
            Baseline
        End Enum

        Public Shadows Function CreateLineItem(strName As String, ppl As ZedGraph.PointPairList, lineType As eEnvResponseLineType) As ZedGraph.LineItem
            Dim clr As Color
            Select Case lineType
                Case eEnvResponseLineType.Histogram : clr = Color.Gray
                Case eEnvResponseLineType.Response : clr = Color.SandyBrown
                Case eEnvResponseLineType.Baseline : clr = Color.Red
                Case Else : Debug.Assert(False)
            End Select
            Return MyBase.CreateLineItem(strName, Definitions.eSketchDrawModeTypes.Line, clr, ppl, lineType)
        End Function

        Protected Overrides Function FormatTooltip(pane As ZedGraph.GraphPane, curve As ZedGraph.CurveItem, iPoint As Integer) As String

            'This is not a very good way to do this 
            'It may be better to not use a tool tip at all 
            'instead pass out the X and Y Axis value(s) and let the container figure out how to show the data
            Try

                Dim bUseBase As Boolean = True

                If curve.Tag IsNot Nothing Then
                    If TypeOf curve.Tag Is cCurveInfo Then
                        Dim ci As cCurveInfo = DirectCast(curve.Tag, cCurveInfo)
                        Dim tag As eEnvResponseLineType = DirectCast(ci.Tag, eEnvResponseLineType)

                        Select Case tag
                            Case eEnvResponseLineType.Response
                                bUseBase = False
                            Case eEnvResponseLineType.Histogram
                                Return ""
                            Case eEnvResponseLineType.Baseline
                                ' NOP
                            Case Else
                                Debug.Assert(False, "Unsupported line type")
                        End Select
                    End If ' If TypeOf curve.Tag Is cCurveInfo Then
                End If ' If curve.Tag IsNot Nothing Then

                If bUseBase Then
                    Return MyBase.FormatTooltip(pane, curve, iPoint)
                End If

                Debug.Assert(curve.IsLine, "ToolTip wrong line type.")

                Dim sb As New System.Text.StringBuilder()
                sb.AppendLine(My.Resources.TOOLTIP_MEDIATION_CAPACITY)

                Dim pp As PointPair = curve(iPoint)
                sb.AppendLine(cStringUtils.Localize(My.Resources.TOOLTIP_MEDIATION_DRIVER_VALUE, Me.StyleGuide.FormatNumber(pp.X)))
                sb.AppendLine(cStringUtils.Localize(My.Resources.TOOLTIP_MEDIATION_CAPACITY_MULTIPLIER, Me.StyleGuide.FormatNumber(pp.Y)))
                Return sb.ToString
            Catch ex As Exception

            End Try
            Return ""

        End Function

    End Class

End Namespace
