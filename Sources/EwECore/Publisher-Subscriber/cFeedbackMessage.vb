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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
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

#Region " Internal vars "

    Private m_customlabels(4) As String

#End Region ' Internal vars

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
    Public Sub New(msgStr As String,
                   msgSource As eCoreComponentType,
                   msgType As eMessageType,
                   msgImportance As eMessageImportance,
                   Optional replyStyle As eMessageReplyStyle = eMessageReplyStyle.OK_CANCEL,
                   Optional msgDataType As eDataTypes = eDataTypes.NotSet,
                   Optional defaultReply As eMessageReply = eMessageReply.CANCEL)

        MyBase.New(msgStr, msgType, msgSource, msgImportance, msgDataType)

        Me.ReplyStyle = replyStyle
        Me.Reply = defaultReply

    End Sub

#End Region ' Construction

#Region " Property access "

    Public Overrides Property Importance As EwEUtils.Core.eMessageImportance
        Get
            If MyBase.Importance <> eMessageImportance.Critical And MyBase.Importance <> eMessageImportance.Warning Then
                ' JS 21Oct13: Revised importance feedback in case it is unknown. 
                '             If only possible answer is 'OK', and text does not contain an obvious question,
                '             then set unknown importance to 'information'. In all other cases, revert to a question mark.
                If (Me.ReplyStyle = eMessageReplyStyle.OK) And (Me.Message.IndexOf("?"c) = -1) Then
                    Return eMessageImportance.Information
                Else
                    Return eMessageImportance.Question
                End If
            End If
            Return MyBase.Importance
        End Get
        Set(value As EwEUtils.Core.eMessageImportance)
            MyBase.Importance = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IFeedbackMessage.Reply"/>
    ''' -----------------------------------------------------------------------
    Public Property Reply() As EwEUtils.Core.eMessageReply Implements IFeedbackMessage.Reply

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IFeedbackMessage.ReplyStyle"/>
    ''' -----------------------------------------------------------------------
    Public Property ReplyStyle() As EwEUtils.Core.eMessageReplyStyle Implements IFeedbackMessage.ReplyStyle

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the text to display for licensing options.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property HyperlinkOption As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set a custom reply label to use when this message is shown in a user interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CustomReplyLabel(reply As EwEUtils.Core.eMessageReply) As String
        Get
            Return Me.m_customlabels(reply)
        End Get
        Set(value As String)
            Me.m_customlabels(reply) = value
        End Set
    End Property

#End Region ' Property access 

End Class
