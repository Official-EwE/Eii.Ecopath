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

#End Region

Public Class cHashResults

    Private m_match As eMatchState = eMatchState.NotSet

    Public Enum eMatchState As Integer
        NotSet = 0
        Match
        NoMatch
    End Enum

    Public Sub New(strKeyRunFile As String, iNumDigits As Integer)
        Me.HashPairs = New List(Of cHashResultPair)
        Me.KeyRunFile = strKeyRunFile
        Me.NumDigits = iNumDigits
        ' Assume all is ok
        Me.m_match = eMatchState.Match
    End Sub

    Public Sub Add(KeyRun As cHashValues, CurModel As cHashValues, Match As Boolean)
        Me.HashPairs.Add(New cHashResultPair(KeyRun, CurModel, Match))
        If Not Match Then Me.m_match = eMatchState.NoMatch
    End Sub

    ''' <summary>Hash results</summary>
    Public ReadOnly Property HashPairs As List(Of cHashResultPair)

    ''' <summary>File name the results were loaded from, if any.</summary>
    Public ReadOnly Property KeyRunFile As String = ""

    ''' <summary>The number of digits used to generate the hash results.</summary>
    Public ReadOnly Property NumDigits As Integer = 3

    ''' <summary>Match state when compared to other results</summary>
    Public ReadOnly Property Match As eMatchState
        Get
            Return Me.m_match
        End Get
    End Property

    Public Sub Invalidate()
        Me.m_match = eMatchState.NotSet
    End Sub

End Class
