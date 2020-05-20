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
    Implements IMessage

#Region " Private variables "

    ''' <summary>List of <see cref="cVariableStatus">variables</see> attached
    ''' to the message. These variables are presumed affected by the event described 
    ''' in the message, and will be used to update core contents. User interfaces are
    ''' encouraged to use these variables to provide detailed event feedback.</summary>
    Private m_variables As New List(Of cVariableStatus)

#End Region ' Private variables

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a default <see cref="eMessageImportance.Maintenance">maintenance</see> message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub New()
        Me.Message = ""
        Me.Type = eMessageType.NotSet
        Me.Source = eCoreComponentType.NotSet
        Me.Importance = eMessageImportance.Maintenance
        Me.DataType = eDataTypes.NotSet
        Me.Hyperlink = ""
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
    Sub New(strMessage As String,
            msgType As eMessageType,
            msgSource As eCoreComponentType,
            msgImportance As eMessageImportance,
            Optional msgDataType As eDataTypes = eDataTypes.NotSet,
            Optional hyperlink As String = "")
        Me.Message = strMessage
        Me.Type = msgType
        Me.Source = msgSource
        Me.Importance = msgImportance
        Me.DataType = msgDataType
        Me.Hyperlink = hyperlink
    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' <summary>
    ''' Add a cVariableStatus object to the list of variables that this message applies to.
    ''' </summary>
    ''' <param name="Variable"></param>
    ''' <returns></returns>
    ''' <remarks>This is used when the message object is being created to add variables to the message</remarks>
    Public Function AddVariable(Variable As cVariableStatus) As Boolean

        ' Check for duplicates
        For Each vs As cVariableStatus In Me.m_variables
            If Variable.Equals(vs) Then Return True
        Next
        Me.m_variables.Add(Variable)
        ' JS 06Sep18: Performance boost for messages with large number of variables. Importance is elevated
        ' the moment a message is added; no longer need to iterate of (potentially long) message lists as before
        If (Variable.Importance > Me.Importance) Then Me.Importance = Variable.Importance
        Return True

    End Function

    ''' <summary>
    ''' Returns whether a message has a given variable attached.
    ''' </summary>
    ''' <param name="Variable"></param>
    ''' <returns></returns>
    Public Function HasVariable(Variable As cVariableStatus) As Boolean
        For Each vs As cVariableStatus In Me.Variables
            If (ReferenceEquals(vs.Source, Variable.Source)) And
               (vs.Index = Variable.Index) And
               (vs.Status = Variable.Status) And
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
    Public Function HasVariable(varname As eVarNameFlags) As Boolean
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

    ''' <inheritdocs cref="IMessage.Message"/>
    Public Overridable Property Message() As String Implements IMessage.Message

    ''' <inheritdocs cref="IMessage.Type"/>
    Public Overridable Property Type() As eMessageType Implements IMessage.Type

    ''' <inheritdocs cref="IMessage.Source"/>
    Public Overridable Property Source() As eCoreComponentType Implements IMessage.Source

    ''' <inheritdocs cref="IMessage.Importance"/>
    Public Overridable Property Importance() As eMessageImportance Implements IMessage.Importance

    ''' <inheritdocs cref="IMessage.DataType"/>
    Public Overridable Property DataType() As eDataTypes Implements IMessage.DataType

    ''' <inheritdocs cref="IMessage.Suppressable"/>
    Public Overridable Property Suppressable() As Boolean Implements IMessage.Suppressable

    ''' <inheritdocs cref="IMessage.Suppressable"/>
    Public Overridable Property Suppressed() As Boolean Implements IMessage.Suppressed

    ''' <summary>
    ''' Get/set the hyperlink for this message.
    ''' </summary>
    Public Property Hyperlink() As String

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
    Public Overrides Function Equals(obj As Object) As Boolean
        If (TypeOf obj Is cMessage) Then
            Dim msg As cMessage = DirectCast(obj, cMessage)

            ' Compare main msg properties
            Dim bEquals As Boolean = (msg.DataType = Me.DataType) And (msg.Importance = Me.Importance) And
                                     (msg.Source = Me.Source) And (msg.Type = Me.Type) And (msg.Message = Me.Message)

            ' Return comparison result
            Return bEquals
        Else
            Return MyBase.Equals(obj)
        End If
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetType.ToString() & " " & Me.Message
    End Function

#End Region ' Public access

End Class




