Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimFleetInput
    Inherits cCoreInputOutputBase

    Public Sub New(ByRef TheCore As cCore, ByVal iFleet As Integer)
        MyBase.New(TheCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        AllowValidation = False

        Me.m_dataType = eDataTypes.EcosimFleetInput
        Me.m_coreComponent = eCoreComponentType.EcoSim

        Me.Index = iFleet
        Me.DBID = TheCore.m_EcoPathData.FleetDBID(iFleet)

        'EPower
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.EPower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'PcapBase
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.PcapBase, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'CapDepreciate
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.CapDepreciate, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'CapBaseGrowth
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.CapBaseGrowth, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        AllowValidation = True

    End Sub

#Region " Variable via dot '.' operator "

    ''' <summary>
    ''' Effort response pow.fi
    ''' </summary>
    Public Property EPower() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EPower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EPower, value)
        End Set

    End Property

    ''' <summary>
    ''' capital depreciation rate
    ''' </summary>
    Public Property CapDepreciateRate() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.CapDepreciate))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    ''' <summary>
    ''' Initial effort / capital capacity
    ''' </summary>
    Public Property PcapBase() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.PcapBase))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    ''' <summary>
    ''' initial capitial growth
    ''' </summary>
    Public Property CapBaseGrowth() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.CapBaseGrowth))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property

#End Region ' Variable via dot '.' operator

#Region " Status via dot '.' operator "

    Public Property CapBaseGrowthStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CapBaseGrowth)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property

    Public Property CapDepreciateRateStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.CapDepreciate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    Public Property PcapBaseStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.PcapBase)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    Public Property EPowerStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EPower)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EPower, value)
        End Set
    End Property

#End Region ' Status via dot '.' operator

End Class
