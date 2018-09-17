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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcosimArenaManager

    ' ISsue arena objects, where DBID is constructed from pred and prey combo
    ' Offers array of pred contributions to arena
    ' As Variable (with varname, datatype, validation). Status = NULL where no predator

    ' For every prey, need array of preds, arena no, and share
    Private ReadOnly m_arenas As New List(Of cEcosimArenaShare)
    Private ReadOnly m_core As cCore

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Friend Sub Load()

        Dim pathdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.m_arenas.Clear()

        For i As Integer = 1 To simdata.inlinks
            ' ToDo: obtain arena index from Ecosim
            Dim share As New cEcosimArenaShare(m_core, i)
            share.AllowValidation = False
            Dim iPred As Integer = share.Pred
            Dim iPrey As Integer = share.Prey
            For j As Integer = 1 To Me.m_core.nLivingGroups
                share.ArenaShare(j) = simdata.PeatArena(i, j)
                If (pathdata.DCInput(j, iPrey) = 0) Then
                    share.ArenaShareStatus(j) = eStatusFlags.Null Or eStatusFlags.NotEditable
                Else
                    share.ArenaShareStatus(j) = eStatusFlags.OK
                End If
            Next
            share.AllowValidation = True
            Me.m_arenas.Add(share)
        Next

    End Sub

    Friend Sub Update()

        Dim pathdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Dim ii As Integer = 0

        For Each share As cEcosimArenaShare In Me.m_arenas
            For j As Integer = 1 To pathdata.NumLiving
                If share.ArenaShare(j) > 0 Then ii += 1
            Next
        Next

        simdata.NlinksSet = ii
        ReDim simdata.IlinkSet(ii)
        ReDim simdata.JlinkSet(ii)
        ReDim simdata.KlinkSet(ii)
        ReDim simdata.PeatArena(ii, pathdata.NumGroups)

        ii = 0
        For Each share As cEcosimArenaShare In Me.m_arenas
            For j As Integer = 1 To pathdata.NumLiving
                Dim prop As Single = share.ArenaShare(j)
                If (prop > 0) Then
                    ii += 1
                    simdata.IlinkSet(ii) = share.Prey
                    simdata.JlinkSet(ii) = share.Pred
                    simdata.KlinkSet(ii) = j
                    simdata.PeatArena(ii, j) = prop
                End If
            Next
        Next

    End Sub

#Region " Public access "

    Public ReadOnly Property Arenas(prey As Integer) As cEcosimArenaShare()
        Get
            Dim pathdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim lArenas As New List(Of cEcosimArenaShare)
            For Each arena As cEcosimArenaShare In Me.m_arenas
                If (arena.Prey = prey) Or (prey <= 0) Then lArenas.Add(arena)
            Next
            Return lArenas.ToArray()
        End Get
    End Property

    Public ReadOnly Property NumArenas As Integer
        Get
            Return m_arenas.Count
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iArena">One-based arena index</param>
    ''' <returns></returns>
    Public ReadOnly Property Arena(iArena As Integer) As cEcosimArenaShare
        Get
            If (iArena < 1 Or iArena > Me.NumArenas) Then Return Nothing
            Return Me.m_arenas(iArena - 1)
        End Get
    End Property

#End Region ' Public access

End Class
