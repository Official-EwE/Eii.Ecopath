Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceGroupSummary
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue

        Me.DBID = iGroup '????
        Me.Index = iGroup
        'no validators
        val = New cValue(0, eVarNameFlags.EcospaceGroupBiomassStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcospaceGroupBiomassEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceGroupCatchEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceGroupCatchStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceGroupValueStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceGroupValueEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

    End Sub

#End Region

#Region "Properties via dot '.' operator"

    Public Property BiomassStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupBiomassStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupBiomassEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupCatchStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupCatchStart, value, iFleet)
        End Set
    End Property


    Public Property CatchEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupCatchEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupCatchEnd, value, iFleet)
        End Set
    End Property


    Public Property ValueStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupValueStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupValueStart, value, iFleet)
        End Set
    End Property

    Public Property ValueEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceGroupValueEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceGroupValueEnd, value, iFleet)
        End Set
    End Property


#End Region

#Region "Status via dot '.' operator"

    Public Property BiomassStartStatus() As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupBiomassStart), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEndStatus() As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupBiomassEnd), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStartBiomassStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupCatchStart, IFleet), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupCatchStart, value, IFleet)
        End Set
    End Property


    Public Property CatchEndBiomassStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupCatchEnd, IFleet), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupCatchEnd, value, IFleet)
        End Set
    End Property


    Public Property ValueStartStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupValueStart, IFleet), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupValueStart, value, IFleet)
        End Set
    End Property

    Public Property ValueEndStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return DirectCast(GetStatus(eVarNameFlags.EcospaceGroupValueEnd, IFleet), eStatusFlags)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceGroupValueEnd, value, IFleet)
        End Set
    End Property

#End Region

End Class
