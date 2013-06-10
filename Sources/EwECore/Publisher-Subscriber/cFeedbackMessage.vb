' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

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
    Implements IFeedbackMessage

#Region " Private bits "

    ''' <summary>Reply to message.</summary>
    Private m_reply As EwEUtils.Core.eMessageReply = EwEUtils.Core.eMessageReply.CANCEL
    ''' <summary>Reply style requested for this message.</summary>
    Private m_replyStyle As EwEUtils.Core.eMessageReplyStyle = EwEUtils.Core.eMessageReplyStyle.OK_CANCEL

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
    ''' <param name="replyStyle"><see cref="eMessageReplyStyle">Reply style</see> of the message.</param>
    ''' <param name="msgDataType"><see cref="eDataTypes">Data type</see> associated with the message, if any.</param>
    ''' <param name="msgType"></param>
    ''' <param name="defaultReply"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal msgStr As String, _
                   ByVal msgSource As eCoreComponentType, _
                   ByVal msgType As eMessageType, _
                   ByVal msgImportance As eMessageImportance, _
                   Optional ByVal replyStyle As eMessageReplyStyle = eMessageReplyStyle.OK_CANCEL, _
                   Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet, _
                   Optional ByVal defaultReply As eMessageReply = eMessageReply.CANCEL)

        MyBase.New(msgStr, msgType, msgSource, msgImportance, msgDataType)

        Me.m_replyStyle = replyStyle
        Me.m_reply = defaultReply

    End Sub

#End Region ' Construction

#Region " Property access "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IFeedbackMessage.Reply"/>
    ''' -----------------------------------------------------------------------
    Public Property Reply() As EwEUtils.Core.eMessageReply Implements IFeedbackMessage.Reply
        Get
            Return Me.m_reply
        End Get
        Set(ByVal value As EwEUtils.Core.eMessageReply)
            Me.m_reply = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IFeedbackMessage.ReplyStyle"/>
    ''' -----------------------------------------------------------------------
    Public Property ReplyStyle() As EwEUtils.Core.eMessageReplyStyle Implements IFeedbackMessage.ReplyStyle
        Get
            Return m_replyStyle
        End Get
        Set(ByVal value As EwEUtils.Core.eMessageReplyStyle)
            m_replyStyle = value
        End Set
    End Property

#End Region ' Property access 

End Class
