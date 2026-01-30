' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On


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
        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Hook up message sources
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}

            Me.m_handler = New cTimeSeriesShapeGUIHandler(Me.UIContext)
            Me.m_handler.Attach(Me.m_shapeToolbox, Me.m_shapeToolboxToolbar, Me.m_sketchPad, Me.m_sketchPadToolbar)

            ' Once hooked up, try to get TS if not here yet
            If Not Me.UIContext.Core.HasTimeSeries Then
                Dim cmdh As cCommandHandler = Me.CommandHandler
                Dim cmd As cCommand = cmdh.GetCommand("LoadTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.Invoke()
                End If
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_handler.Detach()
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Overrides

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            If ((msg.Source = eCoreComponentType.TimeSeries) And
                (msg.Type = eMessageType.DataAddedOrRemoved Or msg.Type = eMessageType.DataModified)) Then
                ' Refresh content
                Me.m_handler.Refresh()
            End If
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace

