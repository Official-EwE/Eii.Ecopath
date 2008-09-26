Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceFleetSummary
    Inherits cCoreInputOutputBase

    Public Sub New(ByRef TheCore As cCore, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue


        Me.Index = iGroup
        Me.DBID = iGroup '????
        'no validators
        'Catch biomass
        val = New cValue(0, eVarNameFlags.EcospaceFleetCatchStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcospaceFleetCatchEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Value
        val = New cValue(0, eVarNameFlags.EcospaceFleetValueStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcospaceFleetValueEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'Cost
        val = New cValue(0, eVarNameFlags.EcospaceFleetCostStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcospaceFleetCostEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)


    End Sub

#Region "Variable via dot '.' operator"


    Public Property CatchStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetCatchStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetCatchEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetCatchEnd, value)
        End Set
    End Property


    Public Property ValueStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetValueStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetValueEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetCostStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetCostStart, value)
        End Set
    End Property

    Public Property CostEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetCostEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetCostEnd, value)
        End Set
    End Property

#End Region

#Region "Status via dot '.' operator"

    Public Property CatchStartStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetCatchStart), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEndStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetCatchEnd), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetCatchEnd, value)
        End Set
    End Property

    Public Property ValueStartStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetValueStart), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEndStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetValueEnd), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStartStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetCostStart), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetCostStart, value)
        End Set
    End Property

    Public Property CostEndStatus() As eStatusFlags
        Get
            Return DirectCast(GetVariable(eVarNameFlags.EcospaceFleetCostEnd), eStatusFlags)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetVariable(eVarNameFlags.EcospaceFleetCostEnd, value)
        End Set
    End Property

#End Region

End Class
