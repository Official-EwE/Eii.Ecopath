'==============================================================================
'
' $Log: frmTimeSeries.vb,v $
' Revision 1.2  2008/12/15 16:03:02  jeroens
' Shape controls moved to ScIntShared
'
' Revision 1.1  2008/09/26 07:31:45  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/09/23 16:09:47  jeroens
' Load Time Series invoked if form opened w/o TS present
'
' Revision 1.1  2007/10/29 13:54:07  jeroens
' *** empty log message ***
'
' Revision 1.12  2007/10/15 15:25:28  jeroens
' * Filters incoming messages for appropriate source and type
'
' Revision 1.11  2007/10/14 18:21:33  jeroens
' - Released post-mortem event handlers
'
' Revision 1.10  2007/10/12 20:19:11  jeroens
' * Inherited from EwEForm
'
' Revision 1.9  2007/10/07 03:33:33  jeroens
' * Removed Time series season/long-term option; is not supported by the core
'
' Revision 1.8  2007/08/20 15:31:40  jeroens
' - Disabled Options dlg for Time Series
'
' Revision 1.7  2007/07/30 15:11:24  jeroens
' * Disabled reset option on sketch pad menu
'
' Revision 1.6  2007/07/18 15:11:19  jeroens
' * Uses SketchPad.Editable flag to disable mouse interaction
'
' Revision 1.5  2007/07/18 05:15:17  joeh
' Use the newly added TimeSeriesSketchPad.vb
'
' Revision 1.4  2007/07/14 00:03:01  joeh
' Instantiate TimeSeriesToolboxHandler class
'
' Revision 1.3  2007/07/13 17:24:35  jeroens
' - Removed Forcing namespace
'
' Revision 1.2  2007/07/13 16:33:32  jeroens
' * Fixed build
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
        Private m_stbHandler As ShapeGUIHandler = Nothing

#End Region

#Region "Constructors"
        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Get the only core reference
            m_core = cCore.GetInstance()

            Me.m_stbHandler = New TimeSeriesShapeGUIHandler(Me.m_core, _
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
            Me.MessageSources = New eMessageSource() {eMessageSource.TimeSeries}

            ' Once hooked up, try to get TS if not here yet
            If Not Me.m_core.HasTimeSeries Then
                Dim cmdh As CommandHandler = CommandHandler.GetInstance
                Dim cmd As Command = cmdh.GetCommand("LoadTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.Invoke()
                End If
            End If

        End Sub

        Private Sub TimeSeries_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.MessageSources = Nothing
        End Sub

#End Region ' Event handlers

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If ((msg.Source = eMessageSource.TimeSeries) And _
                (msg.Type = eMessageType.DataAddedOrRemoved Or msg.Type = eMessageType.DataModified)) Then
                ' Refresh content
                Me.m_stbHandler.Refresh()
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


