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

#End Region ' Imports

Public Class cResilienceData

    Public Sub Resize(nTimes As Integer, nYears As Integer)
        ReDim SupplyAtT(nTimes)
        ReDim SupplyAtY(nYears)
        ReDim DemandAtT(nTimes)
        ReDim DemandAtY(nYears)
    End Sub

    Public Property SupplyAtT As Double()
    Public Property SupplyAtY As Double()
    Public Property DemandAtT As Double()
    Public Property DemandAtY As Double()

    Public Function SaveToCSV(strFile As String, bAnnual As Boolean) As Boolean
        Return True
    End Function

End Class
