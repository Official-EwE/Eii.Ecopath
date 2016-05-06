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

#End Region ' Imports

Public Class cEcosimEnvForcingSummarizer
    Implements IHashSummarizer

    Private m_core As cCore
    Private m_vars As List(Of eVarNameFlags)

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcosimEnvironmentalForcing"
    End Function

    Public Sub Init() Implements IHashSummarizer.Init

        Me.m_vars = New List(Of eVarNameFlags)

        Me.m_vars.Add(eVarNameFlags.NutForceFunctionNumber)
        Me.m_vars.Add(eVarNameFlags.TemperatureForceFunctionNumber)
        Me.m_vars.Add(eVarNameFlags.SalinityForceFunctionNumber)

    End Sub

    Public Function HashValues() As cHashValues() _
        Implements IHashSummarizer.HashValues

        Dim man As cForcingFunctionManager = Me.m_core.ForcingShapeManager
        Dim parms As cEcoSimModelParameters = Me.m_core.EcoSimModelParameters
        Dim iShape As Integer
        Dim shape As cForcingFunction = Nothing
        Dim sbSummary As New Text.StringBuilder()

        Dim lHashValues As New List(Of cHashValues)

        ' Do not use for-each
        For i As Integer = 0 To Me.m_vars.Count - 1
            iShape = CInt(parms.GetVariable(Me.m_vars(i)))
            If (sbSummary.Length > 0) Then sbSummary.Append(",")
            If (iShape > 0) Then
                shape = man.Item(i)
                sbSummary.Append(cStringConverters.ShapeToString(shape))
            End If
        Next
        lHashValues.Add(New cHashValues(Me.Name, "EnvForcing", sbSummary.ToString))
        Return lHashValues.ToArray()

    End Function

End Class