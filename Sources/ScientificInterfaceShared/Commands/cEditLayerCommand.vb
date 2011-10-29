#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'EditPedigree' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditLayerCommand
        Inherits cCommand

        Private m_layer As cLayer = Nothing
        Private m_edittype As eLayerEditTypes

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~EditLayer"

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cEditLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal layer As cLayer, ByVal edittype As eLayerEditTypes)
            Me.m_layer = layer
            Me.m_edittype = edittype
            MyBase.Invoke()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the layer that the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Layer() As cLayer
            Get
                Return Me.m_layer
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the reference depth layer that the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property EditType() As eLayerEditTypes
            Get
                Return Me.m_edittype
            End Get
        End Property

    End Class

End Namespace ' Commands
