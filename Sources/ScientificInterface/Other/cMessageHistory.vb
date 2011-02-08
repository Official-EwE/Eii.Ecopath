#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports System.Text

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' <para>Class that performs message functions:</para>
''' <list type="bullet">
''' <item><description>Keep a history of <see cref="cMessage">core messages</see>;</description></item>
''' <item><description>Invoke user prompts when the core requests <see cref="cFeedbackMessage">user feedback</see>;</description></item>
''' <item><description>Suppress user prompts.</description></item>
''' </list>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMessageHistory
    Implements IUIElement
    Implements IDisposable

#Region " Privates "

    ''' <summary>The connected UI context.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>Message suppressor.</summary>
    Private m_msh As New cMessageStateHandler()
    ''' <summary>Message history.</summary>
    Private m_lHistory As New List(Of cHistoryItem)
    ''' <summary>Core message handlers.</summary>
    Private m_dtMessageHanders As New Dictionary(Of eCoreComponentType, cMessageHandler)

#End Region ' Privates

#Region " Helper class "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A history item logged in the <see cref="cMessageHistory">message history</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cHistoryItem

#Region " Private vars "

        ''' <summary>Message text</summary>
        Private m_strText As String = ""
        ''' <summary>Message importance</summary>
        Private m_importance As eMessageImportance = eMessageImportance.Information
        ''' <summary>History item children.</summary>
        Private m_lItems As New List(Of cHistoryItem)
        ''' <summary>Abstract representation of the value that corresponded to a message.</summary>
        Private m_strValueID As String = ""
        ''' <summary>Core component where this message came from.</summary>
        ''' <remarks>This value can be deducted from the value ID, but that is too
        ''' cumbersome. Instead, core component is cached for easy access.</remarks>
        Private m_source As eCoreComponentType = eCoreComponentType.NotSet
        ''' <summary>Date and time message was generated.</summary>
        Private m_time As DateTime = Nothing

#End Region ' Private vars 

#Region " Construction "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Create a history item for a message.
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see>
        ''' to obtain abstract value representation from.</param>
        ''' <param name="msg"><see cref="cMessage">Message</see> to create
        ''' history item for.</param>
        ''' <remarks>
        ''' This will create sub-items for all information attached to the
        ''' <paramref name="msg">message</paramref>, such as 
        ''' <see cref="cVariableStatus">variable status information</see>.
        ''' </remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal pm As cPropertyManager, _
                       ByVal msg As cMessage)

            Me.New(msg.Message, msg.Importance)
            Me.m_source = msg.Source

            For Each vs As cVariableStatus In msg.Variables
                Me.m_lItems.Add(New cHistoryItem(pm, vs, msg.Importance, Me.m_source))
            Next

            ' Is Feedback message?
            If (TypeOf msg Is cFeedbackMessage) Then

                ' #Yes: include reply as a child item
                Dim fmsg As cFeedbackMessage = DirectCast(msg, cFeedbackMessage)
                Dim strReply As String = ""

                If Me.m_importance <> eMessageImportance.Critical And Me.m_importance <> eMessageImportance.Warning Then
                    Me.m_importance = eMessageImportance.Question
                End If

                Select Case fmsg.ReplyStyle

                    Case cFeedbackMessage.eReplyStyle.OK_CANCEL
                        Select Case fmsg.Reply
                            Case cFeedbackMessage.eReply.OK
                                strReply = My.Resources.GENERIC_REPLY_OK
                            Case cFeedbackMessage.eReply.CANCEL
                                strReply = My.Resources.GENERIC_REPLY_CANCEL
                        End Select

                    Case cFeedbackMessage.eReplyStyle.YES_NO, _
                         cFeedbackMessage.eReplyStyle.YES_NO_CANCEL

                        Select Case fmsg.Reply
                            Case cFeedbackMessage.eReply.YES
                                strReply = My.Resources.GENERIC_REPLY_YES
                            Case cFeedbackMessage.eReply.NO
                                strReply = My.Resources.GENERIC_REPLY_NO
                            Case cFeedbackMessage.eReply.CANCEL
                                strReply = My.Resources.GENERIC_REPLY_CANCEL

                        End Select

                End Select

                If (Not String.IsNullOrEmpty(strReply)) Then
                    ' Add reply node
                    Me.m_lItems.Add(New cHistoryItem(strReply, eMessageImportance.Information))
                End If
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal constructor, generate a history item for a
        ''' <see cref="cVariableStatus">variable status</see>.
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see>
        ''' to obtain abstract value representation from.</param>
        ''' <param name="vs"><see cref="cVariableStatus">variable status</see>
        ''' to create item for.</param>
        ''' <param name="imp"><see cref="eMessageImportance">message importance</see>,
        ''' inherited from the parent message.</param>
        ''' -------------------------------------------------------------------
        Private Sub New(ByVal pm As cPropertyManager, _
                        ByVal vs As cVariableStatus, _
                        ByVal imp As eMessageImportance, _
                        ByVal source As eCoreComponentType)

            Me.New(vs.Message, imp)
            Me.m_strValueID = pm.ExtractPropertyID(vs)
            Me.m_source = source

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal constructor, generate a history item.
        ''' </summary>
        ''' <param name="strMessage">Text to generate history item for.</param>
        ''' <param name="imp"><see cref="eMessageImportance">message importance</see>,
        ''' inherited from the parent message.</param>
        ''' -------------------------------------------------------------------
        Private Sub New(ByVal strMessage As String, _
                        ByVal imp As eMessageImportance)

            Me.m_strText = strMessage
            Me.m_importance = imp
            Me.m_time = DateTime.Now

        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the text a message was logged with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Text() As String
            Get
                Return Me.m_strText
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eMessageImportance">message importance</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Importance() As eMessageImportance
            Get
                Return Me.m_importance
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get all child history items for this item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Children() As cHistoryItem()
            Get
                Return Me.m_lItems.ToArray
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eCoreComponentType">source</see> that this message
        ''' originated from.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Source() As eCoreComponentType
            Get
                Return Me.m_source
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the time that this message was created.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Time() As DateTime
            Get
                Return Me.m_time
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract properties for logged history items.
        ''' </summary>
        ''' <param name="pm"><see cref="cPropertyManager">Property manager</see>
        ''' to extract <see cref="cProperty">properties</see> from.</param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Properties(ByVal pm As cPropertyManager) As cProperty()
            Get
                Dim lProps As New List(Of cProperty)
                If Me.IsValid Then Me.GetProperties(pm, lProps)
                Return lProps.ToArray
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether this history item is still linked to its core data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return (Me.m_source <> eCoreComponentType.NotSet)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Invalidate the core data link of this history item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Invalidate()
            If Not Me.IsValid Then Return
            Me.m_source = eCoreComponentType.NotSet
            For Each item As cHistoryItem In Me.m_lItems
                item.Invalidate()
            Next
        End Sub

#End Region ' Public interfaces 

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recursively extract all properties from this item and onward.
        ''' </summary>
        ''' <param name="pm"></param>
        ''' <param name="lProps"></param>
        ''' -------------------------------------------------------------------
        Private Sub GetProperties(ByVal pm As cPropertyManager, ByVal lProps As List(Of cProperty))
            Dim prop As cProperty = pm.GetProperty(Me.m_strValueID)
            If (prop IsNot Nothing) Then lProps.Add(prop)
            For Each item As cHistoryItem In Me.m_lItems
                item.GetProperties(pm, lProps)
            Next
        End Sub

#End Region ' Internals

    End Class

#End Region ' Helper class

#Region " Construction / destruction "

    Public Sub New()
    End Sub

    Public Property UIContext() As ScientificInterfaceShared.Controls.cUIContext _
        Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            If (Object.Equals(Me.m_uic, value)) Then Return
            If (Me.m_uic IsNot Nothing) Then Me.ConfigMessageHandlers(False)
            Me.m_uic = value
            If (Me.m_uic IsNot Nothing) Then Me.ConfigMessageHandlers(True)
        End Set
    End Property

    Private m_bDisposed As Boolean = False        ' To detect redundant calls

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not Me.m_bDisposed Then
            Me.UIContext = Nothing
        End If
        Me.m_bDisposed = True
        GC.SuppressFinalize(Me)
    End Sub

#End Region ' Construction / destruction

#Region " Public interfaces "

    ''' <summary>Event to signify that an item was added to the history.</summary>
    ''' <param name="sender">The history instance the item was added to.</param>
    ''' <param name="item">The added <see cref="cHistoryItem">item</see>.</param>
    Public Event OnHistoryItemAdded(ByVal sender As cMessageHistory, ByVal item As cHistoryItem)

    ''' <summary>Event to signify that something big changed about the history log.</summary>
    ''' <param name="sender">The history instance that was refreshed.</param>
    Public Event OnHistoryRefreshed(ByVal sender As cMessageHistory)

    ''' <summary>
    ''' Clear the message suppress cache.
    ''' </summary>
    Public Sub Refresh()
        Try
            RaiseEvent OnHistoryRefreshed(Me)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Get all history items.
    ''' </summary>
    Public ReadOnly Property Items() As cHistoryItem()
        Get
            Return Me.m_lHistory.ToArray
        End Get
    End Property


#End Region ' Public interfaces

#Region " Internals "

    Private Sub ConfigMessageHandler(ByVal src As eCoreComponentType, ByVal bSet As Boolean)

        Dim mh As cMessageHandler = Nothing

        If (src = eCoreComponentType.NotSet) Then Return

        If bSet Then
            mh = New cMessageHandler(AddressOf AllMessagesHandler, src, eMessageType.Any, Me.UIContext.SyncObject)
#If DEBUG Then
            mh.Name = "owned by cMessageHistory"
#End If
            Me.m_dtMessageHanders(src) = mh
            Me.UIContext.Core.Messages.AddMessageHandler(mh)
        Else
            mh = Me.m_dtMessageHanders(src)
            Me.m_dtMessageHanders.Remove(src)
            Me.UIContext.Core.Messages.RemoveMessageHandler(mh)
            mh = Nothing
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Hook up to, or connect from, core messages.
    ''' </summary>
    ''' <param name="bSet">True to set, False to clear.</param>
    ''' -------------------------------------------------------------------
    Private Sub ConfigMessageHandlers(ByVal bSet As Boolean)

        ' Set up message handlers
        For Each src As eCoreComponentType In [Enum].GetValues(GetType(eCoreComponentType))
            Me.ConfigMessageHandler(src, bSet)
        Next

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Universal messages listener
    ''' </summary>
    ''' <param name="msg">The message to listen to</param>
    ''' -------------------------------------------------------------------
    Private Sub AllMessagesHandler(ByRef msg As cMessage)

        Dim strMessage As String = msg.Message
        Dim bSuppressVarMessage As Boolean = False
        Dim iMaxMessages As Integer = Math.Max(10, Math.Min(200, My.Settings.StatusMaxMessages))

        If String.IsNullOrEmpty(msg.Message) Then Return

        ' Requires feedback (overrules popup settings)
        If (TypeOf msg Is cFeedbackMessage) Then
            ' #Yes: handle it
            Try
                Me.HandleFeedbackMessage(DirectCast(msg, cFeedbackMessage))
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        Else
            Select Case msg.Importance
                Case eMessageImportance.Critical, eMessageImportance.Warning
                    Try
                        Me.ShowMessageBox(msg)
                    Catch ex As Exception
                        Debug.Assert(False, ex.Message)
                    End Try
                Case eMessageImportance.Information
                    ' NOP
                Case eMessageImportance.Maintenance, _
                     eMessageImportance.Progress
                    Return
                Case Else
                    Return
            End Select
        End If

        Try
            Dim item As New cHistoryItem(Me.m_uic.PropertyManager, msg)
            Me.m_lHistory.Add(item)
            RaiseEvent OnHistoryItemAdded(Me, item)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        ' Has a major change occurred?
        If (msg.Type = eMessageType.DataAddedOrRemoved) Then

            Dim acomps() As eCoreComponentType = Me.GetChildComponents(msg.Source)

            ' Invalidate messages linking to 'old' data
            For Each item As cHistoryItem In Me.m_lHistory
                If Array.IndexOf(acomps, item.Source) > -1 Then item.Invalidate()
            Next

            ' Clear all suppressed message flags
            For Each comp As eCoreComponentType In acomps
                Me.m_msh.Clear(comp)
            Next

        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; handles a feedback message by presenting the user with
    ''' a message box.
    ''' </summary>
    ''' <param name="msg">The <see cref="cFeedbackMessage">feedback message</see>
    ''' to handle.</param>
    ''' -------------------------------------------------------------------
    Private Sub HandleFeedbackMessage(ByVal msg As cFeedbackMessage)

        Dim mbb As MessageBoxButtons = MessageBoxButtons.YesNo
        Dim mbi As MessageBoxIcon = MessageBoxIcon.Question
        Dim dlr As DialogResult = Windows.Forms.DialogResult.No
        Dim strMessage As String = ""

        If (msg Is Nothing) Then Return

        ' Translate feedback style into .NET MessageBox style
        Select Case msg.ReplyStyle
            Case cFeedbackMessage.eReplyStyle.OK_CANCEL
                mbb = MessageBoxButtons.OKCancel
            Case cFeedbackMessage.eReplyStyle.YES_NO
                mbb = MessageBoxButtons.YesNo
            Case cFeedbackMessage.eReplyStyle.YES_NO_CANCEL
                mbb = MessageBoxButtons.YesNoCancel
        End Select

        Select Case msg.Importance
            Case eMessageImportance.Progress, eMessageImportance.Maintenance, eMessageImportance.Information
                mbi = MessageBoxIcon.Question
            Case eMessageImportance.Warning
                mbi = MessageBoxIcon.Warning
            Case eMessageImportance.Critical
                mbi = MessageBoxIcon.Error
        End Select

        ' Pop the question
        Me.ToMessageBoxText(msg, strMessage)

        ' Is message suppressable?
        If msg.Suppressable Then

            ' #Yes: handle autoreply

            ' Sanity check
            Debug.Assert(msg.Type <> eMessageType.NotSet, "Feedback message not propery configured for auto-reply: messagetype not set")

            ' Get reply, if any
            dlr = Me.m_msh.AutoReply(msg.Source, msg.Type)
            ' Is 'none'?
            If (dlr = Windows.Forms.DialogResult.None) Then
                ' #Yes: prompt needed

                ' Assume to repeat the question
                Dim bChecked As Boolean = False
                ' Show dialog
                dlr = cCustomMessageBox.Show(strMessage, AppLauncher.GetInstance().Text, _
                                             mbb, mbi, _
                                             bChecked, My.Resources.PROMPT_MESSAGE_HIDE)
                ' Auto-reply requested?
                If bChecked Then
                    ' #Yes: store auto-reply
                    Me.m_msh.AutoReply(msg.Source, msg.Type) = dlr
                End If
            End If
        Else
            dlr = MessageBox.Show(strMessage, AppLauncher.GetInstance().Text, mbb, mbi)
        End If

        ' Translate .NET MessageBox result into reply
        Select Case dlr
            Case DialogResult.Cancel
                msg.Reply = cFeedbackMessage.eReply.CANCEL
            Case DialogResult.OK
                msg.Reply = cFeedbackMessage.eReply.OK
            Case DialogResult.Yes
                msg.Reply = cFeedbackMessage.eReply.YES
            Case DialogResult.No
                msg.Reply = cFeedbackMessage.eReply.NO
            Case Else
                Debug.Assert(False, String.Format("Message box result {0} not supported", dlr))
        End Select

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; invokes a Windows Message Box for a EwE Core
    ''' <see cref="cMessage">Message</see>.
    ''' </summary>
    ''' <param name="msg">The <see cref="cMessage">Message</see> to show a
    ''' Message Box for.</param>
    ''' <returns>
    ''' True if a problem occurred displaying the message
    ''' </returns>
    ''' -------------------------------------------------------------------
    Private Function ShowMessageBox(ByVal msg As cMessage) As Boolean

        Dim strMessage As String = ""
        Dim mbb As MessageBoxButtons = MessageBoxButtons.OK
        Dim mbi As MessageBoxIcon = MessageBoxIcon.Information
        Dim bError As Boolean = False

        ' Sanity check
        If msg IsNot Nothing Then

            bError = ToMessageBoxText(msg, strMessage)

            ' Resolve what icon to show
            Select Case msg.Importance
                Case eMessageImportance.Critical
                    mbi = MessageBoxIcon.Error
                Case eMessageImportance.Warning
                    mbi = MessageBoxIcon.Warning
                Case eMessageImportance.Information
                    mbi = MessageBoxIcon.Information
            End Select

            ' == Show the message ==

            ' Can the message be suppressed?
            If msg.Suppressable Then

                ' #Yes: check suppressed state

                ' Sanity check
                Debug.Assert(msg.Type <> eMessageType.NotSet, "Message not propery configured for suppression: messagetype not set")

                If (Not Me.m_msh.Suppress(msg.Source, msg.Type)) Then
                    ' #No: Good, prepare to show message
                    ' Assume message will not be suppressed
                    Dim bSuppress As Boolean = False
                    ' Invoke the special message box
                    cCustomMessageBox.Show(strMessage, AppLauncher.GetInstance().Text, _
                                           mbb, mbi, _
                                           bSuppress, My.Resources.PROMPT_MESSAGE_HIDE)
                    If bSuppress Then
                        '#Yes: suppress it during the rest of this session
                        Me.m_msh.Suppress(msg.Source, msg.Type) = True
                    End If
                End If
            Else
                ' #No: show the message
                MessageBox.Show(strMessage, AppLauncher.GetInstance().Text, mbb, mbi, MessageBoxDefaultButton.Button1)
            End If
        End If

        Return bError
    End Function

    Private Function ToMessageBoxText(ByVal msg As cMessage, ByRef strMessage As String) As Boolean

        Dim sb As New StringBuilder(msg.Message)
        Dim iNumSubLines As Integer = 0
        Dim strTmp As String = ""
        Dim bError As Boolean = False

        ' Sanity check
        If msg IsNot Nothing Then

            ' Concatenate all child messages
            For Each vs As cVariableStatus In msg.Variables
                Select Case iNumSubLines

                    Case 0 To 9
                        strTmp = vs.Message
                        If Not String.IsNullOrEmpty(strTmp) Then
                            sb.AppendLine()
                            sb.Append(strTmp)
                            iNumSubLines += 1
                        End If

                    Case 10
                        sb.AppendLine()
                        sb.AppendLine("...")
                        sb.AppendLine(My.Resources.PROMPT_STATUS_FURTHERDETAILS)
                        bError = True
                        Exit For

                End Select
            Next
        End If

        strMessage = sb.ToString().Replace("\n", vbNewLine)
        Return bError

    End Function

    Private Function GetChildComponents(ByVal source As eCoreComponentType) As eCoreComponentType()

        Select Case source

            Case eCoreComponentType.Ecotracer
                ' No children
                Return New eCoreComponentType() {eCoreComponentType.Ecotracer}

            Case eCoreComponentType.EcoSpace
                Return New eCoreComponentType() {eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer}

            Case eCoreComponentType.EcoSim
                Return New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer}

            Case eCoreComponentType.EcoPath
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer}

            Case eCoreComponentType.Core
                Return New eCoreComponentType() {eCoreComponentType.Core, eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer}

        End Select
        Return New eCoreComponentType() {source}

    End Function

#End Region ' Internals 

End Class
