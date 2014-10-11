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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cResilienceWriter

    ''' <summary>EwE core to use</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Resilience data to use</summary>
    Private m_data As cResilienceData = Nothing

    Public Sub New(core As cCore, data As cResilienceData)
        Me.m_core = core
        Me.m_data = data
    End Sub

    Public Function SaveDataToFile() As Boolean

        Dim msg As cMessage = Nothing
        Dim bSuccess As Boolean = Me.SaveDataToFile(True) And Me.SaveDataToFile(False)

        If (bSuccess) Then
            msg = New cMessage(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputPath
        Else
            msg = New cMessage(String.Format(My.Resources.STATUS_SAVE_FAILED, Me.OutputPath), _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
        End If
        Me.m_core.Messages.SendMessage(msg)

        Return True

    End Function

#Region " Internals "

    Private Function SaveDataToFile(bAnnual As Boolean) As Boolean
        Return True
    End Function

    Private ReadOnly Property OutputPath As String
        Get
            Return Me.m_core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
        End Get
    End Property

    Private Function GetOutputFileName(ByVal strPath As String, _
                                       ByVal bSaveAnnual As Boolean) As String

        Dim strFileName As String = ""
        Dim strExt As String = ".csv"

        If bSaveAnnual Then
            strFileName = "Resilience_annual"
        Else
            strFileName = "Resilience"
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False) & strExt)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get default model details to report in output file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetModelDetails() As String
        Return Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim)
    End Function

#End Region ' Internals

End Class
