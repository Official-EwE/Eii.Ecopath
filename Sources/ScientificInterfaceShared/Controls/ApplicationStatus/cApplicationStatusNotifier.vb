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

        ''' <summary>The dispatcher that will receives and visualizes status feedback.</summary>
        Private m_core As cCore = Nothing

#Region " Singleton "

        ''' <summary>Singleton instance of the status notifier.</summary>
        Private Shared __inst__ As cApplicationStatusNotifier

        <Obsolete("Deprecated, but will be continued until SetStatusText is no longer used")> _
        Public Sub New(ByVal core As cCore)

            ' Singleton asserts
            Debug.Assert(__inst__ Is Nothing)
            Debug.Assert(core IsNot Nothing)

            ' Store dispatcher
            Me.m_core = core

            ' Store singleton instance
            __inst__ = Me

        End Sub

#End Region ' Singleton

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set status text feedback.
        ''' </summary>
        ''' <param name="strText">The text to set.</param>
        ''' <param name="tsUseWaitCursor">
        ''' <para>Flag stating whether to show a wait cursor.</para>
        ''' <para>Flag values are interpreted as follows:
        ''' <list>
        ''' <item><term>True</term><description>Shows a wait cursor, and increases the wait cursor count by one.</description></item>
        ''' <item><term>False</term><description>Decreases the wait cursor count by one, and once the wait cursor count reaches zero a default cursor is restored.</description></item>
        ''' <item><term>UseDefault</term><description>Use the current application cursor.</description></item>
        ''' </list>
        ''' </para>
        ''' </param>
        ''' <param name="sProgress">
        ''' <para>Progress of the process that is running.</para>
        ''' <para>Values are interpreted as follows:
        ''' <list>
        ''' <item><term>0</term><description>The status feedback will not provide progress information.</description></item>
        ''' <item><term>[0, 1]</term><description>If non-zero, status feedback will show progress information with the given value, where 1 equates to 100% progress.</description></item>
        ''' <item><term>-1</term><description>Shows continuous progress information that is not related to a given progress value.</description></item>
        ''' </list>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        <Obsolete("Deprecated, use StartProcess, UpdateProgress, EndProgress instead")> _
        Public Shared Sub SetStatusText(Optional ByVal strText As String = "", _
                                        Optional ByVal tsUseWaitCursor As Microsoft.VisualBasic.TriState = Microsoft.VisualBasic.TriState.UseDefault, _
                                        Optional ByVal sProgress As Single = 0.0)

            ' Sanity check
            If (__inst__ Is Nothing) Then Return

            Select Case tsUseWaitCursor
                Case TriState.True
                    StartProgress(__inst__.m_core, strText)
                Case TriState.False
                    EndProgress(__inst__.m_core)
                Case TriState.UseDefault
                    UpdateProgress(__inst__.m_core, strText, sProgress)
            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start progress status text feedback.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Sub StartProgress(ByVal core As cCore, ByVal strText As String, _
                                        Optional ByVal sProgress As Single = 0.0!)


            If (core Is Nothing) Then Return
            If (core.Messages Is Nothing) Then Return

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
