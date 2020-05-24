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
Imports EwECore

#End Region ' Imports

Public Class cEcosimAreaViewData

    Private m_Arena1 As Integer()
    Private m_Nar As Integer()
    Private m_groups As New List(Of Integer)

    Public Sub New(core As cCore)
        Me.m_groups.Clear()

        Me.Manager = core.EcosimArenaManager
        ReDim m_Arena1(core.nGroups)
        ReDim m_Nar(core.nGroups)

        ' From EwE5, need to dissect further
        Dim i1 As Integer = Me.Manager.IArena(1)
        Dim Minarena As Integer = 1
        For i As Integer = i1 To core.nGroups
            For ii As Integer = Minarena To Me.Manager.NumArenas
                If Me.Manager.IArena(ii) = i Then
                    Me.m_Nar(i) += 1
                    If Me.m_Nar(i) = 1 Then
                        Me.m_Arena1(i) = ii
                        Me.m_groups.Add(i)
                    End If
                End If
            Next
            Minarena = Me.m_Arena1(i) + Me.m_Nar(i)
        Next
    End Sub

    Public ReadOnly Property Manager As cEcosimArenaManager

    Public ReadOnly Property Groups As Integer()
        Get
            Return Me.m_groups.ToArray()
        End Get
    End Property

    Public ReadOnly Property GroupArenaNo(i As Integer) As Integer
        Get
            Return Me.m_Nar(i)
        End Get
    End Property

    Public ReadOnly Property Arena1(i As Integer) As Integer
        Get
            Return Me.m_Arena1(i)
        End Get
    End Property

End Class
