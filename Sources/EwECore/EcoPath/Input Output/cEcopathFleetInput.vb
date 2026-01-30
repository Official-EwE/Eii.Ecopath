' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

''' <summary>
''' Class to encapsulate a variables for a single Fishing Fleet Input
''' </summary>
''' <remarks></remarks>
Public Class cEcopathFleetInput
    Inherits cCoreInputOutputBase

#Region " Construction and Intialization "

    Public Sub New(core As cCore, DBID As Integer, iIndex As Integer)
        MyBase.New(core)

        'stop the data validation for now
        'ToDo_jb add data validation for Fleets
        Me.AllowValidation = False
        Me.m_coreComponent = eCoreComponentType.Ecopath
        Me.m_dataType = eDataTypes.FleetInput

        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        Me.Index = iIndex
        Me.DBID = DBID

        Dim val As cValue

        'For fisheries data validation see EwE5 frmInputData.vaInput_Change(...)

        'FixedCost
        val = New cValue(core, New Single, eVarNameFlags.FixedCost, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'CPUECost
        val = New cValue(core, New Single, eVarNameFlags.EffortCost, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'SailCost
        val = New cValue(core, New Single, eVarNameFlags.SailCost, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Nominal effort
        val = New cValue(core, New Single, eVarNameFlags.NominalEffort, eStatusFlags.Null, eValueTypes.Sng)
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)

        'PoolColor
        val = New cValue(core, New Integer, eVarNameFlags.Color, eStatusFlags.Null, eValueTypes.Int)
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)

        'arrayed values
        'Landings
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Landings, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        'Discards
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.Discards, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        'Off-vessel price (formerly known as MarketPrice)
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.OffVesselPrice, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        'DiscardFate
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.DiscardFate, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        'DiscardMortality
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.DiscardMortality, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

    End Sub

#End Region

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        If Not MyBase.ResetStatusFlags(bForceReset) Then Return False
        Return Me.m_core.Set_OffVesselValue_Flags(Me, False) And Me.m_core.Set_DiscardMort_Flags(Me, False)
    End Function

#End Region ' Overrides

#Region "Variables via dot (.) operator"

    Public Property FixedCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FixedCost))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.FixedCost, value)
        End Set
    End Property

    Public Property SailCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SailCost))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.SailCost, value)
        End Set
    End Property

    Public Property EffortCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EffortCost))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EffortCost, value)
        End Set
    End Property

    Public Property NominalEffort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NominalEffort))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.NominalEffort, value)
        End Set
    End Property

    Public Property Landings(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Landings, iGroup))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Landings, value, iGroup)
        End Set
    End Property

    Public Property Landings() As Single()
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Landings), Single())
        End Get
        Set(value() As Single)
            Me.SetVariable(eVarNameFlags.Landings, value)
        End Set
    End Property

    Public Property PoolColor() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.Color))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.Color, value)
        End Set
    End Property

#Region "Indexed Variables"

    Public Property OffVesselValue(iGroup As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.OffVesselPrice, iGroup))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.OffVesselPrice, value, iGroup)
        End Set

    End Property

    <Obsolete("Use OffVesselValue instead")>
    Public Property OffVesselPrice() As Single()

        Get
            Return Me.OffVesselValue()
        End Get
        Set(value() As Single)
            Me.OffVesselValue() = value
        End Set

    End Property

    Public Property OffVesselValue() As Single()

        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.OffVesselPrice), Single())
        End Get
        Set(value() As Single)
            Me.SetVariable(eVarNameFlags.OffVesselPrice, value)
        End Set

    End Property

    Public Property Discards(iGroup As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Discards, iGroup))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Discards, value, iGroup)
        End Set

    End Property

    Public Property Discards() As Single()

        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.Discards), Single())
        End Get
        Set(value() As Single)
            Me.SetVariable(eVarNameFlags.Discards, value)
        End Set

    End Property

    Public Property DiscardFate(iGroup As Integer) As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.DiscardFate, iGroup))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DiscardFate, value, iGroup)
        End Set

    End Property

    Public Property DiscardMortality(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.DiscardMortality, iGroup))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.DiscardMortality, value, iGroup)
        End Set
    End Property

#End Region

    Public Property NominalEffortStatus As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.NominalEffort)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NominalEffort, value)
        End Set
    End Property

    Public Property EffortCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EffortCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EffortCost, value)
        End Set
    End Property

    Public Property DiscardFateStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.DiscardFate)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DiscardFate, value)
        End Set
    End Property

    Public Property DiscardsStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.Discards)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Discards, value)
        End Set
    End Property

    Public Property FixedCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.FixedCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.FixedCost, value)
        End Set
    End Property

    Public Property iFleetStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.Index)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Index, value)
        End Set
    End Property

    Public Property LandingsStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.Landings)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Landings, value)
        End Set
    End Property

    <Obsolete("Use OffVesselValueStatus instead")>
    Public Property OffVesselPriceStatus() As eStatusFlags
        Get
            Return Me.OffVesselValueStatus
        End Get
        Friend Set(value As eStatusFlags)
            Me.OffVesselValueStatus = value
        End Set
    End Property

    Public Property OffVesselValueStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.OffVesselPrice)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.OffVesselPrice, value)
        End Set
    End Property

    Public Property SailCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.SailCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.SailCost, value)
        End Set
    End Property

    Public Property DiscardMortalityStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.DiscardMortality, iGroup)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.DiscardMortality, value, iGroup)
        End Set
    End Property

#End Region

End Class

#Region "Deprecated"

<Obsolete("Use cEcopathFleetInput instead")>
Public Class cFleetInput
    Inherits cEcopathFleetInput

    Public Sub New(TheCore As cCore, DBID As Integer, iIndex As Integer)
        MyBase.New(TheCore, DBID, iIndex)
    End Sub

End Class

#End Region ' Deprecated
