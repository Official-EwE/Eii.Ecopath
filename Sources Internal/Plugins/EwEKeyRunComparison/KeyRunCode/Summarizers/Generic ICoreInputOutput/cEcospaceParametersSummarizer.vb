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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceParametersSummarizer
    Inherits cCoreIOSummarizerBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        Me.m_objects.Add(Me.Core.EcospaceModelParameters)

        Me.m_variables.Add(eVarNameFlags.UseIBM)
        Me.m_variables.Add(eVarNameFlags.UseNewMultiStanza)
        Me.m_variables.Add(eVarNameFlags.AdjustSpace)
        Me.m_variables.Add(eVarNameFlags.PredictEffort)
        Me.m_variables.Add(eVarNameFlags.ConSimOnEcoSpace)
        Me.m_variables.Add(eVarNameFlags.PacketsMultiplier)
        Me.m_variables.Add(eVarNameFlags.TotalTime)
        Me.m_variables.Add(eVarNameFlags.NumTimeStepsPerYear)
        Me.m_variables.Add(eVarNameFlags.Tolerance)
        Me.m_variables.Add(eVarNameFlags.SOR)
        Me.m_variables.Add(eVarNameFlags.MaxIterations)
        Me.m_variables.Add(eVarNameFlags.UseExact)
        Me.m_variables.Add(eVarNameFlags.EcospaceIBMMovePacketOnStanza)

    End Sub

    Public Overrides Function HashValues() As cHashValues()
        Return MyBase.GetVarResults()
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcospaceParameters"
        End Get
    End Property

#End Region ' Internals

End Class
