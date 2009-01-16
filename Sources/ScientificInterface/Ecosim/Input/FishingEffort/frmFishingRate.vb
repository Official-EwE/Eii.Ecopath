'==============================================================================
'
' $Log: frmFishingRate.vb,v $
' Revision 1.3  2009/01/16 18:30:41  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 16:03:01  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.1  2008/09/26 07:31:36  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/07/01 19:13:10  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.6.2.2  2008/07/01 18:36:28  sherman
' Merged Fix_Ecopat_EcosimUpdate...
'
' Revision 1.6  2008/06/06 16:01:38  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.5  2008/05/29 23:43:43  jeroens
' Added Values dialog
'
' Revision 1.4  2008/02/06 21:11:01  jeroens
' Fixed issue 398
'
' Revision 1.3  2007/10/31 16:04:15  jeroens
' * Respond to shape manager messages
'
' Revision 1.2  2007/10/30 02:45:36  jeroens
' * Debugged
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Text.RegularExpressions
Imports ScientificInterface.Other
Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' <summary>
    ''' </summary>
    Public Class frmFishingRate

#Region " Private variables "

        Private m_Core As cCore = Nothing
        Private m_handler As FishingRateShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            m_Core = cCore.GetInstance()

            Me.m_handler = New FishingRateShapeGUIHandler(Me.m_Core, _
                    Me.m_shapeToolBox, Me.m_sketchPad, _
                    Me.m_shapeToolboxToolbar, Me.m_sketchPadToolbar)

        End Sub

        Public Sub New(ByVal text As String)

            Me.New()
            'Set the tab title
            Me.TabText = text
            ' Set the windows text
            Me.Text = text

        End Sub

#End Region ' Constructors

#Region " Private event handlers "

        Private Sub frmFishingRate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.MessageSources = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

        Private Sub frmFishingRate_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.MessageSources = Nothing
        End Sub

#End Region ' Private event handlers

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Select Case msg.Source
                Case eCoreComponentType.ShapesManager
                    If (msg.DataType = eDataTypes.FishingRate) Then
                        Me.m_handler.Refresh()
                    End If
            End Select
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


