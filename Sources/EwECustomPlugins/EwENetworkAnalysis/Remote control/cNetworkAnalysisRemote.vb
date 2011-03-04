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
                    Me.SaveIndices(cmdX.Parameter("path"), _
                                   Convert.ToBoolean(cmdX.Parameter("annual")), _
                                   Convert.ToBoolean(cmdX.Parameter("ppr")))
                Catch ex As Exception

                End Try
        End Select

    End Sub

    Private Sub SaveIndices(ByVal strPath As String, _
                            ByVal bAnnualAverages As Boolean, _
                            ByVal bWithPPR As Boolean)

        Dim writer As New cResultWriter(Me.m_manager)

        ' ToDo: provide succes info to command

        ' Fix path
        If String.IsNullOrEmpty(strPath) Then strPath = ".\"

        If bWithPPR Then
            writer.WriteIndicesWithPPR(strPath, bAnnualAverages)
        Else
            writer.WriteIndicesWithoutPPR(strPath, bAnnualAverages)
        End If

    End Sub
End Class
