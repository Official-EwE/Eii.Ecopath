Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceFleetOutput
    Inherits cCoreInputOutputBase

    Private m_CoreArrays As New Dictionary(Of eVarNameFlags, IResultsWrapper)
    Private m_spacedata As cEcospaceDataStructures

    Public Sub New(ByRef TheCore As cCore, ByVal EcospaceData As cEcospaceDataStructures, ByVal FleetIndex As Integer)
        MyBase.New(TheCore)

        Dim val As cValue

        Me.Index = FleetIndex
        Me.DBID = FleetIndex '????
        Me.m_dataType = eDataTypes.EcospaceFleetOuput
        m_spacedata = EcospaceData

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


        val = New cValue(0, eVarNameFlags.EcospaceFleetEffortES, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)


    End Sub

    Public Sub Init()

        m_CoreArrays.Clear()
        m_CoreArrays.Add(eVarNameFlags.EcospaceFleetCatch, New c3DResultsWrapper2Fixed(m_spacedata.ResultsByFleet, eSpaceResultsFleets.CatchBio, Me.Index))
        m_CoreArrays.Add(eVarNameFlags.EcospaceFleetValue, New c3DResultsWrapper2Fixed(m_spacedata.ResultsByFleet, eSpaceResultsFleets.Value, Me.Index))

    End Sub


    Public Overrides Function GetVariable(ByVal VarName As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex1 As Integer = -9999, Optional ByVal iIndex2 As Integer = -9999, Optional ByVal iIndex3 As Integer = cCore.NULL_VALUE) As Object

        Try
            If Not m_CoreArrays.ContainsKey(VarName) Then
                'NOT in list of sim vars so get the value from the base class GetVariable(...)
                Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
            Else
                'Varname is access directly via the core data
                Return m_CoreArrays.Item(VarName).Value(iIndex1, iIndex2)
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return Nothing 'Oh this could hurt

    End Function


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


    Public Property EffortES() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetEffortES))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceFleetEffortES, value)
        End Set
    End Property
    Public ReadOnly Property CatchBiomass(ByVal Time As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetCatch))
        End Get
    End Property

    Public ReadOnly Property Value(ByVal Time As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceFleetValue))
        End Get
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
