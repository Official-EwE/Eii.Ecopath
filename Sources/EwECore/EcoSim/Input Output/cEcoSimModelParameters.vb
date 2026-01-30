' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



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

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcoSimModelParameters)()

#Region " Constructor "

    Public Sub New(core As cCore)
        MyBase.New(core)

        Try
            'no data validation at this time
            Me.AllowValidation = False
            Me.m_coreComponent = eCoreComponentType.Ecosim
            Me.m_dataType = eDataTypes.EcoSimModelParameter

            Dim val As cValue

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'StepSize
            val = New cValue(core, New Single, eVarNameFlags.StepSize, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Discount
            val = New cValue(core, New Single, eVarNameFlags.Discount, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'EquilibriumStepSize
            val = New cValue(core, New Single, eVarNameFlags.EquilibriumStepSize, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'EquilMaxFishingRate
            val = New cValue(core, New Single, eVarNameFlags.EquilMaxFishingRate, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'NumStepAvg
            val = New cValue(core, New Single, eVarNameFlags.NumStepAvg, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'NutBaseFreeProp
            val = New cValue(core, New Single, eVarNameFlags.NutBaseFreeProp, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'NutPBMax
            val = New cValue(core, New Single, eVarNameFlags.NutPBMax, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'SystemRecovery
            val = New cValue(core, New Single, eVarNameFlags.SystemRecovery, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'VulnerabilityCap
            val = New cValue(core, New Single, eVarNameFlags.VulnerabilityCap, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'ForagingTimeLowerLimit
            val = New cValue(core, New Single, eVarNameFlags.ForagingTimeLowerLimit, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'boolean
            'NudgeChecked
            val = New cValue(core, New Boolean, eVarNameFlags.NudgeChecked, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'UseVarPQ
            val = New cValue(core, New Boolean, eVarNameFlags.UseVarPQ, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'BiomassOn
            val = New cValue(core, New Boolean, eVarNameFlags.BiomassOn, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ''integers
            'NutForceFunctionNumber
            val = New cValue(core, New Integer, eVarNameFlags.NutForceFunctionNumber, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'EcoSimNYears max 1000 year?!
            val = New cValue(core, New Integer, eVarNameFlags.EcoSimNYears, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'start summary
            val = New cValue(core, New Single, eVarNameFlags.EcosimSumStart, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'end summary
            val = New cValue(core, New Single, eVarNameFlags.EcosimSumEnd, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'summary num time steps
            val = New cValue(core, New Integer, eVarNameFlags.EcosimSumNTimeSteps, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'Contaminant tracing
            val = New cValue(core, New Boolean, eVarNameFlags.ConSimOnEcoSim, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'PredictEffort
            val = New cValue(core, New Boolean, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'end summary
            val = New cValue(core, New Single, eVarNameFlags.EcosimSORWt, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = True
            val.AffectsRunState = True
            Me.m_values.Add(val.varName, val)

            '  Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            m_logger.LogError(ex, "cEcoSimModelParameters.New() Exception")

        End Try

    End Sub


#End Region

#Region "Mustoverride Method implementation for this class"

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean

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
            Return CInt(Me.GetVariable(eVarNameFlags.EcoSimNYears))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcoSimNYears, value)
        End Set

    End Property

    Public Property BiomassOn() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.BiomassOn))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.BiomassOn, value)
        End Set

    End Property

    Public Property Discount() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Discount))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Discount, value)
        End Set

    End Property

    Public Property EquilibriumStepSize() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EquilibriumStepSize))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EquilibriumStepSize, value)
        End Set

    End Property

    Public Property EquilMaxFishingRate() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EquilMaxFishingRate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EquilMaxFishingRate, value)
        End Set

    End Property

    Public Property NudgeChecked() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.NudgeChecked))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.NudgeChecked, value)
        End Set

    End Property

    'Public Property NumStepAvg() As Single

    '    Get
    '        Return CSng(GetVariable(eVarNameFlags.NumStepAvg))
    '    End Get

    '    Set(value As Single)
    '        SetVariable(eVarNameFlags.NumStepAvg, value)
    '    End Set

    'End Property

    Public Property NutBaseFreeProp() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NutBaseFreeProp))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.NutBaseFreeProp, value)
        End Set

    End Property

    Public Property NutForceFunctionNumber() As Integer

        Get
            Return CInt(Me.GetVariable(eVarNameFlags.NutForceFunctionNumber))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.NutForceFunctionNumber, value)
        End Set

    End Property

    Public Property NutPBMax() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NutPBMax))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.NutPBMax, value)
        End Set

    End Property

    Public Property VulnerabilityCap() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.VulnerabilityCap))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.VulnerabilityCap, value)
        End Set

    End Property

    Public Property StepSize() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.StepSize))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.StepSize, value)
        End Set

    End Property

    Public Property SystemRecovery() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SystemRecovery))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.SystemRecovery, value)
        End Set

    End Property

    Public Property UseVarPQ() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.UseVarPQ))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.UseVarPQ, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Compatibility flag. EwE up to version 6.4 limited the foraging time to 0.1, 
    ''' but this turned some ecosystems volaltile. The limit has been changed to
    ''' a min of 0.01. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ForagingTimeLowerLimit As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ForagingTimeLowerLimit))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.ForagingTimeLowerLimit, value)
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
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimSumStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimSumStart, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start of the last summary period (in years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EndSummaryTime() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimSumEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimSumEnd, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number to time steps to summarize the data over.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberSummaryTimeSteps() As Integer

        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcosimSumNTimeSteps))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcosimSumNTimeSteps, value)
        End Set

    End Property

    Public Property ContaminantTracing() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.ConSimOnEcoSim))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.ConSimOnEcoSim, value)
        End Set

    End Property

    Public Property PredictEffort() As Boolean

        Get
            Return CBool(Me.GetVariable(eVarNameFlags.PredictEffort))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.PredictEffort, value)
        End Set

    End Property

    'Public Property RegFeedBack() As Boolean

    '    Get
    '        Return CBool(GetVariable(eVarNameFlags.RegFeedBack))
    '    End Get

    '    Set(value As Boolean)
    '        SetVariable(eVarNameFlags.RegFeedBack, value)
    '    End Set

    'End Property

    Public Property SORWt() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimSORWt))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimSORWt, value)
        End Set

    End Property

#End Region

#Region "Status via dot (.) operator"

    Public Property BiomassOnStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.BiomassOn)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.BiomassOn, value)
        End Set

    End Property

    Public Property ContaminantTracingStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.ConSimOnEcoSim)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.ConSimOnEcoSim, value)
        End Set

    End Property


    Public Property PredictEffortStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PredictEffort)
        End Get

        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PredictEffort, value)
        End Set

    End Property



    Public Property DiscountStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Discount)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Discount, value)
        End Set

    End Property

    Public Property EquilibriumStepSizeStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EquilibriumStepSize)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EquilibriumStepSize, value)
        End Set

    End Property

    Public Property EquilMaxFishingRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EquilMaxFishingRate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EquilMaxFishingRate, value)
        End Set

    End Property

    Public Property NudgeCheckStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NudgeChecked)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NudgeChecked, value)
        End Set

    End Property

    Public Property NumberYearStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EcoSimNYears)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcoSimNYears, value)
        End Set

    End Property

    Public Property NumStepAvgStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NumStepAvg)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NumStepAvg, value)
        End Set

    End Property

    Public Property NutFreeBasePropStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NutBaseFreeProp)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NutBaseFreeProp, value)
        End Set

    End Property

    Public Property NutForceFunctionNumberStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NutForceFunctionNumber)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NutForceFunctionNumber, value)
        End Set

    End Property

    Public Property NutPBMaxStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NutPBMax)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NutPBMax, value)
        End Set

    End Property

    Public Property StepSizeStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.StepSize)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.StepSize, value)
        End Set

    End Property

    Public Property SystemRecoveryStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.SystemRecovery)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.SystemRecovery, value)
        End Set

    End Property

    Public Property UseVarPQStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.UseVarPQ)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.UseVarPQ, value)
        End Set

    End Property

    Public Property SORWtStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.EcosimSORWt)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimSORWt, value)
        End Set

    End Property

#End Region

End Class
