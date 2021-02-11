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

        While Me.m_core.EcoSimGroupOutputs(PredIndexEcosim).Name <> Me.m_Predator
            PredIndexEcosim += 1
        End While
        Return PredIndexEcosim

    End Function

    Public Function GetIndexPreyForEcoSim(i As Integer) As Integer
        Dim PreyIndexEcosim As Integer = 1

        While Me.m_core.EcoSimGroupOutputs(PreyIndexEcosim).Name <> Me.m_Prey(i)
            PreyIndexEcosim += 1
        End While
        Return PreyIndexEcosim

    End Function

#End Region


End Class

