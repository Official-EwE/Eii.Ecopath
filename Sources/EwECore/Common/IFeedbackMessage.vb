' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for requesting user feedback via the EwE messaging system.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IFeedbackMessage
        Inherits IMessage

        ''' <summary>
        ''' Get or set the reply to this message.
        ''' </summary>
        Property Reply() As eMessageReply

        ''' <summary>
        ''' Get or set the reply style to this message.
        ''' </summary>
        Property ReplyStyle() As eMessageReplyStyle

    End Interface

End Namespace
