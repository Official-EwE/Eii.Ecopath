Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#Region "Validator Default Class"

''' <summary>
''' Default validator for all data types
''' </summary>
''' <remarks></remarks>
Public Class cValidatorDefault

    Protected m_VarName As eVarNameFlags

    Sub New(ByVal VarName As eVarNameFlags)
        m_VarName = VarName
    End Sub

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        m_VarName = eVarNameFlags.NotSet
    End Sub

    ''' <summary>
    ''' Variable name of variable to validate.
    ''' </summary>
    ''' <remarks>This is set in the constructor.</remarks>
    Public ReadOnly Property VarName() As eVarNameFlags
        Get
            Return m_VarName
        End Get
    End Property

    Public Overridable Function Validate(ByVal ValueObject As cValue, ByVal MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim bCleared As Boolean = False

        Select Case ValueObject.varType

            Case eValueTypes.Int, eValueTypes.IntArray, eValueTypes.Sng, eValueTypes.SingleArray
                'numeric values
                Dim sValue As Single = CSng(ValueObject.Value(iSecondaryIndex))
                If MetaData.MinOperator.Compare(sValue, MetaData.Min) And MetaData.MaxOperator.Compare(sValue, MetaData.Max) Then
                    'passed validation
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ' Check vs default out of [min, max] range
                    If (sValue = CSng(MetaData.NullValue)) Then
                        'passed validation
                        ValueObject.ValidationStatus = eStatusFlags.OK
                    Else
                        'failed the validation 
                        ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                    End If

                    ' Always flag successfully validated cCore.NULL_VALUE values as Null
                    If sValue = cCore.NULL_VALUE And ValueObject.ValidationStatus = eStatusFlags.OK Then
                        ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
                        bCleared = True
                    End If

                End If


            Case eValueTypes.Str
                'strings

                'no null strings
                If ValueObject.Value Is Nothing Then
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
                End If

                If ValueObject.Value.ToString.Length <= MetaData.Length Then
                    ValueObject.ValidationStatus = eStatusFlags.OK
                    ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Else
                    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
                End If

            Case eValueTypes.Bool, eValueTypes.BoolArray
                'all boolean values are OK
                ValueObject.ValidationStatus = eStatusFlags.OK
                ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK

        End Select

        ' Prepare message
        If ValueObject.ValidationStatus = eStatusFlags.OK Then
            If bCleared Then
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_CLEARED, cni.GetVarName(ValueObject.varName))
            Else
                If TypeOf ValueObject.Value Is System.Array And iSecondaryIndex >= 0 Then
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value(iSecondaryIndex))
                Else
                    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                End If
            End If
        Else
            If TypeOf ValueObject.Value Is System.Array And iSecondaryIndex >= 0 Then
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value(iSecondaryIndex))
            Else
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            End If
        End If

        Return True

    End Function

End Class

#End Region

#Region "Derived Validators"

#Region "Numeric Set invalid values to NULL"


Public Class cValidatorNumericSetToNull
    Inherits cValidatorDefault

    Public Overrides Function Validate(ByVal ValueObject As cValue, ByVal MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        ' JS 10Jan08: First check whether value is the one allowed NULL value. Secondly check
        ' whether the value fils within the allowed metadata range.
        ' The null value check is performed first because the allowed NULL value may fit within 
        ' the allowed metadata range; in this special case the variable status will be set to OK
        ' instead of NULL which is not correct.

        ' Check whether value equals the one allowed metadata null value
        If (CSng(ValueObject.Value(iSecondaryIndex)) = CSng(MetaData.NullValue)) Then
            'passed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
            Return True
        End If

        ' Check whether value fits the allowed metadata range
        If MetaData.MinOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), MetaData.Min) And _
                MetaData.MaxOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), MetaData.Max) Then
            'passed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
            Return True
        End If

        ' JS 09Jan08: If validation failed, set status to Failed Validation at any time.
        ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
        ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        Return True

        ''failed the validation 
        'If Not MetaData.MinOperator.Compare(CType(ValueObject.Value(iSecondaryIndex), Single), MetaData.Min) Then
        '    'if the value is less than the min then status is FailedValidation
        '    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_CLEARED, cni.GetVarName(ValueObject.varName))
        '    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        '    ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
        '    Return True
        'End If

        'If Not MetaData.MaxOperator.Compare(CType(ValueObject.Value(iSecondaryIndex), Single), MetaData.Max) Then
        '    'if the value is greater than max then status is FailedValidation
        '    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
        '    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        '    ' ValueObject.Status(iSecondaryIndex) = eStatusFlags.FailedValidation
        '    Return True
        'End If

    End Function

End Class

#End Region

#Region "Core Validator call the core for validation"

''' <summary>
''' Have the core do the data validation via it's cCore.Validate() method
''' </summary>
''' <remarks>This is used for variables that need to use values from other parts of the core for data validation</remarks>
Public Class cValidatorCore
    Inherits cValidatorDefault

    Private m_core As cCore

    Public Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub

    Public Overrides Function Validate(ByVal ValueObject As cValue, ByVal MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean
        'Call Validate in the core to do the validation
        Return m_core.Validate(ValueObject, MetaData, iSecondaryIndex)

    End Function

End Class

#End Region

#Region "Core counter validator"

''' <summary>
''' Validate the value via one of the core counters
''' </summary>
''' <remarks></remarks>
Public Class cValidatorCounter
    Inherits cValidatorDefault

    Private m_core As cCore
    Private m_counter As eCoreCounterTypes

    Public Sub New(ByRef theCore As cCore, ByVal counterType As eCoreCounterTypes)
        m_core = theCore
        m_counter = counterType
    End Sub


    Public Overrides Function Validate(ByVal ValueObject As cValue, ByVal MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean

        Try
            Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

            Dim n As Integer = m_core.GetCoreCounter(m_counter)
            If MetaData.MinOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), 0) And _
             MetaData.MaxOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), n) Then
                'passed validation
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
                ValueObject.ValidationStatus = eStatusFlags.OK
                ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
                Return True
            End If

            ' JS 09Jan08: If validation failed, set status to Failed Validation at any time.
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, cni.GetVarName(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.FailedValidation
            Return True


        Catch ex As Exception
            cLog.Write(ex)
            Return False
        End Try


    End Function

End Class

#End Region

#Region " Odd values validator "

Public Class cValidatorOddEven
    Inherits cValidatorDefault

    Private m_bOdd As Boolean = True

    Public Sub New(ByVal bOdd As Boolean)
        Me.m_bOdd = bOdd
    End Sub

    Public Overrides Function Validate(ByVal ValueObject As ValueWrapper.cValue, _
                                       ByVal MetaData As cVariableMetaData, _
                                       Optional ByVal iSecondaryIndex As Integer = -9999) As Boolean

        ' Perform 'normal' validation
        If Not MyBase.Validate(ValueObject, MetaData, iSecondaryIndex) Then Return False

        Dim cni As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim iValue As Integer = 0
        Dim dTest As Double = 0
        Dim bOdd As Boolean = True

        Try
            If Not (TypeOf (ValueObject.Value(iSecondaryIndex)) Is Integer) Then
                cLog.Write("Validator cannot be used for this type of value")
                Return False ' Unable to validate, report error
            End If
        Catch ex As Exception
            cLog.Write(ex)
            Return False
        End Try

        iValue = CInt(ValueObject.Value(iSecondaryIndex))
        dTest = 2.0 * Math.Floor(iValue / 2.0)
        bOdd = (dTest <> iValue)

        ' Do not test if null value is 'odd' or 'even'
        If (iValue = CInt(MetaData.NullValue)) Then Return True

        If (bOdd <> Me.m_bOdd) Then
            If Me.m_bOdd Then
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED_ODD, _
                                                              cni.GetVarName(ValueObject.varName), ValueObject.Value)
            Else
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED_EVEN, _
                                                              cni.GetVarName(ValueObject.varName), ValueObject.Value)
            End If
            ValueObject.ValidationStatus = eStatusFlags.FailedValidation
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.FailedValidation
        Else
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
        End If

        Return True

    End Function

End Class

#End Region ' Odd/even values validator

''' <summary>
''' Validate output objects. This will set the status flag to a value appropriate to the output
''' </summary>
''' <remarks></remarks>
Public Class cValidatorOutput
    Inherits cValidatorDefault

    Dim m_defaultstatus As eStatusFlags

    Public Sub New(ByVal DefaultStatus As eStatusFlags)

        m_defaultstatus = DefaultStatus

    End Sub

    Public Overrides Function Validate(ByVal ValueObject As cValue, ByVal MetaData As cVariableMetaData, Optional ByVal iSecondaryIndex As Integer = cCore.NULL_VALUE) As Boolean
        'Ok for now there is no validation of output values!!! this just sets the status flag
        'if the model set the value it is assumed to be OK
        'if there is a problem then the core will need to set the status flag some other way
        'For Now

        ValueObject.Status(iSecondaryIndex) = m_defaultstatus 'the default status was passed in during construction of this object 
        ValueObject.ValidationStatus = eStatusFlags.OK
        Return True

    End Function

End Class

#End Region

#Region "Validation Manger"

''' <summary>
''' Manager for data validators. This provides access to data validator objects through its getValidator(eVarNameFlags) method
''' </summary>
''' <remarks>To add a validator create a new instance in the constructor</remarks>
Public Class cValidatorManager

    Private m_validators As Dictionary(Of eVarNameFlags, cValidatorDefault)

    ''' <summary>
    ''' Create an instance of the ValidatorManger. 
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <remarks>To add data validation. Create an instance of the data validator in this constructor. 
    ''' Add the data validator to the dictionary (m_validators) of validators using its Type (eVarNameFlags) as the key. 
    ''' When getValidator(eVarNameFlags) is called it will return this instance of the validator.
    ''' This way only one instance of a validator need to be created and it can be used to do all the validation of a given variable. </remarks>
    Sub New(ByRef theCore As cCore)

        Dim validator As cValidatorDefault

        m_validators = New Dictionary(Of eVarNameFlags, cValidatorDefault)

        'default validator in the NotSet Key
        validator = New cValidatorDefault(eVarNameFlags.NotSet)
        m_validators.Add(validator.VarName, validator)

        'Numeric validator that sets the Validation status to NULL if the value is less than the Min
        'this is used for variables that will have there value computed by a model if they are not supplied by a user
        'I.e. EE
        'Create one validator and use it for all the variables
        validator = New cValidatorNumericSetToNull()
        m_validators.Add(eVarNameFlags.EEInput, validator)
        m_validators.Add(eVarNameFlags.PBInput, validator)
        m_validators.Add(eVarNameFlags.QBInput, validator)
        m_validators.Add(eVarNameFlags.GEInput, validator)
        m_validators.Add(eVarNameFlags.BiomassAreaInput, validator)
        m_validators.Add(eVarNameFlags.Biomass, validator)

        'the same core validator for all the Ecospace summary data
        'the validator will figure out which varaible is being validated
        validator = New cValidatorCore(theCore)
        m_validators.Add(eVarNameFlags.EcospaceNumberSummaryTimeSteps, validator)
        m_validators.Add(eVarNameFlags.EcospaceSummaryTimeEnd, validator)
        m_validators.Add(eVarNameFlags.EcospaceSummaryTimeStart, validator)
        ''Fishing Policy Search blocks
        m_validators.Add(eVarNameFlags.SearchBlock, validator)
        'MSE FleetWeight must be a valid fleet
        m_validators.Add(eVarNameFlags.MSEFleetWeight, validator)

        m_validators.Add(eVarNameFlags.MSEFixedEscapement, validator)
        m_validators.Add(eVarNameFlags.MSEFixedF, validator)

        'MPAOpt
        m_validators.Add(eVarNameFlags.MPAOptStartYear, validator)
        m_validators.Add(eVarNameFlags.MPAOptEndYear, validator)

        m_validators.Add(eVarNameFlags.EcosimSumNTimeSteps, validator)
        m_validators.Add(eVarNameFlags.EcosimSumStart, validator)
        m_validators.Add(eVarNameFlags.EcosimSumEnd, validator)

        'Output validator
        validator = New cValidatorOutput(eStatusFlags.NotEditable Or eStatusFlags.OK)
        m_validators.Add(eVarNameFlags.EcospaceBiomassOverTime, validator)
        m_validators.Add(eVarNameFlags.MSELowerRiskPercent, validator)
        m_validators.Add(eVarNameFlags.MSEUpperRiskPercent, validator)

        'Fishing Policy search base year validated via a core counter
        validator = New cValidatorCounter(theCore, eCoreCounterTypes.nEcosimYears)
        m_validators.Add(eVarNameFlags.SearchBaseYear, validator)

        'MSE Results and Run start and end year use core counter
        m_validators.Add(eVarNameFlags.MSEResultsStartYear, validator)
        m_validators.Add(eVarNameFlags.MSEResultsEndYear, validator)
        m_validators.Add(eVarNameFlags.MSEStartYear, validator)

        'PSD validator(s)
        validator = New cValidatorOddEven(True)
        m_validators.Add(eVarNameFlags.NumPtsMovAvg, validator)

        'Pedigree
        validator = New cValidatorCore(theCore)
        m_validators.Add(eVarNameFlags.Pedigree, validator)

    End Sub

    ''' <summary>
    ''' Return a validator for the specified eVarNameFlags
    ''' </summary>
    ''' <param name="VarName">eVarNameFlags of validator to return</param>
    ''' <returns>A valid validator for this eVarNameFlags type or the default validator if no other validator could be found.</returns>
    ''' <remarks>Validator are created in the constructor and kept in a dictionary. 
    ''' Only one instance of each validator is use. This will return the same validator on each call for a VarName.
    ''' </remarks>
    Public Function getValidator(ByVal VarName As eVarNameFlags) As cValidatorDefault

        Try
            If m_validators.ContainsKey(VarName) Then
                Return m_validators.Item(VarName)
            Else
                'System.Console.WriteLine(VarName.ToString & " No Validator. Default will be used.")
                Return m_validators.Item(eVarNameFlags.NotSet)
            End If

        Catch ex As Exception
            'bummer
            cLog.Write(Me.ToString & "getValidator() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return Nothing
        End Try

    End Function

End Class

#End Region
