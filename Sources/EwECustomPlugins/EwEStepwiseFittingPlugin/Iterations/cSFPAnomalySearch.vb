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
Imports EwECore.FitToTimeSeries



Public Class cSFPAnomalySearch
    Inherits cSFPGenericIteration

    Public Sub New(baseSearchMode As ISFPIteration.eBaseSearchMode, sps As Integer)
        Me.BaseSearchMode = baseSearchMode
        Me.k = sps
        Me.SplinePoints = Me.k
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        If Not MyBase.Load(core) Then Return False

        'Enable specific time series for Baseline or Fishing
        If Not Me.EnableTimeSeries(core) Then Return False

        'Reset vunerabilities
        Return Me.ResetVs(core) And Me.ResetFF(core)

    End Function

    Public Overrides Function Run(core As cCore) As Boolean
        If Not Me.RunAnomalySearch(core) Then Return False
        Return MyBase.Run(core)
    End Function

End Class
