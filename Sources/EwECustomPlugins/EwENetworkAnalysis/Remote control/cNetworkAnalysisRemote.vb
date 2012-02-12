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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Remote controller for Network Analysis plug-in
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cNetworkAnalysisRemote

    Private m_uic As cUIContext = Nothing
    Private m_manager As cNetworkManager = Nothing

    Public Sub Attach(ByVal uic As cUIContext, _
                      ByVal manager As cNetworkManager)

        Me.m_uic = uic

        Try
            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand(cExecuteCommand.COMMAND_NAME)
            If (cmd IsNot Nothing) Then
                AddHandler cmd.OnInvoke, AddressOf OnExecuteCommand
            End If
        Catch ex As Exception

        End Try

        Me.m_manager = manager

    End Sub

    Public Sub Detach()

        Try
            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand(cExecuteCommand.COMMAND_NAME)

            If (cmd IsNot Nothing) Then
                RemoveHandler cmd.OnInvoke, AddressOf OnExecuteCommand
            End If
        Catch ex As Exception

        End Try

        Me.m_manager = Nothing
    End Sub

    Private Sub OnExecuteCommand(ByVal cmd As cCommand)

        If Not (TypeOf (cmd) Is cExecuteCommand) Then Return

        Dim cmdX As cExecuteCommand = DirectCast(cmd, cExecuteCommand)

        Select Case cmdX.Command.ToLower
            Case "na_save_indices"
                Try
                    If Not Me.SaveIndices(cmdX.Parameter("path"), Convert.ToBoolean(cmdX.Parameter("ppr"))) Then
                        'cmd.Status = "Failed"
                    End If
                Catch ex As Exception

                End Try
        End Select

    End Sub

    Private Function SaveIndices(ByVal strPath As String, ByVal bWithPPR As Boolean) As Boolean

        Dim writer As New cResultWriter(Me.m_manager)
        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_uic.Core.OutputPath

        If bWithPPR Then
            Return writer.WriteIndicesWithPPR(strPath)
        Else
            Return writer.WriteIndicesWithoutPPR(strPath)
        End If
    End Function

End Class
