' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cEcosimFleetOutput
    Inherits cCoreInputOutputBase

    'dictionary of vars and wrappers that directly access the core data
    Private m_coreData As New Dictionary(Of eVarNameFlags, IResultsWrapper)
    Private m_simData As cEcosimDatastructures

    Public Sub New(core As cCore, iFleet As Integer)
        MyBase.New(core)

        Dim val As cValue
        Me.m_simData = core.m_EcoSimData

        Me.m_dataType = eDataTypes.EcosimFleetOutput
        Me.Index = iFleet
        Me.DBID = core.m_EcoSimData.FleetDBID(iFleet)

        'no validators
        'Catch biomass
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetCatchStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcosimFleetCatchEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Value
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetValueStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcosimFleetValueEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Cost
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetCostStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcosimFleetCostEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Effort
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetEffort, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Profit
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetProfit, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Jobs
        val = New cValue(core, 0, eVarNameFlags.EcosimFleetJobs, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

    End Sub

    Public Sub Init()

        'the results arrays of ecosim are redim for each run
        'this means the reference to the results data is lost on each run 
        'so reset the reference
        Me.m_coreData.Clear()

        Me.m_coreData.Add(eVarNameFlags.EcosimFleetValueTime, New c2DResultsWrapper(Me.m_simData.ResultsSumValueByGear, Me.Index))
        Me.m_coreData.Add(eVarNameFlags.EcosimFleetCatchTime, New c2DResultsWrapper(Me.m_simData.ResultsSumCatchByGear, Me.Index))
        'ResultsSumCatchByGear

    End Sub

    Public Overrides Function GetVariable(VarName As eVarNameFlags, Optional iIndex1 As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = cCore.NULL_VALUE) As Object

        If Not Me.m_coreData.ContainsKey(VarName) Then
            'NOT in list of sim vars so get the value from the base class GetVariable(...)
            Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
        Else
            'Varname is access directly via the core data
            Return Me.m_coreData.Item(VarName).Value(iIndex1, iIndex2)
        End If

    End Function

#Region "Variable via dot '.' operator"

    Public Property ProfitSummary() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetProfit))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetProfit, value)
        End Set
    End Property

    Public Property JobsSummary() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetJobs))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetJobs, value)
        End Set
    End Property

    Public Property CatchStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetCatchStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetCatchEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetCatchEnd, value)
        End Set
    End Property

    Public Property ValueStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetValueStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetValueEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetValueEnd, value)
        End Set
    End Property

    Public Property CostStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetCostStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetCostStart, value)
        End Set
    End Property

    Public Property CostEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetCostEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetCostEnd, value)
        End Set
    End Property

    Public Property Effort() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetEffort))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimFleetEffort, value)
        End Set
    End Property

    Public ReadOnly Property Value(Time As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetValueTime, Time))
        End Get

    End Property

    Public ReadOnly Property CatchBiomass(Time As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimFleetCatchTime, Time))
        End Get

    End Property

#End Region

#Region "Status via dot '.' operator"

    Public Property CatchStartStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetCatchStart)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEndStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetCatchEnd)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetCatchEnd, value)
        End Set
    End Property

    Public Property ValueStartStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetValueStart)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEndStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetValueEnd)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetValueEnd, value)
        End Set
    End Property

    Public Property CostStartStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetCostStart)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetCostStart, value)
        End Set
    End Property

    Public Property CostEndStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetCostEnd)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetCostEnd, value)
        End Set
    End Property

    Public Property EffortStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimFleetEffort)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimFleetEffort, value)
        End Set
    End Property

#End Region

End Class
