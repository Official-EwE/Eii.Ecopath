Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Enum eAssessmentMethods
    CatchEstmBio = 1
    DirectExploitation = 2
End Enum

Public Class cMSEParameters
    Inherits cCoreInputOutputBase

    'ToDo_jb cMSEParameters get the parameters from EwE5

    Public Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Me.AllowValidation = False
        Me.DBID = cCore.NULL_VALUE
        Me.m_dataType = eDataTypes.MSEParameters
        Me.m_coreComponent = eCoreComponentType.MSE
        AllowValidation = False

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, Me.m_dataType, _
                                                     Me.m_coreComponent, Index, cCore.NULL_VALUE)

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
        m_values.Add(val.varName, val)

        'Kalman gain
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEKalmanGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEKalmanGain))
        m_values.Add(val.varName, val)


        'Forcast Gain
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEForcastGain))
        m_values.Add(val.varName, val)

        'Assess Power
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.MSEAssessPower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEAssessPower))
        m_values.Add(val.varName, val)

        'nTrials
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.MSENTrials, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSENTrials))
        m_values.Add(val.varName, val)

        ResetStatusFlags()
        AllowValidation = True

    End Sub

    Public Property AssessmentMethod() As eAssessmentMethods
        Get
            Return DirectCast(GetVariable(eVarNameFlags.MSEAssessMethod), eAssessmentMethods)
        End Get

        Set(ByVal value As eAssessmentMethods)
            SetVariable(eVarNameFlags.MSEAssessMethod, value)
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

    Public Property ForcastGain() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MSEForcastGain))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MSEForcastGain, value)
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



    Public Property AssessmentMethodStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MSEAssessMethod)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSEAssessMethod, value)
        End Set
    End Property


    Public Property KalmanGainStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MSEKalmanGain)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MSEKalmanGain, value)
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
End Class