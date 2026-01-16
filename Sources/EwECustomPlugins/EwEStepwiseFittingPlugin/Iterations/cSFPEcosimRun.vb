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



Public Class cSFPEcosimRun
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
                If MyBase.RunSensitivityOfSSToV(core) Then
                    bOK = True
                End If
            End If
        End If

        Return bOK

    End Function

End Class
