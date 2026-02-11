' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write network analysis results to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cNetworkAnalysisResultWriter

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Shazaam
    ''' </summary>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(manager As cNetworkManager)
        Me.Manager = manager
    End Sub

    Protected ReadOnly Property Manager As cNetworkManager

    Public MustOverride Function WriteResults(strPath As String) As Boolean

#Region " Internals "

    Protected Function WriteFile(strFileName As String, strData As String) As Boolean

        Dim strPath As String = Path.GetDirectoryName(strFileName)
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVE_NOACCESS, strPath), True, strPath)
            Return False
        End If

        Dim sw As New StreamWriter(strFileName)
        If (sw IsNot Nothing) Then
            sw.Write(strData)
            sw.Close()
            Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVE_SUCCESS, strFileName), False, strPath)
            Return True
        End If

        Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVE_FAILED, strFileName), True, strPath)
        Return False

    End Function

    Protected Sub SendMessage(strMessage As String, Optional bError As Boolean = False, Optional strURL As String = "")
        Dim msg As New cMessage(strMessage, eMessageType.DataExport, eCoreComponentType.External,
                                If(bError, eMessageImportance.Warning, eMessageImportance.Information))
        msg.Hyperlink = strURL
        Me.Manager.Core.Messages.SendMessage(msg)
    End Sub

#End Region ' Internals

End Class
