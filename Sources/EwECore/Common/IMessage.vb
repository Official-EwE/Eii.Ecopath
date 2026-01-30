' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for broadcasting messages via the EwE messaging system.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IMessage

        ''' <summary>
        ''' Get/set the text of the message.
        ''' </summary>
        Property Message As String

        ''' <summary>
        ''' Get/set the <see cref="eMessageType">event type</see> of the message.
        ''' </summary>
        Property Type() As eMessageType

        ''' <summary>
        ''' Get/set the <see cref="eCoreComponentType">source witin EwE</see> that
        ''' the message originates from.
        ''' </summary>
        Property Source() As eCoreComponentType

        ''' <summary>
        ''' Get/set the <see cref="eMessageImportance">importance</see> of the message.
        ''' </summary>
        Property Importance() As eMessageImportance

        ''' <summary>
        ''' Get/set the <see cref="eDataTypes">core objects</see> that the message describes.
        ''' </summary>
        Property DataType() As eDataTypes

        ''' <summary>
        ''' Get/set whether an user interface may suppress repeated instances of a message.
        ''' </summary>
        Property Suppressable() As Boolean

        ''' <summary>
        ''' Get/set whether a message was suppressed.
        ''' </summary>
        Property Suppressed() As Boolean

    End Interface

End Namespace
