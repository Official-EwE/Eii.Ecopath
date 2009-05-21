'==============================================================================
'
' $Log: cMessageStateHandler.vb,v $
' Revision 1.4  2009/05/21 18:53:37  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:31  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:58:24  jeroens
' no message
'
' Revision 1.1  2008/07/23 21:18:18  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

'''============================================================================
''' <summary>
''' Helper class, manages the suppressed state and auto-replies for core messages.
''' </summary>
'''============================================================================
Public Class cMessageStateHandler

#Region " Private helper classes "

    '''========================================================================
    ''' <summary>
    ''' Helper class, maintains suppressed state and auto-replies for a category
    ''' of messages.
    ''' </summary>
    '''========================================================================
    Private Class cMessageStateCache

#Region " Private variables "

        ''' <summary>List of suppressed messages</summary>
        Private m_lSuppressedMessageTypes As List(Of eMessageType) = Nothing
        ''' <summary>Dictionary of auto-replies</summary>
        Private m_dictAutoReplies As Dictionary(Of eMessageType, DialogResult) = Nothing

#End Region ' Private variables

#Region " Construction "

        Public Sub New()
            Me.m_lSuppressedMessageTypes = New List(Of eMessageType)
            Me.m_dictAutoReplies = New Dictionary(Of eMessageType, DialogResult)
        End Sub

#End Region ' Construction

#Region " Public bits "

        Public Property AutoReply(ByVal mt As eMessageType) As DialogResult
            Get
                If Me.m_dictAutoReplies.ContainsKey(mt) Then
                    Return Me.m_dictAutoReplies(mt)
                End If
                Return Windows.Forms.DialogResult.None
            End Get
            Set(ByVal value As DialogResult)
                If Me.m_dictAutoReplies.ContainsKey(mt) Then
                    Me.m_dictAutoReplies.Remove(mt)
                End If
                Me.m_dictAutoReplies(mt) = value
            End Set
        End Property

        Public Property Suppress(ByVal mt As eMessageType) As Boolean
            Get
                Return (Me.m_lSuppressedMessageTypes.IndexOf(mt) > -1)
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Me.m_lSuppressedMessageTypes.Add(mt)
                Else
                    Me.m_lSuppressedMessageTypes.Remove(mt)
                End If
            End Set
        End Property

        Public Sub Clear()
            Me.m_lSuppressedMessageTypes.Clear()
            Me.m_dictAutoReplies.Clear()
        End Sub

#End Region ' Public bits

    End Class

#End Region ' Private helper classes

#Region " Private variables "

    Private m_dtMessageState As Dictionary(Of eCoreComponentType, cMessageStateCache)

#End Region ' Private variables

#Region " Construction "

    Public Sub New()
        Me.m_dtMessageState = New Dictionary(Of eCoreComponentType, cMessageStateCache)
    End Sub

#End Region ' Construction

#Region " Public bits "

    Public Property Suppress(ByVal source As eCoreComponentType, ByVal mt As eMessageType) As Boolean
        Get
            Return Me.GetCache(source).Suppress(mt)
        End Get
        Set(ByVal value As Boolean)
            Me.GetCache(source).Suppress(mt) = value
        End Set
    End Property

    Public Property AutoReply(ByVal source As eCoreComponentType, ByVal mt As eMessageType) As DialogResult
        Get
            Return Me.GetCache(source).AutoReply(mt)
        End Get
        Set(ByVal value As DialogResult)
            Me.GetCache(source).AutoReply(mt) = value
        End Set
    End Property

    Public Sub CheckState(ByVal msg As cMessage)

        If (msg.Type = eMessageType.DataAddedOrRemoved Or msg.Type = eMessageType.DataModified) Then
            Select Case msg.Source
                Case eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer
                    Me.Clear(msg.Source)
                Case Else
                    Me.Clear(eCoreComponentType.Core)
            End Select
        End If
    End Sub

    Public Sub Clear(ByVal src As eCoreComponentType)

        Select Case src
            Case eCoreComponentType.Ecotracer
                Me.GetCache(eCoreComponentType.Ecotracer).Clear()

            Case eCoreComponentType.EcoSpace
                Me.GetCache(eCoreComponentType.Ecotracer).Clear()
                Me.GetCache(eCoreComponentType.EcoSpace).Clear()

            Case eCoreComponentType.EcoSim
                Me.GetCache(eCoreComponentType.Ecotracer).Clear()
                Me.GetCache(eCoreComponentType.EcoSpace).Clear()
                Me.GetCache(eCoreComponentType.EcoSim).Clear()

            Case eCoreComponentType.EcoPath
                Me.GetCache(eCoreComponentType.Ecotracer).Clear()
                Me.GetCache(eCoreComponentType.EcoSpace).Clear()
                Me.GetCache(eCoreComponentType.EcoSim).Clear()
                Me.GetCache(eCoreComponentType.EcoPath).Clear()

            Case eCoreComponentType.Core
                Me.GetCache(eCoreComponentType.Ecotracer).Clear()
                Me.GetCache(eCoreComponentType.EcoSpace).Clear()
                Me.GetCache(eCoreComponentType.EcoSim).Clear()
                Me.GetCache(eCoreComponentType.EcoPath).Clear()
                Me.GetCache(eCoreComponentType.Core).Clear()

        End Select

    End Sub

#End Region ' Public bits

#Region " Internals "

    Private Function GetCache(ByVal source As eCoreComponentType) As cMessageStateCache

        Dim c As cMessageStateCache = Nothing

        Select Case source
            Case eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Ecotracer
                ' NOP
            Case Else
                source = eCoreComponentType.Core
        End Select

        If (Me.m_dtMessageState.ContainsKey(source) = False) Then
            c = New cMessageStateCache()
            Me.m_dtMessageState(source) = c
        Else
            c = Me.m_dtMessageState(source)
        End If
        Return c

    End Function


#End Region ' Internals

End Class
