'==============================================================================
'
' $Log: cVariableStatus.vb,v $
' Revision 1.1  2008/09/26 07:30:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/09/24 16:58:40  jeroens
' Nitty-gritty: fixed incorrect constructor param names
'
' Revision 1.16  2008/05/29 22:22:52  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.15  2007/06/20 11:07:27  jeroens
' + Added Equals
'
' Revision 1.14  2007/06/12 15:24:04  jeroens
' - Removed unnecessary ByRefs
'
' Revision 1.13  2006/12/16 02:02:06  jeroens
' Commented
'
' Revision 1.12  2006/11/23 04:25:55  jeroens
' + Added option to constructor to set arrayindex
'
' Revision 1.11  2006/07/20 14:09:59  joeb
' Validation using MetaData and Operator classes.
'
' Revision 1.10  2006/07/04 04:42:49  jeroens
' * Changed cValue iGroup references to iIndex; renamed iGroup property to Index
'
' Revision 1.9  2006/06/28 13:59:28  jeroens
' * Renamed iGroup member vars, properties to Index
' * Renamed GroupName vartype and usage to Name where applicable
' * Merged usage of varName Name (fleet) with GroupName
'
' Revision 1.8  2006/06/21 02:57:30  jeroens
' Added pretty header ;)
' Fixed VarName vs. VarType confusion. VarType is actually a deprecated VB# Var(iant)Type declarator...
'
'==============================================================================

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

    ''' <summary>Name of the Variable this Status or Message applies to </summary>
    Public VarName As eVarNameFlags

    ''' <summary>
    ''' The Data structure/class this variable belongs to 
    ''' </summary>
    ''' <remarks>I.e Inputs for EcoPath are eDataTypes.EcoPathInputs</remarks>
    Public DataType As eDataTypes

    ''' <summary>Status of this variable </summary>
    Public Status As eStatusFlags

    ''' <summary>Descriptive message</summary>
    Public Message As String

    ''' <summary>Source of the message. I.e. EcoPath, EcoSim...</summary>
    Public Source As eMessageSource

    ''' <summary>Index of the item in its containing list (was iGroup)</summary>
    Public Index As Integer

    ''' <summary>
    ''' Index to the array element for this variable i.e. DietComp(iArrayIndex)
    ''' </summary>
    Public iArrayIndex As Integer

    ''' <summary>
    ''' Reference to the <see cref="ICoreInterface">ICoreInterface</see> data object that holds this variable
    ''' </summary>
    Public CoreDataObject As ICoreInterface

    ''' <summary>
    ''' Reference to the secundary <see cref="ICoreInterface">ICoreInterface</see> data object that represents
    ''' the index on an indexed variable.
    ''' </summary>
    Public CoreDataObjectSecundary As ICoreInterface

    Sub New()
        VarName = eVarNameFlags.NotSet
        DataType = eDataTypes.NotSet
        Status = eStatusFlags.Null
        Message = ""
        Source = eMessageSource.NotSet
        Index = cCore.NULL_VALUE
        iArrayIndex = cCore.NULL_VALUE
        CoreDataObject = Nothing
        CoreDataObjectSecundary = Nothing
    End Sub

    ''' <summary>
    ''' Copy constructor
    ''' </summary>
    ''' <param name="SourceStatusObject">cVariableStatus instance to copy</param>
    ''' <remarks></remarks>
    Sub New(ByRef SourceStatusObject As cVariableStatus)

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
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable ID</see> that this status applies to.</param>
    ''' <param name="TypeOfData"><see cref="eDataTypes">Datatype ID</see> of the variable.</param>
    ''' <param name="MessageSource"><see cref="eMessageSource">EwE component ID</see> that sent this variable belongs to.</param>
    ''' <param name="iIndex">Index of the <paramref name="MessageSource">EwE component instance</paramref> that this variable belongs to.</param>
    ''' <param name="iArrayIndex">Secundary ID, or <see cref="cCore.NULL_VALUE">CORE NULL</see> if not applicable.</param>
    Sub New(ByVal StatusFlag As eStatusFlags, ByVal MessageStr As String, ByVal VarName As eVarNameFlags, _
            ByVal TypeOfData As eDataTypes, ByVal MessageSource As eMessageSource, ByVal iIndex As Integer, Optional ByVal iArrayIndex As Integer = cCore.NULL_VALUE)

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
    ''' Create and Initialize a new instance
    ''' </summary>
    ''' <param name="ParentCoreDataObject"></param>
    ''' <param name="StatusFlag">Status to set.</param>
    ''' <param name="MessageStr">Message to accompany this variable status.</param>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable ID</see> that this status applies to.</param>
    ''' <param name="TypeOfData"><see cref="eDataTypes">Datatype ID</see> of the variable.</param>
    ''' <param name="MessageSource"><see cref="eMessageSource">EwE component ID</see> that sent this variable belongs to.</param>
    ''' <param name="iIndex">Index of the <paramref name="MessageSource">EwE component instance</paramref> that this variable belongs to.</param>
    ''' <param name="iArrayIndex">Secundary ID, or <see cref="cCore.NULL_VALUE">CORE NULL</see> if not applicable.</param>
    Sub New(ByVal ParentCoreDataObject As ICoreInterface, ByVal StatusFlag As eStatusFlags, ByVal MessageStr As String, ByVal VarName As eVarNameFlags, _
            ByVal TypeOfData As eDataTypes, ByVal MessageSource As eMessageSource, ByVal iIndex As Integer, ByVal iArrayIndex As Integer)

        Me.VarName = VarName
        Me.Status = StatusFlag
        Me.Message = MessageStr
        Me.Source = MessageSource
        Me.DataType = TypeOfData
        Me.Index = iIndex
        Me.CoreDataObject = ParentCoreDataObject
        Me.iArrayIndex = iArrayIndex

    End Sub

    ''' <summary>
    ''' Copy the public contents of a cValue object into this object
    ''' </summary>
    ''' <param name="ValueObject">cValue object to copy</param>
    ''' <remarks></remarks>
    Public Sub Copy(ByVal ValueObject As EwECore.ValueWrapper.cValue)

        Me.VarName = ValueObject.varName
        Me.Status = ValueObject.ValidationStatus
        Me.Message = ValueObject.ValidationMessage
        Me.Index = ValueObject.Index

    End Sub

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If Not TypeOf (obj) Is cVariableStatus Then Return False

        Dim vsCompare As cVariableStatus = DirectCast(obj, cVariableStatus)

        Return (Me.Source = vsCompare.Source) And (Me.Status = vsCompare.Status) And _
               (Me.VarName = vsCompare.VarName) And (Me.Index = vsCompare.Index) And (Me.iArrayIndex = vsCompare.iArrayIndex) And _
               (String.Compare(Me.Message, vsCompare.Message) = 0)

    End Function

End Class