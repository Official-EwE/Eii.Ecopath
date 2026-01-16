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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'



Imports EwECore
Imports EwEUtils.Utilities



Public Class cSFPGroupsWithTimeSeries
    Inherits cSFPGenericIteration

    Public Sub New(baseSearchMode As ISFPIteration.eBaseSearchMode)
        Me.BaseSearchMode = baseSearchMode
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        Dim bOK As Boolean = False
        If Not MyBase.Load(core) Then Return bOK

        'Enable specific time series for Baseline or Fishing
        If Me.EnableTimeSeries(core) Then
            'Reset vunerabilities
            If MyBase.ResetVs(core) And MyBase.ResetFF(core) Then
                'Run a sensitivity of SS to V search for baseline
                Dim man As cF2TSManager = core.EcosimFitToTimeSeries
                man.SetNBlocksForGroupsWithTimeSeries()
                Me.Report(cStringUtils.Localize(My.Resources.REPORT_V_FOR_GROUPS_WITH_TIMESERIES, man.nBlockCodes), eReportState.Success)
                bOK = True
            End If
        End If

        Return bOK

    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return cStringUtils.Localize(My.Resources.NAME_WITH_TS_ONLY, MyBase.Name)
        End Get
    End Property

    Public Overrides ReadOnly Property IsGroupsWithTimeSeriesOnly As Boolean
        Get
            Return True
        End Get
    End Property

End Class
