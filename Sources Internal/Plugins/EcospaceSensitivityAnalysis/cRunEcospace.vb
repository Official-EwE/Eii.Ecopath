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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwECore.Ecospace

#End Region

Public Class cRunEcospace

    Private Ecospace As cEcoSpace

    Private Core As cCore
    Public nTimeStepPerYear As Integer
    Public StartOfLastYear As Integer

    Public Sub Init(ByVal theCore As cCore, ByVal EcospaceModel As cEcoSpace)

        Me.Core = theCore
        Me.Ecospace = EcospaceModel

    End Sub

    Public Sub SetRunParameters(ByVal parameters As cRunPeriods)

        '  Core.EwEModel.FirstYear = parameters.StartYear
        Core.EcospaceModelParameters.TotalTime = parameters.nYears

        nTimeStepPerYear = CInt(Core.EcospaceModelParameters.NumberOfTimeStepsPerYear)
        StartOfLastYear = CInt((Core.EcospaceModelParameters.TotalTime - 1) * nTimeStepPerYear)

    End Sub


    Public Sub Run()
        Core.StopEcospace()
        Core.RunEcoSpace(Nothing, False)

    End Sub

End Class
