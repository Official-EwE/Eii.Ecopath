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

#End Region ' Imports

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
            Dim pred As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.Core.nGroups
                If pred.DietComp(iPrey) > 0 Then
                    Dim prey As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iPred)
                    If Me.UseSymbolicNames Then
                        lSrc.Add(cStringUtils.ToExcelColumnName(iPred))
                        lTgt.Add(cStringUtils.ToExcelColumnName(iPrey))
                    Else
                        lSrc.Add(ToRString(pred.Name))
                        lTgt.Add(ToRString(prey.Name))
                    End If
                End If
            Next
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine("library(networkD3)")
        sb.AppendLine()
        sb.AppendLine("# Diet network from EwE model " & Me.ToRString(Me.Core.EwEModel.Name))
        If (UseSymbolicNames) Then sb.AppendLine("# Group names have been replaced with symbolic names")
        sb.AppendLine(MakeArray(src, lSrc))
        sb.AppendLine(MakeArray(target, lTgt))
        sb.AppendLine()
        sb.AppendLine("networkData <- data.frame(" & src & ", " & target & ")")
        sb.AppendLine("# Plot")
        sb.AppendLine("simpleNetwork(networkData)")

        Return sb.ToString

    End Function

End Class
