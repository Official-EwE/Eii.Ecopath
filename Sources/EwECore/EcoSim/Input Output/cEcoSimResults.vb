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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

''' <summary>
''' Results from the last completed EcoSim Time Step
''' Passed out by EcoSim via the EcoSimTimeStepDelegate(iTime,Results) delegate
''' </summary>
''' <remarks></remarks>
Public Class cEcoSimResults
    Public nGroups As Integer
    Public nFleets As Integer
    Public CurrentT As Long
    Public Biomass() As Single
    Public TLCatch As Single
    Public FIB As Single
    Public Effort() As Single

    ''' <summary> Computed number of Fish by group </summary>
    Public FishCount() As Single

    Public Yield() As Single
    ''' <summary>Catch {group x fleet}</summary>
    Public BCatch(,) As Single ' by group, by fleet

    ''' <summary>Landings discards not included {group x fleet}</summary>
    Public Landings(,) As Single

    Public nStanza As Integer
    Public nMaxLifeStages As Integer

    ''' <summary>
    ''' relative biomass of stock
    ''' </summary>
    ''' <remarks>X axis in EwE5 sr plot</remarks>
    Public BStock(,) As Single

    ''' <summary>
    ''' relative biomass of recruits
    ''' </summary>
    ''' <remarks>Y axis in EwE5 sr plot</remarks>
    Public BRecruitment(,) As Single

    Private m_hasSRData(,) As Boolean
    Private m_hasData As Boolean

    ''' <summary>
    ''' Is there stock recruitment data for Adult juvenile pair
    ''' </summary>
    ''' <remarks></remarks>
    Public Property hasSRData(ByVal iAdult As Integer, ByVal iJuv As Integer) As Boolean
        Get
            Return m_hasSRData(iAdult, iJuv)
        End Get
        Set(ByVal value As Boolean)
            m_hasSRData(iAdult, iJuv) = value
        End Set
    End Property


    ''' <summary>
    ''' Is there stock recruitment data for this time step
    ''' </summary>
    Public Property hasSRData() As Boolean
        Get
            Return m_hasData
        End Get
        Set(ByVal value As Boolean)
            m_hasData = value
        End Set
    End Property

    Public Sub New(ByRef numGroups As Integer, ByVal numStanzas As Integer, ByVal maxLifeStages As Integer, ByVal nFleets As Integer)

        nGroups = numGroups
        nStanza = numStanzas
        nFleets = nFleets
        nMaxLifeStages = maxLifeStages

        ReDim Biomass(nGroups)
        ReDim Yield(nGroups)
        ReDim BCatch(nGroups, nFleets)
        ReDim Landings(nGroups, nFleets)

        ReDim m_hasSRData(nStanza, nMaxLifeStages)
        ReDim BStock(nStanza, nMaxLifeStages)
        ReDim BRecruitment(nStanza, nMaxLifeStages)
        ReDim FishCount(nGroups)
        ReDim Effort(nFleets)

    End Sub


    Public Sub clear()

        Array.Clear(Me.Biomass, 0, nGroups)
        Array.Clear(Me.Yield, 0, nGroups)
        Array.Clear(Me.FishCount, 0, nGroups)

        Array.Clear(Me.BCatch, 0, BCatch.Length)

        Array.Clear(Me.Landings, 0, Landings.Length)
        Array.Clear(Me.m_hasSRData, 0, m_hasSRData.Length)
        Array.Clear(Me.BRecruitment, 0, BRecruitment.Length)
        Array.Clear(Me.BStock, 0, BStock.Length)
        Array.Clear(Me.Effort, 0, nFleets)

    End Sub

End Class
