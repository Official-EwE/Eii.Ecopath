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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Public Class cDietCalculator
    Dim m_EcopathData As EwECore.cEcopathDataStructures
    Private m_Core As EwECore.cCore

    Public Sub New(EwECore As EwECore.cCore, EcopathData As EwECore.cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData
    End Sub

    Public Function UpdateDietsFromPreferences(DiestPreferences As cDietPreferences) As Boolean

        Return True
    End Function


End Class
