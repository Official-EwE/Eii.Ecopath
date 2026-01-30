' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On




Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing all functionality to add, remove and edit
    ''' <see cref="cMediationFunction">mediation shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmPriceElasticity

#Region " Private variables "

        ''' <summary>Controller for shape-related GUI components in this form.</summary>
        Private m_shapeguihandler As cLandingsShapeGUIHandler = Nothing

#End Region '  Private variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_assignments.UIContext = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; implemented to make sure that this form receives 
        ''' <see cref="cMessage">messages</see> from specific 
        ''' <see cref="eCoreComponentType">message sources</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.UIContext Is Nothing) Then Return
            Me.m_shapeguihandler = New cLandingsShapeGUIHandler(Me.UIContext)
            Me.m_shapeguihandler.Attach(Me.m_shapeToolBox, Me.m_shapeToolboxToolbar,
                                        Me.m_sketchPad, Me.m_sketchPadToolbar,
                                        Me.m_assignments, Me.m_assignmentsToolbar)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_shapeguihandler.Detach()
            MyBase.OnFormClosed(e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic EwEForm message handler; implemented to respond to Forcing 
        ''' Function shape changes.
        ''' </summary>
        ''' <param name="msg">Incoming core <see cref="cMessage">messages</see>.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)

            If msg.Source = eCoreComponentType.ShapesManager Then
                If (((msg.Type = eMessageType.DataAddedOrRemoved) Or (msg.Type = eMessageType.DataModified)) And
                     (msg.DataType = eDataTypes.Mediation)) Then
                    Me.m_shapeguihandler.Refresh()
                End If
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace


