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

    Friend Sub Load()

        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.Clear()

        ReDim m_arenas(simdata.Narena)

        For i As Integer = 1 To simdata.NlinksSet
            Dim iPrey As Integer = simdata.IlinkSet(i)
            Dim iPred As Integer = simdata.JlinkSet(i)
            Dim iLink As Integer = simdata.KlinkSet(i)
            Dim iArena As Integer = simdata.ArenaNo(iPrey, iPred)

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

            arena.AllowValidation = False
            arena.ArenaShare(iLink) = simdata.PeatArena(iArena, iLink)
            arena.ArenaShareStatus(iLink) = eStatusFlags.OK
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
            Console.WriteLine("#Arena links changed to " & ii)
        Else
            Array.Clear(simdata.IlinkSet, 0, simdata.IlinkSet.Length)
            Array.Clear(simdata.JlinkSet, 0, simdata.JlinkSet.Length)
            Array.Clear(simdata.KlinkSet, 0, simdata.KlinkSet.Length)
            Array.Clear(simdata.PeatArena, 0, simdata.PeatArena.Length)
        End If

        ii = 0
        For Each arena As cEcosimArena In Me.m_arenas
            If (arena IsNot Nothing) Then
                For j As Integer = 1 To simdata.nGroups
                    Dim prop As Single = arena.ArenaShare(j)
                    If (prop > 0) Then

                        ' Sanity check
                        Debug.Assert(pathdata.DCInput(arena.Pred, arena.Prey) > 0)

                        ii += 1
                        simdata.IlinkSet(ii) = arena.Prey
                        simdata.JlinkSet(ii) = arena.Pred
                        simdata.KlinkSet(ii) = j
                        Dim iArena As Integer = simdata.ArenaNo(arena.Prey, arena.Pred)
                        simdata.PeatArena(iArena, j) = prop

                    End If
                Next
            End If
        Next

        simdata.DefaultArenas()

    End Sub

#Region " Public access "

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

    'Public ReadOnly Property NumArenas As Integer
    '    Get
    '        Return m_arenas.Count
    '    End Get
    'End Property

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="iArena">One-based arena index</param>
    '''' <returns></returns>
    'Public ReadOnly Property Arena(iArena As Integer) As cEcosimArena
    '    Get
    '        If (iArena < 1 Or iArena > Me.NumArenas) Then Return Nothing
    '        Return Me.m_arenas(iArena - 1)
    '    End Get
    'End Property

    '' Passthrough
    'Public ReadOnly Property IArena(i As Integer) As Integer
    '    Get
    '        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData
    '        Return simdata.Iarena(i)
    '    End Get
    'End Property

    '' Passthrough
    'Public ReadOnly Property JArena(i As Integer) As Integer
    '    Get
    '        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData
    '        Return simdata.Jarena(i)
    '    End Get
    'End Property

    '' Passthrough
    'Public ReadOnly Property ArenaNo(i As Integer, j As Integer) As Integer
    '    Get
    '        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData
    '        Return simdata.ArenaNo(i, j)
    '    End Get
    'End Property

#End Region ' Public access

End Class
