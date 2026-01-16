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



Namespace ValueWrapper

    Public Class cValueArrayTripleIndex
        Inherits cValueArrayIndexed

        Shadows m_statusarray(,) As eStatusFlags

        Protected m_counter2 As eCoreCounterTypes
        ' Protected m_counter3 As eCoreCounterTypes

        Public Property iThirdIndex As Integer

        Sub New(core As cCore, theValueType As eValueTypes, VarName As eVarNameFlags, Status As eStatusFlags,
                CounterType As eCoreCounterTypes, CounterType2 As eCoreCounterTypes, iFixedIndex1 As Integer, DataType As eDataTypes)

            MyBase.New(core, theValueType, VarName, Status, CounterType2, iFixedIndex1, DataType)

            Me.m_Countertype = CounterType
            Me.m_counter2 = CounterType2

            'm_counter3 = CounterType3

            Me.varType = theValueType
            Me.m_varName = VarName
            'm_dataType = DataType
            Me.m_iArrayIndex = iFixedIndex1
            Me.Index = iFixedIndex1


            If Me.SetSize() Then 'this will redim the arrays and set m_nObjects

                Dim n1 As Integer = Me.m_core.GetCoreCounter(Me.m_Countertype)
                Dim n2 As Integer = Me.m_core.GetCoreCounter(Me.m_counter2)
                For i As Integer = 1 To n1
                    For j As Integer = 1 To n2
                        Me.m_statusarray(i, j) = Status
                    Next
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

            Dim bCountersSet As Boolean = Not (Me.m_Countertype = eCoreCounterTypes.NotSet Or Me.m_counter2 = eCoreCounterTypes.NotSet)
            If Me.m_Countertype <> eCoreCounterTypes.NotSet And bCountersSet Then

                Dim index1 As Integer = Me.m_core.GetCoreCounter(Me.m_Countertype)
                Dim index2 As Integer = Me.m_core.GetCoreCounter(Me.m_counter2)

                'only resize the data if it is different
                If index1 <> Me.m_nObjects Then
                    Me.m_nObjects = index1
                    Select Case Me.varType
                        Case eValueTypes.BoolArray
                            Dim b(,) As Boolean = New Boolean(index1, index2) {}
                            Me.m_values = b
                        Case eValueTypes.IntArray
                            Dim i(,) As Integer = New Integer(index1, index2) {}
                            Me.m_values = i
                        Case eValueTypes.SingleArray
                            Dim s(,) As Single = New Single(index1, index2) {}
                            Me.m_values = s
                    End Select

                    Me.m_statusarray = New eStatusFlags(index1, index2) {}

                End If

                Return True

            Else
                'System.Console.WriteLine(Me.ToString & ".setSize() not implemented.")
                'When a cValueArrayIndexed object in constructed it will call the base class constructor will a null m_CounterDelegate
                'which in turn calls this method before cValueArrayIndexed has had a chance to set m_CounterDelegate
                Return False
            End If

        End Function

        ''' <summary>
        ''' Get/set the actual value of a Value.
        ''' </summary>
        ''' <param name="iIndex2">Optional value index.</param>
        Public Overrides Property Value(Optional iIndex2 As Integer = cCore.NULL_VALUE, Optional iIndex3 As Integer = cCore.NULL_VALUE) As Object
            Get
                'Return m_values(Me.Index, iIndex, iIndex2)

                If iIndex2 = cCore.NULL_VALUE And iIndex3 = cCore.NULL_VALUE Then
                    'pass it on down the chain
                    Return MyBase.Value(iIndex2)
                Else
                    'Index 1 is implied
                    'It's the Me.Index property of this group or fleet the object was created for
                    Return DirectCast(Me.m_values, Array).GetValue(iIndex2, iIndex3)
                End If


            End Get
            Set(value As Object)

                If iIndex2 = cCore.NULL_VALUE And iIndex3 = cCore.NULL_VALUE Then
                    'pass it on down the chain
                    Me.Validate(value)
                Else
                    Me.iSecondIndex = iIndex2
                    Me.iThirdIndex = iIndex3
                    'Index 1 is implied
                    'It's the Me.Index property of this group or fleet the object was created for
                    Me.Validate(value, iIndex2, iIndex3)
                End If

            End Set
        End Property

        ''' <summary>
        ''' Validate an array value object
        ''' </summary>
        ''' <param name="NewValue"></param>
        ''' <param name="iSecondIndex"></param>
        ''' <returns></returns>
        ''' <remarks>This can not be handled by the cValue base class because the underlying data is handled differently. Array values are stored in an array (duh...)</remarks>
        Protected Overrides Function Validate(ByRef NewValue As Object,
                                            Optional iSecondIndex As Integer = cCore.NULL_VALUE,
                                            Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean

            'convert null or empty inputs into something that can be used
            NewValue = Me.convertEmptyInputs(NewValue)

            ' JS 06Mar11: Array.SetValue cannot perform certain type conversions, such as Single to Integer.
            '             If an integer array receives a single value Array.SetValue will throw an exception.
            '             A dynamic type conversion will prevent this problem.

            ' Determine the type that this array accepts
            Dim arr As Array = DirectCast(Me.m_values, Array)
            Dim tArr As Type = arr.GetType.GetElementType

            'set the value to the newvalue 
            'keep the old value in case the newvalue fails validation
            Me.m_orgvalue = arr.GetValue(iSecondIndex, iThirdIndex)
            arr.SetValue(Convert.ChangeType(NewValue, tArr), iSecondIndex, iThirdIndex)

            ' Unable to validate? Boot out of here
            If (Not Me.AllowValidation) Or (Me.m_validator Is Nothing) Then
                Me.m_validationstatus = eStatusFlags.OK
                Return False
            End If

            'Ok run the validator
            If Me.m_validator.Validate(Me, Me.m_metadata, iSecondIndex, iThirdIndex) Then

                If Me.m_validationstatus = eStatusFlags.FailedValidation Then
                    'if the new value failed validation then set the value back to it's original value
                    Try
                        arr.SetValue(Me.m_orgvalue, iSecondIndex, iThirdIndex)
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to reset value")
                    End Try
                End If

                If Me.m_statusarray(iSecondIndex, iThirdIndex) = eStatusFlags.Null Then
                    ' m_values(iSecondaryIndex) = m_metadata.NullValue
                    Try
                        arr.SetValue(Convert.ChangeType(Me.m_metadata.NullValue, tArr), iSecondIndex, iThirdIndex)
                    Catch ex As Exception
                        Debug.Assert(False, "Failed to set default value")
                    End Try
                End If

            End If

            Return True ' validation run

        End Function

        ''' <summary>
        ''' Run the validator to set the status flag without setting the value
        ''' </summary>
        ''' <param name="iSecondIndex"></param>
        ''' <remarks>This is use be the cCoreInputOutputBase to set the status flags of all its values </remarks>
        Public Overrides Sub setStatusFlag(Optional iSecondIndex As Integer = cCore.NULL_VALUE,
                                            Optional iThirdIndex As Integer = cCore.NULL_VALUE)

            If Me.m_validator IsNot Nothing Then
                '  Me.Status(iSecondIndex, iThirdIndex) = eStatusFlags.OK

                Dim iGrp As Integer = iSecondIndex
                ' Dim n1 As Integer = m_CounterDelegate(m_counter2)
                Dim nTimeSteps As Integer = Me.m_core.GetCoreCounter(Me.m_counter2)
                ' For i As Integer = 1 To n1
                For its As Integer = 1 To nTimeSteps
                    'm_statusarray(i, j) = eStatusFlags.OK
                    Me.m_validator.Validate(Me, Me.m_metadata, iGrp, its)
                Next
                '   Next

                ' m_validator.Validate(Me, m_metadata, iSecondIndex, iThirdIndex)
            Else
                ' System.Console.WriteLine("No validator definded for " & m_varType.ToString)
            End If

        End Sub

        ''' <summary>
        ''' Get/set the status flag for a Value.
        ''' </summary>
        ''' <param name="iSecondIndex">Optional value index.</param>
        Public Overrides Property Status(Optional iSecondIndex As Integer = cCore.NULL_VALUE, Optional iThirdIndex As Integer = cCore.NULL_VALUE) As eStatusFlags
            Get
                Return Me.m_statusarray(iSecondIndex, iThirdIndex)
            End Get
            Friend Set(value As eStatusFlags)
                Me.m_statusarray(iSecondIndex, iThirdIndex) = value
            End Set
        End Property

        Public Overrides ReadOnly Property Length() As Integer
            Get
                Return Me.m_core.GetCoreCounter(Me.m_Countertype)
            End Get
        End Property

    End Class

End Namespace
