
Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#Region "MSE Groups outputs"

Public Class cMSEGroupOutput
    Inherits cCoreGroupBase

#Region "Construction"

    Public Sub New(ByRef theCore As cCore, ByVal GroupDBID As Integer, ByVal groupIndex As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_dataType = eDataTypes.MSEOutput
        m_coreComponent = eCoreComponentType.MSE
        Me.DBID = GroupDBID
        Me.Index = groupIndex

        'Allow validation should be false for MSE output values
        'the status flag is set in Me.ResetStatusFlags() and should always stay the same eStatusFlags.NotEditable Or eStatusFlags.OK not via the validation
        'If a validator is used then it must be made thread safe as outputs for the MSE are set on a different thread then the core/interface thread
        'the default validator will throw a threading error
        Me.AllowValidation = False

        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.NotEditable Or eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.MSEGroupInput, eCoreComponentType.MSE, Index, cCore.NULL_VALUE)

        'Risk

        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.MSELowerRiskCount, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.MSEUpperRiskCount, eStatusFlags.Null, eValueTypes.Sng, meta)
        m_values.Add(val.varName, val)


        meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.MSEBiomass, eStatusFlags.Null, eCoreCounterTypes.nEcosimTimeSteps, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBiomass))
        m_values.Add(val.varName, val)

    End Sub

#End Region

#Region "Variable access via dot operators"

    Public Property LowerRiskCount() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MSELowerRiskCount))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MSELowerRiskCount, value)
        End Set
    End Property


    Public Property UpperRiskCount() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MSEUpperRiskCount))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MSEUpperRiskCount, value)
        End Set
    End Property


    Public Property LowerRiskCountStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MSELowerRiskCount)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSELowerRiskCount, value)
        End Set
    End Property


    Public Property UpperRiskCountStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MSEUpperRiskCount)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSEUpperRiskCount, value)
        End Set
    End Property


    Public Property Biomass(ByVal iTime As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEBiomass, iTime))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.MSEBiomass, value, iTime)
        End Set
    End Property

    Public Property BiomassStatus(ByVal iTime As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MSEBiomass, iTime)
        End Get

        Set(ByVal value As eStatusFlags)
            Me.SetStatusFlags(eVarNameFlags.MSEBiomass, value, iTime)
        End Set
    End Property
#End Region

#Region "Over ridden methods"

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

#End Region

End Class

#End Region

#Region "MSE non dimensioned outputs. I.e. Values"

Public Class cMSEOutput
    Inherits cCoreGroupBase

#Region "Construction"

    Public Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_dataType = eDataTypes.MSEOutput
        m_coreComponent = eCoreComponentType.MSE

        'Allow validation should be false for MSE output values
        'the status flag is set in Me.ResetStatusFlags() and should always stay the same eStatusFlags.NotEditable Or eStatusFlags.OK not via the validation
        'If a validator is used then it must be made thread safe as outputs for the MSE are set on a different thread then the core/interface thread
        'the default validator will throw a threading error
        Me.AllowValidation = False
        Me.DBID = cCore.NULL_VALUE

        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.NotEditable Or eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.MSEGroupInput, eCoreComponentType.MSE, Index, cCore.NULL_VALUE)

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

#End Region

#Region "Variable access via dot operators"

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

#End Region

End Class

#End Region




