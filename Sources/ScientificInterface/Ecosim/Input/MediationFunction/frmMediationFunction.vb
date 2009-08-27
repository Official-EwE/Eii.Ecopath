'==============================================================================
'
' $Log: frmMediationFunction.vb,v $
' Revision 1.8  2009/03/24 20:28:35  jeroens
' Uses mediation tool bar
'
' Revision 1.7  2009/03/24 16:34:19  jeroens
' Fixed crash on Edit w/o shape selection
'
' Revision 1.6  2009/03/20 17:55:41  jeroens
' Shape controls are multiple selection
'
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
' Revision 1.1  2008/09/26 07:31:38  sherman
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
Imports ScientificInterfaceShared

#End Region ' Imports

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing all functionality to add, remove and edit
    ''' <see cref="cMediationFunction">mediation shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmMediationFunction

#Region "Private variables"

        ''' <summary>Reference to the core class</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Controller for shape-related GUI components in this form.</summary>
        Private m_shapeguihandler As cShapeGUIHandler = Nothing

#End Region

#Region "Constructors"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            m_core = cCore.GetInstance()

            Me.m_shapeguihandler = New cMediationShapeGUIHandler(Me.m_core, _
                    Me.m_shapeToolBox, Me.m_shapeToolboxToolbar, _
                    Me.m_sketchPad, Me.m_sketchPadToolbar, _
                    Me.m_bioPercent, Me.m_biopercenttoolbar)

        End Sub

        Public Sub New(ByVal text As String)
            Me.New()
            ' Set the tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text
        End Sub

#End Region

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; implemented to make sure that this form receives 
        ''' <see cref="cMessage">messages</see> from specific 
        ''' <see cref="eCoreComponentType">message sources</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; implemented to make sure that this form stops receiving
        ''' <see cref="cMessage">messages</see> from specific 
        ''' <see cref="eCoreComponentType">message sources</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Me.CoreComponents = Nothing
            MyBase.OnFormClosed(e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub tsBtnEditBioPert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)



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
                     (msg.DataType = eDataTypes.Mediation)) Then
                    Me.m_shapeguihandler.Refresh()
                End If
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace


