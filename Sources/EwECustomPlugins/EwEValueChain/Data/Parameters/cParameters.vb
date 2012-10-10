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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports System.Reflection

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Parameters that dictate the behaviour of the Value Chain plug-in.
''' </summary>
''' ===========================================================================
<Serializable()> _
Public Class cParameters
    Inherits EwEUtils.Database.cEwEDatabase.cOOPStorable

    Private m_bRunWithEcopath As Boolean = False
    Private m_bRunWithEcosim As Boolean = False
    Private m_bRunSearches As Boolean = False
    Private m_bResultsByFleet As Boolean = False
    Private m_sEffortMin As Single = 0.0!
    Private m_sEffortMax As Single = 4.0!
    Private m_sEffortInc As Single = 0.25!
    Private m_liFleets As New List(Of Integer)
    Private m_sZoomFactor As Single = 1.0!
    Private m_bShowGrid As Boolean = False
    Private m_bDeletePrompt As Boolean = True

#Region " Properties "

    Public ReadOnly Property EquilibriumFleetsToVary() As List(Of Integer)
        Get
            Return Me.m_liFleets
        End Get
    End Property

    Public Property EquilibriumEffortMin() As Single
        Get
            Return Me.m_sEffortMin
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortMin = value
        End Set
    End Property

    Public Property EquilibriumEffortMax() As Single
        Get
            Return Me.m_sEffortMax
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortMax = value
        End Set
    End Property

    Public Property EquilibriumEffortIncrement() As Single
        Get
            Return Me.m_sEffortInc
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortInc = value
        End Set
    End Property

    Public Property RunWithEcopath() As Boolean
        Get
            Return Me.m_bRunWithEcopath
        End Get
        Set(ByVal bRunWithEcopath As Boolean)
            If (bRunWithEcopath <> Me.m_bRunWithEcopath) Then
                Me.m_bRunWithEcopath = bRunWithEcopath
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property RunWithEcosim() As Boolean
        Get
            Return Me.m_bRunWithEcosim
        End Get
        Set(ByVal bRunWithEcosim As Boolean)
            If (Me.m_bRunWithEcosim <> bRunWithEcosim) Then
                Me.m_bRunWithEcosim = bRunWithEcosim
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property RunWithSearches() As Boolean
        Get
            Return Me.m_bRunSearches
        End Get
        Set(ByVal bRunWithFishingPolicySearch As Boolean)
            If (Me.m_bRunSearches <> bRunWithFishingPolicySearch) Then
                Me.m_bRunSearches = bRunWithFishingPolicySearch
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property ZoomFactor As Single
        Get
            Return Me.m_sZoomFactor
        End Get
        Set(value As Single)
            Me.m_sZoomFactor = value
        End Set
    End Property

    Public Property ShowGrid As Boolean
        Get
            Return Me.m_bShowGrid
        End Get
        Set(bShowGrid As Boolean)
            Me.m_bShowGrid = bShowGrid
        End Set
    End Property

    Public Property ResultsByFleet() As Boolean
        Get
            Return Me.m_bResultsByFleet
        End Get
        Set(ByVal bResultsByFleet As Boolean)
            If (Me.m_bResultsByFleet <> bResultsByFleet) Then
                Me.m_bResultsByFleet = bResultsByFleet
                Me.SetChanged()
            End If
        End Set
    End Property

    <DefaultValue(True)> _
    Public Property DeletePrompt As Boolean
        Get
            Return Me.m_bDeletePrompt
        End Get
        Set(value As Boolean)
            If (value <> Me.m_bDeletePrompt) Then
                Me.m_bDeletePrompt = value
                Me.SetChanged()
            End If
        End Set
    End Property

#End Region ' Parameters

End Class
