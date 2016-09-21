Option Strict On
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

Imports System.IO
Imports EwECore

#End Region

Public Class cConfiguration

    Private m_core As cCore = Nothing
    Private Property Configuration As Boolean(,)

    Public Enum eResultTypes As Integer
        tsb
        tcb
        b10cm
        b30cm
        tc
        tcb10cm
        tcb30cm
        ' tla
    End Enum

    Public Sub New(core As cCore)
        Me.m_core = core
        ReDim Me.Configuration(core.nGroups, [Enum].GetValues(GetType(eResultTypes)).Length)
    End Sub

    Default Public Property Item(iGroup As Integer, cat As eResultTypes) As Boolean
        Get
            Return Me.Configuration(iGroup, cat)
        End Get
        Set(value As Boolean)
            Me.Configuration(iGroup, cat) = value
        End Set
    End Property

    Public Sub Load()

        Dim core As cCore = Me.m_core
        Dim strFile As String = Me.ConfigFileName()

        For i As Integer = 1 To core.nGroups
            For j As Integer = 0 To [Enum].GetValues(GetType(eResultTypes)).Length - 1
                Me.Configuration(i, j) = False
            Next
        Next

        If File.Exists(strFile) Then
            Dim r As New StreamReader(strFile)
            Dim l As String = ""

            Try
                While Not r.EndOfStream
                    l = r.ReadLine
                    If Not String.IsNullOrWhiteSpace(l) Then
                        If Not l.Trim.StartsWith("#"c) Then
                            Dim bits As String() = l.Split("="c)
                            Dim j As eResultTypes = 0
                            If [Enum].TryParse(bits(0), j) Then
                                For Each strGroup As String In bits(1).Split(" "c)
                                    Dim i As Integer = CInt(strGroup)
                                    Configuration(i, j) = True
                                Next
                            End If
                        End If
                    End If
                End While
            Catch ex As Exception
                ' Woopsy
            End Try
            r.Close()
        End If

    End Sub

    Public Sub Save()

        Dim core As cCore = Me.m_core
        Dim strFile As String = Me.ConfigFileName()
        Dim w As New StreamWriter(strFile)

        w.WriteLine("# FishMIP data aggregation scheme for model " & Me.m_core.EwEModel.Name)
        w.WriteLine("# Full model path " & Me.m_core.DataSource.ToString)
        w.WriteLine()

        For Each j As eResultTypes In [Enum].GetValues(GetType(eResultTypes))
            Dim b As Boolean = False
            w.Write(j.ToString & "=")
            For i As Integer = 1 To core.nGroups
                If (Me.Configuration(i, j)) Then
                    If (b) Then w.Write(" ")
                    w.Write(i)
                    b = True
                End If
            Next
            w.WriteLine()
        Next
        w.Flush()
        w.Close()

    End Sub

    Private Function ConfigFileName() As String
        Dim strFile As String = Me.m_core.DataSource.ToString
        Return Path.Combine(Path.GetDirectoryName(strFile), Path.GetFileNameWithoutExtension(strFile) & "_fishmip.config")
    End Function

End Class
