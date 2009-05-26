'==============================================================================
'
' $Log: cFishingPolicyParameters.vb,v $
' Revision 1.5  2009/05/26 22:02:34  jeroens
' EconData availability variable value and status obtained from plug-in
'
' Revision 1.4  2009/05/26 20:17:07  jeroens
' Variables no longer Stored
'
' Revision 1.3  2009/05/26 16:45:22  joeb
' Added useEconomicPlugin and isEconomicAvailable to FPS and MSE
'
' Revision 1.2  2009/01/16 18:30:30  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:24  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

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
        Me.m_dataType = eDataTypes.FishingPolicyParameters
        Me.m_coreComponent = eCoreComponentType.FishingPolicySearch
        AllowValidation = False

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FishingPolicyParameters, _
                                                    eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

        Dim val As cValue
        Dim meta As cVariableMetaData

        'FPSMaxNumEval
        meta = New cVariableMetaData(1, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.FPSMaxNumEval, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxNumEval))
        val.Stored = False
        m_values.Add(val.varName, val)


        'FPSMaxEffChange
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.FPSMaxEffChange, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxEffChange))
        val.Stored = False
        m_values.Add(val.varName, val)

        'xxxxxxxxxxxxxxxxxxxxxx
        'Enumerators are stored as Integer!!
        'FPSInitOption 
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eInitOption)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSInitOption, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSInitOption))
        val.Stored = False
        m_values.Add(val.varName, val)

        'FPSSearchOption
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eSearchOptionTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSSearchOption, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSSearchOption))
        val.Stored = False
        m_values.Add(val.varName, val)

        'FPSOptimizeApproach
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eOptimizeApproachTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSOptimizeApproach, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSOptimizeApproach))
        val.Stored = False
        m_values.Add(val.varName, val)

        'FPSOptimizeApproach
        meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eOptimizeOptionTypes)).Length - 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSOptimizeOptions, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSOptimizeOptions))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Number of runs 500 Max ???
        meta = New cVariableMetaData(1, 500, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.FPSNRuns, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSNRuns))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Me.AllowValidation = True

        'Boolean parameters
        'FPSMaxPortUtil
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSMaxPortUtil, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSMaxPortUtil))
        val.Stored = False
        m_values.Add(val.varName, val)

        ''FPSPrevCostEarning
        'meta = New cVariableMetaData()
        'val = New cValue(New Boolean, eVarNameFlags.SearchPrevCostEarning, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.SearchPrevCostEarning))
        'val.Stored = False
        'm_values.Add(val.varName, val)

        'FPSIncludeComp
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSIncludeComp, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSIncludeComp))
        val.Stored = False
        m_values.Add(val.varName, val)

        'FPSBatchRun
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSBatchRun, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSBatchRun))
        val.Stored = False
        m_values.Add(val.varName, val)

        'FPSUseEcospace
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSUseEcospace, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSUseEcospace))
        val.Stored = False
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.FPSUseEconomicPlugin, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.FPSUseEconomicPlugin))
        val.Stored = False
        m_values.Add(val.varName, val)

        'meta = New cVariableMetaData()
        'val = New cValue(New Boolean, eVarNameFlags.isEconomicAvailable, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.isEconomicAvailable))
        'val.Stored = False
        'm_values.Add(val.varName, val)

        Me.ResetStatusFlags()

        AllowValidation = True

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean

        If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
        Me.m_core.Set_EconomicAvailable_Flags(Me, eVarNameFlags.FPSUseEconomicPlugin)
        Return True

    End Function

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

    Public Property UseEconomicPlugin() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.FPSUseEconomicPlugin))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.FPSUseEconomicPlugin, value)
        End Set
    End Property

    'Public Property isEconomicAvailable() As Boolean
    '    Get
    '        Return CBool(GetVariable(eVarNameFlags.isEconomicAvailable))
    '    End Get

    '    Set(ByVal value As Boolean)
    '        SetVariable(eVarNameFlags.isEconomicAvailable, value)
    '    End Set
    'End Property

End Class




#End Region


