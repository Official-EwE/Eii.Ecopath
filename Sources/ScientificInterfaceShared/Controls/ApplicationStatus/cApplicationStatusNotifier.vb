Imports EwECore
Imports EwEUtils.Core

#Region " Imports "

#End Region ' Imports

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
        Public Shared Sub StartProgress(ByVal core As cCore, _
                                        Optional ByVal strText As String = "", _
                                        Optional ByVal sProgress As Single = 0.0!)

            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            ' Provide default
            If (String.IsNullOrWhiteSpace(strText)) Then strText = My.Resources.GENERIC_STATUS_BUSY

            Dim pmsg As New cProgressMessage(sProgress, strText, eMessageType.Progress)
            pmsg.ProgressState = eProgressState.Start
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
        Public Shared Sub UpdateProgress(ByVal core As cCore, ByVal strText As String, ByVal sProgress As Single)


            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            Dim pmsg As New cProgressMessage(sProgress, strText, eMessageType.Progress)
            pmsg.ProgressState = eProgressState.Running
            core.Messages.SendMessage(pmsg, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' End running progress feedback
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Sub EndProgress(ByVal core As cCore)


            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

            Dim pmsg As New cProgressMessage(0, "", eMessageType.Progress)
            pmsg.ProgressState = eProgressState.Finished
            core.Messages.SendMessage(pmsg, True)

        End Sub
    End Class

End Namespace ' Controls
