'==============================================================================
'
' $Log: cEcoSimModelParameters.vb,v $
' Revision 1.6  2009/04/04 14:10:21  jeroens
' VarPQ no longer stored
'
' Revision 1.5  2009/04/04 14:09:30  jeroens
' Added header
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Contains the Model Run Parameters for EcoSim
''' i.e. 'NumberYears' number of years to run the model for
''' </summary>
''' <remarks>
''' This class is used by the interface to get/set parameters for running the EcoSim model
''' For Group related info see cEcoSimGroupInfo
''' </remarks>
Public Class cEcoSimModelParameters
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef m_core As cCore)
        MyBase.New(m_core)


        Try
            'no data validation at this time
            Me.AllowValidation = False
            m_coreComponent = eCoreComponentType.EcoSim
            m_dataType = eDataTypes.EcoSimModelParameter

            Dim val As cValue
            Dim meta As cVariableMetaData

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimModelParameter, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            'StepSize
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.StepSize, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.StepSize))
            m_values.Add(val.varName, val)

            'Relaxation
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.Relaxation, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Relaxation))
            m_values.Add(val.varName, val)

            'Discount
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.Discount, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Discount))
            m_values.Add(val.varName, val)

            'EquilibriumStepSize
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.EquilibriumStepSize, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.EquilibriumStepSize))
            m_values.Add(val.varName, val)

            'EquilMaxFishingRate
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.EquilMaxFishingRate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.EquilMaxFishingRate))
            m_values.Add(val.varName, val)

            'NumStepAvg
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.NumStepAvg, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NumStepAvg))
            m_values.Add(val.varName, val)

            'NutBaseFreeProp
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.NutBaseFreeProp, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NutBaseFreeProp))
            m_values.Add(val.varName, val)

            'NutPBMax
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.NutPBMax, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NutPBMax))
            m_values.Add(val.varName, val)

            'SystemRecovery
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.SystemRecovery, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.SystemRecovery))
            m_values.Add(val.varName, val)

            'EquilScaleMax

            'boolean
            'NudgeChecked
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.NudgeChecked, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.NudgeChecked))
            m_values.Add(val.varName, val)

            'UseVarPQ
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.UseVarPQ, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.UseVarPQ))
            val.Stored = False
            m_values.Add(val.varName, val)

            'BiomassOn
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.BiomassOn, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.BiomassOn))
            m_values.Add(val.varName, val)

            ''integers
            'NutForceFunctionNumber
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.NutForceFunctionNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NutForceFunctionNumber))
            m_values.Add(val.varName, val)

            ''integers
            'SalinityForceFunctionNumber
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.SalinityForceFunctionNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.SalinityForceFunctionNumber))
            m_values.Add(val.varName, val)

            'TempForceFunctionNumber
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.TemperatureForceFunctionNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.TemperatureForceFunctionNumber))
            m_values.Add(val.varName, val)


            'EcoSimNYears max 1000 year?!
            meta = New cVariableMetaData(0, cCore.MAX_RUN_LENGTH, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.EcoSimNYears, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.EcoSimNYears))
            m_values.Add(val.varName, val)

            'start summary
            meta = New cVariableMetaData(0, 999, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.EcosimSumStart, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.EcosimSumStart))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'end summary
            meta = New cVariableMetaData(0, 999, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.EcosimSumEnd, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.EcosimSumEnd))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'summary num time steps
            meta = New cVariableMetaData(1, 999, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.EcosimSumNTimeSteps, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.EcosimSumNTimeSteps))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'Contaminant tracing
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.ConSimOnEcoSim, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.ConSimOnEcoSim))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'PredictEffort
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.PredictEffort))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            '  Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            cLog.Write(Me.ToString & ".New() Error: " & ex.Message)

        End Try

    End Sub


#End Region

#Region "Mustoverride Method implementation for this class"

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean

        MyBase.ResetStatusFlags(bForceReset)

        If (Me.m_core.ActiveEcotracerScenarioIndex >= 0) Then
            Me.ClearStatusFlags(eVarNameFlags.ConSimOnEcoSim, eStatusFlags.NotEditable)
        Else
            Me.SetStatusFlags(eVarNameFlags.ConSimOnEcoSim, eStatusFlags.NotEditable)
        End If

        'Try

        '    Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        '    Dim value As cValue

        '    For Each keyvalue In m_values
        '        value = keyvalue.Value

        '        Select Case value.varType

        '            Case eValueTypes.Sng, eValueTypes.Int

        '                If CInt(value.Value) = cCore.NULL_VALUE Then
        '                    value.Status = eStatusFlags.Null
        '                Else
        '                    value.Status = eStatusFlags.OK
        '                End If


        '            Case eValueTypes.Str

        '                If CStr(value.Value) = "" Then
        '                    value.Status = eStatusFlags.Null Or eStatusFlags.NotEditable
        '                Else
        '                    value.Status = eStatusFlags.OK Or eStatusFlags.NotEditable
        '                End If

        '            Case eValueTypes.Bool
        '                'all boolean values must be OK???????
        '                value.Status = eStatusFlags.OK

        '            Case Else
        '                Debug.Assert(False, Me.ToString & "UnKnown value type " & value.varType.ToString)

        '        End Select

        '    Next keyvalue

        '    Return True

        'Catch ex As Exception

        '    Debug.Assert(False)
        '    Return False

        'End Try

    End Function

    'Protected Overrides Sub variableValidated(ByRef variableWrapper As cValueValidationWrapper)

    'End Sub

#End Region

#Region "Variables via dot (.) operator"


    ''' <summary>
    ''' Number of years to run the EcoSim model for
    ''' </summary>
    ''' <value></value>
    ''' <remarks>
    ''' This is a property so that when the user changes NumberYears this class can tell the EcoSim model to redim 
    ''' all variables that are dimensioned by time
    ''' </remarks>
    Public Property NumberYears() As Integer

        Get
            Return CType(getVariable(eVarNameFlags.EcoSimNYears), Integer)
        End Get

        Set(ByVal value As Integer)
            setVariable(eVarNameFlags.EcoSimNYears, value)
        End Set

    End Property

    Public Property BiomassOn() As Boolean

        Get
            Return CType(getVariable(eVarNameFlags.BiomassOn), Boolean)
        End Get

        Set(ByVal value As Boolean)
            setVariable(eVarNameFlags.BiomassOn, value)
        End Set

    End Property

    Public Property Discount() As Single

        Get
            Return CType(getVariable(eVarNameFlags.Discount), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Discount, value)
        End Set

    End Property

    Public Property EquilibriumStepSize() As Single

        Get
            Return CType(getVariable(eVarNameFlags.EquilibriumStepSize), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.EquilibriumStepSize, value)
        End Set

    End Property

    Public Property EquilMaxFishingRate() As Single

        Get
            Return CType(getVariable(eVarNameFlags.EquilMaxFishingRate), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.EquilMaxFishingRate, value)
        End Set

    End Property

    Public Property NudgeChecked() As Boolean

        Get
            Return CType(getVariable(eVarNameFlags.NudgeChecked), Boolean)
        End Get

        Set(ByVal value As Boolean)
            setVariable(eVarNameFlags.NudgeChecked, value)
        End Set

    End Property

    'Public Property NumStepAvg() As Single

    '    Get
    '        Return CType(getVariable(eVarNameFlags.NumStepAvg), Single)
    '    End Get

    '    Set(ByVal value As Single)
    '        setVariable(eVarNameFlags.NumStepAvg, value)
    '    End Set

    'End Property

    Public Property NutBaseFreeProp() As Single

        Get
            Return CType(getVariable(eVarNameFlags.NutBaseFreeProp), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.NutBaseFreeProp, value)
        End Set

    End Property

    Public Property NutForceFunctionNumber() As Integer

        Get
            Return CType(getVariable(eVarNameFlags.NutForceFunctionNumber), Integer)
        End Get

        Set(ByVal value As Integer)
            setVariable(eVarNameFlags.NutForceFunctionNumber, value)
        End Set

    End Property

    Public Property SalinityForceFunctionNumber() As Integer

        Get
            Return CType(GetVariable(eVarNameFlags.SalinityForceFunctionNumber), Integer)
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.SalinityForceFunctionNumber, value)
        End Set

    End Property

    Public Property TemperatureForceFunctionNumber() As Integer

        Get
            Return CType(GetVariable(eVarNameFlags.TemperatureForceFunctionNumber), Integer)
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.TemperatureForceFunctionNumber, value)
        End Set

    End Property


    Public Property NutPBMax() As Single

        Get
            Return CType(getVariable(eVarNameFlags.NutPBMax), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.NutPBMax, value)
        End Set

    End Property


    Public Property Relaxation() As Single

        Get
            Return CType(getVariable(eVarNameFlags.Relaxation), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Relaxation, value)
        End Set

    End Property

    Public Property StepSize() As Single

        Get
            Return CType(getVariable(eVarNameFlags.StepSize), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.StepSize, value)
        End Set

    End Property

    Public Property SystemRecovery() As Single

        Get
            Return CType(getVariable(eVarNameFlags.SystemRecovery), Single)
        End Get

        Set(ByVal value As Single)
            setVariable(eVarNameFlags.SystemRecovery, value)
        End Set

    End Property

    Public Property UseVarPQ() As Boolean

        Get
            Return CType(getVariable(eVarNameFlags.UseVarPQ), Boolean)
        End Get

        Set(ByVal value As Boolean)
            setVariable(eVarNameFlags.UseVarPQ, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcosimDataStructures.SumStart">start</see>
    ''' of the first summary period (in years) for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property StartSummaryTime() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSumStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSumStart, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start of the last summary period (in years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EndSummaryTime() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSumEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSumEnd, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number to time steps to summarize the data over.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberSummaryTimeSteps() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.EcosimSumNTimeSteps))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcosimSumNTimeSteps, value)
        End Set

    End Property

    Public Property ContaminantTracing() As Boolean

        Get
            Return CType(GetVariable(eVarNameFlags.ConSimOnEcoSim), Boolean)
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.ConSimOnEcoSim, value)
        End Set

    End Property

    Public Property PredictEffort() As Boolean

        Get
            Return CType(GetVariable(eVarNameFlags.PredictEffort), Boolean)
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PredictEffort, value)
        End Set

    End Property

    'Public Property RegFeedBack() As Boolean

    '    Get
    '        Return CType(GetVariable(eVarNameFlags.RegFeedBack), Boolean)
    '    End Get

    '    Set(ByVal value As Boolean)
    '        SetVariable(eVarNameFlags.RegFeedBack, value)
    '    End Set

    'End Property

#End Region

#Region "Status via dot (.) operator"

    Public Property BiomassOnStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.BiomassOn)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.BiomassOn, value)
        End Set

    End Property

    Public Property ContaminantTracingStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.ConSimOnEcoSim)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.ConSimOnEcoSim, value)
        End Set

    End Property


    Public Property PredictEffortStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.PredictEffort)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PredictEffort, value)
        End Set

    End Property



    Public Property DiscountStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Discount)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Discount, value)
        End Set

    End Property

    Public Property EquilibriumStepSizeStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EquilibriumStepSize)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EquilibriumStepSize, value)
        End Set

    End Property

    Public Property EquilMaxFishingRateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EquilMaxFishingRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EquilMaxFishingRate, value)
        End Set

    End Property

    Public Property NudgeCheckStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NudgeChecked)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NudgeChecked, value)
        End Set

    End Property

    Public Property NumberYearStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.EcoSimNYears)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcoSimNYears, value)
        End Set

    End Property

    Public Property NumStepAvgStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NumStepAvg)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NumStepAvg, value)
        End Set

    End Property

    Public Property NutFreeBasePropStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NutBaseFreeProp)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NutBaseFreeProp, value)
        End Set

    End Property

    Public Property NutForceFunctionNumberStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NutForceFunctionNumber)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NutForceFunctionNumber, value)
        End Set

    End Property


    Public Property SalinityForceFunctionNumberStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.SalinityForceFunctionNumber)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SalinityForceFunctionNumber, value)
        End Set

    End Property

    Public Property TemperatureForceFunctionNumberStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.TemperatureForceFunctionNumber)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.TemperatureForceFunctionNumber, value)
        End Set

    End Property

    Public Property NutPBMaxStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.NutPBMax)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.NutPBMax, value)
        End Set

    End Property

    Public Property RelaxationStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.Relaxation)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Relaxation, value)
        End Set

    End Property

    Public Property StepSizeStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.StepSize)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.StepSize, value)
        End Set

    End Property

    Public Property SystemRecoveryStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.SystemRecovery)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SystemRecovery, value)
        End Set

    End Property

    Public Property UseVarPQStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.UseVarPQ)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.UseVarPQ, value)
        End Set

    End Property

#End Region

End Class
