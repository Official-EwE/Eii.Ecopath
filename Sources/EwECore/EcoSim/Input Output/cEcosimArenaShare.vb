' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcosimArenaShare
    Inherits cCoreInputOutputBase

    Public Sub New(core As cCore, iArena As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.m_dataType = eDataTypes.EcosimArenaShare
        Me.m_coreComponent = eCoreComponentType.EcoSim

        Me.AllowValidation = False

        Me.Index = iArena

        ' There are no arena objects in EwE. But we can fake a very likely unique ID
        Me.Prey = simdata.ilink(iArena)
        Me.Pred = simdata.jlink(iArena)
        Me.DBID = simdata.GroupDBID(Me.Prey) * 10000 + simdata.GroupDBID(Me.Pred)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        'arrayed values
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimArenaShare, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

    Public ReadOnly Property iArena As Integer
        Get
            Return Me.Index
        End Get
    End Property

    Public Property ArenaShare(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimArenaShare, iPred))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimArenaShare, value, iPred)
        End Set
    End Property

    Public Property ArenaShareStatus(iPred As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimArenaShare, iPred)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimArenaShare, value, iPred)
        End Set
    End Property

    Public ReadOnly Property Prey As Integer

    Public ReadOnly Property Pred As Integer

    Public ReadOnly Property NumArenas As Integer
        Get
            Dim pathds As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim n As Integer = 0
            For i As Integer = 1 To pathds.NumLiving
                If (pathds.DCInput(i, Me.Prey) > 0) And (Me.ArenaShare(i) > 0) Then
                    n += 1
                End If
            Next
            Return n
        End Get
    End Property

End Class
