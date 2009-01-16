'==============================================================================
'
' $Log: cProgressMessage.vb,v $
' Revision 1.2  2009/01/16 18:30:29  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/06/06 15:56:06  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.2  2007/04/23 18:37:18  joeb
' Added State and Max to Progress messages
'
' Revision 1.1  2006/08/15 22:25:02  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

Public Class cProgressMessage
    Inherits cMessage

    Private m_sProgress As Single
    'jb added state to identify what state the process is in
    Private m_state As eProgressState
    Private m_max As Single

    Sub New(ByVal sProgress As Single, ByVal msgStr As String, ByVal msgType As eMessageType, ByVal msgSource As eCoreComponentType, Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.m_sProgress = sProgress
        Me.Message = msgStr
        Me.Type = msgType
        Me.Source = msgSource
        Me.DataType = msgDataType
        Me.Importance = eMessageImportance.Progress
    End Sub


    Sub New(ByVal State As eProgressState, ByVal MaxValue As Single, ByVal sProgress As Single, ByVal msgStr As String, ByVal msgType As eMessageType, _
                    ByVal msgSource As eCoreComponentType, Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.New(sProgress, msgStr, msgType, msgSource, msgDataType)

        Me.m_state = State
        Me.m_max = MaxValue

    End Sub


    Public Property Progress() As Single
        Get
            Return Me.m_sProgress
        End Get
        Set(ByVal value As Single)
            m_sProgress = value
        End Set
    End Property

    ''' <summary>
    ''' State of the process
    ''' </summary>
    ''' <remarks>The State can be Start, Running or Finished</remarks>
    Public Property ProgressState() As eProgressState
        Get
            Return Me.m_state
        End Get
        Set(ByVal value As eProgressState)
            m_state = value
        End Set
    End Property

    Public Property Max() As Single
        Get
            Return Me.m_max
        End Get
        Set(ByVal value As Single)
            m_max = value
        End Set
    End Property


End Class
