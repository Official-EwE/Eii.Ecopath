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

Public Class cEcosimInputSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)

    End Sub

    Public Overrides Function HashValues() As cHashValues()
        Return MyBase.GetVarResults()
    End Function

    Public Overrides Sub Init()

        MyBase.Init()

        For igrp As Integer = 1 To Me.m_core.nGroups
            Me.m_objects.Add(Me.Core.EcoSimGroupInputs(igrp))
        Next

        Me.m_variables.Add(eVarNameFlags.MaxRelFeedingTime)
        Me.m_variables.Add(eVarNameFlags.FeedingTimeAdjRate)
        Me.m_variables.Add(eVarNameFlags.OtherMortFeedingTime)
        Me.m_variables.Add(eVarNameFlags.PredEffectFeedingTime)
        Me.m_variables.Add(eVarNameFlags.DenDepCatchability)
        Me.m_variables.Add(eVarNameFlags.QBMaxQBio)
        Me.m_variables.Add(eVarNameFlags.SwitchingPower)

        Me.m_variables.Add(eVarNameFlags.SalinityOpt)
        Me.m_variables.Add(eVarNameFlags.SalinitySpreadLeft)

        Me.m_variables.Add(eVarNameFlags.SalinitySpreadRight)

        Me.m_variables.Add(eVarNameFlags.TemperatureOpt)
        Me.m_variables.Add(eVarNameFlags.TemperatureSpreadLeft)
        Me.m_variables.Add(eVarNameFlags.TemperatureSpreadRight)

    End Sub

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcosimGroupInfo"
        End Get
    End Property

#End Region ' Internals

End Class
