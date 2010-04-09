#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared

#End Region ' Imports

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing all functionality to add, remove and edit
    ''' Egg Production <see cref="cForcingFunction">forcing shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmEggProduction

#Region " Private variables "

        ''' <summary>Controller for shape-related GUI components in this form.</summary>
        Private m_shapeguihandler As cShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initialzes a new instance of this form.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            Me.InitializeComponent()
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
        Protected Overrides Sub onLoad(ByVal e As System.EventArgs)
            Me.m_shapeguihandler = New cEggProductionShapeGUIHandler(Me.UIContext, _
                Me.m_shapeToolBox, Me.m_shapeToolboxToolbar, _
                Me.m_sketchPad, Me.m_sketchPadToolbar)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

#End Region ' Events 

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Generic EwEForm message handler; implemented to respond to Egg
        ''' Production shape changes.
        ''' </summary>
        ''' <param name="msg">Incoming core <see cref="cMessage">messages</see>.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            If msg.Source = eCoreComponentType.ShapesManager Then
                If (((msg.Type = eMessageType.DataAddedOrRemoved) Or (msg.Type = eMessageType.DataModified)) And _
                     (msg.DataType = eDataTypes.EggProd)) Then
                    Me.m_shapeguihandler.Refresh()
                End If
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace


