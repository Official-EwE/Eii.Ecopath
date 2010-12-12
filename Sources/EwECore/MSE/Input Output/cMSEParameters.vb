Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Namespace MSE

    Public Enum eAssessmentMethods
        Exact = 0
        CatchEstmBio = 1
        DirectExploitation = 2
    End Enum

    Public Class cMSEParameters
        Inherits cCoreInputOutputBase

#Region "Construction"

        Public Sub New(ByRef theCore As cCore)
            MyBase.New(theCore)

            Me.AllowValidation = False
            Me.DBID = cCore.NULL_VALUE
            Me.m_dataType = eDataTypes.MSEParameters
            Me.m_coreComponent = eCoreComponentType.MSE
            AllowValidation = False

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

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
            val = New cValue(New Integer, eVarNameFlags.MSEAssessMethod, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEAssessMethod))
            val.Stored = False
            m_values.Add(val.varName, val)

            'Kalman gain
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEKalmanGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEKalmanGain))
            val.Stored = False
            m_values.Add(val.varName, val)

            'Forcast Gain
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEForcastGain))
            val.Stored = False
            m_values.Add(val.varName, val)

            'Assess Power
            meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.MSEAssessPower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEAssessPower))
            val.Stored = False
            m_values.Add(val.varName, val)

            'nTrials
            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Integer, eVarNameFlags.MSENTrials, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSENTrials))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSEUseEconomicPlugin, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEUseEconomicPlugin))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSEPredictEffort, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEPredictEffort))
            val.Stored = False
            m_values.Add(val.varName, val)

            'Regualtory Mode
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eMSERegulationMode)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MSERegulatoryMode, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEEffortSource))
            val.Stored = False
            m_values.Add(val.varName, val)


            'Effort Mode
            meta = New cVariableMetaData(0, System.Enum.GetValues(GetType(eMSEEffortSource)).Length, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MSEEffortSource, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEEffortSource))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSEStop, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEStop))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSESave, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSESave))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSYRunSilent, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSYRunSilent))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.MSYEvalValue, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSYEvalValue))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Integer, eVarNameFlags.MSYStartTime, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSYStartTime))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Integer, eVarNameFlags.MSEStartYear, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEStartYear))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Integer, eVarNameFlags.MSEResultsStartYear, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEResultsStartYear))
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(1, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.MSEResultsEndYear, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEResultsEndYear))
            val.Stored = False
            m_values.Add(val.varName, val)

            ResetStatusFlags()
            AllowValidation = True

        End Sub

#End Region

#Region "Public Properties"

        Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
            If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
            Me.m_core.Set_EconomicAvailable_Flags(Me, eVarNameFlags.MSEUseEconomicPlugin)
            Return True
        End Function

        Public Property AssessmentMethod() As eAssessmentMethods
            Get
                Return DirectCast(GetVariable(eVarNameFlags.MSEAssessMethod), eAssessmentMethods)
            End Get

            Set(ByVal value As eAssessmentMethods)
                SetVariable(eVarNameFlags.MSEAssessMethod, value)
            End Set
        End Property


        Public Property ForcastGain() As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSEForcastGain))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property


        Public Property KalmanGain() As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSEKalmanGain))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEKalmanGain, value)
            End Set
        End Property


        Public Property AssessPower() As Single
            Get
                Return CType(GetVariable(eVarNameFlags.MSEAssessPower), Single)
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEAssessPower, value)
            End Set
        End Property

        Public Property NTrials() As Integer
            Get
                Return CType(GetVariable(eVarNameFlags.MSENTrials), Integer)
            End Get

            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.MSENTrials, value)
            End Set
        End Property

        Public Property UseEconomicPlugin() As Boolean
            Get
                Return CBool(GetVariable(eVarNameFlags.MSEUseEconomicPlugin))
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property StopRun() As Boolean
            Get
                Return CBool(GetVariable(eVarNameFlags.MSEStop))
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSEStop, value)
            End Set
        End Property

        Public Property Save() As Boolean
            Get
                Return CBool(GetVariable(eVarNameFlags.MSESave))
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSESave, value)
            End Set
        End Property

        Public Property MSYStartTimeIndex() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.MSYStartTime))
            End Get

            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.MSYStartTime, value)
            End Set
        End Property

        Public Property MSYRunSilent() As Boolean
            Get
                Return CBool(GetVariable(eVarNameFlags.MSYRunSilent))
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSYRunSilent, value)
            End Set
        End Property

        Public Property MSYEvaluateValue() As Boolean
            Get
                Return CBool(GetVariable(eVarNameFlags.MSYEvalValue))
            End Get

            Set(ByVal value As Boolean)
                SetVariable(eVarNameFlags.MSYEvalValue, value)
            End Set
        End Property

        Public Property MSEStartYear() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.MSEStartYear))
            End Get

            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.MSEStartYear, value)
            End Set
        End Property

        ''' <summary>
        ''' NOT IMPLEMENTED 
        ''' </summary>
        Public Property MSEResultsStartYear() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.MSEResultsStartYear))
            End Get

            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.MSEResultsStartYear, value)
            End Set
        End Property

        ''' <summary>
        '''  NOT IMPLEMENTED 
        ''' </summary>
        Public Property MSEResultsEndYear() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.MSEResultsEndYear))
            End Get

            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.MSEResultsEndYear, value)
            End Set
        End Property

#End Region

#Region "Status Properties"


    Public Property RegulatoryMode() As eMSERegulationMode
        Get
            Return DirectCast(GetVariable(eVarNameFlags.MSERegulatoryMode), eMSERegulationMode)
        End Get
            Set(ByVal value As eMSERegulationMode)
                SetVariable(eVarNameFlags.MSERegulatoryMode, value)
            End Set
        End Property



    Public Property EffortSource() As eMSEEffortSource
        Get
            Return DirectCast(GetVariable(eVarNameFlags.MSEEffortSource), eMSEEffortSource)
        End Get

        Set(ByVal value As eMSEEffortSource)
            SetVariable(eVarNameFlags.MSEEffortSource, value)
        End Set
    End Property


        Public Property StopRunStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEUseEconomicPlugin)
            End Get


            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property UseEconomicPluginStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEUseEconomicPlugin)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEUseEconomicPlugin, value)
            End Set
        End Property

        Public Property ForcastGainStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEForcastGain)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEForcastGain, value)
            End Set
        End Property

        Public Property AssessPowerStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEAssessPower)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEAssessPower, value)
            End Set
        End Property

        Public Property NTrialsStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSENTrials)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSENTrials, value)
            End Set
        End Property

        Public Property AssessmentMethodStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEAssessMethod)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEAssessMethod, value)
            End Set
        End Property


        Public Property MSEResultsStartYearStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEResultsStartYear)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEResultsStartYear, value)
            End Set
        End Property


        Public Property MSEResultsEndYearStatus() As eStatusFlags
            Get
                Return GetStatus(eVarNameFlags.MSEResultsEndYear)
            End Get

            Set(ByVal value As eStatusFlags)
                SetStatus(eVarNameFlags.MSEResultsEndYear, value)
            End Set
        End Property

#End Region

    End Class

End Namespace
