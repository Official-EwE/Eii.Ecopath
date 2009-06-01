'==============================================================================
'
' $Log: cMSEParameters.vb,v $
' Revision 1.6  2009/06/01 17:07:38  joeb
' MSE debugging
'
' Revision 1.5  2009/05/26 22:02:34  jeroens
' EconData availability variable value and status obtained from plug-in
'
' Revision 1.4  2009/05/26 20:19:41  jeroens
' Variables no longer Stored
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Enum eAssessmentMethods
    Exact = 0
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
        val.Stored = False
        m_values.Add(val.varName, val)

        'Kalman gain
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.MSEKalmanGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEKalmanGain))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Forcast Gain
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.MSEForcastGain, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEForcastGain))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Assess Power
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.MSEAssessPower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEAssessPower))
        val.Stored = False
        m_values.Add(val.varName, val)

        'nTrials
        meta = New cVariableMetaData(1, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.MSENTrials, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSENTrials))
        val.Stored = False
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEUseEconomicPlugin, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEUseEconomicPlugin))
        val.Stored = False
        m_values.Add(val.varName, val)

        ResetStatusFlags()
        AllowValidation = True

    End Sub

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

    Public Property UseEconomicPlugin() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEUseEconomicPlugin))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEUseEconomicPlugin, value)
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