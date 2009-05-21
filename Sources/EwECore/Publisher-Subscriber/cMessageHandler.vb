'==============================================================================
'
' $Log: cMessageHandler.vb,v $
' Revision 1.4  2009/05/21 18:53:37  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/19 18:07:24  jeroens
' MessageHandlers, CoreStateMonitor have sync objects
'
' Revision 1.2  2009/01/16 18:30:29  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:29  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict Off
Imports System.ComponentModel
Imports EwEUtils.Core

''' <summary>
''' <para>This class is the message handler portion of the Message-Publisher/Message-Handler pattern use to pass messages from the core(publisher) to an interface(handler).</para>
''' <para>This pattern is based on the Subject/Observer or Publisher/Subscriber patterns.
''' Basically the cMessageHandler provides a wrapper for a delegate in the interface that handles the message. 
''' This allows the interface to implement the actual message handling without having to expose its internal structure.</para>
''' <para>It also allows an interface to split the message handling into a series of smaller routines that only know how to handle one type of message.</para>
''' </summary>
''' <remarks>
''' <para>How to use cMessageHandler:</para>
''' <para>Define a method in your interface that will do the actual message handling with the same signature as EwECore.cCore.CoreMessageDelegate(cMessage).</para>
''' <para>Create a cMessageHandler object and in its constructor pass in 3 arguments</para>
''' <para>1.) The AddressOf the delegate that will handle the message</para>
''' <para>2.) Source of the message (eCoreComponentType)</para>
''' <para>3.) The message to handle (eMessageType)</para>
''' <para>This tells the handler what type of message to handle and where to send the message.</para>
''' <para>Next</para>
''' <para>Add the cMessageHandler object to the cCore.Messages.AddMessageHandler(cMessageHandler) interface. This will register the message handler with the core and any messages of this type will be sent to the delegate defined in the interface.</para>
''' <para>For a default message handler set the eMessageType flag to 'eMessageType.Any'. This handler will be sent any messages that do not have a specific handler.</para>
'''  </remarks>
''' <history>
''' <revision>jb 15/mar/06 Removed SendMessageUseDefaults()</revision>
'''</history>
Public Class cMessageHandler

    Private m_DelegateNotifier As EwECore.cCore.CoreMessageDelegate
    Private m_syncobj As ISynchronizeInvoke = Nothing
    Private m_corecomponent As eCoreComponentType
    Private m_msgtype As eMessageType

    ''' <summary>
    ''' Constructs a new cMessageHandler object that will send messages of a given type to the DelegateToCall argument.
    ''' </summary>
    ''' <param name="DelegateToCall">Delegate that will handle the message</param>
    ''' <param name="SourceToHandle">Source of the message i.e EcoPath</param>
    ''' <param name="MessageTypeToHandle">Type of message to handle i.e. DietComp this message will only handle the DietComp not summing to one message</param>
    ''' <remarks>
    ''' <para>For a default handler set the MessageTypeToHandle flag to eMessageType.Any this will send any unhandled message to this delegate.</para>
    ''' <para>To have s single delegate handle multiple messages create a new cMessageHandler with this same 'DelegateToCall' argument and a different MessageTypeToHandle flag.</para>
    ''' </remarks>
    Sub New(ByVal DelegateToCall As EwECore.cCore.CoreMessageDelegate, ByVal SourceToHandle As eCoreComponentType, ByVal MessageTypeToHandle As eMessageType, ByVal syncobj As ISynchronizeInvoke)

        Me.m_DelegateNotifier = DelegateToCall
        Me.m_corecomponent = SourceToHandle
        Me.m_msgtype = MessageTypeToHandle
        Me.m_syncobj = syncobj

    End Sub

    ''' <summary>
    ''' Called by the cMessagePublisher to send a message to a message specific handler. 
    ''' If this cMessageHandler can handle this type of message the message will be sent to the Delegate passed in when this object was constructed.
    ''' </summary>
    ''' <param name="message">Message to send. This handler will used the Type and Source flags of the message to see if it can handle this type of message</param>
    ''' <returns>
    ''' <para>True if this message handler can handle this type of message.</para>
    ''' <para>False if the message was not handled or a problem was encountered.</para>
    ''' </returns>
    ''' <remarks>
    ''' For the message to be handled it must have the same Type and Source as this handler.
    ''' </remarks>
    Friend Function SendMessage(ByRef message As cMessage) As Boolean

        Debug.Assert(Not m_DelegateNotifier Is Nothing)

        Try
            'test for a NULL delegate this should not be possible but check anyway
            If Not m_DelegateNotifier Is Nothing Then

                'test the type and source of the message
                ' JS 15Mar06: test for MessageType.Any
                If (message.Type = m_msgtype Or m_msgtype = eMessageType.Any) And _
                   (message.Source = m_corecomponent) Then

                    Try
                        If Object.ReferenceEquals(Me.m_syncobj, Nothing) Then
                            m_DelegateNotifier(message)
                        Else
                            Me.m_syncobj.Invoke(Me.m_DelegateNotifier, New Object() {message})
                        End If
                    Catch ex As Exception
                        'Error thrown in the handler by an interface that was not handled 
                        'we have no idea if this message got handled or not
                        cLog.Write(ex)
                        Debug.Assert(False, Me.ToString & ".SendMessage(cMessage) Error thrown by an interface message handler.")
                        Return False
                    End Try

                    'this message was handled so return True
                    Return True
                End If 'If message.MessageType = m_Type And message.MessageSource = m_source Then

            Else 'If Not m_DelegateNotifier Is Nothing Then

                'delegate = NULL
                'can't really send a message now can we!!!
                cLog.Write(Me.ToString & ".SendMessage(cMessage) Delegate has not been initialized.")
                Return False

            End If 'If Not m_DelegateNotifier Is Nothing Then

        Catch ex As Exception
            cLog.Write(Me.ToString & ".SendMessage(cMessage)  Error:" & ex.Message)
            Debug.Assert(False, "Error in Subscriber.SendMessage")
            Return False
        End Try

        'this handler can not handle this type of message so return False
        Return False

    End Function

    ''' <summary>
    ''' Test for equality of Delegates.
    ''' </summary>
    ''' <param name="Handler">cMessageHandler object to test</param>
    ''' <returns>True if this Message Handlers delagate is equal to the one bing passed in. False otherwise</returns>
    ''' <remarks>This tests the underlying delegates for equality NOT the cMessageHandlers them selves.</remarks>
    Public Overrides Function Equals(ByVal Handler As Object) As Boolean

        If m_DelegateNotifier.Equals(Handler.getDelegate) Then
            Return True
        Else
            Return False
        End If

    End Function

    ''' <summary>
    ''' Return the underlying Delagate.
    ''' </summary>
    ''' <returns>CoreMessageDelegate delagate object.</returns>
    ''' <remarks>This is used by  Equals() to test for equality of two cMessageHandler objects.</remarks>
    Public Function getDelegate() As cCore.CoreMessageDelegate
        Return m_DelegateNotifier
    End Function


End Class

