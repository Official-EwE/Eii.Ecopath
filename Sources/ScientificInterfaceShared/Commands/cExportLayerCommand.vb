' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Export Ecospace Layer Data' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cExportLayerCommand
        Inherits cCommand

        Private m_alayers() As cEcospaceLayer = Nothing
        Private m_format As eNativeLayerFileFormatTypes = eNativeLayerFileFormatTypes.Default

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~exportLayer"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cExportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.Invoke(New cEcospaceLayer() {})
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="alayers">The layers to export data from.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(alayers() As cEcospaceLayer,
                                    Optional format As eNativeLayerFileFormatTypes = eNativeLayerFileFormatTypes.Default)
            Me.m_alayers = alayers
            Me.m_format = format
            MyBase.Invoke()
            Me.m_alayers = Nothing
            Me.m_format = eNativeLayerFileFormatTypes.Default
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the raster layers the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Layers() As cEcospaceLayer()
            Get
                Return Me.m_alayers
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eNativeLayerFileFormatTypes">format types</see> the command was 
        ''' invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Format As eNativeLayerFileFormatTypes
            Get
                Return Me.m_format
            End Get
        End Property

    End Class

End Namespace ' Commands
