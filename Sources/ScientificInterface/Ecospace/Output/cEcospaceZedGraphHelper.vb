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

            ' ToDo_JS: localize this
            Me.m_pane = Me.ConfigurePane("", "Time steps", 0, nTotalSteps, "Biomass (log, relative)", -1, 1, True)
            Me.m_nGroups = nGroups
            Me.m_nTotalSteps = nTotalSteps

            Me.m_pane.CurveList.Clear()
            For iGroup As Integer = 1 To nGroups
                li = Me.CreateLineItem(eLineType.ModelData, iGroup, New PointPairList())
                li.Tag = iGroup
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

        Public Sub UpdateCurveVisibility()

            Dim li As CurveItem = Nothing
            Dim bShowGroup As Boolean = True

            Try
                For iGroup As Integer = 1 To Me.m_nGroups

                    li = Me.m_pane.CurveList(iGroup - 1)

                    Select Case Me.GroupShowMode
                        Case RunEcospace.eShowGroupType.ShowAll
                            bShowGroup = True
                        Case RunEcospace.eShowGroupType.ShowNonHidden
                            bShowGroup = Me.StyleGuide.GroupVisible(iGroup)
                        Case RunEcospace.eShowGroupType.ShowSingle
                            bShowGroup = (iGroup = Me.m_iGroupToShow)
                    End Select

                    li.IsVisible = bShowGroup
                Next
            Catch ex As Exception

            End Try
        End Sub

        Public Property GroupShowMode() As RunEcospace.eShowGroupType
            Get
                Return Me.m_showGroupMode
            End Get
            Set(ByVal value As RunEcospace.eShowGroupType)
                Me.m_showGroupMode = value
            End Set
        End Property

        Public Property GroupToShow() As Integer
            Get
                Return Me.m_iGroupToShow
            End Get
            Set(ByVal value As Integer)
                Me.m_iGroupToShow = value
            End Set
        End Property

    End Class

End Namespace ' Ecospace
