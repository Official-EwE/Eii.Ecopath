
Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cMSEOutput
    Inherits cCoreGroupBase

    Public Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_DataType = eDataTypes.MSEOutput
        m_messageSource = eMessageSource.MSE

        'Allow validation should be false for MSE output values
        'the status flag is set in Me.ResetStatusFlags() and should always stay the same eStatusFlags.NotEditable Or eStatusFlags.OK not via the validation
        'If a validator is used then it must be made thread safe as outputs for the MSE are set on a different thread then the core/interface thread
        'the default validator will throw a threading error
        Me.AllowValidation = False
        Me.DBID = cCore.NULL_VALUE

        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.NotEditable Or eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.MSEGroupInput, eMessageSource.MSE, Index, cCore.NULL_VALUE)

        ''arrayed values
        'Risk
        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.IntArray, eVarNameFlags.MSELowerRiskCount, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSELowerRiskCount))
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.IntArray, eVarNameFlags.MSEUpperRiskCount, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEUpperRiskCount))
        m_values.Add(val.varName, val)

        'Trial Number 
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.MSETrialNumber, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        'Total values
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSETotalValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEEcologicalValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)


        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEEmployValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)


        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEMandatedValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        'Mean values
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEMeanTotalValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEMeanEcologicalValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)


        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEMeanEmployValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)


        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEMeanMandatedValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEBestTotalValue, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

    End Sub


    Public Property LowerRiskCount(ByVal iGroup As Integer) As Single
        Get
            Return CType(GetVariable(eVarNameFlags.MSELowerRiskCount, iGroup), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSELowerRiskCount, value, iGroup)
        End Set
    End Property


    Public Property UpperRiskCount(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEUpperRiskCount, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEUpperRiskCount, value, iGroup)
        End Set
    End Property


    Public Property LowerRiskCountStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.MSELowerRiskCount, iGroup), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSELowerRiskCount, value, iGroup)
        End Set
    End Property


    Public Property UpperRiskCountStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.MSEUpperRiskCount, iGroup), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSEUpperRiskCount, value, iGroup)
        End Set
    End Property


    Public Property EmployValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEEmployValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEEmployValue, value)
        End Set
    End Property

    Public Property MandatedValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEMandatedValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEMandatedValue, value)
        End Set
    End Property

    Public Property EcologicalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEEcologicalValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEEcologicalValue, value)
        End Set
    End Property

    Public Property TotalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSETotalValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSETotalValue, value)
        End Set
    End Property

    'mean
    Public Property MeanEmployValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEMeanEmployValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEMeanEmployValue, value)
        End Set
    End Property

    Public Property MeanMandatedValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEMeanMandatedValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEMeanMandatedValue, value)
        End Set
    End Property

    Public Property MeanEcologicalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEMeanEcologicalValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEMeanEcologicalValue, value)
        End Set
    End Property

    Public Property MeanTotalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEMeanTotalValue))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEMeanTotalValue, value)
        End Set
    End Property

    Public Property BestTotalValue() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.MSEBestTotalValue), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEBestTotalValue, value)
        End Set
    End Property


    Public Property TrialNumber() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MSETrialNumber))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MSETrialNumber, value)
        End Set
    End Property



    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim statusflag As eStatusFlags = eStatusFlags.NotEditable Or eStatusFlags.OK

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray
                        For i = 0 To value.Length : value.Status(i) = statusflag : Next i
                    Case Else
                        value.Status = statusflag
                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

End Class



