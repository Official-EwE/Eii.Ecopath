' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities



Public Class cSimpleNetwork
    Inherits cNetwork

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Function Name() As String
        Return "SimpleNetwork"
    End Function

    Public Overrides Function GenerateScript() As String

        Dim lSrc As New List(Of String)
        Dim src As String = "predators"
        Dim lTgt As New List(Of String)
        Dim target As String = "preys"

        For iPred As Integer = 1 To Me.Core.nLivingGroups
            Dim pred As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.Core.nGroups
                If pred.DietComp(iPrey) > 0 Then
                    Dim prey As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(iPred)
                    If My.Settings.UseSymbolicNames Then
                        lSrc.Add(cStringUtils.ToExcelColumnName(iPred))
                        lTgt.Add(cStringUtils.ToExcelColumnName(iPrey))
                    Else
                        lSrc.Add(Me.ToRString(pred.Name))
                        lTgt.Add(Me.ToRString(prey.Name))
                    End If
                End If
            Next
        Next

        Dim sb As New StringBuilder()

        sb.AppendLine(Me.HeaderLine())
        sb.AppendLine()
        sb.AppendLine("library(networkD3)")
        sb.AppendLine()
        sb.AppendLine(Me.ArrayLine(src, lSrc))
        sb.AppendLine(Me.ArrayLine(target, lTgt))
        sb.AppendLine()
        sb.AppendLine("networkData <- data.frame(" & src & ", " & target & ")")
        sb.AppendLine("# Plot")
        sb.AppendLine("simpleNetwork(networkData)")

        Return sb.ToString

    End Function

End Class
