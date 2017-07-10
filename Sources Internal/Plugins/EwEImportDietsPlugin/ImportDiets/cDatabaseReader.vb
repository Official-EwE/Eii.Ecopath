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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Ecopath
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

Public Class cDatabaseReader

    Private m_EcopathData As cEcopathDataStructures
    Private m_Core As cCore

    Public Sub New(EwECore As cCore, EcopathData As cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData
    End Sub

    Public Function ImportDietPreferences(ModelFileName As String, ByRef DietPrefenences As cDietPreferences) As Boolean
        'read diets from external database

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Temp for debugging
        'Really just pass out the existing diets for now
        DietPrefenences = New cDietPreferences(Me.m_EcopathData)
        Return True
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


    End Function


End Class
