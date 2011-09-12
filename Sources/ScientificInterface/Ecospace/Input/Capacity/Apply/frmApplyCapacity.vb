#Region " Imports "

Option Strict On
Option Explicit On


Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Forms

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecospace 'Apply capacity map' interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmApplyCapacity
        Inherits Ecosim.frmApplyShapeBase

#Region " Constructor "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Hook up to core messages
            ' For this form only
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MapResponseInteractionManager}

        End Sub

        Protected Overrides ReadOnly Property Grid() As Ecosim.ApplyShapeGrid
            Get
                Return Me.m_grid
            End Get
        End Property

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            If (msg.Source = eCoreComponentType.MapResponseInteractionManager) Then
                Me.Grid.UpdateContent()
            End If


        End Sub

#End Region

    End Class

End Namespace
