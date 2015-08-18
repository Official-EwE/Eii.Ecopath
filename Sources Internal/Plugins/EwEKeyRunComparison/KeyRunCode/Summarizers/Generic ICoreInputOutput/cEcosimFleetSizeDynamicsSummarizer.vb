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
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcosimFleetSizeDynamicsSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()
        MyBase.Init()

        For iFlt As Integer = 1 To Me.m_core.nFleets
            Me.m_objects.Add(Me.Core.EcosimFleetInputs(iFlt))
        Next

        Me.m_variables.Add(eVarNameFlags.EPower)
        Me.m_variables.Add(eVarNameFlags.PcapBase)
        Me.m_variables.Add(eVarNameFlags.CapDepreciate)
        Me.m_variables.Add(eVarNameFlags.CapBaseGrowth)

    End Sub

    Public Overrides Function HashValues() As cHashValues()
        If Me.Core.EcoSimModelParameters.PredictEffort Then
            Return MyBase.GetVarResults()
        Else
            'PredictEffort Turned off
            Dim results As New List(Of cHashValues)
            results.Add(New cHashValues(Me.Name, eVarNameFlags.PredictEffort, "False"))
            Return results.ToArray()
        End If
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcosimFleetSizeDynamics"
        End Get
    End Property

#End Region ' Internals

End Class
