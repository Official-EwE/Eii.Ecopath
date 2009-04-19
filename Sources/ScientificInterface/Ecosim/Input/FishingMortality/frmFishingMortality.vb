'==============================================================================
'
' $Log: frmFishingMortality.vb,v $
' Revision 1.6  2009/04/19 13:40:40  jeroens
' Removed sketchpad toolbar
'
' Revision 1.5  2009/03/02 01:52:36  jeroens
' Properly named handlers
'
' Revision 1.4  2009/02/05 17:48:36  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.3  2009/01/16 18:30:41  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 16:03:01  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.1  2008/09/26 07:31:36  sherman
' --== DELETED HISTORY ==--
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
    Public Class frmFishingMortality

#Region " Private variables "

        Private m_Core As cCore = Nothing
        Private m_handler As cFishingMortalityShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            m_Core = cCore.GetInstance()

            Me.m_handler = New cFishingMortalityShapeGUIHandler(Me.m_Core, _
                    Me.m_shapeToolBox, Me.m_sketchPad, _
                    Nothing, Me.m_sketchPadToolbar)

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
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

        Private Sub frmFishingRate_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.CoreComponents = Nothing
        End Sub

#End Region ' Private event handlers

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Select Case msg.Source
                Case eCoreComponentType.ShapesManager
                    If (msg.DataType = eDataTypes.FishMort) Then
                        Me.m_handler.Refresh()
                    End If
            End Select
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


