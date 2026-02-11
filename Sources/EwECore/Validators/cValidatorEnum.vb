' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cValidatorEnum
    Inherits cValidatorDefault

    Private m_type As Type = Nothing

    Public Sub New(t As Type)
        Debug.Assert(t.IsEnum)
        Me.m_type = t
    End Sub

    Public Overrides Function Validate(ValueObject As cValue, MetaData As cVariableMetaData,
                                       Optional iSecondaryIndex As Integer = cCore.NULL_VALUE,
                                       Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean

        ' ToDo: add support for FlagsAttribute enums

        ' Perform 'normal' validation
        If Not MyBase.Validate(ValueObject, MetaData, iSecondaryIndex) Then Return False
        ' Check type
        If Not [Enum].IsDefined(Me.m_type, ValueObject.Value(iSecondaryIndex)) Then
            ValueObject.ValidationStatus = eStatusFlags.FailedValidation
        Else
            ValueObject.ValidationStatus = eStatusFlags.OK
        End If
        Return True

    End Function

End Class