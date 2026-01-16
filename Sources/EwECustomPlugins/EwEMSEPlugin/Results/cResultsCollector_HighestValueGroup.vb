' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright:
'    1991- Ecopath International Initiative, Barcelona, Spain
'
' Cefas MSE plug-in copyright: 
'    2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Imports EwEUtils.Utilities

Public Class cResultsCollector_HighestValueGroup
    Inherits cResultsCollector_1DArray

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Highest Value Group"
        End Get
    End Property

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return "NA"
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Fleet"
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return Me.m_MSE.Core.EcopathFleetInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return Me.m_MSE.Core.nFleets
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return Me.m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim StrategyIndex = Me.m_MSE.Strategies.IndexOf(Me.m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For iFleet = 1 To Me.m_MSE.Core.nFleets
            For iTime = 1 To Me.NumberOfTimeRecords
                Me.SetValue(StrategyIndex, iFleet, iTime) = Me.m_MSE.HighestValueGroup(iFleet, iTime)
            Next
        Next
    End Sub

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.ToCSVField(Me.GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "HighestValueGroup_"
        End Get
    End Property

End Class
