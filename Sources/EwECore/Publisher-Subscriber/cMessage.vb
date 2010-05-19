Option Strict On
Imports EwEUtils.Core

''' <summary>
''' This class encapsulates a message that is passed from the Core to an Interface via the cMessagePublisher-cMessageHandler system
''' </summary>
''' <remarks>
''' A message object is created by the Core and passed to cMessagePublisher.SendMessage(cMessage) 
''' where it is handled by which ever cMessageHandler object can handle this type of message.
''' A message object can contain a list of variables that relate to the message 
''' i.e. If cMessage.Type = eMessageType.EE  then cMessage.Variables will contain a list of cVariableStatus objects that represent variables that have an EE > 1.
''' </remarks>
Public Class cMessage

    ''' <summary>
    ''' A string describing the message
    ''' </summary>
    Private m_strMessage As String

    ''' <summary>
    ''' Message <see cref="eMessageType">Type</see> indicates
    ''' </summary>
    Private m_type As eMessageType

    ''' <summary>
    ''' Enumerated type discribing the <see cref="eCoreComponentType">Source</see> of the message,
    ''' indicating what part of the EwE core a message originated from.
    ''' </summary>
    Private m_source As eCoreComponentType

    ''' <summary>
    ''' Message <see cref="eMessageImportance">Importance</see> indicates the impact 
    ''' that the event, discribed in the message, has on the workings of the EwE Core.
    ''' </summary>
    Private m_importance As eMessageImportance

    ''' <summary>
    ''' Group of <see cref="eDataTypes">data types</see> that this message has affected.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_dataType As eDataTypes

    ''' <summary>
    ''' List of <see cref="cVariableStatus">variables</see> that are affected by the event 
    ''' that is described in the message.
    ''' </summary>
    Private m_variables As New List(Of cVariableStatus)

    ''' <summary>
    ''' Flag stating whether this message may be suppressed by the user
    ''' </summary>
    Private m_bSuppressable As Boolean = False

    ''' <summary>
    ''' List of cVariableStatus objects that are associated with this message
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' Not every type of message contains variable information. 
    ''' Check the Variables.Count property to find out if there are variables in this message
    ''' </remarks>
    Public ReadOnly Property Variables() As List(Of cVariableStatus)
        Get
            Return m_variables
        End Get
    End Property

    Sub New()
        Me.Message = ""
        Me.Type = eMessageType.NotSet
        Me.Source = eCoreComponentType.NotSet
        Me.Importance = eMessageImportance.Maintenance
        Me.DataType = eDataTypes.NotSet
    End Sub

    Sub New(ByVal msgStr As String, ByVal msgType As eMessageType, ByVal msgSource As eCoreComponentType, ByVal msgImportance As eMessageImportance, Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.Message = msgStr
        Me.Type = msgType
        Me.Source = msgSource
        Me.Importance = msgImportance
        Me.DataType = msgDataType
    End Sub


    ''' <summary>
    ''' Add a cVariableStatus object to the list of variables that this message applies to.
    ''' </summary>
    ''' <param name="Variable"></param>
    ''' <returns></returns>
    ''' <remarks>This is used when the message object is being created to add variables to the message</remarks>
    Public Function AddVariable(ByVal Variable As cVariableStatus) As Boolean

        If m_variables Is Nothing Then
            m_variables = New List(Of cVariableStatus)
        Else
            ' Check for duplicates
            For Each vs As cVariableStatus In m_variables
                If Variable.Equals(vs) Then Return True
            Next
        End If

        m_variables.Add(Variable)
        Return True
    End Function

    ''' <summary>
    ''' Returns whether a message has a given variable attached.
    ''' </summary>
    ''' <param name="Variable"></param>
    ''' <returns></returns>
    Public Function HasVariable(ByVal Variable As cVariableStatus) As Boolean
        For Each vs As cVariableStatus In Me.Variables
            If (ReferenceEquals(vs.Source, Variable.Source)) And _
               (vs.Index = Variable.Index) And _
               (vs.Status = Variable.Status) And _
               String.Compare(vs.Message, Variable.Message, True) = 0 Then
                Return True
            End If
        Next
        Return False
    End Function


    ''' <summary>
    ''' Get or set message Text.
    ''' </summary>
    Public Property Message() As String
        Get
            Return Me.m_strMessage
        End Get
        Set(ByVal strMessage As String)
            Me.m_strMessage = strMessage
        End Set
    End Property

    Public Property Type() As eMessageType
        Get
            Return Me.m_type
        End Get
        Set(ByVal value As eMessageType)
            Me.m_type = value
        End Set
    End Property

    Public Property Source() As eCoreComponentType
        Get
            Return Me.m_source
        End Get
        Set(ByVal value As eCoreComponentType)
            Me.m_source = value
        End Set
    End Property

    Public Property Importance() As eMessageImportance
        Get
            Return Me.m_importance
        End Get
        Set(ByVal value As eMessageImportance)
            Me.m_importance = value
        End Set
    End Property

    Public Property DataType() As eDataTypes
        Get
            Return Me.m_dataType
        End Get
        Set(ByVal value As eDataTypes)
            Me.m_dataType = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set whether an interface may suppress repeated instances of a message.
    ''' </summary>
    Public Property Suppressable() As Boolean
        Get
            Return Me.m_bSuppressable
        End Get
        Set(ByVal value As Boolean)
            Me.m_bSuppressable = value
        End Set
    End Property

    ''' <summary>
    ''' Helper method, compares this message to another object
    ''' </summary>
    ''' <param name="obj">The object to compare to</param>
    ''' <returns>True if equals</returns>
    ''' <remarks>
    ''' Two messages are considered equal if main fields <see cref="DataType">DataType</see>,
    ''' <see cref="Importance">Importance</see>, <see cref="Source">Source</see>,
    ''' <see cref="cMessage.Type">Type</see> and <see cref="Message">Message</see> have
    ''' equal values, AND neither message contain attached <see cref="Variables">Variables</see>.
    ''' </remarks>
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is cMessage Then
            Dim msg As cMessage = DirectCast(obj, cMessage)

            ' Compare main msg properties
            Dim bEquals As Boolean = (msg.DataType = Me.DataType) And (msg.Importance = Me.Importance) And _
                   (msg.Source = Me.Source) And (msg.Type = Me.Type) And (msg.Message = Me.Message)

            ' Return comparison result
            Return bEquals
        Else
            Return MyBase.Equals(obj)
        End If
    End Function
End Class




