' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Central class for providing appliation status feedback. This class
    ''' enables code outside the Scientific Interface application framework
    ''' such as plug-ins to provide status bar feedack on running operations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cApplicationStatusNotifier

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start progress status text feedback.
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="strText">Progress message to show, if any. If left emtpy a generic busy message will be used.</param>
        ''' <param name="sProgress">Optional progress indicator [0, 1] to use.</param>
        ''' -------------------------------------------------------------------
        Public Shared Sub StartProgress(core As cCore,
                                        Optional strText As String = "",
                                        Optional sProgress As Single = 0.0!)

            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            ' Provide default
            If (String.IsNullOrWhiteSpace(strText)) Then strText = My.Resources.GENERIC_STATUS_BUSY

            Dim pmsg As New cProgressMessage(eProgressState.Start, 1.0!, sProgress, strText, eMessageType.Progress)
            core.Messages.SendMessage(pmsg, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update progress.
        ''' </summary>
        ''' <param name="strText">The text to set.</param>
        ''' <param name="sProgress">A value between 0 and 1 to control a progress
        ''' bar, or -1 to display a continuous progress bar.</param>
        ''' -------------------------------------------------------------------
        Public Shared Sub UpdateProgress(core As cCore, strText As String, sProgress As Single)

            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            Dim pmsg As New cProgressMessage(eProgressState.Running, 1.0!, sProgress, strText, eMessageType.Progress)
            core.Messages.SendMessage(pmsg, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' End running progress feedback
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Sub EndProgress(core As cCore)

            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            Dim pmsg As New cProgressMessage(eProgressState.Finished, 1, 1, "", eMessageType.Progress)
            core.Messages.SendMessage(pmsg, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Send a status message to the UI. This only passes through if the core
        ''' is not <see cref="cCoreStateMonitor.IsBusy">busy</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Sub UpdateStatus(core As cCore, strText As String)

            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return
            If (core.StateMonitor.IsBusy) Then Return

            Dim pmsg As New cProgressMessage(eProgressState.Running, 0, 0, strText, eMessageType.Progress)
            core.Messages.SendMessage(pmsg, True)

        End Sub
    End Class

End Namespace ' Controls
