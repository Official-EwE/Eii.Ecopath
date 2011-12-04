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
    ''' Command to invoke the 'Export Ecospace Layer Data' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cExportLayerCommand
        Inherits cCommand

        Private m_alayers() As cLayer = Nothing

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~exportLayer"

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cExportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.Invoke(Nothing)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="alayers">The layers to export data from.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal alayers() As cLayer)
            Me.m_alayers = alayers
            MyBase.Invoke()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the layers the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Layers() As cLayer()
            Get
                Return Me.m_alayers
            End Get
        End Property

    End Class

End Namespace ' Commands
