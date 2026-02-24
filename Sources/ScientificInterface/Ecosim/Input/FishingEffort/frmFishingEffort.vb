' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form providing the interface to sketch fishing effort.
    ''' </summary>
    ''' =======================================================================
    Public Class frmFishingEffort

#Region " Private variables "

        Private m_handler As cFishingEffortShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Form overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Me.m_handler = New cFishingEffortShapeGUIHandler(Me.UIContext)
            Me.m_handler.Attach(Me.m_shapeToolBox, Me.m_shapeToolboxToolbar, Me.m_sketchPad, Me.m_sketchPadToolbar)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_handler.Detach()
            Me.m_handler = Nothing
            MyBase.OnFormClosed(e)
        End Sub

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            Select Case msg.Source
                Case eCoreComponentType.ShapesManager
                    If (msg.DataType = eDataTypes.FishingEffort) Then
                        Me.m_handler.Refresh()
                    End If
            End Select
        End Sub

#End Region ' Form overrides 

    End Class

End Namespace

