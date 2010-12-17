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

        Private m_nTotalSteps As Integer
        Private m_nGroups As Integer
        Private m_pane As GraphPane = Nothing

        Private m_showGroupMode As RunEcospace.eShowGroupType = RunEcospace.eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = cCore.NULL_VALUE

        Public Sub Reset(ByVal nGroups As Integer, ByVal nTotalSteps As Integer)

            Dim li As LineItem = Nothing

            Me.m_pane = Me.ConfigurePane(My.Resources.ECOSPACE_HEADER_RELB, ScientificInterfaceShared.My.Resources.HEADER_TIME, _
                                         0, nTotalSteps, My.Resources.ECOSPACE_HEADER_LOGBREL, -1, 1, True)
            Me.m_nGroups = nGroups
            Me.m_nTotalSteps = nTotalSteps

            Me.m_pane.CurveList.Clear()
            For iGroup As Integer = 1 To nGroups
                li = Me.CreateLineItem(Me.Core.EcoPathGroupInputs(iGroup), New PointPairList())
                Me.m_pane.CurveList.Add(li)
            Next

            Me.Redraw()

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

    End Class

End Namespace ' Ecospace
