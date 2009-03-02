'==============================================================================
'
' $Log: frmForcingFunction.vb,v $
' Revision 1.5  2009/03/02 01:52:35  jeroens
' Properly named handlers
'
' Revision 1.4  2009/02/05 17:48:37  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.3  2009/01/16 18:30:42  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 16:03:02  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.1  2008/09/26 07:31:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/07/04 15:27:18  jeroens
' Fixed update issue
'
' Revision 1.3  2008/06/06 16:01:38  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.2  2007/11/22 18:15:26  jeroens
' * Made proper EwEForm
'
' Revision 1.1  2007/10/29 13:28:30  jeroens
' Renamed, being reworked
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
Imports ScientificInterfaceShared

#End Region

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing all functionality to add, remove and edit
    ''' <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmForcingFunction

#Region " Private variables "

        ''' <summary>Reference to the core class</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Controller for shape-related GUI components in this form.</summary>
        Private m_shapeguihandler As cShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            Me.m_core = cCore.GetInstance()

            Me.m_shapeguihandler = New cForcingShapeGUIHandler(Me.m_core, _
                    Me.m_shapeToolbox, Me.m_shapeToolboxToolbar, _
                    Me.m_sketchPad, Me.m_sketchPadToolbar)

        End Sub

        Public Sub New(ByVal strText As String)

            Me.New()

            'Set the tab title
            Me.TabText = strText
            ' Set the windows text
            Me.Text = strText

        End Sub

#End Region ' Constructors

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; implemented to make sure that this form receives 
        ''' <see cref="cMessage">messages</see> from specific 
        ''' <see cref="eCoreComponentType">message sources</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub frmForcingFunction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; implemented to make sure that this form stops receiving
        ''' <see cref="cMessage">messages</see> from specific 
        ''' <see cref="eCoreComponentType">message sources</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub frmForcingFunction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.CoreComponents = Nothing
        End Sub

#End Region ' Events 

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic EwEForm message handler; implemented to respond to Forcing 
        ''' Function shape changes.
        ''' </summary>
        ''' <param name="msg">Incoming core <see cref="cMessage">messages</see>.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            If msg.Source = eCoreComponentType.ShapesManager Then
                If (((msg.Type = eMessageType.DataAddedOrRemoved) Or (msg.Type = eMessageType.DataModified)) And _
                     (msg.DataType = eDataTypes.Forcing)) Then
                    Me.m_shapeguihandler.Refresh()
                End If
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace


