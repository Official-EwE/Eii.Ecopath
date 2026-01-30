' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



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

        If Not Me.Core.StateMonitor.HasEcopathRan Then Me.Core.RunEcopath()

        Dim sb As New StringBuilder()
        Dim strLinks As String = "links_df"
        Dim strNodes As String = "nodes_df"

        sb.AppendLine(Me.HeaderLine())
        sb.AppendLine()
        sb.AppendLine("library(networkD3)")
        sb.AppendLine()
        sb.AppendLine("# Links")
        sb.AppendLine(Me.MakeLinks(strLinks))
        sb.AppendLine("# Nodes")
        sb.AppendLine(Me.MakeNodes(strNodes))
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
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.Core.nGroups
                If (pred.DietComp(iPrey)) > 0 Then
                    lSrc.Add(iPred - 1)
                    lTgt.Add(iPrey - 1)
                    lDC.Add(pred.DietComp(iPrey))
                End If
            Next
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine(Me.ArrayLine("src", lSrc))
        sb.AppendLine(Me.ArrayLine("tgt", lTgt))
        sb.AppendLine(Me.ArrayLine("dc", lDC))
        sb.AppendLine(strVar & " <- data.frame(src, tgt, dc)")
        sb.AppendLine(Me.ArrayLine("colnames(" & strVar & ")", lCol))
        Return sb.ToString()

    End Function

    Private Function MakeNodes(strVar As String) As String

        Dim lCol As String() = {"name", "group", "biomass"}
        Dim lNms As New List(Of String)
        Dim lGrp As New List(Of String)
        Dim lSz As New List(Of Double)
        Dim lSzLog As New List(Of Double)

        For iGroup As Integer = 1 To Me.Core.nGroups
            Dim grp As cEcopathGroupOutput = Me.Core.EcopathGroupOutputs(iGroup)
            If (My.Settings.UseSymbolicNames) Then
                lNms.Add(cStringUtils.ToExcelColumnName(iGroup))
            Else
                lNms.Add(Me.ToRString(grp.Name))
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
        sb.AppendLine(Me.ArrayLine("name", lNms))
        sb.AppendLine(Me.ArrayLine("group", lGrp))
        sb.AppendLine(Me.ArrayLine("biomass", lSz))
        sb.AppendLine(strVar & " <- data.frame(name, group, biomass)")
        sb.Append(Me.ArrayLine("colnames(" & strVar & ")", lCol))
        Return sb.ToString()

    End Function

End Class
