Option Strict On

#Region "Operator Manager"

''' <summary>
''' Manages the operators
''' </summary>
''' <remarks>Operators are use for variable validation </remarks>
Public Class cOperatorManager
    Private Shared m_operators As Dictionary(Of eOperators, cOperatorBase)

    ''' <summary>
    ''' Get the operator object for this eOperators type
    ''' </summary>
    ''' <param name="OperatorType"></param>
    ''' <returns>A cOperatorBase object of the type specified.</returns>
    ''' <remarks>Operators are stored in a dictionary that is populated on the first call. 
    ''' Calls to getOperator(eOperators) of the same type will return the same cOperatorBase reference.</remarks>
    Public Shared Function getOperator(ByVal OperatorType As eOperators) As cOperatorBase
        Try
            'on the first call populate the dictionary with all the operators
            If m_operators Is Nothing Then
                init()
            End If
            Return m_operators.Item(OperatorType)

        Catch ex As Exception
            Debug.Assert(False, "cComparisonOperators.getOperator( " & OperatorType.ToString & ") Error: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Private Shared Sub init()

        If m_operators Is Nothing Then
            m_operators = New Dictionary(Of eOperators, cOperatorBase)

            m_operators.Add(eOperators.LessThan, New cLessThan)
            m_operators.Add(eOperators.GreaterThan, New cGreaterThan)
            m_operators.Add(eOperators.EqualTo, New cEqualTo)
            m_operators.Add(eOperators.LessThanOrEqualTo, New cLessThanOrEqualTo)
            m_operators.Add(eOperators.GreaterThanOrEqualTo, New cGreaterThanOrEqualTo)
        End If

    End Sub

End Class

#End Region

#Region "Operators"

#Region "Operator Base Class"


''' <summary>
''' Base class for equality comparison of values
''' </summary>
''' <remarks>Use for data validation</remarks>
Public MustInherit Class cOperatorBase

    Public MustOverride Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean

End Class

#End Region

#Region "Operator Classes"

Public Class cLessThan
    Inherits cOperatorBase

    Public Overrides Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean
        Return V1 < V2
    End Function
End Class

Public Class cGreaterThan
    Inherits cOperatorBase

    Public Overrides Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean
        Return V1 > V2
    End Function
End Class

Public Class cEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean
        Return V1 = V2
    End Function
End Class

Public Class cLessThanOrEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean
        Return V1 <= V2
    End Function
End Class

Public Class cGreaterThanOrEqualTo
    Inherits cOperatorBase

    Public Overrides Function Compare(ByVal V1 As Single, ByVal V2 As Single) As Boolean
        Return V1 >= V2
    End Function
End Class

#End Region

#End Region