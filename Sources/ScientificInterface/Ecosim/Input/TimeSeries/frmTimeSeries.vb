#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Text.RegularExpressions
Imports ScientificInterface.Other
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form class implementing the Ecosim 'Time Series' interface. 
    ''' </summary>
    ''' =======================================================================
    Public Class frmTimeSeries

#Region " Private variables "

        ''' <summary></summary>
        Private m_handler As cTimeSeriesShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region

#Region " Overrides "

        ''' <summary>
        ''' The Form's Load event. This method initialized the value of the controls in
        ''' the interface
        ''' </summary>
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Hook up message sources
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}

            Me.m_handler = New cTimeSeriesShapeGUIHandler()
            Me.m_handler.Attach(Me.UIContext, Me.m_shapeToolbox, Me.m_shapeToolboxToolbar, Me.m_sketchPad, Me.m_sketchPadToolbar)

            ' Once hooked up, try to get TS if not here yet
            If Not Me.UIContext.Core.HasTimeSeries Then
                Dim cmdh As cCommandHandler = Me.CommandHandler
                Dim cmd As cCommand = cmdh.GetCommand("LoadTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.Invoke()
                End If
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_handler.Detach()
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Overrides

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If ((msg.Source = eCoreComponentType.TimeSeries) And _
                (msg.Type = eMessageType.DataAddedOrRemoved Or msg.Type = eMessageType.DataModified)) Then
                ' Refresh content
                Me.m_handler.Refresh()
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


