' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region "Operator Manager"

''' <summary>
''' Manages the operators
''' </summary>
''' <remarks>Operators are use for variable validation </remarks>
Public Class cOperatorManager
    Private Shared ReadOnly s_initLock As New Object()
    Private Shared m_operators As Dictionary(Of eOperators, cOperatorBase)

    ''' <summary>
    ''' Get the operator object for this eOperators type
    ''' </summary>
    ''' <param name="OperatorType"></param>
    ''' <returns>A cOperatorBase object of the type specified.</returns>
    ''' <remarks>Operators are stored in a dictionary that is populated on the first call. 
    ''' Calls to getOperator(eOperators) of the same type will return the same cOperatorBase reference.</remarks>
    Public Shared Function getOperator(OperatorType As eOperators) As cOperatorBase
        Try
            Return GetOperators().Item(OperatorType)

        Catch ex As Exception
            Debug.Assert(False, "cComparisonOperators.getOperator( " & OperatorType.ToString & ") Error: " & ex.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>Returns the operator dictionary, populating it once on first use. Thread safe.</summary>
    Private Shared Function GetOperators() As Dictionary(Of eOperators, cOperatorBase)

        Dim ops As Dictionary(Of eOperators, cOperatorBase) = System.Threading.Volatile.Read(m_operators)
        If (ops IsNot Nothing) Then Return ops

        SyncLock s_initLock
            ' Only publish the dictionary once it is completely populated
            If (m_operators Is Nothing) Then
                System.Threading.Volatile.Write(m_operators, New Dictionary(Of eOperators, cOperatorBase) From {
                    {eOperators.LessThan, New cLessThan},
                    {eOperators.GreaterThan, New cGreaterThan},
                    {eOperators.EqualTo, New cEqualTo},
                    {eOperators.LessThanOrEqualTo, New cLessThanOrEqualTo},
                    {eOperators.GreaterThanOrEqualTo, New cGreaterThanOrEqualTo}
                })
            End If
            Return m_operators
        End SyncLock

    End Function

End Class

#End Region

#Region "Operators"

#Region "Operator Base Class"

''' <summary>
''' Base class for equality comparison of values
''' </summary>
''' <remarks>Use for data validation</remarks>
Public MustInherit Class cOperatorBase

    Public MustOverride Function Compare(V1 As Single, V2 As Single) As Boolean

End Class

#End Region

#Region "Operator Classes"

Public Class cLessThan
    Inherits cOperatorBase

    Public Overrides Function Compare(V1 As Single, V2 As Single) As Boolean
        Return V1 < V2
    End Function
End Class

Public Class cGreaterThan
    Inherits cOperatorBase

    Public Overrides Function Compare(V1 As Single, V2 As Single) As Boolean
        Return V1 > V2
    End Function
End Class

Public Class cEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(V1 As Single, V2 As Single) As Boolean
        Return V1 = V2
    End Function
End Class

Public Class cLessThanOrEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(V1 As Single, V2 As Single) As Boolean
        Return V1 <= V2
    End Function
End Class

Public Class cGreaterThanOrEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(V1 As Single, V2 As Single) As Boolean
        Return V1 >= V2
    End Function
End Class

#End Region

#End Region