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
        Private m_dispatcher As IApplicationStatusDispatcher = Nothing

#Region " Singleton "

        ''' <summary>Singleton instance of the status notifier.</summary>
        Private Shared __inst__ As cApplicationStatusNotifier

        Public Sub New(ByVal dispatcher As IApplicationStatusDispatcher)

            ' Singleton asserts
            Debug.Assert(__inst__ Is Nothing)
            Debug.Assert(dispatcher IsNot Nothing)

            ' Store dispatcher
            Me.m_dispatcher = dispatcher

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
        Public Shared Sub SetStatusText(Optional ByVal strText As String = "", _
                                        Optional ByVal tsUseWaitCursor As Microsoft.VisualBasic.TriState = Microsoft.VisualBasic.TriState.UseDefault, _
                                        Optional ByVal sProgress As Single = 0.0)

            ' Sanity check
            If (__inst__ Is Nothing) Then Return

            ' Pass the word
            __inst__.m_dispatcher.SetStatusText(strText, tsUseWaitCursor, sProgress)
        End Sub

    End Class

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface to implement status feedback.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface IApplicationStatusDispatcher

        Sub SetStatusText(Optional ByVal strText As String = "", _
            Optional ByVal tsUseWaitCursor As TriState = TriState.UseDefault, _
            Optional ByVal sProgress As Single = 0.0)

    End Interface

End Namespace ' Controls
