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

Public Class cEcopathInputSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        For igrp As Integer = 1 To Me.m_core.nGroups
            Me.m_objects.Add(Me.Core.EcoPathGroupInputs(igrp))
        Next

        Me.m_variables.Add(eVarNameFlags.Area)
        Me.m_variables.Add(eVarNameFlags.BiomassAreaInput)
        Me.m_variables.Add(eVarNameFlags.PBInput)
        Me.m_variables.Add(eVarNameFlags.QBInput)
        Me.m_variables.Add(eVarNameFlags.EEInput)
        Me.m_variables.Add(eVarNameFlags.GEInput)
        Me.m_variables.Add(eVarNameFlags.GS)

        Me.m_variables.Add(eVarNameFlags.BioAccum)
        Me.m_variables.Add(eVarNameFlags.Immig)
        Me.m_variables.Add(eVarNameFlags.Emig)
        Me.m_variables.Add(eVarNameFlags.BioAccumRate)
        Me.m_variables.Add(eVarNameFlags.EmigRate)
        Me.m_variables.Add(eVarNameFlags.OtherMortInput)
        Me.m_variables.Add(eVarNameFlags.DetImp)

        Me.m_variables.Add(eVarNameFlags.NonMarketValue)

    End Sub

    Public Overrides Function HashValues() As cHashValues()
        Return MyBase.GetVarResults()
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcopathBasicInputs"
        End Get
    End Property

#End Region ' Internals

End Class
