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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

Namespace ValueWrapper


    Public Delegate Function CoreCounterDelegate(ByVal SizeType As eCoreCounterTypes) As Integer
    Public Delegate Function CoreIndexedCounterDelegate(ByVal SizeType As eCoreCounterTypes, ByVal iArrayIndex As Integer) As Integer


    ''' <summary>
    ''' Classes used to wrap variables and there associated data used be the ICoreInputOuput objects
    ''' </summary>
    ''' <remarks>
    ''' These classes are defined as Friend so that they are not exposed outside of the core
    ''' </remarks>
    ''' <history>
    ''' <revision>jb 17/mar/06 Added Length of array size</revision>
    ''' </history>

#Region "Enumerators used by Value objects"

    Public Enum eValueTypes
        Int 'integer
        Str 'string
        Sng 'single
        Bool 'boolean

        SingleArray 'array of singles 
        BoolArray 'array of boolean 
        IntArray 'array of integers

        'Histogram
    End Enum

#End Region

#Region "cValue"


    ''' <summary>
    ''' Wraps the Value, Status, Name and Type of a variable used be an ICoreInputOuput object into one place.
    ''' </summary>
    ''' <remarks>
    ''' cValue acts as the base class for other types of value object.
    ''' ToDo:: the varType enumerator could be change to being a System.Type object.
    ''' </remarks>
    Public Class cValue
        Implements IDisposable

        Private m_value As Object
        Protected m_orgvalue As Object
        Protected m_status As eStatusFlags
        Protected m_orgStatus As eStatusFlags
        Protected m_validationstatus As eStatusFlags

        Protected m_varType As eValueTypes
        Protected m_varName As eVarNameFlags
        Protected m_message As String 'message associated with data validation

        Protected m_iIndex As Integer = 0
        Protected m_bStored As Boolean = False
        Protected m_bAffectsRunState As Boolean = True

        ''' <summary>
        ''' Validator supplied in the constructor of the object.
        ''' </summary>
        ''' <remarks>This validator can be specific to the this variable type or it can be the default supplied by the ValidatorManger.</remarks>
        Protected m_validator As cValidatorDefault

        Protected m_metadata As cVariableMetaData

        Protected m_bValidate As Boolean

        ''' <summary>
        ''' Default constructor.
        ''' </summary>
        ''' <remarks>
        ''' A default value will not be stored, will affect the core run state.
        ''' A default value has no metadata and no validation.
        ''' </remarks>
        Sub New()
            Me.New(Nothing, eVarNameFlags.NotSet, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_bStored = False
        End Sub

        ''' <summary>
        ''' Constructs a new value instance, without metadata and validation
        ''' </summary>
        ''' <param name="Value">The object to hold the value.</param>
        ''' <param name="VarName">The variable name representing the value.</param>
        ''' <param name="Status">Value status.</param>
        ''' <param name="VarType"><see cref="eValueTypes">Value type</see>.</param>
        Sub New(ByVal Value As Object, _
                ByVal VarName As eVarNameFlags, _
                ByVal Status As eStatusFlags, _
                ByVal VarType As eValueTypes)
            Me.New(Value, VarName, Status, VarType, Nothing)
        End Sub

        ''' <summary>
        ''' Constructs a new value instance without validation.
        ''' </summary>
        ''' <param name="Value">The object to hold the value.</param>
        ''' <param name="VarName">The variable name representing the value.</param>
        ''' <param name="Status">Value status.</param>
        ''' <param name="VarType"><see cref="eValueTypes">Value type</see>.</param>
        ''' <param name="MetaData"><see cref="cVariableMetaData">Value metadata</see>.</param>
        Sub New(ByVal Value As Object, _
                ByVal VarName As eVarNameFlags, _
                ByVal Status As eStatusFlags, _
                ByVal VarType As eValueTypes, _
                ByVal MetaData As cVariableMetaData)
            ' JS 15Feb15: Metadata was not passed along which seems like a bug to me
            'Me.New(Value, VarName, Status, VarType, Nothing, Nothing)
            Me.New(Value, VarName, Status, VarType, MetaData, Nothing)
        End Sub

        ''' <summary>
        ''' Constructs a new value instance without validation.
        ''' </summary>
        ''' <param name="Value">The object to hold the value.</param>
        ''' <param name="VarName">The variable name representing the value.</param>
        ''' <param name="Status">Value status.</param>
        ''' <param name="VarType"><see cref="eValueTypes">Value type</see>.</param>
        ''' <param name="MetaData"><see cref="cVariableMetaData">Value metadata</see> to use.</param>
        ''' <param name="Validator"><see cref="cValidatorDefault">Validator</see> to use.</param>
        Sub New(ByVal Value As Object, _
                ByVal VarName As eVarNameFlags, _
                ByVal Status As eStatusFlags, _
                ByVal VarType As eValueTypes, _
                ByRef MetaData As cVariableMetaData, _
                ByRef Validator As cValidatorDefault)

            Me.m_value = Value
            Me.m_varType = VarType
            Me.m_varName = VarName
            Me.m_status = Status
            Me.m_metadata = MetaData

            ' Set the validator and its properties
            Me.m_bValidate = (Validator IsNot Nothing)
            Me.m_validator = Validator

            Me.m_bStored = True
            Me.m_bAffectsRunState = True

            ' Complement metadata
            If (Me.m_metadata Is Nothing) Then
                Me.m_metadata = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            End If

            Me.Metadata.Attach(Me)

        End Sub

        ''' <summary>
        ''' Get/set the Index of a Value
        ''' </summary>
        Public Property Index() As Integer
            Get
                Return m_iIndex
            End Get
            Friend Set(ByVal value As Integer)
                m_iIndex = value
            End Set
        End Property

        ''' <summary>
        ''' Set the size of the array to the new Value based on the CoreCounterDelegate passed in via the consturctor
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>This is for array value objects only.</remarks>
        Public Overridable Function SetSize() As Boolean
            Return False
        End Function

        Public Overridable ReadOnly Property IsArray() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Get/set the status flag for a Value.
        ''' </summary>
        ''' <param name="iIndex">Optional value index.</param>
        Public Overridable Property Status(Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As eStatusFlags
            Get
                Return m_status
            End Get
            Friend Set(ByVal value As eStatusFlags)
                m_status = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the last valation result for a Value.
        ''' </summary>
        ''' <param name="iIndex">Optional value index.</param>
        Public Overridable Property ValidationStatus(Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As eStatusFlags
            Get
                Return m_validationstatus
            End Get
            Set(ByVal value As eStatusFlags)
                m_validationstatus = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the actual value of a Value.
        ''' </summary>
        ''' <param name="iIndex">Optional value index.</param>
        Public Overridable Property Value(Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As Object
            Get
                Return m_value
            End Get
            Set(ByVal value As Object)
                Validate(value)
            End Set
        End Property

        Public Property varName() As eVarNameFlags
            Get
                Return m_varName
            End Get
            Friend Set(ByVal value As eVarNameFlags)
                m_varName = value
            End Set
        End Property

        Public Property varType() As eValueTypes
            Get
                Return m_varType
            End Get
            Friend Set(ByVal value As eValueTypes)
                m_varType = value
            End Set
        End Property

        Public Property ValidationMessage() As String
            Get
                Return m_message
            End Get
            Friend Set(ByVal value As String)
                m_message = value
            End Set
        End Property

        ''' <summary>
        ''' Flag stating whether a variable can be stored in the database.
        ''' </summary>
        Public Property Stored() As Boolean
            Get
                Return Me.m_bStored
            End Get
            Friend Set(ByVal value As Boolean)
                Me.m_bStored = value
            End Set
        End Property

        ''' <summary>
        ''' Flag stating whether a variable will affect the core run state when it is modified.
        ''' </summary>
        Public Property AffectsRunState() As Boolean
            Get
                Return Me.m_bAffectsRunState
            End Get
            Friend Set(ByVal value As Boolean)
                Me.m_bAffectsRunState = value
            End Set
        End Property

        ''' <summary>
        ''' Number of elements in the underlying array for an array object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Length() As Integer
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property Metadata() As cVariableMetaData
            Get
                Return Me.m_metadata
            End Get
        End Property

        Protected Overridable Function Validate(ByRef NewValue As Object, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

            'set the value of this object to the new value passed in 
            'this allows the validator the access the new value via the public interface
            m_orgvalue = m_value
            'convert null or empty inputs into something that can be used
            m_value = Me.convertEmptyInputs(NewValue)

            'is it ok to run the validator?
            If Not m_bValidate Then
                'No Validation set the value without running the validator
                m_validationstatus = eStatusFlags.OK
                Return False 'validation was not run???
            End If

            'not every value object has a validator?
            'outputs are validated by the core once it has run the model because only it knows the working of the models and what the model results mean
            If m_validator Is Nothing Then
                'set the value without running the validator
                m_validationstatus = eStatusFlags.OK
                System.Console.WriteLine(m_varName.ToString & " does not have a validator.")
                Return False 'validation was not run???
            End If

            If m_validator.Validate(Me, m_metadata, iSecondaryIndex) Then
                If m_validationstatus = eStatusFlags.FailedValidation Then
                    'if the new value failed validation then set the value back to it's original value
                    m_value = m_orgvalue
                End If

                ' JS 10Jan08: disabled the following logic. Setting a validation status to NULL will 
                '             obscure any failed validation attempts, which in turn prevents the user
                '             from knowing what happened. As such, the Validation status flag can only be OK or Failed.
                '             The Status status flag provides more further detailed information about a variable.

                'If m_status = eStatusFlags.Null Then
                '    m_validationstatus = eStatusFlags.Null
                '    ' m_value = m_metadata.DefaultValue
                'End If
            Else 'If m_validator.Validate(Me, iSecondaryIndex) Then
                'for some reason the validator returned False it could not validate the value
                Debug.Assert(False, "Validator for " & m_varName.ToString & " failed.")
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Run the validator to set the status flag without setting the value
        ''' </summary>
        ''' <param name="iSecondaryIndex"></param>
        ''' <remarks>This is use be the cCoreInputOutputBase to set the status flags of all its values </remarks>
        Public Overridable Sub setStatusFlag(Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE)

            If m_validator IsNot Nothing Then
                m_validator.Validate(Me, m_metadata, iSecondaryIndex)
            Else
                ' System.Console.WriteLine("No validator definded for " & m_varType.ToString)
            End If

        End Sub

        Public Property AllowValidation() As Boolean
            Get
                Return m_bValidate
            End Get
            Set(ByVal value As Boolean)
                m_bValidate = value
            End Set
        End Property


        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub



        ''' <summary>
        ''' Convert from some kind of NULL/Empty into a value of some sort
        ''' </summary>
        ''' <param name="newValue"></param>
        ''' <returns></returns>
        ''' <remarks>This is because different types of controls pass empty values differently</remarks>
        Protected Function convertEmptyInputs(ByVal newValue As Object) As Object

            ' Test whether provided value is empty
            Dim bNeedDefault As Boolean = (newValue Is Nothing) Or (TypeOf newValue Is System.DBNull)

            ' Convert enums to storage types
            If newValue.GetType.IsEnum Then
                Select Case Me.m_varType
                    Case eValueTypes.Int
                        newValue = CInt(newValue)
                    Case eValueTypes.Bool
                        newValue = CBool(newValue)
                    Case eValueTypes.Sng
                        newValue = CSng(newValue)
                    Case Else
                        Debug.Assert(False)
                End Select
            End If

            ' Not an empty value?
            If Not bNeedDefault Then
                ' #Yes: is a numerical variable being set?
                If (Me.varType <> eValueTypes.Str) Then
                    ' #Yes: is a string provided for a numerical variable?
                    If (TypeOf newValue Is String) Then
                        ' #Yes: is the value string empty?
                        If String.IsNullOrEmpty(newValue.ToString) Then
                            ' #Yes: we'll need the default value here to ensure core data remains valid
                            bNeedDefault = True
                        End If
                    Else
                        ' #No: numerical value provided for a numerical variable

                        ' Ok, here's a tricky bit. For SOME type of variables entering a '0' will clear it. This only
                        ' applies to numerical variables whose metadata does not allow '0' values.

                        ' Is a numerical var?
                        Select Case Me.varType
                            Case eValueTypes.Int, eValueTypes.Sng
                                ' Is 0.0! entered and metadata available?
                                Dim x As Single
                                Single.TryParse(newValue.ToString, x)
                                If x = 0.0F And (Me.Metadata IsNot Nothing) Then
                                    ' #Yes: does metadata NOT allow 0.0?
                                    If Not (Metadata.MinOperator.Compare(0.0!, Metadata.Min) And Metadata.MaxOperator.Compare(0.0!, Metadata.Max)) Then
                                        ' #Yes: '0' clears the variable
                                        bNeedDefault = True
                                    End If
                                End If
                        End Select
                    End If
                End If
            End If

            If (bNeedDefault) Then
                Select Case Me.varType
                    Case eValueTypes.Str
                        newValue = CStr(Me.m_metadata.NullValue)
                    Case eValueTypes.Int, eValueTypes.IntArray
                        newValue = CInt(Me.m_metadata.NullValue)
                    Case eValueTypes.Sng, eValueTypes.SingleArray
                        newValue = CSng(Me.m_metadata.NullValue)
                    Case eValueTypes.Bool, eValueTypes.BoolArray
                        newValue = CBool(Me.m_metadata.NullValue)
                    Case Else
                        ' JS: status flag is overwritten later on. No need trying to set
                        ' Status = eStatusFlags.ErrorEncountered
                        Debug.Assert(False, Me.ToString & ".convertEmptyInputs(...) unsupported varType " & Me.varType)
                End Select
            End If

            'value that got passed in as a string but it is supposed to be something else
            ' JS 070122: String-to-number implemented with blunt Var() since this method is the most
            '            robust alternative by ignoring rubbish characters on a presumed number string.
            '            For instance, this thing converts "4foo" to 4 and "plop8" to 0.
            '            The calling logic will need to decide whether this is proper behaviour. This
            '            method of conversion is simply selected to keep the core from exploding.

            'jb Mar-2012 Mono compatibility 
            'Val() is in the Microsoft.VisualBasic library
            'So I've replace the Val() code with TryParse(string,x)
            'Hope this works the same...
            If (TypeOf newValue Is System.String) Then

                Select Case Me.varType
                    Case eValueTypes.Str
                        ' Ok
                    Case eValueTypes.Int
                        Dim x As Integer = CInt(cSystemUtils.Val(newValue))
                        newValue = x
                    Case eValueTypes.Sng
                        Dim x As Single = CSng(cSystemUtils.Val(newValue))
                        newValue = x
                    Case eValueTypes.Bool
                        Dim x As Boolean
                        Boolean.TryParse(newValue.ToString, x)
                        newValue = x
                    Case Else
                        ' JS: status flag is overwritten later on. No need trying to set
                        'Status = eStatusFlags.ErrorEncountered
                        Debug.Assert(False, Me.ToString & ".convertEmptyInputs() unsupported varType " & Me.varType)
                End Select
            End If

            Return newValue

        End Function

        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            Me.m_metadata = Nothing
            Me.m_orgStatus = Nothing
            Me.m_status = Nothing
            Me.m_value = Nothing
            Me.m_orgvalue = Nothing
            Me.m_validator = Nothing
        End Sub

    End Class

#End Region

#Region "cValueArray"

    ''' <summary>
    ''' Provides an implemention of cValue that is used for Array values
    ''' </summary>
    ''' <remarks>At this time the internal array is weak typed as an object</remarks>
    Public Class cValueArray
        Inherits cValue

        Protected m_statusarray() As eStatusFlags
        Protected m_values As Object
        Protected m_nObjects As Integer = cCore.NULL_VALUE 'number of object in the array
        Protected m_CounterDelegate As CoreCounterDelegate = Nothing
        Protected m_Countertype As eCoreCounterTypes


        Sub New(ByVal theValueType As eValueTypes, ByVal VarName As eVarNameFlags, ByVal Status As eStatusFlags, ByVal CounterType As eCoreCounterTypes, _
                ByRef CounterDelegate As CoreCounterDelegate, ByRef MetaData As cVariableMetaData, ByRef Validator As cValidatorDefault)
            MyBase.New(Nothing, VarName, Status, theValueType)

            varType = theValueType
            m_varName = VarName

            ' Complement metadata
            m_metadata = MetaData
            If (Me.m_metadata Is Nothing) Then
                Me.m_metadata = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            End If
            Me.Metadata.Attach(Me)

            m_validator = Validator

            m_CounterDelegate = CounterDelegate
            m_Countertype = CounterType
            Me.m_bStored = True

            If SetSize() Then 'this will redim the arrays and set m_nObjects
                For i As Integer = 0 To m_nObjects
                    m_statusarray(i) = Status
                Next
            End If

        End Sub

        ''' <summary>
        ''' Construct a value object of array data that does not do data validation
        ''' </summary>
        ''' <param name="VarName">eVarNameFlags of the data to hold</param>
        ''' <param name="Status">Default status</param>
        ''' <param name="CounterType">Type of core counter to use for dimensioning the array</param>
        ''' <param name="CounterDelegate">Delegate supplied by the core use to retrieve the size of the data</param>
        ''' <remarks></remarks>
        Sub New(ByVal theValueType As eValueTypes, ByVal VarName As eVarNameFlags, ByVal Status As eStatusFlags, _
                ByVal CounterType As eCoreCounterTypes, ByRef CounterDelegate As CoreCounterDelegate)
            Me.New(theValueType, VarName, Status, CounterType, CounterDelegate, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Set the size of the array to the value in the cores data counter i.e. nGroups
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>This will only dimension the array data if the core counter is of a different size then the existing data.
        '''  Once the data has been resized it will need to be repopulated.</remarks>
        Public Overrides Function SetSize() As Boolean

            If m_CounterDelegate IsNot Nothing Then

                Dim newsize As Integer = m_CounterDelegate(m_Countertype)

                'only resize the data if it is different
                If newsize <> m_nObjects Then
                    m_nObjects = newsize

                    Select Case Me.varType
                        Case eValueTypes.BoolArray
                            Dim s(m_nObjects) As Boolean
                            m_values = s
                        Case eValueTypes.IntArray
                            Dim s(m_nObjects) As Integer
                            m_values = s
                        Case eValueTypes.SingleArray
                            Dim s(m_nObjects) As Single
                            m_values = s
                    End Select

                    ReDim m_statusarray(m_nObjects)

                    For i As Integer = 0 To m_nObjects
                        m_statusarray(i) = eStatusFlags.Null
                    Next
                End If

                Return True

            Else
                System.Console.WriteLine(Me.ToString & ".setSize() not implemented.")
                Return False
            End If

        End Function

        Public Overrides Property Status(Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As eStatusFlags
            Get
                If iSecondaryIndex <> cCore.NULL_VALUE Then
                    Return m_statusarray(iSecondaryIndex)
                Else
                    'if iSecondaryIndex is NULL for an arrayed value then return NULL
                    'we have no way of know what the user wanted
                    Return eStatusFlags.Null
                End If
            End Get
            Friend Set(ByVal value As eStatusFlags)
                If iSecondaryIndex <> cCore.NULL_VALUE Then
                    m_statusarray(iSecondaryIndex) = value
                Else
                    'no index so set all status flags to the new value
                    For i As Integer = 1 To m_nObjects
                        m_statusarray(i) = value
                    Next
                End If
            End Set
        End Property

        Public Overrides Property Value(Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Object

            Get
                Try
                    If iSecondaryIndex <> cCore.NULL_VALUE Then
                        'Debug.Assert(iSecondaryIndex <= m_nObjects And iSecondaryIndex >= 0, String.Format("{0}.Value({1}, {2}) secondary index out of bounds", Me.ToString(), Me.m_varName, iSecondaryIndex))
                        Return DirectCast(m_values, Array).GetValue(iSecondaryIndex)
                    Else
                        Return m_values
                    End If
                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".Value Error: " & ex.Message)
                    Return Nothing
                End Try

            End Get

            Set(ByVal value As Object)

                Try
                    If TypeOf value Is System.Array Then
                        'no data validation on arrays
                        'Oh my..........
                        Try
                            System.Array.Copy(DirectCast(value, Array), DirectCast(m_values, Array), DirectCast(m_values, Array).Length)
                        Catch ex As Exception
                            Debug.Assert(False, Me.ToString & ".Value() Failed to convert value to array.")
                            Me.Status = eStatusFlags.ErrorEncountered ' I think this will work???
                        End Try

                    Else
                        Debug.Assert(iSecondaryIndex <= m_nObjects And iSecondaryIndex >= 0, Me.ToString & ".Value() iGroup out of bounds.")
                        Validate(value, iSecondaryIndex)
                    End If
                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".Value Error: " & ex.Message)
                End Try

            End Set

        End Property

        Public Overrides ReadOnly Property Length() As Integer
            Get
                Return m_nObjects
            End Get
        End Property

        Public ReadOnly Property CoreCounterType() As eCoreCounterTypes
            Get
                Return Me.m_Countertype
            End Get
        End Property

        Public Overrides ReadOnly Property IsArray() As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Validate an array value object
        ''' </summary>
        ''' <param name="NewValue"></param>
        ''' <param name="iSecondaryIndex"></param>
        ''' <returns></returns>
        ''' <remarks>This can not be handled by the cValue base class because the underlying data is handled differently. Array values are stored in an array (duh...)</remarks>
        Protected Overrides Function Validate(ByRef NewValue As Object, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

            'convert null or empty inputs into something that can be used
            NewValue = Me.convertEmptyInputs(NewValue)

            ' JS 06Mar11: Array.SetValue cannot perform certain type conversions, such as Single to Integer.
            '             If an integer array receives a single value Array.SetValue will throw an exception.
            '             A dynamic type conversion will prevent this problem.

            ' Determine the type that this array accepts
            Dim arr As Array = DirectCast(m_values, Array)
            Dim tArr As Type = arr.GetType.GetElementType

            'set the value to the newvalue 
            'keep the old value in case the newvalue fails validation
            Me.m_orgvalue = arr.GetValue(iSecondaryIndex)
            arr.SetValue(Convert.ChangeType(NewValue, tArr), iSecondaryIndex)

            If Not m_bValidate Then
                m_validationstatus = eStatusFlags.OK
                Return False ' validation not run
            End If

            'no validator so boot out of here
            If m_validator Is Nothing Then
                m_validationstatus = eStatusFlags.OK
                ' System.Console.WriteLine("No Validator for " & m_varName.ToString)
                Return False
            End If

            'Ok run the validator
            If m_validator.Validate(Me, m_metadata, iSecondaryIndex) Then

                If m_validationstatus = eStatusFlags.FailedValidation Then
                    'if the new value failed validation then set the value back to it's original value
                    Try
                        arr.SetValue(Me.m_orgvalue, iSecondaryIndex)
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to reset value")
                    End Try
                End If

                If m_statusarray(iSecondaryIndex) = eStatusFlags.Null Then
                    ' m_values(iSecondaryIndex) = m_metadata.NullValue
                    Try
                        arr.SetValue(Convert.ChangeType(m_metadata.NullValue, tArr), iSecondaryIndex)
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to set default value")
                    End Try
                End If

            End If

            Return True ' validation run

        End Function

        Public Overrides Sub Dispose()
            MyBase.Dispose()
            Me.m_values = Nothing
            Me.m_CounterDelegate = Nothing

        End Sub

    End Class

#End Region ' cValueArray


#Region "cValueArrayIndexed"

    Public Class cValueArrayIndexed
        Inherits cValueArray

        Protected m_dataType As eDataTypes
        Protected m_iArrayIndex As Integer
        Shadows m_CounterDelegate As CoreIndexedCounterDelegate

        ''' <summary>
        ''' Constructor with no validation object
        ''' </summary>
        ''' <param name="theValueType"></param>
        ''' <param name="VarName"></param>
        ''' <param name="Status"></param>
        ''' <param name="CounterType"></param>
        ''' <param name="CounterDelegate"></param>
        ''' <remarks></remarks>
        Sub New(ByVal theValueType As eValueTypes, ByVal VarName As eVarNameFlags, ByVal Status As eStatusFlags, ByVal CounterType As eCoreCounterTypes, _
                ByRef CounterDelegate As CoreIndexedCounterDelegate, ByVal iArrayIndex As Integer, ByVal DataType As eDataTypes)
            MyBase.New(theValueType, VarName, Status, CounterType, Nothing)

            varType = theValueType
            m_varName = VarName
            m_dataType = DataType
            m_iArrayIndex = iArrayIndex

            m_CounterDelegate = CounterDelegate
            m_Countertype = CounterType

            If SetSize() Then 'this will redim the arrays and set m_nObjects
                For i As Integer = 0 To m_nObjects
                    m_statusarray(i) = Status
                Next
            Else
                Debug.Assert(False, "Something is wrong in " & Me.ToString & ".New()")
            End If

        End Sub


        ''' <summary>
        ''' Set the size of the array to the value in the cores data counter i.e. nGroups
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>This will only dimension the array data if the core counter is of a different size then the existing data.
        '''  Once the data has been resized it will need to be repopulated.</remarks>
        Public Overrides Function SetSize() As Boolean

            If m_CounterDelegate IsNot Nothing Then

                Dim newsize As Integer = m_CounterDelegate(m_Countertype, m_iArrayIndex)

                'only resize the data if it is different
                If newsize <> m_nObjects Then
                    m_nObjects = newsize
                    Select Case Me.varType
                        Case eValueTypes.BoolArray
                            Dim s(m_nObjects) As Boolean
                            m_values = s
                        Case eValueTypes.IntArray
                            Dim s(m_nObjects) As Integer
                            m_values = s
                        Case eValueTypes.SingleArray
                            Dim s(m_nObjects) As Single
                            m_values = s
                    End Select

                    ReDim m_statusarray(m_nObjects)

                End If

                Return True

            Else
                'System.Console.WriteLine(Me.ToString & ".setSize() not implemented.")
                'When a cValueArrayIndexed object in constructed it will call the base class constructor will a null m_CounterDelegate
                'which in turn calls this method before cValueArrayIndexed has had a chance to set m_CounterDelegate
                Return False
            End If

        End Function


        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.m_CounterDelegate = Nothing

        End Sub


    End Class

#End Region


End Namespace
