' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports EwECore.ValueWrapper

Public Class cEcospaceFleetOutput
    Inherits cCoreInputOutputBase

    Private m_CoreArrays As New Dictionary(Of eVarNameFlags, IResultsWrapper)
    Private m_spacedata As cEcospaceDataStructures

    Public Sub New(core As cCore, EcospaceData As cEcospaceDataStructures, FleetIndex As Integer)
        MyBase.New(core)

        Dim val As cValue

        Me.Index = FleetIndex
        Me.DBID = FleetIndex '????
        Me.m_dataType = eDataTypes.EcospaceFleetOuput
        Me.m_spacedata = EcospaceData

        'no validators
        'Catch biomass
        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetCatchStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetCatchEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Value
        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetValueStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetValueEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'Cost
        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetCostStart, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetCostEnd, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, 0, eVarNameFlags.EcospaceFleetEffortES, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

    End Sub

    Public Sub Init()

        Me.m_CoreArrays.Clear()
        Me.m_CoreArrays.Add(eVarNameFlags.EcospaceFleetCatch, New c3DResultsWrapper2Fixed(Me.m_spacedata.ResultsByFleet, eSpaceResultsFleets.CatchBio, Me.Index))
        Me.m_CoreArrays.Add(eVarNameFlags.EcospaceFleetValue, New c3DResultsWrapper2Fixed(Me.m_spacedata.ResultsByFleet, eSpaceResultsFleets.Value, Me.Index))

        Me.m_CoreArrays.Add(eVarNameFlags.EcospaceFleetProfit, New c2DResultsWrapper2Fixed(Me.m_spacedata.ResultsSummaryByFleet, 0, Me.Index))
        Me.m_CoreArrays.Add(eVarNameFlags.EcospaceFleetJobs, New c2DResultsWrapper2Fixed(Me.m_spacedata.ResultsSummaryByFleet, 1, Me.Index))

    End Sub


    Public Overrides Function GetVariable(VarName As eVarNameFlags, Optional iIndex1 As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = cCore.NULL_VALUE) As Object

        Try
            If Not Me.m_CoreArrays.ContainsKey(VarName) Then
                'NOT in list of sim vars so get the value from the base class GetVariable(...)
                Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
            Else
                'Varname is access directly via the core data
                Return Me.m_CoreArrays.Item(VarName).Value(iIndex1, iIndex2)
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return Nothing 'Oh this could hurt

    End Function


#Region "Variable via dot '.' operator"


    Public Property CatchStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetCatchStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetCatchEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCatchEnd, value)
        End Set
    End Property


    Public Property ValueStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetValueStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetValueEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStart() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetCostStart))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCostStart, value)
        End Set
    End Property

    Public Property CostEnd() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetCostEnd))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCostEnd, value)
        End Set
    End Property


    Public Property EffortES() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetEffortES))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceFleetEffortES, value)
        End Set
    End Property
    Public ReadOnly Property CatchBiomass(Time As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetCatch, Time))
        End Get
    End Property

    Public ReadOnly Property Value(Time As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetValue, Time))
        End Get
    End Property


    Public ReadOnly Property Profit() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetProfit))
        End Get
    End Property

    Public ReadOnly Property Jobs() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceFleetJobs))
        End Get
    End Property

#End Region

#Region "Status via dot '.' operator"

    Public Property CatchStartStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetCatchStart), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCatchStart, value)
        End Set
    End Property

    Public Property CatchEndStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetCatchEnd), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCatchEnd, value)
        End Set
    End Property

    Public Property ValueStartStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetValueStart), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetValueStart, value)
        End Set
    End Property

    Public Property ValueEndStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetValueEnd), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetValueEnd, value)
        End Set
    End Property


    Public Property CostStartStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetCostStart), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCostStart, value)
        End Set
    End Property

    Public Property CostEndStatus() As eStatusFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcospaceFleetCostEnd), eStatusFlags)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetVariable(eVarNameFlags.EcospaceFleetCostEnd, value)
        End Set
    End Property

#End Region

End Class
