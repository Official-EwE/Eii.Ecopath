Option Strict On
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' A feedback message is the only vehicle for the EwE Core to prompt a user
''' interface for feedback.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFeedbackMessage
    Inherits cMessage

#Region " Public helper classes and enumerators "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, implements a choice presented by a cFeedbackMessage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cChoice
        Private m_strText As String = ""
        Private m_tag As Object = Nothing
        Private m_bSelected As Boolean = False

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="strText">The text to display for this choice.</param>
        ''' <param name="tag">An optional value to associate with this choice.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strText As String, Optional ByVal tag As Object = Nothing)
            Me.m_strText = strText
            Me.m_tag = tag
        End Sub

        ''' <summary>
        ''' Get the text for this choice.
        ''' </summary>
        Public ReadOnly Property Text() As String
            Get
                Return Me.m_strText
            End Get
        End Property

        ''' <summary>
        '''  Get the Tag for this choice.
        ''' </summary>
        Public ReadOnly Property Tag() As Object
            Get
                Return Me.m_tag
            End Get
        End Property

        ''' <summary>
        ''' Get or set the selection state of this choice.
        ''' </summary>
        Public Property Selected() As Boolean
            Get
                Return Me.m_bSelected
            End Get
            Set(ByVal bSelected As Boolean)
                Me.m_bSelected = bSelected
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that defines possible replies to a cFeedbackMessage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eReply As Byte
        ''' <summary>This reply indicates that the situation pertaining to the message has to be aborted.</summary>
        CANCEL = 0
        ''' <summary><para>This reply indicates that the situation pertaining to the message is positively confirmed.</para>
        ''' <para>A YES reply is identical to an <see cref="eReply.OK">OK</see> reply.</para></summary>
        YES
        ''' <summary><para>This reply indicates that the situation pertaining to the message is positively confirmed.</para>
        ''' <para>An OK reply is identical to a <see cref="eReply.YES">YES</see> reply.</para></summary>
        OK = YES
        ''' <summary>This reply indicates that the situation pertaining to the message is negatively confirmed.</summary>
        NO
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type that defines possible replie styles that cFeedbackMessages can handle.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eReplyStyle As Byte
        ''' <summary>The reply expected by a message with this <see cref="ReplyStyle">reply style</see> is either <see cref="eReply.OK">OK</see> or <see cref="eReply.CANCEL">CANCEL</see>.</summary>
        OK_CANCEL
        ''' <summary>The reply expected by a message with this <see cref="ReplyStyle">reply style</see> is either <see cref="eReply.YES">YES</see> or <see cref="eReply.NO">NO</see>.</summary>
        YES_NO
        ''' <summary>The reply expected by a message with this <see cref="ReplyStyle">reply style</see> must be <see cref="eReply.YES">YES</see>, <see cref="eReply.NO">NO</see> or <see cref="eReply.CANCEL">CANCEL</see>.</summary>
        YES_NO_CANCEL
    End Enum

#End Region ' Public helper classes and enumerators 

#Region " Private bits "

    ''' <summary>Reply to message.</summary>
    Private m_reply As eReply = eReply.CANCEL
    ''' <summary>Reply style requested for this message.</summary>
    Private m_replyStyle As eReplyStyle = eReplyStyle.OK_CANCEL
    ''' <summary>Available choices to offer for selection in the feedback message.</summary>
    Private m_choices As New List(Of cChoice)

#End Region ' Private bits 

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Default constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="msgStr">Message text.</param>
    ''' <param name="msgSource"><see cref="eCoreComponentType">Source</see> of the message.</param>
    ''' <param name="msgImportance"><see cref="eMessageImportance">Importance</see> of the message.</param>
    ''' <param name="replyStyle"><see cref="eReplyStyle">Reply style</see> of the message.</param>
    ''' <param name="msgDataType"><see cref="eDataTypes">Data type</see> associated with the message, if any.</param>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal msgStr As String, ByVal msgSource As eCoreComponentType, ByVal msgImportance As eMessageImportance, _
            Optional ByVal replyStyle As eReplyStyle = eReplyStyle.OK_CANCEL, Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet, _
            Optional ByVal defaultReply As eReply = eReply.CANCEL)
        MyBase.New(msgStr, eMessageType.Any, msgSource, msgImportance, msgDataType)

        Me.m_replyStyle = replyStyle
        Me.m_reply = defaultReply
    End Sub

#End Region ' Construction

#Region " Property access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the reply to this message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Reply() As eReply
        Get
            Return Me.m_reply
        End Get
        Set(ByVal value As eReply)
            Me.m_reply = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the reply style to this message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ReplyStyle() As eReplyStyle
        Get
            Return m_replyStyle
        End Get
        Set(ByVal value As eReplyStyle)
            m_replyStyle = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the list of <see cref="cChoice">choices</see> that this message offers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Choices() As List(Of cChoice)
        Get
            Return Me.m_choices
        End Get
    End Property

#End Region ' Property access 

End Class
