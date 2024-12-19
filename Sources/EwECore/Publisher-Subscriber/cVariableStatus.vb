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
''' Status or Message that applies to a variable (VarType) for a Group (iGroup)
''' This is used by the message passing system to pass the status of a variable without passing the variable itself
''' </summary>
''' <remarks>
''' Used by Wrapper class for data validation messages see cEcoPathGroupInputs.CurrentStatus(). 
''' Used by the Core messaging system to pass out state or error information of a variable. See cMessage class.
''' </remarks>
''' <history>
''' <revision>jb 14/march/06: Added ICoreInputOutput reference. This references the parent Core data object that holds that variable. </revision>
''' <revision>jb 15/march/06: Added iArrayIndex this is the array index for this variable. It will equal cCore.NULL_VALUE if it is not used. </revision>
''' <revision>jb 17/march/06: Update iArrayIndex in Copy constructor. Added iArrayIndex to constructor </revision>
'''</history>
Public Class cVariableStatus

    Sub New()
        Me.VarName = eVarNameFlags.NotSet
        Me.DataType = eDataTypes.NotSet
        Me.Status = eStatusFlags.Null
        Me.Message = ""
        Me.Source = eCoreComponentType.NotSet
        Me.Index = cCore.NULL_VALUE
        Me.iArrayIndex = cCore.NULL_VALUE
        Me.CoreDataObject = Nothing
        Me.CoreDataObjectSecundary = Nothing
    End Sub

    ''' <summary>
    ''' Copy constructor
    ''' </summary>
    ''' <param name="SourceStatusObject">cVariableStatus instance to copy</param>
    Sub New(SourceStatusObject As cVariableStatus)

        Debug.Assert(Not SourceStatusObject Is Nothing, Me.ToString & ".New(cVariableStatus) Null cVariableStatus passed in.")

        Me.VarName = SourceStatusObject.VarName
        Me.Status = SourceStatusObject.Status
        Me.Message = SourceStatusObject.Message
        Me.Source = SourceStatusObject.Source
        Me.DataType = SourceStatusObject.DataType
        Me.Index = SourceStatusObject.Index
        Me.CoreDataObject = SourceStatusObject.CoreDataObject
        Me.CoreDataObjectSecundary = SourceStatusObject.CoreDataObjectSecundary
        Me.iArrayIndex = SourceStatusObject.iArrayIndex

    End Sub

    ''' <summary>
    ''' Create and Initialize a new instance
    ''' </summary>
    ''' <param name="StatusFlag">Status to set.</param>
    ''' <param name="MessageStr">Message to accompany this variable status.</param>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> that this status applies to.</param>
    ''' <param name="TypeOfData"><see cref="eDataTypes">Datatype</see> of the variable.</param>
    ''' <param name="MessageSource"><see cref="eCoreComponentType">EwE component</see> that sent this variable belongs to.</param>
    ''' <param name="iIndex">Index of the <paramref name="MessageSource">EwE component </paramref> that this variable belongs to.</param>
    ''' <param name="iArrayIndex">Secundary index within <paramref name="VarName"/>, or <see cref="cCore.NULL_VALUE">CORE NULL</see> if not applicable.</param>
    Sub New(StatusFlag As eStatusFlags,
            MessageStr As String,
            VarName As eVarNameFlags,
            TypeOfData As eDataTypes,
            MessageSource As eCoreComponentType,
            iIndex As Integer,
            Optional iArrayIndex As Integer = cCore.NULL_VALUE)

        Me.VarName = VarName
        Me.Status = StatusFlag
        Me.Message = MessageStr
        Me.Source = MessageSource
        Me.DataType = TypeOfData
        Me.Index = iIndex
        Me.CoreDataObject = Nothing
        Me.CoreDataObjectSecundary = Nothing
        Me.iArrayIndex = iArrayIndex

    End Sub

    ''' <summary>
    ''' Create and Initialize a new instance.
    ''' </summary>
    ''' <param name="ParentCoreDataObject"></param>
    ''' <param name="StatusFlag">Status to set.</param>
    ''' <param name="MessageStr">Message to accompany this variable status.</param>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> that this status applies to.</param>
    ''' <param name="TypeOfData"><see cref="eDataTypes">Datatype</see> of the variable.</param>
    ''' <param name="MessageSource"><see cref="eCoreComponentType">EwE component</see> that sent this variable belongs to.</param>
    ''' <param name="iIndex">Index of the <paramref name="MessageSource">EwE component </paramref> that this variable belongs to.</param>
    ''' <param name="iArrayIndex">Secundary index within <paramref name="VarName"/>, or <see cref="cCore.NULL_VALUE">CORE NULL</see> if not applicable.</param>
    Sub New(ParentCoreDataObject As ICoreInterface,
            StatusFlag As eStatusFlags,
            MessageStr As String,
            VarName As eVarNameFlags,
            TypeOfData As eDataTypes,
            MessageSource As eCoreComponentType,
            iIndex As Integer,
            Optional iArrayIndex As Integer = cCore.NULL_VALUE)

        Me.VarName = VarName
        Me.Status = StatusFlag
        Me.Message = MessageStr
        Me.Source = ParentCoreDataObject.CoreComponent
        Me.DataType = ParentCoreDataObject.DataType
        Me.Index = ParentCoreDataObject.Index
        Me.CoreDataObject = ParentCoreDataObject
        Me.iArrayIndex = iArrayIndex

    End Sub

    ''' <summary>
    ''' Create and Initialize a new instance.
    ''' </summary>
    ''' <param name="ParentCoreDataObject"></param>
    ''' <param name="StatusFlag">Status to set.</param>
    ''' <param name="MessageStr">Message to accompany this variable status.</param>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> that this status applies to.</param>
    ''' <param name="iArrayIndex">Secundary index within <paramref name="VarName"/>, or <see cref="cCore.NULL_VALUE">CORE NULL</see> if not applicable.</param>
    Sub New(ParentCoreDataObject As ICoreInterface,
            StatusFlag As eStatusFlags, MessageStr As String, VarName As eVarNameFlags,
            Optional iArrayIndex As Integer = cCore.NULL_VALUE)

        Me.VarName = VarName
        Me.Status = StatusFlag
        Me.Message = MessageStr
        If (ParentCoreDataObject IsNot Nothing) Then
            Me.Source = ParentCoreDataObject.CoreComponent
            Me.DataType = ParentCoreDataObject.DataType
            Me.Index = ParentCoreDataObject.Index
        End If
        Me.CoreDataObject = ParentCoreDataObject
        Me.iArrayIndex = iArrayIndex

    End Sub

    ''' <summary>
    ''' Copy the public contents of a cValue object into this object
    ''' </summary>
    ''' <param name="ValueObject">cValue object to copy</param>
    ''' <remarks></remarks>
    Public Sub Copy(ValueObject As EwECore.ValueWrapper.cValue)

        Me.VarName = ValueObject.varName
        Me.Status = ValueObject.ValidationStatus
        Me.Message = ValueObject.ValidationMessage
        Me.Index = ValueObject.Index

    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf (obj) Is cVariableStatus Then Return False

        Dim vsCompare As cVariableStatus = DirectCast(obj, cVariableStatus)

        ' Text comparison is too slow
        Return (Me.Source = vsCompare.Source) And (Me.Status = vsCompare.Status) And
               (Me.VarName = vsCompare.VarName) And (Me.Index = vsCompare.Index) And (Me.iArrayIndex = vsCompare.iArrayIndex)

    End Function

#Region " Public properties "

    ''' <summary>Name of the Variable this Status or Message applies to </summary>
    Public Property VarName As eVarNameFlags

    ''' <summary>
    ''' The Data structure/class this variable belongs to 
    ''' </summary>
    ''' <remarks>I.e Inputs for EcoPath are eDataTypes.EcoPathInputs</remarks>
    Public Property DataType As eDataTypes

    ''' <summary>Status of this variable </summary>
    Public Property Status As eStatusFlags

    ''' <summary>
    ''' Infer a <see cref="eMessageImportance"/> value from the message <see cref="Status"/>.
    ''' </summary>
    Public ReadOnly Property Importance As eMessageImportance
        Get
            If ((Me.Status And eStatusFlags.ErrorEncountered) > 0) Then Return eMessageImportance.Critical
            If ((Me.Status And (eStatusFlags.FailedValidation Or eStatusFlags.MissingParameter Or eStatusFlags.MissingParameter)) > 0) Then Return eMessageImportance.Warning
            Return eMessageImportance.Maintenance
        End Get
    End Property

    ''' <summary>Descriptive message</summary>
    Public Property Message As String

    ''' <summary>Source of the message. I.e. EcoPath, EcoSim...</summary>
    Public Property Source As eCoreComponentType

    ''' <summary>Index of the item in its containing list (was iGroup)</summary>
    Public Property Index As Integer

    ''' <summary>
    ''' Index to the array element for this variable i.e. DietComp(iArrayIndex)
    ''' </summary>
    Public Property iArrayIndex As Integer

    ''' <summary>
    ''' Reference to the <see cref="ICoreInterface">ICoreInterface</see> data object that holds this variable
    ''' </summary>
    Public Property CoreDataObject As ICoreInterface

    ''' <summary>
    ''' Reference to the secundary <see cref="ICoreInterface">ICoreInterface</see> data object that represents
    ''' the index on an indexed variable.
    ''' </summary>
    Public Property CoreDataObjectSecundary As ICoreInterface

#End Region ' Public properties

End Class