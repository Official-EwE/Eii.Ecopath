Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core


Public Class cEcosimFleetOutput
    Inherits cCoreInputOutputBase

    Public Sub New(ByRef TheCore As cCore, ByVal iFleet As Integer)
        MyBase.New(TheCore)

        Dim val As cValue

        Me.m_DataType = eDataTypes.EcosimFleetOutput
        Me.Index = iFleet
        Me.DBID = TheCore.m_EcoPathData.FleetDBID(iFleet)

        'no validators
        'Catch biomass
        val = New cValue(0, eVarNameFlags.EcosimFleetCatchStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcosimFleetCatchEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Value
        val = New cValue(0, eVarNameFlags.EcosimFleetValueStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcosimFleetValueEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Cost
        val = New cValue(0, eVarNameFlags.EcosimFleetCostStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcosimFleetCostEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Effort
        val = New cValue(0, eVarNameFlags.EcosimFleetEffort, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)


        'Profit
        val = New cValue(0, eVarNameFlags.EcosimFleetProfit, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Jobs
        val = New cValue(0, eVarNameFlags.EcosimFleetJobs, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)


    End Sub

#Region "Variable via dot '.' operator"



    Public Property Profit() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetProfit))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetProfit, value)
        End Set
    End Property


    Public Property Jobs() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetJobs))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetJobs, value)
        End Set
    End Property




    Public Property CatchStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetCatchStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetCatchEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetCatchEnd, value)
        End Set
    End Property


    Public Property ValueStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetValueStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetValueEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetCostStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetCostStart, value)
        End Set
    End Property

    Public Property CostEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetCostEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetCostEnd, value)
        End Set
    End Property

    Public Property Effort() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFleetEffort))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFleetEffort, value)
        End Set
    End Property

#End Region

#Region "Status via dot '.' operator"

    Public Property CatchStartStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetCatchStart)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEndStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetCatchEnd)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetCatchEnd, value)
        End Set
    End Property

    Public Property ValueStartStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetValueStart)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEndStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetValueEnd)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStartStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetCostStart)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetCostStart, value)
        End Set
    End Property

    Public Property CostEndStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetCostEnd)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetCostEnd, value)
        End Set
    End Property

    Public Property EffortStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimFleetEffort)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimFleetEffort, value)
        End Set
    End Property

#End Region


End Class
