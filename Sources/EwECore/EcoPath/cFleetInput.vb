Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Class to encapsulate a variables for a single Fishing Fleet Input
''' </summary>
''' <remarks></remarks>
Public Class cFleetInput
    Inherits cCoreInputOutputBase

    Private m_nGroups As Integer
    Private m_nDetritusGroups As Integer

#Region "Construction and Intialization"


    Friend Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        'stop the data validation for now
        'ToDo_jb add data validation for Fleets
        AllowValidation = False
        m_coreComponent = eCoreComponentType.EcoPath
        m_dataType = eDataTypes.FleetInput

        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FleetInput, eCoreComponentType.EcoPath, Index, cCore.NULL_VALUE)

        nGroups = m_core.nGroups
        nDetritusGroups = m_core.nDetritusGroups
        Me.DBID = DBID

        Dim val As cValue
        Dim meta As cVariableMetaData

        'For fisheries data validation see EwE5 frmInputData.vaInput_Change(...)

        'FixedCost
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.FixedCost, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'CPUECost
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.CPUECost, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'SailCost
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.SailCost, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ''PoolColor
        'meta = New cVariableMetaData(0, 255 << 16 + 255 << 8 + 255, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        'val = New cValue(New Integer, eVarNameFlags.PoolColor, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        'm_values.Add(val.varName, val)

        'arrayed values
        'Landings
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Landings, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'Discards
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.Discards, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'Off-vessel price (formerly known as MarketPrice)
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.OffVesselPrice, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'DiscardFate
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.DiscardFate, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'DiscardMortality
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.DiscardMortality, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.DiscardMortality))
        m_values.Add(val.varName, val)

    End Sub

#End Region

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
        Return Me.m_core.Set_MarketPrice_Flags(Me, False) And Me.m_core.Set_DiscardMort_Flags(Me, False)
    End Function

#End Region ' Overrides

#Region "Variables via dot (.) operator"

    Public Property nGroups() As Integer

        Get
            Return m_nGroups
        End Get

        Set(ByVal value As Integer)
            m_nGroups = value
        End Set

    End Property

    Public Property nDetritusGroups() As Integer

        Get
            Return m_nDetritusGroups
        End Get

        Set(ByVal value As Integer)
            m_nDetritusGroups = value
        End Set

    End Property

    Public Property FixedCost() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.FixedCost))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.FixedCost, value)
        End Set

    End Property

    Public Property SailCost() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.SailCost))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SailCost, value)
        End Set

    End Property

    Public Property CPUECost() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.CPUECost))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CPUECost, value)
        End Set

    End Property

    Public Property Landings(ByVal iGroup As Integer) As Single

        Get
            Return CSng(getVariable(eVarNameFlags.Landings, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Landings, value, iGroup)
        End Set

    End Property

    Public Property Landings() As Single()

        Get
            Return DirectCast(getVariable(eVarNameFlags.Landings), Single())
        End Get

        Set(ByVal value() As Single)
            SetVariable(eVarNameFlags.Landings, value)
        End Set

    End Property

    'Public Property PoolColor() As Integer
    '    Get
    '        Return CInt(GetVariable(eVarNameFlags.PoolColor))
    '    End Get
    '    Set(ByVal value As Integer)
    '        SetVariable(eVarNameFlags.PoolColor, value)
    '    End Set
    'End Property

    'Public Property PoolColorArgb() As System.Drawing.Color
    '    Get
    '        Dim iColor As Integer = Me.PoolColor
    '        Return Drawing.Color.FromArgb(255, (iColor >> 16) And &HFF, (iColor >> 8) And &HFF, iColor And &HFF)
    '    End Get
    '    Set(ByVal value As System.Drawing.Color)
    '        Me.PoolColor = (value.R << 16) + (value.G << 8) + value.B
    '    End Set
    'End Property

#Region "Indexed Variables"

    Public Property OffVesselPrice(ByVal iGroup As Integer) As Single

        Get
            Return CSng(getVariable(eVarNameFlags.OffVesselPrice, iGroup))
        End Get
        Set(ByVal value As Single)
            setVariable(eVarNameFlags.OffVesselPrice, value, iGroup)
        End Set

    End Property


    Public Property OffVesselPrice() As Single()

        Get
            Return DirectCast(getVariable(eVarNameFlags.OffVesselPrice), Single())
        End Get
        Set(ByVal value() As Single)
            setVariable(eVarNameFlags.OffVesselPrice, value)
        End Set

    End Property



    Public Property Discards(ByVal iGroup As Integer) As Single

        Get
            Return CSng(getVariable(eVarNameFlags.Discards, iGroup))
        End Get
        Set(ByVal value As Single)
            setVariable(eVarNameFlags.Discards, value, iGroup)
        End Set

    End Property

    Public Property Discards() As Single()

        Get
            Return DirectCast(getVariable(eVarNameFlags.Discards), Single())
        End Get
        Set(ByVal value() As Single)
            setVariable(eVarNameFlags.Discards, value)
        End Set

    End Property

    Public Property DiscardFate(ByVal iGroup As Integer) As Single

        Get
            Return CSng(getVariable(eVarNameFlags.DiscardFate, iGroup))
        End Get
        Set(ByVal value As Single)
            setVariable(eVarNameFlags.DiscardFate, value, iGroup)
        End Set

    End Property

    Public Property DiscardMortality(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.DiscardMortality, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.DiscardMortality, value, iGroup)
        End Set
    End Property

#End Region

    Public Property CPUECostStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.CPUECost)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CPUECost, value)
        End Set

    End Property

    Public Property DiscardFateStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.DiscardFate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DiscardFate, value)
        End Set

    End Property

    Public Property DiscardsStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Discards)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Discards, value)
        End Set

    End Property

    Public Property FixedCostStatus() As eStatusFlags
        Get
            Return getStatus(eVarNameFlags.FixedCost)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.FixedCost, value)
        End Set

    End Property

    Public Property iFleetStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Index)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Index, value)
        End Set

    End Property

    Public Property LandingsStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.Landings)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            setStatus(eVarNameFlags.Landings, value)
        End Set

    End Property

    Public Property OffVesselPriceStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.OffVesselPrice)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.OffVesselPrice, value)
        End Set

    End Property

    Public Property SailCostStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.SailCost)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.SailCost, value)
        End Set

    End Property

    Public Property DiscardMortalityStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.DiscardMortality, iGroup)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.DiscardMortality, value, iGroup)
        End Set
    End Property

#End Region

End Class
