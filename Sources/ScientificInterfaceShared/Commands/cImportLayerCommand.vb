' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports System.IO

#End Region ' Imports

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

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cImportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal alayers() As cEcospaceLayer, strFile As String)

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
        Public Overloads Sub Invoke(ByVal alayers() As cEcospaceLayer, format As eNativeLayerFileFormatTypes)
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
