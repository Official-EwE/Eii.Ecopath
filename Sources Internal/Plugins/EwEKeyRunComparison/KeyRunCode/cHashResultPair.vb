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

Public Class cHashResultPair

    Public Enum eMatchState
        NotEqual = 0
        MissingCurrentModel
        MissingKeyRun
        Equal
    End Enum

    Private m_isMatch As Boolean

    Public Property Component As String
    Public Property VariableID As String
    Public Property SortOrder As Integer
    Public Property MatchedState As eMatchState

    Public Sub New(KeyRun As cHashValues, CurModel As cHashValues, Match As Boolean)
        m_isMatch = Match

        If Match Then
            Me.MatchedState = eMatchState.Equal
        Else
            Me.MatchedState = eMatchState.NotEqual
        End If

        If KeyRun IsNot Nothing Then
            Me.Component = KeyRun.Component
            Me.VariableID = KeyRun.VariableID
            Me.SortOrder = KeyRun.SortOrder
        Else
            Debug.Assert(CurModel IsNot Nothing, "Oppsss Null HashValue passed to Results.")
            Me.Component = CurModel.Component
            Me.VariableID = CurModel.VariableID
            Me.SortOrder = CurModel.SortOrder
        End If

        If KeyRun Is Nothing Then
            Me.MatchedState = eMatchState.MissingKeyRun
        ElseIf CurModel Is Nothing Then
            Me.MatchedState = eMatchState.MissingCurrentModel
        End If

    End Sub

    Public ReadOnly Property isMatch As Boolean
        Get
            Return m_isMatch
        End Get
    End Property

End Class
