' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cEcosimFleetInput
    Inherits cCoreInputOutputBase

    Public Sub New(core As cCore, iFleet As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.m_dataType = eDataTypes.EcosimFleetInput
        Me.m_coreComponent = eCoreComponentType.Ecosim

        Me.AllowValidation = False

        Me.Index = iFleet
        Me.DBID = simdata.FleetDBID(iFleet)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        'EPower
        val = New cValue(core, New Single, eVarNameFlags.EPower, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'PcapBase
        val = New cValue(core, New Single, eVarNameFlags.PcapBase, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'CapDepreciate
        val = New cValue(core, New Single, eVarNameFlags.CapDepreciate, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'CapBaseGrowth
        val = New cValue(core, New Single, eVarNameFlags.CapBaseGrowth, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' FleetEffortConversion
        val = New cValue(core, New Single, eVarNameFlags.FleetEffortConversion, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimRelQ, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

#Region " Variable via dot '.' operator "

    ''' <summary>
    ''' Effort response pow.fi
    ''' </summary>
    Public Property EPower() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EPower))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EPower, value)
        End Set

    End Property

    ''' <summary>
    ''' capital depreciation rate
    ''' </summary>
    Public Property CapDepreciateRate() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CapDepreciate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    ''' <summary>
    ''' Initial effort / capital capacity
    ''' </summary>
    Public Property PcapBase() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PcapBase))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    ''' <summary>
    ''' initial capitial growth
    ''' </summary>
    Public Property CapBaseGrowth() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CapBaseGrowth))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property

    ''' <summary>
    ''' Effort conversion factor used for summing maps into a single Effort value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EffortConversionFactor() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FleetEffortConversion))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.FleetEffortConversion, value)
        End Set
    End Property

    ''' <summary>
    ''' Start-up catchability
    ''' </summary>
    Public Property RelQ(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimRelQ, iGroup))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimRelQ, value, iGroup)
        End Set
    End Property

#End Region ' Variable via dot '.' operator

#Region " Status via dot '.' operator "

    Public Property CapBaseGrowthStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CapBaseGrowth)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property

    Public Property CapDepreciateRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CapDepreciate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    Public Property PcapBaseStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PcapBase)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    Public Property EPowerStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EPower)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EPower, value)
        End Set
    End Property

    Public Property EffortConversionFactorStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.FleetEffortConversion)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.FleetEffortConversion, value)
        End Set
    End Property

    ''' <summary>
    ''' Start-up catchability
    ''' </summary>
    Public Property RelQStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimRelQ, iGroup)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimRelQ, value, iGroup)
        End Set
    End Property

#End Region ' Status via dot '.' operator

End Class
