#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports System.ComponentModel
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls

    ''' <summary>
    ''' Derived Zedgraph helper class that just overrides the ToolTip formating for the EnvironmentalResponse graphs
    ''' </summary>
    ''' <remarks></remarks>
    <CLSCompliant(False)> _
    Public Class cZedGraphEnviroResponseHelper
        Inherits cZedGraphHelper

        Public Enum eEnvResponseLineType As Integer
            Response
            Histogram
        End Enum

        Public Shadows Function CreateLineItem(ByVal strName As String, ByVal ppl As ZedGraph.PointPairList, ByVal lineType As eEnvResponseLineType) As ZedGraph.LineItem
            Dim clr As Color
            Select Case lineType
                Case eEnvResponseLineType.Histogram : clr = Color.RoyalBlue
                Case eEnvResponseLineType.Response : clr = Color.SandyBrown
                Case Else : Debug.Assert(False)
            End Select
            Return MyBase.CreateLineItem(strName, Definitions.eLineType.NotSet, clr, ppl, lineType)
        End Function

        Protected Overrides Function FormatTooltip(ByVal pane As ZedGraph.GraphPane, ByVal curve As ZedGraph.CurveItem, ByVal iPoint As Integer) As String

            ' ToDo: localize this

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
                            Case Else
                                Debug.Assert(False, "Unsupported line type")
                        End Select
                    End If ' If TypeOf curve.Tag Is cCurveInfo Then
                End If ' If curve.Tag IsNot Nothing Then

                If bUseBase Then
                    Return MyBase.FormatTooltip(pane, curve, iPoint)
                End If

                Debug.Assert(curve.IsLine, "ToolTip wrong line type.")

                ' ToDo: localize this
                Dim sb As New System.Text.StringBuilder()
                sb.AppendLine("Capacity for Map input.")

                Dim pp As PointPair = curve(iPoint)
                sb.AppendLine("Map input " & Me.StyleGuide.FormatNumber(pp.X))
                sb.AppendLine("Capacity multiplier" & Me.StyleGuide.FormatNumber(pp.Y))
                Return sb.ToString
            Catch ex As Exception

            End Try
            Return ""

        End Function

    End Class

End Namespace
