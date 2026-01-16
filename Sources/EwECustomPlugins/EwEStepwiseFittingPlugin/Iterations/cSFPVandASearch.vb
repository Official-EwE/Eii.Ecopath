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



Public Class cSFPVandASearch
    Inherits cSFPGenericIteration

    Public Sub New(BOrF As ISFPIteration.eBaseSearchMode, estimatedParameters As Integer, sps As Integer)
        Me.BaseSearchMode = BOrF
        Me.k = estimatedParameters + sps
        Me.EstimatedV = estimatedParameters
        Me.SplinePoints = sps
    End Sub

    Public Overrides Function Load(core As cCore) As Boolean

        Dim bOK As Boolean = False
        If Not MyBase.Load(core) Then Return bOK

        'Enable specific time series for Baseline or Fishing
        If Me.EnableTimeSeries(core) Then
            'Reset vunerabilities
            If Me.ResetVs(core) And Me.ResetFF(core) Then
                'Run a sensitivity of SS to V search for baseline
                If Me.RunSensitivityOfSSToV(core) Then
                    bOK = True
                End If
            End If
        End If

        Return bOK

    End Function

    Public Overrides Function Run(core As cCore) As Boolean
        If Me.RunVandASearch(core) Then
            Return MyBase.Run(core)
        End If
        Return False
    End Function

End Class
