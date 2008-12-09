'==============================================================================
'
' $Log: ICoreInputOutput.vb,v $
' Revision 1.2  2008/12/09 19:44:44  joeb
' Added IResultsWrapper this wraps a core array so it can be used by a CoreInputOutput directly instead of buffering the data
'
' Revision 1.1  2008/09/26 07:30:11  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.65  2008/09/15 16:58:16  joeb
' Added more Ecospace output for Game Server
'
' Revision 1.64  2008/07/16 15:12:03  jeroens
' Added functions for lazy people :p
'
' Revision 1.63  2008/07/02 01:55:22  jeroens
' Added option to force status flag total reset (fixes bug 503)
'
' Revision 1.62  2008/05/29 22:22:48  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.61  2008/01/25 17:44:06  jeroens
' 'Documented' PP
'
' Revision 1.60  2007/10/30 18:41:26  jeroens
' * Description limited to 250 chars
'
' Revision 1.59  2007/08/27 02:23:09  jeroens
' - Disabled list change events; list are not going to be exposed in this EwE   installment
' - Removed m_isMultiStanza buffer
'
' Revision 1.58  2007/08/25 19:24:35  jeroens
' * Auxillary data no longer needs datatype, DBID. StringID is enough
'
' Revision 1.57  2007/08/15 23:38:16  joeb
' Changed wording of an Assert
'
' Revision 1.56  2007/07/11 00:37:39  jeroens
' * Exposed message source
'
' Revision 1.55  2007/07/06 23:51:20  jeroens
' - Disabled *SLOW* debug assert for profiling
'
' Revision 1.54  2007/07/06 21:27:29  jeroens
' + Added list item assertion
'
' Revision 1.53  2007/07/06 20:09:38  jeroens
' * Rewrote base list
'
' Revision 1.52  2007/06/20 01:28:09  jeroens
' + Exposes variable metadata
'
' Revision 1.51  2007/06/04 16:46:42  jeroens
' Some value arrays have 0-based indexes, such as PrefHab. Changes reset status flags start index to 0 to cater to such situations.
'
' Revision 1.50  2007/05/30 02:54:27  jeroens
' + Added two utility methods to set/clear status flags
'
' Revision 1.49  2007/05/29 15:26:41  jeroens
' * Fixed SetVariable potential bug when clearing a value with Null (Nothing)
'
' Revision 1.48  2007/05/20 00:35:04  jeroens
' * Optimized SetVariable: abort when the operation will not change the variable value
'
' Revision 1.47  2007/05/18 01:52:18  jeroens
' + Added XML comments
'
' Revision 1.46  2007/05/04 15:25:35  jeroens
' + MessageSource exposed
'
' Revision 1.45  2007/04/06 17:24:55  joeb
' Change to Friend Overridable Function Resize() As Boolean
'
' Revision 1.44  2007/03/28 01:16:34  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.43  2007/03/27 16:18:32  jeroens
' + Included IntArray in ResetStatusFlags
'
' Revision 1.42  2007/03/07 15:10:15  jeroens
' + Added cCoreInputOutputBaseList
'
' Revision 1.41  2007/01/25 16:44:30  jeroens
' Minor changes
'
' Revision 1.40  2007/01/20 00:27:23  joeb
' Bug Fix
'
' Revision 1.39  2007/01/19 18:29:49  joeb
' Added Array of Boolean to cValueArray
'
' Revision 1.38  2007/01/19 00:47:31  joeb
' Added eValueTypes.PointArray
'
' Revision 1.37  2007/01/17 20:11:28  joeb
' Added Error message to an Assert
'
' Revision 1.36  2007/01/15 14:49:15  jeroens
' * Improved ResetStatusFlags error assessment
'
'==============================================================================
Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#Region " Definition of interfaces "

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for exposing Core data entities.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ICoreInterface

    ''' <summary>Globally unique ID identifying a core data entity.</summary>
    Function GetID() As String
    ''' <summary>Unique ID per type of core data used to distinguish a core data entity in a storage medium. DBID is short for Database ID</summary>
    Property DBID() As Integer
    ''' <summary>A human readable name identifying a core data entity.</summary>
    Property Name() As String
    ''' <summary>The ordinal number in the core storage structures for a core data entity.</summary>
    Property Index() As Integer
    ''' <summary><see cref="eDataTypes">Data type</see> identifying the class of a core data entity.</summary>
    ReadOnly Property DataType() As eDataTypes

End Interface ' ICoreInterface

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for accessing Core input or output objects.
''' </summary>
''' <remarks>
''' This allows all model/scenario input and output entities to be accessed through one interface.
'''</remarks>
''' ---------------------------------------------------------------------------
Public Interface ICoreInputOutput

    ''' <summary>
    ''' Returns the value exposed by a Core input or output object.
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> type to access.</param>
    ''' <param name="iIndex2">Optional index of the value to return when accessing an array-type variable.</param>
    ''' <returns>Any loose-typed value, or Nothing if an error occurred.</returns>
    Function GetVariable(ByVal VarName As eVarNameFlags, Optional ByVal iIndex1 As Integer = cCore.NULL_VALUE, Optional ByVal iIndex2 As Integer = cCore.NULL_VALUE) As Object

    ''' <summary>
    ''' Sets the value of a variable exposed by a Core input or output object.
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> type to access.</param>
    ''' <param name="iIndex">Optional index of the value to set when accessing an array-type variable.</param>
    ''' <returns>True if succesful.</returns>
    Function SetVariable(ByVal VarName As eVarNameFlags, ByVal newValue As Object, Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As Boolean

    ''' <summary>
    ''' Returns the <see cref="eStatusFlags">Status</see> of a value exposed by a Core input or output object.
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> type to access.</param>
    ''' <param name="iIndex">Optional index of the value status to query when accessing an array-type variable.</param>
    ''' <returns>Any loose-typed value, or Nothing if an error occurred.</returns>
    Function GetStatus(ByVal VarName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As eStatusFlags

    ''' <summary>
    ''' Sets the <see cref="eStatusFlags">Status</see> of a value exposed by a Core input or output object.
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Variable</see> type to access.</param>
    ''' <param name="iIndex">Optional index of the value status to set when accessing an array-type variable.</param>
    ''' <returns>Any loose-typed value, or Nothing if an error occurred.</returns>
    Function SetStatus(ByVal VarName As eVarNameFlags, ByVal newStatus As eStatusFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As Boolean

    ''' <summary>
    ''' Returns the <see cref="cVariableStatus">result</see> of the most recent 
    ''' attempt to <see cref="SetVariable">Set a variable</see>.
    ''' </summary>
    ''' <returns>A <see cref="cVariableStatus">cVariableStatus</see> containing 
    ''' the result of the most recent attempt to <see cref="SetVariable">Set</see> 
    ''' a variable.</returns>
    ReadOnly Property ValidationStatus() As cVariableStatus

End Interface ' ICoreInputOutput

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for defining a group.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ICoreGroup

    ''' <summary>
    ''' Get/set whether the group is part of a multi-stanza configuration.
    ''' </summary>
    ReadOnly Property isMultiStanza() As Boolean

    ''' <summary>
    ''' Get/set the <see cref="ICoreInterface.DBID">Database ID</see> of the
    ''' <see cref="cStanzaGroup">Stanza configuration</see> that this group
    ''' belongs to.
    ''' </summary>
    Property StanzaID() As Integer

    ''' <summary>
    ''' The ratio that this group contributes to Primary Production.
    ''' </summary>
    ''' <returns>This method will return one of the following values:
    ''' <list type="bullet">
    ''' <item>0-1 for mixed consumer/producer groups</item>
    ''' <item>1 for primary producers</item>
    ''' <item>2 for detritus groups</item>
    ''' </list>
    ''' </returns>
    ''' <remarks>This can be used as a flag to tell if a group is mixed consumer/producer, primary producer or a detritus group.</remarks>
    Property PP() As Single

    ReadOnly Property IsConsumer() As Boolean
    ReadOnly Property IsProducer() As Boolean
    ReadOnly Property IsDetritus() As Boolean

End Interface ' ICoreGroup

#End Region ' Definition of interfaces 

#Region " cCoreInputOutputBase "

''' ---------------------------------------------------------------------------
''' <summary>
''' Base class implementation of the ICoreInterface, ICoreInputOutput interfaces.
''' </summary>
''' <remarks>
''' <para>This class provides the code that implements the ICoreInputOutput interface.</para>
''' <para>Classes that inherit from this base class need to populate the lookup tables that are
''' used to store the internal data in the New constructor and define a dot (.) operator
''' for any variables that requires to be accessed via Properties.</para>
''' <para>For examples on how to implement this class, refer to <see cref="cEcoPathGroupInput">cEcoPathGroupInput</see>,
''' <see cref="cFleetInput">cFleetInput</see>, etc.</para>
'''</remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cCoreInputOutputBase
    Implements ICoreInterface
    Implements ICoreInputOutput

#Region " Protected variables "

    ''' <summary>
    ''' States whether <see cref="cValue.AllowValidation">Variable validation</see> is enabled for this object.
    ''' </summary>
    ''' <remarks>
    ''' Validation is typically required in response to <see cref="SetVariable">SetVariable</see> 
    ''' calls triggered by user actions. Whenever an object is populated by the 
    ''' <see cref="cCore">EwE Core</see> validation may be temporarily disabled.
    ''' </remarks>
    Protected m_bValidate As Boolean = False

    ''' <summary>
    ''' States whether an object will allow its values to be modified via <see cref="SetVariable">SetVariable</see>.
    ''' </summary>
    Protected Friend m_bReadOnly As Boolean = False

    ''' <summary>
    ''' Container for the <see cref="cCoreInputOutputBase.ValidationStatus">Validation status</see> of the object.
    ''' </summary>
    Protected m_ValidationStatus As cVariableStatus = Nothing

    ''' <summary>
    ''' Container for the <see cref="ICoreInterface.DataType">data type</see> describing the object.
    ''' </summary>
    Protected m_DataType As eDataTypes = eDataTypes.NotSet

    ''' <summary>
    ''' The variables maintained by this object. Implemented as a collection of <see cref="cValue">variable values</see>
    ''' indexed by <see cref="eVarNameFlags">Variable name</see>.
    ''' </summary>
    Friend m_values As New Dictionary(Of eVarNameFlags, cValue)

    ''' <summary>
    ''' The <see cref="eMessageSource">EwE core component</see> that this object belongs to
    ''' </summary>
    ''' <remarks></remarks>
    Protected m_messageSource As eMessageSource = eMessageSource.NotSet

    ''' <summary>
    ''' Reference to the <see cref="cCore">EwE Core</see> that exposes the object.
    ''' </summary>
    Protected m_core As cCore = Nothing

#End Region ' Protected variables

#Region "Constructor and Initialization"

    ''' <summary>
    ''' Create and populate the Lookup tables, as well as <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>-defined variables.
    ''' </summary>
    ''' <remarks>A class the inherits from this base class will need to define its own variables in its constructor</remarks>
    Sub New(ByRef TheCore As cCore)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim name() As Char
        Dim validator As cValidatorDefault

        m_core = TheCore

        m_ValidationStatus = New cVariableStatus()

        'all variable use the default validator
        validator = m_core.m_validators.getValidator(eVarNameFlags.NotSet)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(name), eVarNameFlags.Name, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.Index, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Int, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.DBID, eStatusFlags.Null, eValueTypes.Int, meta, validator)
        m_values.Add(val.varName, val)

    End Sub

    ''' <summary>
    ''' Resize any indexed variables i.e. DietComp to the size of the <see cref="eCoreCounterTypes">core counter</see> that it is dimensioned by.
    ''' </summary>
    Friend Overridable Function Resize() As Boolean
        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue

        For Each keyvalue In m_values
            value = keyvalue.Value
            'only cValueArray objects will actually resize the underlying data
            value.SetSize()
        Next

    End Function

#End Region

#Region "Public Functions/Methods"

    ''' <summary>
    ''' Returns the unique ID for this object
    ''' </summary>
    Public Function getID() As String Implements ICoreInterface.GetID
        ' Return unique ID
        Return cValueID.getDataTypeID(Me.m_DataType, Me.DBID)
    End Function

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return Me.m_DataType
        End Get
    End Property

    Public Property Remark(Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name, Optional ByVal objSec As cCoreInputOutputBase = Nothing) As String
        Get
            Dim strValueID As String = cValueID.Generate(Me, varName, objSec)
            Return Me.m_core.Remark(strValueID)
        End Get
        Set(ByVal strRemark As String)
            Dim strValueID As String = cValueID.Generate(Me, varName, objSec)
            Me.m_core.Remark(strValueID) = strRemark
        End Set
    End Property

    ''' <summary>
    ''' Returns the <see cref="eMessageSource">EwE core component</see> that this object belongs to.
    ''' </summary>
    Public Function MessageSource() As eMessageSource
        Return Me.m_messageSource
    End Function

#End Region

#Region " Mustoverride Methods "

    ''' <summary>
    ''' Public access to set the status flags by calling each validator.
    ''' </summary>
    ''' <returns>True is successful. False otherwise</returns>
    ''' <remarks>This is the default behaviour for Input objects. Output 
    ''' objects will need to provide their own implementation due to the 
    ''' absence of validators.</remarks>
    Friend Overridable Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            If bForceReset Then
                                value.Status(i) = 0
                            Else
                                value.setStatusFlag(i)
                            End If
                        Next i
                    Case Else
                        If bForceReset Then
                            value.Status = 0
                        Else
                            value.setStatusFlag()
                        End If
                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

#End Region ' Mustoverride Methods

#Region " Get/Set Status"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="VarName"></param>
    ''' <param name="iIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function GetStatus(ByVal VarName As eVarNameFlags, Optional ByVal iIndex As Integer = -9999) As eStatusFlags Implements ICoreInputOutput.GetStatus
        Try
            Return m_values.Item(VarName).Status(iIndex)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".getVariable()Error " & ex.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Replaces current status flags for a given variable with a new set of status flags.
    ''' </summary>
    ''' <param name="VarName"></param>
    ''' <param name="newStatus"></param>
    ''' <param name="iIndex"></param>
    ''' <returns>True if succesful.</returns>
    Friend Function SetStatus(ByVal VarName As eVarNameFlags, ByVal newStatus As eStatusFlags, Optional ByVal iIndex As Integer = -9999) As Boolean Implements ICoreInputOutput.SetStatus
        Try
            m_values.Item(VarName).Status(iIndex) = newStatus
            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".setStatus(...) Failed to set Status " & VarName.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Adds a given set of status flags to existing status flags for a given variable.
    ''' </summary>
    ''' <param name="VarName"></param>
    ''' <param name="statusFlags"></param>
    ''' <param name="iIndex"></param>
    ''' <returns>True if succesful.</returns>
    Friend Function SetStatusFlags(ByVal VarName As eVarNameFlags, ByVal statusFlags As eStatusFlags, Optional ByVal iIndex As Integer = -9999) As Boolean
        Try
            m_values.Item(VarName).Status(iIndex) = m_values.Item(VarName).Status(iIndex) Or statusFlags
            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".SetStatusFlags(...) Failed to set status flags " & VarName.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Clears a given set of status flags from existing status flags for a given variable.
    ''' </summary>
    ''' <param name="VarName"></param>
    ''' <param name="statusFlags"></param>
    ''' <param name="iIndex"></param>
    ''' <returns>True if succesful.</returns>
    Friend Function ClearStatusFlags(ByVal VarName As eVarNameFlags, ByVal statusFlags As eStatusFlags, Optional ByVal iIndex As Integer = -9999) As Boolean
        Try
            m_values.Item(VarName).Status(iIndex) = m_values.Item(VarName).Status(iIndex) And (Not statusFlags)
            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ClearStatusFlags(...) Failed to clear status flags " & VarName.ToString)
            Return False
        End Try
    End Function

#End Region

#Region " Get/set variable "

    ''' <summary>
    ''' Return the value of a variable 
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Name</see> of the variable to set.</param>
    ''' <param name="iIndex">Optional index for indexed variables.</param>
    ''' <param name="iIndex2">Optional index for indexed variables.</param>
    ''' <returns></returns>
    ''' <remarks>This only provides variables for one optional index Override this if you you need access to variables with two indexes</remarks>
    Public Overridable Function GetVariable(ByVal VarName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE, Optional ByVal iIndex2 As Integer = cCore.NULL_VALUE) As Object Implements ICoreInputOutput.GetVariable

        Try
            Debug.Assert(iIndex2 = cCore.NULL_VALUE, Me.ToString & ".GetVariable(eVarNameFlags,Option Integer, Optional Integer) Called with optional argument iIndex2 this behavior must be implemented in a derived class.")
            Return m_values.Item(VarName).Value(iIndex)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".getVariable()Error: " & VarName.ToString & " " & ex.Message)
            Return Nothing
        End Try

    End Function
    ''' <summary>
    ''' Set the value of a variable.
    ''' </summary>
    ''' <param name="VarName"><see cref="eVarNameFlags">Name</see> of the variable to set.</param>
    ''' <param name="newValue">Value to set.</param>
    ''' <param name="iSecondaryIndex">Optional index for indexed variables.</param>
    ''' <returns>True if a variable is succesfully changed.</returns>
    ''' <remarks>The outcome of the SetVariable call can be examined via 
    ''' <see cref="cValue.ValidationStatus">cValue.ValidationStatus</see>.</remarks>
    Public Overridable Function SetVariable(ByVal VarName As eVarNameFlags, ByVal newValue As Object, Optional ByVal iSecondaryIndex As Integer = -9999) As Boolean Implements ICoreInputOutput.SetVariable
        Dim bSucces As Boolean = True
        Dim valueobject As cValue

        'get the cValue object for the dictionary
        Try
            valueobject = m_values.Item(VarName)

            ' Optimization: abort when the set operation will not change the variable value.
            If Object.Equals(newValue, valueobject.Value(iSecondaryIndex)) Then
                ' Report that variable has NOT been set.
                Return False
            End If

            'validate the variable by setting its value
            valueobject.Value(iSecondaryIndex) = newValue
            If valueobject.ValidationStatus = eStatusFlags.FailedValidation Then bSucces = False

            If m_bValidate Then

                ' Prepare status
                m_ValidationStatus.Copy(valueobject)
                m_ValidationStatus.iArrayIndex = iSecondaryIndex

                '' Notify core, if provided
                'If (Me.m_core IsNot Nothing) Then
                '    Me.m_core.OnValidated(valueobject, Me)
                'End If

            End If

            If AllowValidation Then
                ' Notify core, if provided
                If (Me.m_core IsNot Nothing) Then
                    Me.m_core.OnValidated(valueobject, Me)
                End If
            End If


        Catch ex As KeyNotFoundException
            'this is most likely a programing error so assert and try to figure out why
            m_ValidationStatus.Status = eStatusFlags.ErrorEncountered
            m_ValidationStatus.Message = Me.ToString & ".setVariable(...) Failed to find variable: " & VarName.ToString
            Debug.Assert(False, Me.ToString & ".setVariable(...) Failed to find variable: " & VarName.ToString)
            bSucces = False

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".setVariable(...) Failed to set variable " & VarName.ToString & " " & ex.Message)
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Get/set variable

#Region " Experimental "

    Public Function GetVariableMetadata(ByVal varName As eVarNameFlags) As cVariableMetaData

        Dim objValue As cValue = Nothing
        Try
            objValue = m_values.Item(varName)
            Return objValue.Metadata
        Catch ex As Exception

        End Try
        Return Nothing
    End Function

#End Region ' Experimental

#Region " Properties by dot(.) operator "

    Friend Property AllowValidation() As Boolean
        Get
            Return m_bValidate
        End Get
        Set(ByVal value As Boolean)

            m_bValidate = value

            'set the do validation flag in all the values
            Dim valueobject As cValue
            For Each keyvalue As KeyValuePair(Of eVarNameFlags, cValue) In m_values
                valueobject = keyvalue.Value
                valueobject.AllowValidation = m_bValidate
            Next

        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return DirectCast(GetVariable(eVarNameFlags.Name), String)
        End Get

        Set(ByVal newValue As String)
            SetVariable(eVarNameFlags.Name, newValue)
        End Set
    End Property

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return DirectCast(GetVariable(eVarNameFlags.Index), Integer)
        End Get

        Set(ByVal newValue As Integer)

            Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
            Dim value As cValue
            For Each keyvalue In m_values
                value = keyvalue.Value
                value.Index = newValue
            Next

            SetVariable(eVarNameFlags.Index, newValue)
        End Set
    End Property

    ''' <summary>
    ''' Returns the persistent unique ID for an ICoreInputOutput.
    ''' </summary>
    ''' <remarks>
    ''' Applicaton layers built on top of the core will probably never need direct 
    ''' access to this property. To abstract its storage methods it seems best to
    ''' restrict access to this property to the Core assembly only.</remarks>
    Friend Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return DirectCast(GetVariable(eVarNameFlags.DBID), Integer)
        End Get
        Set(ByVal newValue As Integer)
            SetVariable(eVarNameFlags.DBID, newValue)
        End Set
    End Property

    Public ReadOnly Property ValidationStatus() As cVariableStatus Implements ICoreInputOutput.ValidationStatus
        Get
            Return m_ValidationStatus
        End Get
    End Property

    ''' <summary>
    ''' Returns the <see cref="cValue">Value descriptor</see> for a given
    ''' variable name, associated with this object.
    ''' </summary>
    ''' <param name="varName"><see cref="eVarNameFlags">Variable name</see>
    ''' to retrieve the value descriptor for.</param>
    Public ReadOnly Property ValueDescriptor(ByVal varName As eVarNameFlags) As cValue
        Get
            If Me.m_values.ContainsKey(varName) Then Return Me.m_values(varName)
            Return Nothing
        End Get
    End Property

#End Region ' Properties by dot(.) operator

End Class ' CoreInputOutputBase

#End Region

#Region " cCoreGroupBase "

Public Class cCoreGroupBase
    Inherits cCoreInputOutputBase
    Implements ICoreGroup

    'Protected m_isMultiStanza As Boolean = False
    Protected m_StanzaID As Integer = 0

    ''' <summary>
    ''' Create and populate the Lookup tables 
    ''' </summary>
    ''' <remarks>A class the inherits from this base class will need to define its own variables in its constructor</remarks>
    Sub New(ByRef core As cCore)
        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim validator As cValidatorDefault

        'all variable use the default validator
        validator = m_core.m_validators.getValidator(eVarNameFlags.NotSet)

        meta = New cVariableMetaData(0, 2, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.PP, eStatusFlags.Null, eValueTypes.Sng, meta, validator)
        m_values.Add(val.varName, val)

    End Sub

    Public ReadOnly Property isMultiStanza() As Boolean Implements ICoreGroup.isMultiStanza
        Get
            'Return m_isMultiStanza
            Return Me.StanzaID <> cCore.NULL_VALUE
        End Get
    End Property

    ''' <summary>
    ''' See <see cref="ePrimaryProductionTypes">ePrimaryProductionTypes</see> for possible values.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PP() As Single Implements ICoreGroup.PP
        Get
            Return DirectCast(GetVariable(eVarNameFlags.PP), Single)
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.PP, value)
        End Set
    End Property

    Public Property StanzaID() As Integer Implements ICoreGroup.StanzaID
        Get
            Return m_StanzaID
        End Get
        Set(ByVal value As Integer)
            m_StanzaID = value
            ''if the stanza id is not NULL then this is a multi stanza group
            'If value <> cCore.NULL_VALUE Then
            '    m_isMultiStanza = True
            'Else
            '    m_isMultiStanza = False
            'End If
        End Set
    End Property

    Public ReadOnly Property IsConsumer() As Boolean Implements ICoreGroup.IsConsumer
        Get
            Return (Me.PP < 1.0)
        End Get
    End Property

    Public ReadOnly Property IsDetritus() As Boolean Implements ICoreGroup.IsDetritus
        Get
            Return (Me.PP = 2.0)
        End Get
    End Property

    Public ReadOnly Property IsProducer() As Boolean Implements ICoreGroup.IsProducer
        Get
            Return (Me.PP > 0 And Me.PP <= 1.0)
        End Get
    End Property

End Class ' cCoreGroupBase

#End Region

#Region " cCoreInputOutputList "

''' ---------------------------------------------------------------------------
''' <summary>
''' Strong-typed list that handles item index offset headaches transparently.
''' </summary>
''' <remarks>
''' JS 27Aug07: list change event functionality is suspended to prevent confusion in different methods on how to use these list.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cCoreInputOutputList(Of T)
    Implements IList(Of T)

#Region " Construction "

    ''' <summary>
    ''' Offset for items in the list.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_iItemOffset As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The <see cref="eDataTypes">data type</see> of objects that this list contains.
    ''' </summary>
    ''' <param name="dt">The <see cref="eDataTypes">data type</see> of objects that this list holds.</param>
    ''' <param name="iItemOffset">The offset for items in this list.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal dt As eDataTypes, ByVal iItemOffset As Integer)
        Me.m_dt = dt
        Me.m_iItemOffset = iItemOffset
    End Sub

#End Region ' Construction

#If 0 Then ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Public event, notifying the world of list changes.
    ''' </summary>
    ''' <param name="list">The list that fired the event.</param>
    ''' -----------------------------------------------------------------------
    Public Event OnListChanged(ByVal list As cCoreInputOutputList(Of T))

    ''' <summary>Event lock flag, stating whether events are allowed to be sent out.</summary>
    ''' <remarks>This flag should be used to suppress events when a list is being configured.</remarks>
    Private m_bAllowEvents As Boolean = True
    ''' <summary>Flag stating whether events have been withheld under an active event lock.</summary>
    Private m_bHasWithheldEvents As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the event lock flag.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property AllowEvents() As Boolean

        Get
            Return Me.m_bAllowEvents
        End Get

        Set(ByVal bAllow As Boolean)
            ' Set the flag
            Me.m_bAllowEvents = bAllow
            ' If an event was withheld, send it now.
            If m_bHasWithheldEvents Then Me.FireEvent()
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fire a change event.
    ''' </summary>
    ''' <remarks>
    ''' If an event lock is active, the withheld event flag is set to make sure
    ''' the event is sent when the lock is released.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub FireEvent()
        ' Send event if no lock active
        If Me.m_bAllowEvents Then RaiseEvent OnListChanged(Me)
        ' Update the withheld flag
        Me.m_bHasWithheldEvents = Not Me.m_bAllowEvents
    End Sub

#End Region ' Events

#End If

#Region " Public properties "

    ''' <summary>My datatype.</summary>
    Private m_dt As eDataTypes = eDataTypes.NotSet

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the <see cref="eDataTypes">data type</see> of the 
    ''' <see cref="cCoreInputOutputBase">core objects</see> that this list contains.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DataType() As eDataTypes
        Get
            Return Me.m_dt
        End Get
    End Property

#End Region ' Public properties

#Region " List interfaces "

    ''' <summary>The actual list.</summary>
    Private m_list As New List(Of T)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Contains">List.Add</see> impementation.
    ''' Restricted access because the content of this list is managed by the EwE Core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Overridable Sub Add(ByVal item As T) _
            Implements System.Collections.Generic.ICollection(Of T).Add
        Me.m_list.Add(item)
        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.FireEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Clear">List.Clear</see> impementation. 
    ''' Restricted access because the content of this list is managed by the EwE Core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub Clear() _
             Implements System.Collections.Generic.ICollection(Of T).Clear
        Me.m_list.Clear()
        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.FireEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Contains">List.Contains</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Contains(ByVal item As T) As Boolean _
             Implements System.Collections.Generic.ICollection(Of T).Contains
        Return Me.m_list.Contains(item)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.CopyTo">List.CopyTo</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub CopyTo(ByVal aItems() As T, ByVal iIndex As Integer) _
            Implements System.Collections.Generic.ICollection(Of T).CopyTo
        Me.m_list.CopyTo(aItems, iIndex - Me.m_iItemOffset)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Count">List.Count</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Count() As Integer _
            Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Return Me.m_list.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.CopyTo">List.IsReadOnly</see> implementation.
    ''' </summary>
    ''' <returns>
    ''' Always true; because the content of this list is managed by the EwE Core.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsReadOnly() As Boolean _
            Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
        Get
            Return True
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Remove">List.Remove</see> impementation. 
    ''' Restricted access because the content of this list is managed by the EwE Core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Overridable Function Remove(ByVal item As T) As Boolean _
             Implements System.Collections.Generic.ICollection(Of T).Remove
        Me.m_list.Remove(item)
        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.FireEvent()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.GetEnumerator">List.GetEnumerator</see> impementation. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) _
             Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
        Return Me.m_list.GetEnumerator()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.IndexOf">List.IndexOf</see> impementation. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IndexOf(ByVal item As T) As Integer _
             Implements System.Collections.Generic.IList(Of T).IndexOf
        Return Me.m_list.IndexOf(item) + Me.m_iItemOffset
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Insert">List.Insert</see> impementation. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Insert(ByVal iIndex As Integer, ByVal item As T) _
             Implements System.Collections.Generic.IList(Of T).Insert
        Me.m_list.Insert(iIndex - Me.m_iItemOffset, item)
        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.FireEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.Item">List.Item</see> impementation. 
    ''' Restricted set access because the content of this list is managed by the EwE Core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Default Public Property Item(ByVal iIndex As Integer) As T _
            Implements System.Collections.Generic.IList(Of T).Item
        Get
            Try
                Return Me.m_list.Item(iIndex - Me.m_iItemOffset)
            Catch ex As Exception
                Debug.Assert(False, "index out of bounds")
                Return Nothing
            End Try
        End Get
        Friend Set(ByVal value As T)
            Me.m_list.Item(iIndex - Me.m_iItemOffset) = value
            ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
            'Me.FireEvent()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed <see cref="List.RemoveAt">List.RemoveAt</see> impementation. 
    ''' Restricted access because the content of this list is managed by the EwE Core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub RemoveAt(ByVal iIndex As Integer) _
             Implements System.Collections.Generic.IList(Of T).RemoveAt
        Me.m_list.RemoveAt(iIndex - Me.m_iItemOffset)
        ' JS 27aug07: disabled list events to avoid confusion about possible list interfaces
        'Me.FireEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obligatory but totally useless list implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function ComeToAScreechingHaltInTheSandbox() As System.Collections.IEnumerator _
            Implements System.Collections.IEnumerable.GetEnumerator
        Return Nothing
    End Function

#End Region ' List interfaces

End Class ' cCoreInputOutputList

#End Region

#Region "Ecosim and Ecospace Results Wrappers"

''' <summary>
''' Interface for a helper class that wraps Ecosim or EcoSpace data structure results arrays
''' </summary>
''' <remarks>Ouput (model time step results) objects <see cref="cEcoSimGroupOutput">cEcoSimGroupOutput</see> hold a reference to core data that is wrapped for the interface to access via dot operators or getVariable(eVarNameFalgs,index,index)  </remarks>
Friend Interface IResultsWrapper

    Property Value(ByVal Index1 As Integer, ByVal index2 As Integer) As Single

End Interface


''' <summary>
''' 4D array with the first two indexes fixed
''' </summary>
''' <remarks> cEcosimDataStrucures.PredPreyResultsOverTime(var,prey,pred,time)</remarks>
Friend Class c4DResultsWrapper
    Implements IResultsWrapper

    'var, group, group, time
    Private m_data(,,,) As Single
    Private m_VarIndex As Integer
    Private m_GroupIndex As Integer

    Public Sub New(ByVal TheBuffer(,,,) As Single, ByVal VarIndex As Integer, ByVal GroupIndex As Integer)
        m_data = TheBuffer
        m_VarIndex = VarIndex
        m_GroupIndex = GroupIndex
    End Sub

    Public Property Value(ByVal GroupIndex As Integer, ByVal TimeIndex As Integer) As Single Implements IResultsWrapper.Value
        Get
            Return m_data(m_VarIndex, m_GroupIndex, GroupIndex, TimeIndex)
        End Get
        Set(ByVal value As Single)
            m_data(m_VarIndex, m_GroupIndex, GroupIndex, TimeIndex) = value
        End Set
    End Property
End Class


''' <summary>
''' 3D array with the first index fixed
''' </summary>
''' <remarks></remarks>
Friend Class c3DResultsWrapper
    Implements IResultsWrapper

    ' group, group, time
    Private m_data(,,) As Single
    Private m_FixedGroupIndex As Integer

    Public Sub New(ByVal TheBuffer(,,) As Single, ByVal FixedGroupIndex As Integer)
        m_data = TheBuffer
        m_FixedGroupIndex = FixedGroupIndex
    End Sub

    Public Property Value(ByVal GroupIndex As Integer, ByVal TimeIndex As Integer) As Single Implements IResultsWrapper.Value
        Get
            Return m_data(m_FixedGroupIndex, GroupIndex, TimeIndex)
        End Get
        Set(ByVal value As Single)
            m_data(m_FixedGroupIndex, GroupIndex, TimeIndex) = value
        End Set
    End Property
End Class

''' <summary>
''' 3D array with the first TWO indexes fixed i.e. ResultsOverTime(FixedVar,FixedGroup,time) 
''' </summary>
''' <remarks>cEcosimDataStructures.ResultsOverTime(var,group,time)</remarks>
Friend Class c3DResultsWrapper2Fixed
    Implements IResultsWrapper

    'var, group, time
    Private m_data(,,) As Single
    Private m_FixedGroupIndex As Integer
    Private m_FixedVarIndex As Integer


    Public Sub New(ByVal TheBuffer(,,) As Single, ByVal FixedVarIndex As Integer, ByVal FixedGroupIndex As Integer)
        m_data = TheBuffer
        m_FixedGroupIndex = FixedGroupIndex
        m_FixedVarIndex = FixedVarIndex
    End Sub

    Public Property Value(ByVal TimeIndex As Integer, ByVal NotUsed As Integer) As Single Implements IResultsWrapper.Value
        Get
            Return m_data(m_FixedVarIndex, m_FixedGroupIndex, TimeIndex)
        End Get
        Set(ByVal value As Single)
            m_data(m_FixedVarIndex, m_FixedGroupIndex, TimeIndex) = value
        End Set
    End Property
End Class

#End Region


