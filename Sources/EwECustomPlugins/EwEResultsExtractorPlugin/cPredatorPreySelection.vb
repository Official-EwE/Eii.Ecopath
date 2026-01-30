' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cPredatorPreySelection

#Region "Private fields"

    Private m_Predator As String
    Private m_Prey As List(Of String)
    Private m_core As cCore

#End Region

#Region "Constructor(s)"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Predator"></param>
    ''' <param name="Core"></param>
    ''' <remarks>
    ''' JS 01Mar11: Core must be provided as a parameter.
    ''' </remarks>
    Public Sub New(ByRef Predator As String, Core As cCore)
        Me.m_core = Core
        Me.m_Predator = Predator
        Me.m_Prey = New List(Of String)
    End Sub

#End Region

#Region "Properties"

    Public Property PredatorName() As String
        Get
            Return Me.m_Predator
        End Get
        Set(value As String)
            Me.m_Predator = value
        End Set
    End Property

    Public Property PreyName(i As Integer) As String
        Get
            Return Me.m_Prey(i)
        End Get
        Set(value As String)
            Me.m_Prey(i) = value
        End Set
    End Property

#End Region

#Region "Subroutines"

    Public Sub AddPrey(PreyName As String)
        Me.m_Prey.Add(PreyName)
    End Sub

    Public Sub RemovePrey(i As Integer)
        Me.m_Prey.RemoveAt(i)
    End Sub

#End Region

#Region "Functions"

    Public Function CountPrey() As Integer
        Return Me.m_Prey.Count
    End Function

    Public Function GetIndexPredatorForEcoSim() As Integer
        Dim PredIndexEcosim As Integer = 1

        While Me.m_core.EcosimGroupOutputs(PredIndexEcosim).Name <> Me.m_Predator
            PredIndexEcosim += 1
        End While
        Return PredIndexEcosim

    End Function

    Public Function GetIndexPreyForEcoSim(i As Integer) As Integer
        Dim PreyIndexEcosim As Integer = 1

        While Me.m_core.EcosimGroupOutputs(PreyIndexEcosim).Name <> Me.m_Prey(i)
            PreyIndexEcosim += 1
        End While
        Return PreyIndexEcosim

    End Function

#End Region


End Class

