'==============================================================================
'
' $Log: cEcosimGroupSummary.vb,v $
' Revision 1.1  2008/09/26 07:30:19  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/05/29 22:22:43  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.3  2008/02/12 16:23:28  jeroens
' Fixed dbid, datatype, status access bugs
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimGroupSummary
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue

        Me.m_DataType = eDataTypes.EcosimGroupSummary
        Me.DBID = TheCore.m_EcoPathData.GroupDBID(iGroup)
        Me.Index = iGroup

        'no validators
        val = New cValue(0, eVarNameFlags.EcosimGroupBiomassStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcosimGroupBiomassEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

    End Sub

#End Region

#Region "Properties via dot '.' operator"

    Public Property BiomassStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupBiomassStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupBiomassEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupCatchStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupCatchStart, value, iFleet)
        End Set
    End Property


    Public Property CatchEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupCatchEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupCatchEnd, value, iFleet)
        End Set
    End Property


    Public Property ValueStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupValueStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupValueStart, value, iFleet)
        End Set
    End Property

    Public Property ValueEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupValueEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupValueEnd, value, iFleet)
        End Set
    End Property


#End Region

#Region "Status via dot '.' operator"

    Public Property BiomassStartStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupBiomassStart)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEndStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupBiomassEnd)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStartBiomassStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupCatchStart, IFleet)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupCatchStart, value, IFleet)
        End Set
    End Property


    Public Property CatchEndBiomassStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupCatchEnd, IFleet)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupCatchEnd, value, IFleet)
        End Set
    End Property


    Public Property ValueStartStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupValueStart, IFleet)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupValueStart, value, IFleet)
        End Set
    End Property

    Public Property ValueEndStatus(ByVal IFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimGroupValueEnd, IFleet)
        End Get

        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimGroupValueEnd, value, IFleet)
        End Set
    End Property

#End Region

End Class
