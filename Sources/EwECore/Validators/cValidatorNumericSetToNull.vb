' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cValidatorNumericSetToNull
    Inherits cValidatorDefault

    Public Overrides Function Validate(ValueObject As cValue, MetaData As cVariableMetaData,
                                         Optional iSecondaryIndex As Integer = cCore.NULL_VALUE,
                                         Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean

        Dim fmt As New Style.cVarnameTypeFormatter()

        ' JS 10Jan08: First check whether value is the one allowed NULL value. Secondly check
        ' whether the value fits within the allowed metadata range.
        ' The null value check is performed first because the allowed NULL value may fit within 
        ' the allowed metadata range; in this special case the variable status will be set to OK
        ' instead of NULL which is not correct.

        ' Check whether value equals the one allowed metadata null value
        If (CSng(ValueObject.Value(iSecondaryIndex)) = CSng(MetaData.NullValue)) Then
            'passed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_CLEARED, fmt.ToString(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
            Return True
        End If

        ' Check whether value fits the allowed metadata range
        If MetaData.MinOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), MetaData.Min) And
                MetaData.MaxOperator.Compare(CSng(ValueObject.Value(iSecondaryIndex)), MetaData.Max) Then
            'passed validation
            ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_PASSED, fmt.ToString(ValueObject.varName), ValueObject.Value)
            ValueObject.ValidationStatus = eStatusFlags.OK
            ValueObject.Status(iSecondaryIndex) = eStatusFlags.OK
            Return True
        End If

        ' JS 09Jan08: If validation failed, set status to Failed Validation at any time.
        ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.ToString(ValueObject.varName), ValueObject.Value)
        ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        Return True

        ''failed the validation 
        'If Not MetaData.MinOperator.Compare(CType(ValueObject.Value(iSecondaryIndex), Single), MetaData.Min) Then
        '    'if the value is less than the min then status is FailedValidation
        '    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_CLEARED, fmt.GetDescriptor(ValueObject.varName))
        '    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        '    ValueObject.Status(iSecondaryIndex) = eStatusFlags.Null
        '    Return True
        'End If

        'If Not MetaData.MaxOperator.Compare(CType(ValueObject.Value(iSecondaryIndex), Single), MetaData.Max) Then
        '    'if the value is greater than max then status is FailedValidation
        '    ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED, fmt.GetDescriptor(ValueObject.varName), ValueObject.Value)
        '    ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        '    ' ValueObject.Status(iSecondaryIndex) = eStatusFlags.FailedValidation
        '    Return True
        'End If

    End Function

End Class