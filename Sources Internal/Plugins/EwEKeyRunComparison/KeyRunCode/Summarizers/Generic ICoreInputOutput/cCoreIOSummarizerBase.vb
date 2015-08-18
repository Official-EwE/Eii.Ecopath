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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public MustInherit Class cCoreIOSummarizerBase
    Implements IHashSummarizer

#Region " Protected variables "

    Protected m_objects As List(Of ICoreInputOutput) = Nothing
    Protected m_variables As List(Of eVarNameFlags) = Nothing
    Protected m_core As cCore = Nothing

#End Region ' Protected variables

#Region " Pure virtual methods "

    ''' <summary>
    ''' Initialize this instance for use. This base implementation initializes the worker arrays.
    ''' </summary>
    Public Overridable Sub Init() _
        Implements IHashSummarizer.Init

        Me.m_objects = New List(Of ICoreInputOutput)
        Me.m_variables = New List(Of eVarNameFlags)

    End Sub

    Public MustOverride Function HashValues() As cHashValues() _
        Implements IHashSummarizer.HashValues

    Protected MustOverride ReadOnly Property ObjectDescriptor As String

    Public Function Name() As String _
        Implements IHashSummarizer.Name
        Return Me.ObjectDescriptor
    End Function

#End Region ' Pure virtual methods

#Region " Construction "

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Construction

#Region " Base class properties and methods "

    Protected ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

    ''' <summary>
    ''' Get variable values for a non-indexed variable.
    ''' </summary>
    Protected Function GetVarResults() As cHashValues()

        Dim sb As New StringBuilder()
        Dim lstResults As New List(Of cHashValues)

        ' Don't use for..each because item order cannot be guaranteed

        For i As Integer = 0 To m_variables.Count - 1
            Dim var As eVarNameFlags = m_variables(i)
            For j As Integer = 0 To m_objects.Count - 1
                Dim obj As ICoreInputOutput = Me.m_objects(j)
                Try
                    Dim value As Object = obj.GetVariable(var)
                    If (j > 0) Then sb.Append("|")
                    sb.Append(cStringUtils.ToCSVField(value))
                Catch ex As Exception
                    Debug.Assert(False, Me.ToString() & ".HashString() Failed to find variable for Core object " & obj.ToString() & " variable " & var.ToString())
                End Try
            Next
            lstResults.Add(New cHashValues(Me.ObjectDescriptor, var, sb.ToString))
            sb.Clear()
        Next

        Return lstResults.ToArray()

    End Function

    ''' <summary>
    ''' Get variable values for a variable with one dimension.
    ''' </summary>
    ''' <param name="n">'Core counter'</param>
    Protected Function GetVarResults(ByVal n As Integer) As cHashValues()

        Dim sb As New StringBuilder()
        Dim lstResults As New List(Of cHashValues)

        ' Don't use for..each because item order cannot be guaranteed

        For i As Integer = 0 To m_variables.Count - 1
            Dim var As eVarNameFlags = m_variables(i)
            For j As Integer = 0 To m_objects.Count - 1
                Dim obj As ICoreInputOutput = Me.m_objects(j)
                Try
                    For k As Integer = 1 To n
                        Dim value As Object = obj.GetVariable(var, k)
                        If (k > 1) Then sb.Append(",")
                        sb.Append(cStringUtils.ToCSVField(value))
                    Next
                Catch ex As Exception
                    Debug.Assert(False, Me.ToString() & ".HashString() Failed to find variable for Core object " & obj.ToString() & " variable " & var.ToString())
                End Try
                sb.Append("|")
            Next
            lstResults.Add(New cHashValues(Me.Name, var, sb.ToString))
            sb.Clear()
        Next

        Return lstResults.ToArray()

    End Function

#End Region ' Base class properties and methods

End Class
