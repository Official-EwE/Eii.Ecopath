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

#Region "Private variables"

        ''' <summary></summary>
        Private m_stbHandler As cShapeGUIHandler = Nothing

#End Region

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region

#Region " Event handlers "

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                MyBase.UIContext = value
                Me.m_sketchPad.UIContext = value
                Me.m_stbHandler = New cTimeSeriesShapeGUIHandler(Me.UIContext, _
                        Me.m_shapeToolbox, Me.m_shapeToolboxToolbar, _
                        Me.m_sketchPad, Me.m_sketchPadToolbar)
            End Set
        End Property

        ''' <summary>
        ''' The Form's Load event. This method initialized the value of the controls in
        ''' the interface
        ''' </summary>
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            ' Hook up message sources
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}

            ' Once hooked up, try to get TS if not here yet
            If Not Me.UIContext.Core.HasTimeSeries Then
                Dim cmdh As cCommandHandler = cCommandHandler.GetInstance
                Dim cmd As cCommand = cmdh.GetCommand("LoadTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.Invoke()
                End If
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
            MyBase.OnFormClosed(e)
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


