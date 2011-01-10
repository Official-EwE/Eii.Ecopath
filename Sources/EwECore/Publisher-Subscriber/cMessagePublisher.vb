Option Strict On
Imports EwEUtils.Core

''' <summary>
''' This class provides a single interface for dealing with different types of messages.
''' It uses a Publisher/Subscriber or Observer pattern
''' </summary>
''' <remarks>
''' How to use:
''' For a given message create a new cMessageHandler object and add it to the message handlers via the cMessagePublisher.AddMessageHandler(...) method
''' Then when you want to send a message create a new cMessage object with the information you want sent and call the cMessagePublisher.SendMessage(cMessage) method 
''' This will send the message to any message handler that has been set-up to listen for this type of message. 
''' If no handler was defined for this type of message then the default handler will be used. 
''' If no default handler has been created then cMessagePublisher.SendMessage will return False.
''' </remarks>
Public Class cMessagePublisher

    Private m_handlers As New List(Of cMessageHandler)
    Private m_msglist As New List(Of cMessage)
    Private m_iMessageLockCount As Integer = 0
    Private m_bSendPending As Boolean = False

    ''' <summary>
    ''' Add a Message Handler to list of message handlers
    ''' </summary>
    ''' <param name="MessageHandler">The Message handler to add</param>
    ''' <returns>True is successfull. False otherwise</returns>
    ''' <remarks>
    ''' At this time there is no checking to see if there already is a message handler for this message
    ''' So if you define two message handler for the same message they will both get called
    ''' </remarks>
    Public Function AddMessageHandler(ByVal MessageHandler As cMessageHandler) As Boolean

        Try
            m_handlers.Add(MessageHandler)

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Add a <see cref="cMessage">message</see> to the message publisher.
    ''' </summary>
    ''' <param name="msg"><see cref="cMessage">Message</see> to be sent.</param>
    ''' <returns>True is the message was added to the Queue or sent to the handler</returns>
    ''' <outdated_remarks>
    ''' If bHoldMessages = True then the message will be added to the queue and on the next call to SendAllMessages()
    ''' IF bHoldMessages = False then the message will be sent immediately and will not be added to the queue
    ''' </outdated_remarks>
    ''' <remarks>
    ''' <see cref="cMessage.Equals">Duplicate messages</see> are not added.
    ''' </remarks>
    Public Function AddMessage(ByVal msg As cMessage) As Boolean

        Try
            ' Is this message not a duplicate?
            If (Not Me.FindDuplicateMessage(msg)) Then
                ' Queue the message
                m_msglist.Add(msg)
            Else
                ' Ignore the message
                ' Console.WriteLine("Ignoring duplicate message")
            End If
            ' All well
            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Send all the messages in the queue.
    ''' </summary>
    ''' <returns>True if no problems were encountered. False if an error was thrown.</returns>
    Friend Function sendAllMessages() As Boolean
        Dim msg As cMessage = Nothing

        ' JS 13oct06: sending messages from a for..each loop may affect the hidden enumerator used here IF 
        '             particular message handles decide to add or remove new messages, or if handlers get
        '             terminated in response to a message. A less error-prone solution is to send 
        '             messages from a temporary copy of the messages list... :(
        Dim msgs() As cMessage = m_msglist.ToArray()

        Try
            ' Clear the list
            m_msglist.Clear()

            ' Send all messages
            For i As Integer = 0 To msgs.Length - 1
                SendMessage(msgs(i))
            Next

            ' Done
            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".SendAllMessages() Error: " & ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Send the message to the all handlers that can handle this type of message or to the default handler for this eCoreComponentType.
    ''' </summary>
    ''' <param name="Message">The <see cref="cMessage">message</see> to send right away.</param>
    ''' <param name="bPassLock">Flag stating that the message should pass through an active 
    ''' <see cref="SetMessageLock">message lock</see>. This is not allowed by default.</param>
    ''' <returns>True if the message was sent. False if the message was not sent.</returns>
    ''' <remarks>
    ''' <para>This will first try to send the message to the handler specific to this message type. 
    ''' If that fails, it will try to find the default message handler for this MessageSource.
    ''' If that also fails, this method will return False.</para>
    ''' <para>No messages are sent if a <see cref="SetMessageLock">message lock</see> is in place.
    ''' Locked messages will be sent the moment this lock is <see cref="RemoveMessageLock">cleared</see>.</para>
    ''' <para>Note that <see cref="cFeedbackMessage">Feedback messages</see> are allowed to pass the lock at any time.</para>
    ''' </remarks>
    Public Function SendMessage(ByVal Message As cMessage, Optional ByVal bPassLock As Boolean = False) As Boolean
        Dim handler As cMessageHandler
        Dim bMessageHandled As Boolean

        ' Special case: feedback messages always pass the lock
        bPassLock = bPassLock Or (TypeOf Message Is cFeedbackMessage)

        ' Message lock in place?
        If (Me.m_iMessageLockCount > 0 And Not bPassLock) Then
            ' Hmm, cannot send message. Just queue it then; AddMessage will prevent queueing of duplicate messages.
            Me.AddMessage(Message)
            ' Remember this
            Me.m_bSendPending = True
            ' Report succes, message will be sent at some point
            Return True
        End If

        Try

            ' Wrapped sanity checks
            If Message Is Nothing Then
                Debug.Assert(False, "Cannot send a Null message.")
            End If
            If (Message.Source = eCoreComponentType.NotSet) Then
                Debug.Assert(False, "Message source must be set.")
            End If

            ' JS 27sep07: log only messages of certain importance
            Select Case Message.Importance
                Case eMessageImportance.Critical, eMessageImportance.Warning, eMessageImportance.Information
                    cLog.Write(Message)
                Case eMessageImportance.Progress, eMessageImportance.Maintenance
                    ' Do not log these
            End Select

            ' JS 09Dec08: Handle messages on a COPY of the original handlers list, since message handlers may
            '             unsubscribe themselves from messages in response to a message. In the current
            '             approach, where messages are sent by iterating over the original message handlers list,
            '             the handler collection may thus change and the iteration process will throw an
            '             exception, aborting the message handling process.
            '             Some handlers may then not receive messages.
            Dim aHandlers As cMessageHandler() = Me.m_handlers.ToArray()
            'Loop over the list of handlers and asking each one to post the message
            'if a handler can handle this type of message it will return True (the message has been handled)
            For Each handler In aHandlers
                If handler.SendMessage(Message) Then
                    'this handler is for this type of message
                    bMessageHandled = True
                    'even though the message has been handled 
                    'try all the handlers to see if there is another one that can handle this type of message
                End If
            Next handler

            If Not bMessageHandled Then
                'something is very wrong 
                'the owner of this publisher has tried to send a message that has no handler

                'Debugging issue you have not defined a handler for this type of message
                '  Debug.Assert(messageHandled, "No default message handler defined for source = " & Message.Source.ToString)
                cLog.Write(Me.ToString & ".SendMessage(...) No default message handler defined for source = " & Message.Source.ToString)
            End If

            Return bMessageHandled

        Catch ex As Exception
            cLog.Write(Me.ToString & ".SendMessage(...) Error: " & ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Remove ALL instances of a Message Handler
    ''' </summary>
    ''' <param name="MessageHandlerToRemove">cMessageHandler object to remove</param>
    ''' <returns>True if successful. False otherwise</returns>
    ''' <remarks>This tests the Delegate contained in MessageHandlerToRemove not the cMessageHandler itself. 
    ''' So it will remove All of the message handlers that use this delegate.
    ''' This calls cMessageHandler.Equals().
    ''' </remarks>
    Public Function RemoveMessageHandler(ByVal MessageHandlerToRemove As cMessageHandler) As Boolean

        Dim bFound As Boolean
        Try

            bFound = m_handlers.Remove(MessageHandlerToRemove)

            Return bFound
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' This method is too dangerous. Calling classes should properly call
    ' RemoveMessageHandler for each handler that were added.
    'Public Sub Clear()

    '    Try
    '        For Each handler As cMessageHandler In Me.m_handlers
    '            handler.Clear()
    '        Next
    '        m_handlers.Clear()
    '    Catch ex As Exception
    '        cLog.Write(ex)
    '    End Try

    'End Sub

#Region " Locking "

    ''' <summary>
    ''' Increase the Messages lock count. While this lock count is active, no messages will
    ''' be sent.
    ''' </summary>
    Public Sub SetMessageLock()
        ' Increase lock count
        Me.m_iMessageLockCount += 1
    End Sub

    ''' <summary>
    ''' Decrease the Messages lock count. While this lock count is active, no messages will
    ''' be sent. When the lock reaches zero, any obstructed SendMessage call will be completed.
    ''' </summary>
    Public Sub RemoveMessageLock()
        ' Decrease lock count
        Me.m_iMessageLockCount -= 1
        ' Lock cleared and send pending?
        If (0 = Me.m_iMessageLockCount) And (Me.m_bSendPending = True) Then
            ' #Yes: send all
            Me.sendAllMessages()
            ' Clear pending flag
            Me.m_bSendPending = False
        End If
    End Sub

#End Region ' Locking

#Region " Private helper methods "

    ''' <summary>
    ''' Test whether the message queue already contains a duplicate of a test message.
    ''' </summary>
    ''' <param name="msg">The message to test for.</param>
    ''' <returns>
    ''' True if the queue contains a duplicate of the test message, False otherwise.
    ''' </returns>
    Private Function FindDuplicateMessage(ByVal msg As cMessage) As Boolean

        ' Check if similar message not already in the list
        For Each msgQueued As cMessage In Me.m_msglist
            ' Message already found?
            If msg.Equals(msgQueued) Then
                ' #Yes: copy all new variables from msg to msqQueued
                For Each vs As cVariableStatus In msg.Variables
                    ' Variable not already present?
                    If Not msgQueued.HasVariable(vs) Then
                        ' #Yes: add message
                        msgQueued.Variables.Add(vs)
                    End If
                Next
                Return True
            End If
        Next
        Return False
    End Function

#End Region ' Private helper methods

End Class






