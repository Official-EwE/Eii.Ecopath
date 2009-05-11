'==============================================================================
'
' $Log: frmTimeSeries.vb,v $
' Revision 1.7  2009/05/11 01:50:53  jeroens
' Renamed command classes
'
' Revision 1.6  2009/04/16 17:38:00  jeroens
' --
'
' Revision 1.5  2009/03/02 01:52:34  jeroens
' Properly named handlers
'
' Revision 1.4  2009/02/05 17:48:37  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.3  2009/01/16 18:30:43  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 16:03:02  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.1  2008/09/26 07:31:45  sherman
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
Imports EwEUtils.Commands
Imports ScientificInterfaceShared

#End Region ' Imports

Namespace Ecosim

    ''' <summary>
    ''' This class contains all the event handling codes relating to the forcing function 
    ''' interface. 
    ''' </summary>
    Public Class frmTimeSeries

#Region "Private variables"

        ''' <summary>Reference to the core class.</summary>
        Private m_core As cCore
        ''' <summary></summary>
        Private m_stbHandler As cShapeGUIHandler = Nothing

#End Region

#Region "Constructors"
        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            m_core = cCore.GetInstance()

            Me.m_stbHandler = New cTimeSeriesShapeGUIHandler(Me.m_core, _
                    Me.m_shapeToolbox, Me.m_shapeToolboxToolbar, _
                    Me.m_sketchPad, Me.m_sketchPadToolbar)

        End Sub

        Public Sub New(ByVal text As String)

            Me.New()
            'Set the tab title
            Me.TabText = text
            ' Set the windows text
            Me.Text = text

        End Sub
#End Region

#Region " Event handlers "

        ''' <summary>
        ''' The Form's Load event. This method initialized the value of the controls in
        ''' the interface
        ''' </summary>
        Private Sub TimeSeries_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Hook up message sources
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}

            ' Once hooked up, try to get TS if not here yet
            If Not Me.m_core.HasTimeSeries Then
                Dim cmdh As cCommandHandler = cCommandHandler.GetInstance
                Dim cmd As cCommand = cmdh.GetCommand("LoadTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.Invoke()
                End If
            End If

        End Sub

        Private Sub TimeSeries_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.CoreComponents = Nothing
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If ((msg.Source = eCoreComponentType.TimeSeries) And _
                (msg.Type = eMessageType.DataAddedOrRemoved Or msg.Type = eMessageType.DataModified)) Then
                ' Refresh content
                Me.m_stbHandler.Refresh()
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


