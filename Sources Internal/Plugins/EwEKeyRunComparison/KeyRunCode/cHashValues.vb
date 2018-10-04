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

            If cHashValues.IsHashRecord(Record) Then

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

    Public Shared Function IsHashRecord(record As String) As Boolean

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
