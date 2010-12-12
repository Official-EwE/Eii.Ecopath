Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerGroupInput
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Friend Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        Try

            Me.DBID = iDBID
            Me.m_dataType = eDataTypes.EcotracerGroupInput
            Me.m_coreComponent = eCoreComponentType.Ecotracer

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' CZero
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CZero, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CZero))
            Me.m_values.Add(val.varName, val)

            ' CImmig
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CImmig, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CImmig))
            Me.m_values.Add(val.varName, val)

            ' CEnvironment
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CEnvironment, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CEnvironment))
            Me.m_values.Add(val.varName, val)

            ' CDecay
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CDecay, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CDecay))
            Me.m_values.Add(val.varName, val)

            ' CExcretionRate
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.CExcretionRate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.CExcretionRate))
            Me.m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcotracerScenarioGroup.")
            cLog.Write(Me.ToString & ".New Error creating new cEcotracerScenarioGroup. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variable via dot(.) operator"

    Public Property CZero() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.CZero), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CZero, value)
        End Set
    End Property

    Public Property CImmig() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.CImmig), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CImmig, value)
        End Set
    End Property

    Public Property CEnvironment() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.CEnvironment), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CEnvironment, value)
        End Set
    End Property

    Public Property CDecay() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.CDecay), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CDecay, value)
        End Set
    End Property

    Public Property CExcretionRate() As Single
        Get
            Return CType(GetVariable(eVarNameFlags.CExcretionRate), Single)
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CExcretionRate, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property CZeroStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CZero)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CZero, value)
        End Set

    End Property

    Public Property CImmigStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CImmig)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CImmig, value)
        End Set

    End Property

    Public Property CEnvironmentStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CEnvironment)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CEnvironment, value)
        End Set

    End Property

    Public Property CDecayStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CDecay)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CDecay, value)
        End Set

    End Property

    Public Property CExcretionRateStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CExcretionRate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CExcretionRate, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
