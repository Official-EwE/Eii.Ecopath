' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Public Class cEcosimArena
    Inherits cCoreInputOutputBase

    Public Sub New(core As cCore, iDBID As Integer, iArena As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Me.m_dataType = eDataTypes.EcosimArenaShare
        Me.m_coreComponent = eCoreComponentType.Ecosim

        Me.AllowValidation = False

        Me.Index = iArena
        Me.DBID = iDBID

        'arrayed values
        val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimArenaShare, eStatusFlags.Null, eCoreCounterTypes.nGroups)
        val.AffectsRunState = False
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

    Public ReadOnly Property iArena As Integer
        Get
            Return Me.Index
        End Get
    End Property

    ''' <summary>
    ''' One-based prey index.
    ''' </summary>
    Public Property Prey As Integer

    ''' <summary>
    ''' One-based pred index.
    ''' </summary>
    Public Property Pred As Integer

    Public Sub Reset()
        Me.AllowValidation = False
        For i As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nLivingGroups)
            Me.ArenaShare(i) = If(i = Me.Pred, 1, 0)
        Next
        Me.AllowValidation = True
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Index & ": prey " & Me.Prey & ", pred " & Me.Pred
    End Function

#Region " Variable via dot '.' operator "

    Public Property ArenaShare(iPred As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimArenaShare, iPred))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimArenaShare, value, iPred)
        End Set
    End Property

#End Region ' Variable via dot '.' operator

#Region " Status via dot '.' operator "

    Public Property ArenaShareStatus(iPred As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimArenaShare, iPred)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimArenaShare, value, iPred)
        End Set
    End Property

#End Region ' Status via dot '.' operator

End Class
