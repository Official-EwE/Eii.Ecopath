' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO

Namespace Commands

    Public Enum eNativeLayerFileFormatTypes As Byte
        [Default] = 0
        CSV = [Default]
        XYZ
        ASCII
        TXT = ASCII
    End Enum

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Import Ecospace Layer Data' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cImportLayerCommand
        Inherits cCommand

        Private m_alayers() As cEcospaceLayer = Nothing
        Private m_format As eNativeLayerFileFormatTypes = eNativeLayerFileFormatTypes.Default
        Private m_file As String = ""

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~importLayer"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cImportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(alayers() As cEcospaceLayer, strFile As String)

            Dim fmt As eNativeLayerFileFormatTypes = eNativeLayerFileFormatTypes.Default
            Select Case Path.GetExtension(strFile).ToLower
                Case ".asc" : fmt = eNativeLayerFileFormatTypes.ASCII
                Case ".csv" : fmt = eNativeLayerFileFormatTypes.CSV
                Case ".txt" : fmt = eNativeLayerFileFormatTypes.XYZ
                Case Else : fmt = eNativeLayerFileFormatTypes.Default
            End Select
            Me.m_alayers = alayers
            Me.m_format = fmt
            Me.m_file = strFile
            MyBase.Invoke()
            Me.m_alayers = Nothing
            Me.m_format = eNativeLayerFileFormatTypes.Default
            Me.m_file = ""
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(alayers() As cEcospaceLayer, format As eNativeLayerFileFormatTypes)
            Me.m_alayers = alayers
            Me.m_format = format
            Me.m_file = ""
            MyBase.Invoke()
            Me.m_alayers = Nothing
            Me.m_format = eNativeLayerFileFormatTypes.Default
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the layers the command was invoked for.
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

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the file name that the command was launched for.
        ''' THe user will be prompted to select a file if no file was specified.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property File As String
            Get
                Return Me.m_file
            End Get
        End Property

    End Class

End Namespace ' Commands
