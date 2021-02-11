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
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcosimArenaManager

    ' ISsue arena objects, where DBID is constructed from pred and prey combo
    ' Offers array of pred contributions to arena
    ' As Variable (with varname, datatype, validation). Status = NULL where no predator

    ' For every prey, need array of preds, arena no, and share
    Private m_arenas() As cEcosimArena
    Private ReadOnly m_core As cCore

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Sub Clear()
        Me.m_arenas = Nothing
    End Sub

    Friend Sub Init()

        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.Clear()

        ReDim Me.m_arenas(simdata.Narena)

        ' Initialize arenas from all available links, not from only set links!
        For i As Integer = 1 To simdata.inlinks
            Dim iPrey As Integer = simdata.ilink(i)
            Dim iPred As Integer = simdata.jlink(i)
            Dim iArena As Integer = simdata.ArenaNo(iPrey, iPred)

            Debug.Assert(iArena > 0)

            ' A bit of cleverness here: arenas may be reused, remember? That's the entire fun about sharing arenas
            Dim arena As cEcosimArena = Me.m_arenas(iArena)
            If (arena Is Nothing) Then
                ' Fake a likely unique arena ID
                Dim iDBID As Integer = simdata.GroupDBID(iPrey) * 10000 + simdata.GroupDBID(iPred)
                arena = New cEcosimArena(Me.m_core, iDBID, iArena)
                arena.Prey = iPrey
                arena.Pred = iPred
                arena.ResetStatusFlags(True)
                Me.m_arenas(iArena) = arena
            End If

        Next

    End Sub

    ''' <summary>
    ''' Reloads and rebuilds the arena data structures.
    ''' </summary>
    Friend Sub Load()

        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        For i As Integer = 1 To simdata.NlinksSet
            Dim iPrey As Integer = simdata.IlinkSet(i)
            Dim iPred As Integer = simdata.JlinkSet(i)
            Dim iArena As Integer = simdata.ArenaNo(iPrey, iPred)

            Debug.Assert(iArena > 0)

            ' A bit of cleverness here: arenas may be reused, remember? That's the entire fun about sharing arenas
            Dim arena As cEcosimArena = Me.m_arenas(iArena)

            arena.AllowValidation = False
            Dim iPredShared As Integer = simdata.KlinkSet(i)
            arena.ArenaShare(iPredShared) = simdata.PeatArena(iArena, iPredShared)
            arena.ArenaShareStatus(iPredShared) = eStatusFlags.OK
            arena.AllowValidation = True

        Next

    End Sub

    Friend Sub Update()

        Dim pathdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Dim ii As Integer = 0
        For Each arena As cEcosimArena In Me.m_arenas
            If (arena IsNot Nothing) Then
                For j As Integer = 1 To simdata.nGroups
                    Dim prop As Single = arena.ArenaShare(j)
                    If prop > 0 Then ii += 1
                Next
            End If
        Next

        If (ii <> simdata.NlinksSet) Then
            simdata.NlinksSet = ii
            simdata.RedimArenaLinks()
            'Console.WriteLine("#Arena links changed to " & ii)
        Else
            Array.Clear(simdata.IlinkSet, 0, simdata.IlinkSet.Length)
            Array.Clear(simdata.JlinkSet, 0, simdata.JlinkSet.Length)
            Array.Clear(simdata.KlinkSet, 0, simdata.KlinkSet.Length)
            Array.Clear(simdata.PeatArena, 0, simdata.PeatArena.Length)
        End If

        ii = 0
        For iArena As Integer = 0 To Me.m_arenas.Count - 1
            Dim arena As cEcosimArena = Me.m_arenas(iArena)
            If (arena IsNot Nothing) Then

                ' This should not have changed, but does not hurt to check
                Debug.Assert(simdata.ArenaNo(arena.Prey, arena.Pred) = iArena)
                For j As Integer = 1 To simdata.nGroups
                    Dim prop As Single = arena.ArenaShare(j)
                    If (prop > 0) Then

                        ' Sanity check
                        Debug.Assert(pathdata.DCInput(arena.Pred, arena.Prey) > 0)

                        ii += 1
                        simdata.IlinkSet(ii) = arena.Prey
                        simdata.JlinkSet(ii) = arena.Pred
                        simdata.KlinkSet(ii) = j
                        simdata.PeatArena(iArena, j) = prop

                    End If
                Next
            End If
        Next

        ' simdata.DefaultSharedArenas()

    End Sub

#Region " Public access "

    ''' <summary>
    ''' Resets the arenas for a given prey. If Prey is 0 or less, all arenas will be reset.
    ''' </summary>
    ''' <param name="iPrey">The i prey.</param>
    Public Sub ResetArenas(iPrey As Integer)

        Dim min As Integer = If(iPrey < 1, 1, iPrey)
        Dim max As Integer = If(iPrey < 1, Me.m_core.nGroups, iPrey)
        Dim obj As cEcosimArena = Nothing

        For Each arena As cEcosimArena In Me.m_arenas
            If (arena IsNot Nothing) Then
                If (arena.Prey >= min And arena.Prey <= max) Then
                    arena.Reset()
                    If (obj Is Nothing) Then obj = arena
                End If
            End If
        Next
        Me.Update()

        If (obj IsNot Nothing) Then
            Me.m_core.onChanged(obj, eMessageType.DataModified)
        End If

    End Sub

    Public ReadOnly Property Arenas(prey As Integer) As cEcosimArena()
        Get
            Dim pathdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim lArenas As New List(Of cEcosimArena)
            For Each arena As cEcosimArena In Me.m_arenas
                If (arena IsNot Nothing) Then
                    If (arena.Prey = prey) Or (prey <= 0) Then lArenas.Add(arena)
                End If
            Next
            Return lArenas.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' Get prey indices for which there are multiple arenas
    ''' </summary>
    Public ReadOnly Property Groups(bEwE5 As Boolean) As Integer()
        Get
            Dim lGroups As New List(Of Integer)
            Dim n(Me.m_core.nGroups) As Integer
            For Each arena As cEcosimArena In Me.m_arenas
                If (arena IsNot Nothing) Then
                    n(arena.Prey) += 1
                    If (n(arena.Prey) = If(bEwE5, 1, 2)) Then
                        lGroups.Add(arena.Prey)
                    End If
                End If
            Next
            Return lGroups.ToArray()
        End Get
    End Property

#End Region ' Public access

End Class
