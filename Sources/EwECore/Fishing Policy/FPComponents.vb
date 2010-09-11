Option Strict On
Imports EwECore.ValueWrapper

#Region "Enumerators"

Public Enum eInitOption
    EcopathBaseF
    CurrentF
    RandomF
End Enum

Public Enum eOptimizeOptionTypes
    BatchRun
    MaxPortUtil
    PrevCostEarning
    UseEcospace
    IncludeComp
End Enum

Public Enum eSearchOptionTypes
    Fletch
    DFPmin
    BaseProfitability
End Enum


Public Enum eOptimizeApproachTypes
    SystemObjective
    FleetValues
End Enum

#End Region

#Region "Fishing Policy parameters"


Public Class cFishingPolicyParameters
    Inherits cCoreInputOutputBase


    Public Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.AllowValidation = False
        Me.DBID = DBID
        Me.m_DataType = eDataTypes.FishingPolicyParameters
        Me.m_messageSource = eMessageSource.FishingPolicySearch
        AllowValidation = False

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FishingPolicyParameters, _
                                                    eMessageSource.EcoSim, Index, cCore.NULL_VALUE)

        Dim val As cValue
        Dim meta As cVariableMetaData

        'FPSMaxNumEval
        meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.FPSMaxNumEval, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxNumEval))
        m_values.Add(val.varName, val)


        'FPSMaxEffChange
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.FPSMaxEffChange, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxEffChange))
        m_values.Add(val.varName, val)


        'xxxxxxxxxxxxxxxxxxxxxx
        'Enumerators are stored as Integer!!
        'FPSInitOption 
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eInitOption)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSInitOption, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSInitOption))
        m_values.Add(val.varName, val)

        'FPSSearchOption
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eSearchOptionTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSSearchOption, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSSearchOption))
        m_values.Add(val.varName, val)

        'FPSOptimizeApproach
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eOptimizeApproachTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSOptimizeApproach, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSOptimizeApproach))
        m_values.Add(val.varName, val)

        'FPSOptimizeApproach
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eOptimizeOptionTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSOptimizeOptions, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSOptimizeOptions))
        m_values.Add(val.varName, val)

        'Number of runs 500 Max ???
        meta = New cVariableMetaData(1, 500, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSNRuns, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSNRuns))
        m_values.Add(val.varName, val)
        Me.AllowValidation = True


        'Boolean parameters
        'FPSMaxPortUtil
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSMaxPortUtil, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxPortUtil))
        m_values.Add(val.varName, val)

        ''FPSPrevCostEarning
        'meta = New cVariableMetaData()
        'val = New cValue(New Boolean, eVarNameFlags.SearchPrevCostEarning, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.SearchPrevCostEarning))
        'm_values.Add(val.varName, val)

        'FPSIncludeComp
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSIncludeComp, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSIncludeComp))
        m_values.Add(val.varName, val)


        'FPSBatchRun
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSBatchRun, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSBatchRun))
        m_values.Add(val.varName, val)


        'FPSUseEcospace
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSUseEcospace, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSUseEcospace))
        m_values.Add(val.varName, val)

        ResetStatusFlags()
        AllowValidation = True

    End Sub

    Public Property InitOption() As eInitOption
        Get
            Return CType(GetVariable(eVarNameFlags.FPSInitOption), eInitOption)
        End Get

        Set(ByVal value As eInitOption)
            SetVariable(eVarNameFlags.FPSInitOption, value)
        End Set
    End Property


    Public Property MaxNumEval() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.FPSMaxNumEval), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.FPSMaxNumEval, value)
        End Set
    End Property


    Public Property MaxEffChange() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.FPSMaxEffChange), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.FPSMaxEffChange, value)
        End Set
    End Property


    Public Property SearchOption() As eSearchOptionTypes
        Get
            Return CType(GetVariable(eVarNameFlags.FPSSearchOption), eSearchOptionTypes)
        End Get

        Set(ByVal value As eSearchOptionTypes)
            SetVariable(eVarNameFlags.FPSSearchOption, value)
        End Set
    End Property

 
    Public Property OptimizeApproach() As eOptimizeApproachTypes
        Get
            Return CType(GetVariable(eVarNameFlags.FPSOptimizeApproach), eOptimizeApproachTypes)
        End Get

        Set(ByVal value As eOptimizeApproachTypes)
            SetVariable(eVarNameFlags.FPSOptimizeApproach, value)
        End Set
    End Property

    Public Property nRuns() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.FPSNRuns))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.FPSNRuns, value)
        End Set
    End Property


    Public Property MaxPortUtil() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.FPSMaxPortUtil))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.FPSMaxPortUtil, value)
        End Set
    End Property

  
    Public Property IncludeComp() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.FPSIncludeComp))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.FPSIncludeComp, value)
        End Set
    End Property


    Public Property BatchRun() As Boolean
        Get
            Return False
        End Get

        Set(ByVal value As Boolean)
            Debug.Assert(False, Me.ToString & ".BatchRun() has not been implemented yet!")
        End Set
    End Property

    Public Property UseEcospace() As Boolean
        Get
            Return False
        End Get

        Set(ByVal value As Boolean)
            Debug.Assert(False, Me.ToString & ".UseEcospace() has not been implemented yet!")
        End Set
    End Property

End Class




#End Region


