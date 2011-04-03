Option Strict On
Imports EwEUtils.Core

Public Class cProgressMessage
    Inherits cMessage

    Private m_sProgress As Single
    'jb added state to identify what state the process is in
    Private m_state As eProgressState
    Private m_max As Single

    Sub New(ByVal sProgress As Single, ByVal msgStr As String, ByVal msgType As eMessageType, Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.m_sProgress = sProgress
        Me.Message = msgStr
        Me.Type = msgType
        Me.Source = eCoreComponentType.External
        Me.DataType = msgDataType
        Me.Importance = eMessageImportance.Progress
    End Sub


    Sub New(ByVal State As eProgressState, ByVal MaxValue As Single, ByVal sProgress As Single, ByVal msgStr As String, ByVal msgType As eMessageType, _
            Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.New(sProgress, msgStr, msgType, msgDataType)

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
