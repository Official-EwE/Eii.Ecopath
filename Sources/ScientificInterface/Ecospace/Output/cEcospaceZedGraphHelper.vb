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
Imports EwEUtils.Utilities
Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' <see cref="cZedGraphHelper">ZedGraph helper</see>-derived class to
    ''' make Ecospace plots look a lot more pretty.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class cEcospaceZedGraphHelper
        Inherits cZedGraphHelper

#Region " Private variables "

        Private m_nTotalSteps As Integer = 0
        Private m_iFirstYear As Integer = 0
        Private m_sNumStepsPerYear As Single = 0.0!
        Private m_nGroups As Integer = 0
        Private m_pane As GraphPane = Nothing

#End Region ' Private variables

#Region " Overrides "

        Public Overrides Sub Attach(uic As ScientificInterfaceShared.Controls.cUIContext, zgc As ZedGraph.ZedGraphControl, Optional iNumPanels As Integer = 1)
            MyBase.Attach(uic, zgc, iNumPanels)
            For i As Integer = 0 To Me.NumPanes - 1
                AddHandler Me.GetPane(i + 1).XAxis.ScaleFormatEvent, AddressOf Me.XScaleFormatEvent
                AddHandler Me.GetPane(i + 1).YAxis.ScaleFormatEvent, AddressOf Me.YScaleFormatEvent
            Next
        End Sub

        Public Overrides Sub Detach()
            For i As Integer = 0 To Me.NumPanes - 1
                RemoveHandler Me.GetPane(i + 1).XAxis.ScaleFormatEvent, AddressOf Me.XScaleFormatEvent
                RemoveHandler Me.GetPane(i + 1).YAxis.ScaleFormatEvent, AddressOf Me.YScaleFormatEvent
            Next
            MyBase.Detach()
        End Sub

        Protected Overrides Function FormatTooltipValue(pane As GraphPane, curve As CurveItem, iPoint As Integer) As String
            Dim pp As PointPair = curve(iPoint)
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_POINT, curve.Label.Text, Me.FormatXValue(pp.X), Me.FormatYValue(pp.Y))
        End Function

        Protected Overrides Function IsCurveVisible(ci As ZedGraph.CurveItem) As Boolean
            Dim info As cCurveInfo = Me.CurveInfo(ci)
            Select Case Me.ItemShowMode
                Case frmRunEcospace.eShowItemType.ShowAll
                    Return True
                Case frmRunEcospace.eShowItemType.ShowCustom
                    Return MyBase.IsCurveVisible(ci)
                Case frmRunEcospace.eShowItemType.ShowSingle
                    Return (info.Index = Me.ItemToShow)
            End Select
            Return True
        End Function

#End Region ' Overrides

#Region " Public methods "

        Public Sub Reset(strTitle As String, strYAxisLabel As String, nGroups As Integer, nTotalSteps As Integer, iFirstYear As Integer, sNumStepsPerYear As Single)

            Dim li As LineItem = Nothing
            Dim YMin As Single, YMax As Single
            If Me.LogScale Then
                YMin = -1
                YMax = 1
            End If

            Me.m_pane = Me.ConfigurePane(strTitle,
                                         ScientificInterfaceShared.My.Resources.HEADER_YEAR,
                                         0, nTotalSteps, strYAxisLabel, YMin, YMax, False)
            'Auto Scale the Y Axis if not using a log scale
            Me.m_pane.YAxis.Scale.MaxAuto = (Not Me.LogScale)

            Me.m_nGroups = nGroups
            Me.m_nTotalSteps = nTotalSteps
            Me.m_iFirstYear = iFirstYear
            Me.m_sNumStepsPerYear = sNumStepsPerYear

            Me.m_pane.CurveList.Clear()
            For iGroup As Integer = 1 To nGroups
                li = Me.CreateLineItem(Me.Core.EcoPathGroupInputs(iGroup), New PointPairList())
                Me.m_pane.CurveList.Add(li)
            Next

            Me.RescaleAndRedraw(1)

        End Sub

        Public Sub Overlay(nGroups As Integer)
            'For igroup As Integer = 1 To nGroups
            '    Me.m_agpLines(igroup).StartFigure()
            'Next
        End Sub


        Public Sub AddValue(iGroup As Integer, iTimeStep As Integer, sValue As Single)

            If Not cNumberUtils.IsFinite(sValue) Then
                cNumberUtils.FixValue(sValue)
#If DEBUG Then
                If 2 = 3 Then
                    Debug.Assert(False, "Point contains invalid values")
                End If
#End If
            End If
            Try
                Dim li As CurveItem = Me.m_pane.CurveList(iGroup - 1)
                li.AddPoint(iTimeStep, sValue)
            Catch ex As Exception

            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group or fleet show mode. Note that this will not refresh the graph;
        ''' the calling process will have to invoke <see cref="UpdateCurveVisibility">UpdateCurveVisibility</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ItemShowMode() As frmRunEcospace.eShowItemType = frmRunEcospace.eShowItemType.ShowAll

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group to show. Note that this will not refresh the graph;
        ''' the calling process will have to invoke <see cref="UpdateCurveVisibility">UpdateCurveVisibility</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ItemToShow() As Integer = cCore.NULL_VALUE

        Public Property LogScale() As Boolean = True

#End Region ' Public methods

#Region " Internals "

        Private Function XScaleFormatEvent(pane As GraphPane, axis As Axis, dValue As Double, iIndex As Integer) As String
            Return FormatXValue(dValue)
        End Function

        Private Function YScaleFormatEvent(pane As GraphPane, axis As Axis, dValue As Double, iIndex As Integer) As String
            Return Me.FormatYValue(dValue)
        End Function

        Private Function FormatXValue(dValue As Double) As String
            Dim d As DateTime = Me.Core.EcospaceTimestepToAbsoluteTime(CInt(Math.Max(1, Math.Round(dValue))))
            Return d.ToShortDateString()
        End Function

        Private Function FormatYValue(dValue As Double) As String
            If Me.LogScale Then
                Return Me.StyleGuide.FormatNumber(Math.Pow(10, dValue))
            Else
                Return Me.StyleGuide.FormatNumber(dValue)
            End If
        End Function

#End Region ' Internals

    End Class

End Namespace ' Ecospace
