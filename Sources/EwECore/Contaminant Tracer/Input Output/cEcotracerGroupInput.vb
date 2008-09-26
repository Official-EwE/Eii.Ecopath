'==============================================================================
'
' $Log: cEcotracerGroupInput.vb,v $
' Revision 1.1  2008/09/26 07:30:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/05/29 22:22:45  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.2  2008/01/06 11:00:12  jeroens
' * Changed Excretion rate value upper limit to 1
'
' Revision 1.1  2007/12/08 00:56:25  jeroens
' * Renamed file
'
' Revision 1.3  2007/11/26 02:07:19  jeroens
' + Added CExcretionRate + Status
'
' Revision 1.2  2007/11/25 02:14:49  jeroens
' * Set correct message source, datatype
'
' Revision 1.1  2007/11/25 00:33:08  jeroens
' Initial version
'
'==============================================================================

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
            Me.m_DataType = eDataTypes.EcotracerGroupInput
            Me.m_messageSource = eMessageSource.Ecotracer

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimScenario, eMessageSource.EcoSim, Index, cCore.NULL_VALUE)

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
