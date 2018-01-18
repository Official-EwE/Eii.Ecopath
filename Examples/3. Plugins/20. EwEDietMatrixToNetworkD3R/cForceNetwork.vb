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
Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' <summary>
''' Helper class for generating a NetworkD3 forceNetwork graph
''' </summary>
Public Class cForceNetwork
    Inherits cNetwork

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Function Name() As String
        Return "ForceNetwork"
    End Function

    Public Overrides Function GenerateScript() As String

        If Not Me.Core.StateMonitor.HasEcopathRan Then Me.Core.RunEcoPath()

        Dim sb As New StringBuilder()
        Dim strLinks As String = "links_df"
        Dim strNodes As String = "nodes_df"

        sb.AppendLine(Me.ModelLine())
        sb.AppendLine()
        sb.AppendLine("library(networkD3)")
        sb.AppendLine()
        sb.AppendLine("# Links")
        sb.AppendLine(MakeLinks(strLinks))
        sb.AppendLine("# Nodes")
        If (UseSymbolicNames) Then sb.AppendLine(Me.SymbolicLine)
        sb.AppendLine(MakeNodes(strNodes))
        sb.AppendLine()
        sb.AppendLine("# Plot")
        sb.AppendLine("forceNetwork(Links = " & strLinks & ", Nodes = " & strNodes & ", Source = 'source',")
        sb.AppendLine("             Target = 'target', Value = 'value', NodeID = 'name', Nodesize = 'biomass',")
        sb.AppendLine("             Group = 'group', opacity = 1, zoom = T, legend = T, bounded = T)")

        Return sb.ToString()

    End Function

    Private Function MakeLinks(strVar As String) As String

        Dim lCol As String() = {"source", "target", "value"}
        Dim lSrc As New List(Of Integer)
        Dim lTgt As New List(Of Integer)
        Dim lDC As New List(Of Single)

        For iPred As Integer = 1 To Me.Core.nLivingGroups
            Dim pred As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.Core.nGroups
                If (pred.DietComp(iPrey)) > 0 Then
                    lSrc.Add(iPred - 1)
                    lTgt.Add(iPrey - 1)
                    lDC.Add(pred.DietComp(iPrey))
                End If
            Next
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine(ArrayLine("src", lSrc))
        sb.AppendLine(ArrayLine("tgt", lTgt))
        sb.AppendLine(ArrayLine("dc", lDC))
        sb.AppendLine(strVar & " <- data.frame(src, tgt, dc)")
        sb.AppendLine(ArrayLine("colnames(" & strVar & ")", lCol))
        Return sb.ToString()

    End Function

    Private Function MakeNodes(strVar As String) As String

        Dim lCol As String() = {"name", "group", "biomass"}
        Dim lNms As New List(Of String)
        Dim lGrp As New List(Of String)
        Dim lSz As New List(Of Double)
        Dim lSzLog As New List(Of Double)

        For iGroup As Integer = 1 To Me.Core.nGroups
            Dim grp As cEcoPathGroupOutput = Me.Core.EcoPathGroupOutputs(iGroup)
            If (UseSymbolicNames) Then
                lNms.Add(cStringUtils.ToExcelColumnName(iGroup))
            Else
                lNms.Add(ToRString(grp.Name))
            End If
            If grp.IsConsumer Then
                lGrp.Add(SharedResources.HEADER_CONSUMER)
            ElseIf grp.IsProducer Then
                lGrp.Add(SharedResources.HEADER_PRODUCER)
            Else
                lGrp.Add(SharedResources.HEADER_DETRITUS)
            End If
            lSz.Add(grp.Biomass)
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine(ArrayLine("name", lNms))
        sb.AppendLine(ArrayLine("group", lGrp))
        sb.AppendLine(ArrayLine("biomass", lSz))
        sb.AppendLine(strVar & " <- data.frame(name, group, biomass)")
        sb.Append(ArrayLine("colnames(" & strVar & ")", lCol))
        Return sb.ToString()

    End Function

End Class
