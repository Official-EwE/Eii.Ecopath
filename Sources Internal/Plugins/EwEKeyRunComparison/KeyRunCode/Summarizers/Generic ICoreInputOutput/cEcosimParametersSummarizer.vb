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
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcosimParametersSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        Me.m_objects.Add(Me.Core.EcoSimModelParameters)

        Me.m_variables.Add(eVarNameFlags.EcoSimNYears)
        Me.m_variables.Add(eVarNameFlags.NutBaseFreeProp)

        ' The forcing numbers are already checked in cEcosimEnvForcingSummarizer, including the shape of the attached functions
        'Me.m_variables.Add(eVarNameFlags.NutForceFunctionNumber)
        'Me.m_variables.Add(eVarNameFlags.SalinityForceFunctionNumber)
        'Me.m_variables.Add(eVarNameFlags.TemperatureForceFunctionNumber)

        Me.m_variables.Add(eVarNameFlags.PredictEffort)
        Me.m_variables.Add(eVarNameFlags.UseVarPQ)
        Me.m_variables.Add(eVarNameFlags.ForagingTimeLowerLimit)

    End Sub

    Public Overrides Function HashValues() As System.Collections.Generic.List(Of cHashValues)
        Return MyBase.getVarResults()
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcosimParameters"
        End Get
    End Property

#End Region ' Internals

End Class
