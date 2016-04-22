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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cHashValues

    Private Const FILE_RECORD_TAG As String = "<HASH_VALUE_RECORD>"
    Private Const N_RECORD_FIELDS As Integer = 5
    Private Const DELIM As Char = "/"c
    Private Shared s_sortOrder As Integer = 0

#Region "Private Variables"

    Private Enum eFieldOrder
        TAG
        SortOrder
        DataID
        VaribleID
        HashValue
    End Enum

    Private m_component As String
    Private m_variableid As String
    Private m_value As String
    Private m_strHash As String

    Private m_sortOrder As Integer

#End Region

#Region "Construction"

    Public Sub New()
        m_component = "NA"
        m_variableid = "NA"
        m_value = "NA"
        m_strHash = "NA"
    End Sub

    Public Sub New(CoreObject As String, VarName As eVarNameFlags, StringValue As String)
        Me.New(CoreObject, VarName.ToString, StringValue)
    End Sub

    Public Sub New(CoreObject As String, VarName As String, StringValue As String)

        cHashValues.IncrementSortOrder()

        Me.m_component = CoreObject
        Me.m_variableid = VarName
        Me.m_value = StringValue
        Me.m_strHash = cEncryptionUtilities.MD5(Me.m_value)
        Me.m_sortOrder = s_sortOrder

    End Sub

#End Region

#Region "Public Properties"

    Public ReadOnly Property Component As String
        Get
            Return Me.m_component
        End Get
    End Property

    Public ReadOnly Property VariableID As String
        Get
            Return Me.m_variableid
        End Get
    End Property

    Public ReadOnly Property Value As String
        Get
            Return Me.m_value
        End Get
    End Property

    Public ReadOnly Property Hash As String
        Get
            Return m_strHash
        End Get
    End Property

    Public ReadOnly Property SortOrder As Integer
        Get
            Return m_sortOrder
        End Get
    End Property

    Public ReadOnly Property Key As String
        Get
            Return Me.Component & DELIM & Me.VariableID
            'Removed the sort order from the key
            'Because it prevents you from finding keys in older formatted files
            'When you add a variable it automatically changes the keys that follow
            'Return Me.SortOrder.ToString & DELIM & Me.Component & DELIM & Me.VariableID
        End Get
    End Property

#End Region

#Region "Public and Private methods"

    Private Shared Sub IncrementSortOrder()
        cHashValues.s_sortOrder += 1
    End Sub

    Public Shared Sub ClearSort()
        cHashValues.s_sortOrder = 0
    End Sub

    Public Function ToRecordString() As String
        Return FILE_RECORD_TAG & "," & cStringConverters.FormatNumber(Me.SortOrder) & "," & cStringUtils.ToCSVField(Me.Component) & "," & cStringUtils.ToCSVField(Me.VariableID) & "," & cStringUtils.ToCSVField(Me.Hash)
    End Function

    Public Function FromRecordString(Record As String) As Boolean
        Try

            If cHashValues.isHashRecord(Record) Then

                Dim data() As String = cStringUtils.SplitQualified(Record, ",")
                Me.m_sortOrder = Integer.Parse(data(eFieldOrder.SortOrder))
                Me.m_component = data(eFieldOrder.DataID)
                Me.m_variableid = data(eFieldOrder.VaribleID)
                Me.m_strHash = data(eFieldOrder.HashValue)

                Return True

            End If

        Catch ex As Exception

        End Try

        Return False

    End Function

    Public Shared Function isHashRecord(record As String) As Boolean

        Try
            Dim data() As String = record.Split(CChar(","))
            If data.Length = N_RECORD_FIELDS Then
                If String.Compare(data(eFieldOrder.TAG).Trim, FILE_RECORD_TAG) = 0 Then
                    Return True
                End If
            End If
        Catch ex As Exception

        End Try

        Return False

    End Function

#End Region

End Class

Public Class cHashResults

    Private m_pairs As List(Of cHashResultPair)
    Private m_match As eMatchState = eMatchState.NotSet
    Private m_keyRunFile As String

    Public Enum eMatchState As Integer
        NotSet = 0
        Match
        NoMatch
    End Enum

    Public Sub New(strKeyRunFile As String)
        Me.m_pairs = New List(Of cHashResultPair)
        Me.m_keyRunFile = strKeyRunFile
        ' Assume all is ok
        Me.m_match = eMatchState.Match
    End Sub

    Public Sub Add(KeyRun As cHashValues, CurModel As cHashValues, Match As Boolean)
        Me.m_pairs.Add(New cHashResultPair(KeyRun, CurModel, Match))
        If Not Match Then Me.m_match = eMatchState.NoMatch
    End Sub

    Public ReadOnly Property HashPairs As List(Of cHashResultPair)
        Get
            Return m_pairs
        End Get
    End Property

    Public ReadOnly Property KeyRunFile As String
        Get
            Return Me.m_keyRunFile
        End Get
    End Property

    Public ReadOnly Property Match As eMatchState
        Get
            Return Me.m_match
        End Get
    End Property

    Public Sub Invalidate()
        Me.m_match = eMatchState.NotSet
    End Sub

End Class


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
