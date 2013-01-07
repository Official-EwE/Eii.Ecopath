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

#Region " Private variables "

    ''' <summary>A string describing the message.</summary>
    Private m_strMessage As String = ""

    ''' <summary>The <see cref="eMessageType">type</see> of the message, which 
    ''' encodes the internal event that a message pertains to.</summary>
    Private m_type As eMessageType = eMessageType.NotSet

    ''' <summary>The <see cref="eCoreComponentType">source within EwE</see> 
    ''' that the message originated from.</summary>
    Private m_source As eCoreComponentType = eCoreComponentType.NotSet

    ''' <summary>The <see cref="eMessageImportance">importance</see> of the 
    ''' message.</summary>
    Private m_importance As eMessageImportance = eMessageImportance.Maintenance

    ''' <summary>The <see cref="eDataTypes">type of object</see> that this 
    ''' message has affected. Use this flag with care; the EwE core will interpret
    ''' this flag as change notifications of <see cref="ICoreInterface"/> instances
    ''' that need further processing.</summary>
    Private m_dataType As eDataTypes = eDataTypes.NotSet

    ''' <summary>List of <see cref="cVariableStatus">variables</see> attached
    ''' to the message. These variables are presumed affected by the event described 
    ''' in the message, and will be used to update core contents. User interfaces are
    ''' encouraged to use these variables to provide detailed event feedback.</summary>
    Private m_variables As New List(Of cVariableStatus)

    ''' <summary>Flag stating whether this message may be suppressed by the user.</summary>
    Private m_bSuppressable As Boolean = False

    ''' <summary>Hyperlink that may accompany the message.</summary>
    Private m_strHyperlink As String = ""

#End Region ' Private variables

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a default <see cref="eMessageImportance.Maintenance">maintenance</see> message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub New()
        Me.m_strMessage = ""
        Me.m_type = eMessageType.NotSet
        Me.m_source = eCoreComponentType.NotSet
        Me.m_importance = eMessageImportance.Maintenance
        Me.m_dataType = eDataTypes.NotSet
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a message.
    ''' </summary>
    ''' <param name="strMessage">The message <see cref="Message">text</see>.</param>
    ''' <param name="msgType">The <see cref="Type"/> of the message.</param>
    ''' <param name="msgSource">The <see cref="Source"/> of the message.</param>
    ''' <param name="msgImportance">The <see cref="Importance"/> of the message.</param>
    ''' <param name="msgDataType">The <see cref="DataType"/> of the message.</param>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal strMessage As String, _
            ByVal msgType As eMessageType, _
            ByVal msgSource As eCoreComponentType, _
            ByVal msgImportance As eMessageImportance, _
            Optional ByVal msgDataType As eDataTypes = eDataTypes.NotSet)
        Me.Message = strMessage
        Me.Type = msgType
        Me.Source = msgSource
        Me.Importance = msgImportance
        Me.DataType = msgDataType
    End Sub

#End Region ' Constructor

#Region " Public access "

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
    ''' Returns whether a message has a given variable attached.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' <returns></returns>
    Public Function HasVariable(ByVal varname As eVarNameFlags) As Boolean
        For Each vs As cVariableStatus In Me.m_variables
            If (vs.VarName = varname) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>Get the <see cref="cVariableStatus">variables</see> associated with 
    ''' this message.</summary>
    ''' <remarks>
    ''' Not every type of message contains variable information. Check the Variables.Count 
    ''' property to find out if there are variables in this message
    ''' </remarks>
    Public ReadOnly Property Variables() As List(Of cVariableStatus)
        Get
            Return m_variables
        End Get
    End Property

    ''' <summary>
    ''' Get/set the text of the message.
    ''' </summary>
    Public Property Message() As String
        Get
            Return Me.m_strMessage
        End Get
        Set(ByVal strMessage As String)
            Me.m_strMessage = strMessage
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eMessageType">event type</see> of the message.
    ''' </summary>
    Public Property Type() As eMessageType
        Get
            Return Me.m_type
        End Get
        Set(ByVal value As eMessageType)
            Me.m_type = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eCoreComponentType">source witin EwE</see> that
    ''' the message originates from.
    ''' </summary>
    Public Property Source() As eCoreComponentType
        Get
            Return Me.m_source
        End Get
        Set(ByVal value As eCoreComponentType)
            Me.m_source = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eMessageImportance">importance</see> of the message.
    ''' </summary>
    Public Property Importance() As eMessageImportance
        Get
            Return Me.m_importance
        End Get
        Set(ByVal value As eMessageImportance)
            Me.m_importance = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eDataTypes">core objects</see> that the message
    ''' describes.
    ''' </summary>
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
    ''' Get/set the hyperlink for this message.
    ''' </summary>
    Public Property Hyperlink() As String
        Get
            Return Me.m_strHyperlink
        End Get
        Set(ByVal value As String)
            Me.m_strHyperlink = value
        End Set
    End Property

    ''' <summary>
    ''' Helper method, compares this message to another object.
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
        If (TypeOf obj Is cMessage) Then
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

    Public Overrides Function ToString() As String
        Return Me.GetType.ToString() & " " & Me.m_strMessage
    End Function

#End Region ' Public access

End Class




