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

Namespace ValueWrapper

    Public Class cValueArrayIndexed
        Inherits cValueArray

        Protected m_dataType As eDataTypes
        Protected m_iArrayIndex As Integer

        Public Property iSecondIndex As Integer


        ''' <summary>
        ''' Constructor with no validation object
        ''' </summary>
        ''' <param name="theValueType"></param>
        ''' <param name="VarName"></param>
        ''' <param name="Status"></param>
        ''' <param name="CounterType"></param>
        ''' <remarks></remarks>
        Sub New(core As cCore, theValueType As eValueTypes, VarName As eVarNameFlags, Status As eStatusFlags, CounterType As eCoreCounterTypes,
                iArrayIndex As Integer, DataType As eDataTypes)
            MyBase.New(core, theValueType, VarName, Status, CounterType, Nothing)

            Me.varType = theValueType
            Me.m_varName = VarName
            Me.m_dataType = DataType
            Me.m_iArrayIndex = iArrayIndex

            Me.m_Countertype = CounterType

            If Me.SetSize() Then 'this will redim the arrays and set m_nObjects
                For i As Integer = 0 To Me.m_nObjects
                    Me.m_statusarray(i) = Status
                Next
                'Else
                '    Debug.Assert(False, "Something is wrong in " & Me.ToString & ".New()")
            End If

        End Sub


        ''' <summary>
        ''' Set the size of the array to the value in the cores data counter i.e. nGroups
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>This will only dimension the array data if the core counter is of a different size then the existing data.
        '''  Once the data has been resized it will need to be repopulated.</remarks>
        Public Overrides Function SetSize() As Boolean

            If Me.m_Countertype <> eCoreCounterTypes.NotSet Then

                Dim newsize As Integer = Me.m_core.GetCoreCounter(Me.m_Countertype, Me.m_iArrayIndex)

                'only resize the data if it is different
                If newsize <> Me.m_nObjects Then
                    Me.m_nObjects = newsize
                    Select Case Me.varType
                        Case eValueTypes.BoolArray
                            Dim s(Me.m_nObjects) As Boolean
                            Me.m_values = s
                        Case eValueTypes.IntArray
                            Dim s(Me.m_nObjects) As Integer
                            Me.m_values = s
                        Case eValueTypes.SingleArray
                            Dim s(Me.m_nObjects) As Single
                            Me.m_values = s
                    End Select

                    ReDim Me.m_statusarray(Me.m_nObjects)

                End If

                Return True

            Else
                'System.Console.WriteLine(Me.ToString & ".setSize() not implemented.")
                'When a cValueArrayIndexed object in constructed it will call the base class constructor will a null m_CounterDelegate
                'which in turn calls this method before cValueArrayIndexed has had a chance to set m_CounterDelegate
                Return False
            End If

        End Function

    End Class

End Namespace
