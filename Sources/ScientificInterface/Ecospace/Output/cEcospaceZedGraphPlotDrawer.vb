'==============================================================================
'
' $Log: cEcospaceZedGraphPlotDrawer.vb,v $
' Revision 1.3  2009/05/28 12:37:10  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.2  2009/04/22 00:56:48  jeroens
' Getting pretty
'
' Revision 1.1  2009/04/21 17:59:50  jeroens
' Initial version
'
'==============================================================================

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

    <CLSCompliant(False)> _
    Public Class cEcospaceZedGraphPlotDrawer

        Private m_core As cCore = Nothing
        Private m_nTotalSteps As Integer
        Private m_nGroups As Integer
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_sg As cStyleGuide = Nothing
        Private m_pane As GraphPane = Nothing

        Private m_showGroupMode As RunEcospace.eShowGroupType = RunEcospace.eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = cCore.NULL_VALUE

        Public Sub New(ByVal core As cCore, ByVal zgh As cZedGraphHelper)
            Me.m_core = core
            Me.m_zgh = zgh
            Me.m_sg = cStyleGuide.GetInstance()
        End Sub

        Public Sub Reset(ByVal nGroups As Integer, ByVal nTotalSteps As Integer)

            Dim li As LineItem = Nothing

            Me.m_pane = Me.m_zgh.ConfigurePane("", "Time", 0, nTotalSteps, "Biomass", -1, 1, False)
            Me.m_nGroups = nGroups
            Me.m_nTotalSteps = nTotalSteps

            Me.m_pane.CurveList.Clear()
            For iGroup As Integer = 1 To nGroups
                li = Me.m_zgh.CreateLineItem(cZedGraphHelper.eCurveTypes.EcosimOutput, iGroup, New PointPairList())
                li.Tag = iGroup
                Me.m_pane.CurveList.Add(li)
            Next

            Me.m_zgh.Redraw()

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

        Public Sub UpodateCurveVisibility()

            Dim li As CurveItem = Nothing
            Dim bShowGroup As Boolean = True

            Try
                For iGroup As Integer = 1 To Me.m_nGroups

                    li = Me.m_pane.CurveList(iGroup - 1)

                    Select Case Me.m_showGroupMode
                        Case RunEcospace.eShowGroupType.ShowAll
                            bShowGroup = True
                        Case RunEcospace.eShowGroupType.ShowNonHidden
                            bShowGroup = Me.m_sg.GroupVisible(iGroup)
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
