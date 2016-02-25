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

Public Class cEcospaceDispersalSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        For iGrp As Integer = 1 To Me.m_core.nGroups
            Me.m_objects.Add(Me.Core.EcospaceGroups(iGrp))
        Next

        Me.m_variables.Add(eVarNameFlags.MVel)
        Me.m_variables.Add(eVarNameFlags.RelMoveBad)
        Me.m_variables.Add(eVarNameFlags.RelVulBad)
        Me.m_variables.Add(eVarNameFlags.EatEffBad)
        Me.m_variables.Add(eVarNameFlags.IsAdvected)
        Me.m_variables.Add(eVarNameFlags.IsMigratory)
        'Me.m_variables.Add(eVarNameFlags.MigrationConcRow)
        'Me.m_variables.Add(eVarNameFlags.MigrationConcCol)
        Me.m_variables.Add(eVarNameFlags.BarrierAvoidanceWeight)

    End Sub

    Public Overrides Function HashValues() As cHashValues()
        Return MyBase.GetVarResults(Me.m_core.nGroups)
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcospaceDispersal"
        End Get
    End Property

#End Region ' Internals

End Class
