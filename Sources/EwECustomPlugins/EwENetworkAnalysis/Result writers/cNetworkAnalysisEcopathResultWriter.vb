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
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write network analysis results to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cNetworkAnalysisEcopathResultWriter
    Inherits cNetworkAnalysisResultWriter

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Shazaam
    ''' </summary>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(manager As cNetworkManager)
        MyBase.New(manager)
    End Sub

    Public Overrides Function WriteResults(strPath As String) As Boolean

        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return False

        If Not Me.Manager.IsMainNetworkRun Then
            If Not Me.Manager.RunMainNetwork() Then
                Return False
            End If
        End If

        ' ToDo: write other ENA indicators to file when requested

        Return Me.WriteFile(Me.GetMTIFileName(strPath), Me.GetMTIData())

    End Function

#Region " Internals "

    Private Function GetNAIndicatorsFileName(strPath As String, bWithPPR As Boolean, bAnnual As Boolean) As String
        Dim strFile As String = "NA_" &
                                If(bAnnual, My.Resources.HEADER_ANNUAL, My.Resources.HEADER_MONTHLY) & "_" &
                                If(bWithPPR, "IndicesPPR", "IndicesWithoutPPR") &
                                ".csv"
        Return Path.Combine(strPath, strFile)
    End Function

    Private Function GetMTIFileName(strPath As String) As String
        Return Path.Combine(strPath, "NA_MTI.csv")
    End Function

    Private Function GetMTIData() As String

        Dim sb As New StringBuilder()
        Dim core As cCore = Me.Manager.Core

        ' Header line
        For iGroup As Integer = 1 To core.nGroups
            sb.Append(",")
            sb.Append(cStringUtils.ToCSVField(core.EcopathGroupInputs(iGroup).Name))
        Next
        For iFleet As Integer = 1 To core.nFleets
            sb.Append(",")
            sb.Append(cStringUtils.ToCSVField(core.EcopathFleetInputs(iFleet).Name))
        Next
        sb.AppendLine("")

        For i As Integer = 1 To core.nGroups + core.nFleets
            If i <= core.nGroups Then
                sb.Append(cStringUtils.ToCSVField(core.EcopathGroupInputs(i).Name))
            Else
                sb.Append(cStringUtils.ToCSVField(core.EcopathFleetInputs(i - core.nGroups).Name))
            End If
            For j As Integer = 1 To core.nGroups + core.nFleets
                sb.Append(",")
                sb.Append(cStringUtils.ToCSVField(Me.Manager.MixedTrophicImpacts(i, j)))
            Next
            sb.AppendLine()
        Next
        Return sb.ToString

    End Function

#End Region ' Internals

End Class
