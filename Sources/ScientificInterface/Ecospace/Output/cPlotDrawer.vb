'==============================================================================
'
' $Log: cPlotDrawer.vb,v $
' Revision 1.1  2008/09/26 07:32:02  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/08/22 00:20:26  joeh
' Fix graph overlay. Overlay is possible now.
'
' Revision 1.3  2008/07/02 18:50:42  jeroens
' Plot responds to group show selections
'
' Revision 1.2  2008/07/02 18:07:21  jeroens
' Inversed Y axis
'
' Revision 1.1  2008/05/13 17:47:54  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Option Explicit On

Imports System
Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterface.Other

Namespace Ecospace

    Public Class cPlotDrawer

        Private m_core As cCore = Nothing
        Private m_nTotalSteps As Integer
        Private m_nGroups As Integer
        Private m_asValues As Single(,)
        Private m_agpLines As GraphicsPath()

        Private m_showGroupMode As RunEcospace.eShowGroupType = RunEcospace.eShowGroupType.ShowAll
        Private m_iGroupToShow As Integer = cCore.NULL_VALUE

        Public Sub New(ByVal core As cCore)
            Me.m_core = core
        End Sub

        Public Sub Reset(ByVal nGroups As Integer, ByVal nTotalSteps As Integer)

            Dim gp As GraphicsPath = Nothing

            Me.m_nGroups = nGroups
            Me.m_nTotalSteps = nTotalSteps

            ReDim Me.m_agpLines(nGroups)
            ReDim Me.m_asValues(nGroups, nTotalSteps)

            For igroup As Integer = 1 To nGroups
                gp = New GraphicsPath()
                Me.m_agpLines(igroup) = gp
            Next

        End Sub

        Public Sub Overlay(ByVal nGroups As Integer)
            For igroup As Integer = 1 To nGroups
                Me.m_agpLines(igroup).StartFigure()
            Next
        End Sub

        Public Sub AddValue(ByVal iGroup As Integer, ByVal iTimeStep As Integer, ByVal sValue As Single)

            Try

                Me.m_asValues(iGroup, iTimeStep) = sValue
                If iTimeStep > 1 Then
                    Me.m_agpLines(iGroup).AddLine(iTimeStep - 1, Me.m_asValues(iGroup, iTimeStep - 1), iTimeStep, sValue)
                End If
            Catch ex As Exception

            End Try

        End Sub

        Public Sub Draw(ByVal g As Graphics, ByVal rc As Rectangle)
            Dim mx As New Matrix()
            Dim mxTmp As Matrix = g.Transform
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim bShowGroup As Boolean = True

            mx.Translate(0, CSng(rc.Height / 2))
            mx.Scale(CSng(rc.Width / Me.m_nTotalSteps), -CSng(rc.Height / 2))
            g.Transform = mx

            Try
                For iGroup As Integer = 1 To Me.m_nGroups

                    Select Case Me.m_showGroupMode
                        Case RunEcospace.eShowGroupType.ShowAll
                            bShowGroup = True
                        Case RunEcospace.eShowGroupType.ShowNonHidden
                            bShowGroup = appl.GroupDisplayFlags(iGroup)
                        Case RunEcospace.eShowGroupType.ShowSingle
                            bShowGroup = (iGroup = Me.m_iGroupToShow)
                    End Select

                    If bShowGroup Then
                        ' Use a tiny pen size to avoid scaling anomalies
                        Using p As New Pen(sg.GroupColor(Me.m_core, iGroup), 0.00001)
                            g.DrawPath(p, Me.m_agpLines(iGroup))
                        End Using
                    End If
                Next
            Catch ex As Exception

            End Try

            g.Transform = mxTmp
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
