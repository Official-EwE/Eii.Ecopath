' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cProgressMessage
    Inherits cMessage

    Private m_sProgress As Single = 0.0!
    'jb added state to identify what state the process is in
    Private m_state As eProgressState = eProgressState.Start

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new progress message.
    ''' </summary>
    ''' <param name="state"><see cref="eProgressState">State</see> of a process.</param>
    ''' <param name="sMaxValue">Maximum progress scale.</param>
    ''' <param name="sProgress">Current progress value [0, <paramref name="sMaxValue"/>], 
    ''' which will be scaled to <paramref name="sMaxValue"/>.</param>
    ''' <param name="strMessage">Message text.</param>
    ''' <param name="msgType">Optional <see cref="eMessageType">type</see> of the message.</param>
    ''' <param name="msgDataType">Optional <see cref="eDataTypes">data type</see> associated with the message.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(state As eProgressState,
                   sMaxValue As Single, sProgress As Single,
                   strMessage As String,
                   Optional msgType As eMessageType = eMessageType.Progress,
                   Optional msgDataType As eDataTypes = eDataTypes.NotSet)

        If (sMaxValue = 0) Then sMaxValue = 1

        Me.m_state = state
        ' Scale progress
        Me.m_sProgress = sProgress / sMaxValue
        Me.Message = strMessage
        Me.Type = msgType
        Me.DataType = msgDataType
        Me.Importance = eMessageImportance.Progress
        Me.Source = eCoreComponentType.External

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the progress [0, 1] of the operation that this message
    ''' reports on.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Progress() As Single
        Get
            Return Me.m_sProgress
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="eProgressState">state</see> of the operation that this
    ''' message reports on.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ProgressState() As eProgressState
        Get
            Return Me.m_state
        End Get
    End Property

End Class
