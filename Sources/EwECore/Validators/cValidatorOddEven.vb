' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging

Public Class cValidatorOddEven
    Inherits cValidatorDefault

    Private m_bOdd As Boolean = True
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cValidatorOddEven)()

    Public Sub New(bOdd As Boolean)
        Me.m_bOdd = bOdd
    End Sub

    Public Overrides Function Validate(ValueObject As cValue, MetaData As cVariableMetaData,
                                         Optional iSecondaryIndex As Integer = cCore.NULL_VALUE,
                                         Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean

        ' Perform 'normal' validation
        If Not MyBase.Validate(ValueObject, MetaData, iSecondaryIndex) Then Return False

        Dim fmt As New Style.cVarnameTypeFormatter()
        Dim iValue As Integer = 0
        Dim dTest As Double = 0
        Dim bOdd As Boolean = True

        Try
            If Not (TypeOf (ValueObject.Value(iSecondaryIndex)) Is Integer) Then
                m_logger.LogInformation("Validator cannot be used for this type of value")
                Return False ' Unable to validate, report error
            End If
        Catch ex As Exception
            m_logger.LogError(ex, "Exception in odd/even validator")
            Return False
        End Try

        iValue = CInt(ValueObject.Value(iSecondaryIndex))
        dTest = 2.0 * Math.Floor(iValue / 2.0)
        bOdd = (dTest <> iValue)

        ' Do not test if null value is 'odd' or 'even'
        If (iValue = CInt(MetaData.NullValue)) Then Return True

        If (bOdd <> Me.m_bOdd) Then
            If Me.m_bOdd Then
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED_ODD,
                                                              fmt.ToString(ValueObject.varName), ValueObject.Value)
            Else
                ValueObject.ValidationMessage = String.Format(My.Resources.CoreMessages.VARIABLE_VALIDATION_FAILED_EVEN,
                                                              fmt.ToString(ValueObject.varName), ValueObject.Value)
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
