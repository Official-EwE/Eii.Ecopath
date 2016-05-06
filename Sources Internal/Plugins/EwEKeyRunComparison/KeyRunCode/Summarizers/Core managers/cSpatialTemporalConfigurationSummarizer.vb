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
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Text

#End Region ' Imports

Public Class cSpatialTemporalConfigurationSummarizer
    Implements IHashSummarizer

    Private m_core As cCore = Nothing
    Private m_man As cSpatialDataConnectionManager = Nothing

    Public Sub New(core As cCore)
        Me.m_core = core
        Me.m_man = core.SpatialDataConnectionManager
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcospaceExternalData"
    End Function

    Public Sub Init() _
        Implements IHashSummarizer.Init

    End Sub

    Public Function HashValues() As cHashValues() Implements IHashSummarizer.HashValues

        Dim sbSummary As New StringBuilder()
        Dim lstHashValues As New List(Of cHashValues)
        Dim adapters As cSpatialDataAdapter() = Me.m_man.Adapters
        Dim adapter As cSpatialDataAdapter = Nothing
        Dim connections As cSpatialDataConnection() = Nothing
        Dim connection As cSpatialDataConnection = Nothing

        For i As Integer = 0 To adapters.Length - 1
            adapter = Me.m_man.Adapters(i)
            For j As Integer = 1 To adapter.MaxLength - 1
                If adapter.IsEnabled(j) Then
                    connections = adapter.Connections(j)
                    For k As Integer = 0 To connections.Length - 1
                        connection = connections(k)
                        If (connection.IsConfigured) Then
                            If (sbSummary.Length > 0) Then sbSummary.Append(";")
                            sbSummary.Append(cStringConverters.ConnectionToString(connection))
                        End If
                    Next k
                End If
            Next j

            If (sbSummary.Length > 0) Then
                lstHashValues.Add(New cHashValues(Me.Name, adapter.VarName.ToString(), sbSummary.ToString))
                sbSummary.Clear()
            End If
        Next i

        Return lstHashValues.ToArray()

    End Function

End Class
