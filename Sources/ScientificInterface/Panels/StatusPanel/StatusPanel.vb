#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' The status panel tracks core messages. Relevant messages are logged in
''' the GUI. Feedback messages are handled by this class.
''' </summary>
''' -----------------------------------------------------------------------
Public Class StatusPanel

    Private m_msh As New cMessageStateHandler()
    Private m_il As New ImageList

    Public Sub New()

        Me.InitializeComponent()
        ' Set tab label
        Me.TabText = My.Resources.HEADER_STATUS

        ' Prepare image list
        m_il.Images.Add(New Icon(SystemIcons.Information, 40, 40))
        m_il.Images.Add(New Icon(SystemIcons.Warning, 40, 40))
        m_il.Images.Add(New Icon(SystemIcons.Error, 40, 40))

        ' Set image list
        Me.tvStatus.ImageList = Me.m_il
        Me.tvStatus.ImageIndex = -1
        Me.tvStatus.SelectedImageIndex = -1
        Me.tvStatus.SelectedImageKey = ""

        ' Start listening to core messages
        Me.ConfigMessageHandlers(True)
    End Sub

    Private Sub OnDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        ' Stop listening to core messages
        Me.ConfigMessageHandlers(False)
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clear the list of messages, the list suppressed messages and the
    ''' list of auto-replies.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub Reset()
        Me.SetHighlights(Nothing)
        Me.tvStatus.Nodes.Clear()
        Me.m_msh.Clear(eCoreComponentType.Core)
    End Sub

#Region " Core message handling "

    Dim m_dtMessageHanders As New Dictionary(Of eCoreComponentType, cMessageHandler)

    Private Sub ConfigMessageHandler(ByVal src As eCoreComponentType, ByVal bSet As Boolean)

        Dim mh As cMessageHandler = Nothing
        Dim core As cCore = cCore.GetInstance()

        If (src = eCoreComponentType.NotSet) Then Return

        If bSet Then
            mh = New cMessageHandler(AddressOf AllMessagesHandler, src, eMessageType.Any, AppLauncher.GetInstance().SyncObject)
            Me.m_dtMessageHanders(src) = mh
            core.Messages.AddMessageHandler(mh)
        Else
            mh = Me.m_dtMessageHanders(src)
            Me.m_dtMessageHanders.Remove(src)
            core.Messages.RemoveMessageHandler(mh)
            mh = Nothing
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Hook up to core messages
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
    Public Sub AllMessagesHandler(ByRef msg As cMessage)

        Dim iMaxMessages As Integer = Math.Max(10, Math.Min(200, My.Settings.FeedbackMessageLogSize))
        Dim bPopup As Boolean = False
        Dim strMessage As String = msg.Message
        Dim bSuppressVarMessage As Boolean = False

        If String.IsNullOrEmpty(msg.Message) Then Return


        ' Requires feedback (overrules popup settings)
        If (TypeOf msg Is cFeedbackMessage) Then
            ' #Yes: handle it
            Me.HandleFeedbackMessage(DirectCast(msg, cFeedbackMessage))
            ' Disable popup
            bPopup = False
        Else
            ' Check settings
            Select Case msg.Importance
                Case eMessageImportance.Critical
                    bPopup = True
                Case eMessageImportance.Warning
                    bPopup = True
                Case eMessageImportance.Information
                    bPopup = False
                Case eMessageImportance.Maintenance, _
                     eMessageImportance.Progress
                    Return
                Case Else
                    Return
            End Select
        End If

        ' Need to show a popup for this message?
        If bPopup Then
            ' #Yes: go ahead, Jimmy
            ' JS 26feb09: If an error occurred the status panel will have to show this message
            Me.ShowMessageBox(msg)
        End If

        ' Show message in status panel

        ' Prepare treenode
        Dim tnMessage As TreeNode = New TreeNode(Me.ToTreeNodeText(strMessage))
        ' Set image index
        tnMessage.ImageIndex = CInt(msg.Importance) - 1
        ' Set selected image to equal image index
        tnMessage.SelectedImageIndex = tnMessage.ImageIndex
        ' Add original message text to tooltip
        tnMessage.ToolTipText = msg.Message

        ' JS 07may07: Whoah, a hack... if a message comes in with only one variable AND the 
        '             message for the variable equals the text of the main message THEN suppress 
        '             the variable message...
        If msg.Variables.Count = 1 Then
            bSuppressVarMessage = String.Compare(msg.Variables(0).Message, msg.Message, True) = 0
        End If

        ' ***********************************************************************
        ' ***                                                                 ***
        ' *** If your code crashes below for an unexplained reason a message  ***
        ' *** has probably been sent from a thread other than the GUI thread. ***
        ' ***                                                                 ***
        ' ***********************************************************************

        If bSuppressVarMessage Then
            tnMessage.Tag = msg.Variables(0)
        Else

            ' Add original message to the master node
            tnMessage.Tag = msg

            ' Create subnodes for each variable status entry in the message
            For Each vs As cVariableStatus In msg.Variables
                ' Prepare child node
                Dim tnVariable As New TreeNode(Me.ToTreeNodeText(vs.Message))
                ' Set same image as parent node
                tnVariable.ImageIndex = tnMessage.ImageIndex
                tnVariable.SelectedImageIndex = tnMessage.ImageIndex
                tnVariable.ToolTipText = vs.Message

                '' Set selected image to equal image index
                'tnVariable.SelectedImageIndex = tnVariable.ImageIndex

                ' Add variable status to the tag of the node. This will be used to
                ' highlight cProperties at runtime whenever a user presses the mouse
                ' button on the treenode. The properties are not resolved here because
                ' this message came from the Core. The GUI might not have created the
                ' properties yet.
                tnVariable.Tag = vs
                ' Now add the variable child node to the message parent node
                tnMessage.Nodes.Add(tnVariable)
            Next

            ' Add feedback reply
            If (TypeOf (msg) Is cFeedbackMessage) Then

                Dim fmsg As cFeedbackMessage = DirectCast(msg, cFeedbackMessage)
                Dim tnReply As New TreeNode()

                Select Case fmsg.ReplyStyle

                    Case cFeedbackMessage.eReplyStyle.OK_CANCEL
                        Select Case fmsg.Reply
                            Case cFeedbackMessage.eReply.OK
                                tnReply.Text = My.Resources.GENERIC_REPLY_OK
                            Case cFeedbackMessage.eReply.CANCEL
                                tnReply.Text = My.Resources.GENERIC_REPLY_CANCEL
                        End Select

                    Case cFeedbackMessage.eReplyStyle.YES_NO, _
                         cFeedbackMessage.eReplyStyle.YES_NO_CANCEL

                        Select Case fmsg.Reply
                            Case cFeedbackMessage.eReply.YES
                                tnReply.Text = My.Resources.GENERIC_REPLY_YES
                            Case cFeedbackMessage.eReply.NO
                                tnReply.Text = My.Resources.GENERIC_REPLY_NO
                            Case cFeedbackMessage.eReply.CANCEL
                                tnReply.Text = My.Resources.GENERIC_REPLY_CANCEL

                        End Select

                End Select

                tnReply.ImageIndex = tnMessage.ImageIndex
                tnReply.SelectedImageIndex = tnMessage.ImageIndex
                tnReply.ToolTipText = tnReply.Text

                tnMessage.Nodes.Add(tnReply)

            End If
        End If

        Try
            ' Add node(s) to the TOP of the list
            Me.tvStatus.Nodes.Insert(0, tnMessage)
            ' Truncate log size
            While (Me.tvStatus.Nodes.Count = iMaxMessages)
                ' Remove old messages from the bottom of the list
                Me.tvStatus.Nodes.RemoveAt(iMaxMessages - 1)
            End While

            ' JS 10feb2010: ensure visible not always seem to do reveal the newest item
            'tnMessage.EnsureVisible()
            Me.tvStatus.TopNode = tnMessage

        Catch ex As Exception
            ' Hmm
        End Try

        ' When the core sends out critical or warning message, status panel will slide open temporarily
        If (msg.Importance = eMessageImportance.Critical) Or (msg.Importance = eMessageImportance.Warning) Then
            ' Is dockable AND is in auto-hinding state?
            If (Me.DockPanel IsNot Nothing) And _
               ((Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide) Or _
                (Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide) Or _
                (Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide) Or _
                (Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide)) Then
                Try
                    Me.DockPanel.ActiveAutoHideContent = Me
                Catch ex As Exception
                    ' Nou ja, zeg
                End Try
            End If

        End If

    End Sub

#End Region ' Core message handling 

#Region " Helper methods "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Return the list of <see cref="cProperty">Properties</see> for a given 
    ''' <see cref="TreeNode">TreeNode</see>.
    ''' </summary>
    ''' <param name="tn">The <see cref="TreeNode">TreeNode</see> to extract the
    ''' message from.</param>
    ''' <returns>A list of cProperty objects. This list is empty if an 
    ''' invalid node is provided.</returns>
    ''' -------------------------------------------------------------------
    Private Function GetPropertylistFromNode(ByVal tn As TreeNode) As List(Of cProperty)

        Dim lp As New List(Of cProperty)
        Dim lpChild As List(Of cProperty)
        Dim prop As cProperty = Nothing

        ' If no node clicked then return an empty list
        If Object.ReferenceEquals(tn, Nothing) Then Return lp

        If Not Object.ReferenceEquals(tn.Tag, Nothing) Then
            If TypeOf (tn.Tag) Is cVariableStatus Then
                prop = cPropertyManager.GetInstance().ExtractProperty(DirectCast(tn.Tag, cVariableStatus))
                If Not Object.ReferenceEquals(prop, Nothing) Then
                    lp.Add(prop)
                End If
            End If
        End If

        For Each tnChild As TreeNode In tn.Nodes
            lpChild = GetPropertylistFromNode(tnChild)
            lp.InsertRange(lp.Count, lpChild)
        Next

        Return lp
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; reformats a piece of text to fit in a single-line
    ''' tree node item.
    ''' </summary>
    ''' <param name="strText">The text to format.</param>
    ''' <returns>The formatted text.</returns>
    ''' -------------------------------------------------------------------
    Private Function ToTreeNodeText(ByVal strText As String) As String
        If String.IsNullOrEmpty(strText) Then Return ""
        Return strText.Replace(vbNewLine, " ")
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

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; handles a feedback message by presenting the user with
    ''' a message box.
    ''' </summary>
    ''' <param name="msg">The <see cref="cFeedbackMessage">feedback message</see>
    ''' to handle.</param>
    ''' -------------------------------------------------------------------
    Private Sub HandleFeedbackMessage(ByVal msg As cFeedbackMessage)

        Dim mbs As MessageBoxButtons = MessageBoxButtons.YesNo
        Dim mbi As MessageBoxIcon = MessageBoxIcon.Question
        Dim dlr As DialogResult = Windows.Forms.DialogResult.No
        Dim strMessage As String = ""

        If (msg Is Nothing) Then Return

        ' Translate feedback style into .NET MessageBox style
        Select Case msg.ReplyStyle
            Case cFeedbackMessage.eReplyStyle.OK_CANCEL
                mbs = MessageBoxButtons.OKCancel
            Case cFeedbackMessage.eReplyStyle.YES_NO
                mbs = MessageBoxButtons.YesNo
            Case cFeedbackMessage.eReplyStyle.YES_NO_CANCEL
                mbs = MessageBoxButtons.YesNoCancel
        End Select

        Select Case msg.Importance
            Case eMessageImportance.Progress, eMessageImportance.Maintenance, eMessageImportance.Information
                mbi = MessageBoxIcon.Question
            Case eMessageImportance.Warning
                mbi = MessageBoxIcon.Warning
            Case eMessageImportance.Critical
                mbi = MessageBoxIcon.Error
        End Select

        ' ToDo_JS: (LOW) Consider how to handle a list of choices in the message. This will require dynamic dialog construction, ouch!

        ' Pop the question
        Me.ToMessageBoxText(msg, strMessage)

        '' Is message suppressable?
        'If msg.Suppressable Then

        '    ' #Yes: handle autoreply

        '    ' Sanity check
        '    Debug.Assert(msg.Type <> eMessageType.NotSet, "Feedback message not propery configured for auto-reply: messagetype not set")

        '    ' Get reply, if any
        '    dlr = Me.m_msh.AutoReply(msg.Source, msg.Type)
        '    ' Is 'none'?
        '    If (dlr = Windows.Forms.DialogResult.None) Then
        '        ' #Yes: prompt needed

        '        ' Assume to repeat the question
        '        Dim bChecked As Boolean = False
        '        ' Show dialog
        '        dlr = CheckedMessageBox.Show(strMessage, "test", mbs, mbi, _
        '            bChecked, My.Resources.GENERIC_PROMPT_USEANSWERAGAIN)
        '        ' Auto-reply requested?
        '        If bChecked Then
        '            ' #Yes: store auto-reply
        '            Me.m_msh.AutoReply(msg.Source, msg.Type) = dlr
        '        End If
        '    End If
        'Else
        dlr = MessageBox.Show(strMessage, AppLauncher.GetInstance().Text, mbs, mbi)
        'End If

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
            'If msg.Suppressable Then

            '    ' #Yes: check suppressed state

            '    ' Sanity check
            '    Debug.Assert(msg.Type <> eMessageType.NotSet, "Message not propery configured for suppression: messagetype not set")

            '    If (Not Me.m_msh.Suppress(msg.Source, msg.Type)) Then
            '        ' #No: Good, prepare to show message
            '        ' Assume message will not be suppressed
            '        Dim bSuppress As Boolean = False
            '        ' Invoke the special message box
            '        CheckedMessageBox.Show(strMessage, "test", mbb, mbi, _
            '                bSuppress, My.Resources.GENERIC_PROMPT_DONOTSHOWMESSAGEAGAIN, _
            '                MessageBoxDefaultButton.Button1)
            '        If bSuppress Then
            '            '#Yes: suppress it during the rest of this session
            '            Me.m_msh.Suppress(msg.Source, msg.Type) = True
            '        End If
            '    End If
            'Else
            '' #No: show the message
            MessageBox.Show(strMessage, AppLauncher.GetInstance().Text, mbb, mbi, MessageBoxDefaultButton.Button1)
            'End If
        End If

            Return bError
    End Function

#End Region ' Helper methods

#Region " Message highlighting "

    ''' <summary>List of highlighted properties.</summary>
    Private m_lpHighlighted As New List(Of cProperty)

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Sets the properties to highlight
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub SetHighlights(ByVal lp As List(Of cProperty))

        ' Clear current highlights, if any
        If m_lpHighlighted.Count > 0 Then
            ' Clear current highlights
            HighlightProperties(False)
            ' Clear list of highlights
            Me.m_lpHighlighted.Clear()
        End If

        If lp Is Nothing Then Return

        ' Set new highlights, if any
        If lp.Count > 0 Then
            ' Update list of highlights
            Me.m_lpHighlighted.InsertRange(0, lp)
            ' Set the highlights
            HighlightProperties(True)
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; sets the highlight state for the properties for a given message
    ''' </summary>
    ''' <param name="bHighlight">Flag, stating the new highlight state for the proeprties for this cMessage</param>
    ''' -------------------------------------------------------------------
    Private Sub HighlightProperties(ByVal bHighlight As Boolean)

        Dim bsm As cProperty.eBitSetMode = cProperty.eBitSetMode.BitwiseOn

        ' Figure out if highlight bits need to be set or cleared
        If bHighlight Then
            ' Highlight bit needs to be set
            bsm = cProperty.eBitSetMode.BitwiseOn
        Else
            ' Highlight bit needs to be cleared
            bsm = cProperty.eBitSetMode.BitwiseOff
        End If

        ' Toggle highlight bit for each property
        For Each p As cProperty In Me.m_lpHighlighted
            p.SetStyle(cStyleGuide.eStyleFlags.Highlight, TriState.UseDefault, bsm)
        Next
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; traps the mouse down event to initiate property highlighting for a given index
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub lbStatus_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tvStatus.MouseDown
        ' Get node that the user clicked on, if any
        Dim tn As TreeNode = Me.tvStatus.GetNodeAt(e.Location)
        ' Extract list op properties for this node and its child nodes
        Dim lp As List(Of cProperty) = Me.GetPropertylistFromNode(tn)
        ' Highlight these properties
        SetHighlights(lp)
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; traps the mouse up event to end property highlighting for a given index
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub lbStatus_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tvStatus.MouseUp
        ' Clear any highlights
        SetHighlights(Nothing)
    End Sub

#End Region ' Message highlighting

#Region " Context Menu "

    Private Sub Item_Remove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Item_Remove.Click
        If Not Object.ReferenceEquals(Me.tvStatus.SelectedNode, Nothing) Then
            Me.tvStatus.Nodes.Remove(Me.tvStatus.SelectedNode)
        End If
    End Sub

    Private Sub Item_RemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Item_RemoveAll.Click
        Me.Reset()
    End Sub

    Private Sub cmenuListBox_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmenuListBox.Opening
        ' Update menu items state
        Me.Item_Remove.Enabled = Not Object.ReferenceEquals(Me.tvStatus.SelectedNode, Nothing)
        Me.Item_RemoveAll.Enabled = (Me.tvStatus.Nodes.Count > 0)
    End Sub

#End Region ' Context menu

End Class

