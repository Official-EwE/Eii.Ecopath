'==============================================================================
'
' $Log: cFleetInput.vb,v $
' Revision 1.2  2008/10/08 17:55:06  jeroens
' DiscardMortality about to be removed from Ecosim
'
' Revision 1.1  2008/09/26 07:30:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.24  2008/08/04 02:27:44  jeroens
' Renamed varname MarketPrice to OffVesselPrice
'
' Revision 1.23  2008/07/21 14:13:48  jeroens
' Disabled pool color
'
' Revision 1.22  2008/07/02 01:55:24  jeroens
' Added option to force status flag total reset (fixes bug 503)
'
' Revision 1.21  2008/05/29 22:22:43  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.20  2007/08/23 14:50:50  jeroens
' * Moved NonMarketValue from fleet to groupinput
'
' Revision 1.19  2007/06/15 00:03:22  jeroens
' + Uses Set_MarketPrice_Flags
'
' Revision 1.18  2007/05/22 13:24:26  jeroens
' * Nitty-gritty
'
' Revision 1.17  2007/04/12 20:26:23  jeroens
' + Added PoolColor
'
' Revision 1.16  2007/03/28 01:16:32  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.15  2007/03/08 03:24:58  jeroens
' * Dropped iFleet property, replaced its use by generic Index property
'
' Revision 1.14  2007/01/19 18:31:08  joeb
' Changes to cValueArray constructor
'
' Revision 1.13  2007/01/19 00:49:53  joeb
' Changes to cValueArray Constructor
'
' Revision 1.12  2006/12/14 23:32:59  jeroens
' * Updated variable definitions to metadata default value
'
' Revision 1.11  2006/09/19 00:08:52  jeroens
' * Fixed spelling error in operator constant
'
' Revision 1.10  2006/09/08 21:22:21  joeb
' Comments
'
' Revision 1.9  2006/08/22 19:25:04  joeb
' Rename cFleetInputs to cFleetInput
'
' Revision 1.8  2006/08/22 19:00:10  joeb
' Renamed cFleetInputs to cFleetInput
'
' Revision 1.7  2006/08/18 15:11:44  joeb
' Renamed ICoreInputOutput.CurrentStatus to ValidationStatus
'
' Revision 1.6  2006/07/20 14:08:30  joeb
' Validation using MetaData and Operator classes
'
' Revision 1.5  2006/07/13 19:10:32  joeb
' ICoreInputOutputBase uses a reference to the core instead of a delegates to communicate with the core.
'
' Revision 1.4  2006/07/12 16:10:11  jeroens
' - Reverted silly enum bit, sorry guys!
'
' Revision 1.3  2006/07/11 00:30:08  jeroens
' * Activated all present variables
'
' Revision 1.2  2006/07/10 18:44:46  jeroens
' + Added sec. indexes
'
' Revision 1.1  2006/07/07 11:32:16  jeroens
' * Renamed from cFleet
'
'==============================================================================

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
        m_messageSource = eMessageSource.EcoPath
        m_DataType = eDataTypes.FleetInput

        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FleetInput, eMessageSource.EcoPath, Index, cCore.NULL_VALUE)

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
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
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

    ''' <summary>
    ''' Effort response pow.fi
    ''' </summary>
    Public Property EPower() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.EPower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EPower, value)
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

    ''' <summary>
    ''' capital depreciation rate
    ''' </summary>
    Public Property CapDepreciateRate() As Single

        Get
            Return CSng(getVariable(eVarNameFlags.CapDepreciate))
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
            Return CSng(getVariable(eVarNameFlags.PcapBase))
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
            Return CSng(getVariable(eVarNameFlags.CapBaseGrowth))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.CapBaseGrowth, value)
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
            Return getStatus(eVarNameFlags.CapDepreciate)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

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

    Public Property EPowerStatus() As eStatusFlags
        Get
            Return getStatus(eVarNameFlags.EPower)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EPower, value)
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

    Public Property PcapBaseStatus() As eStatusFlags

        Get
            Return getStatus(eVarNameFlags.PcapBase)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PcapBase, value)
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
