#Region " Imports "

Option Strict On
Imports System
Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterface.Other
Imports ScientificInterfaceShared.Style
Imports ZedGraph

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

        Private m_nTotalSteps As Integer = 0
        Private m_iFirstYear As Integer = 0
        Private m_sNumStepsPerYear As Single = 0.0!
        Private m_nGroups As Integer = 0
        Private m_pane As GraphPane = Nothing

        Private m_showGroupMode As RunEcospace.eShowGroupType = RunEcospace.eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = cCore.NULL_VALUE

        Public Overrides Sub Attach(ByVal uic As ScientificInterfaceShared.Controls.cUIContext, ByVal zgc As ZedGraph.ZedGraphControl, Optional ByVal iNumPanels As Integer = 1)
            MyBase.Attach(uic, zgc, iNumPanels)
            For i As Integer = 0 To Me.NumPanes - 1
                AddHandler Me.GetPane(i + 1).XAxis.ScaleFormatEvent, AddressOf XScaleFormatEvent
            Next
        End Sub

        Public Overrides Sub Detach()
            For i As Integer = 0 To Me.NumPanes - 1
                RemoveHandler Me.GetPane(i + 1).XAxis.ScaleFormatEvent, AddressOf XScaleFormatEvent
            Next
            MyBase.Detach()
        End Sub

        Public Sub Reset(ByVal nGroups As Integer, _
                         ByVal nTotalSteps As Integer, _
                         ByVal iFirstYear As Integer, _
                         ByVal sNumStepsPerYear As Single)

            Dim li As LineItem = Nothing

            Me.m_pane = Me.ConfigurePane(My.Resources.ECOSPACE_HEADER_RELB, _
                                         ScientificInterfaceShared.My.Resources.HEADER_YEAR, _
                                         0, nTotalSteps, My.Resources.ECOSPACE_HEADER_LOGBREL, -1, 1, True)
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

        Public Sub Overlay(ByVal nGroups As Integer)
            'For igroup As Integer = 1 To nGroups
            '    Me.m_agpLines(igroup).StartFigure()
            'Next
        End Sub

        Public Sub AddValue(ByVal iGroup As Integer, ByVal iTimeStep As Integer, ByVal sValue As Single)
            Try
                Dim li As CurveItem = Me.m_pane.CurveList(iGroup - 1)
                li.AddPoint(iTimeStep, sValue)
            Catch ex As Exception

            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group show mode. Note that this will not refresh the graph;
        ''' the calling process will have to invoke <see cref="UpdateCurveVisibility">UpdateCurveVisibility</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property GroupShowMode() As RunEcospace.eShowGroupType
            Get
                Return Me.m_showGroupMode
            End Get
            Set(ByVal value As RunEcospace.eShowGroupType)
                Me.m_showGroupMode = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group to show. Note that this will not refresh the graph;
        ''' the calling process will have to invoke <see cref="UpdateCurveVisibility">UpdateCurveVisibility</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property GroupToShow() As Integer
            Get
                Return Me.m_iGroupToShow
            End Get
            Set(ByVal value As Integer)
                Me.m_iGroupToShow = value
            End Set
        End Property

        Protected Overrides Function IsCurveVisible(ByVal ci As ZedGraph.CurveItem) As Boolean

            Dim info As cCurveInfo = Me.CurveInfo(ci)

            Select Case Me.GroupShowMode
                Case RunEcospace.eShowGroupType.ShowAll
                    Return True
                Case RunEcospace.eShowGroupType.ShowNonHidden
                    Return MyBase.IsCurveVisible(ci)
                Case RunEcospace.eShowGroupType.ShowSingle
                    Return (info.Index = Me.m_iGroupToShow)
            End Select

            Return True

        End Function

        Private Function XScaleFormatEvent(ByVal pane As GraphPane, _
                                           ByVal axis As Axis, _
                                           ByVal dValue As Double, _
                                           ByVal iIndex As Integer) As String
            Dim sNumStepsPerYear As Single = Me.m_sNumStepsPerYear
            Dim sYear As Single = 0.0!

            If (sNumStepsPerYear <= 0.0!) Then sNumStepsPerYear = 1.0!
            sYear = Me.m_iFirstYear + CSng(dValue / sNumStepsPerYear)
            Return sYear.ToString

        End Function

    End Class

End Namespace ' Ecospace
