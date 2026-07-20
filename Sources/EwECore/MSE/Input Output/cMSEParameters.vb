' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Namespace MSE

    Public Enum eModelTypes
        Ecosim
        EcoSpace
    End Enum


    Public Enum eAssessmentMethods
        Exact = 0
        CatchEstmBio = 1
        DirectExploitation = 2
    End Enum

    Public Class cMSEParameters
        Inherits cCoreInputOutputBase

#Region "Construction"

        Public Sub New(core As cCore)
            MyBase.New(core)

            Me.AllowValidation = False
            Me.DBID = cCore.NULL_VALUE
            Me.m_dataType = eDataTypes.MSEParameters
            Me.m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'fishing assesment methods
            'Catch estimated biomass
            'Direct explotation rate

            'Kalman gain
            'stock forcast gain g
            'survy vs. biomass power param
            'ntrials

            Dim val As cValue
            Dim meta As cVariableMetaData

            'Assessment method
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eAssessmentMethods)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSEAssessMethod, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEAssessMethod))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            'Forcast Gain
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEForcastGain))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'Assess Power
            meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Single, eVarNameFlags.MSEAssessPower, eStatusFlags.Null, eValueTypes.Sng, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEAssessPower))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            'nTrials
            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Integer, eVarNameFlags.MSENTrials, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSENTrials))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(core, New Boolean, eVarNameFlags.MSEUseEconomicPlugin, eStatusFlags.Null, eValueTypes.Bool, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEUseEconomicPlugin))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(core, New Boolean, eVarNameFlags.MSEPredictEffort, eStatusFlags.Null, eValueTypes.Bool, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEPredictEffort))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'Regualtory Mode
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eMSERegulationMode)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSERegulatoryMode, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEEffortSource))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'Effort Mode
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eMSEEffortSource)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSEEffortSource, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEEffortSource))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'jb 30-May-2011 StopRun is not a property it is a Method (stop the run) so it has been moved to the MSEManager.StopRun()
            'meta = New cVariableMetadata( )
            'val = New cValue(core, New Boolean, eVarNameFlags.MSEStop, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEStop))
            'val.Stored = False
            'm_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(core, New Boolean, eVarNameFlags.MSYRunSilent, eStatusFlags.Null, eValueTypes.Bool, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSYRunSilent))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(core, New Boolean, eVarNameFlags.MSYEvalValue, eStatusFlags.Null, eValueTypes.Bool, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSYEvalValue))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Integer, eVarNameFlags.MSYStartTime, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSYStartTime))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Integer, eVarNameFlags.MSEStartYear, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEStartYear))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(core, New Integer, eVarNameFlags.MSEResultsStartYear, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEResultsStartYear))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSEResultsEndYear, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEResultsEndYear))
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSEMaxEffort, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEMaxEffort))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            'UseLP
            meta = New cVariableMetaData(False)
            val = New cValue(core, New Boolean, eVarNameFlags.MSELPSolution, eStatusFlags.Null, eValueTypes.Bool, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSELPSolution))
            val.Stored = False
            Me.m_values.Add(val.varName, val)


            'MSEModelType
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eModelTypes)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(core, New Integer, eVarNameFlags.MSEModelType, eStatusFlags.Null, eValueTypes.Int, meta, Me.m_core.m_validators.getValidator(eVarNameFlags.MSEModelType))
            val.Stored = True
            Me.m_values.Add(val.varName, val)

            Me.ResetStatusFlags()
            Me.AllowValidation = True

        End Sub

#End Region

#Region "Public Properties"

        Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
            If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
            Me.m_core.Set_EconomicAvailable_Flags(Me, eVarNameFlags.MSEUseEconomicPlugin)
            Return True
        End Function

        Public Property AssessmentMethod() As eAssessmentMethods
            Get
                Return DirectCast(Me.GetVariable(eVarNameFlags.MSEAssessMethod), eAssessmentMethods)
            End Get

            Set(value As eAssessmentMethods)
                Me.SetVariable(eVarNameFlags.MSEAssessMethod, value)
            End Set
        End Property


        Public Property ModelType() As eModelTypes
            Get
                Return DirectCast(Me.GetVariable(eVarNameFlags.MSEModelType), eModelTypes)
            End Get

            Set(value As eModelTypes)
                Me.SetVariable(eVarNameFlags.MSEModelType, value)
            End Set
        End Property



        Public Property ForcastGain() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEForcastGain))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property

        Public Property AssessPower() As Single
            Get
                Return CType(Me.GetVariable(eVarNameFlags.MSEAssessPower), Single)
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEAssessPower, value)
            End Set
        End Property

        Public Property NTrials() As Integer
            Get
                Return CType(Me.GetVariable(eVarNameFlags.MSENTrials), Integer)
            End Get

            Set(value As Integer)
                Me.SetVariable(eVarNameFlags.MSENTrials, value)
            End Set
        End Property

        Public Property UseEconomicPlugin() As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSEUseEconomicPlugin))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property UseLPSolution() As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSELPSolution))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSELPSolution, value)
            End Set
        End Property

        'Public Property StopRun() As Boolean
        '    Get
        '        Return CBool(GetVariable(eVarNameFlags.MSEStop))
        '    End Get

        '    Set(value As Boolean)
        '        SetVariable(eVarNameFlags.MSEStop, value)
        '    End Set
        'End Property

        Public Property MSYStartTimeIndex() As Integer
            Get
                Return CInt(Me.GetVariable(eVarNameFlags.MSYStartTime))
            End Get

            Set(value As Integer)
                Me.SetVariable(eVarNameFlags.MSYStartTime, value)
            End Set
        End Property

        Public Property MSYRunSilent() As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSYRunSilent))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSYRunSilent, value)
            End Set
        End Property

        Public Property MSYEvaluateValue() As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSYEvalValue))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSYEvalValue, value)
            End Set
        End Property

        Public Property MSEStartYear() As Integer
            Get
                Return CInt(Me.GetVariable(eVarNameFlags.MSEStartYear))
            End Get

            Set(value As Integer)
                Me.SetVariable(eVarNameFlags.MSEStartYear, value)
            End Set
        End Property

        ''' <summary>
        ''' NOT IMPLEMENTED 
        ''' </summary>
        Public Property MSEResultsStartYear() As Integer
            Get
                Return CInt(Me.GetVariable(eVarNameFlags.MSEResultsStartYear))
            End Get

            Set(value As Integer)
                Me.SetVariable(eVarNameFlags.MSEResultsStartYear, value)
            End Set
        End Property

        ''' <summary>
        '''  NOT IMPLEMENTED 
        ''' </summary>
        Public Property MSEResultsEndYear() As Integer
            Get
                Return CInt(Me.GetVariable(eVarNameFlags.MSEResultsEndYear))
            End Get

            Set(value As Integer)
                Me.SetVariable(eVarNameFlags.MSEResultsEndYear, value)
            End Set
        End Property

        Public Property MaxEffort As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEMaxEffort))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEMaxEffort, value)
            End Set
        End Property

        Public Property LPSolution As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSELPSolution))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSELPSolution, value)
            End Set
        End Property

#End Region

#Region "Status Properties"

        Public Property RegulatoryMode() As eMSERegulationMode
            Get
                Return DirectCast(Me.GetVariable(eVarNameFlags.MSERegulatoryMode), eMSERegulationMode)
            End Get
            Set(value As eMSERegulationMode)
                Me.SetVariable(eVarNameFlags.MSERegulatoryMode, value)
            End Set
        End Property

        Public Property EffortSource() As eMSEEffortSource
            Get
                Return DirectCast(Me.GetVariable(eVarNameFlags.MSEEffortSource), eMSEEffortSource)
            End Get

            Set(value As eMSEEffortSource)
                Me.SetVariable(eVarNameFlags.MSEEffortSource, value)
            End Set
        End Property

        Public Property StopRunStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEUseEconomicPlugin)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property UseEconomicPluginStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEUseEconomicPlugin)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property ForcastGainStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEForcastGain)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property

        Public Property AssessPowerStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEAssessPower)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEAssessPower, value)
            End Set
        End Property

        Public Property NTrialsStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSENTrials)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSENTrials, value)
            End Set
        End Property

        Public Property AssessmentMethodStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEAssessMethod)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEAssessMethod, value)
            End Set
        End Property

        Public Property MSEResultsStartYearStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEResultsStartYear)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEResultsStartYear, value)
            End Set
        End Property

        Public Property MSEResultsEndYearStatus() As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.MSEResultsEndYear)
            End Get

            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.MSEResultsEndYear, value)
            End Set
        End Property

#End Region

    End Class

End Namespace
