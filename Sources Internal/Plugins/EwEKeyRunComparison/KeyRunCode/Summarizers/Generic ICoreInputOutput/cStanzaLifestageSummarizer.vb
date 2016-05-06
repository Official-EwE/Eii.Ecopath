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
Imports EwECore
Imports EwEUtils.Core
Imports System.Text
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cStanzaLifestageSummarizer
    Implements IHashSummarizer

    Private m_core As cCore
    Private m_variables As List(Of eVarNameFlags)

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "StanzaLifeStages"
    End Function

    Public Sub Init() _
        Implements IHashSummarizer.Init
        Me.m_variables = New List(Of eVarNameFlags)
    End Sub

    Public Function HashValues() As cHashValues() _
        Implements IHashSummarizer.HashValues

        Dim sb As New StringBuilder()
        Dim lResults As New List(Of cHashValues)

        Me.m_variables.Add(eVarNameFlags.Bat)
        Me.m_variables.Add(eVarNameFlags.StartAge)
        Me.m_variables.Add(eVarNameFlags.StanzaNumberAtAge)
        Me.m_variables.Add(eVarNameFlags.StanzaWeightAtAge)
        Me.m_variables.Add(eVarNameFlags.StanzaBiomassAtAge)
        Me.m_variables.Add(eVarNameFlags.StanzaBiomass)
        Me.m_variables.Add(eVarNameFlags.StanzaCB)
        Me.m_variables.Add(eVarNameFlags.StanzaMortaility)

        For i As Integer = 0 To Me.m_variables.Count - 1
            Dim var As eVarNameFlags = Me.m_variables(i)
            For j As Integer = 0 To Me.m_core.nStanzas - 1
                Dim sg As cStanzaGroup = Me.m_core.StanzaGroups(j)
                For k As Integer = 0 To sg.nLifeStages - 1
                    Try
                        Dim value As Object = sg.GetVariable(var, k)
                        If (j > 0) Then sb.Append("|")
                        sb.Append(cStringConverters.FormatNumber(value))
                    Catch ex As Exception
                        Debug.Assert(False, Me.ToString() & ".HashString() Failed to find variable for Core object " & sg.ToString() & " variable " & var.ToString())
                    End Try
                Next k ' Lifestage
            Next j ' Stanza
            lResults.Add(New cHashValues(Me.Name, var, sb.ToString))
            sb.Clear()
        Next ' Variable

        Return lResults.ToArray()

    End Function

End Class
